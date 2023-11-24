using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Forms;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class ProcessManagementUIHandler : StageTypeUIHandlerBase
    {
        private static readonly object stageMode = 0;
        private static readonly object groupMode = 1;
        private static readonly object signalMode = 5;

        private const string StageGroupAlias = "StageGroup";
        private const string StageRowAlias = "StageRow";
        private const string SignalAlias = "Signal";

        private DefaultFormSimpleViewModel form;
        private IControlViewModel stageControl;
        private IControlViewModel groupControl;
        private IControlViewModel signalControl;
        private bool initialized;

        /// <inheritdoc />
        public override Task Initialize(
            IKrStageTypeUIHandlerContext context)
        {
            // Достаем все необходимые элементы управления
            this.form = context.RowModel.MainForm as DefaultFormSimpleViewModel;
            if (this.form is null
                || !this.form.Controls.TryGet(StageRowAlias, out this.stageControl)
                || !this.form.Controls.TryGet(StageGroupAlias, out this.groupControl)
                || !this.form.Controls.TryGet(SignalAlias, out this.signalControl))
            {
                return Task.CompletedTask;
            }

            if (this.stageControl is null
                || this.groupControl is null
                || this.signalControl is null)
            {
                return Task.CompletedTask;
            }

            this.initialized = true;

            // Обновляем видимость элементов управления
            this.UpdateVisibility(context.Row[KrConstants.KrProcessManagementStageSettingsVirtual.ModeID]);
            // Подписываемся на события изменения режима
            context.Row.FieldChanged += this.ModeChanged;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task Finalize(
            IKrStageTypeUIHandlerContext context)
        {
            if (this.initialized)
            {
                // Отписываемся от события изменения режима
                context.Row.FieldChanged -= this.ModeChanged;
            }

            this.initialized = false;
            return Task.CompletedTask;
        }

        public override Task Validate(IKrStageTypeUIHandlerContext context)
        {
            // Проверяем, что режим указан
            if (context.Row.TryGet<int?>(KrConstants.KrProcessManagementStageSettingsVirtual.ModeID) is null)
            {
                // Если режим не указан, то показываем ошибку
                context.ValidationResult.AddError(this, "$KrStages_ProcessManagement_ModeNotSpecified");
            }

            return Task.CompletedTask;
        }

        private void ModeChanged(
            object s,
            CardFieldChangedEventArgs args)
        {
            if (args.FieldName != KrConstants.KrProcessManagementStageSettingsVirtual.ModeID)
            {
                return;
            }

            this.UpdateVisibility(args.FieldValue);
        }

        private void UpdateVisibility(
            object value)
        {
            this.stageControl.ControlVisibility = Visibility.Collapsed;
            this.groupControl.ControlVisibility = Visibility.Collapsed;
            this.signalControl.ControlVisibility = Visibility.Collapsed;

            if (Equals(value, stageMode))
            {
                this.stageControl.ControlVisibility = Visibility.Visible;
            }
            else if (Equals(value, groupMode))
            {
                this.groupControl.ControlVisibility = Visibility.Visible;
            }
            else if (Equals(value, signalMode))
            {
                this.signalControl.ControlVisibility = Visibility.Visible;
            }
            this.form.Rearrange();
        }
    }
}