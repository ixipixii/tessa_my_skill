using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class ForkStageTypeFormatter : StageTypeFormatterBase
    {
        /// <inheritdoc />
        public override void FormatClient(IStageTypeFormatterContext context)
        {
            var rows = context.Card.Sections[KrConstants.KrForkSecondaryProcessesSettingsVirtual.Synthetic].Rows;
            context.DisplaySettings = string.Join(Environment.NewLine, ExtractRows(context.StageRow.RowID, rows));
        }

        /// <inheritdoc />
        public override void FormatServer(IStageTypeFormatterContext context)
        {
            var rows = context.Settings.TryGet<IList<object>>(KrConstants.KrForkSecondaryProcessesSettingsVirtual.Synthetic);
            context.DisplaySettings = string.Join(
                Environment.NewLine, ExtractRows(context.StageRow.RowID, rows.Cast<IDictionary<string, object>>()));
        }

        private static IEnumerable<string> ExtractRows(
            Guid stageRowID,
            IEnumerable<IDictionary<string, object>> rows)
        {
            foreach (var row in rows)
            {
                if (row.TryGet<Guid>(KrConstants.StageRowID) == stageRowID)
                {
                    var state = (CardRowState)row.TryGet<int>(CardRow.SystemStateKey);
                    if (state != CardRowState.Deleted)
                    {
                        var name = row.TryGet<string>(KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessName);
                        yield return name.StartsWith("$", StringComparison.Ordinal) ? "{" + name + "}" : name;
                    }
                }
            }
        }
    }
}