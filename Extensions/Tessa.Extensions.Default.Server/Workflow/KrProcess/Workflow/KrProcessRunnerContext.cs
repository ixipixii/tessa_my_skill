using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessRunnerContext : IKrProcessRunnerContext
    {
        #region fields

        private readonly bool resurrection;
        
        private readonly Lazy<KrProcessRunnerInitiationCause> initiationCauseLazy;

        #endregion
        
        #region constructor

        public KrProcessRunnerContext(
            IWorkflowAPIBridge workflowAPI,
            IKrTaskHistoryResolver taskHistoryResolver,
            IMainCardAccessStrategy mainCardAccessStrategy,
            Guid? cardID,
            Guid? cardTypeID,
            string cardTypeName,
            string cardTypeCaption,
            Guid? docTypeID,
            KrComponents? krComponents,
            Card contextualSatellite,
            Card processHolderSatellite,
            WorkflowProcess workflowProcess,
            ProcessHolder processHolder,
            IWorkflowProcessInfo processInfo,
            IValidationResultBuilder validationResult,
            ICardExtensionContext cardContext,
            Func<IPreparingGroupRecalcStrategy> defaultPreparingGroupStrategyFunc,
            string parentProcessTypeName,
            Guid? parentProcessID,
            IKrSecondaryProcess secondaryProcess = null,
            bool ignoreGroupScripts = false,
            bool resurrection = false)
        {
            this.WorkflowAPI = workflowAPI;
            this.TaskHistoryResolver = taskHistoryResolver;
            this.MainCardAccessStrategy = mainCardAccessStrategy;
            this.CardID = cardID;
            this.CardTypeID = cardTypeID;
            this.CardTypeName = cardTypeName;
            this.CardTypeCaption = cardTypeCaption;
            this.DocTypeID = docTypeID;
            this.KrComponents = krComponents;
            this.ContextualSatellite = contextualSatellite;
            this.ProcessHolderSatellite = processHolderSatellite;
            this.WorkflowProcess = workflowProcess;
            this.ProcessHolder = processHolder;
            this.ProcessInfo = processInfo;
            this.ValidationResult = validationResult;
            this.CardContext = cardContext;
            this.DefaultPreparingGroupStrategyFunc = defaultPreparingGroupStrategyFunc;
            this.ParentProcessTypeName = parentProcessTypeName;
            this.ParentProcessID = parentProcessID;
            this.SecondaryProcess = secondaryProcess;
            this.IgnoreGroupScripts = ignoreGroupScripts;
            this.resurrection = resurrection;
            this.initiationCauseLazy = new Lazy<KrProcessRunnerInitiationCause>(this.DetermineInitiationCause);
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IWorkflowAPIBridge WorkflowAPI { get; }

        /// <inheritdoc />
        public IKrTaskHistoryResolver TaskHistoryResolver { get; }

        /// <inheritdoc />
        public IMainCardAccessStrategy MainCardAccessStrategy { get; }

        /// <inheritdoc />
        public Guid? CardID { get; }

        /// <inheritdoc />
        public Guid? CardTypeID { get; }

        /// <inheritdoc />
        public string CardTypeName { get; }

        /// <inheritdoc />
        public string CardTypeCaption { get; }

        /// <inheritdoc />
        public Guid? DocTypeID { get; }

        /// <inheritdoc />
        public KrComponents? KrComponents { get; }

        /// <inheritdoc />
        public ProcessHolder ProcessHolder { get; }

        /// <inheritdoc />
        public Card ContextualSatellite { get; }

        /// <inheritdoc />
        public Card ProcessHolderSatellite { get; }

        /// <inheritdoc />
        public WorkflowProcess WorkflowProcess { get; }

        /// <inheritdoc />
        public KrProcessRunnerInitiationCause InitiationCause => this.initiationCauseLazy.Value;

        /// <inheritdoc />
        public IWorkflowProcessInfo ProcessInfo { get; }

        /// <inheritdoc />
        public IWorkflowTaskInfo TaskInfo => this.ProcessInfo as IWorkflowTaskInfo;

        /// <inheritdoc />
        public IWorkflowSignalInfo SignalInfo => this.ProcessInfo as IWorkflowSignalInfo;

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult { get; }

        /// <inheritdoc />
        public ICardExtensionContext CardContext { get; }

        /// <inheritdoc />
        public IKrSecondaryProcess SecondaryProcess { get; }

        /// <inheritdoc />
        public string ParentProcessTypeName { get; }

        /// <inheritdoc />
        public Guid? ParentProcessID { get; }

        /// <inheritdoc />
        public bool IgnoreGroupScripts { get; }

        /// <inheritdoc />
        public Dictionary<Guid, IKrExecutionUnit> ExecutionUnitCache { get; } =
            new Dictionary<Guid, IKrExecutionUnit>();

        /// <inheritdoc />
        public List<Guid> SkippedStagesByCondition { get; } = new List<Guid>(16);

        /// <inheritdoc />
        public List<Guid> SkippedGroupsByCondition { get; } = new List<Guid>(8);

        /// <inheritdoc />
        public Func<IPreparingGroupRecalcStrategy> DefaultPreparingGroupStrategyFunc { get; }

        /// <inheritdoc />
        public IPreparingGroupRecalcStrategy PreparingGroupStrategy { get; set; }

        #endregion

        #region private

        private KrProcessRunnerInitiationCause DetermineInitiationCause()
        {
            if (this.SignalInfo != null)
            {
                return KrProcessRunnerInitiationCause.Signal;
            }
            // Избежание нескольких кастов.
            var taskInfo = this.TaskInfo;
            if (taskInfo != null)
            {
                return taskInfo.Task.Action == CardTaskAction.Reinstate
                    ? KrProcessRunnerInitiationCause.ReinstateTask
                    : KrProcessRunnerInitiationCause.CompleteTask;
            }
            if (this.ProcessInfo != null)
            {
                return KrProcessRunnerInitiationCause.StartProcess;
            }
            
            return this.resurrection
                ? KrProcessRunnerInitiationCause.Resurrection
                : KrProcessRunnerInitiationCause.InMemoryLaunching;
        }

        #endregion
    }
}