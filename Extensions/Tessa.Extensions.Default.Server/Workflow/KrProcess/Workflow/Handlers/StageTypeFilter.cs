using System;
using System.Collections.Generic;
using System.Linq;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public struct StageTypeFilter : IKrProcessFilter<Guid>
    {
        public static StageTypeFilter Exclude(params Guid[] handlerIDs) =>
            new StageTypeFilter(handlerIDs.ToList().AsReadOnly());

        public static StageTypeFilter Exclude(ICollection<Guid> handlerIDs) =>
            new StageTypeFilter(handlerIDs.ToList().AsReadOnly());

        private StageTypeFilter(
            IReadOnlyCollection<Guid> excluded)
        {
            this.Excluded = excluded;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Guid> Excluded { get; }
    }
}