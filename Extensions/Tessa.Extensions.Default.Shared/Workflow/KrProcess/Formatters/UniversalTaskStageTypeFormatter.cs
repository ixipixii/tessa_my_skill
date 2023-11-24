using System.Collections.Generic;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class UniversalTaskStageTypeFormatter : StageTypeFormatterBase
    {
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            base.FormatClient(context);

            var settings = context.StageRow.Fields;
            FormatInternal(context, settings);
        }

        /// <inheritdoc />
        public override void FormatServer(IStageTypeFormatterContext context)
        {
            base.FormatServer(context);

            var settings = context.Settings;
            FormatInternal(context, settings);
        }


        private static void FormatInternal(
            IStageTypeFormatterContext context,
            IDictionary<string, object> settings)
        {
            var builder = StringBuilderHelper.Acquire();

            AppendString(builder, settings, KrConstants.KrUniversalTaskSettingsVirtual.KindCaption, string.Empty, true);
            AppendString(builder, settings, KrConstants.KrUniversalTaskSettingsVirtual.Digest, string.Empty, true, limit: 30);

            context.DisplaySettings = builder.ToStringAndRelease();
        }
    }
}