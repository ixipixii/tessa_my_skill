using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Json;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public sealed class WorkflowProcess :
        IEquatable<WorkflowProcess>,
        ISealable
    {
        #region fields
        
        private string authorComment;
        private KrState state;
        private SealableObjectList<Stage> stages;
        private readonly Lazy<dynamic> infoDynamicLazy;
        private readonly Lazy<dynamic> mainProcessInfoLazy;

        private bool affectMainCardVersionWhenStateChanged = true;
        private Guid? currentApprovalStageRowID;
        private Author author;
        private long authorCommentTimestamp;
        private long authorTimestamp;
        private long stateTimestamp;
        private long affectMainCardVersionWhenStateChangedTimestamp;

        #endregion

        #region constructor

        private WorkflowProcess()
        {
            this.infoDynamicLazy = new Lazy<dynamic>(() => DynamicStorageAccessor.Create(this.InfoStorage), LazyThreadSafetyMode.PublicationOnly);
            this.mainProcessInfoLazy = new Lazy<dynamic>(() => DynamicStorageAccessor.Create(this.MainProcessInfoStorage), LazyThreadSafetyMode.PublicationOnly);
        }

        public WorkflowProcess(
            IDictionary<string, object> infoStorage,
            IDictionary<string, object> mainProcessInfoStorage,
            SealableObjectList<Stage> stages,
            bool saveInitialStages,
            Guid? nestedProcessID) : this()
        {
            this.InfoStorage = infoStorage;
            this.MainProcessInfoStorage = mainProcessInfoStorage;
            this.stages = stages;
            this.NestedProcessID = nestedProcessID;
            if (saveInitialStages)
            {
                this.InitialWorkflowProcess = new WorkflowProcess(this);
                this.InitialWorkflowProcess.Seal();
            }
        }

        private WorkflowProcess(
            WorkflowProcess workflowProcess,
            Func<Stage, bool> stageFilterPredicate = null,
            bool saveInitialStages = false) : this()
        {
            this.author = workflowProcess.Author;
            this.authorComment = workflowProcess.authorComment;
            this.state = workflowProcess.state;
            this.InfoStorage = StorageHelper.Clone(workflowProcess.InfoStorage);
            this.MainProcessInfoStorage = StorageHelper.Clone(workflowProcess.MainProcessInfoStorage);
            this.NestedProcessID = workflowProcess.NestedProcessID;

            this.stages = CopyStages(workflowProcess, stageFilterPredicate);

            if (saveInitialStages)
            {
                this.InitialWorkflowProcess = new WorkflowProcess(this);
                this.InitialWorkflowProcess.Seal();
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// ID вложенного процесса.
        /// </summary>
        public Guid? NestedProcessID { get; private set; }
        
        /// <summary>
        /// ID текущего активного этапа процесса.
        /// </summary>
        public Guid? CurrentApprovalStageRowID
        {
            get => this.currentApprovalStageRowID;
            set
            {
                Check.ObjectNotSealed(this);
                this.currentApprovalStageRowID = value;
            }
        }

        /// <summary>
        /// Дополнительная информация по процессу.
        /// </summary>
        public IDictionary<string, object> InfoStorage { get; private set; }

        /// <summary>
        /// Дополнительная информация по процессу.
        /// </summary>
        [JsonIgnore]
        public dynamic Info => this.infoDynamicLazy.Value;

        /// <summary>
        /// Дополнительная информация по корневому процессу. Актуально для вложенных,
        /// для корневого MainProcessInfo = Info
        /// </summary>
        public IDictionary<string, object> MainProcessInfoStorage { get; private set; }

        /// <summary>
        /// Дополнительная информация по корневому процессу. Актуально для вложенных,
        /// для корневого MainProcessInfo = Info
        /// </summary>
        [JsonIgnore]
        public dynamic MainProcessInfo => this.mainProcessInfoLazy.Value;

        /// <summary>
        /// Инициатор процесса
        /// </summary>
        public Author Author
        {
            get => this.author;
            set => this.SetAuthor(value);
        }

        /// <summary>
        /// Штамп времени изменения комментария автора.
        /// </summary>
        public long AuthorTimestamp => this.authorTimestamp;

        /// <summary>
        /// Комментарий инициатора.
        /// </summary>
        public string AuthorComment
        {
            get => this.authorComment;
            set => this.SetAuthorComment(value);
        }

        /// <summary>
        /// Штамп времени изменения комментария автора.
        /// </summary>
        public long AuthorCommentTimestamp => this.authorCommentTimestamp;

        /// <summary>
        /// Состояние процесса согласования.
        /// </summary>
        public KrState State
        {
            get => this.state;
            set => this.SetState(value);
        }

        /// <summary>
        /// Штамп времени изменения состояния.
        /// </summary>
        public long StateTimestamp => this.stateTimestamp;

        /// <summary>
        /// Версия основной карточки должна быть изменена, если состояние документа изменилось.
        /// </summary>
        public bool AffectMainCardVersionWhenStateChanged
        {
            get => this.affectMainCardVersionWhenStateChanged;
            set => this.SetAffectMainCardVersionWhenStateChanged(value);
        }

        /// <summary>
        /// Штамп времени изменения флага версии основной карточки.
        /// </summary>
        public long AffectMainCardVersionWhenStateChangedTimestamp => this.affectMainCardVersionWhenStateChangedTimestamp;

        /// <summary>
        /// Объектная модель этапов согласования.
        /// </summary>
        public SealableObjectList<Stage> Stages
        {
            get => this.stages;
            set
            {
                Check.ObjectNotSealed(this);
                this.stages = value;
            }
        }
        
        /// <summary>
        /// Начальное состояние этапов до выполнения воркера.
        /// </summary>
        public WorkflowProcess InitialWorkflowProcess { get; private set; }

        #endregion

        #region ISealable Members

        /// <summary>
        /// Признак того, что объект был защищён от изменений.
        /// </summary>
        public bool IsSealed { get; private set; }  // = false

        /// <summary>
        /// Защищает объект от изменений.
        /// </summary> 
        public void Seal()
        {
            this.IsSealed = true;
            this.stages.Seal();
        } 

        #endregion

        #region base overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is WorkflowProcess other 
                && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

        #endregion

        #region operators

        public static bool operator ==(
            WorkflowProcess left,
            WorkflowProcess right)
        {
            if (left is null
                && right is null)
            {
                return true;
            }
            return left?.Equals(right) == true;
        } 

        public static bool operator !=(WorkflowProcess left, WorkflowProcess right)
        {
            if (left is null
                && right is null)
            {
                return false;
            }
            return left?.Equals(right) != true;
        }

        #endregion

        #region public

        /// <summary>
        /// Установить инициатора процесса. В общем случае необходимо использовать свойство <see cref="Author"/>.
        /// </summary>
        /// <param name="value">Новый инициатор</param>
        /// <param name="withTimestamp">Проставить время изменения автора</param>
        public void SetAuthor(
            Author value,
            bool withTimestamp = true) =>
            this.SetWorkflowProcessValue(
                value,
                withTimestamp,
                out this.author, 
                ref this.authorTimestamp);

        /// <summary>
        /// Установить комментарий инициатора процесса.
        /// В общем случае необходимо использовать свойство <see cref="AuthorComment"/>.
        /// </summary>
        /// <param name="value">Новый комментарий</param>
        /// <param name="withTimestamp">Проставить время изменения комментария</param>
        public void SetAuthorComment(
            string value,
            bool withTimestamp = true) =>
            this.SetWorkflowProcessValue(
                value,
                withTimestamp,
                out this.authorComment, 
                ref this.authorCommentTimestamp);
        
        /// <summary>
        /// Установить состояние.
        /// В общем случае необходимо использовать свойство <see cref="State"/>.
        /// </summary>
        /// <param name="value">Новое состояние</param>
        /// <param name="withTimestamp">Проставить время изменения состояния</param>
        public void SetState(
            KrState value,
            bool withTimestamp = true) =>
            this.SetWorkflowProcessValue(
                value,
                withTimestamp,
                out this.state, 
                ref this.stateTimestamp);
        
        /// <summary>
        /// Установить флаг изменения версии документа при изменении состояния.
        /// В общем случае необходимо использовать свойство <see cref="AffectMainCardVersionWhenStateChanged"/>.
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <param name="withTimestamp">Проставить время изменения</param>
        public void SetAffectMainCardVersionWhenStateChanged(
            bool value,
            bool withTimestamp = true) =>
            this.SetWorkflowProcessValue(
                value,
                withTimestamp,
                out this.affectMainCardVersionWhenStateChanged, 
                ref this.affectMainCardVersionWhenStateChangedTimestamp);
        
        public WorkflowProcess CloneWithDedicatedStageGroup(
            Guid groupID,
            bool saveInitialStages = false)
        {
            return new WorkflowProcess(
                this,
                s => s.StageGroupID == groupID,
                saveInitialStages);
        }

        /// <inheritdoc />
        public bool Equals(WorkflowProcess other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var currentAuthor = this.Author;
            var otherAuthor = other.Author;
            var authorsAreEqual = (currentAuthor is null && otherAuthor is null)
                || currentAuthor?.Equals(otherAuthor) == true;

            return authorsAreEqual
                && string.Equals(this.AuthorComment, other.AuthorComment)
                && this.State == other.State;
        }

        #endregion

        #region private

        private static SealableObjectList<Stage> CopyStages(
            WorkflowProcess workflowProcess,
            Func<Stage, bool> actionPredicate = null)
        {
            return actionPredicate != null
                ? workflowProcess
                    .stages
                    .Where(actionPredicate)
                    .Select(p => new Stage(p))
                    .ToSealableObjectList()
                : workflowProcess
                    .stages
                    .Select(p => new Stage(p))
                    .ToSealableObjectList();
        }

        private static long GetTimestamp() => DateTime.UtcNow.Ticks;

        private void SetWorkflowProcessValue<T>(
            T value,
            bool withTimestamp,
            out T field,
            ref long timestamp)
        {
            Check.ObjectNotSealed(this);
            field = value;
            if(withTimestamp)
            {
                timestamp = GetTimestamp();
            }
        }
        
        #endregion
    }
}