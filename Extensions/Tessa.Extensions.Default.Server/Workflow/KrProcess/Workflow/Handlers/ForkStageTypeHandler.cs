using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Properties.Resharper;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class ForkStageTypeHandler: ForkStageTypeHandlerBase
    {
        #region nested types

        // ReSharper disable once ClassCanBeSealed.Global
        public class BranchInfo: StorageObject
        {
            public BranchInfo(
                Guid rowID,
                Guid nestedWorkflowProcessID,
                Guid secondaryProcessID,
                bool completed) 
                : base(new Dictionary<string, object>())
            {
                this.Init(nameof(this.RowID), rowID);
                this.Init(nameof(this.ProcessID), nestedWorkflowProcessID);
                this.Init(nameof(this.SecondaryProcessID), secondaryProcessID);
                this.Init(nameof(this.Completed), BooleanBoxes.Box(completed));
            }

            /// <inheritdoc />
            public BranchInfo(
                Dictionary<string, object> storage)
                : base(storage)
            {
            }

            /// <inheritdoc />
            public BranchInfo(
                SerializationInfo info,
                StreamingContext context)
                : base(info, context)
            {
            }

            public Guid RowID => this.Get<Guid>(nameof(this.RowID));
            
            public Guid ProcessID => this.Get<Guid>(nameof(this.ProcessID));
            
            public Guid SecondaryProcessID => this.Get<Guid>(nameof(this.SecondaryProcessID));
            
            public bool Completed => this.Get<bool>(nameof(this.Completed));
        }
        
        // ReSharper disable once ClassCanBeSealed.Global
        public class ScriptContext
        {
            [UsedImplicitly]
            public bool SkipForkAndContinueRoute;
            
            [UsedImplicitly]
            public bool KeepBranchesAlive;
            
            [UsedImplicitly]
            public IDictionary<string, object> ProcessInfo;

            [UsedImplicitly]
            public IKrSecondaryProcess SecondaryProcess;

            [UsedImplicitly]
            public ListStorage<BranchInfo> BranchInfos;
        }

        #endregion

        #region fields

        public static readonly string ScriptContextParameterType = 
            $"global::{typeof(ForkStageTypeHandler).FullName}.{typeof(ScriptContext).Name}";
        
        public const string AfterNestedMethodName = "AfterNested";
        
        public const string MethodParameterName = "NestedProcessInfo";
        
        protected static readonly IStorageValueFactory<int, BranchInfo> BranchInfoFactory =
            new DictionaryStorageValueFactory<int, BranchInfo>(
                (key, storage) => new BranchInfo(storage));
        
        protected const string PendingProcesses = nameof(PendingProcesses);
        
        protected readonly IKrProcessLauncher ProcessLauncher;

        protected readonly IKrProcessCache ProcessCache;

        protected readonly IKrCompilationCache CompilationCache;

        protected readonly IUnityContainer UnityContainer;
        
        protected readonly IDbScope DbScope;

        #endregion

        #region constructor

        public ForkStageTypeHandler(
            IKrProcessLauncher processLauncher,
            IKrProcessCache processCache,
            IKrCompilationCache compilationCache,
            IUnityContainer unityContainer,
            IDbScope dbScope)
        {
            this.ProcessLauncher = processLauncher;
            this.ProcessCache = processCache;
            this.CompilationCache = compilationCache;
            this.UnityContainer = unityContainer;
            this.DbScope = dbScope;
        }

        #endregion

        #region base overrides
        
        /// <inheritdoc />
        public override StageHandlerResult HandleStageStart(
            IStageTypeHandlerContext context)
        {
            if (context.ProcessHolderSatellite != null)
            {
                NestedStagesCleaner.ClearStage(context.ProcessHolderSatellite, context.Stage.RowID);
            }

            var branchInfos = new ListStorage<BranchInfo>(new List<object>(), BranchInfoFactory);
            var processInfos = GetProcessInfos(context.Stage);
            var result = this.StartNestedProcesses(
                context,
                branchInfos, 
                processInfos,
                EnumerateSecondaryProcessRows(context));
            if (result != StageHandlerResult.EmptyResult)
            {
                return result;
            }

            context.Stage.InfoStorage[PendingProcesses] = branchInfos.GetStorage();
            
            return branchInfos.All(p => p.Completed)
                ? StageHandlerResult.CompleteResult
                : StageHandlerResult.InProgressResult;
        }

        /// <inheritdoc />
        public override StageHandlerResult HandleSignal(
            IStageTypeHandlerContext context)
        {
            if (context.SignalInfo.Signal.Name == KrConstants.AsyncForkedProcessCompletedSingal
                && context.SignalInfo.Signal.Parameters.TryGetValue(KrConstants.Keys.ProcessID, out var pidObj) 
                && pidObj is Guid pid)
            {
                return this.HandleBranchCompletion(context, pid);
            }
            if (context.SignalInfo.Signal.Name == KrConstants.ForkAddBranchSignal
                && context.SignalInfo.Signal.Parameters.TryGetValue(nameof(BranchAdditionInfo), out var bai) 
                && bai is List<object> baiStorage)
            {
                return this.HandleBranchAddition(context, new ListStorage<BranchAdditionInfo>(baiStorage, BranchAdditionInfoFactory));
            }
            if (context.SignalInfo.Signal.Name == KrConstants.ForkRemoveBranchSignal
                && context.SignalInfo.Signal.Parameters.TryGetValue(nameof(BranchRemovalInfo), out var bri) 
                && bri is Dictionary<string, object> briStorage)
            {
                return this.HandleBranchRemoval(context, new BranchRemovalInfo(briStorage));
            }
            return base.HandleSignal(context);
        }

        /// <inheritdoc />
        public override bool HandleStageInterrupt(
            IStageTypeHandlerContext context)
        {
            var branchInfos = new ListStorage<BranchInfo>((List<object>)context.Stage.InfoStorage[PendingProcesses], BranchInfoFactory);
            string signal;
            switch (context.DirectionAfterInterrupt)
            {
                case DirectionAfterInterrupt.Forward:
                    signal = KrConstants.KrSkipProcessGlobalSignal;
                    break;
                case DirectionAfterInterrupt.Backward:
                    signal = KrConstants.KrCancelProcessGlobalSignal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            foreach (var binfo in branchInfos.Where(p => !p.Completed))
            {
                context.MainCardAccessStrategy
                    .GetCard()
                    .GetWorkflowQueue()
                    .AddSignal(
                        KrConstants.KrNestedProcessName,
                        signal,
                        processID: binfo.ProcessID);
            }
            
            return true;
        }
        #endregion

        #region protected

        protected StageHandlerResult HandleBranchCompletion(
            IStageTypeHandlerContext context, 
            Guid pid)
        {
            var branchInfos = new ListStorage<BranchInfo>((List<object>)context.Stage.InfoStorage[PendingProcesses], BranchInfoFactory);
            
            // Если процесс окажется неизвестным или голос подает уже завершенный процесс,
            // то просто пропускаем, ничего не делая.
            var binfo = branchInfos.FirstOrDefault(p => p.ProcessID.Equals(pid));
            if (binfo is null
                || binfo.Completed)
            {
                return base.HandleSignal(context);
            }
            
            SetCompleted(binfo);

            var nestedProcessInfo = context
                .SignalInfo
                .Signal
                .Parameters
                .TryGet<IDictionary<string, object>>(KrConstants.Keys.ProcessInfoAtEnd);
            SetProcessInfo(GetProcessInfos(context.Stage), binfo.RowID, nestedProcessInfo);
            
            if (this.RunScript(
                context, 
                binfo, 
                context.SignalInfo.Signal.Parameters.TryGet<IDictionary<string, object>>(KrConstants.Keys.ProcessInfoAtEnd),
                branchInfos).SkipForkAndContinueRoute)
            {
                return StageHandlerResult.CompleteResult;
            }

            return branchInfos.All(p => p.Completed)
                ? StageHandlerResult.CompleteResult
                : StageHandlerResult.InProgressResult;
        }

        protected StageHandlerResult HandleBranchAddition(
            IStageTypeHandlerContext context,
            ListStorage<BranchAdditionInfo> additionInfos)
        {
            var stageRowID = context.Stage.RowID;
            var newRows = new List<object>(additionInfos.Count);
            var processInfos = new Dictionary<string, object>(additionInfos.Count);
            foreach (var ai in additionInfos)
            {
                var rowID = Guid.NewGuid();
                newRows.Add(new Dictionary<string, object>
                {
                    [KrConstants.RowID] = rowID,
                    [KrConstants.StageRowID] = stageRowID,
                    [KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessID] = ai.SecondaryProcessID,
                    [KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessName] = ai.SecondaryProcessName,
                });
                SetProcessInfo(processInfos, rowID, ai.StartingProcessInfo ?? new Dictionary<string, object>());
            }
            
            var branchInfos = new ListStorage<BranchInfo>((List<object>)context.Stage.InfoStorage[PendingProcesses], BranchInfoFactory);
            var result = this.StartNestedProcesses(context, branchInfos, processInfos, newRows.Cast<Dictionary<string, object>>());
            if (result != StageHandlerResult.EmptyResult)
            {
                return result;
            }
            
            context
                .Stage
                .SettingsStorage
                .TryGet<List<object>>(KrConstants.KrForkSecondaryProcessesSettingsVirtual.Synthetic)
                .AddRange(newRows);
            return StageHandlerResult.InProgressResult;
        }
        
        protected StageHandlerResult HandleBranchRemoval(
            IStageTypeHandlerContext context,
            BranchRemovalInfo branchRemovalInfo)
        {
            var branchInfos = new ListStorage<BranchInfo>((List<object>)context.Stage.InfoStorage[PendingProcesses], BranchInfoFactory);
            string signal;
            switch (branchRemovalInfo.DirectionAfterInterrupt)
            {
                case DirectionAfterInterrupt.Forward:
                    signal = KrConstants.KrSkipProcessGlobalSignal;
                    break;
                case DirectionAfterInterrupt.Backward:
                    signal = KrConstants.KrCancelProcessGlobalSignal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var sp = branchRemovalInfo.SecondaryProcesses;
            // Сателлит есть всегда, т.к. синхронный процесс не может получать сигналы.
            var np = this.GetProcessIDsOfNesteds(context.ProcessHolderSatellite.ID, branchInfos, branchRemovalInfo.NestedProcesses);
            foreach (var binfo in branchInfos
                .Where(p => !p.Completed && (sp.Contains(p.SecondaryProcessID) || np.Contains(p.ProcessID))))
            {
                context.MainCardAccessStrategy
                    .GetCard()
                    .GetWorkflowQueue()
                    .AddSignal(
                        KrConstants.KrNestedProcessName,
                        signal,
                        processID: binfo.ProcessID);

                SetCompleted(binfo);
            }
            
            return branchInfos.All(p => p.Completed)
                ? StageHandlerResult.CompleteResult
                : StageHandlerResult.InProgressResult;
        }
        
        protected ScriptContext RunScript(
            IStageTypeHandlerContext context,
            BranchInfo binfo,
            IDictionary<string, object> processInfo,
            ListStorage<BranchInfo> branchInfos)
        {
            var inst = HandlerHelper.CreateScriptInstance(
                this.CompilationCache,
                context.Stage.ID,
                context.ValidationResult);
            HandlerHelper.InitScriptContext(this.UnityContainer, inst, context);
            var ctx = new ScriptContext
            {
                SecondaryProcess = this.ProcessCache.GetSecondaryProcess(binfo.SecondaryProcessID),
                ProcessInfo = processInfo,
                BranchInfos = branchInfos,
            };
            inst.InvokeExtra(AfterNestedMethodName, ctx);

            if (ctx.SkipForkAndContinueRoute)
            {
                ProcessKeepBranchesAlive(context, ctx, branchInfos);
            }

            return ctx;
        }

        protected static void ProcessKeepBranchesAlive(
            IStageTypeHandlerContext context,
            ScriptContext ctx,
            ListStorage<BranchInfo> branchInfos)
        {
            if (ctx.KeepBranchesAlive)
            {
                return;
            }

            foreach (var binfo in branchInfos.Where(p => !p.Completed))
            {
                context.MainCardAccessStrategy
                    .GetCard()
                    .GetWorkflowQueue()
                    .AddSignal(
                        KrConstants.KrNestedProcessName,
                        KrConstants.KrSkipProcessGlobalSignal,
                        processID: binfo.ProcessID);
            }
        }

        protected bool CheckSecondaryProcess(
            Guid processID,
            IStageTypeHandlerContext context)
        {
            try
            {
                var process = this.ProcessCache.GetSecondaryProcess(processID);
                if (process.Async
                    && context.RunnerMode == KrProcessRunnerMode.Sync)
                {
                    context.ValidationResult.AddError("$KrStages_Fork_CannotRunAsyncIntoSync");
                    return false;
                }

                return true;
            }
            catch (InvalidOperationException)
            {
                context.ValidationResult.AddError("$KrStages_Fork_SecondaryProcessMissed");
                return false;
            }
        }

        protected StageHandlerResult StartNestedProcesses(
            IStageTypeHandlerContext context,
            ListStorage<BranchInfo> branchInfos,
            IDictionary<string, object> processInfos,
            IEnumerable<IDictionary<string, object>> secProcsRows)
        {
            var parentProcessTypeName = context.ProcessInfo?.ProcessTypeName;
            var parentProcessID = context.ProcessInfo?.ProcessID;
            var order = branchInfos.Count;
            foreach (var secondaryProcessRow in secProcsRows)
            {
                var rowID = secondaryProcessRow.TryGet<Guid>(KrConstants.RowID);
                var processID =
                    secondaryProcessRow.TryGet<Guid>(
                        KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessID);
                var processInfo = GetProcessInfo(processInfos, rowID);
                if (!this.CheckSecondaryProcess(processID, context))
                {
                    return StageHandlerResult.CompleteResult;
                }
                
                var forkProcessBuilder = KrProcessBuilder
                    .CreateProcess()
                    .SetProcess(processID)
                    .SetProcessInfo(processInfo);
                if (context.MainCardID.HasValue)
                {
                    forkProcessBuilder.SetCard(context.MainCardID.Value);
                }
                
                forkProcessBuilder.SetNestedProcess(
                    context.Stage.RowID,
                    parentProcessTypeName,
                    parentProcessID,
                    context.ProcessHolder.ProcessHolderID,
                    order++);
                var forkProcess = forkProcessBuilder.Build();

                var result = this.ProcessLauncher.Launch(forkProcess);
                context.ValidationResult.Add(result.ValidationResult);
                var complete = result.LaunchStatus == KrProcessLaunchStatus.Complete;
                var branchInfo = new BranchInfo(
                    rowID,
                    result.ProcessID ?? Guid.Empty,
                    processID,
                    complete);
                branchInfos.Add(branchInfo);
                
                if (complete)
                {
                    SetProcessInfo(processInfos, rowID, result.ProcessInfo);
                    if (this.RunScript(context, branchInfo, result.ProcessInfo, branchInfos).SkipForkAndContinueRoute)
                    {
                        return StageHandlerResult.CompleteResult;
                    }
                }
                if (result.LaunchStatus == KrProcessLaunchStatus.Error)
                {
                    break;
                }
            }
            return StageHandlerResult.EmptyResult;
        }

        protected IList<Guid> GetProcessIDsOfNesteds(
            Guid id,
            ListStorage<BranchInfo> branchInfos,
            IList<Guid> nestedProcessIDs)
        {
            if (nestedProcessIDs.Count == 0)
            {
                return EmptyHolder<Guid>.Collection;
            }
            
            var resultProcesses = new List<Guid>();
            using (this.DbScope.Create())
            {
                var db = this.DbScope.Db;
                var query = this.DbScope.BuilderFactory
                    .Select()
                    .C("RowID")
                    .C("Params")
                    .From("WorkflowProcesses").NoLock()
                    .Where()
                    .C("ID").Equals().P("ID")
                    .And().C("RowID").In(branchInfos.Where(p => !p.Completed).Select(p => p.ProcessID))
                    .Build();
                db
                    .SetCommand(query, db.Parameter("ID", id))
                    .LogCommand();

                using var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    var serialized = reader.GetNullableBytes(1);
                    if (serialized is null)
                    {
                        continue;
                    }
                    var serializableObject = new SerializableObject();
                    serializableObject.Deserialize(serialized);
                    var nestedProcessID = serializableObject.TryGet<Guid>(KrConstants.Keys.NestedProcessID);
                    if (nestedProcessIDs.Contains(nestedProcessID))
                    {
                        resultProcesses.Add(reader.GetGuid(0));   
                    }
                }
            }

            return resultProcesses;
        }
        
        protected static void SetCompleted(
            BranchInfo binfo)
        {
            // У BranchInfo.Completed отсутствует сеттер, чтобы нельзя его было сменить в скриптах
            // Но тут нам можно. Распространять такую практику не следует.
            binfo.GetStorage()[nameof(BranchInfo.Completed)] = BooleanBoxes.True;
        }
        
        #endregion
    }
}