using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NLog;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine;
using Tessa.Extensions.Default.Shared.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public abstract class KrProcessRunnerBase : IKrProcessRunner
    {
        #region nested types

        protected struct NextAction
        {
            public bool ContinueToNextStage;
            public bool AfterTransition;
            public bool AfterGroupTransition;
        }

        #endregion

        #region fields

        protected readonly IKrProcessContainer ProcessContainer;
         
        protected readonly IKrCompilationCache CompilationCache;
        
        protected readonly Func<IKrExecutor> ExecutorFunc;
         
        protected readonly IKrScope Scope;
         
        protected readonly IDbScope DbScope;
         
        protected readonly IKrProcessCache ProcessCache;
         
        protected readonly IUnityContainer UnityContainer;
         
        protected readonly ISession Session;

        protected readonly IKrProcessRunnerProvider RunnerProvider;

        protected readonly IKrTypesCache TypesCache;

        protected readonly ICardMetadata CardMetadata;

        protected readonly IKrProcessStateMachine StateMachine;

        protected readonly IKrStageInterrupter Interrupter;

        protected readonly IKrSqlExecutor SqlExecutor;

        protected readonly ICardCache CardCache;

        protected readonly IKrStageSerializer StageSerializer;

        protected readonly IObjectModelMapper ObjectModelMapper;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region constructor

        protected KrProcessRunnerBase(
            IKrProcessContainer processContainer,
            IKrCompilationCache compilationCache,
            Func<IKrExecutor> executorFunc,
            IKrScope scope,
            IDbScope dbScope,
            IKrProcessCache processCache,
            IUnityContainer unityContainer,
            ISession session,
            IKrProcessRunnerProvider runnerProvider,
            IKrTypesCache typesCache,
            ICardMetadata cardMetadata,
            IKrProcessStateMachine stateMachine,
            IKrStageInterrupter interrupter,
            IKrSqlExecutor sqlExecutor,
            ICardCache cardCache,
            IKrStageSerializer stageSerializer,
            IObjectModelMapper objectModelMapper)
        {
            this.ProcessContainer = processContainer;
            this.CompilationCache = compilationCache;
            this.ExecutorFunc = executorFunc;
            this.Scope = scope;
            this.DbScope = dbScope;
            this.ProcessCache = processCache;
            this.UnityContainer = unityContainer;
            this.Session = session;
            this.RunnerProvider = runnerProvider;
            this.TypesCache = typesCache;
            this.CardMetadata = cardMetadata;
            this.StateMachine = stateMachine;
            this.Interrupter = interrupter;
            this.SqlExecutor = sqlExecutor;
            this.CardCache = cardCache;
            this.StageSerializer = stageSerializer;
            this.ObjectModelMapper = objectModelMapper;
        }

        #endregion

        #region public

        /// <inheritdoc />
        public void Run(
            IKrProcessRunnerContext context)
        {
            this.AssertKrScope();
            
            Exception exc = null;
            try
            {
                using (this.DbScope.Create())
                {
                    if (this.Prepare(context))
                    {
                        this.RunInternal(context);
                    }
                }
            }
            catch (ProcessRunnerInterruptedException e)
            {
                exc = e;
                if (!string.IsNullOrWhiteSpace(e.Message))
                {
                    ValidationSequence
                        .Begin(context.ValidationResult)
                        .SetObjectName(this)
                        .ErrorDetails(e.Message, e.GetFullText())
                        .End();
                }
                else if (context.ValidationResult.IsSuccessful())
                {
                    ValidationSequence
                        .Begin(context.ValidationResult)
                        .SetObjectName(this)
                        .ErrorDetails(LocalizationManager.GetString("KrProcessRunner_Interrupted"), e.GetFullText())
                        .End();
                }
            }
            finally
            {
                this.Finalize(context, exc);
            }
        }

        #endregion

        #region protected

        protected abstract KrProcessRunnerMode RunnerMode { get; }

        protected virtual bool Prepare(
            IKrProcessRunnerContext context) => true;

        protected virtual void Finalize(
            IKrProcessRunnerContext context,
            Exception exception = null)
        {
        }

        protected virtual NextAction ProcessStageHandlerResult(
            Stage stage,
            StageHandlerResult result,
            IKrProcessRunnerContext context)
        {
            switch (result.Action)
            {
                case StageHandlerAction.Complete:
                case StageHandlerAction.Skip:
                {
                    // Ставится Complete или Skipped
                    SetStageFinalState(stage, result);
                    this.RunAfter(stage, context);
                    return new NextAction { ContinueToNextStage = true };
                }
                case StageHandlerAction.Transition:
                {
                    if (result.TransitionID is null)
                    {
                        throw new NullReferenceException(
                            $"Result with action = {StageHandlerAction.Transition} " +
                            "lacks transitionID (group into current stage group)");
                    }

                    this.RunAfter(stage, context);
                    var transit = TransitToStage(stage, result, context);
                    context.PreparingGroupStrategy = transit
                        ? new DisableRecalcPreparingGroupRecalcStrategy()
                        : null;
                    return new NextAction { ContinueToNextStage = true, AfterTransition = transit };
                }
                case StageHandlerAction.GroupTransition:
                {
                    if (result.TransitionID is null)
                    {
                        throw new NullReferenceException(
                            $"Result with action = {StageHandlerAction.GroupTransition} " +
                            "lacks transitionID (stage group)");
                    }

                    this.RunAfter(stage, context);
                    var transit = TransitToStageGroup(stage, result, context);
                    context.PreparingGroupStrategy = transit
                        ? new ExplicitlySelectedPreparingGroupRecalcStrategy()
                        : null;
                    return new NextAction { ContinueToNextStage = true, AfterGroupTransition = transit };
                }
                case StageHandlerAction.NextGroupTransition:
                {
                    var nextGroupIndex =
                        TransitionHelper.TransitToNextGroup(context.WorkflowProcess.Stages, stage.StageGroupID);
                    if (nextGroupIndex == TransitionHelper.NotFound)
                    {
                        return this.StopEntireProcess(result.Action, stage, context);
                    }
                    this.RunAfter(stage, context);
                    var transit = TransitByIndex(stage, nextGroupIndex, result.KeepStageStates ?? false, context);
                    context.PreparingGroupStrategy = new ForwardPreparingGroupRecalcStrategy(this.DbScope, this.Session);
                    return new NextAction { ContinueToNextStage = true, AfterGroupTransition = transit };
                }
                case StageHandlerAction.PreviousGroupTransition:
                {
                    this.RunAfter(stage, context);
                    var prevGroupIndex =
                        TransitionHelper.TransitToPreviousGroup(context.WorkflowProcess.Stages, stage.StageGroupID);
                    if (prevGroupIndex == TransitionHelper.NotFound)
                    {
                        return TransitToCurrentGroup();
                    }
                    var transit = TransitByIndex(stage, prevGroupIndex, result.KeepStageStates ?? false, context);
                    context.PreparingGroupStrategy = new BackwardPreparingGroupRecalcStrategy(this.DbScope, this.Session);
                    return new NextAction { ContinueToNextStage = true, AfterGroupTransition = transit };
                }
                case StageHandlerAction.CurrentGroupTransition:
                {
                    this.RunAfter(stage, context);
                    return TransitToCurrentGroup();
                }
                case StageHandlerAction.SkipProcess:
                case StageHandlerAction.CancelProcess:
                {
                    return this.StopEntireProcess(result.Action, stage, context);
                }
                default:
                    throw new InvalidOperationException($"Stage {stage.Name} ({stage.RowID}) " +
                        $"returns prohibited result {result.Action:G}.");
            }

            NextAction TransitToCurrentGroup()
            {
                var currGroupIndex =
                    TransitionHelper.TransitToStageGroup(context.WorkflowProcess.Stages, stage.StageGroupID);
                if (currGroupIndex == TransitionHelper.NotFound)
                {
                    stage.State = KrStageState.Skipped;
                    return new NextAction { ContinueToNextStage = true };
                }
                var transit = TransitByIndex(stage, currGroupIndex, result.KeepStageStates ?? false, context);
                context.PreparingGroupStrategy = new CurrentPreparingGroupRecalcStrategy();
                return new NextAction { ContinueToNextStage = true, AfterGroupTransition = transit };
            }
            
        }

        protected void RunInternal(IKrProcessRunnerContext context)
        {
            var stage = GetCurrentStage(context);
            var stateHandlerChangedStage = false;
            var globalSignalHandled = false;
            if (context.WorkflowProcess.CurrentApprovalStageRowID.HasValue)
            {
                // Обрабатываем состояния Runner-а
                var stateResult = this.ProcessState(stage, context, out var stageAfterStateProcessing);
                // Важный нюанс: 
                // При наличии состояния, отличного дефолтного, устанавливается режим один запуск за реквест (Scope.DisableMultirunPerRequest)
                // Это позволяет обрабатывать состояния процесса как машину состояний.
                // Однако, для это может препятствовать обработки нескольких сигналов за один запрос,
                // Поэтому не рекомендуется за один раз раннеру отправлять больше одного сигнала (старт процесса + сигнал допустимо)
                if (stateResult.Handled 
                    && !stateResult.ContinueCurrentRun)
                {
                    return;
                }

                stateHandlerChangedStage = stageAfterStateProcessing?.RowID != stage.RowID;

                if (stageAfterStateProcessing?.StageGroupID != stage.StageGroupID
                    || stateResult.ForceStartGroup)
                {
                    stageAfterStateProcessing = this.TryStartStageGroup(stageAfterStateProcessing, stage, context);
                }

                stage = stageAfterStateProcessing;

                if (stage == null)
                {
                    return;
                }

                // Сначала слово дается глобальным сигналам, которые могут перевернуть весь процесс.
                var signalResult = this.ProcessGlobalSignals(stage, context, out var stageAfterSignal);
                if (signalResult.Handled
                    && !signalResult.ContinueCurrentRun)
                {
                    // Глобальный сигнал перехвачен и обработан.
                    // При этом этап не изменился и нам нечего сказать этапу
                    // Или текущий процесс завершен вообще.
                    return;
                }

                globalSignalHandled = signalResult.Handled;

                if (stageAfterSignal?.StageGroupID != stage.StageGroupID
                    || signalResult.ForceStartGroup)
                {
                    stageAfterSignal = this.TryStartStageGroup(stageAfterSignal, stage, context);
                }

                stage = stageAfterSignal;
                
                if (stage == null)
                {
                    return;
                }
            }
            else
            {
                context.WorkflowProcess.CurrentApprovalStageRowID = stage.RowID;
                stage = this.TryStartStageGroup(stage, null, context);
                if (stage == null)
                {
                    // Группа этапов так сильно запустилась, что этапы кончились
                    return;
                }

                if (context.CardID.HasValue
                    && context.CardContext is ICardStoreExtensionContext storeContext)
                {
                    // Для процесса с сигналами при старте проверяем, есть ли запланированные сигналы.
                    // Проверять достаточно только для основного процесса, т.к. только он поддерживает получение сигналов.
                    var hasUnhandledSignals = storeContext.Request.Card
                        .TryGetWorkflowQueue()
                        ?.Items
                        ?.Any(p => !p.Handled
                            && p.Signal.ProcessTypeName == KrConstants.KrProcessName);

                    if (hasUnhandledSignals == true)
                    {
                        // Процессу еще перед запуском дали сигналы.
                        // Такие сигналы должны быть выполнены вместо старта этапа.
                        return;
                    }
                }
            }
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            var result = this.ProcessCurrentStage(stage, context, globalSignalHandled || stateHandlerChangedStage);
            this.AddTrace(context, stage, result);
            var nextAction = this.ProcessStageHandlerResult(stage, result, context);

            while (nextAction.ContinueToNextStage)
            {
                if (!context.ValidationResult.IsSuccessful())
                {
                    return;
                }

                var afterTransition = nextAction.AfterTransition || nextAction.AfterGroupTransition;
                var nextStage = afterTransition
                    ? GetCurrentStage(context)
                    : GetNextStage(context);

                if (nextStage != null)
                {
                    if (nextStage.StageGroupID != stage?.StageGroupID
                        || nextAction.AfterGroupTransition)
                    {
                        this.RunAfterStageGroup(stage, context);
                        nextStage = this.TryStartStageGroup(nextStage, stage, context);
                        if (nextStage != null)
                        {
                            nextAction = this.RunNextStage(context, nextStage);
                        }
                    }
                    else
                    {
                        nextAction = this.RunNextStage(context, nextStage);
                    }
                }

                // За верхний if (nextStage != null) сам nextStage мог изменится
                if (nextStage is null)
                {
                    if (!context.ValidationResult.IsSuccessful())
                    {
                        return;
                    }
                    
                    this.RunAfterStageGroup(stage, context);
                    // Попытка пересчитать конец маршрута, мб там будут новые группы, ранее не подходившие.
                    nextStage = this.TryStartStageGroup(null, stage, context);
                    if (nextStage is null)
                    {
                        // Больше групп нет, отчаиваемся и завершаем процесс.
                        context.WorkflowProcess.CurrentApprovalStageRowID = null;
                        nextAction = new NextAction();
                    }
                    else
                    {
                        nextAction = this.RunNextStage(context, nextStage);
                    }
                }
                stage = nextStage;
            }
        }

        protected NextAction RunNextStage(IKrProcessRunnerContext context, Stage nextStage)
        {
            context.WorkflowProcess.CurrentApprovalStageRowID = nextStage.RowID;
            var nextResult = this.ProcessNextStage(nextStage, context);
            this.AddTrace(context, nextStage, nextResult);
            return this.ProcessStageHandlerResult(nextStage, nextResult, context);
        }

        protected IStateHandlerResult ProcessState(
            Stage stage,
            IKrProcessRunnerContext context,
            out Stage newStage)
        {
            if (this.RunnerMode == KrProcessRunnerMode.Sync)
            {
                newStage = stage;
                return StateHandlerResult.EmptyResult;
            }

            var stateContext = new StateHandlerContext(
                this.Scope.GetRunnerState(context.ProcessInfo.ProcessID), 
                stage,
                this.RunnerMode, 
                context);
            if (stateContext.State != KrProcessState.Default)
            {
                this.Scope.DisableMultirunForRequest(context.ProcessInfo.ProcessID);
            }

            this.Scope.SetDefaultState(context.ProcessInfo.ProcessID);
            var stateResult = this.StateMachine.HandleState(stateContext);
            var lastHandledResult = stateResult;
            newStage = GetCurrentStage(context, nullIsFirst: false);
            // Выполняем состояния, пока они обрабатываются и не требуют вложенного сохранения
            while (stateResult.Handled
                && stateResult.ContinueCurrentRun)
            {
                lastHandledResult = stateResult;
                stateContext = new StateHandlerContext(
                    this.Scope.GetRunnerState(context.ProcessInfo.ProcessID),
                    newStage, 
                    this.RunnerMode,
                    context);
                this.Scope.SetDefaultState(context.ProcessInfo.ProcessID);
                stateResult = this.StateMachine.HandleState(stateContext);
                newStage = GetCurrentStage(context, nullIsFirst: false);
            }

            // Обработчик состояния сменил этап
            if (stage.RowID != newStage?.RowID
                || stateResult.ForceStartGroup)
            {
                this.RunAfter(stage, context);
                if (newStage is null
                    || newStage.StageGroupID != stage.StageGroupID
                    || stateResult.ForceStartGroup)
                {
                    this.RunAfterStageGroup(stage, context);
                }
            }

            return lastHandledResult;
        }

        protected IGlobalSignalHandlerResult ProcessGlobalSignals(
            Stage currentStage,
            IKrProcessRunnerContext context,
            out Stage newStage)
        {
            var signalInfo = context.SignalInfo;
            newStage = currentStage;
            if (signalInfo == null)
            {
                return GlobalSignalHandlerResult.EmptyHandlerResult;
            }

            IGlobalSignalHandlerResult aggregateHandlerResult = GlobalSignalHandlerResult.EmptyHandlerResult;
            var handlers = this.ProcessContainer.ResolveSignal(signalInfo.Signal.Name);
            if (handlers?.Count > 0)
            {
                var ctx = new GlobalSignalHandlerContext(
                    currentStage,
                    context,
                    this.RunnerMode);
                var anyHandled = false;
                var allContinue = true;
                var forceStartGroup = false;
                foreach (var handler in handlers)
                {
                    var res = handler.Handle(ctx);
                    anyHandled |= res.Handled;
                    allContinue &= res.ContinueCurrentRun;
                    forceStartGroup |= res.ForceStartGroup;
                }
                aggregateHandlerResult = new GlobalSignalHandlerResult(anyHandled, allContinue, forceStartGroup);
            }

            if (currentStage.RowID == context.WorkflowProcess.CurrentApprovalStageRowID
                && !aggregateHandlerResult.ForceStartGroup)
            {
                return aggregateHandlerResult;
            }
            this.RunAfter(currentStage, context);
            newStage = GetCurrentStage(context, nullIsFirst: false);
            if (newStage?.StageGroupID != currentStage.StageGroupID
                || aggregateHandlerResult.ForceStartGroup)
            {
                this.RunAfterStageGroup(currentStage, context);
            }

            return aggregateHandlerResult;
        }

        protected static Stage GetCurrentStage(IKrProcessRunnerContext context, bool nullIsFirst = true)
        {
            var currentApprovalStageRowID = context.WorkflowProcess.CurrentApprovalStageRowID;
            if (currentApprovalStageRowID.HasValue)
            {
                return context.WorkflowProcess.Stages.FirstOrDefault(p => p.RowID == currentApprovalStageRowID);
            }

            if (nullIsFirst && context.WorkflowProcess.Stages.Count > 0)
            {
                return context.WorkflowProcess.Stages[0];
            }

            return null;
        }

        protected StageHandlerResult ProcessCurrentStage(
            Stage currentStage,
            IKrProcessRunnerContext context,
            bool forceStartProcess)
        {
            if (currentStage == null
                || !currentStage.StageTypeID.HasValue)
            {
                KrErrorHelper.WarnStageTypeIsNull(context, currentStage);
                return StageHandlerResult.SkipResult;
            }

            var handler = this.ProcessContainer.ResolveHandler(currentStage.StageTypeID.Value);
            if (handler == null)
            {
                KrErrorHelper.WarnStageHandlerIsNull(context, currentStage);
                return StageHandlerResult.SkipResult;
            }
            this.AssertRunnerMode(currentStage.StageTypeID.Value);

            var stageContext = new StageTypeHandlerContext(context, currentStage, this.RunnerMode, null);

            if (forceStartProcess)
            {
                return this.TryStartStage(handler, stageContext, currentStage, context);
            }

            switch (context.InitiationCause)
            {
                case KrProcessRunnerInitiationCause.StartProcess:
                    return this.TryStartStage(handler, stageContext, currentStage, context);
                case KrProcessRunnerInitiationCause.CompleteTask:
                    return handler.HandleTaskCompletion(stageContext);
                case KrProcessRunnerInitiationCause.ReinstateTask:
                    return handler.HandleTaskReinstate(stageContext);
                case KrProcessRunnerInitiationCause.Signal:
                    return handler.HandleSignal(stageContext);
                case KrProcessRunnerInitiationCause.InMemoryLaunching:
                    return this.TryStartStage(handler, stageContext, currentStage, context);
                case KrProcessRunnerInitiationCause.Resurrection:
                    return this.TryResurrectStage(handler, stageContext);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected static Stage GetNextStage(IKrProcessRunnerContext context)
        {
            var currentApprovalStageRowID = context.WorkflowProcess.CurrentApprovalStageRowID;
            if (currentApprovalStageRowID is null)
            {
                // Если нет текущего этапа, то и следующего за ним быть не может
                return null;
            }
            var currentStageIndex = context.WorkflowProcess.Stages.IndexOf(p => p.RowID == currentApprovalStageRowID);
            if (currentStageIndex == -1)
            {
                // Если нет текущего этапа, то и следующего за ним быть не может
                return null;
            }
            return currentStageIndex + 1 < context.WorkflowProcess.Stages.Count
                ? context.WorkflowProcess.Stages[currentStageIndex + 1]
                : null;
        }

        protected StageHandlerResult ProcessNextStage(
            Stage stage,
            IKrProcessRunnerContext context)
        {
            context.WorkflowProcess.CurrentApprovalStageRowID = stage.RowID;
            if (!stage.StageTypeID.HasValue)
            {
                KrErrorHelper.WarnStageTypeIsNull(context, stage);
                return StageHandlerResult.SkipResult;
            }
            var handler = this.ProcessContainer.ResolveHandler(stage.StageTypeID.Value);
            var stageContext = new StageTypeHandlerContext(context, stage, this.RunnerMode, null);
            if (handler == null)
            {
                KrErrorHelper.WarnStageHandlerIsNull(context, stage);
                return StageHandlerResult.SkipResult;
            }
            this.AssertRunnerMode(stage.StageTypeID.Value);

            return this.TryStartStage(handler, stageContext, stage, context);
        }

        protected IKrExecutionUnit CreateRuntimeStageInstance(
            Stage currentStage,
            IKrProcessRunnerContext context)
        {
            var cachedExecutionUnits = context.ExecutionUnitCache;
            IKrExecutionUnit unit;
            IKrScript instance;
            if (cachedExecutionUnits.TryGetValue(currentStage.RowID, out var cached))
            {
                // Взятие объекта из кэша с проинициализированными общими зависимостями
                unit = cached;
                instance = unit.Instance;
            }
            else
            {
                // Создание нового объекта с инициализацией общих зависимостей
                if (!this.ProcessCache.GetAllRuntimeStages().TryGetValue(currentStage.ID, out var runtimeStage))
                {
                    return null;
                }
                instance = this.CreateInstance(currentStage.ID, SourceIdentifiers.StageAlias, context.ValidationResult);
                unit = new KrExecutionUnit(runtimeStage, instance);
                cachedExecutionUnits[currentStage.RowID] = unit;

                instance.MainCardAccessStrategy = context.MainCardAccessStrategy;
                instance.CardID = context.CardID ?? Guid.Empty;
                instance.CardTypeID = context.CardTypeID ?? Guid.Empty;
                instance.CardTypeName = context.CardTypeName;
                instance.CardTypeCaption = context.CardTypeCaption;
                instance.DocTypeID = context.DocTypeID ?? Guid.Empty;
                if (context.KrComponents.HasValue)
                {
                    instance.KrComponents = context.KrComponents.Value;
                }

                instance.WorkflowProcessInfo = context.ProcessInfo;
                instance.ProcessID = context.ProcessInfo?.ProcessID;
                instance.ProcessTypeName = context.ProcessInfo?.ProcessTypeName;
                instance.InitiationCause = context.InitiationCause;
                instance.ContextualSatellite = context.ContextualSatellite;
                instance.ProcessHolderSatellite = context.ProcessHolderSatellite;
                instance.SecondaryProcess = context.SecondaryProcess;
                instance.CardContext = context.CardContext;
                instance.ValidationResult = context.ValidationResult;
                instance.TaskHistoryResolver = context.TaskHistoryResolver;
                instance.Session = this.Session;
                instance.DbScope = this.DbScope;
                instance.UnityContainer = this.UnityContainer;
                instance.CardMetadata = this.CardMetadata;
                instance.KrScope = this.Scope;
                instance.CardCache = this.CardCache;
                instance.KrTypesCache = this.TypesCache;
                instance.StageSerializer = this.StageSerializer;
            }
            if (currentStage.TemplateID == null
                || !this.ProcessCache.GetAllStageTemplates().TryGetValue(currentStage.TemplateID.Value, out var stageTemplate))
            {
                return null;
            }

            // Информация о конкретных этапах и шаблонах не является общей, 
            // поэтому инициализируется всегда
            instance.StageGroupID = currentStage.StageGroupID;
            instance.StageGroupName = currentStage.StageGroupName;
            instance.StageGroupOrder = currentStage.StageGroupOrder;
            instance.TemplateID = currentStage.TemplateID ?? Guid.Empty;
            instance.TemplateName = currentStage.TemplateName;
            instance.Order = stageTemplate?.Order ?? -1;
            instance.Position = stageTemplate?.Position ?? GroupPosition.Unspecified;
            instance.CanChangeOrder = stageTemplate?.CanChangeOrder ?? true;
            instance.IsStagesReadonly = stageTemplate?.IsStagesReadonly ?? true;

            // На данном этапе нет контейнера, способного пересчитывать положения этапов.
            instance.StagesContainer = null;
            instance.WorkflowProcess = context.WorkflowProcess;
            instance.Stage = currentStage;

            // Необходимо сбросить информацию о переключении контекста
            instance.DifferentContextCardID = null;
            instance.DifferentContextWholeCurrentGroup = false;
            instance.DifferentContextProcessInfo = null;
            instance.DifferentContextSetupScriptType = null;
            
            return unit;
        }

        protected IKrExecutionUnit CreateStageGroupInstance(
            Stage stage,
            IKrProcessRunnerContext context)
        {
            if (!this.ProcessCache.GetAllStageGroups().TryGetValue(stage.StageGroupID, out var stageGroup))
            {
                return null;
            }

            var cachedExecutionUnits = context.ExecutionUnitCache;
            IKrExecutionUnit unit;
            IKrScript instance;
            if (cachedExecutionUnits.TryGetValue(stage.StageGroupID, out var cached))
            {
                // Взятие объекта из кэша с проинициализированными общими зависимостями
                unit = cached;
                instance = unit.Instance;
            }
            else
            {
                instance = this.CreateInstance(stage.StageGroupID, SourceIdentifiers.GroupAlias, context.ValidationResult);
                unit = new KrExecutionUnit(stageGroup, instance);
                cachedExecutionUnits[stage.StageGroupID] = unit;

                instance.MainCardAccessStrategy = context.MainCardAccessStrategy;
                instance.CardID = context.CardID ?? Guid.Empty;
                instance.CardTypeID = context.CardTypeID ?? Guid.Empty;
                instance.CardTypeName = context.CardTypeName;
                instance.CardTypeCaption = context.CardTypeCaption;
                instance.DocTypeID = context.DocTypeID ?? Guid.Empty;
                if (context.KrComponents.HasValue)
                {
                    instance.KrComponents = context.KrComponents.Value;
                }

                instance.WorkflowProcessInfo = context.ProcessInfo;

                instance.ProcessID = context.ProcessInfo?.ProcessID;
                instance.ProcessTypeName = context.ProcessInfo?.ProcessTypeName;
                instance.InitiationCause = context.InitiationCause;
                instance.ContextualSatellite = context.ContextualSatellite;
                instance.ProcessHolderSatellite = context.ProcessHolderSatellite;
                instance.SecondaryProcess = context.SecondaryProcess;
                instance.CardContext = context.CardContext;
                instance.ValidationResult = context.ValidationResult;
                instance.TaskHistoryResolver = context.TaskHistoryResolver;
                instance.Session = this.Session;
                instance.DbScope = this.DbScope;
                instance.UnityContainer = this.UnityContainer;
                instance.CardMetadata = this.CardMetadata;
                instance.KrScope = this.Scope;
            }

            instance.StageGroupID = stageGroup.ID;
            instance.StageGroupName = stageGroup.Name;
            instance.StageGroupOrder = stageGroup.Order;

            // На данном этапе нет контейнера, способного пересчитывать положения этапов.
            instance.StagesContainer = null;
            instance.WorkflowProcess = context.WorkflowProcess;

            instance.DifferentContextCardID = null;
            instance.DifferentContextWholeCurrentGroup = false;
            instance.DifferentContextProcessInfo = null;
            instance.DifferentContextSetupScriptType = null;

            return unit;
        }

        protected StageHandlerResult TryStartStage(
            IStageTypeHandler handler,
            IStageTypeHandlerContext handlerContext,
            Stage stage,
            IKrProcessRunnerContext context)
        {
            if (stage.StageTypeID is null)
            {
                throw new ProcessRunnerInterruptedException();
            }
            var descriptor = this.ProcessContainer.GetHandlerDescriptor(stage.StageTypeID.Value);
            
            // На старте удаляем старые переопределения
            HandlerHelper.RemoveTaskHistoryGroupOverride(stage);
            context.PreparingGroupStrategy = null;

            handler.BeforeInitialization(handlerContext);

            if (!context.ValidationResult.IsSuccessful())
            {
                return StageHandlerResult.EmptyResult;
            }

            stage.State = KrStageState.Active;
            if (!stage.BasedOnTemplateStage)
            {
                CheckTime(stage, descriptor);
                CheckPerformers(stage, descriptor);
                return handler.HandleStageStart(handlerContext);
            }

            var unit = this.CreateRuntimeStageInstance(stage, context);
            if (unit is null)
            {
                // Не получилось создать элемент выполнения этапа.
                // Скорее всего, какой-то связанный элемнет удален, поэтому просто выполним хендлер
                CheckTime(stage, descriptor);
                CheckPerformers(stage, descriptor);
                return handler.HandleStageStart(handlerContext);
            }
            // Переносим переключение контекста из After предыдущего этапа.
            var changeContext = false;
            if (stage.ChangeContextToCardID.HasValue)
            {
                unit.Instance.DifferentContextCardID = stage.ChangeContextToCardID.Value;
                unit.Instance.DifferentContextWholeCurrentGroup = stage.ChangeContextWholeGroupToDifferentCard;
                unit.Instance.DifferentContextProcessInfo = stage.ChangeContextProcessInfo;
                unit.Instance.DifferentContextSetupScriptType = KrScriptType.Before;
                
                // При этом необходима очистка этапа.
                stage.ChangeContextToCardID = null;
                stage.ChangeContextWholeGroupToDifferentCard = false;
                stage.ChangeContextProcessInfo = null;
                changeContext = true;
            }

            if (!changeContext)
            {
                var condition = !stage.Skip && this.SafeRun(context, u => this.RunConditions(u, context), unit);
                if (!condition)
                {
                    context.SkippedStagesByCondition.Add(stage.ID);
                    return StageHandlerResult.SkipResult;
                }

                this.RecalcSqlRoles(stage, context);
                this.SafeRun(context, RunBefore, unit);
                changeContext = unit.Instance.DifferentContextCardID.HasValue
                    && unit.Instance.DifferentContextCardID != context.CardID;
            }
            // Место подмены контекста.
            if (changeContext)
            {
                var nextGroupID = Guid.Empty;
                var stageSubset = unit.Instance.DifferentContextWholeCurrentGroup
                    ? GetSubsequentStages(context, stage, out nextGroupID)
                    : new SealableObjectList<Stage> { new Stage(stage) };

                this.RunInDifferentCardContext(
                    context,
                    unit.Instance.DifferentContextCardID.Value, 
                    stageSubset, 
                    unit.Instance.DifferentContextProcessInfo);
                if (stageSubset.Count <= 1)
                {
                    return StageHandlerResult.SkipResult;
                }

                if (nextGroupID != Guid.Empty)
                {
                    return StageHandlerResult.GroupTransition(nextGroupID);
                }
                
                return StageHandlerResult.SkipProcessResult;
            }

            CheckTime(stage, descriptor);
            CheckPerformers(stage, descriptor);

            if (!context.ValidationResult.IsSuccessful())
            {
                return StageHandlerResult.EmptyResult;
            }

            return handler.HandleStageStart(handlerContext);
        }

        protected StageHandlerResult TryResurrectStage(
            IStageTypeHandler handler,
            IStageTypeHandlerContext handlerContext)
        {
            return handler.HandleResurrection(handlerContext);
        }

        protected void RunAfter(
            Stage stage,
            IKrProcessRunnerContext context)
        {
            if (context.SkippedStagesByCondition.Contains(stage.ID))
            {
                return;
            }

            if (stage.BasedOnTemplateStage)
            {
                var unit = this.CreateRuntimeStageInstance(stage, context);
                if (unit is null)
                {
                    // Не получилось создать элемент выполнения этапа.
                    // Пропускаем весь этап
                    return;
                }
                this.SafeRun(context, RunAfter, unit);
                if (unit.Instance.DifferentContextCardID.HasValue
                    && unit.Instance.DifferentContextSetupScriptType == KrScriptType.After)
                {
                    var nextStage = GetNextStage(context);
                    if (nextStage != null)
                    {
                        nextStage.ChangeContextToCardID = unit.Instance.DifferentContextCardID;
                        nextStage.ChangeContextWholeGroupToDifferentCard = unit.Instance.DifferentContextWholeCurrentGroup;
                        nextStage.ChangeContextProcessInfo = unit.Instance.DifferentContextProcessInfo;
                    }
                }
            }
            
            if (stage.StageTypeID.HasValue)
            {
                var handler = this.ProcessContainer.ResolveHandler(stage.StageTypeID.Value);
                if (handler != null)
                {
                    var stageContext = new StageTypeHandlerContext(context, stage, this.RunnerMode, null);
                    handler.AfterPostprocessing(stageContext);
                }
            }
        }

        protected Stage TryStartStageGroup(
            Stage stage,
            Stage previousStage,
            IKrProcessRunnerContext context)
        {
            if (context.IgnoreGroupScripts)
            {
                return stage;
            }

            var preparingStrategy = context.PreparingGroupStrategy 
                ?? context.DefaultPreparingGroupStrategyFunc?.Invoke()
                ?? throw new NullReferenceException($"{context.DefaultPreparingGroupStrategyFunc} doesn't set.");
            context.PreparingGroupStrategy = null;
            preparingStrategy.Apply(context, stage, previousStage);

            if (preparingStrategy.ExecutionUnits.Count != 0)
            {
                this.PartialGroupRecalc(preparingStrategy, context);
            }
            
            var nextStage = preparingStrategy.GetSuitableStage(context.WorkflowProcess.Stages);
            if (nextStage is null)
            {
                context.WorkflowProcess.CurrentApprovalStageRowID = null;
                return null;
            }

            var unit = this.CreateStageGroupInstance(nextStage, context);

            context.WorkflowProcess.CurrentApprovalStageRowID = nextStage.RowID;
            if (this.SafeRun(context, u => this.RunConditions(u, context), unit))
            {
                this.SafeRun(context, RunBefore, unit);
                return nextStage;
            }

            // Пропускаем все этапы внутри
            context.SkippedGroupsByCondition.Add(nextStage.StageGroupID);
            var stages = context.WorkflowProcess.Stages;
            var currentIdx = stages.IndexOf(p => p.RowID == nextStage.RowID);
            var count = stages.Count;
            while (currentIdx < count && stages[currentIdx].StageGroupID == nextStage.StageGroupID)
            {
                stages[currentIdx].State = KrStageState.Skipped;
                currentIdx++;
            }
            // Пометим следующий этап как текущий, чтобы с него потом стартануть
            if (currentIdx < count)
            {
                context.WorkflowProcess.CurrentApprovalStageRowID = stages[currentIdx].RowID;
                return stages[currentIdx];
            }

            return null;
        }

        protected void PartialGroupRecalc(
            IPreparingGroupRecalcStrategy preparingStrategy,
            IKrProcessRunnerContext context)
        {
            if (context.ProcessHolderSatellite != null)
            {
                NestedStagesCleaner.ClearGroup(
                    context.ProcessHolderSatellite, 
                    context.WorkflowProcess.NestedProcessID,
                    preparingStrategy.ExecutionUnits
                );
            }
            var executor = this.ExecutorFunc();
            var ctx = new KrExecutionContext(
                context.CardContext,
                context.MainCardAccessStrategy,
                context.CardID,
                context.CardTypeID,
                context.CardTypeName,
                context.CardTypeCaption,
                context.DocTypeID,
                context.KrComponents,
                context.WorkflowProcess,
                compilationResult: null,
                executionUnits: preparingStrategy.ExecutionUnits,
                secondaryProcess: context.SecondaryProcess);
            var result = executor.Execute(ctx);
            context.ValidationResult.Add(result.Result);
        }

        protected void RunAfterStageGroup(
            Stage stage,
            IKrProcessRunnerContext context)
        {
            if (context.IgnoreGroupScripts
                || context.SkippedGroupsByCondition.Contains(stage.StageGroupID))
            {
                return;
            }

            var unit = this.CreateStageGroupInstance(stage, context);
            this.SafeRun(context, RunAfter, unit);
        }

        protected static bool TransitToStage(
            Stage currentStage,
            StageHandlerResult stageHandlerResult,
            IKrProcessRunnerContext context)
        {
            var transitTo = stageHandlerResult.TransitionID;
            var keepStageStates = stageHandlerResult.KeepStageStates ?? false;
            return Transit(currentStage, keepStageStates, s => s.ID == transitTo, context);
        }

        protected static bool TransitToStageGroup(
            Stage currentStage,
            StageHandlerResult stageHandlerResult,
            IKrProcessRunnerContext context)
        {
            var transitTo = stageHandlerResult.TransitionID;
            var keepStageStates = stageHandlerResult.KeepStageStates ?? false;
            return Transit(currentStage, keepStageStates, s => s.StageGroupID == transitTo, context);
        }

        protected static bool TransitByIndex(
            Stage currentStage,
            int index,
            bool keepStageStates,
            IKrProcessRunnerContext context)
        {
            if (!(0 <= index && index < context.WorkflowProcess.Stages.Count))
            {
                return false;
            }

            var transitTo = context.WorkflowProcess.Stages[index].ID;
            return Transit(currentStage, keepStageStates, s => s.ID == transitTo, context);
        }

        protected static bool Transit(
            Stage currentStage,
            bool keepStageStates,
            Func<Stage, bool> transitionPredicate,
            IKrProcessRunnerContext context)
        {
            var stageIndex = 0;
            var stages = context.WorkflowProcess.Stages;
            var cnt = stages.Count;

            foreach (var stage in stages)
            {
                if (transitionPredicate(stage))
                {
                    context.WorkflowProcess.CurrentApprovalStageRowID = stage.RowID;
                    currentStage.State = KrStageState.Completed;
                    break;
                }

                stageIndex++;
            }
            // Некуда переходить или не надо менять состояние
            if (stageIndex == cnt)
            {
                currentStage.State = KrStageState.Skipped;
                return false;
            }

            if (!keepStageStates)
            {
                TransitionHelper.ChangeStatesTransition(
                    stages,
                    currentStage.RowID,
                    // ReSharper disable once PossibleInvalidOperationException
                    context.WorkflowProcess.CurrentApprovalStageRowID.Value,
                    context.ProcessHolderSatellite);
            }

            return true;
        }

        protected NextAction StopEntireProcess(
            StageHandlerAction action,
            Stage currentStage,
            IKrProcessRunnerContext context)
        {
            var skip = action == StageHandlerAction.SkipProcess
                || action == StageHandlerAction.NextGroupTransition;

            context.WorkflowProcess.CurrentApprovalStageRowID = null;
            // В случае перехода на следующую группу дадим возможность еще досчитать следующую группу.
            context.PreparingGroupStrategy = action == StageHandlerAction.NextGroupTransition
                ? (IPreparingGroupRecalcStrategy)new ForwardPreparingGroupRecalcStrategy(this.DbScope, this.Session)
                : new DisableRecalcPreparingGroupRecalcStrategy();
            if (skip)
            {
                TransitionHelper.SetSkipStateToSubsequentStages(
                    currentStage,
                    context.WorkflowProcess.Stages,
                    context.ProcessHolderSatellite);
            }
            else
            {
                TransitionHelper.SetInactiveStateToAllStages(
                    context.WorkflowProcess.Stages,
                    context.ProcessHolderSatellite);
            }
            
            // Переход к следующему этапу даст понять раннеру, что следующий этап == null и пора выходить            
            return new NextAction { ContinueToNextStage = true};
        }

        protected static bool RunBefore(IKrExecutionUnit unit)
        {
            try
            {
                unit.Instance.RunBefore();
                return true;
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.RuntimeError(unit, e.Message);
                throw new ScriptExecutionException(text, unit.RuntimeSources.RuntimeSourceBefore, e);
            }
        }

        protected bool RunConditions(IKrExecutionUnit unit, IKrProcessRunnerContext context)
        {
            return ExecCondition(unit) && this.ExecSQL(unit, context);
        }

        protected static bool ExecCondition(IKrExecutionUnit unit)
        {
            try
            {
                return unit.Instance.RunCondition();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.RuntimeError(unit, e.Message);
                throw new ScriptExecutionException(text, unit.RuntimeSources.RuntimeSourceCondition, e);
            }
        }

        protected bool ExecSQL(IKrExecutionUnit unit, IKrProcessRunnerContext context)
        {
            var sqlExecutionContext = new KrSqlExecutorContext(
                unit.RuntimeSources.RuntimeSqlCondition,
                context.ValidationResult,
                (ctx, txt, args) => KrErrorHelper.SqlRuntimeError(unit, txt, args),
                unit,
                context.SecondaryProcess,
                context.CardID,
                context.CardTypeID,
                context.DocTypeID,
                context.WorkflowProcess.State);
            return this.SqlExecutor.ExecuteCondition(sqlExecutionContext);
        }

        protected static bool RunAfter(IKrExecutionUnit unit)
        {
            try
            {
                unit.Instance.RunAfter();
                return true;
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.RuntimeError(unit, e.Message);
                throw new ScriptExecutionException(text, unit.RuntimeSources.RuntimeSourceAfter, e);
            }
        }

        protected bool SafeRun(
            IKrProcessRunnerContext context,
            Func<IKrExecutionUnit, bool> action,
            IKrExecutionUnit unit)
        {
            if (unit is null)
            {
                return false;
            }

            try
            {
                return action(unit);
            }
            catch (ExecutionExceptionBase eeb)
            {
                var validator = ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(eeb.ErrorMessageText, eeb.SourceText);
                if (eeb.InnerException != null)
                {
                    validator.ErrorException(eeb.InnerException);
                }
                validator.End();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.UnexpectedError(unit);

                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorText(text)
                    .ErrorException(e)
                    .End();
            }

            return false;
        }

        protected static void SetStageFinalState(
            Stage stage,
            StageHandlerResult result)
        {
            switch (result.Action)
            {
                case StageHandlerAction.None:
                case StageHandlerAction.Skip:
                    if (stage.State == KrStageState.Active)
                    {
                        stage.State = KrStageState.Skipped;
                    }
                    break;
                case StageHandlerAction.Complete:
                    if (stage.State == KrStageState.Active)
                    {
                        stage.State = KrStageState.Completed;
                    }
                    break;
                case StageHandlerAction.Transition:
                case StageHandlerAction.GroupTransition:
                case StageHandlerAction.InProgress:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected IKrScript CreateInstance(
            Guid id,
            string alias,
            IValidationResultBuilder validationResult)
        {
            var compilationResult = this.CompilationCache.Get();
            if (compilationResult.Result.Assembly == null)
            {
                logger.LogResult(compilationResult.ValidationResult);
                validationResult.Add(compilationResult.ToMissingAssemblyResult());
                throw new ProcessRunnerInterruptedException();
            }

            try
            {
                return compilationResult.CreateInstance(
                    SourceIdentifiers.KrRuntimeClass,
                    alias,
                    id);
            }
            catch (KeyNotFoundException)
            {
                validationResult.AddError(
                    this,
                    string.Format(LocalizationManager.GetString("KrProcess_ClassMissed"), $"{SourceIdentifiers.KrRuntimeClass}_{id:N}"));
                throw new ProcessRunnerInterruptedException();
            }
        }

        protected static SealableObjectList<Stage> GetSubsequentStages(
            IKrProcessRunnerContext context,
            Stage currentStage,
            out Guid nextGroupID)
        {
            nextGroupID = Guid.Empty;
            var allStages = context.WorkflowProcess.Stages;
            var currentStageIndex = allStages.IndexOf(p => p.RowID == currentStage.RowID);
            if (currentStageIndex == -1)
            {
                return new SealableObjectList<Stage> { new Stage(currentStage) };
            }
            var subset = new SealableObjectList<Stage>(allStages.Count);
            Stage stage;
            var i = currentStageIndex;
            for (;
                i < allStages.Count && (stage = allStages[i]).StageGroupID == currentStage.StageGroupID;
                i++)
            {
                subset.Add(new Stage(stage));
            }

            if (i != allStages.Count)
            {
                nextGroupID = allStages[i].StageGroupID;
            }
            return subset;
        }

        protected void RunInDifferentCardContext(
            IKrProcessRunnerContext context,
            Guid differentContextCardID,
            SealableObjectList<Stage> stageSubset,
            IDictionary<string, object> processInfo = null)
        {
            var db = this.DbScope.Db;
            db.SetCommand(this.DbScope.BuilderFactory
                    .Select()
                        .C("dci", "CardTypeID", "DocTypeID")
                        .C("t", "Name", "Caption")
                    .From("DocumentCommonInfo", "dci").NoLock()
                    .InnerJoin("Types", "t").On().C("t", "ID").Equals().C("dci", "CardTypeID")
                    .Where().C("dci", "ID").Equals().P("ID")
                    .Build(),
                    db.Parameter("ID", differentContextCardID));
            Guid typeID;
            string typeName;
            string typeCaption;
            Guid? docTypeID;
            using (var reader = db.ExecuteReader())
            {
                if (!reader.Read())
                {
                    context.ValidationResult.AddError(
                        this, 
                        string.Format(
                            LocalizationManager.Localize("$KrProcessRunner_DifferentContextCardDoesnotExist"), 
                            differentContextCardID));
                    return;
                }

                typeID = reader.GetGuid(0);
                docTypeID = reader.GetNullableGuid(1);
                typeName = reader.GetString(2);
                typeCaption = reader.GetString(3);
            }
            
            // Здесь обращаемся к другой карточке, значит лок нужен при чтении в ApplyChanges
            using var level = this.Scope.EnterNewLevel(context.ValidationResult);
            var satellite = this.Scope.GetKrSatellite(differentContextCardID);
            var processHolder = new ProcessHolder
            {
                Persistent = false,
                MainProcessType = KrConstants.KrSecondaryProcessName,
                ProcessHolderID = Guid.NewGuid(),
                PrimaryProcessCommonInfo = this.ObjectModelMapper.GetMainProcessCommonInfo(satellite, false),
                MainProcessCommonInfo = new MainProcessCommonInfo(null, processInfo, null, null, null, null, 0),
            };
            processInfo = processInfo ?? new Dictionary<string, object>();
            var workflowProcess = new WorkflowProcess(
                processInfo,
                processInfo,
                stageSubset,
                saveInitialStages: true,
                nestedProcessID: null);
            processHolder.MainWorkflowProcess = workflowProcess;
            this.ObjectModelMapper.FillWorkflowProcessFromPci(
                workflowProcess, 
                processHolder.MainProcessCommonInfo, 
                processHolder.PrimaryProcessCommonInfo);
                
            var mainCardLoadingStrategy = new KrScopeMainCardAccessStrategy(differentContextCardID, this.Scope);
            var historyManager =
                context.TaskHistoryResolver?.TaskHistoryManager ?? this.UnityContainer.Resolve<ICardTaskHistoryManager>();
            var taskHistoryResolver = new KrTaskHistoryResolver(
                mainCardLoadingStrategy,
                context.CardContext,
                context.ValidationResult,
                historyManager);

            var components = KrComponentsHelper.GetKrComponents(typeID, docTypeID, this.TypesCache);
            var runnerContext = new KrProcessRunnerContext(
                workflowAPI: null,
                taskHistoryResolver: taskHistoryResolver,
                mainCardAccessStrategy: mainCardLoadingStrategy,
                cardID: differentContextCardID,
                cardTypeID: typeID,
                cardTypeName: typeName,
                cardTypeCaption: typeCaption,
                docTypeID: docTypeID,
                krComponents: components,
                contextualSatellite: satellite,    // Контекстуальный сателлит есть, т.к. процесс в рамках карточки работает
                processHolderSatellite: null,      // Процессного сателлита нет, процесс синхронный весь в памяти
                workflowProcess: workflowProcess,
                processHolder: processHolder,
                processInfo: null,
                validationResult: context.ValidationResult,
                cardContext: context.CardContext,
                secondaryProcess: null,
                parentProcessID: null,
                parentProcessTypeName: null,
                ignoreGroupScripts: true,
                defaultPreparingGroupStrategyFunc: () => new DisableRecalcPreparingGroupRecalcStrategy());
            var syncRunner = this.RunnerProvider.GetRunner(KrProcessRunnerNames.Sync);
            syncRunner.Run(runnerContext);
                
            this.ObjectModelMapper.ObjectModelToPci(
                processHolder.MainWorkflowProcess,
                processHolder.MainProcessCommonInfo,
                processHolder.MainProcessCommonInfo,
                processHolder.PrimaryProcessCommonInfo);
            this.ObjectModelMapper.SetMainProcessCommonInfo(
                differentContextCardID, satellite, processHolder.PrimaryProcessCommonInfo);
            level.ApplyChanges(differentContextCardID, context.ValidationResult);
                
            // Заполнять ValidationResult и ClientCommands не нужно, т.к. это не верхний уровень,
            // Раннер может быть запущен только в скопе.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AddTrace(IKrProcessRunnerContext context, Stage stage, StageHandlerResult result) =>
            this.Scope.TryAddToTrace(new KrProcessTraceItem(stage, result, context.CardID));

        protected void AssertRunnerMode(
            Guid stageTypeID)
        {
            var descriptor = this.ProcessContainer.GetHandlerDescriptor(stageTypeID);
            if (!descriptor.SupportedModes.Contains(this.RunnerMode))
            {
                // Тип этапа "имя" не поддерживает синхронный/асинхронный режим. Поддерживаемые режимы: ...
                var text = LocalizationManager.Format(
                    "$KrProcess_StageTypeDoesNotSupportMode",
                    descriptor.Caption,
                    this.RunnerMode.GetCaption(),
                    string.Join(", ", descriptor.SupportedModes.Select(p => p.GetCaption())));
                throw new ProcessRunnerInterruptedException(text);
            }
        }

        protected void RecalcSqlRoles(
            Stage stage,
            IKrProcessRunnerContext context)
        {
            if (stage.RowChanged)
            {
                return;
            }

            try
            {
                var oldRowIDs = new List<Guid>(stage.Performers.Count);
                stage.Performers.RemoveAll(p =>
                {
                    if (!p.IsSql)
                    {
                        return false;
                    }

                    oldRowIDs.Add(p.RowID);
                    return true;
                });

                if (!string.IsNullOrWhiteSpace(stage.SqlPerformers))
                {
                    this.RecalcRole(stage, oldRowIDs, context);
                }
            }
            catch (ExecutionExceptionBase eeb)
            {
                var validator = ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(eeb.ErrorMessageText, eeb.SourceText);
                if (eeb.InnerException != null)
                {
                    validator.ErrorException(eeb.InnerException);
                }
                validator.End();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.UnexpectedError(stage);

                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorText(text)
                    .ErrorException(e)
                    .End();
            }
            
        }

        protected void RecalcRole(
            Stage stage, 
            List<Guid> oldRowIDs,
            IKrProcessRunnerContext context)
        {
            StageTypeDescriptor descriptor;
            if (!stage.StageTypeID.HasValue
                || (descriptor = this.ProcessContainer.GetHandlerDescriptor(stage.StageTypeID.Value)) is null)
            {
                return;
            }
            var sqlPreprocessorContext = new KrSqlExecutorContext(
                stage,
                context.ValidationResult,
                (ctx, txt, args) => KrErrorHelper.SqlPerformersError(ctx.StageName, ctx.TemplateName, ctx.GroupName, ctx.SecondaryProcess?.Name, txt, args),
                context.SecondaryProcess,
                context.CardID,
                context.CardTypeID,
                context.DocTypeID,
                context.WorkflowProcess.State);
            var newPerformers = this.SqlExecutor.ExecutePerformers(sqlPreprocessorContext);
            if (newPerformers is null)
            {
                return;
            }
            switch (descriptor.PerformerUsageMode)
            {
                case PerformerUsageMode.Single:
                    UpdateSingleSqlPerformer(stage, newPerformers);
                    break;
                case PerformerUsageMode.Multiple:
                    UpdateSqlPerformers(stage, oldRowIDs, newPerformers);
                    break;
            }
        }

        protected static void UpdateSingleSqlPerformer(
            Stage stage,
            List<Performer> newPerformers)
        {
            if (newPerformers.Count != 0)
            {
                var p = newPerformers[0];
                stage.Performer = new Performer(p.PerformerID, p.PerformerName);
            }
        }

        protected static void UpdateSqlPerformers(
            Stage stage,
            List<Guid> oldRowIDs,
            List<Performer> newPerformers)
        {
            var insertionIndex = stage.SqlPerformersIndex is null
                || stage.SqlPerformersIndex > stage.Performers.Count
                ? stage.Performers.Count
                : stage.SqlPerformersIndex.Value;

            var idsEnum = oldRowIDs.GetEnumerator();
            try
            {
                foreach (var perf in newPerformers)
                {
                    var id = idsEnum.MoveNext()
                        ? idsEnum.Current
                        : Guid.NewGuid();
                    // Инкапсуляция разбивается о скалы суровой реальности
                    perf.GetStorage()[nameof(Performer.RowID)] = id;
                    stage.Performers.Insert(insertionIndex++, perf);
                }
            }
            finally
            {
                idsEnum.Dispose();
            }
        }

        private static void CheckTime(
            Stage stage,
            StageTypeDescriptor descriptor)
        {
            if (descriptor.UseTimeLimit
                && descriptor.UsePlanned)
            {
                if (stage.TimeLimit is null
                    && stage.Planned is null)
                {
                    KrErrorHelper.PlannedNotSpecified(stage);
                }
            }
            else if (descriptor.UsePlanned
                && stage.Planned is null)
            {
                KrErrorHelper.PlannedNotSpecified(stage);
            }
            else if (descriptor.UseTimeLimit
                && stage.TimeLimit is null)
            {
                KrErrorHelper.TimeLimitNotSpecified(stage);
            }
        }
        
        private static void CheckPerformers(
            Stage stage,
            StageTypeDescriptor descriptor)
        {
            if (descriptor.PerformerIsRequired)
            {
                switch (descriptor.PerformerUsageMode)
                {
                    case PerformerUsageMode.Single:
                        if (stage.Performer is null)
                        {
                            KrErrorHelper.PerformerNotSpecified(stage);
                        }
                        break;
                    case PerformerUsageMode.Multiple:
                        if (stage.Performers?.Count > 0 != true)
                        {
                            KrErrorHelper.PerformerNotSpecified(stage);
                        }
                        break;
                }
            }
        }

        private void AssertKrScope()
        {
            if (!this.Scope.Exists)
            {
                throw new InvalidOperationException($"{this.GetType().FullName} can't run without KrScope.");
            }
        }
        
        #endregion

    }
}