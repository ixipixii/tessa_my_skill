using System;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public sealed class StageTypeHandlerContext : IStageTypeHandlerContext
    {
        public StageTypeHandlerContext(
            Guid? mainCardID,
            Guid? mainCardTypeID,
            string mainCardTypeName,
            string mainCardTypeCaption,
            Guid? mainCardDocTypeID,
            KrComponents? krComponents,
            Card contextualSatellite,
            Card processHolderSatellite,
            ICardExtensionContext cardExtensionContext,
            IValidationResultBuilder validationResult,
            Stage stage,
            WorkflowProcess workflowProcess,
            ProcessHolder processHolder,
            IWorkflowProcessInfo processInfo,
            IWorkflowAPIBridge workflowAPI,
            IKrTaskHistoryResolver taskHistoryResolver,
            KrProcessRunnerMode runnerMode,
            KrProcessRunnerInitiationCause initiationCause,
            IKrProcessButton button,
            IMainCardAccessStrategy mainCardAccessStrategy,
            DirectionAfterInterrupt? directionAfterInterrupt,
            string parentProcessTypeName,
            Guid? parentProcessID)
        {
            this.MainCardID = mainCardID;
            this.MainCardTypeID = mainCardTypeID;
            this.MainCardTypeName = mainCardTypeName;
            this.MainCardTypeCaption = mainCardTypeCaption;
            this.MainCardDocTypeID = mainCardDocTypeID;
            this.KrComponents = krComponents;
            this.ContextualSatellite = contextualSatellite;
            this.ProcessHolderSatellite = processHolderSatellite;
            this.ProcessHolder = processHolder;
            this.CardExtensionContext = cardExtensionContext;
            this.ValidationResult = validationResult;
            this.Stage = stage;
            this.WorkflowProcess = workflowProcess;
            this.ProcessInfo = processInfo;
            this.WorkflowAPI = workflowAPI;
            this.TaskHistoryResolver = taskHistoryResolver;
            this.RunnerMode = runnerMode;
            this.InitiationCause = initiationCause;
            this.SecondaryProcess = button;
            this.MainCardAccessStrategy = mainCardAccessStrategy;
            this.DirectionAfterInterrupt = directionAfterInterrupt;
            this.ParentProcessTypeName = parentProcessTypeName;
            this.ParentProcessID = parentProcessID;

            this.TaskInfo = this.ProcessInfo as IWorkflowTaskInfo;
            this.SignalInfo = this.ProcessInfo as IWorkflowSignalInfo;
        }


        public StageTypeHandlerContext(
            IKrProcessRunnerContext runnerContext,
            Stage stage,
            KrProcessRunnerMode runnerMode,
            DirectionAfterInterrupt? directionAfterInterrupt)
        {
            this.MainCardAccessStrategy = runnerContext.MainCardAccessStrategy;
            this.MainCardID = runnerContext.CardID;
            this.MainCardTypeID = runnerContext.CardTypeID;
            this.MainCardTypeName = runnerContext.CardTypeName;
            this.MainCardTypeCaption = runnerContext.CardTypeCaption;
            this.MainCardDocTypeID = runnerContext.DocTypeID;
            this.KrComponents = runnerContext.KrComponents;
            this.SecondaryProcess = runnerContext.SecondaryProcess;
            this.ContextualSatellite = runnerContext.ContextualSatellite;
            this.ProcessHolderSatellite = runnerContext.ProcessHolderSatellite;
            this.ProcessHolder = runnerContext.ProcessHolder;
            this.CardExtensionContext = runnerContext.CardContext;
            this.ValidationResult = runnerContext.ValidationResult;
            this.WorkflowProcess = runnerContext.WorkflowProcess;
            this.ProcessInfo = runnerContext.ProcessInfo;
            this.TaskInfo = runnerContext.TaskInfo;
            this.SignalInfo = runnerContext.SignalInfo;
            this.InitiationCause = runnerContext.InitiationCause;
            this.Stage = stage;
            this.WorkflowAPI = runnerContext.WorkflowAPI;
            this.TaskHistoryResolver = runnerContext.TaskHistoryResolver;
            this.RunnerMode = runnerMode;
            this.DirectionAfterInterrupt = directionAfterInterrupt;
            this.ParentProcessTypeName = runnerContext.ParentProcessTypeName;
            this.ParentProcessID = runnerContext.ParentProcessID;
        }

        /// <inheritdoc />
        public IMainCardAccessStrategy MainCardAccessStrategy { get; }

        /// <inheritdoc />
        public Guid? MainCardID { get; }

        /// <inheritdoc />
        public Guid? MainCardTypeID { get; }

        /// <inheritdoc />
        public string MainCardTypeName { get; }

        /// <inheritdoc />
        public string MainCardTypeCaption { get; }

        /// <inheritdoc />
        public Guid? MainCardDocTypeID { get; }

        /// <inheritdoc />
        public KrComponents? KrComponents { get; }

        /// <inheritdoc />
        public IKrSecondaryProcess SecondaryProcess { get; }

        /// <inheritdoc />
        public Card ContextualSatellite { get; }

        /// <inheritdoc />
        public Card ProcessHolderSatellite { get; }

        /// <inheritdoc />
        public ProcessHolder ProcessHolder { get; }

        /// <inheritdoc />
        public ICardExtensionContext CardExtensionContext { get; }

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult { get; }

        /// <inheritdoc />
        public Stage Stage { get; }

        /// <inheritdoc />
        public WorkflowProcess WorkflowProcess { get; }

        /// <inheritdoc />
        public IWorkflowProcessInfo ProcessInfo { get; }

        /// <inheritdoc />
        public IWorkflowTaskInfo TaskInfo { get; }

        /// <inheritdoc />
        public IWorkflowSignalInfo SignalInfo { get; }

        /// <inheritdoc />
        public IWorkflowAPIBridge WorkflowAPI { get; }

        /// <inheritdoc />
        public IKrTaskHistoryResolver TaskHistoryResolver { get; }

        /// <inheritdoc />
        public KrProcessRunnerMode RunnerMode { get; }

        /// <inheritdoc />
        public KrProcessRunnerInitiationCause InitiationCause { get; }

        /// <inheritdoc />
        public DirectionAfterInterrupt? DirectionAfterInterrupt { get; }

        /// <inheritdoc />
        public string ParentProcessTypeName { get; }

        /// <inheritdoc />
        public Guid? ParentProcessID { get; }
    }
}