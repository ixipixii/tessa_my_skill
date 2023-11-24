using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class DisableRecalcPreparingGroupRecalcStrategy : IPreparingGroupRecalcStrategy
    {
        private bool hasStage;

        private Guid stageID;

        /// <inheritdoc />
        public bool Used { get; private set; } = false;

        /// <inheritdoc />
        public IList<Guid> ExecutionUnits { get; } = EmptyHolder<Guid>.Collection;

        /// <inheritdoc />
        public Stage GetSuitableStage(
            IList<Stage> stages)
        {
            return this.hasStage
                ? stages.FirstOrDefault(p => p.ID == this.stageID)
                : null;
        }

        /// <inheritdoc />
        public void Apply(
            IKrProcessRunnerContext context,
            Stage stage,
            Stage prevStage)
        {
            if (this.Used)
            {
                throw new InvalidOperationException($"Current object {this.GetType().FullName} was used previously.");
            }

            this.Used = true;

            this.hasStage = stage != null;
            if (this.hasStage)
            {
                this.stageID = stage.ID;
            }
        }
    }
}