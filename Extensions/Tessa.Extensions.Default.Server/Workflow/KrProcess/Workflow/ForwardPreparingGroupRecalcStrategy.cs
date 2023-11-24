using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class ForwardPreparingGroupRecalcStrategy: IPreparingGroupRecalcStrategy
    {
        private readonly IDbScope dbScope;

        private readonly ISession session;

        private int minOrder;

        public ForwardPreparingGroupRecalcStrategy(
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
            
            if (stage != null && prevStage != null)
            {
                if (prevStage.StageGroupOrder < stage.StageGroupOrder)
                {
                    // Переход между группами
                    this.ExecutionUnits = this.GetNextStageGroups(prevStage, stage, context);
                    this.minOrder = prevStage.StageGroupOrder + 1;

                    if (this.ExecutionUnits.Count == 0)
                    {
                        // Такая ситуация возможна, когда следующая группа удалена, а между ними ничего нет.
                        // Нужно попытаться найти что-нибудь для старта, а также добавить удаленную группу в расчет,
                        // чтобы она вылетела из маршрута.
                        var nextGroups = this.GetNextStageGroups(prevStage, null, context);
                        this.ExecutionUnits = new List<Guid>();
                        if (nextGroups.Count > 0)
                        {
                            // Они отсортированы по Order, поэтому в 0 будет с минимальным ордером
                            this.ExecutionUnits.Add(nextGroups[0]);
                        }

                        this.ExecutionUnits.Add(stage.StageGroupID);
                    }
                }
                else
                {
                    context.ValidationResult.AddError(this, "$KrProcess_ErrorMessage_StageStageGroupOrderLessPrevStageStageGroupOrder");
                    this.ExecutionUnits = EmptyHolder<Guid>.Array;
                    return;
                }
            }
            else if (stage is null)
            {
                // Новой группы нет, процесс завершается.
                // Пытаемся найти еще что-нибудь
                this.ExecutionUnits = this.GetNextStageGroups(prevStage, null, context);
                this.minOrder = prevStage.StageGroupOrder + 1;
            }
            else/* if (prevStage is null)*/
            {
                // Процесс только начался, старой группы нет, только новая
                // При старте процесса считаем маршрут посчитанным и пересчет отдельной группы не имеет смысла.
                this.ExecutionUnits = EmptyHolder<Guid>.Array;
                this.minOrder = stage.StageGroupOrder;
            }
        }

        private IList<Guid> GetNextStageGroups(
            Stage from,
            Stage to,
            IKrProcessRunnerContext context)
        {
            return KrCompilersSqlHelper
                .SelectFilteredStageGroups(
                    this.dbScope,
                    context.DocTypeID ?? context.CardTypeID ?? Guid.Empty,
                    this.session.User.ID,
                    from?.StageGroupOrder + 1,
                    to?.StageGroupOrder,
                    context.SecondaryProcess?.ID);
        }
    }
}