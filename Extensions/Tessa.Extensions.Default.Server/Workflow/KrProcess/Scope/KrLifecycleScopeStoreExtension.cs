using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope
{
    public sealed class KrLifecycleScopeStoreExtension : CardStoreExtension
    {
        #region fields

        private readonly IKrTypesCache krTypesCache;
        private readonly IKrScope scope;
        private KrScopeLevel level;

        #endregion

        #region constructor

        public KrLifecycleScopeStoreExtension(
            IKrTypesCache krTypesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository cardRepositoryEwt,
            IKrTokenProvider tokenProvider,
            IKrScope scope)
        {
            this.krTypesCache = krTypesCache;
            this.scope = scope;
        }

        #endregion

        #region base overrides

        public override Task AfterBeginTransaction(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, this.krTypesCache))
            {
                return Task.CompletedTask;
            }

            this.level = this.scope.EnterNewLevel(context.ValidationResult, false);
            return Task.CompletedTask;
        }

        public override Task AfterRequest(
            ICardStoreExtensionContext context)
        {
            this.level?.Exit();
            this.level = null;
            return Task.CompletedTask;
        }

        #endregion
    }
}