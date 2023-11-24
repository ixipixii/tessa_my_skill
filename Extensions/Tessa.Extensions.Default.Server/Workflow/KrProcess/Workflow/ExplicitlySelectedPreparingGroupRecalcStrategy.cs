using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class ExplicitlySelectedPreparingGroupRecalcStrategy : IPreparingGroupRecalcStrategy
    {
        private int minOrder;

        /// <inheritdoc />
        public bool Used { get; private set; } = false;

        /// <inheritdoc />
        public IList<Guid> ExecutionUnits { get; private set; }

        /// <inheritdoc />
        public Stage GetSuitableStage(
            IList<Stage> stages)
        {
            foreach (var stage in stages)
            {
                if (stage.StageGroupOrder >= this.minOrder)
                {
                    return stage;
                }
            }

            return null;
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

            if (stage != null)
            {
                // Переход в конкретное место
                this.ExecutionUnits = new[] { stage.StageGroupID };
                this.minOrder = stage.StageGroupOrder;
            }
            else
            {
                this.ExecutionUnits = EmptyHolder<Guid>.Collection;
                this.minOrder = int.MaxValue;
            }
        }
    }
}