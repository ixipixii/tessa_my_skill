using System.Collections.Generic;
using System.Linq;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals
{
    public struct SignalHandlerFilter : IKrProcessFilter<SignalFilterItem>
    {
        public static SignalHandlerFilter Exclude(params SignalFilterItem[] signalTypes) =>
            new SignalHandlerFilter(signalTypes.ToList().AsReadOnly());

        public static SignalHandlerFilter Exclude(ICollection<SignalFilterItem> signalTypes) =>
            new SignalHandlerFilter(signalTypes.ToList().AsReadOnly());

        private SignalHandlerFilter(
            IReadOnlyCollection<SignalFilterItem> excluded)
        {
            this.Excluded = excluded;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<SignalFilterItem> Excluded { get; }
    }
}