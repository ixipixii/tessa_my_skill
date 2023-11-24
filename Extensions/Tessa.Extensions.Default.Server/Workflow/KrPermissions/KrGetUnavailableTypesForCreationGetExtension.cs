using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrGetUnavailableTypesForCreationGetExtension : CardRequestExtension
    {
        #region Fields

        private readonly IKrTypesCache typesCache;
        private readonly IKrPermissionsManager permissionsManager;

        #endregion

        #region Constructors

        public KrGetUnavailableTypesForCreationGetExtension(
            IKrTypesCache typesCache,
            IKrPermissionsManager permissionsManager)
        {
            this.typesCache = typesCache;
            this.permissionsManager = permissionsManager;
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            var cardTypes = await this.typesCache.GetCardTypesAsync(context.CancellationToken);
            var docTypes = await this.typesCache.GetDocTypesAsync(context.CancellationToken);

            if (cardTypes == null
                && docTypes == null)
            {
                return;
            }

            List<object> unavaibleTypes = new List<object>();
            await using (context.DbScope.Create())
            {
                foreach (var cardType in cardTypes)
                {
                    if (!cardType.UseDocTypes)
                    {
                        var permContext = await permissionsManager.TryCreateContextAsync(
                            new KrPermissionsCreateContextParams
                            {
                                CardTypeID = cardType.ID,
                                ExtensionContext = context,
                            },
                            cancellationToken: context.CancellationToken);

                        if (!await permissionsManager.CheckRequiredPermissionsAsync(
                            permContext,
                            KrPermissionFlagDescriptors.CreateCard))
                        {
                            unavaibleTypes.Add(cardType.ID);
                        }
                    }
                }

                foreach (var docType in docTypes)
                {
                    var permContext = await permissionsManager.TryCreateContextAsync(
                        new KrPermissionsCreateContextParams
                        {
                            CardTypeID = docType.CardTypeID,
                            DocTypeID = docType.ID,
                            ExtensionContext = context,
                        },
                        cancellationToken: context.CancellationToken);

                    if (!await permissionsManager.CheckRequiredPermissionsAsync(
                        permContext,
                        KrPermissionFlagDescriptors.CreateCard))
                    {
                        unavaibleTypes.Add(docType.ID);
                    }
                }
            }

            context.Response.Info.Add(KrPermissionsHelper.UnavaliableTypesKey, unavaibleTypes.ToArray());
        }

        #endregion
    }
}
