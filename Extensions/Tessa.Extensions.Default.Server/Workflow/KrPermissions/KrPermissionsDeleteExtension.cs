using System;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <summary>
    /// Расширение должно выполняться до того, как будут удалены
    /// </summary>
    public sealed class KrPermissionsDeleteExtension :
        CardDeleteExtension
    {
        #region Constructors

        public KrPermissionsDeleteExtension(IKrPermissionsManager permissionsManager)
        {
            this.permissionsManager = permissionsManager;
        }

        #endregion

        #region Fields

        private readonly IKrPermissionsManager permissionsManager;

        #endregion

        #region Base Overrides

        public override async Task AfterBeginTransaction(ICardDeleteExtensionContext context)
        {
            Guid? cardID;

            if (context.CardType == null
                || !(cardID = context.Request.CardID).HasValue)
            {
                return;
            }

            await using (context.DbScope.Create())
            {
                var permContext = await permissionsManager.TryCreateContextAsync(
                    new KrPermissionsCreateContextParams
                    {
                        CardID = cardID,
                        CardTypeID = context.CardType.ID,
                        ValidationResult = context.ValidationResult,
                        AdditionalInfo = context.Info,
                        ExtensionContext = context,
                    },
                    cancellationToken: context.CancellationToken);

                if (permContext != null)
                {
                    await permissionsManager.CheckRequiredPermissionsAsync(
                        permContext,
                        KrPermissionFlagDescriptors.DeleteCard);
                }
            }
        }

        #endregion
    }
}
