using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class ResolutionStageTypeFormatter : StageTypeFormatterBase
    {
        /// <inheritdoc />
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

            AppendString(builder, settings, KrConstants.KrResolutionSettingsVirtual.KindCaption, "{$CardTypes_Controls_Kind}", true);
            AppendString(builder, settings, KrConstants.KrAuthorSettingsVirtual.AuthorName, "{$CardTypes_Controls_From}");
            AppendString(builder, settings, KrConstants.KrResolutionSettingsVirtual.ControllerName, "{$CardTypes_Controls_Controller}", canBeWithoutValue: true);

            context.DisplaySettings = builder.ToStringAndRelease();

            DateTime? planned = settings.TryGet<DateTime?>(KrConstants.KrResolutionSettingsVirtual.Planned);
            double? timeLimit = settings.TryGet<double?>(KrConstants.KrResolutionSettingsVirtual.DurationInDays);
            DefaultDateFormatting(planned, timeLimit, context);
        }
    }
}
