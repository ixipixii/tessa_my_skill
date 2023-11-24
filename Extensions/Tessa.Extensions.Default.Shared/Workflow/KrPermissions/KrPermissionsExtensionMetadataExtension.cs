using System;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Shared.Workflow.KrPermissions
{
    public sealed class KrPermissionsExtensionMetadataExtension :
        CardTypeMetadataExtension
    {
        #region Constructors

        public KrPermissionsExtensionMetadataExtension(IDbScope dbScope)
        {
            //конструктор для сервера
            this.dbScope = dbScope;
        }

        public KrPermissionsExtensionMetadataExtension(ICardMetadata clientCardMetadata, ICardCache cache) : base(clientCardMetadata)
        {
            //конструктор для клиента
            this.cache = cache;
        }

        #endregion

        #region Fields

        private readonly IDbScope dbScope;

        private readonly ICardCache cache;

        #endregion

        #region Private Methods

        private static void CloneBlocks(CardType source, CardType target)
        {
            CardTypeBlock marker = target.Blocks.FirstOrDefault(x => x.Name == "ExtensionMarker");

            int order = 0;
            if (marker != null)
            {
                order = marker.Order + 1;
            }

            foreach (var block in target.Blocks)
            {
                if (block.Order >= order)
                {
                    block.Order += source.Blocks.Count;
                }
            }

            foreach (var block in source.Blocks)
            {
                CardTypeBlock clone = block.DeepClone();
                clone.Order = order++;

                target.Blocks.Add(clone);
            }
        }

        #endregion

        #region Base Overrides

        public override async Task ModifyTypes(ICardMetadataExtensionContext context)
        {
            await AddFlagsAsync(context);
            await AddExtensionsAsync(context);
        }

        #endregion

        #region Private Methods

        private async Task AddFlagsAsync(ICardMetadataExtensionContext context)
        {
            CardType permissionsType = await TryGetCardTypeAsync(context, DefaultCardTypes.KrPermissionsTypeID, false).ConfigureAwait(false);
            if (permissionsType == null)
            {
                return;
            }

            CardTypeBlock flagsBlock = permissionsType.Blocks.FirstOrDefault(x => x.Name == "Flags");
            SchemeTable permissionsSection = context.SchemeService.GetTable("KrPermissions");

            if (flagsBlock != null
                && permissionsType.SchemeItems.FirstOrDefault(x => x.SectionID == permissionsSection.ID) is CardTypeSchemeItem permissionsSchemeItem)
            {
                // Для каждого флага добавляем его контрол в блок и добавляем флаг в секцию, если он еще не был добавлен
                foreach(var flag in KrPermissionFlagDescriptors.Full.IncludedPermissions.OrderBy(x => x.Order))
                {
                    if (flag.IsVirtual
                        || !(permissionsSection.Columns.GetColumn(flag.SqlName) is SchemeColumn column))
                    {
                        continue;
                    }

                    var columnID = column.ID;
                    if (!permissionsSchemeItem.ColumnIDList.Contains(columnID))
                    {
                        permissionsSchemeItem.ColumnIDList.Add(columnID);
                    }

                    flagsBlock.Controls.Add(
                        new CardTypeEntryControl()
                        {
                            Caption = flag.ControlCaption,
                            Name = flag.Name,
                            Order = flag.Order,
                            PhysicalColumnIDList = new Tessa.Platform.Collections.SealableList<Guid>() { columnID },
                            SectionID = permissionsSection.ID,
                            ToolTip = flag.ControlTooltip,
                            Type = CardControlTypes.Boolean,
                        });
                }
            }
        }

        private async Task AddExtensionsAsync(ICardMetadataExtensionContext context)
        {
            Guid? extensionTypeID;
            if (this.ClientMode)
            {
                if (context.CardTypes.All(x => x.ID != DefaultCardTypes.KrPermissionsTypeID))
                {
                    // на клиенте просматривают тип карточки с расширениями, но не KrPermissions
                    return;
                }

                extensionTypeID = (await this.cache.Cards.GetAsync("KrSettings", context.CancellationToken).ConfigureAwait(false))
                    .Sections["KrSettings"].Fields.Get<Guid?>("PermissionsExtensionTypeID");
            }
            else
            {
                await using (dbScope.Create())
                {
                    var db = dbScope.Db;
                    var builder = dbScope.BuilderFactory
                        .Select().Top(1).C("PermissionsExtensionTypeID")
                        .From("KrSettings").NoLock()
                        .Limit(1);

                    extensionTypeID = await db
                        .SetCommand(builder.Build())
                        .LogCommand()
                        .ExecuteAsync<Guid?>(context.CancellationToken).ConfigureAwait(false);
                }
            }

            if (!extensionTypeID.HasValue)
            {
                // нечего расширять
                return;
            }

            CardType extensionSource = await TryGetCardTypeAsync(context, extensionTypeID.Value).ConfigureAwait(false);
            CardType permissionsTarget = await TryGetCardTypeAsync(context, DefaultCardTypes.KrPermissionsTypeID).ConfigureAwait(false);
            StorageHelper.Merge(extensionSource.FormSettings, permissionsTarget.FormSettings);

            extensionSource.SchemeItems.CopyToTheBeginningOf(permissionsTarget.SchemeItems);
            extensionSource.Forms.CopyToTheBeginningOf(permissionsTarget.Forms);
            extensionSource.Validators.CopyToTheBeginningOf(permissionsTarget.Validators);
            extensionSource.Extensions.CopyToTheBeginningOf(permissionsTarget.Extensions);

            CloneBlocks(extensionSource, permissionsTarget);
        }

        #endregion
    }
}
