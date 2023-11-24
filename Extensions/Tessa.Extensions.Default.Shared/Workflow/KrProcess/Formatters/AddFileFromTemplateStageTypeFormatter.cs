using System;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class AddFileFromTemplateStageTypeFormatter : StageTypeFormatterBase
    {
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            context.DisplayParticipants = string.Empty;
            context.DisplayTimeLimit = string.Empty;
            context.DisplaySettings = string.Join(
                Environment.NewLine,
                context.StageRow.Fields.Get<string>(KrConstants.KrAddFromTemplateSettingsVirtual.FileTemplateName),
                context.StageRow.Fields.Get<string>(KrConstants.KrAddFromTemplateSettingsVirtual.Name)
            ).Trim();
        }

        public override void FormatServer(IStageTypeFormatterContext context)
        {
            context.DisplayParticipants = string.Empty;
            context.DisplayTimeLimit = string.Empty;
            context.DisplaySettings = string.Join(
                Environment.NewLine,
                context.Settings.TryGet<string>(KrConstants.KrAddFromTemplateSettingsVirtual.FileTemplateName),
                context.Settings.TryGet<string>(KrConstants.KrAddFromTemplateSettingsVirtual.Name)
            ).Trim();
        }
    }
}