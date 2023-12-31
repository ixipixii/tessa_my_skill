﻿using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrStageInterrupter: IKrStageInterrupter
    {
        private readonly IKrScope krScope;

        private readonly IKrProcessContainer processContainer;

        public KrStageInterrupter(
            IKrScope krScope,
            IKrProcessContainer processContainer)
        {
            this.krScope = krScope;
            this.processContainer = processContainer;
        }

        /// <inheritdoc />
        public bool InterruptStage(IKrStageInterrupterContext context)
        {
            var currentStage = context.Stage;

            if (!currentStage.StageTypeID.HasValue)
            {
                return true;
            }

            var handler = this.processContainer.ResolveHandler(currentStage.StageTypeID.Value);
            var stageContext = new StageTypeHandlerContext(
                context.RunnerContext,
                currentStage,
                context.RunnerMode,
                context.DirectionAfterInterrupt);
            var completelyInterrupted = handler.HandleStageInterrupt(stageContext);
            if (completelyInterrupted)
            {
                this.krScope.TryAddToTrace(new KrProcessTraceItem(currentStage, null, context.RunnerContext.CardID));
                if (context.SetupNextState != null)
                {
                    this.krScope.SetRunnerState(
                        context.RunnerContext.ProcessInfo.ProcessID,
                        context.SetupNextState.Invoke(true));
                }
                return true;
            }
            this.krScope.SetRunnerState(
                context.RunnerContext.ProcessInfo.ProcessID,
                new KrProcessState(
                    KrConstants.InterruptionProcessState,
                    null,
                    context.SetupNextState?.Invoke(false)));
            return false;
        }
    }
}