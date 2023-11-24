using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public abstract class KrScript : IKrScript
    {
        #region fields

        internal const string ExtraMethodsName = nameof(ExtraMethods);

        // ReSharper disable once CollectionNeverUpdated.Global
        protected readonly Dictionary<string, Func<object, object>> ExtraMethods = new
            Dictionary<string, Func<object, object>>();

        private IKrSecondaryProcess secondaryProcess;
        private Guid stageGroupID;
        private string stageGroupName;
        private int stageGroupOrder;
        private Guid templateID;
        private string templateName;
        private int order;
        private GroupPosition position;
        private bool canChangeOrder;
        private bool isStagesReadonly;

        private StagesContainer stagesContainer;
        private WorkflowProcess workflowProcess;
        private Stage currentStage;

        private ICardExtensionContext cardContext;
        private ISession session;
        private IDbScope dbScope;
        private IUnityContainer unityContainer;
        private IValidationResultBuilder validationResult;
        private Guid? processID;
        private string processTypeName;
        private Card contextualSatellite;
        private Card processHolderSatellite;
        private ICardMetadata cardMetadata;
        private IKrScope krScope;

        private bool alreadyTriedToLoadContextualSatellite = false;
        private IKrTypesCache krTypesCache;
        private ICardCache cardCache;
        private IMainCardAccessStrategy mainCardAccessStrategy;
        private Guid cardID;
        private Guid cardTypeID;
        private string cardTypeName;
        private string cardTypeCaption;
        private Guid docTypeID;
        private IKrTaskHistoryResolver taskHistoryResolver;
        private KrProcessRunnerInitiationCause? initiationCause;
        private KrComponents? krComponents;
        private IWorkflowProcessInfo workflowProcessInfo;
        private IKrStageSerializer stageSerializer;

        #endregion

        #region IKrScope

        /// <inheritdoc />
        public Guid? ProcessID
        {
            get => this.processID;
            set
            {
                this.CheckSealed();
                this.processID = value;
            }
        }

        /// <inheritdoc />
        public string ProcessTypeName
        {
            get => this.processTypeName;
            set
            {
                this.CheckSealed();
                this.processTypeName = value;
            }
        }

        /// <inheritdoc />
        public KrProcessRunnerInitiationCause InitiationCause
        {
            get => this.initiationCause ??
                throw new InvalidOperationException(
                    $"{nameof(this.InitiationCause)} available only in runtime scripts.");
            set
            {
                this.CheckSealed();
                this.initiationCause = value;
            }
        }

        /// <inheritdoc />
        public Card ContextualSatellite
        {
            get
            {
                if (!this.alreadyTriedToLoadContextualSatellite
                    && this.contextualSatellite is null
                    && this.CardID != default(Guid))
                {
                    this.contextualSatellite = this.krScope.GetKrSatellite(this.cardID);
                    this.alreadyTriedToLoadContextualSatellite = true;
                }

                return this.contextualSatellite;
            }
            set
            {
                this.CheckSealed();
                this.contextualSatellite = value;
            }
        }

        /// <inheritdoc />
        public Card ProcessHolderSatellite
        {
            get => this.processHolderSatellite;
            set
            {
                this.CheckSealed();
                this.processHolderSatellite = value;
            }
        }

        /// <inheritdoc />
        public IKrSecondaryProcess SecondaryProcess
        {
            get => this.secondaryProcess;
            set
            {
                this.CheckSealed();
                this.secondaryProcess = value;
            }
        }

        /// <inheritdoc />
        public IKrPureProcess PureProcess => this.secondaryProcess as IKrPureProcess;

        /// <inheritdoc />
        public IKrProcessButton Button => this.secondaryProcess as IKrProcessButton;

        /// <inheritdoc />
        public IKrAction Action => this.secondaryProcess as IKrAction;

        /// <inheritdoc />
        public Guid StageGroupID
        {
            get => this.stageGroupID;
            set
            {
                this.CheckSealed();
                this.stageGroupID = value;
            }
        }

        /// <inheritdoc />
        public string StageGroupName
        {
            get => this.stageGroupName;
            set
            {
                this.CheckSealed();
                this.stageGroupName = value;
            }
        }

        /// <inheritdoc />
        public int StageGroupOrder
        {
            get => this.stageGroupOrder;
            set
            {
                this.CheckSealed();
                this.stageGroupOrder = value;
            }
        }

        /// <inheritdoc />
        public Guid TemplateID
        {
            get => this.templateID;
            set
            {
                this.CheckSealed();
                this.templateID = value;
            }
        }

        /// <inheritdoc />
        public string TemplateName
        {
            get => this.templateName;
            set
            {
                this.CheckSealed();
                this.templateName = value;
            }
        }

        /// <inheritdoc />
        public int Order
        {
            get => this.order;
            set
            {
                this.CheckSealed();
                this.order = value;
            }
        }

        /// <inheritdoc />
        public GroupPosition Position
        {
            get => this.position;
            set
            {
                this.CheckSealed();
                this.position = value;
            }
        }

        /// <inheritdoc />
        public bool CanChangeOrder
        {
            get => this.canChangeOrder;
            set
            {
                this.CheckSealed();
                this.canChangeOrder = value;
            }
        }

        /// <inheritdoc />
        public bool IsStagesReadonly
        {
            get => this.isStagesReadonly;
            set
            {
                this.CheckSealed();
                this.isStagesReadonly = value;
            }
        }

        /// <inheritdoc />
        public StagesContainer StagesContainer
        {
            get => this.stagesContainer;
            set
            {
                this.CheckSealed();
                this.stagesContainer = value;
            }
        }

        /// <inheritdoc />
        public WorkflowProcess WorkflowProcess
        {
            get => this.workflowProcess;
            set
            {
                this.CheckSealed();
                this.workflowProcess = value;
            }
        }

        /// <inheritdoc />
        public SealableObjectList<Stage> InitialStages => this.WorkflowProcess.InitialWorkflowProcess.Stages;

        /// <inheritdoc />
        public SealableObjectList<Stage> Stages => this.WorkflowProcess.Stages;

        /// <inheritdoc />
        public ReadOnlyCollection<Stage> CurrentStages
        {
            get
            {
                if (this.Stage != null)
                {
                    return new ReadOnlyCollection<Stage>(new[] { this.Stage });
                }

                if (this.TemplateID != Guid.Empty)
                {
                    return new ReadOnlyCollection<Stage>(
                        this.WorkflowProcess.Stages.Where(p => p.TemplateID == this.templateID).ToList());
                }

                return new ReadOnlyCollection<Stage>(
                    this.WorkflowProcess.Stages.Where(p => p.StageGroupID == this.StageGroupID).ToList());
            }
        }


        /// <inheritdoc />
        public Stage Stage
        {
            get => this.currentStage;
            set
            {
                this.CheckSealed();
                this.currentStage = value;
            }
        }

        /// <inheritdoc />
        public int Cycle
        {
            get => UserAPIHelper.GetCycle(this);
            set
            {
                this.CheckSealed();
                UserAPIHelper.SetCycle(this, value);
            }
        }

        /// <inheritdoc />
        public Author Initiator
        {
            get => this.WorkflowProcess.Author;
            set
            {
                this.CheckSealed();
                this.WorkflowProcess.Author = value;
            }
        }

        /// <inheritdoc />
        public string InitiatorComment
        {
            get => this.WorkflowProcess.AuthorComment;
            set
            {
                this.CheckSealed();
                this.WorkflowProcess.AuthorComment = value;
            }
        }

        /// <inheritdoc />
        public IMainCardAccessStrategy MainCardAccessStrategy
        {
            get => this.mainCardAccessStrategy;
            set
            {
                this.CheckSealed();
                this.mainCardAccessStrategy = value;
            }
        }

        /// <inheritdoc />
        public Card CardObject => this.mainCardAccessStrategy.GetCard();

        /// <inheritdoc />
        public IWorkflowProcessInfo WorkflowProcessInfo
        {
            get => this.workflowProcessInfo;
            set
            {
                this.CheckSealed();
                this.workflowProcessInfo = value;
            }
        }

        /// <inheritdoc />
        public IWorkflowTaskInfo WorkflowTaskInfo => this.workflowProcessInfo as IWorkflowTaskInfo;

        /// <inheritdoc />
        public IWorkflowSignalInfo WorkflowSignalInfo => this.workflowProcessInfo as IWorkflowSignalInfo;

        /// <inheritdoc />
        public Guid CardID
        {
            get => this.cardID;
            set
            {
                this.CheckSealed();
                this.cardID = value;
            }
        }

        /// <inheritdoc />
        public Guid CardTypeID
        {
            get => this.cardTypeID;
            set
            {
                this.CheckSealed();
                this.cardTypeID = value;
            }
        }

        /// <inheritdoc />
        public string CardTypeName
        {
            get => this.cardTypeName;
            set
            {
                this.CheckSealed();
                this.cardTypeName = value;
            }
        }

        /// <inheritdoc />
        public string CardTypeCaption
        {
            get => this.cardTypeCaption;
            set
            {
                this.CheckSealed();
                this.cardTypeCaption = value;
            }
        }

        /// <inheritdoc />
        public Guid DocTypeID
        {
            get => this.docTypeID;
            set
            {
                this.CheckSealed();
                this.docTypeID = value;
            }
        }

        /// <inheritdoc />
        public Guid TypeID =>
            this.DocTypeID != default(Guid)
                ? this.DocTypeID
                : this.CardTypeID;

        /// <inheritdoc />
        public KrComponents KrComponents
        {
            get => this.krComponents ??
                throw new InvalidOperationException($"{nameof(this.KrComponents)} available only in local scripts.");
            set
            {
                this.CheckSealed();
                this.krComponents = value;
            }
        }

        /// <inheritdoc />
        public int Version => this.mainCardAccessStrategy.GetCard()?.Version ?? -1;

        /// <inheritdoc />
        public dynamic Card => this.mainCardAccessStrategy.GetCard()?.DynamicEntries;

        /// <inheritdoc />
        public dynamic CardTables => this.mainCardAccessStrategy.GetCard()?.DynamicTables;

        /// <inheritdoc />
        public ListStorage<CardFile> Files => this.mainCardAccessStrategy.GetCard()?.Files;

        /// <inheritdoc />
        public ICardExtensionContext CardContext
        {
            get => this.cardContext;
            set
            {
                this.CheckSealed();
                this.cardContext = value;
            }
        }

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult
        {
            get => this.validationResult;
            set
            {
                this.CheckSealed();
                this.validationResult = value;
            }
        }

        /// <inheritdoc />
        public IDictionary<string, object> Info { get; } = new Dictionary<string, object>();

        /// <inheritdoc />
        public IDictionary<string, object> StageInfoStorage => this.Stage?.InfoStorage;

        /// <inheritdoc />
        public dynamic StageInfo => this.Stage?.Info;

        /// <inheritdoc />
        public IDictionary<string, object> ProcessInfoStorage => this.WorkflowProcess.InfoStorage;

        /// <inheritdoc />
        public dynamic ProcessInfo => this.WorkflowProcess.Info;

        /// <inheritdoc />
        public Guid? CurrentTaskHistoryGroup => UserAPIHelper.GetCurrentTaskHistoryGroup(this);

        /// <inheritdoc />
        public IDictionary<string, object> MainProcessInfoStorage => this.WorkflowProcess.MainProcessInfoStorage;

        /// <inheritdoc />
        public dynamic MainProcessInfo => this.WorkflowProcess.MainProcessInfo;

        /// <inheritdoc />
        public dynamic NewCard => this.GetNewCard().Entries;

        /// <inheritdoc />
        public dynamic NewCardTables => this.GetNewCard().Tables;

        /// <inheritdoc />
        public ISession Session
        {
            get => this.session;
            set
            {
                this.CheckSealed();
                this.session = value;
            }
        }

        /// <inheritdoc />
        public IDbScope DbScope
        {
            get => this.dbScope;
            set
            {
                this.CheckSealed();
                this.dbScope = value;
            }
        }

        /// <inheritdoc />
        public DbManager Db => this.DbScope.Db;

        /// <inheritdoc />
        public IUnityContainer UnityContainer
        {
            get => this.unityContainer;
            set
            {
                this.CheckSealed();
                this.unityContainer = value;
            }
        }

        /// <inheritdoc />
        public ICardMetadata CardMetadata
        {
            get => this.cardMetadata;
            set
            {
                this.CheckSealed();
                this.cardMetadata = value;
            }
        }

        /// <inheritdoc />
        public IKrScope KrScope
        {
            get => this.krScope;
            set
            {
                this.CheckSealed();
                this.krScope = value;
            }
        }

        /// <inheritdoc />
        public ICardCache CardCache
        {
            get => this.cardCache;
            set
            {
                this.CheckSealed();
                this.cardCache = value;
            }
        }

        /// <inheritdoc />
        public IKrTypesCache KrTypesCache
        {
            get => this.krTypesCache;
            set
            {
                this.CheckSealed();
                this.krTypesCache = value;
            }
        }

        /// <inheritdoc />
        public ICardTaskHistoryManager TaskHistoryManager => this.taskHistoryResolver.TaskHistoryManager;

        /// <inheritdoc />
        public IKrTaskHistoryResolver TaskHistoryResolver
        {
            get => this.taskHistoryResolver;
            set
            {
                this.CheckSealed();
                this.taskHistoryResolver = value;
            }
        }

        /// <inheritdoc />
        public IKrStageSerializer StageSerializer
        {
            get => this.stageSerializer;
            set
            {
                this.CheckSealed();
                this.stageSerializer = value;
            }
        }

        /// <inheritdoc />
        public bool Confirmed { get; set; }

        /// <inheritdoc />
        public KrScriptType KrScriptType { get; set; }

        /// <inheritdoc />
        public ListStorage<CardRow> CardRows(
            string sectionName) => UserAPIHelper.CardRows(this, sectionName);

        /// <inheritdoc />
        public bool IsMainProcess() => UserAPIHelper.IsMainProcess(this);

        /// <inheritdoc />
        public bool IsMainProcessStarted() => UserAPIHelper.IsMainProcessStarted(this);

        /// <inheritdoc />
        public bool IsMainProcessInactive() => UserAPIHelper.IsMainProcessInactive(this, this.contextualSatellite);

        /// <inheritdoc />
        public void RunNextStageInContext(
            Guid cID,
            bool wholeCurrentGroup = false,
            IDictionary<string, object> processInfo = null) =>
            UserAPIContextChangeableHelper.RunNextStageInContext(this, this, cID, wholeCurrentGroup, processInfo);

        /// <inheritdoc />
        public bool ContextChangePending() => UserAPIContextChangeableHelper.ContextChangePending(this);

        /// <inheritdoc />
        public void DoNotChangeContext() => UserAPIContextChangeableHelper.DoNotChangeContext(this);

        /// <inheritdoc />
        public T Resolve<T>(
            string name = null) => UserAPIHelper.Resolve<T>(this.unityContainer, name);

        /// <inheritdoc />
        public void Show(
            object obj) => UserAPIIOHelper.Show(this, obj);

        /// <inheritdoc />
        public void Show(
            string message,
            string details = "") => UserAPIIOHelper.Show(this, message, details);

        /// <inheritdoc />
        public void Show(
            Stage stage) => UserAPIIOHelper.Show(this, stage);

        /// <inheritdoc />
        public void Show(
            IEnumerable<Stage> stages) => UserAPIIOHelper.Show(this, stages);

        /// <inheritdoc />
        public void Show(
            Performer performer) => UserAPIIOHelper.Show(this, performer);

        /// <inheritdoc />
        public void Show(
            IEnumerable<Performer> performers) => UserAPIIOHelper.Show(this, performers);

        /// <inheritdoc />
        public void Show(
            IDictionary<string, object> storage) => UserAPIIOHelper.Show(this, storage);

        /// <inheritdoc />
        public void Show(
            IStorageDictionaryProvider storage) => UserAPIIOHelper.Show(this, storage);

        /// <inheritdoc />
        public void AddError(
            string text) => UserAPIIOHelper.AddError(this, text);

        /// <inheritdoc />
        public void AddWarning(
            string text) => UserAPIIOHelper.AddWarning(this, text);

        /// <inheritdoc />
        public void AddInfo(
            string text) => UserAPIIOHelper.AddInfo(this, text);

        /// <inheritdoc />
        public void ForEachStage(
            Action<CardRow> rowAction,
            bool withNesteds = false) => UserAPIHelper.ForEachStage(this, rowAction, withNesteds);

        /// <inheritdoc />
        public void ForEachStageInMainProcess(
            Action<CardRow> rowAction,
            bool withNesteds = false) => UserAPIHelper.ForEachStageInMainProcess(this, rowAction, withNesteds);

        /// <inheritdoc />
        public void SetStageState(
            CardRow stage,
            KrStageState stageState) => UserAPIHelper.SetStageState(this, stage, stageState);

        /// <inheritdoc />
        public Stage GetOrAddStage(
            string name,
            StageTypeDescriptor descriptor,
            int pos = int.MaxValue) =>
            UserAPIHelper.GetOrAddStage(this, name, descriptor, pos);

        /// <inheritdoc />
        public Stage AddStage(
            string name,
            StageTypeDescriptor descriptor,
            int pos = int.MaxValue) =>
            UserAPIHelper.AddStage(this, name, descriptor, pos);

        /// <inheritdoc />
        public bool RemoveStage(
            string name) =>
            UserAPIHelper.RemoveStage(this, name);

        /// <inheritdoc />
        public void SetSinglePerformer(
            Guid id,
            string name,
            Stage intoStage,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.SetSinglePerformer(id, name, intoStage, ignoreManualChanges);

        /// <inheritdoc />
        public void SetSinglePerformer(
            string id,
            string name,
            Stage intoStage,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.SetSinglePerformer(Guid.Parse(id), name, intoStage, ignoreManualChanges);

        /// <inheritdoc />
        public void ResetSinglePerformer(
            Stage stage,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.ResetSinglePerformer(stage, ignoreManualChanges);

        /// <inheritdoc />
        public Performer AddPerformer(
            Guid id,
            string name,
            Stage intoStage,
            int pos = int.MaxValue,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.AddPerformer(this, id, name, intoStage, pos, ignoreManualChanges);

        /// <inheritdoc />
        public Performer AddPerformer(
            string id,
            string name,
            Stage intoStage,
            int pos = int.MaxValue,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.AddPerformer(this, Guid.Parse(id), name, intoStage, pos, ignoreManualChanges);

        /// <inheritdoc />
        public void RemovePerformer(
            Guid performerID,
            Stage stage,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.RemovePerformer(
                stage.Performers.Where(p => p.PerformerID == performerID).Select(p => p.RowID).ToArray(),
                stage,
                ignoreManualChanges);

        /// <inheritdoc />
        public void RemovePerformer(
            string performerID,
            Stage stage,
            bool ignoreManualChanges = false) =>
            this.RemovePerformer(Guid.Parse(performerID), stage, ignoreManualChanges);

        /// <inheritdoc />
        public void RemovePerformer(
            int index,
            Stage stage,
            bool ignoreManualChanges = false) =>
            UserAPIHelper.RemovePerformer(
                new [] { stage.Performers[index].RowID },
                stage,
                ignoreManualChanges);

        /// <inheritdoc />
        public void AddTaskHistoryRecord(
            Guid typeID,
            string typeName,
            string typeCaption,
            Guid optionID,
            string result = null,
            Guid? performerID = null,
            string performerName = null,
            int? cycle = null,
            int? timeZoneID = null,
            TimeSpan? timeZoneUTCOffset = null,
            Action<CardTaskHistoryItem> modifyAction = null)
        {
            UserAPIHelper.AddTaskHistoryRecord(
                this,
                this.CurrentTaskHistoryGroup,
                typeID,
                typeName,
                typeCaption,
                optionID,
                result,
                performerID,
                performerName,
                cycle,
                timeZoneID,
                timeZoneUTCOffset,
                modifyAction);
        }

        /// <inheritdoc />
        public void AddTaskHistoryRecord(
            Guid? taskHistoryGroup,
            Guid typeID,
            string typeName,
            string typeCaption,
            Guid optionID,
            string result = null,
            Guid? performerID = null,
            string performerName = null,
            int? cycle = null,
            int? timeZoneID = null,
            TimeSpan? timeZoneUTCOffset = null,
            Action<CardTaskHistoryItem> modifyAction = null) =>
            UserAPIHelper.AddTaskHistoryRecord(
                this,
                taskHistoryGroup,
                typeID,
                typeName,
                typeCaption,
                optionID,
                result,
                performerID,
                performerName,
                cycle,
                timeZoneID,
                timeZoneUTCOffset,
                modifyAction);

        /// <inheritdoc />
        public CardTaskHistoryGroup ResolveTaskHistoryGroup(
            Guid groupTypeID,
            Guid? parentGroupTypeID = null,
            bool newIteration = false) =>
            UserAPIHelper.ResolveTaskHistoryGroup(this, groupTypeID, parentGroupTypeID, newIteration);

        /// <inheritdoc />
        public bool HasKrComponents(
            KrComponents components) => UserAPIHelper.HasKrComponents(this, components);

        /// <inheritdoc />
        public bool HasKrComponents(
            params KrComponents[] components) => UserAPIHelper.HasKrComponents(this, components);

        /// <inheritdoc />
        public ISerializableObject GetPrimaryProcessInfo(
            Guid? mainCardID = null) => UserAPIHelper.GetPrimaryProcessInfo(this, mainCardID);

        /// <inheritdoc />
        public ISerializableObject GetSecondaryProcessInfo(
            Guid secondaryProcessID,
            Guid? mainCardID = null) => UserAPIHelper.GetSecondaryProcessInfo(this, secondaryProcessID, mainCardID);

        /// <inheritdoc />
        public void InvokeExtra(
            string name,
            object context,
            bool throwOnError = true)
        {
            this.InvokeExtra<object>(name, context, throwOnError);
        }

        /// <inheritdoc />
        public T InvokeExtra<T>(
            string name,
            object context,
            bool throwOnError = true)
        {
            if (!this.ExtraMethods.TryGetValue(name, out var method))
            {
                if (throwOnError)
                {
                    throw new KeyNotFoundException(
                        $"Method {name} does not initialized properly in {this.GetType().FullName}");
                }

                return default;
            }

            return (T) method.Invoke(context);
        }

        /// <inheritdoc />
        public Card GetNewCard() => UserAPIHelper.GetNewCard(this);

        /// <inheritdoc />
        public IDictionary<string, object> GetProcessInfoForBranch(
            Guid rowID) => UserAPIHelper.GetProcessInfoForBranch(this, rowID);

        /// <inheritdoc />
        public IDictionary<string, object> GetProcessInfoForBranch(
            string rowID) => UserAPIHelper.GetProcessInfoForBranch(this, Guid.Parse(rowID));

        #endregion

        #region IKrProcessItemScript

        /// <inheritdoc />
        public virtual void RunBefore() => UserAPIKrProcessItemHelper.RunBefore(this, this);

        /// <inheritdoc />
        public virtual void Before() => UserAPIKrProcessItemHelper.DefaultAction();

        /// <inheritdoc />
        public virtual void RunAfter() => UserAPIKrProcessItemHelper.RunAfter(this, this);

        /// <inheritdoc />
        public virtual void After() => UserAPIKrProcessItemHelper.DefaultAction();

        /// <inheritdoc />
        public bool RunCondition() => UserAPIKrProcessItemHelper.RunCondition(this, this);

        /// <inheritdoc />
        public virtual bool Condition() => (bool) UserAPIKrProcessItemHelper.DefaultAction();

        #endregion

        #region IKrSecondaryProcessScript

        /// <inheritdoc />
        public bool RunVisibility() => UserAPIKrProcessVisibilityHelper.RunVisibility(this, this);

        /// <inheritdoc />
        public virtual bool Visibility() => (bool) UserAPIKrProcessVisibilityHelper.DefaultAction();

        /// <inheritdoc />
        public bool RunExecution() => UserAPIKrProcessExecutionHelper.RunExecution(this, this);

        /// <inheritdoc />
        public virtual bool Execution() => (bool) UserAPIKrProcessVisibilityHelper.DefaultAction();

        #endregion

        #region IContextChangeableScript

        /// <inheritdoc />
        public Guid? DifferentContextCardID { get; set; }

        /// <inheritdoc />
        public bool DifferentContextWholeCurrentGroup { get; set; }

        /// <inheritdoc />
        public KrScriptType? DifferentContextSetupScriptType { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> DifferentContextProcessInfo { get; set; }

        #endregion

        #region ISealable Members

        /// <summary>
        /// Признак того, что объект был защищён от изменений.
        /// </summary>
        public bool IsSealed { get; private set; } = false;

        /// <summary>
        /// Защищает объект от изменений.
        /// </summary>
        public void Seal()
        {
            this.IsSealed = true;
        }

        /// <summary>
        /// Выбрасывает исключение Tessa.Platform.ObjectSealedException",
        /// если объект был защищён от изменений.
        /// </summary>
        private void CheckSealed()
        {
            Check.ObjectNotSealed(this);
        }

        #endregion
    }
}