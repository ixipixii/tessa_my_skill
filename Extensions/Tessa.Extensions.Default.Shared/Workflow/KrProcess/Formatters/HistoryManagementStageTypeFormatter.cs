using System.Runtime.CompilerServices;
using System.Text;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public class HistoryManagementStageTypeFormatter: StageTypeFormatterBase
    {
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            var caption = context.StageRow.Fields.TryGet<string>(KrConstants.KrHistoryManagementStageSettingsVirtual.TaskHistoryGroupTypeCaption);
            var parentCaption = context.StageRow.Fields.TryGet<string>(KrConstants.KrHistoryManagementStageSettingsVirtual.ParentTaskHistoryGroupTypeCaption);
            var newIteration = context.StageRow.Fields.TryGet<bool?>(KrConstants.KrHistoryManagementStageSettingsVirtual.NewIteration);
            FormatInternal(context, caption, parentCaption, newIteration == true);
        }

        public override void FormatServer(
            IStageTypeFormatterContext context)
        {
            var caption = context.Settings.TryGet<string>(KrConstants.KrHistoryManagementStageSettingsVirtual.TaskHistoryGroupTypeCaption);
            var parentCaption = context.Settings.TryGet<string>(KrConstants.KrHistoryManagementStageSettingsVirtual.ParentTaskHistoryGroupTypeCaption);
            var newIteration = context.Settings.TryGet<bool?>(KrConstants.KrHistoryManagementStageSettingsVirtual.NewIteration);
            FormatInternal(context, caption, parentCaption, newIteration == true);
        }

        private static void FormatInternal(
            IStageTypeFormatterContext context,
            string caption,
            string parentCaption,
            bool newIteration)
        {
            var sb = new StringBuilder();
            bool needNewLine = false;
            if (!string.IsNullOrWhiteSpace(caption))
            {
                sb.Append(ToExtendedLocalization(caption));
                needNewLine = true;
            }

            if (!string.IsNullOrWhiteSpace(parentCaption))
            {
                AppendLine(sb, ref needNewLine);
                sb.Append(ToExtendedLocalization(parentCaption));
                needNewLine = true;
            }

            if (newIteration)
            {
                AppendLine(sb, ref needNewLine);
                sb.Append("{$UI_KrHistoryManagement_NewIteration}");
            }

            context.DisplaySettings = sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ToExtendedLocalization(
            string str)
        {
            if (str[0] == '$')
            {
                str = "{" + str + "}";
            }

            return str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendLine(
            StringBuilder sb,
            ref bool needNewLine)
        {
            if (needNewLine)
            {
                needNewLine = false;
            }

            sb.AppendLine();
        }
    }
}