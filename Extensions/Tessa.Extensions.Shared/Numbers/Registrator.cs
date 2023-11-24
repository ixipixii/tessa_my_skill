using System.Diagnostics;
using Tessa.Cards.Numbers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Shared.Numbers
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            // Регистрируем подмену Sequence для каждого типа документа
            this.UnityContainer
                .RegisterType<INumberDirector, PnrDocumentNumberDirector>(new ContainerControlledLifetimeManager())
                ;
        }

        public override void FinalizeRegistration()
        {
            this.UnityContainer
                .TryResolve<INumberDirectorContainer>()
                ?
                .Register(typeID: PnrCardTypes.PnrOrderTypeID, getDirectorFunc: c => c.Resolve<PnrDocumentNumberDirector>())
                ;
            this.UnityContainer
                .TryResolve<INumberDirectorContainer>()
                ?
                .Register(typeID: PnrCardTypes.PnrIncomingTypeID, getDirectorFunc: c => c.Resolve<PnrDocumentNumberDirector>())
                ;
            this.UnityContainer
                .TryResolve<INumberDirectorContainer>()
                ?
                .Register(typeID: PnrCardTypes.PnrOutgoingTypeID, getDirectorFunc: c => c.Resolve<PnrDocumentNumberDirector>())
                ;
        }
    }
}
