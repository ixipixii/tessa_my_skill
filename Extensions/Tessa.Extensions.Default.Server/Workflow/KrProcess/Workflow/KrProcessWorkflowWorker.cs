using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Events;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessWorkflowWorker: WorkflowWorker<KrProcessWorkflowManager>
    {
        #region constructor

        public KrProcessWorkflowWorker(KrProcessWorkflowManager manager)
            : base(manager)
        {
        }

        #endregion

        #region private

        /// <summary>
        /// WorkflowContext
        /// </summary>
        private KrProcessWorkflowContext WCtx => this.Manager.WorkflowContext;

        private async Task StartRunnerAsync(IWorkflowProcessInfo info, CancellationToken cancellationToken = default)
        {
            // ValidationResult здесь и во всех вложенных выводах используется из context.ValidationResult
            // расширения на сохранение карточки. Таким образом все, что могло возникнуть здесь и глубже, будет корректно
            // выведено.

            var scope = this.WCtx.KrScope;
            if (!scope.MultirunEnabled(info.ProcessID)
                && !scope.FirstLaunchPerRequest(info.ProcessID))
            {
                return;
            }
            scope.AddToLaunchedLevels(info.ProcessID);

            var startingSecondaryProcess = this.WCtx.CardStoreContext.Request.GetStartingSecondaryProcess();
            StoreParentProcess(info, startingSecondaryProcess);

            var mainCardID = this.Manager.Request.Card.ID;
            var contextualSatellite = this.WCtx.KrScope.GetKrSatellite(mainCardID);
            // Получаем холдер процесса в зависимости от типа процесса и его вложенности
            this.GetProcessHolder(
                info,
                startingSecondaryProcess,
                contextualSatellite,
                out var processHolderSatellite,
                out var processHolderID,
                out var processHolder);

            // Строим модель процесса на основе доступных сателлитов и холдеров
            var workflowProcess = this.GetWorkflowProcess(
                info,
                startingSecondaryProcess,
                contextualSatellite,
                processHolderSatellite,
                processHolderID,
                ref processHolder,
                out var processHolderCreated,
                out var pci);

            if (!processHolder.Persistent)
            {
                this.Manager.ValidationResult.AddError("$KrProcess_Error_AsyncProcessWithoutPersistentHolder");
                return;
            }

            var mainCardKey = scope.LockCard(mainCardID);
            var contextualKey = scope.LockCard(contextualSatellite.ID);
            var processHolderKey = scope.LockCard(processHolderSatellite.ID);

            if (startingSecondaryProcess != null)
            {
                StorageHelper.Merge(startingSecondaryProcess.ProcessInfo, workflowProcess.InfoStorage);
            }
            var secondaryProcess = this.GetSecondaryProcess(
                info, startingSecondaryProcess?.SecondaryProcessID, pci);

            var cardLoadingStrategy = new KrScopeMainCardAccessStrategy(this.WCtx.CardID, this.WCtx.KrScope);
            var taskHistoryResolver = new KrTaskHistoryResolver(
                cardLoadingStrategy,
                this.WCtx,
                this.WCtx.ValidationResult,
                this.WCtx.TaskHistoryManager);
            var api = new WorkflowAPIBridge(this.Manager, info);
            var runnerContext = new KrProcessRunnerContext(
                workflowAPI: api,
                taskHistoryResolver: taskHistoryResolver,
                mainCardAccessStrategy: cardLoadingStrategy,
                cardID: this.WCtx.CardID,
                cardTypeID: this.WCtx.CardTypeID,
                cardTypeName: this.WCtx.CardTypeName,
                cardTypeCaption: this.WCtx.CardTypeCaption,
                docTypeID: this.WCtx.DocTypeID,
                krComponents: this.WCtx.KrComponents,
                contextualSatellite: contextualSatellite,
                processHolderSatellite: processHolderSatellite,
                workflowProcess: workflowProcess,
                processHolder: processHolder,
                processInfo: info,
                validationResult: this.Manager.ValidationResult,
                cardContext: this.WCtx.CardStoreContext,
                secondaryProcess: secondaryProcess,
                parentProcessID: info.ProcessParameters.TryGet<Guid?>(Keys.ParentProcessID),
                parentProcessTypeName: info.ProcessParameters.TryGet<string>(Keys.ParentProcessType),
                defaultPreparingGroupStrategyFunc: this.DefaultPreparingStrategy);

            this.WCtx.AsyncProcessRunner.Run(runnerContext);

            if (runnerContext.WorkflowProcess.CurrentApprovalStageRowID is null)
            {
                await this.WCtx.EventManager.RaiseAsync(
                    DefaultEventTypes.AsyncProcessCompleted,
                    currentStage: null,
                    runnerMode: KrProcessRunnerMode.Async,
                    runnerContext: runnerContext,
                    cancellationToken: cancellationToken);

                this.Manager.ProcessesAwaitingRemoval.Add(info);

                this.WCtx.CardStoreContext.Info.SetProcessInfoAtEnd(workflowProcess.InfoStorage);

                if (runnerContext.InitiationCause == KrProcessRunnerInitiationCause.StartProcess)
                {
                    this.WCtx.CardStoreContext.Info.SetAsyncProcessCompletedSimultaniosly();
                }
            }

            // Только создающий процесс холдер может его переводить обратно
            if (processHolderCreated)
            {
                this.ObjectModelToCardRows(processHolder, processHolderSatellite, contextualSatellite);
            }

            scope.ReleaseCard(mainCardID, mainCardKey);
            scope.ReleaseCard(contextualSatellite.ID, contextualKey);
            scope.ReleaseCard(processHolderSatellite.ID, processHolderKey);
        }

        private WorkflowProcess GetWorkflowProcess(
            IWorkflowProcessInfo info,
            StartingSecondaryProcessInfo startingSecondaryProcess,
            Card contextualSatellite,
            Card processHolderSatellite,
            Guid processHolderID,
            ref ProcessHolder processHolder,
            out bool processHolderCreated,
            out ProcessCommonInfo pci)
        {
            processHolderCreated = false;
            var nested = info.ProcessTypeName == KrNestedProcessName;
            var nestedProcessID = GetNestedProcessID(info, nested);

            if (processHolder is null)
            {
                // Холдер отсутствует, т.е. для текущего сателлита-холдер запускается самый верхний процесс.
                processHolder = new ProcessHolder();
                processHolderCreated = true;
                // Если запускается нестед, значит по нестеду совершено какое-то действие (уже был запущен ранее)
                if (nested)
                {
                    processHolder.MainProcessType = GetMainProcessType(info, null);
                    processHolder.Persistent = true;
                    processHolder.ProcessHolderID = processHolderID;
                }
                // Иначе запускается основной процесс
                else
                {
                    processHolder.MainProcessType = info.ProcessTypeName;
                    processHolder.Persistent = true;
                    processHolder.ProcessHolderID = processHolderID;
                }
                this.WCtx.KrScope.AddProcessHolder(processHolder);
            }

            if (processHolder.MainWorkflowProcess is null)
            {
                processHolder.MainProcessCommonInfo =
                    this.WCtx.ObjectModelMapper.GetMainProcessCommonInfo(processHolderSatellite);
            }
            if (processHolder.PrimaryProcessCommonInfo is null)
            {
                processHolder.PrimaryProcessCommonInfo = processHolder.MainProcessType == KrProcessName
                    ? processHolder.MainProcessCommonInfo
                    : this.WCtx.ObjectModelMapper.GetMainProcessCommonInfo(contextualSatellite, false);
            }

            if (nested
                && processHolder.NestedProcessCommonInfos is null)
            {
                processHolder.NestedProcessCommonInfosList =
                    this.WCtx.ObjectModelMapper.GetNestedProcessCommonInfos(processHolderSatellite);
            }

            pci = null;
            WorkflowProcess workflowProcess = null;
            if (nested
                && nestedProcessID.HasValue)
            {
                if (!processHolder.NestedProcessCommonInfos.TryGetItem(nestedProcessID.Value, out var npci))
                {
                    npci = new NestedProcessCommonInfo(
                        null,
                        new Dictionary<string, object>(),
                        startingSecondaryProcess.SecondaryProcessID ?? Guid.Empty,
                        nestedProcessID.Value,
                        startingSecondaryProcess.ParentStageRowID ?? Guid.Empty,
                        startingSecondaryProcess.NestedOrder ?? 0
                        );
                    processHolder.NestedProcessCommonInfos.Add(npci);
                }
                pci = npci;

                if (!processHolder.NestedWorkflowProcesses.TryGetValue(nestedProcessID.Value, out workflowProcess))
                {
                    var (templates, stages) = this.WCtx.ProcessCache.GetRelatedTemplates(processHolderSatellite, nestedProcessID);
                    workflowProcess = this.WCtx.ObjectModelMapper.CardRowsToObjectModel(
                        processHolderSatellite,
                        npci,
                        processHolder.MainProcessCommonInfo,
                        templates,
                        stages,
                        nestedProcessID: npci.NestedProcessID);
                    this.WCtx.ObjectModelMapper.FillWorkflowProcessFromPci(
                        workflowProcess,
                        pci,
                        processHolder.PrimaryProcessCommonInfo);
                    processHolder.NestedWorkflowProcesses[nestedProcessID.Value] = workflowProcess;
                }
            }
            else if (!nested)
            {
                workflowProcess = processHolder.MainWorkflowProcess;
                if (workflowProcess == null)
                {
                    var (templates, stages) = this.WCtx.ProcessCache.GetRelatedTemplates(processHolderSatellite, nestedProcessID);
                    workflowProcess = this.WCtx.ObjectModelMapper.CardRowsToObjectModel(
                        processHolderSatellite,
                        processHolder.MainProcessCommonInfo,
                        processHolder.MainProcessCommonInfo,
                        templates,
                        stages);
                    this.WCtx.ObjectModelMapper.FillWorkflowProcessFromPci(
                        workflowProcess,
                        processHolder.MainProcessCommonInfo,
                        processHolder.PrimaryProcessCommonInfo);
                    processHolder.MainWorkflowProcess = workflowProcess;
                }

                pci = processHolder.MainProcessCommonInfo;
            }

            return workflowProcess;
        }

        private static Guid? GetNestedProcessID(IWorkflowProcessInfo info, bool nested)
        {
            var nestedProcessID = info.ProcessParameters.TryGet<Guid?>(Keys.NestedProcessID);
            if (nestedProcessID == null && nested)
            {
                nestedProcessID = Guid.NewGuid();
                info.ProcessParameters[Keys.NestedProcessID] = nestedProcessID;
                info.PendingProcessParametersUpdate = true;
                return nestedProcessID;
            }

            return nestedProcessID;
        }

        private void GetProcessHolder(
            IWorkflowProcessInfo info,
            StartingSecondaryProcessInfo startingSecondaryProcess,
            Card contextualSatellite,
            out Card processHolderSatellite,
            out Guid processHolderID,
            out ProcessHolder processHolder)
        {
            var mainCardID = this.Manager.Request.Card.ID;
            switch (info.ProcessTypeName)
            {
                case KrProcessName:
                    processHolderSatellite = contextualSatellite;
                    processHolderID = mainCardID;
                    processHolder = this.TryGetProcessHolder(processHolderID);
                    break;
                case KrSecondaryProcessName:
                    processHolderSatellite = this.WCtx.KrScope.GetSecondaryKrSatellite(info.ProcessID);
                    processHolderID = info.ProcessID;
                    processHolder = this.TryGetProcessHolder(processHolderID);
                    break;
                case KrNestedProcessName:
                    var processHolderIDNullable = GetProcessHolderID(info, startingSecondaryProcess);
                    if (processHolderIDNullable is null)
                    {
                        throw new InvalidOperationException(
                            $"Process holder ID is null for process type {KrNestedProcessName}");
                    }
                    processHolder = this.TryGetProcessHolder(processHolderIDNullable.Value);
                    var mainProcessType = GetMainProcessType(info, processHolder);

                    switch (mainProcessType)
                    {
                        case KrProcessName:
                            processHolderID = mainCardID;
                            processHolderSatellite = contextualSatellite;
                            break;
                        case KrSecondaryProcessName:
                            processHolderID = processHolderIDNullable.Value;
                            processHolderSatellite = this.WCtx.KrScope.GetSecondaryKrSatellite(processHolderID);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void StoreParentProcess(
            IWorkflowProcessInfo info,
            StartingSecondaryProcessInfo startingSecondaryProcess)
        {
            if (startingSecondaryProcess != null)
            {
                info.ProcessParameters[Keys.ParentProcessType] = startingSecondaryProcess.ParentProcessTypeName;
                info.ProcessParameters[Keys.ParentProcessID] = startingSecondaryProcess.ParentProcessID;
                info.PendingProcessParametersUpdate = true;
            }
        }

        private static Guid? GetProcessHolderID(
            IWorkflowProcessInfo info,
            StartingSecondaryProcessInfo startingSecondaryProcess)
        {
            var processHolderID = startingSecondaryProcess?.ProcessHolderID;
            if (processHolderID != null)
            {
                info.ProcessParameters[Keys.ProcessHolderID] = processHolderID;
                info.PendingProcessParametersUpdate = true;
                return processHolderID;
            }

            return info.ProcessParameters.TryGet<Guid?>(Keys.ProcessHolderID);
        }

        private static string GetMainProcessType(
            IWorkflowProcessInfo info,
            ProcessHolder holder)
        {
            if (holder != null)
            {
                info.ProcessParameters[Keys.MainProcessType] = holder.MainProcessType;
                info.PendingProcessParametersUpdate = true;
                return holder.MainProcessType;
            }
            return info.ProcessParameters.TryGet<string>(Keys.MainProcessType);
        }

        private void ObjectModelToCardRows(
            ProcessHolder processHolder,
            Card processHolderSatellite,
            Card contextualSatellite)
        {
            foreach (var nested in processHolder.NestedWorkflowProcesses)
            {
                this.WCtx.ObjectModelMapper.ObjectModelToCardRows(
                    nested.Value,
                    processHolderSatellite,
                    processHolder.NestedProcessCommonInfos[nested.Key]);
                this.WCtx.ObjectModelMapper.ObjectModelToPci(
                    nested.Value,
                    processHolder.NestedProcessCommonInfos[nested.Key],
                    processHolder.MainProcessCommonInfo,
                    processHolder.PrimaryProcessCommonInfo);
            }

            if (processHolder.MainWorkflowProcess != null)
            {
                this.WCtx.ObjectModelMapper.ObjectModelToCardRows(
                    processHolder.MainWorkflowProcess,
                    processHolderSatellite,
                    processHolder.MainProcessCommonInfo);
                this.WCtx.ObjectModelMapper.ObjectModelToPci(
                    processHolder.MainWorkflowProcess,
                    processHolder.MainProcessCommonInfo,
                    processHolder.MainProcessCommonInfo,
                    processHolder.PrimaryProcessCommonInfo);
            }

            var mainCardID = this.Manager.Request.Card.ID;
            // Переносим основную инфу по главному процессу.
            // Это состояние карточки, инициатор и т.д
            this.WCtx.ObjectModelMapper.SetMainProcessCommonInfo(
                mainCardID, contextualSatellite, processHolder.PrimaryProcessCommonInfo);
            // Если это не основной процесс, то переносим инфу по главному процессу в его карточку-холдер
            if (!ReferenceEquals(contextualSatellite, processHolderSatellite))
            {
                this.WCtx.ObjectModelMapper.SetMainProcessCommonInfo(
                    mainCardID, processHolderSatellite, processHolder.MainProcessCommonInfo);
            }
            // Переносим оставшиеся процессы-нестеды
            this.WCtx.ObjectModelMapper.SetNestedProcessCommonInfos(
                processHolderSatellite, processHolder.NestedProcessCommonInfos);

            this.WCtx.KrScope.RemoveProcessHolder(processHolder.ProcessHolderID);
        }

        private IPreparingGroupRecalcStrategy DefaultPreparingStrategy() =>
            new ForwardPreparingGroupRecalcStrategy(this.WCtx.DbScope, this.WCtx.Session);

        private IKrSecondaryProcess GetSecondaryProcess(
            IWorkflowProcessInfo info,
            Guid? secondaryProcessID,
            ProcessCommonInfo pci)
        {
            if (info.ProcessTypeName != KrSecondaryProcessName
                && info.ProcessTypeName != KrNestedProcessName)
            {
                return null;
            }

            var process = secondaryProcessID.HasValue
                ? this.WCtx.ProcessCache.GetSecondaryProcess(secondaryProcessID.Value)
                : null;

            if (process != null)
            {
                pci.SecondaryProcessID = process.ID;
                return process;
            }
            if (pci.SecondaryProcessID.HasValue)
            {
                return this.WCtx.ProcessCache.GetSecondaryProcess(pci.SecondaryProcessID.Value);
            }

            return null;
        }

        private ProcessHolder TryGetProcessHolder(Guid? processHolderID)
        {
            return processHolderID is null
                ? null
                : this.WCtx.KrScope.GetProcessHolder(processHolderID.Value);
        }

        #endregion

        #region base overrides

        protected override Task StartProcessCoreAsync(IWorkflowProcessInfo processInfo, CancellationToken cancellationToken = default)
        {
            return this.StartRunnerAsync(processInfo, cancellationToken);
        }

        protected override Task CompleteTaskCoreAsync(IWorkflowTaskInfo taskInfo, CancellationToken cancellationToken = default)
        {
            if (taskInfo.ProcessTypeName == KrSecondaryProcessName)
            {
                var card = this.WCtx.KrScope.GetSecondaryKrSatellite(taskInfo.ProcessID);
                this.Manager.SpecifySatelliteID(card.ID, false);
            }

            return this.StartRunnerAsync(taskInfo, cancellationToken);
        }

        protected override Task ReinstateTaskCoreAsync(IWorkflowTaskInfo taskInfo, CancellationToken cancellationToken = default)
        {
            if (taskInfo.ProcessTypeName == KrSecondaryProcessName)
            {
                var card = this.WCtx.KrScope.GetSecondaryKrSatellite(taskInfo.ProcessID);
                this.Manager.SpecifySatelliteID(card.ID);
            }

            return this.StartRunnerAsync(taskInfo, cancellationToken);
        }

        protected override async Task<bool> ProcessSignalCoreAsync(IWorkflowSignalInfo signalInfo, CancellationToken cancellationToken = default)
        {
            if (signalInfo.ProcessTypeName == KrSecondaryProcessName)
            {
                var card = this.WCtx.KrScope.GetSecondaryKrSatellite(signalInfo.ProcessID);
                this.Manager.SpecifySatelliteID(card.ID, false);
            }

            await this.StartRunnerAsync(signalInfo, cancellationToken);
            return true;
        }

        #endregion

    }
}
