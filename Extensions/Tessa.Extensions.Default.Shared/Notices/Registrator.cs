using Tessa.Cards.Caching;
using Tessa.Notices;
using Tessa.Platform.Data;
using Tessa.Platform.Licensing;
using Tessa.Platform.Runtime;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Shared.Notices
{
    [Registrator(Tag = RegistratorTag.Client)]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<INotificationSubscriptionPermissionManager, KrNotificationSubscriptionPermissionManager>(new ContainerControlledLifetimeManager())
                ;
        }
    }
}
