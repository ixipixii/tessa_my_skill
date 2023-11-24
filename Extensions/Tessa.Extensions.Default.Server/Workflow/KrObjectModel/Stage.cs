using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Json;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public sealed class Stage : 
        IEquatable<Stage>,
        ISealable
    {
        #region nested types

        private sealed class PerformerObjectComparer : IComparer<object>
        {
            /// <inheritdoc />
            public int Compare(
                object x,
                object y)
            {
                var firstOrder = (x as IDictionary<string, object>)?.TryGet(KrConstants.Order, 0) ?? 0;
                var secondOrder = (y as IDictionary<string, object>)?.TryGet(KrConstants.Order, 0) ?? 0;
                return firstOrder - secondOrder;
            }
        }

        #endregion

        #region fields

        private const double DefaultTimeLimit = 1.0;

        private const double Epsilon = 0.01;

        private static readonly PerformerObjectComparer performerObjectComparer = new PerformerObjectComparer();

        private static readonly IStorageValueFactory<int, Performer> multiPerformerFactory =
            new DictionaryStorageValueFactory<int, Performer>((key, storage) => new MultiPerformer(storage));

        private string name;
        private double? timeLimit;
        private DateTime? planned;
        private KrStageState state = KrStageState.Inactive;
        private int? templateStageOrder;
        private Guid? stageTypeID;
        private string stageTypeCaption;
        private IDictionary<string, object> settings;
        private Lazy<dynamic> settingsDynamicLazy;
        private Lazy<dynamic> infoDynamicLazy;
        private bool hidden;
        private ListStorage<Performer> performers;
        private bool? authorExists;
        private AuthorProxy author;
        private bool? performerExists;
        private SinglePerformerProxy performer;
        
        /// <summary>
        /// Признак пропуска этапа.
        /// </summary>
        private bool skip;

        /// <summary>
        /// Флаг, показывающий, разрешён ли пропуск этапа.
        /// </summary>
        private bool canBeSkipped;

        #endregion

        #region constructors

        public Stage()
        {
            this.InitLazyDynamics();
        }

        /// <summary>
        /// Конструктор копирования. Запечатанность не переносится.
        /// </summary>
        /// <param name="stage"></param>
        public Stage(Stage stage) : this()
        {
            this.TemplateID = stage.TemplateID;
            this.TemplateName = stage.TemplateName;
            this.GroupPosition = stage.GroupPosition;
            this.CanChangeOrder = stage.CanChangeOrder;
            this.TemplateOrder = stage.TemplateOrder;
            this.IsStageReadonly = stage.IsStageReadonly;

            this.RowID = stage.RowID;
            this.ID = stage.ID;
            this.stageTypeID = stage.StageTypeID;
            this.stageTypeCaption = stage.StageTypeCaption;
            this.StageGroupID = stage.StageGroupID;
            this.StageGroupOrder = stage.StageGroupOrder;
            this.BasedOnTemplateStage = stage.BasedOnTemplateStage;
            this.name = stage.name;
            this.timeLimit = stage.timeLimit;
            this.planned = stage.planned;
            this.hidden = stage.hidden;
            this.state = stage.state;
            this.SqlPerformers = stage.SqlPerformers;
            this.SqlPerformersIndex = stage.SqlPerformersIndex;
            this.Skip = stage.Skip;
            this.CanBeSkipped = stage.CanBeSkipped;

            this.RowChanged = stage.RowChanged;
            this.OrderChanged = stage.OrderChanged;

            this.settings = StorageHelper.Clone(stage.SettingsStorage);
            this.InfoStorage = StorageHelper.Clone(stage.InfoStorage);

            this.InitialStage = stage.InitialStage;
            this.Ancestor = stage.Ancestor;
            this.TemplateStageOrder = stage.TemplateStageOrder;
        }

        /// <summary>
        /// Создание нового пустого этапа согласования
        /// Предназначено для использования в пользовательском коде
        /// </summary>
        /// <param name="name">Название этапа</param>
        /// <param name="stageTypeID"></param>
        /// <param name="stageTypeCaption"></param>
        public Stage(string name, Guid stageTypeID, string stageTypeCaption) :
            this(Guid.NewGuid(),
                 name,
                 stageTypeID,
                 stageTypeCaption,
                 Guid.Empty,
                 -1,
                 null,
                 null,
                 null,
                 false,
                 GroupPosition.Unspecified)
        {
        }

        /// <summary>
        /// Создание нового этапа согласования с привязкой к шаблону
        /// Предназначено для использования в пользовательском коде
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="stageTypeID"></param>
        /// <param name="stageTypeCaption"></param>
        /// <param name="stageGroupID"></param>
        /// <param name="stageGroupOrder"></param>
        /// <param name="templateID"></param>
        /// <param name="templateName"></param>
        /// <param name="templateOrder"></param>
        /// <param name="canChangeOrder"></param>
        /// <param name="groupPosition"></param>
        /// <param name="ancestor"></param>
        /// <param name="isStageReadonly"></param>
        /// <param name="timeLimit"></param>
        /// <param name="planned"></param>
        /// <param name="hidden"></param>
        /// <param name="stageState"></param>
        /// <param name="skip">Признак пропуска этапа.</param>
        /// <param name="canBeSkipped">Флаг, показывающий, разрешено ли пропускать этап.</param>
        public Stage(
            Guid id,
            string name,
            Guid stageTypeID,
            string stageTypeCaption,
            Guid stageGroupID,
            int stageGroupOrder,
            Guid? templateID,
            string templateName,
            int? templateOrder,
            bool canChangeOrder,
            GroupPosition groupPosition,
            Stage ancestor = null,
            bool isStageReadonly = true,
            int timeLimit = 1,
            DateTime? planned = null,
            bool hidden = false,
            KrStageState? stageState = null,
            bool skip = default,
            bool canBeSkipped = default) : this()
        {
            stageState = stageState ?? KrStageState.Inactive;

            this.TemplateID = templateID;
            this.TemplateName = templateName;
            this.GroupPosition = groupPosition;
            this.CanChangeOrder = canChangeOrder;
            this.TemplateOrder = templateOrder;
            this.IsStageReadonly = isStageReadonly;

            this.RowID = id;
            this.ID = id;
            this.stageTypeID = stageTypeID;
            this.stageTypeCaption = stageTypeCaption;
            this.StageGroupID = stageGroupID;
            this.StageGroupOrder = stageGroupOrder;
            this.BasedOnTemplateStage = false;
            this.name = name;
            this.timeLimit = timeLimit;
            this.planned = planned;
            this.hidden = hidden;
            this.state = stageState.Value;
            this.SqlPerformers = string.Empty;
            this.SqlPerformersIndex = -1;
            this.Skip = skip;
            this.CanBeSkipped = canBeSkipped;

            this.RowChanged = false;
            this.OrderChanged = false;

            this.settings = new Dictionary<string, object>();
            this.InfoStorage = new Dictionary<string, object>();

            this.InitialStage = false;
            this.Ancestor = ancestor;
            this.TemplateStageOrder = 0;


            this.ID = id;
            this.Name = name;
            this.StageGroupID = stageGroupID;
            this.StageGroupOrder = stageGroupOrder;

            this.TemplateID = templateID;
            this.TemplateName = templateName;
            this.TemplateOrder = templateOrder;
            this.CanChangeOrder = canChangeOrder;
            this.GroupPosition = groupPosition ?? GroupPosition.Unspecified;
            this.Ancestor = ancestor;
            this.IsStageReadonly = isStageReadonly;

            this.InfoStorage = new Dictionary<string, object>();

            this.BasedOnTemplateStage = false;
            this.InitialStage = false;
        }

        public Stage(
            IKrRuntimeStage runtimeStage,
            IKrStageTemplate stageTemplate = null,
            bool initialStage = false) : this()
        {
            this.InitialStage = initialStage;
            // Создавая по IKrRuntimeStage не будет объекта-предшественника.
            this.Ancestor = null;
            this.FillStageProperties(runtimeStage);
            
            this.settings = runtimeStage.GetSettings();
            this.InfoStorage = new Dictionary<string, object>();
            
            // Полное создание по шаблону. 
            // StageRow - строка из карточки KrStageTemplates
            this.ID = runtimeStage.StageID;
            this.BasedOnTemplateStage = true;
            // Только создаем строку - ID для новой строки в документе
            this.RowID = Guid.NewGuid();

            int? sqlApproverIndex = this.Performers.IndexOf(p => p.PerformerID == KrConstants.SqlApproverRoleID);
            if (sqlApproverIndex != -1)
            {
                this.Performers.RemoveAt(sqlApproverIndex.Value);
            }
            
            var performersObj =  this.settings?[KrConstants.KrPerformersVirtual.Synthetic];
            if (performersObj is IList<object> perfList)
            {
                for (var i = 0; i < perfList.Count; i++)
                {
                    if (perfList[i] is IDictionary<string, object> perf
                        && perf.TryGet<Guid?>(KrConstants.KrPerformersVirtual.PerformerID) ==
                        KrConstants.SqlApproverRoleID
                        && perf.TryGetValue(KrConstants.Order, out var ord)
                        && ord is int order)
                    {
                        this.SqlPerformersIndex = order;
                    }
                }
            }

            this.SqlPerformers = runtimeStage.SqlRoles;

            this.FillTemplateProperties(stageTemplate, runtimeStage);
            
        }
        
        public Stage(
            CardRow stageRow,
            IDictionary<string, object> settings,
            IDictionary<string, object> infoStorage,
            IKrStageTemplate stageTemplate = null,
            IReadOnlyCollection<IKrRuntimeStage> stages = null,
            bool initialStage = false) : this()
        {
            this.InitialStage = initialStage;
            // Создавая по CardRow не будет объекта-предшественника.
            this.Ancestor = null;
            this.FillStageProperties(stageRow);

            this.settings = settings;
            this.InfoStorage = infoStorage;

            var hasTemplateID = stageRow.TryGetValue(KrConstants.KrStages.BasedOnStageTemplateID, out var basedOnTemplateID) && basedOnTemplateID != null;
            var hasStageRowID = stageRow.TryGetValue(KrConstants.KrStages.BasedOnStageRowID, out var basedOnStageRowID) && basedOnStageRowID != null;

            if (stageTemplate != null
                && stages != null)
            {
                IKrRuntimeStage stagePrototype = null;
                var templateStage = stages.FirstOrDefault(p => p.StageID == stageRow.RowID);
                if (templateStage != null)
                {
                    // Полное создание по шаблону. 
                    // StageRow - строка из карточки KrStageTemplates
                    this.ID = stageRow.RowID;
                    this.BasedOnTemplateStage = true;
                    // Только создаем строку - ID для новой строки в документе
                    this.RowID = Guid.NewGuid();

                    stagePrototype = templateStage;
                    
                    int? sqlApproverIndex = this.Performers.IndexOf(p => p.PerformerID == KrConstants.SqlApproverRoleID);
                    if (sqlApproverIndex != -1)
                    {
                        this.Performers.RemoveAt(sqlApproverIndex.Value);
                    }
                }
                else if (hasTemplateID && hasStageRowID)
                {
                    // Этап в карточке был ранее создан подстановкой из таблицы этапов в шаблоне
                    // в таблицу этапов карточки
                    this.ID = (Guid)basedOnStageRowID;
                    this.BasedOnTemplateStage = true;
                    // ID для строки в документе
                    this.RowID = stageRow.RowID;
                    
                    stagePrototype = stages.FirstOrDefault(p => (Guid)basedOnStageRowID == p.StageID);
                }
                else if (hasTemplateID)
                {
                    // Этап был создан с привязкой к карточке, 
                    // но без привязки к конкретному этапу в шаблоне
                    this.ID = stageRow.RowID;
                    this.BasedOnTemplateStage = false;
                    this.RowID = stageRow.RowID;
                }

                var performersObj = this.settings?[KrConstants.KrPerformersVirtual.Synthetic];
                if (performersObj is IList<object> perfList)
                {
                    for (var i = 0; i < perfList.Count; i++)
                    {
                        if (perfList[i] is IDictionary<string, object> perf
                            && perf.TryGet<Guid?>(KrConstants.KrPerformersVirtual.PerformerID) ==
                            KrConstants.SqlApproverRoleID
                            && perf.TryGetValue(KrConstants.Order, out var ord)
                            && ord is int order)
                        {
                            this.SqlPerformersIndex = order;
                        }
                    }
                }

                this.SqlPerformers = stagePrototype?.SqlRoles;

                this.FillTemplateProperties(stageRow, stageTemplate, stagePrototype);
            }
            else
            {
                // stageTemplate == null - этап ручной или шаблон удален.
                this.ID = stageRow.RowID;
                this.RowID = stageRow.RowID;
                this.BasedOnTemplateStage = false;
                this.FillTemplatePropertiesByDefaultValues();

                if (hasTemplateID)
                {
                    // Этап был создан по шаблону, но сам шаблон уже удален
                    if (hasStageRowID)
                    {
                        // Это этап из таблицы карточки шаблона, которая была удалена.
                        this.ID = (Guid)basedOnStageRowID;
                        this.BasedOnTemplateStage = true;
                    }
                    this.TemplateID = basedOnTemplateID as Guid?;
                }
            }
        }

        #endregion

        #region properties

        #region template properties

        /// <summary>
        /// ID карточки шаблона этапа KrStageTemplates
        /// </summary>
        public Guid? TemplateID { get; private set; }

        /// <summary>
        /// Имя карточки шаблона из секции KrStageTemplates["Name"]
        /// </summary>
        public string TemplateName { get; private set; }

        /// <summary>
        /// Расположение генерируемого этапа: перед вручную добавленными, после или Unspecified
        /// Unspecified применяется для вручную добавленных этапов
        /// </summary>
        public GroupPosition GroupPosition { get; private set; }
        
        /// <summary>
        /// Сможет ли пользователь поменять порядок текущего этапа
        /// Если это запрещено, то для этапов "в начале" этап окажется перед теми, для которых разрешено менять порядок. 
        /// для этапов "в конце" этапы, для которых разрешено менять порядок, будут выше, чем строго зафиксированные
        /// </summary>
        public bool CanChangeOrder { get; private set; }

        /// <summary>
        /// Порядок в группе по условию группировки пары (GroupPosition, CanChangeOrder)
        /// </summary>
        public int? TemplateOrder { get; private set; }

        /// <summary>
        /// Являются ли этап редактируемым для конечного пользователя
        /// </summary>
        public bool IsStageReadonly { get; private set; }

        #endregion

        #region stage properties

        /// <summary>
        /// RowID строки в конкретном документе.
        /// Если этап только создан по шаблону, то здесь будет новый гуид
        /// </summary>
        public Guid RowID { get; private set; }

        /// <summary>
        /// ID этапа. 
        /// Если этап создан по шаблону, то RowID этапа из карточки шаблона
        /// Если этап создан вручную, то RowID из карточки документа.
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// ID группы этапов, к которой принадлежит этап.
        /// </summary>
        public Guid StageGroupID { get; private set; }

        /// <summary>
        /// Название группы этапов, к которой принадлежит этап.
        /// </summary>
        public string StageGroupName { get; private set; }

        /// <summary>
        /// Порядок сортировки для группы этапов, к которой относится этап.
        /// </summary>
        public int StageGroupOrder { get; private set; }

        /// <summary>
        /// Добавлено в шаблоне из таблицы или постскрипта
        /// </summary>
        [JsonIgnore]
        public bool BasedOnTemplate => this.TemplateID.HasValue;

        /// <summary>
        /// Добавлено в шаблоне из таблицы
        /// </summary>
        public bool BasedOnTemplateStage { get; private set; }

        /// <summary>
        /// Название этапа
        /// </summary>
        public string Name
        {
            get => this.name;
            set
            {
                this.CheckSealed();
                this.name = value;
            }
        }

        /// <summary>
        /// Текущее состояние этапа согласования. Актуально только при работе процесса.
        /// </summary>
        public KrStageState State
        {
            get => this.state;
            set
            {
                this.CheckSealed();
                this.state = value;
            }
        }

        /// <summary>
        /// Срок
        /// </summary>
        public double? TimeLimit
        {
            get => this.timeLimit;
            set
            {
                this.CheckSealed();
                this.timeLimit = value;
                if (value != null)
                {
                    this.planned = null;
                }
            }
        }

        /// <summary>
        /// Срок, если указан. Иначе стандартное значение <see cref="DefaultTimeLimit"/>
        /// </summary>
        [JsonIgnore]
        public double TimeLimitOrDefault => this.timeLimit ?? DefaultTimeLimit;

        /// <summary>
        /// Срок в квантах, если нет даты окончания задания <see cref="Planned"/>
        /// </summary>
        [JsonIgnore]
        public int? PlannedQuants => 
            this.Planned == null 
                ? (int?)Math.Round(this.TimeLimitOrDefault * TimeZonesHelper.QuantsInDay)
                : null;

        /// <summary>
        /// Дата выполнения
        /// </summary>
        public DateTime? Planned
        {
            get => this.planned;
            set
            {
                this.CheckSealed();
                this.planned = value;
                if (value != null)
                {
                    this.timeLimit = null;
                }
            }
        }
        
        /// <summary>
        /// Этап скрыт.
        /// </summary>
        public bool Hidden
        {
            get => this.hidden;
            set
            {
                this.CheckSealed();
                this.hidden = value;
            }
        }


        /// <summary>
        /// Запрос на получение SQL-согласующих
        /// </summary>
        public string SqlPerformers { get; private set; }

        /// <summary>
        /// Индекс в массиве, куда необходимо подставлять SQL согласующих при пересчете.
        /// Загружается при создании на основе строки карточки с указанием карточки-шаблона.
        /// Никак не отображает куда были подставлены SQL согласующие за предыщуий пересчет.
        /// Чтобы это узнать, нужно найти индекс первого согласующего с флагом SqlApprover = true,
        /// однако если этап изменялся вручную (RowChanged), о предыдущей подстановке SQL согласующих
        /// делать выводы нельзя.
        /// </summary>
        public int? SqlPerformersIndex { get; private set; }

        /// <summary>
        /// Признак того, что порядок менялся пользователем
        /// Не зависит от изменения порядка в коде
        /// </summary>
        public bool OrderChanged { get; private set; }

        /// <summary>
        /// Признак того, что этап менялся пользователем
        /// Не зависит от изменений в коде.
        /// </summary>
        public bool RowChanged { get; private set; }
        

        /// <summary>
        /// Порядок сортировки для этапа в рамках шаблона этапов.
        /// Необходимо для обнаружения изменений в подмаршруте из конкретного шаблона этапа
        /// (например, если в шаблоне был добавлен еще один этап на первое место, при построении этот этап необходимо поместить также выше)
        /// </summary>
        public int? TemplateStageOrder
        {
            get => this.templateStageOrder;
            set
            {
                this.CheckSealed();
                this.templateStageOrder = value;
            }
        }

        /// <summary>
        /// ID типа этапа.
        /// </summary>
        public Guid? StageTypeID
        {
            get => this.stageTypeID;
            set
            {
                this.CheckSealed();
                this.stageTypeID = value;
            }
        }

        /// <summary>
        /// Отображаемое имя типа этапа.
        /// </summary>
        public string StageTypeCaption
        {
            get => this.stageTypeCaption;
            set
            {
                this.CheckSealed();
                this.stageTypeCaption = value;
            }
        }

        /// <summary>
        /// Настройки этапа.
        /// </summary>
        public IDictionary<string, object> SettingsStorage
        {
            get => this.settings;
            set
            {
                this.CheckSealed();
                this.settings = value;
            }
        }

        /// <summary>
        /// Настройки этапа.
        /// </summary>
        [JsonIgnore]
        public dynamic Settings => this.settingsDynamicLazy.Value;

        /// <summary>
        /// Данные этапа в хранилище ключ/значение.
        /// </summary>
        public IDictionary<string, object> InfoStorage { get; private set; }

        /// <summary>
        /// Данные этапа.
        /// </summary>
        [JsonIgnore]
        public dynamic Info => this.infoDynamicLazy.Value;

        /// <summary>
        /// Исполнитель текущего этапа. Актуально только для режима <see cref="PerformerUsageMode.Single"/>
        /// </summary>
        [JsonIgnore]
        public Performer Performer
        {
            get
            {
                if (!this.performerExists.HasValue)
                {
                    this.performer = new SinglePerformerProxy(this.settings);
                    this.performerExists = this.settings.TryGet<Guid?>(KrConstants.KrSinglePerformerVirtual.PerformerID) != null;
                }

                return this.performerExists.Value
                    ? this.performer
                    : null;
            } 
            set
            {
                this.CheckSealed();
                if (!this.performerExists.HasValue)
                {
                    this.performer = new SinglePerformerProxy(this.settings);
                    this.performerExists = this.settings.TryGet<Guid?>(KrConstants.KrSinglePerformerVirtual.PerformerID) != null;
                }
                
                if (value != null)
                {
                    this.performerExists = true;
                    this.settings[KrConstants.KrSinglePerformerVirtual.PerformerID] = value.PerformerID;
                    this.settings[KrConstants.KrSinglePerformerVirtual.PerformerName] = value.PerformerName;
                }
                else
                {
                    this.performerExists = false;
                    this.settings[KrConstants.KrSinglePerformerVirtual.PerformerID] = null;
                    this.settings[KrConstants.KrSinglePerformerVirtual.PerformerName] = null;
                }
            }
        }

        /// <summary>
        /// Список исполнителей текущего этапа. Актуально только для режима <see cref="PerformerUsageMode.Multiple"/>
        /// </summary>
        [JsonIgnore]
        public ListStorage<Performer> Performers
        {
            get
            {
                if (this.performers is null)
                {
                    if (!this.settings.TryGetValue(KrConstants.KrPerformersVirtual.Synthetic, out var kpvObj)
                        || !(kpvObj is List<object> kvp))
                    {
                        kvp = new List<object>();
                        this.settings[KrConstants.KrPerformersVirtual.Synthetic] = kvp;
                    }
                    kvp.Sort(performerObjectComparer);
                    this.performers = new ListStorage<Performer>(kvp, multiPerformerFactory);    
                }
                
                return this.performers;
            }
        }

        /// <summary>
        /// Переопределенный автор
        /// </summary>
        [JsonIgnore]
        public Author Author
        {
            get
            {
                if (!this.authorExists.HasValue)
                {
                    this.author = new AuthorProxy(this.settings);
                    this.authorExists = this.settings.TryGet<Guid?>(KrConstants.KrAuthorSettingsVirtual.AuthorID) != null;
                }
                
                return this.authorExists.Value
                    ? this.author
                    : null;
            }
            set
            {
                this.CheckSealed();
                if (!this.authorExists.HasValue)
                {
                    this.author = new AuthorProxy(this.settings);
                    this.authorExists = this.settings.TryGet<Guid?>(KrConstants.KrAuthorSettingsVirtual.AuthorID) != null;
                }
                
                if (value != null)
                {
                    this.authorExists = true;
                    this.settings[KrConstants.KrAuthorSettingsVirtual.AuthorID] = value.AuthorID;
                    this.settings[KrConstants.KrAuthorSettingsVirtual.AuthorName] = value.AuthorName;
                }
                else
                {
                    this.authorExists = false;
                    this.settings[KrConstants.KrAuthorSettingsVirtual.AuthorID] = null;
                    this.settings[KrConstants.KrAuthorSettingsVirtual.AuthorName] = null;
                }
            }
        }

        /// <summary>
        /// Флаг регулирует, в каком объеме информация о заданиях будет указываться по ключу <see cref="KrConstants.Keys.Tasks"/>
        /// в Info этапа. Если указано <c>true</c> - информация будет полной, включая карточку задания. Иначе перед записью будут
        /// удалены некоторые поля. Является обетркой над флагом, лежашим в Info по ключу, равному названию поля. Отсутствие значения
        /// в Info трактуется как false.
        /// </summary>
        [JsonIgnore]
        public bool WriteTaskFullInformation
        {
            get => this.InfoStorage.TryGet<bool>(nameof(WriteTaskFullInformation));
            set => this.InfoStorage[nameof(WriteTaskFullInformation)] = value;
        }

        /// <summary>
        /// Возвращает или задаёт признак пропуска этапа.
        /// </summary>
        public bool Skip
        {
            get => this.skip;
            set
            {
                this.CheckSealed();
                this.skip = value;
            }
        }
        
        /// <summary>
        /// Возвращает или задаёт значение, показывающее, разрешен ли пропуск этапа.
        /// </summary>
        public bool CanBeSkipped
        {
            get => this.canBeSkipped;
            set
            {
                this.CheckSealed();
                this.canBeSkipped = value;
            }
        }
        
        #endregion

        #region internal properties

        /// <summary>
        /// Объект создан при первичном построении исходного маршрута.
        /// </summary>
        internal bool InitialStage { get; set; }

        /// <summary>
        /// Признак того, что этап должен быть отвязан от шаблона.
        /// </summary>
        internal bool UnbindTemplate { get; set; } = false;

        /// <summary>
        /// Предок этапа(если есть), который был изначально в маршруте 
        /// и текущий этап пришел на замену предку.
        /// </summary>
        internal Stage Ancestor { get; private set; }

        /// <summary>
        /// Сообщение runner-у о том, что необходимо для данного этапа
        /// попытаться переключить контекст на указанную карточку.
        /// </summary>
        internal Guid? ChangeContextToCardID { get; set; } = null;

        /// <summary>
        /// Признак того, что при обработке <see cref="ChangeContextToCardID"/>
        /// обработка будет переключена на всю группу.
        /// </summary>
        internal bool ChangeContextWholeGroupToDifferentCard { get; set; } = false;
        
        /// <summary>
        /// Инфо для процесса, который будет создан при переключении контекста.
        /// </summary>
        internal IDictionary<string, object> ChangeContextProcessInfo { get; set; }

        #endregion

        #endregion

        #region public methods

        /// <summary>
        /// Сброс позиции сначала/в конец на "неопределено"
        /// </summary>
        public void SetGroupPositionUnspecified()
        {
            this.CheckSealed();
            this.GroupPosition = GroupPosition.Unspecified;
            this.TemplateOrder = null;
            this.TemplateStageOrder = null;
        }

        /// <summary>
        /// Перенос служебной информации (о положении, внесенных изменениях и др)
        /// При пересчете, когда имеются новая и старая версия этапа,
        /// Нужно сохранить информацию о том, как пользователь воздействовал на этап,
        /// а также поддержать поле GroupPosition для корректной сортировки этапов
        /// Помимо этого переносятся SQL-согласующие, поскольку иначе информация о них будет утеряна.
        /// Переносить SQL согласующих нужно для определения изменений в выборке SQL согласующих.
        /// </summary>
        /// <param name="stage"></param>
        public void Inherit(Stage stage)
        {
            this.CheckSealed();
            this.RowID = stage.RowID;
            this.State = stage.state;
            StorageHelper.Merge(stage.InfoStorage, this.InfoStorage);

            if (stage.GroupPosition == GroupPosition.Unspecified)
            {
                this.GroupPosition = stage.GroupPosition;
                this.TemplateOrder = stage.TemplateOrder;
                this.TemplateStageOrder = stage.TemplateStageOrder;
            }
            this.CanChangeOrder = stage.CanChangeOrder;
            this.IsStageReadonly = stage.IsStageReadonly;
           
            this.RowChanged = stage.RowChanged;
            this.OrderChanged = stage.OrderChanged;

            this.InfoStorage = stage.Info;

            this.Ancestor = null;
            if (stage.Ancestor?.InitialStage == true)
            {
                this.Ancestor = stage.Ancestor;
            }
            else if (stage.Ancestor == null && stage.InitialStage)
            {
                this.Ancestor = stage;
            }

            this.Skip = stage.Skip;
            if (this.Skip)
            {
                this.Hidden = stage.Hidden;
            }
        }

        /// <summary>
        /// Перенос информации о положении
        /// </summary>
        /// <param name="stage"></param>
        public void InheritPosition(Stage stage)
        {
            this.CheckSealed();
            this.GroupPosition = stage.GroupPosition;
            this.TemplateOrder = stage.TemplateOrder;
            this.TemplateStageOrder = stage.TemplateStageOrder;
        }
        
        /// <summary>
        /// Установить флаг CanChangeOrder = true.
        /// Использовать только для этапов, измененных пользователем и неподтвержденным
        /// </summary>
        public void SetCanChangeOrderTrue()
        {
            this.CheckSealed();
            if (this.CanChangeOrder)
            {
                return;
            }
            if (this.BasedOnTemplate
                && this.InitialStage
                && (this.RowChanged || this.OrderChanged))
            {
                this.CanChangeOrder = true;
                return;
            }
            // Если это делают с нормальными этапами, нужно поругаться.
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Изменилась ли пользовательская информация внутри этапа.
        /// </summary>
        /// <param name="currentStageFromThePast"></param>
        /// <returns></returns>
        public bool IsInfoChanged(
            Stage currentStageFromThePast)
        {
            if (currentStageFromThePast.ID != this.ID)
            {
                throw new ArgumentException("Can compare only with the same stage.");
            }
            return !StorageHelper.Equals(this.InfoStorage, currentStageFromThePast.InfoStorage);
        }
            

        #endregion

        #region operators

        public static bool operator ==(Stage left, Stage right)
        {
            if (left is null
                && right is null)
            {
                return true;
            }
            return left?.Equals(right) == true; 
        }

        public static bool operator !=(Stage left, Stage right)
        {
            if (left is null
                && right is null)
            {
                return false;
            }
            return left?.Equals(right) != true;
        }

        #endregion

        #region object

        public override string ToString()
        {
            return $"ID = {this.ID}," +
                $" Name = {this.Name}," +
                $" TemplateName = {this.TemplateName}," +
                $" Performers = {this.Performers.Count} ";
        }

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
            return obj is Stage stage && this.Equals(stage);
        }
        
        public override int GetHashCode()
        {
            // ID setter используется при десериализации. В процесс работы не меняется.
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.ID.GetHashCode();
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Сравнение этапа по значимым полям.
        /// Info необходимо проверять отдельно.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Stage other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Состояние при этом сравнении не учитывается, 
            // т.к. в рантайме оно может отличаться, но по факту этапы одинаковы

            var equal = this.TemplateID.Equals(other.TemplateID)
                && string.Equals(this.TemplateName, other.TemplateName)
                && Equals(this.GroupPosition, other.GroupPosition)
                && this.stageTypeID == other.stageTypeID
                && this.stageTypeCaption == other.stageTypeCaption
                && this.StageGroupID == other.StageGroupID
                && this.StageGroupOrder == other.StageGroupOrder
                && this.CanChangeOrder == other.CanChangeOrder
                && this.TemplateOrder == other.TemplateOrder
                && this.IsStageReadonly == other.IsStageReadonly
                && this.ID.Equals(other.ID)
                && this.BasedOnTemplateStage == other.BasedOnTemplateStage
                && string.Equals(this.Name, other.Name)
                && NullableDoubleNumbersIsEqual(this.TimeLimit, other.TimeLimit)
                && this.Planned == other.Planned
                && this.Hidden == other.Hidden
                && string.Equals(this.SqlPerformers, other.SqlPerformers)
                && this.SqlPerformersIndex == other.SqlPerformersIndex
                && this.OrderChanged == other.OrderChanged
                && this.RowChanged == other.RowChanged
                && this.CanBeSkipped == other.CanBeSkipped;

            if (!equal)
            {
                return false;
            }

            return StorageHelper.Equals(this.settings, other.settings);
        }

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
        }

        #endregion

        #region private

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool NullableDoubleNumbersIsEqual(
            double? first,
            double? second) =>
            first is null && second is null || Math.Abs((first - second) ?? Epsilon) < Epsilon;

        /// <summary>
        /// Выбрасывает исключение Tessa.Platform.ObjectSealedException",
        /// если объект был защищён от изменений.
        /// </summary>
        private void CheckSealed()
        {
            Check.ObjectNotSealed(this);
        }

        private void InitLazyDynamics()
        {
            this.settingsDynamicLazy = new Lazy<dynamic>(() => DynamicStorageAccessor.Create(this.settings), LazyThreadSafetyMode.PublicationOnly);
            this.infoDynamicLazy = new Lazy<dynamic>(() => DynamicStorageAccessor.Create(this.InfoStorage), LazyThreadSafetyMode.PublicationOnly);
        }

        private void FillStageProperties(CardRow stageRow)
        {
            this.state = (KrStageState)stageRow.Fields.TryGet<int>(KrConstants.KrStages.StageStateID);
            this.StageGroupID = (Guid)stageRow.Fields[KrConstants.StageGroupID];
            this.StageGroupName = (string)stageRow.Fields[KrConstants.StageGroupName];
            this.StageGroupOrder = (int)stageRow.Fields[KrConstants.StageGroupOrder];
            this.name = (string)stageRow.Fields[KrConstants.Name];
            this.timeLimit = stageRow.Fields[KrConstants.KrStages.TimeLimit] as double?;
            this.planned = stageRow.Fields[KrConstants.KrStages.Planned] as DateTime?;
            this.hidden = stageRow.Fields.TryGet<bool?>(KrConstants.KrStages.Hidden) ?? false;
            this.skip = stageRow.Fields.TryGet<bool?>(KrConstants.KrStages.Skip) ?? false;
            this.canBeSkipped = stageRow.Fields.TryGet<bool?>(KrConstants.KrStages.CanBeSkipped) ?? false;

            this.RowChanged = (bool)stageRow.Fields[KrConstants.KrStages.RowChanged];
            this.OrderChanged = (bool)stageRow.Fields[KrConstants.KrStages.OrderChanged];

            this.stageTypeID = (Guid?)stageRow.Fields[KrConstants.KrStages.StageTypeID];
            this.stageTypeCaption = (string)stageRow.Fields[KrConstants.KrStages.StageTypeCaption];
        }
        
        private void FillStageProperties(IKrRuntimeStage runtimeStage)
        {
            this.state = KrStageState.Inactive;
            this.StageGroupID = runtimeStage.GroupID;
            this.StageGroupName = runtimeStage.GroupName;
            this.StageGroupOrder = runtimeStage.GroupOrder;
            this.name = runtimeStage.StageName;
            this.timeLimit = runtimeStage.TimeLimit;
            this.planned = runtimeStage.Planned;
            this.hidden = runtimeStage.Hidden;
            this.skip = runtimeStage.Skip;
            this.canBeSkipped = runtimeStage.CanBeSkipped;

            this.RowChanged = false;
            this.OrderChanged = false;

            this.stageTypeID = runtimeStage.StageTypeID;
            this.stageTypeCaption = runtimeStage.StageTypeCaption;
        }

        private void FillTemplateProperties(
            IKrStageTemplate stageTemplate,
            IKrRuntimeStage runtimeStage)
        {
            this.TemplateID = stageTemplate.ID;
            this.TemplateName = stageTemplate.Name;
            this.TemplateOrder = stageTemplate.Order;
            this.GroupPosition = stageTemplate.Position;

            this.CanChangeOrder = stageTemplate.CanChangeOrder;
            this.IsStageReadonly = stageTemplate.IsStagesReadonly;

            // Если чекбоксы запрещают изменения, нужно сбросить флаги и установить значения из шаблонов.
            if (!this.CanChangeOrder 
                && this.OrderChanged)
            {
                this.OrderChanged = false;
            }
            if (this.IsStageReadonly 
                && this.RowChanged)
            {
                this.RowChanged = false;
            }

            this.TemplateStageOrder = runtimeStage.Order;
        }
        
        private void FillTemplateProperties(
            CardRow stageRow, 
            IKrStageTemplate stageTemplate, 
            IKrRuntimeStage runtimeStage)
        {
            var stageFromTemplate = stageRow.RowID == runtimeStage?.StageID;
            
            this.TemplateID = stageTemplate.ID;
            this.TemplateName =
                stageRow.Fields.TryGetValue(KrConstants.KrStages.BasedOnStageTemplateName, out var botnObj)
                    && botnObj is string botnName
                    ? botnName
                    : stageTemplate.Name;
            this.TemplateOrder = stageFromTemplate
                ? stageTemplate.Order
                : stageRow.Fields.TryGet<int?>(KrConstants.KrStages.BasedOnStageTemplateOrder);
            this.GroupPosition = stageFromTemplate
                ? stageTemplate.Position
                : GroupPosition.GetByID(stageRow.Fields.TryGet<int?>(KrConstants.KrStages.BasedOnStageTemplateGroupPositionID));

            this.CanChangeOrder = stageTemplate.CanChangeOrder;
            this.IsStageReadonly = stageTemplate.IsStagesReadonly;

            // Если чекбоксы запрещают изменения, нужно сбросить флаги и установить значения из шаблонов.
            if (!this.CanChangeOrder 
                && this.OrderChanged)
            {
                this.OrderChanged = false;
            }
            if (this.IsStageReadonly 
                && this.RowChanged)
            {
                this.RowChanged = false;
            }

            if (runtimeStage != null && stageFromTemplate)
            {
                this.TemplateStageOrder = runtimeStage.Order;
            }
        }
        
        private void FillTemplatePropertiesByDefaultValues()
        {
            this.TemplateID = null;
            this.TemplateName = null;
            this.TemplateOrder = null;
            this.CanChangeOrder = true;
            this.GroupPosition = GroupPosition.Unspecified;
            this.IsStageReadonly = false;
            this.TemplateStageOrder = null;
        }

        #endregion
        
    }
}