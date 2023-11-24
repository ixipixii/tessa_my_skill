using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.Requests
{
    public sealed class KrCardStoreExtension : CardStoreExtension
    {
        #region fields

        private readonly IKrTypesCache krCache;

        private readonly ParentStageRowIDVisitor visitor;

        #endregion

        #region constructor

        public KrCardStoreExtension(
            IKrTypesCache krCache,
            ICardMetadata metadata)
        {
            this.krCache = krCache;
            this.visitor = new ParentStageRowIDVisitor(metadata);
        }

        #endregion

        #region public

        public override Task BeforeRequest(
            ICardStoreExtensionContext context)
        {
            RemoveRedundantData(context.Request.TryGetCard());

            if (!KrProcessSharedHelper.DesignTimeCard(context.Request.Card.TypeID)
                && KrComponentsHelper.GetKrComponents(context.Request.Card, this.krCache).HasNot(KrComponents.Routes))
            {
                return Task.CompletedTask;
            }

            var card = context.Request.Card;
            this.visitor.Visit(card.Sections, DefaultCardTypes.KrCardTypeID, KrConstants.KrStages.Virtual);

            if (!card.Sections.TryGetValue(KrConstants.KrStages.Virtual, out var stagesSection))
            {
                return Task.CompletedTask;
            }

            foreach (var row in stagesSection.Rows)
            {
                row.SetChanged(KrConstants.KrStages.DisplayTimeLimit, false);
                row.SetChanged(KrConstants.KrStages.DisplayParticipants, false);
                row.SetChanged(KrConstants.KrStages.DisplaySettings, false);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region private

        private static void RemoveRedundantData(Card card)
        {
            if (card is null)
            {
                return;
            }

            card.RemoveLocalTiles();
        }

        #endregion
    }
}