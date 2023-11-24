using System;
using System.Threading.Tasks;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards.Controls;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public class UniversalTaskStageTypeUIHandler : StageTypeUIHandlerBase
    {
        #region Base Overrides

        /// <inheritdoc />
        public override Task Initialize(
            IKrStageTypeUIHandlerContext context)
        {
            var rowModel = context.RowModel;
            if (rowModel.Controls.TryGet("CompletionOptions") is GridViewModel grid)
            {
                grid.RowInvoked += RowInvoked;
                grid.RowEditorClosing += RowClosing;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task Finalize(
            IKrStageTypeUIHandlerContext context)
        {
            var rowModel = context.RowModel;
            if (rowModel.Controls.TryGet("CompletionOptions") is GridViewModel grid)
            {
                grid.RowInvoked -= RowInvoked;
                grid.RowEditorClosing -= RowClosing;
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private static void RowInvoked(object sender, GridRowEventArgs e)
        {
            if (e.Action == GridRowAction.Inserted)
            {
                e.Row.Fields[KrUniversalTaskOptionsSettingsVirtual.OptionID] = Guid.NewGuid();
            }
        }

        private void RowClosing(object sender, GridRowEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Row.TryGet<string>(KrUniversalTaskOptionsSettingsVirtual.Caption)))
            {
                TessaDialog.ShowNotEmpty(ValidationResult.FromText(this, "$KrProcess_UniversalTask_CompletionOptionCaptionEmpty", ValidationResultType.Error));
                e.Cancel = true;
            }
        }

        #endregion
    }
}