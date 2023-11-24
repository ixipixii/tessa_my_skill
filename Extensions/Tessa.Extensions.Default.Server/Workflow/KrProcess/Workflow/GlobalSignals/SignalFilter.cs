using System.Collections.Generic;
using System.Linq;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals
{
    public struct SignalFilter : IKrProcessFilter<string>
    {
        public static SignalFilter Exclude(params string[] signalTypes) =>
            new SignalFilter(signalTypes.ToList().AsReadOnly());

        public static SignalFilter Exclude(ICollection<string> signalTypes) =>
            new SignalFilter(signalTypes.ToList().AsReadOnly());

        private SignalFilter(
            IReadOnlyCollection<string> excluded)
        {
            this.Excluded = excluded;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Excluded { get; }
    }
}