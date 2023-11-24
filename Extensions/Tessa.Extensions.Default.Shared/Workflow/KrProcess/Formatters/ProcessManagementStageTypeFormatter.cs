using System.Collections.Generic;
using Tessa.Platform;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants.KrProcessManagementStageSettingsVirtual;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class ProcessManagementStageTypeFormatter : StageTypeFormatterBase
    {
        private const int StageMode = 0;
        private const int GroupMode = 1;
        private const int SignalMode = 5;

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
            var managePrimaryProcess = storage.TryGet<bool?>(ManagePrimaryProcess) ?? false;
            var modeID = storage.TryGet<int?>(ModeID);
            var modeName = storage.TryGet<string>(ModeName);
            var stageName = storage.TryGet<string>(StageName);
            var groupName = storage.TryGet<string>(KrProcessManagementStageSettingsVirtual.StageGroupName);
            var groupRowName = storage.TryGet<string>(StageRowGroupName);
            var signal = storage.TryGet<string>(Signal);

            var stringBuilder = StringBuilderHelper.Acquire(256);

            if (!string.IsNullOrWhiteSpace(modeName))
            {
                stringBuilder.AppendLine("{" + modeName + "}");
            }

            if (modeID == StageMode
                && !string.IsNullOrWhiteSpace(stageName))
            {
                if (stageName[0] == '$')
                {
                    stageName = "{" + stageName + "}";
                }

                if (!string.IsNullOrWhiteSpace(groupRowName))
                {
                    groupRowName = groupRowName[0] == '$'
                        ? " ({" + groupRowName + "})"
                        : " (" + groupRowName + ")";
                }

                stringBuilder.AppendLine(stageName + groupRowName);
            }
            else if (modeID == GroupMode
                && !string.IsNullOrWhiteSpace(groupName))
            {
                if (groupName[0] == '$')
                {
                    groupName = "{" + groupName + "}";
                }
                stringBuilder.AppendLine(groupName);
            }
            else if (modeID == SignalMode
                && !string.IsNullOrWhiteSpace(signal))
            {
                stringBuilder.AppendLine(signal);
            }
            if (managePrimaryProcess)
            {
                stringBuilder.AppendLine("{$CardTypes_Controls_ManagePrimaryProcess}");
            }

            context.DisplaySettings = stringBuilder.ToStringAndRelease().TrimEnd();
        }

    }
}