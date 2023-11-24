using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessServerLauncher: IKrProcessLauncher
    {
        #region nested types

        public sealed class SpecificParameters : IKrProcessLauncherSpecificParameters
        {
            public IMainCardAccessStrategy MainCardAccessStrategy { get; set; }
            public bool RaiseErrorWhenExecutionIsForbidden { get; set; } = true;
            public bool UseSameRequest { get; set; } = false;
        }

        private sealed class CardInfo
        {
            public Guid CardTypeID;
            public string CardTypeName;
            public string CardTypeCaption;
            public Guid? DocTypeID;
            public int StateID;
        }

        #endregion

        #region fields

        private readonly IKrProcessRunnerProvider runnerProvider;

        private readonly Func<IKrExecutor> executorFunc;

        private readonly IKrScope krScope;

        private readonly ICardRepository cardRepository;

        private readonly ICardRepository cardRepositoryEwt;

        private readonly IKrTokenProvider tokenProvider;

        private readonly IKrTypesCache typesCache;

        private readonly IDbScope dbScope;

        private readonly ICardTransactionStrategy transactionStrategy;

        private readonly ICardGetStrategy getStrategy;

        private readonly ICardMetadata cardMetadata;

        private readonly ICardTaskHistoryManager taskHistoryManager;

        private readonly IKrProcessCache processCache;

        private readonly IKrSecondaryProcessExecutionEvaluator secondaryProcessExecutionEvaluator;

        private readonly ISession session;

        private readonly IKrEventManager eventManager;

        private readonly IObjectModelMapper objectModelMapper;

        private readonly ISignatureProvider signatureProvider;

        #endregion

        #region constructor

        public KrProcessServerLauncher(
            IKrProcessRunnerProvider runnerProvider,
            IKrProcessContainer processContainer,
            [Dependency(KrExecutorNames.CacheExecutor)] Func<IKrExecutor> executorFunc,
            IKrScope krScope,
            [Dependency(CardRepositoryNames.Extended)] ICardRepository cardRepository,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository cardRepositoryEwt,
            IKrTokenProvider tokenProvider,
            IKrTypesCache typesCache,
            IDbScope dbScope,
            [Dependency(CardTransactionStrategyNames.Default)] ICardTransactionStrategy transactionStrategy,
            ICardGetStrategy getStrategy,
            ICardMetadata cardMetadata,
            ICardTaskHistoryManager taskHistoryManager,
            IKrProcessCache processCache,
            IKrSecondaryProcessExecutionEvaluator secondaryProcessExecutionEvaluator,
            ISession session,
            IKrEventManager eventManager,
            IObjectModelMapper objectModelMapper,
            ISignatureProvider signatureProvider)
        {
            this.runnerProvider = runnerProvider;
            this.executorFunc = executorFunc;
            this.krScope = krScope;
            this.cardRepository = cardRepository;
            this.cardRepositoryEwt = cardRepositoryEwt;
            this.tokenProvider = tokenProvider;
            this.typesCache = typesCache;
            this.dbScope = dbScope;
            this.transactionStrategy = transactionStrategy;
            this.getStrategy = getStrategy;
            this.cardMetadata = cardMetadata;
            this.taskHistoryManager = taskHistoryManager;
            this.processCache = processCache;
            this.secondaryProcessExecutionEvaluator = secondaryProcessExecutionEvaluator;
            this.session = session;
            this.eventManager = eventManager;
            this.objectModelMapper = objectModelMapper;
            this.signatureProvider = signatureProvider;
        }

        #endregion

        #region implemenattion

        /// <inheritdoc />
        public IKrProcessLaunchResult Launch(
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext = null,
            IKrProcessLauncherSpecificParameters specificParameters = null)
        {
            bool raiseErrorIfForbbiden = false;
            if (specificParameters is SpecificParameters sp)
            {
                raiseErrorIfForbbiden = sp.RaiseErrorWhenExecutionIsForbidden;
            }

            var pid = krProcess.ProcessID;
            var process = this.processCache.GetSecondaryProcess(pid);
            var validationResult = new ValidationResultBuilder();
            if (process is null)
            {
                return this.ErrorResult(validationResult, "$KrSecondaryProcess_Unknown", krProcess.ProcessID);
            }
            if (process is IKrPureProcess pure
                && !pure.AllowClientSideLaunch
                && IsClientSideLaunch(cardContext))
            {
                return this.ErrorResult(validationResult, "$KrSecondaryProcess_ClientSideIsForbidden", process.ID, process.Name);
            }

            IKrProcessLaunchResult result;
            using (this.dbScope.Create())
            {
                using var level = this.krScope.EnterNewLevel(validationResult, this.WithReaderLocks());
                if (krProcess.CardID.HasValue)
                {
                    if (process.IsGlobal)
                    {
                        return this.ErrorResult(validationResult, "$KrSecondaryProcess_IsNotLocal", process.ID, process.Name);
                    }
                    var cardID = krProcess.CardID.Value;
                    var cardLoadingStrategy =  (specificParameters as SpecificParameters)?.MainCardAccessStrategy
                        ?? new KrScopeMainCardAccessStrategy(cardID, this.krScope, validationResult);
                    var cardInfo = this.SelectCardInfo(cardID, cardContext);
                    var components = KrComponentsHelper.GetKrComponents(cardInfo.CardTypeID, cardInfo.DocTypeID, this.typesCache);

                    if (krProcess.SerializedProcess != null
                        && krProcess.SerializedProcessSignature != null)
                    {
                        if (!KrProcessHelper.VerifyWorkflowProcess(krProcess, this.signatureProvider))
                        {
                            return this.ErrorResult(validationResult, "$KrSecondaryProcess_SignatureVerifyingFailed", process.ID, process.Name);
                        }

                        result = this.StartSyncProcess(
                            krProcess,
                            NullMainCardAccessStrategy.Instance,
                            process,
                            cardContext,
                            cardInfo,
                            (pci, holder, npid) =>
                            {
                                var wp = KrProcessHelper.DeserializeWorkflowProcess(krProcess.SerializedProcess);
                                holder.MainWorkflowProcess = wp;
                                return wp;
                            },
                            true);
                    }
                    else
                    {
                        var evaluateResult = this.EvaluateLocal(
                            process, validationResult, cardLoadingStrategy, cardID,
                            cardInfo, components, cardContext, raiseErrorIfForbbiden, out var errorResult);
                        if (!evaluateResult)
                        {
                            return errorResult;
                        }

                        result = process.Async
                            ? this.StartAsyncProcess(cardID, krProcess, cardContext, specificParameters)
                            : this.StartSyncProcess(krProcess, cardLoadingStrategy, process, cardContext, cardInfo, this.CreateWorkflowProcess);
                    }

                    level.ApplyChanges(cardID);
                }
                else
                {
                    if (!process.IsGlobal)
                    {
                        return this.ErrorResult(validationResult, "$KrSecondaryProcess_IsNotGlobal", process.ID, process.Name);
                    }
                    if (process.Async)
                    {
                        return this.ErrorResult(validationResult, "$KrSecondaryProcess_AsyncWithoutCard", process.ID, process.Name);
                    }

                    if (krProcess.SerializedProcess != null
                        && krProcess.SerializedProcessSignature != null)
                    {
                        if (!KrProcessHelper.VerifyWorkflowProcess(krProcess, this.signatureProvider))
                        {
                            return this.ErrorResult(validationResult, "$KrSecondaryProcess_SignatureVerifyingFailed", process.ID, process.Name);
                        }

                        result = this.StartSyncProcess(
                            krProcess,
                            NullMainCardAccessStrategy.Instance,
                            process,
                            cardContext,
                            null,
                            (pci, holder, npid) =>
                            {
                                var wp = KrProcessHelper.DeserializeWorkflowProcess(krProcess.SerializedProcess);
                                holder.MainWorkflowProcess = wp;
                                return wp;
                            },
                            true);
                    }
                    else
                    {
                        var evaluateResult = this.EvaluateGlobal(
                            process, validationResult, cardContext,
                            raiseErrorIfForbbiden, out var errorResult);
                        if (!evaluateResult)
                        {
                            return errorResult;
                        }

                        result = this.StartSyncProcess(
                            krProcess,
                            NullMainCardAccessStrategy.Instance,
                            process,
                            cardContext,
                            null,
                            this.CreateWorkflowProcess);
                    }
                }

                // Если сейчас верхний уровень krScope, то перед его полным закрытием надо записать клиентские команды.
                if (this.krScope.Depth == 1)
                {
                    var commands = this.krScope.GetKrProcessClientCommands();
                    if (commands != null
                        && cardContext != null)
                    {
                        // Установка команд поддеживается только в два типа реквестов
                        switch (cardContext)
                        {
                            case CardRequestExtensionContext cardRequestExtensionContext:
                                cardRequestExtensionContext.Response?.AddKrProcessClientCommands(commands);
                                break;
                            case CardStoreExtensionContext cardStoreExtensionContext:
                                cardStoreExtensionContext.Response?.AddKrProcessClientCommands(commands);
                                break;
                        }
                    }

                    // Вносим накопившиеся в scope сообщения в результат.
                    validationResult.Add(this.krScope.ValidationResult);
                }
            }

            result.ValidationResult.Add(validationResult);
            return result;
        }

        /// <inheritdoc />
        public async Task<IKrProcessLaunchResult> LaunchAsync(
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext = null,
            IKrProcessLauncherSpecificParameters specificParameters = null)
        {
            return await Task.Run(() => this.Launch(krProcess, cardContext, specificParameters));
        }

        #endregion

        #region private

        private static bool IsClientSideLaunch(
            ICardExtensionContext cardContext)
        {
            // Если контекст отсутствует, считаем, что запускаем код с сервера.
            if (cardContext is null)
            {
                return false;
            }

            bool ClientServiceType(CardServiceType type) => type != CardServiceType.Default;

            switch (cardContext)
            {
                case ICardDeleteExtensionContext cardDeleteExtensionContext:
                    return ClientServiceType(cardDeleteExtensionContext.Request.ServiceType);
                case ICardGetFileContentExtensionContext cardGetFileContentExtensionContext:
                    return ClientServiceType(cardGetFileContentExtensionContext.Request.ServiceType);
                case ICardGetExtensionContext cardGetExtensionContext:
                    return ClientServiceType(cardGetExtensionContext.Request.ServiceType);
                case ICardGetFileVersionsExtensionContext cardGetFileVersionsExtensionContext:
                    return ClientServiceType(cardGetFileVersionsExtensionContext.Request.ServiceType);
                case ICardNewExtensionContext cardNewExtensionContext:
                    return ClientServiceType(cardNewExtensionContext.Request.ServiceType);
                case ICardRequestExtensionContext cardRequestExtensionContext:
                    return ClientServiceType(cardRequestExtensionContext.Request.ServiceType);
                case ICardStoreExtensionContext cardStoreExtensionContext:
                    return ClientServiceType(cardStoreExtensionContext.Request.ServiceType);
                case ICardStoreTaskExtensionContext cardStoreTaskExtensionContext:
                    return ClientServiceType(cardStoreTaskExtensionContext.Request.ServiceType);
                default:
                    throw new ArgumentOutOfRangeException($"Can't recognize client-side launch for {cardContext.GetType().FullName} context");
            }
        }

        private bool EvaluateGlobal(
            IKrSecondaryProcess process,
            IValidationResultBuilder validationResult,
            ICardExtensionContext cardContext,
            bool raiseErrorIfForbbiden,
            out IKrProcessLaunchResult errorResult)
        {
            return this.EvaluateLocal(
                process,
                validationResult,
                NullMainCardAccessStrategy.Instance,
                null,
                null,
                null,
                cardContext,
                raiseErrorIfForbbiden,
                out errorResult
            );
        }

        private bool EvaluateLocal(
            IKrSecondaryProcess process,
            IValidationResultBuilder validationResult,
            IMainCardAccessStrategy mainCardAccessStrategy,
            Guid? cardID,
            CardInfo cardInfo,
            KrComponents? components,
            ICardExtensionContext cardContext,
            bool raiseErrorIfForbbiden,
            out IKrProcessLaunchResult errorResult)
        {
            errorResult = null;
            var evaluatorContext = new KrSecondaryProcessEvaluatorContext(
                process,
                validationResult,
                mainCardAccessStrategy,
                cardID,
                cardInfo?.CardTypeID,
                cardInfo?.CardTypeName,
                cardInfo?.CardTypeCaption,
                cardInfo?.DocTypeID,
                components,
                (KrState)(cardInfo?.StateID ?? KrState.Draft.ID),
                null,
                cardContext);
            var evaluationResult = this.secondaryProcessExecutionEvaluator.Evaluate(evaluatorContext);
            if (!evaluationResult)
            {
                if (raiseErrorIfForbbiden)
                {
                    var msg = string.IsNullOrWhiteSpace(process.ExecutionAccessDeniedMessage)
                        ? "$KrSecondaryProcess_SecondaryProcessLaunchIsForbiddenViaRestrictions"
                        : process.ExecutionAccessDeniedMessage;
                    errorResult = this.ErrorResult(validationResult, msg);
                }
                else
                {
                    errorResult = new KrProcessLaunchResult(
                        KrProcessLaunchStatus.Forbidden,
                        null,
                        validationResult.Build(),
                        null,
                        null,
                        null);
                }
            }

            return evaluationResult;
        }

        private IKrProcessLaunchResult StartAsyncProcess(
            Guid cardID,
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext,
            IKrProcessLauncherSpecificParameters specificParameters)
        {
            var validationResultBuilder = new ValidationResultBuilder();
            bool useSameRequest = false;
            if (specificParameters is SpecificParameters sp)
            {
                useSameRequest = sp.UseSameRequest;
            }

            var processID = Guid.NewGuid();
            var nested = krProcess.ParentStageRowID.HasValue
                && krProcess.ProcessHolderID.HasValue
                && krProcess.NestedOrder.HasValue;
            var startingProcessName = nested
                ? KrConstants.KrNestedProcessName
                : KrConstants.KrSecondaryProcessName;
            var secondaryProcessInfo = new StartingSecondaryProcessInfo(
                krProcess.ProcessID,
                krProcess.ProcessInfo,
                krProcess.ParentStageRowID,
                krProcess.ParentProcessTypeName,
                krProcess.ParentProcessID,
                krProcess.ProcessHolderID,
                krProcess.NestedOrder);

            if (useSameRequest)
            {
                if (!(cardContext is ICardStoreExtensionContext storeCardContext))
                {
                    throw new InvalidOperationException($"Can't apply {nameof(SpecificParameters.UseSameRequest)} " +
                        $"to any CardContext except {typeof(ICardStoreExtensionContext).FullName}.");
                }

                storeCardContext.Request.SetStartingProcessID(processID);
                storeCardContext.Request.SetStartingProcessName(startingProcessName);
                storeCardContext.Request.SetStartingSecondaryProcess(secondaryProcessInfo);
                return new KrProcessLaunchResult(
                    KrProcessLaunchStatus.Undefined, processID, validationResultBuilder.Build(), null, null, null);
            }

            var suitableCardRepo = KrProcessHelper.IsTransactionOpened(cardContext?.DbScope)
                ? this.cardRepositoryEwt
                : this.cardRepository;
            var mainCard = this.GetCardInstance(cardID, out var result);
            if (!result.IsSuccessful)
            {
                validationResultBuilder.Add(result);
                return new KrProcessLaunchResult(
                    KrProcessLaunchStatus.Error, null, validationResultBuilder.Build(), null, null, null);
            }
            var storeRequest = new CardStoreRequest { Card = mainCard };
            storeRequest.SetStartingProcessID(processID);
            storeRequest.SetStartingProcessName(startingProcessName);
            storeRequest.SetStartingSecondaryProcess(secondaryProcessInfo);

            // Разрешим делать с карточкой все что угодно,
            // т.к. необходимо было выполнить проверку при запуске процесса
            var nextKrToken = this.tokenProvider.CreateToken(mainCard);
            nextKrToken.Set(mainCard.Info);
            var storeResponse = suitableCardRepo.StoreAsync(storeRequest).GetAwaiter().GetResult(); // TODO async
            validationResultBuilder.Add(storeResponse.ValidationResult);
            var status = storeResponse.Info.GetAsyncProcessCompletedSimultaniosly()
                ? KrProcessLaunchStatus.Complete
                : KrProcessLaunchStatus.InProgress;

            return new KrProcessLaunchResult(
                status, processID, validationResultBuilder.Build(), storeResponse.Info.GetProcessInfoAtEnd(), storeResponse, null);
        }

        private IKrProcessLaunchResult StartSyncProcess(
            KrProcessInstance krProcess,
            IMainCardAccessStrategy mainCardAccessStrategy,
            IKrSecondaryProcess secondaryProcess,
            ICardExtensionContext cardContext,
            CardInfo cardInfo,
            Func<ProcessCommonInfo, ProcessHolder, Guid?, WorkflowProcess> createWorkflowProcessFunc,
            bool resurrection = false)
        {
            var nestedProcessID = GetNestedProcessID(krProcess);
            var contextualSatellite = this.GetContextualSatellite(krProcess);
            this.GetProcessHolder(
                krProcess,
                contextualSatellite,
                secondaryProcess.ID,
                nestedProcessID,
                out var processHolder,
                out var processHolderCreated,
                out var pci);
            var workflowProcess = createWorkflowProcessFunc(pci, processHolder, nestedProcessID);

            var validationResultBuilder = new ValidationResultBuilder();
            IKrTaskHistoryResolver taskHistoryResolver;
            KrComponents? components;
            if (krProcess.CardID.HasValue)
            {
                taskHistoryResolver =
                    new KrTaskHistoryResolver(mainCardAccessStrategy, cardContext, validationResultBuilder, this.taskHistoryManager);
                components = KrComponentsHelper.GetKrComponents(cardInfo.CardTypeID, cardInfo.DocTypeID, this.typesCache);
            }
            else
            {
                taskHistoryResolver = null;
                components = null;
            }

            if (!resurrection)
            {
                var executor = this.executorFunc();
                var ctx = new KrExecutionContext(
                    cardContext,
                    mainCardAccessStrategy: mainCardAccessStrategy,
                    cardID: krProcess.CardID,
                    cardTypeID: cardInfo?.CardTypeID,
                    cardTypeName: cardInfo?.CardTypeName,
                    cardTypeCaption: cardInfo?.CardTypeCaption,
                    docTypeID: cardInfo?.DocTypeID,
                    krComponents: components,
                    workflowProcess: workflowProcess,
                    compilationResult: null,
                    secondaryProcess: secondaryProcess
                );

                var executorResult = executor.Execute(ctx);
                validationResultBuilder.Add(executorResult.Result);

                if (!validationResultBuilder.IsSuccessful())
                {
                    return new KrProcessLaunchResult(
                        KrProcessLaunchStatus.Error, null, validationResultBuilder.Build(), null, null, null);
                }
            }

            var runnerContext = new KrProcessRunnerContext(
                workflowAPI: null,
                taskHistoryResolver: taskHistoryResolver,
                mainCardAccessStrategy: mainCardAccessStrategy,
                cardID: krProcess.CardID,
                cardTypeID: cardInfo?.CardTypeID,
                cardTypeName: cardInfo?.CardTypeName,
                cardTypeCaption: cardInfo?.CardTypeCaption,
                docTypeID: cardInfo?.DocTypeID,
                krComponents: components,
                contextualSatellite: contextualSatellite,
                processHolderSatellite: null,
                processHolder: processHolder,
                workflowProcess: workflowProcess,
                processInfo: null,
                validationResult: validationResultBuilder,
                cardContext: cardContext,
                secondaryProcess: secondaryProcess,
                parentProcessID: krProcess.ParentProcessID,
                parentProcessTypeName: krProcess.ParentProcessTypeName,
                defaultPreparingGroupStrategyFunc: this.DefaultPreparingStrategy,
                resurrection: resurrection);

            this.runnerProvider.GetRunner(KrProcessRunnerNames.Sync).Run(runnerContext);

            this.eventManager.RaiseAsync(
                DefaultEventTypes.SyncProcessCompleted,
                currentStage: null,
                runnerMode: KrProcessRunnerMode.Sync,
                runnerContext: runnerContext,
                info: null).GetAwaiter().GetResult(); // TODO async

            // Если холдер был создан тут, значит синхронный процесс - главный,
            // достаточно сохранить только основной процесс.
            // Также нужно удостоверится, что CardID осмысленный, т.е.
            // выполнение идет в уже созданной в базе карточке
            // (если null, то карточки нет, если Guid.Empty, то еще не сохранена)
            if (processHolderCreated
                && krProcess.CardID.HasValue
                && krProcess.CardID != Guid.Empty)
            {
                this.objectModelMapper.ObjectModelToPci(
                    processHolder.MainWorkflowProcess,
                    processHolder.MainProcessCommonInfo,
                    processHolder.MainProcessCommonInfo,
                    processHolder.PrimaryProcessCommonInfo);
                this.objectModelMapper.SetMainProcessCommonInfo(
                    krProcess.CardID.Value, contextualSatellite, processHolder.PrimaryProcessCommonInfo);
            }

            return new KrProcessLaunchResult(
                KrProcessLaunchStatus.Complete, null, validationResultBuilder.Build(), workflowProcess.InfoStorage, null, null);
        }

        private IPreparingGroupRecalcStrategy DefaultPreparingStrategy() =>
            new ForwardPreparingGroupRecalcStrategy(this.dbScope, this.session);

        private CardInfo SelectCardInfo(
            Guid cardID,
            ICardExtensionContext cardContext)
        {
            var cardInfo = new CardInfo();
            var ct = cardContext?.CardType;
            var selectTypeInfo = ct is null;
            if (!selectTypeInfo)
            {
                cardInfo.CardTypeID = ct.ID;
                cardInfo.CardTypeName = ct.Name;
                cardInfo.CardTypeCaption = ct.Caption;
            }

            using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var preselectQuery = this.dbScope.BuilderFactory
                    .Select();
                if (selectTypeInfo)
                {
                    preselectQuery
                        .C("t", "ID")
                        .C("t", "Name")
                        .C("t", "Caption");
                }

                preselectQuery
                    .C("dci", "DocTypeID")
                    .C("aci", "StateID")
                    .From(KrConstants.DocumentCommonInfo.Name, "dci").NoLock();
                if (selectTypeInfo)
                {
                    preselectQuery
                        .InnerJoin("Types", "t").NoLock().On().C("dci", "CardTypeID").Equals().C("t", "ID");
                }
                preselectQuery
                    .LeftJoin(KrConstants.KrApprovalCommonInfo.Name, "aci").NoLock().On().C("dci", "ID").Equals().C("aci", "MainCardID")
                    .Where().C("dci", KrConstants.ID).Equals().P("CardID");

                db
                    .SetCommand(
                        preselectQuery.Build(),
                        db.Parameter("CardID", cardID))
                    .LogCommand();

                using var reader = db.ExecuteReader();
                if (!reader.Read())
                {
                    return cardInfo;
                }

                var clmn = 0;
                if (selectTypeInfo)
                {
                    cardInfo.CardTypeID = reader.GetGuid(clmn++);
                    cardInfo.CardTypeName = reader.GetString(clmn++);
                    cardInfo.CardTypeCaption =reader.GetString(clmn++);
                }

                cardInfo.DocTypeID = reader.GetNullableGuid(clmn++);
                cardInfo.StateID = reader.GetNullableInt16(clmn) ?? KrState.Draft.ID;
            }

            return cardInfo;
        }

        private Card GetCardInstance(Guid cardID, out ValidationResult result)
        {
            var validationResultBuilder = new ValidationResultBuilder();
            Card card = null;

            // TODO async
            this.GetSuitableTransactionStrategy().ExecuteInReaderLockAsync(
                cardID,
                validationResultBuilder,
                async p =>
                {
                    var getContext = this.getStrategy
                        .TryLoadCardInstanceAsync(
                            cardID,
                            p.DbScope.Db,
                            this.cardMetadata,
                            p.ValidationResult).GetAwaiter().GetResult(); // TODO async
                    card = getContext.Card;
                }
            ).GetAwaiter().GetResult();
            result = validationResultBuilder.Build();
            return card;
        }

        private ICardTransactionStrategy GetSuitableTransactionStrategy() =>
            this.krScope.CurrentLevel?.CardTransactionStrategy ?? this.transactionStrategy;

        private IKrProcessLaunchResult ErrorResult(
            IValidationResultBuilder validationResult,
            string errorText,
            params object[] args)
        {
            validationResult.AddError(this, errorText, args);
            return new KrProcessLaunchResult(
                KrProcessLaunchStatus.Error, null, validationResult.Build(), null, null, null);
        }

        private Card GetContextualSatellite(KrProcessInstance krProcess)
        {
            if (krProcess.CardID.HasValue
                && krProcess.CardID != default(Guid))
            {
                return this.krScope.GetKrSatellite(krProcess.CardID.Value);
            }

            return null;
        }

        private ProcessHolder CreateProcessHolder(
            Card contextualSatellite,
            KrProcessInstance krProcessInstance) =>
            new ProcessHolder
            {
                Persistent = false,
                MainProcessType = KrConstants.KrSecondaryProcessName,
                ProcessHolderID = Guid.NewGuid(),
                PrimaryProcessCommonInfo = contextualSatellite != null
                    ? this.objectModelMapper.GetMainProcessCommonInfo(contextualSatellite)
                    : null,
                MainProcessCommonInfo = new MainProcessCommonInfo(
                    null,
                    krProcessInstance.ProcessInfo ?? new Dictionary<string, object>(),
                    krProcessInstance.ProcessID,
                    null,
                    null,
                    null,
                    0)
            };

        private static Guid? GetNestedProcessID(KrProcessInstance krProcess) =>
            krProcess.ParentStageRowID.HasValue && krProcess.NestedOrder.HasValue
                ? (Guid?)Guid.NewGuid()
                : null;

        private void GetProcessHolder(
            KrProcessInstance krProcess,
            Card contextualSatellite,
            Guid secondaryProcessID,
            Guid? nestedProcessID,
            out ProcessHolder processHolder,
            out bool processHolderCreated,
            out ProcessCommonInfo pci)
        {
            var processHolderID = krProcess.ProcessHolderID;
            processHolder = processHolderID.HasValue
                ? this.krScope.GetProcessHolder(processHolderID.Value)
                : null;
            processHolderCreated = false;
            // Если pci нет, то запускаем главный процесс
            if (processHolder is null)
            {
                processHolder = this.CreateProcessHolder(contextualSatellite, krProcess);
                pci = processHolder.MainProcessCommonInfo;
                processHolderCreated = true;
            }
            // Иначе располагаемся в нестеде
            else if (nestedProcessID.HasValue
                && krProcess.ParentStageRowID.HasValue
                && krProcess.NestedOrder.HasValue)
            {
                if (processHolder.NestedProcessCommonInfos is null)
                {
                    if (processHolder.Persistent)
                    {
                        Card processHolderSatellite;
                        switch (processHolder.MainProcessType)
                        {
                            case KrConstants.KrProcessName:
                                processHolderSatellite = contextualSatellite;
                                break;
                            case KrConstants.KrSecondaryProcessName:
                                processHolderSatellite = this.krScope.GetSecondaryKrSatellite(processHolder.ProcessHolderID);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        processHolder.NestedProcessCommonInfosList =
                            this.objectModelMapper.GetNestedProcessCommonInfos(processHolderSatellite);
                    }
                    else
                    {
                        processHolder.NestedProcessCommonInfosList = new List<NestedProcessCommonInfo>();
                    }

                }
                var npci = new NestedProcessCommonInfo(
                    null,
                    krProcess.ProcessInfo,
                    secondaryProcessID,
                    nestedProcessID.Value,
                    krProcess.ParentStageRowID.Value,
                    krProcess.NestedOrder.Value
                    );
                processHolder.NestedProcessCommonInfos.Add(npci);
                pci = npci;
            }
            else
            {
                throw new InvalidOperationException("Inconsistent starting sync process parameters.");
            }
        }

        private WorkflowProcess CreateWorkflowProcess(
            ProcessCommonInfo pci,
            ProcessHolder processHolder,
            Guid? nestedProcessID)
        {
            var workflowProcess = new WorkflowProcess(
                pci.Info,
                processHolder.MainProcessCommonInfo.Info,
                new SealableObjectList<Stage>(),
                saveInitialStages: true,
                nestedProcessID: nestedProcessID);

            if (nestedProcessID.HasValue)
            {
                processHolder.NestedWorkflowProcesses[nestedProcessID.Value] = workflowProcess;
            }
            else
            {
                processHolder.MainWorkflowProcess = workflowProcess;
            }

            this.objectModelMapper.FillWorkflowProcessFromPci(
                workflowProcess,
                pci,
                processHolder.PrimaryProcessCommonInfo);

            return workflowProcess;
        }

        private bool WithReaderLocks() => this.krScope.CurrentLevel?.WithReaderLocks ?? true;

        #endregion

    }
}