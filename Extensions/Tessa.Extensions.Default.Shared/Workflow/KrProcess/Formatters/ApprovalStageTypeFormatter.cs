using System.Collections.Generic;
using Tessa.Platform;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class ApprovalStageTypeFormatter : StageTypeFormatterBase
    {
        /// <inheritdoc />
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            base.FormatClient(context);
            FormatInternal(context, context.StageRow);
        }

        /// <inheritdoc />
        public override void FormatServer(IStageTypeFormatterContext context)
        {
            base.FormatServer(context);
            FormatInternal(context, context.Settings);
        }

        private static void FormatInternal(
            IStageTypeFormatterContext context,
            IDictionary<string, object> storage)
        {
            var sb = StringBuilderHelper.Acquire(128);
            if (storage.TryGet<bool>(KrApprovalSettingsVirtual.Advisory))
            {
                sb.AppendLine("{$UI_KrApproval_Advisory}");
            }
            
            sb.Append(storage.TryGet<bool>(KrApprovalSettingsVirtual.IsParallel)
                ? "{$UI_KrApproval_Parallel}" 
                : "{$UI_KrApproval_Sequential}");
            context.DisplaySettings = sb.ToStringAndRelease();
        }
        
    }
}