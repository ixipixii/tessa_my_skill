using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Notices;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Shared.Notices
{
    public class KrNotificationSubscriptionPermissionManager : NotificationSubscriptionPermissionManager
    {
        #region Fields

        private readonly ISession session;
        private readonly IKrTypesCache typesCache;

        #endregion

        #region Constructors

        public KrNotificationSubscriptionPermissionManager(
            ISession session,
            IKrTypesCache typesCache)
            : base(session)
        {
            this.session = session;
            this.typesCache = typesCache;
        }

        #endregion

        #region INotificationSubscriptionPermissionManager Implementation

        public override ValueTask<bool> CheckAccessAsync(
            Card card,
            IValidationResultBuilder validationResult = null,
            CancellationToken cancellationToken = default)
        {
            Guid? typeID = card.TypeID;
            if (typeID == CardHelper.NotificationSubscriptionsTypeID)
            {
                typeID = card.Sections.GetOrAddEntry(Tessa.Notices.NotificationHelper.NotificationSubscriptionSettings).RawFields.TryGet<Guid?>("CardTypeID");
            }

            if (!typeID.HasValue
                || !KrComponentsHelper.HasBase(typeID.Value, typesCache))
            {
                return new ValueTask<bool>(session.User.IsAdministrator());
            }

            var krToken = KrToken.TryGet(card.Info);
            var result = krToken != null &&  krToken.HasPermission(KrPermissionFlagDescriptors.SubscribeForNotifications);

            return new ValueTask<bool>(result);
        }

        #endregion
    }
}