using System;
using System.Linq;
using NLog;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrCacheExecutor : IKrExecutor
    {
        #region fields
        
        private readonly IKrCompilationCache compileCache;
        private readonly Func<IKrExecutor> getExecutor;
        private readonly IDbScope dbScope;
        private readonly ISession session;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        #endregion

        #region constructor

        public KrCacheExecutor(
            IKrCompilationCache compileCache,
            [Dependency(KrExecutorNames.GroupExecutor)] Func<IKrExecutor> getExecutor,
            IDbScope dbScope,
            ISession session)
        {
            this.compileCache = compileCache;
            this.getExecutor = getExecutor;

            this.dbScope = dbScope;
            this.session = session;
        }

        #endregion

        #region implementation

        public IKrExecutionResult Execute(IKrExecutionContext context)
        {
            var compilationResult = this.compileCache.Get();
            if (compilationResult.Result.Assembly == null)
            {
                logger.LogResult(compilationResult.ValidationResult);

                return new KrExecutionResult(
                    compilationResult.ToMissingAssemblyResult(),
                    EmptyHolder<Guid>.Collection,
                    KrExecutionStatus.AssemblyMissed);
            }

            var stageGroupIDs = KrCompilersSqlHelper.SelectFilteredStageGroups(
                this.dbScope, 
                context.TypeID ?? Guid.Empty,
                this.session.User.ID, 
                secondaryProcessID: context.SecondaryProcess?.ID);

            var newExecutionUnitsIDs = context.ExecuteAll
                ? stageGroupIDs
                : context.ExecutionUnitIDs.Intersect(stageGroupIDs);

            var ctx = context.Copy(compilationResult, newExecutionUnitsIDs);
            var executionResult = this.getExecutor().Execute(ctx);

            RemoveRedundantGroups(context, ctx);

            return executionResult;
        }

        #endregion

        #region private

        private static void RemoveRedundantGroups(
            IKrExecutionContext context,
            IKrExecutionContext nestedContext)
        {
            if (context.ExecuteAll)
            {
                // Случай полного пересчета
                // Удаляем все группы, не попавшие под пересчет
                var newStages = new SealableObjectList<Stage>(context.WorkflowProcess.Stages.Count);

                foreach (var stage in context.WorkflowProcess.Stages)
                {
                    if (nestedContext.ExecutionUnitIDs.Contains(stage.StageGroupID))
                    {
                        newStages.Add(stage);
                    }
                    else if (stage.RowChanged || stage.OrderChanged)
                    {
                        stage.UnbindTemplate = true;
                        newStages.Add(stage);
                    }
                }

                context.WorkflowProcess.Stages = newStages;
            }
            else
            {
                // Случай частичного пересчета
                // Удаляем группы из context.ExecutionUnitIDs, не попавшие в ctx.ExecutionUnitIDs
                var redundantGroups = context.ExecutionUnitIDs.Except(nestedContext.ExecutionUnitIDs).ToList();
                var newStages = new SealableObjectList<Stage>(context.WorkflowProcess.Stages.Count);
                foreach (var stage in context.WorkflowProcess.Stages)
                {
                    if (!redundantGroups.Contains(stage.StageGroupID))
                    {
                        newStages.Add(stage);
                    }
                    else if (stage.RowChanged || stage.OrderChanged)
                    {
                        stage.UnbindTemplate = true;
                        newStages.Add(stage);
                    }
                }

                context.WorkflowProcess.Stages = newStages;
            }
        }

        #endregion
    }
}
