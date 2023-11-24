﻿using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Platform.Server.AdSync;
using Tessa.Extensions.Platform.Server.Cards;
using Tessa.Extensions.Platform.Server.DocLoad;
using Tessa.Platform.Placeholders.Compilation;
using Tessa.Roles;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Server.Cards
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<MoveFileRequestExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<DefaultConfigurationVersionStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<DefaultConfigurationVersionDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<DefaultConfigurationVersionNewGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<DocLoadBarcodeTemplateNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<DocLoadBarcodeStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<GetDocTypeInfoRequestExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDocStateNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDocStateGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDocStateStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrDocStateDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<IAdExtension, DefaultAdExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<IDocLoadExtension, DefaultDocLoadExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<CreateOrAddPartnerCardStoreExtension>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor(
                        new ResolvedParameter<ICardRepository>(CardRepositoryNames.ExtendedWithoutTransaction)))
                .RegisterType<KrVirtualFileStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrVirtualFileNewGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrAddVirtualFilesGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrVirtualFileDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrSettingsForumLicenseGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrPermissionRuleNewGetExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrPermissionRuleDeleteExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<ICardStrictSecurity, KrVirtualFileStrictSecurity>(DefaultCardTypes.KrVirtualFileTypeName, new ContainerControlledLifetimeManager())
                .RegisterType<SatelliteRemoveCardNewExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrPersonalRolesStoreExtension>(new ContainerControlledLifetimeManager())
                .RegisterType<KrResetUserSettingsRequestExtension>(new ContainerControlledLifetimeManager())
                ;
        }


        public override void RegisterExtensions(IExtensionContainer extensionContainer)
        {
            // New
            extensionContainer
                .RegisterExtension<ICardNewExtension, CardPermissionsNewExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithSingleton()
                    .WhenMethod(CardNewMethod.Default, CardNewMethod.Template))
                .RegisterExtension<ICardNewExtension, DefaultConfigurationVersionNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 10)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrDocTypeTypeID))
                .RegisterExtension<ICardNewExtension, KrDocStateNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 11)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrDocStateTypeID))
                .RegisterExtension<ICardNewExtension, DocLoadBarcodeTemplateNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 12)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardNewMethod.Template))
                .RegisterExtension<ICardNewExtension, KrVirtualFileNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 13)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrVirtualFileTypeID)
                    .WhenAnyNewMethod())
                .RegisterExtension<ICardNewExtension, KrPermissionRuleNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 14)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrPermissionsTypeID)
                    .WhenAnyNewMethod())
                .RegisterExtension<ICardNewExtension, SatelliteRemoveCardNewExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 15)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(CardHelper.TemplateTypeID)
                    .WhenAnyNewMethod())
                ;

            // Get
            extensionContainer
                .RegisterExtension<ICardGetExtension, CardPermissionsGetExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithSingleton())
                .RegisterExtension<ICardGetExtension, DefaultConfigurationVersionNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 10)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrDocTypeTypeID))
                .RegisterExtension<ICardGetExtension, KrDocStateGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 11)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrDocStateTypeID))
                .RegisterExtension<ICardGetExtension, KrAddVirtualFilesGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 12)
                    .WithUnity(this.UnityContainer))
                .RegisterExtension<ICardGetExtension, KrVirtualFileNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 13)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrVirtualFileTypeID))
                .RegisterExtension<ICardGetExtension, KrSettingsForumLicenseGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 14)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrSettingsTypeID, DefaultCardTypes.KrDocTypeTypeID))
                .RegisterExtension<ICardGetExtension, KrPermissionRuleNewGetExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 15)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrPermissionsTypeID))
                ;

            // Store
            extensionContainer
                .RegisterExtension<ICardStoreExtension, CardPermissionsStoreExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithSingleton())
                .RegisterExtension<ICardStoreExtension, CardNumberStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithSingleton())
                .RegisterExtension<ICardStoreExtension, PartnerContractStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithSingleton()
                    .WhenCardTypes(DefaultCardTypes.PartnerTypeID))
                .RegisterExtension<ICardStoreExtension, ContractStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithSingleton()
                    .WhenCardTypes(DefaultCardTypes.ContractTypeID))
                .RegisterExtension<ICardStoreExtension, DefaultConfigurationVersionStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyStoreMethod()
                    .WhenCardTypes(DefaultCardTypes.KrDocTypeTypeID))
                .RegisterExtension<ICardStoreExtension, CreateOrAddPartnerCardStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 5)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyStoreMethod()
                    .WhenCardTypes(
                        DefaultCardTypes.ContractTypeID,
                        DefaultCardTypes.IncomingTypeID,
                        DefaultCardTypes.OutgoingTypeID))
                .RegisterExtension<ICardStoreExtension, SaveFileTemplateOnCardStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 6)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyStoreMethod())
                .RegisterExtension<ICardStoreExtension, DocLoadBarcodeStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 7)
                    .WithUnity(this.UnityContainer)
                    .WhenMethod(CardStoreMethod.Default))
                .RegisterExtension<ICardStoreExtension, KrDocStateStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 8)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrDocStateTypeID))
                .RegisterExtension<ICardStoreExtension, KrVirtualFileStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 9)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyStoreMethod()
                    .WhenCardTypes(DefaultCardTypes.KrVirtualFileTypeID))
                .RegisterExtension<ICardStoreExtension, KrPersonalRolesStoreExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 10)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(RoleHelper.PersonalRoleTypeID))
                ;

            // Delete
            extensionContainer
                .RegisterExtension<ICardDeleteExtension, CardPermissionsDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithSingleton())
                .RegisterExtension<ICardDeleteExtension, DefaultConfigurationVersionDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyDeleteMethod()
                    .WhenCardTypes(DefaultCardTypes.KrDocTypeTypeID))
                .RegisterExtension<ICardDeleteExtension, KrDocStateDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenCardTypes(DefaultCardTypes.KrDocStateTypeID))
                .RegisterExtension<ICardDeleteExtension, KrVirtualFileDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 3)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyDeleteMethod()
                    .WhenCardTypes(DefaultCardTypes.KrVirtualFileTypeID))
                .RegisterExtension<ICardDeleteExtension, KrPermissionRuleDeleteExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 4)
                    .WithUnity(this.UnityContainer)
                    .WhenAnyDeleteMethod()
                    .WhenCardTypes(DefaultCardTypes.KrPermissionsTypeID))
                ;

            // Request
            extensionContainer
                .RegisterExtension<ICardRequestExtension, MoveFileRequestExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(DefaultRequestTypes.MoveFiles))
                .RegisterExtension<ICardRequestExtension, KrResetUserSettingsRequestExtension>(x => x
                    .WithOrder(ExtensionStage.BeforePlatform, 2)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(CardRequestTypes.ResetUserSettings))
                .RegisterExtension<ICardRequestExtension, GetDocTypeInfoRequestExtension>(x => x
                    .WithOrder(ExtensionStage.AfterPlatform, 1)
                    .WithUnity(this.UnityContainer)
                    .WhenRequestTypes(DefaultRequestTypes.GetDocTypeInfo))
                ;
        }


        public override void FinalizeRegistration()
        {
            this.UnityContainer
                .Resolve<IPlaceholderDocumentBuilderContainer>()
                .Register(WordPlaceholderDocumentBuilder.Build, ".docx")
                .Register(ExcelPlaceholderDocumentBuilder.Build, ".xlsx", ".xlsm")
                ;

            var compiler = this.UnityContainer.Resolve<IPlaceholderExtensionCompiler>();
            compiler.DefaultReferences.Add("DocumentFormat.OpenXml");
            compiler.DefaultUsings.Add("DocumentFormat.OpenXml");
            compiler.DefaultUsings.Add("Tessa.Extensions.Default.Server.Cards");

            this.UnityContainer
                .Resolve<ICardStrictSecurityResolver>()
                .Register<KrVirtualFileStrictSecurity>(DefaultCardTypes.KrVirtualFileTypeName)
                ;
        }
    }
}
