using System;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class RegistrationStageTypeFormatter: StageTypeFormatterBase
    {
        public override void FormatClient(
            IStageTypeFormatterContext context)
        {
            base.FormatClient(context);
            
            context.DisplaySettings = string.Join(
                Environment.NewLine, 
                context.StageRow.TryGet<string>(KrConstants.KrRegistrationStageSettingsVirtual.Comment) ?? string.Empty, 
                context.StageRow.TryGet<bool?>(KrConstants.KrRegistrationStageSettingsVirtual.WithoutTask) == true
                    ? "{$CardTypes_Controls_WithoutTask}"
                    : string.Empty)
                .Trim();
        }

        public override void FormatServer(
            IStageTypeFormatterContext context)
        {
            base.FormatServer(context);

            context.DisplaySettings = string.Join(
                Environment.NewLine, 
                context.Settings.TryGet<string>(KrConstants.KrRegistrationStageSettingsVirtual.Comment) ?? string.Empty, 
                context.Settings.TryGet<bool?>(KrConstants.KrRegistrationStageSettingsVirtual.WithoutTask) == true
                    ? "{$CardTypes_Controls_WithoutTask}"
                    : string.Empty)
                .Trim();
        }
    }
}