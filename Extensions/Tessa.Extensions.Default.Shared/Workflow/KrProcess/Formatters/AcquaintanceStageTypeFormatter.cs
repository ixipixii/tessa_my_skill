using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class AcquaintanceStageTypeFormatter : StageTypeFormatterBase
    {
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            base.FormatClient(context);
            context.DisplayTimeLimit = string.Empty;
            context.DisplaySettings = context.StageRow.Fields.Get<bool>(KrAcquaintanceSettingsVirtual.ExcludeDeputies)
                ? "$UI_KrAcquaintance_ExcludeDeputies" 
                : string.Empty;
        }

        public override void FormatServer(IStageTypeFormatterContext context)
        {
            base.FormatServer(context);
            context.DisplayTimeLimit = string.Empty;
            context.DisplaySettings = context.Settings.TryGet<bool>(KrAcquaintanceSettingsVirtual.ExcludeDeputies)
                ? "$UI_KrAcquaintance_ExcludeDeputies"
                : string.Empty;
        }
    }
}