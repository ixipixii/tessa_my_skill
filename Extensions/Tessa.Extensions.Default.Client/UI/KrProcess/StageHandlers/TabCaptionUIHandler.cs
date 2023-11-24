using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class TabCaptionUIHandler: StageTypeUIHandlerBase
    {
        private TabContentIndicator indicator;

        private readonly ICardMetadata metadata;

        public TabCaptionUIHandler(
            ICardMetadata metadata)
        {
            this.metadata = metadata;
        }

        /// <inheritdoc />
        public override async Task Initialize(
            IKrStageTypeUIHandlerContext context)
        {
            if (context.RowModel.Controls.TryGet(KrConstants.Ui.CSharpSourceTable, out var control)
                && control is TabControlViewModel tabControl)
            {
                var sectionMeta = (await this.metadata.GetSectionsAsync(context.CancellationToken).ConfigureAwait(false))[KrConstants.KrStages.Virtual];
                var fieldIDs = sectionMeta.Columns.ToDictionary(k => k.ID, v => v.Name);
                var storage = context.Row;
                this.indicator = new TabContentIndicator(tabControl, storage, fieldIDs, true);
                this.indicator.Update();
                context.Row.FieldChanged += this.indicator.FieldChangedAction;
            }
        }

        /// <inheritdoc />
        public override Task Finalize(
            IKrStageTypeUIHandlerContext context)
        {
            if (this.indicator != null)
            {
                context.Row.FieldChanged -= this.indicator.FieldChangedAction;
            }

            return Task.CompletedTask;
        }
    }
}