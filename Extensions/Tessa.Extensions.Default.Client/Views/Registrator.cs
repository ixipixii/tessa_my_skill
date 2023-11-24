using Tessa.Extensions.Default.Client.Workplaces;
using Tessa.Platform;
using Tessa.UI.Views.Charting;
using Tessa.UI.Views.Extensions;
using Tessa.Views;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Client.Views
{
    using Workplaces.Manager;

    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            // Регистрация клиентских представлений в контейнере приложения должна осуществлятся с уникальным именем
            // желательно совпадающим с алиасом представления в метаданных. В случае не уникальности имен
            // в контенере и IViewService будет зарегестировано представление последним осуществившее
            // регистрацию в контейнере. Регистрация в IViewService будет осуществлена по алиасу из метаданных представления

            this.UnityContainer
                .RegisterType<ITessaView, ClientProgramView>(nameof(ClientProgramView), new ContainerControlledLifetimeManager())
                ;
        }


        public override void FinalizeRegistration()
        {
            // типы могут быть не зарегистрированы в тестах или плагинах Chronos

            this.UnityContainer
                .RegisterType<ImageCache>(new ContainerControlledLifetimeManager())
                .TryResolve<IWorkplaceExtensionRegistry>()
                
                .Register(typeof(CreateCardExtension))
                .Register(typeof(CustomButtonWorkplaceComponentExtension))
                .Register(typeof(RecordViewExtension))
                .Register(typeof(GetDataWithDelayExtension))
                .Register(typeof(TreeViewItemTestExtension))
                .Register(typeof(CustomFolderViewExtension))
                .Register(typeof(CustomNavigationViewExtension))
                .Register(typeof(ViewsContextMenuExtension))
                .Register(typeof(ChartViewExtension))
                .Register(typeof(AutomaticNodeRefreshExtension))
                .Register(typeof(ManagerWorkplaceExtension))
                .Register(typeof(PreviewExtension))
                .Register(typeof(RefSectionExtension))

                .RegisterConfiguratorType(
                    typeof(AutomaticNodeRefreshExtension),
                    type => this.UnityContainer.Resolve<AutomaticNodeRefreshConfigurator>())
                .RegisterConfiguratorType(
                    typeof(ChartViewExtension),
                    type => this.UnityContainer.Resolve<ChartViewExtensionConfigurator>())
                .RegisterConfiguratorType(
                    typeof(ManagerWorkplaceExtension),
                    type => this.UnityContainer.Resolve<ManagerWorkplaceExtensionConfigurator>())
                .RegisterConfiguratorType(
                    typeof(CreateCardExtension),
                    type => this.UnityContainer.Resolve<CreateCardExtensionConfigurator>())
                .RegisterConfiguratorType(
                    typeof(RefSectionExtension),
                    type => this.UnityContainer.Resolve<RefSectionExtensionConfigurator>())
                ;
        }
    }
}
