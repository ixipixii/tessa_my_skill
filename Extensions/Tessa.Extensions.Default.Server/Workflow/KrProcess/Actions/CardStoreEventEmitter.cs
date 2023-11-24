using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Actions
{
    public sealed class CardStoreEventEmitter: CardStoreExtension
    {
        private readonly IKrEventManager eventManager;

        private readonly IKrTypesCache typesCache;

        private readonly IKrScope scope;

        public CardStoreEventEmitter(
            IKrEventManager eventManager,
            IKrTypesCache typesCache,
            IKrScope scope)
        {
            this.eventManager = eventManager;
            this.typesCache = typesCache;
            this.scope = scope;
        }

        public override Task BeforeRequest(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var cardID = context.Request.Card.ID;
            var card = new ObviousMainCardAccessStrategy(context.Request.Card);
            var validationResult = context.ValidationResult;

            return this.eventManager.RaiseAsync(
                DefaultEventTypes.BeforeStoreCard,
                cardID,
                card,
                context,
                validationResult,
                cancellationToken: context.CancellationToken);
        }

        public override Task BeforeCommitTransaction(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var cardID = context.Request.Card.ID;
            var card = new KrScopeMainCardAccessStrategy(cardID, this.scope);
            var validationResult = context.ValidationResult;

            return this.eventManager.RaiseAsync(
                DefaultEventTypes.StoreCard,
                cardID,
                card,
                context,
                validationResult,
                cancellationToken: context.CancellationToken);
        }
    }
}