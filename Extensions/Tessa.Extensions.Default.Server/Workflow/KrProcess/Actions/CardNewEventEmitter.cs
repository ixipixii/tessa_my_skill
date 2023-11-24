using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Actions
{
    public sealed class CardNewEventEmitter : CardNewExtension
    {
        private readonly IKrEventManager eventManager;

        private readonly IKrTypesCache typesCache;

        public CardNewEventEmitter(
            IKrEventManager eventManager,
            IKrTypesCache typesCache)
        {
            this.eventManager = eventManager;
            this.typesCache = typesCache;
        }


        public override Task AfterRequest(
            ICardNewExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Response.Card.TypeID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var validationResult = context.ValidationResult;
            var cardID = context.Response.Card.ID;
            var strategy = new ObviousMainCardAccessStrategy(context.Response.Card);

            return this.eventManager.RaiseAsync(
                DefaultEventTypes.NewCard,
                cardID,
                strategy,
                context,
                validationResult,
                cancellationToken: context.CancellationToken);
        }

    }
}