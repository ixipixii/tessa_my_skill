using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Localization;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants.KrCreateCardStageSettingsVirtual;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class CreateCardStageTypeFormatter : StageTypeFormatterBase
    {
        /// <inheritdoc />
        public override void FormatClient(
            IStageTypeFormatterContext context)
        {
            FormatInternal(context, context.StageRow.Fields);
        }

        /// <inheritdoc />
        public override void FormatServer(
            IStageTypeFormatterContext context)
        {
            FormatInternal(context, context.Settings);
        }


        private static void FormatInternal(
            IStageTypeFormatterContext context,
            IDictionary<string, object> storage)
        {
            var display = new List<string>
            {
                $"{LocalizationManager.Localize(storage.TryGet(TypeCaption, string.Empty))}{storage.TryGet(TemplateCaption, string.Empty)}",
                $"{LocalizationManager.Localize(storage.TryGet(ModeName, string.Empty))}"
            };

            context.DisplaySettings = string.Join(Environment.NewLine, display.Where(x => !string.IsNullOrWhiteSpace(x)));

        }
    }
}