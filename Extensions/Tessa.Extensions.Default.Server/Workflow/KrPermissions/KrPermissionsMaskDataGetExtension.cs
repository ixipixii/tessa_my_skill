using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Cards.Metadata;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrPermissionsMaskDataGetExtension : CardGetExtension
    {
        #region Nested Types

        /// <summary>
        /// Заменяет данные на значение для колонок по умолчанию
        /// </summary>
        private class DefaultMaskGenerator : IKrPermissionsMaskGenerator
        {
            public object GenerateMaskValue(
                Card card,
                CardSection section,
                CardRow row,
                CardMetadataColumn columnMeta,
                object originalValue,
                string defaultMask)
            {
                return columnMeta.DefaultValue;
            }
        }

        #endregion

        #region Fields

        private readonly ICardCache cache;
        
        private readonly ICardMetadata cardMetadata;

        private readonly IKrPermissionsMaskGenerator maskGenerator;

        private readonly IKrPermissionsMaskGenerator defaultMaskGenerator;

        #endregion

        #region Constructors

        public KrPermissionsMaskDataGetExtension(
            ICardCache cache,
            ICardMetadata cardMetadata,
            IKrPermissionsMaskGenerator maskGenerator)
        {
            this.cache = cache;
            this.cardMetadata = cardMetadata;
            this.maskGenerator = maskGenerator;

            this.defaultMaskGenerator = new DefaultMaskGenerator();
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            object resultObj = null;

            if (!context.RequestIsSuccessful
                || context.CardType == null
                || !KrComponentsHelper.HasBase(context.Response.Card.TypeID, this.cache)
                || (card = context.Response.TryGetCard()) == null
                || !context.Info.TryGetValue(nameof(KrPermissionsNewGetExtension), out resultObj)
                || !(resultObj is IKrPermissionsManagerResult result))
            {
                return;
            }
            
            if (context.Method == CardGetMethod.Default)
            {
                await MaskCardDataAsync(
                    card,
                    result,
                    this.maskGenerator,
                    context.CancellationToken);
            }
            else if (context.Method == CardGetMethod.Export
                && !context.Session.User.IsAdministrator())
            {
                await MaskCardDataAsync(
                    card,
                    result,
                    this.defaultMaskGenerator,
                    context.CancellationToken);
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Метод для маскировки данных карточки по результату расчета прав доступа
        /// </summary>
        /// <param name="card">Карточка</param>
        /// <param name="permissionsResult">Результат расчета прав доступа</param>
        /// <param name="cancellationToken">Токен отмены асинхронной задачи.</param>
        /// <returns>Возвращает асинхронную задачу.</returns>
        private async Task MaskCardDataAsync(
            Card card,
            IKrPermissionsManagerResult permissionsResult,
            IKrPermissionsMaskGenerator maskGenerator,
            CancellationToken cancellationToken = default)
        {
            var cardTypeMeta = await cardMetadata.GetMetadataForTypeAsync(card.TypeID, cancellationToken);
            var cardTypeMetaSections = await cardTypeMeta.GetSectionsAsync(cancellationToken);
            foreach (var sectionSettings in permissionsResult.ExtendedCardSettings)
            {
                if ((sectionSettings.IsMasked
                        || sectionSettings.MaskedFields.Count > 0)
                    && cardTypeMetaSections.TryGetValue(sectionSettings.ID, out var sectionMeta))
                {
                    if (sectionSettings.IsMasked)
                    {
                        if (sectionMeta.SectionType == CardSectionType.Table)
                        {
                            card.Sections.GetOrAddTable(sectionMeta.Name).Rows.Clear();
                        }
                        else
                        {
                            var section = card.Sections.GetOrAddEntry(sectionMeta.Name);
                            foreach (var columnMeta in sectionMeta.Columns)
                            {
                                if (columnMeta.ColumnType == CardMetadataColumnType.Physical)
                                {
                                    section.RawFields[columnMeta.Name] = maskGenerator.GenerateMaskValue(
                                        card,
                                        section,
                                        null,
                                        columnMeta,
                                        section.RawFields.TryGet<object>(columnMeta.Name),
                                        sectionSettings.Mask);
                                }
                            }
                        }
                    }
                    else if (sectionSettings.MaskedFields.Count > 0)
                    {
                        var section = sectionMeta.SectionType == CardSectionType.Table
                            ? card.Sections.GetOrAddTable(sectionMeta.Name)
                            : card.Sections.GetOrAddEntry(sectionMeta.Name);

                        var replacer = sectionMeta.SectionType == CardSectionType.Table
                            ? new Action<CardMetadataColumn, string>((meta, defaultString) =>
                            {
                                foreach (var row in section.Rows)
                                {
                                    row[meta.Name] =
                                        maskGenerator.GenerateMaskValue(
                                            card,
                                            section,
                                            row,
                                            meta,
                                            row.TryGet<object>(meta.Name),
                                            defaultString);
                                }
                            })
                            : new Action<CardMetadataColumn, string>((meta, defaultString)
                                =>
                                    section.RawFields[meta.Name] =
                                        maskGenerator.GenerateMaskValue(
                                            card,
                                            section,
                                            null,
                                            meta,
                                            section.RawFields.TryGet<object>(meta.Name),
                                            defaultString));

                        foreach (var field in sectionSettings.MaskedFields)
                        {
                            sectionSettings.MaskedFieldsData.TryGetValue(field, out var defaultValue);
                            MaskSectionField(sectionMeta, field, replacer, defaultValue);
                        }
                    }
                }
            }
        }

        private void MaskSectionField(
            CardMetadataSection sectionMeta,
            Guid field,
            Action<CardMetadataColumn, string> replacer,
            string defaultValue)
        {
            if (sectionMeta.Columns.TryGetValue(field, out var columnMeta))
            {
                if (columnMeta.ColumnType == CardMetadataColumnType.Complex)
                {
                    foreach (var refColumnMeta in sectionMeta.Columns
                        .Where(x => x.ColumnType == CardMetadataColumnType.Physical
                            && x.ComplexColumnIndex == columnMeta.ComplexColumnIndex))
                    {
                        replacer(refColumnMeta, defaultValue);
                    }
                }
                else
                {
                    replacer(columnMeta, defaultValue);
                }
            }
        }

        #endregion
    }
}
