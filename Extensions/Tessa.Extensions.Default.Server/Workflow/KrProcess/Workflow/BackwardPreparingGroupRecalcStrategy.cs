using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class BackwardPreparingGroupRecalcStrategy : IPreparingGroupRecalcStrategy
    {
        private readonly IDbScope dbScope;

        private readonly ISession session;

        private Guid stageGroupID;
        private bool transitToCurrent;

        public BackwardPreparingGroupRecalcStrategy(
            IDbScope dbScope,
            ISession session)
        {
            this.dbScope = dbScope;
            this.session = session;
        } 

        /// <inheritdoc />
        public bool Used { get; private set; } = false;

        /// <inheritdoc />
        public IList<Guid> ExecutionUnits { get; private set; }

        /// <inheritdoc />
        public Stage GetSuitableStage(
            IList<Stage> stages)
        {
            var idx = this.transitToCurrent
                ? TransitionHelper.TransitByPredicate(stages, p => p.StageGroupID == this.stageGroupID) 
                : TransitionHelper.TransitToPreviousGroup(stages, this.stageGroupID);
            return idx == TransitionHelper.NotFound
                ? null
                : stages[idx];
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
            
            if (stage != null
                && prevStage != null
                && stage.StageGroupOrder < prevStage.StageGroupOrder)
            {
                // Расчет разницы между текущим и новым, который выше
                this.ExecutionUnits = this.GetStageGroups(stage, prevStage, context);
                this.transitToCurrent = false;
                if (this.ExecutionUnits.Count == 0)
                {
                    // Между ними нет этапов, а также сам этап выше удален.
                    // Делается пересчет текущего и производится переход на текущий.
                    this.ExecutionUnits.Add(stage.StageGroupID);
                    this.transitToCurrent = true;
                }
                else
                {
                    this.ExecutionUnits = this.ExecutionUnits.Take(1).ToList();
                }
                this.stageGroupID = prevStage.StageGroupID;
                
                return;
            }

            throw new InvalidOperationException();
        }

        private IList<Guid> GetStageGroups(
            Stage from,
            Stage to,
            IKrProcessRunnerContext context)
        {
            return KrCompilersSqlHelper
                .SelectFilteredStageGroups(
                    this.dbScope,
                    context.DocTypeID ?? context.CardTypeID ?? Guid.Empty,
                    this.session.User.ID,
                    from?.StageGroupOrder,
                    to?.StageGroupOrder - 1,
                    context.SecondaryProcess?.ID);
        }

    }
}