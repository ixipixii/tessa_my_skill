using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Forums;
using Tessa.Platform.Data;
using Tessa.Platform.Licensing;

namespace Tessa.Extensions.Default.Shared.Settings
{
    /// <summary>
    /// Добавляет вкладку "обсуждение" в карточку
    /// </summary>
    public sealed class InjectForumCardMetadataExtension : CardTypeMetadataExtension
    {
        #region Fields

        private readonly IDbScope dbScope;
        private readonly ILicenseManager licenseManager;

        #endregion

        #region Constructors

        public InjectForumCardMetadataExtension(IDbScope dbScope, ILicenseManager licenseManager)
            : base()
        {
            this.licenseManager = licenseManager;
            this.dbScope = dbScope;
        }

        #endregion

        #region Private Methods

        private async Task<List<Guid>> GetCardTypeIDsAsync(CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                return await db
                    .SetCommand(
                        this.dbScope.BuilderFactory
                            .Select()
                            .C(KrConstants.KrSettingsCardTypes.CardTypeID)
                            .From(KrConstants.KrSettingsCardTypes.Name).NoLock()
                            .Where().C(KrConstants.KrSettingsCardTypes.CardTypeID).NotEquals().P("krCardTypeID")
                            .Build(),
                        db.Parameter("krCardTypeID", DefaultCardTypes.KrCardTypeID))
                    .LogCommand()
                    .ExecuteListAsync<Guid>(cancellationToken).ConfigureAwait(false);
            }
        }

        private static void InjectForumTab(CardType sourceType, CardType targetType)
        {
            sourceType.SchemeItems.CopyToTheBeginningOf(targetType.SchemeItems);
            sourceType.Forms.CopyToTheBeginningOf(targetType.Forms);
        }

        #endregion

        #region Base Overrides

        public override async Task ModifyTypes(ICardMetadataExtensionContext context)
        {
            if (!LicensingHelper.CheckForumLicense(this.licenseManager.License, out _))
            {
                return;
            }

            // Получаем карточку с вкладкой "обсуждение"

            CardType forumCardType = await this.TryGetCardTypeAsync(context, ForumHelper.TopicTabTypeID).ConfigureAwait(false);
            if (forumCardType == null)
            {
                //надо заполнить VR context
                return;
            }

            var allowedCardTypeIDs = await this.GetCardTypeIDsAsync(context.CancellationToken).ConfigureAwait(false);
            // вставляем вкладку только в карточку из списка (переделать на список, который можно редактировать из карточки типовое решение)
            foreach (var cardTypeID in allowedCardTypeIDs)
            {
                CardType cardType = await this.TryGetCardTypeAsync(context, cardTypeID).ConfigureAwait(false);
                if (cardType != null)
                {
                    InjectForumTab(forumCardType, cardType);
                }
            }
        }

        #endregion
    }
}
