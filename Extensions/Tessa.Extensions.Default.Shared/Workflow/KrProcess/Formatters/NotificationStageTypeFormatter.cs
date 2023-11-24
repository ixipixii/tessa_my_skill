using System;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class NotificationStageTypeFormatter : StageTypeFormatterBase
    {
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            base.FormatClient(context);

            var excludeDeputies = context.StageRow.Fields.Get<bool>(KrNotificationSettingVirtual.ExcludeDeputies);
            var excludeSubscribers = context.StageRow.Fields.Get<bool>(KrNotificationSettingVirtual.ExcludeSubscribers);

            context.DisplayTimeLimit = string.Empty;
            context.DisplaySettings = GetDisplaySettings(excludeDeputies, excludeSubscribers);
        }

        public override void FormatServer(IStageTypeFormatterContext context)
        {
            base.FormatServer(context);

            var excludeDeputies = context.Settings.TryGet<bool>(KrNotificationSettingVirtual.ExcludeDeputies);
            var excludeSubscribers = context.Settings.TryGet<bool>(KrNotificationSettingVirtual.ExcludeSubscribers);

            context.DisplayTimeLimit = string.Empty;
            context.DisplaySettings = GetDisplaySettings(excludeDeputies, excludeSubscribers);
        }

        private string GetDisplaySettings(bool excludeDeputies, bool excludeSubscribers)
        {
            var settings = string.Empty;
            if (excludeDeputies)
            {
                settings = "{$UI_KrNotification_ExcludeDeputies}";
            }
            if (excludeSubscribers)
            {
                if (excludeDeputies)
                {
                    settings += Environment.NewLine;
                }
                settings += "{$UI_KrNotification_ExcludeSubscribers}";
            }

            return settings;
        }
    }
}