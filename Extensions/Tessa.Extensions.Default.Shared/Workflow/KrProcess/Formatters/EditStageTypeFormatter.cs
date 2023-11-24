using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class EditStageTypeFormatter : StageTypeFormatterBase
    {
        /// <inheritdoc />
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            base.FormatClient(context);
            context.DisplaySettings = context.StageRow.Fields.Get<bool>(KrEditSettingsVirtual.ChangeState)
                ? "$UI_KrEdit_ChangeState"
                : string.Empty;
        }

        /// <inheritdoc />
        public override void FormatServer(IStageTypeFormatterContext context)
        {
            base.FormatServer(context);
            context.DisplaySettings = context.Settings.TryGet<bool>(KrEditSettingsVirtual.ChangeState)
                ? "$UI_KrEdit_ChangeState"
                : string.Empty;
        }
    }
}