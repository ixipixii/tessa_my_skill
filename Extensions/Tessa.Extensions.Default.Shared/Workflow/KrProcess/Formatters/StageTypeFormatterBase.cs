using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Tessa.Cards;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public abstract class StageTypeFormatterBase: IStageTypeFormatter
    {
        /// <inheritdoc />
        public virtual void FormatClient(
            IStageTypeFormatterContext context)
        {
            var containsTimeLimit = context.StageRow.TryGetValue(KrStages.TimeLimit, out var timeLimit);
            var containsPlanned = context.StageRow.TryGetValue(KrStages.Planned, out var planned);
            if (containsTimeLimit
                || containsPlanned)
            {
                DefaultDateFormatting(planned as DateTime?, timeLimit as double?, context);
            }

            if (context.StageRow.TryGetValue(KrSinglePerformerVirtual.PerformerName,
                    out var performerNameObj)
                && performerNameObj is string performerName
                && !string.IsNullOrWhiteSpace(performerName))
            {
                DefaultSinglePerformerFormatting(performerName, context);
            }
            else if (context.Card.Sections.TryGetValue(KrPerformersVirtual.Synthetic, out var appSec))
            {
                IEnumerable<string> FormatPerformers(
                    ListStorage<CardRow> rows)
                {
                    foreach (var row in rows)
                    {
                        if (row.State != CardRowState.Deleted
                            && row.TryGet<Guid?>(StageRowID)?.Equals(context.StageRow.RowID) == true
                            && row.TryGetValue(KrPerformersVirtual.PerformerName, out var pnObj)
                            && pnObj is string name)
                        {
                            yield return name.Length > 0 && name[0] == '$'
                                ? '{' + name + '}'
                                : name;
                        }
                    }
                }

                context.DisplayParticipants = string.Join(
                    Environment.NewLine,
                    FormatPerformers(appSec.Rows));
            }
        }

        /// <inheritdoc />
        public virtual void FormatServer(
            IStageTypeFormatterContext context)
        {
            var containsTimeLimit = context.StageRow.TryGetValue(KrStages.TimeLimit, out var timeLimit);
            var containsPlanned = context.StageRow.TryGetValue(KrStages.Planned, out var planned);
            if (containsTimeLimit
                || containsPlanned)
            {
                DefaultDateFormatting(planned as DateTime?, timeLimit as double?, context);
            }

            if (context.Settings.TryGetValue(KrSinglePerformerVirtual.PerformerName, out var performerNameObj)
                && performerNameObj is string performerName
                && !string.IsNullOrWhiteSpace(performerName))
            {
                DefaultSinglePerformerFormatting(performerName, context);
            }
            else if (context.Settings.TryGetValue(KrPerformersVirtual.Synthetic, out var performers)
                && performers is List<object> performersList)
            {
                IEnumerable<string> FormatPerformers(
                    List<object> rows)
                {
                    foreach (var row in rows)
                    {
                        if (row is IDictionary<string, object> rowStorage
                            && rowStorage.TryGetValue(KrPerformersVirtual.PerformerName, out var pnObj)
                            && pnObj is string name)
                        {
                            yield return name.Length > 0 && name[0] == '$'
                                ? '{' + name + '}'
                                : name;
                        }
                    }
                }

                context.DisplayParticipants = string.Join(
                    Environment.NewLine,
                    FormatPerformers(performersList));
            }
        }


        protected static void DefaultDateFormatting(
            DateTime? planned,
            double? timeLimit,
            IStageTypeFormatterContext context)
        {
            if (planned != null)
            {
                context.DisplayTimeLimit = 
                    FormattingHelper.FormatDateTimeWithoutSeconds(planned.Value.ToUniversalTime() + context.Session.ClientUtcOffset, convertToLocal: false)
                    + " UTC" + FormattingHelper.FormatUtcOffset(context.Session.ClientUtcOffset);
            }
            else if (timeLimit != null)
            {
                context.DisplayTimeLimit = timeLimit.Value.ToString(CultureInfo.InvariantCulture)
                    + LocalizationManager.GetString("KrProcess_WorkingDaysSuffix");
            }
            else
            {
                context.DisplayTimeLimit = string.Empty;
            }
        }

        protected static void DefaultSinglePerformerFormatting(
            string performerName,
            IStageTypeFormatterContext context)
        {
            context.DisplayParticipants = performerName;
        }
        
        protected static void AppendString(
            StringBuilder builder,
            IDictionary<string, object> settings,
            string name,
            string caption,
            bool localizable = false,
            bool canBeWithoutValue = false,
            int limit = -1)
        {
            var value = settings.TryGet<string>(name);
            var valueIsNull = string.IsNullOrEmpty(value);
            if (!canBeWithoutValue
                && valueIsNull)
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(caption))
            {
                builder.Append(caption);
                builder.Append(": ");
            }

            if (!valueIsNull)
            {
                if (localizable)
                {
                    localizable = value[0] == '$';
                }

                if (localizable)
                {
                    builder.Append('{');
                }

                if (limit != -1)
                {
                    value = value.Limit(limit);
                }
                
                builder.Append(value);

                if (localizable)
                {
                    builder.Append('}');
                }
            }
        }
    }
}