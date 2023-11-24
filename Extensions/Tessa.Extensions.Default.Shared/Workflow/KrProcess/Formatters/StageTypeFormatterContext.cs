using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public class StageTypeFormatterContext : IStageTypeFormatterContext
    {
        public StageTypeFormatterContext(
            ISession session,
            Dictionary<string, object> info,
            Card card,
            CardRow stageRow,
            IDictionary<string, object> settings)
        {
            this.Session = session;
            this.Info = info;
            this.Card = card;
            this.StageRow = stageRow;
            this.Settings = settings;
        }

        /// <inheritdoc />
        public ISession Session { get; }

        /// <inheritdoc />
        public Dictionary<string, object> Info { get; }

        /// <inheritdoc />
        public Card Card { get; }

        /// <inheritdoc />
        public CardRow StageRow { get; }

        /// <inheritdoc />
        public IDictionary<string, object> Settings { get; }

        /// <inheritdoc />
        public string DisplayTimeLimit { get; set; }

        /// <inheritdoc />
        public string DisplayParticipants { get; set; }

        /// <inheritdoc />
        public string DisplaySettings { get; set; }
    }
}