using System;
using System.Collections.Generic;
using Tessa.Cards;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public sealed class StageTaskRevokerContext : IStageTasksRevokerContext
    {
        public StageTaskRevokerContext(
            IStageTypeHandlerContext context)
        {
            this.Context = context;
        }

        /// <inheritdoc />
        public IStageTypeHandlerContext Context { get; }

        /// <inheritdoc />
        public Guid CardID { get; set; }

        /// <inheritdoc />
        public Guid TaskID { get; set; }

        /// <inheritdoc />
        public List<Guid> TaskIDs { get; set; }

        /// <inheritdoc />
        public Guid? OptionID { get; set; }

        /// <inheritdoc />
        public bool RemoveFromActive { get; set; }

        /// <inheritdoc />
        public Action<CardTask> TaskModificationAction { get; set; }
    }
}