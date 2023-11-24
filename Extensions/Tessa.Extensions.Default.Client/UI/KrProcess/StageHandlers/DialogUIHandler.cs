using System;
using System.Threading.Tasks;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards.Controls;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public class DialogUIHandler : StageTypeUIHandlerBase
    {
        #region Base Overrides

        /// <inheritdoc />
        public override Task Validate(IKrStageTypeUIHandlerContext context)
        {
            if (context.Row.TryGet<int?>(KrDialogStageTypeSettingsVirtual.CardStoreModeID) is null)
            {
                context.ValidationResult.AddError(this, "$KrStages_Dialog_CardStoreModeNotSpecified");
            }

            if (context.Row.TryGet<int?>(KrDialogStageTypeSettingsVirtual.OpenModeID) is null)
            {
                context.ValidationResult.AddError(this, "$KrStages_Dialog_CardOpenModeNotSpecified");
            }

            if (context.Row.TryGet<Guid?>(KrDialogStageTypeSettingsVirtual.DialogTypeID) is null)
            {
                context.ValidationResult.AddError(this, "$KrStages_Dialog_DialogTypeIDNotSpecified");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task Initialize(IKrStageTypeUIHandlerContext context)
        {
            var rowModel = context.RowModel;
            if (rowModel.Controls.TryGet("ButtonSettings") is GridViewModel grid)
            {
                grid.RowEditorClosing += ButtonSettings_RowClosing;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task Finalize(IKrStageTypeUIHandlerContext context)
        {
            var rowModel = context.RowModel;
            if (rowModel.Controls.TryGet("ButtonSettings") is GridViewModel grid)
            {
                grid.RowEditorClosing -= ButtonSettings_RowClosing;
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Проверяет корректность настройки кнопок.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Информация о событии.</param>
        private void ButtonSettings_RowClosing(object sender, GridRowEventArgs e)
        {
            var row = e.Row;
            var validationResult = new ValidationResultBuilder();

            if (!row.TryGet<int?>(KrDialogButtonSettingsVirtual.TypeID).HasValue)
            {   
                validationResult.AddError(this, "$KrStages_Dialog_ButtonTypeIDNotSpecified");
                e.Cancel = true;
            }

            if (string.IsNullOrEmpty(row.TryGet<string>(KrDialogButtonSettingsVirtual.Caption)))
            {
                validationResult.AddError(this, "$KrStages_Dialog_ButtonCaptionNotSpecified");
                e.Cancel = true;
            }

            if (string.IsNullOrEmpty(row.TryGet<string>(KrDialogButtonSettingsVirtual.Name)))
            {
                validationResult.AddError(this, "$KrStages_Dialog_ButtonAliasNotSpecified");
                e.Cancel = true;
            }

            TessaDialog.ShowNotEmpty(validationResult);
        }

        #endregion
    }
}