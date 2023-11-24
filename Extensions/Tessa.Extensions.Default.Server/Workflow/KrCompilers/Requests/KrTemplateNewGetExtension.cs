using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants.KrApprovalSettingsVirtual;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public abstract class KrTemplateNewGetExtension : CardNewGetExtension
    {
        #region fields

        private readonly IKrStageSerializer serializer;

        #endregion

        #region constructor

        protected KrTemplateNewGetExtension(IKrStageSerializer serializer)
        {
            this.serializer = serializer;
        }

        #endregion

        #region base overrides

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }
            if (context.Method == CardNewMethod.Template)
            {
                foreach (var row in context.Response.Card.GetStagesSection().Rows)
                {
                    row[FirstIsResponsible] = BooleanBoxes.False;
                }

                var card = context.Response.Card;
                await StageRowMigrationHelper.MigrateAsync(
                    card,
                    card,
                    KrProcessSerializerHiddenStageMode.Ignore,
                    this.serializer,
                    cancellationToken: context.CancellationToken);

                KrProcessHelper.SetInactiveStateToStages(context.Response.Card);
                KrCompilersHelper.ClearPhysicalSections(context.Response.Card);
            }
            KrProcessHelper.SetStageDefaultValues(context.Response);
        }

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            var card = context.Response.Card;
            await this.serializer.DeserializeSectionsAsync(card, card, cardContext: context, cancellationToken: context.CancellationToken);
            KrProcessHelper.SetStageDefaultValues(context.Response);
            KrCompilersHelper.ClearPhysicalSections(context.Response.Card);
        }

        #endregion

    }
}