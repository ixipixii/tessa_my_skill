using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class SigningStageTypeFormatter : StageTypeFormatterBase
    {
        /// <inheritdoc />
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            base.FormatClient(context);
            context.DisplaySettings = context.StageRow.TryGet<bool>(KrSigningStageSettingsVirtual.IsParallel)
                ? "$UI_KrApproval_Parallel" 
                : "$UI_KrApproval_Sequential";
        }

        /// <inheritdoc />
        public override void FormatServer(IStageTypeFormatterContext context)
        {
            base.FormatServer(context);
            context.DisplaySettings = context.Settings.TryGet<bool>(KrSigningStageSettingsVirtual.IsParallel)
                ? "$UI_KrApproval_Parallel"
                : "$UI_KrApproval_Sequential";
        }
    }
}