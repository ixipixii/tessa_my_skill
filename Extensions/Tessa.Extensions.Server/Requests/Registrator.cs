using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared;

namespace Tessa.Extensions.Server.Requests
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {

            extensionContainer
                .RegisterExtension<ICardRequestExtension, GetIndexLegalEntityRequest>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.GetIndexLegalEntityRequestTypeID))
                .RegisterExtension<ICardRequestExtension, GetUserDepartmentInfoRequest>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.GetUserDepartmentInfoRequestTypeID))
                .RegisterExtension<ICardRequestExtension, DocxToPdfFileExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.GetPdfFromDocxTypeID))
                .RegisterExtension<ICardRequestExtension, GetPartnerInfoExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.PartnerInfo))
                .RegisterExtension<ICardRequestExtension, GetOrganizationHead>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.GetOrganizationHead))
                .RegisterExtension<ICardRequestExtension, GetIsUserInRoleExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.GetIsUserInRoleExtension))
                .RegisterExtension<ICardRequestExtension, SetFdStateCardExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(PnrRequestTypes.SetFdStateCard))
                ;
        }
    }
}
