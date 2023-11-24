using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public interface IStageTypeFormatterContext
    {
        ISession Session { get; }
        Dictionary<string, object> Info { get; }
        Card Card { get; }
        CardRow StageRow { get; }
        IDictionary<string, object> Settings { get; }
        string DisplayTimeLimit { get; set; }
        string DisplayParticipants { get; set; }
        string DisplaySettings { get; set; }
    }
}