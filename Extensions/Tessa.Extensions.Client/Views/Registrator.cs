using Tessa.UI.Views.Extensions;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Client.Views
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<PnrRefreshTreeViews>(new PerResolveLifetimeManager())
                .RegisterType<PnrRefreshWorkplace>(new PerResolveLifetimeManager())
                ;
        }

        public override void FinalizeRegistration()
        {
            if (this.UnityContainer.IsRegistered<IWorkplaceExtensionRegistry>())
            {
                this.UnityContainer
                    .Resolve<IWorkplaceExtensionRegistry>()
                    .Register(typeof(PnrRefreshTreeViews))
                    .Register(typeof(PnrRefreshWorkplace))
                    ;
            }
        }
    }
}
