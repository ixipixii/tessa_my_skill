using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Extensions.Default.Server.Workflow.KrPermissions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Notices;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Notices
{
    public sealed class KrNotificationSubscriptionPermissionManagerServer : KrNotificationSubscriptionPermissionManager
    {
        #region Fields

        private readonly IDbScope dbScope;
        private readonly IKrTokenProvider krTokenProvider;
        private readonly IKrPermissionsManager permissionsManager;
        private readonly IKrPermissionsCacheContainer permissionsCacheContainer;

        private static readonly KrPermissionFlagDescriptor[] notificationPermissions
            = new KrPermissionFlagDescriptor[] { KrPermissionFlagDescriptors.SubscribeForNotifications };

        #endregion

        #region Constructors

        public KrNotificationSubscriptionPermissionManagerServer(
            ISession session,
            IKrTypesCache typesCache,
            IDbScope dbScope,
            IKrTokenProvider krTokenProvider,
            IKrPermissionsManager permissionsManager,
            IKrPermissionsCacheContainer permissionsCacheContainer)
            : base(session, typesCache)
        {
            this.dbScope = dbScope;
            this.krTokenProvider = krTokenProvider;
            this.permissionsManager = permissionsManager;
            this.permissionsCacheContainer = permissionsCacheContainer;
        }

        #endregion

        #region Base Overrides

        public override async ValueTask<bool> CheckAccessAsync(
            Guid cardID,
            IValidationResultBuilder validationResult = null,
            CancellationToken cancellationToken = default)
        {
            var permContext = await permissionsManager.TryCreateContextAsync(
                new KrPermissionsCreateContextParams
                {
                    CardID = cardID,
                    ValidationResult = validationResult,
                },
                cancellationToken);

            return
                await permissionsManager.CheckRequiredPermissionsAsync(
                    permContext,
                    notificationPermissions);
        }

        public override async ValueTask<bool> CheckAccessAsync(
            Card card,
            IValidationResultBuilder validationResult = null,
            CancellationToken cancellationToken = default)
        {
            var permContext = await permissionsManager.TryCreateContextAsync(
                new KrPermissionsCreateContextParams
                {
                    Card = card,
                    CardTypeID = await GetCardTypeIDAsync(card.ID, cancellationToken),
                    ValidationResult = validationResult,
                },
                cancellationToken);

            if (permContext != null)
            {
                return
                    await permissionsManager.CheckRequiredPermissionsAsync(
                        permContext,
                        notificationPermissions);
            }
            return await base.CheckAccessAsync(card, validationResult, cancellationToken);
        }

        public override async ValueTask SetAccessAsync(Card card, CancellationToken cancellationToken = default)
        {
            krTokenProvider
                .CreateToken(
                    card.ID,
                    CardComponentHelper.DoNotCheckVersion,
                    await permissionsCacheContainer.GetVersionAsync(),
                    notificationPermissions)
                .Set(card.Info);
        }

        #endregion

        #region Private Methods

        private async Task<Guid> GetCardTypeIDAsync(Guid cardID, CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var query = dbScope.BuilderFactory
                    .Select()
                    .C("TypeID")
                    .From("Instances").NoLock()
                    .Where().C("ID").Equals().P("CardID")
                    .Build();
                return await dbScope.Db.SetCommand(
                        query,
                        dbScope.Db.Parameter("CardID", cardID))
                    .LogCommand()
                    .ExecuteAsync<Guid>();
            }
        }

        #endregion

    }
}
