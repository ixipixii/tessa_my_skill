using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB.Data;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public static class KrCompilersSqlHelper
    {
        #region nested types

        private sealed class KrSecondaryProcessData
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public int Mode { get; set; }
            public bool IsGlobal { get; set; }
            public bool Async { get; set; }
            public string Caption { get; set; }
            public string Icon { get; set; }
            public TileSize TileSize { get; set; }
            public string Tooltip { get; set; }
            public string TileGroup { get; set; }
            public string Message { get; set; }
            public string ExecutionAccessDeniedMessage { get; set; }
            public bool RefreshAndNotify { get; set; }
            public bool AskConfirmation { get; set; }
            public string ConfirmationMessage { get; set; }
            public bool ActionGrouping { get; set; }
            public string VisibilitySqlCondition { get; set; }
            public string ExecutionSqlCondition { get; set; }
            public string VisibilitySourceCondition { get; set; }
            public string ExecutionSourceCondition { get; set; }
            public string EventType { get; set; }
            public bool AllowClientSideLaunch { get; set; }
            public bool CheckRecalcRestrictions { get; set; }
            public bool RunOnce { get; set; }
            public string ButtonHotkey { get; set; }

            public ICollection<Guid> ContextRolesIDs { get; set; }

            public void Read(
                IDataReader reader)
            {
                var cnt = 0;
                this.ID = reader.GetGuid(cnt++);
                this.Name = reader.GetNullableString(cnt++);
                this.Mode = reader.GetInt32(cnt++);
                this.IsGlobal = reader.GetBoolean(cnt++);
                this.Async = reader.GetBoolean(cnt++);
                this.Caption = reader.GetNullableString(cnt++);
                this.Icon = reader.GetNullableString(cnt++);
                this.TileSize = (TileSize) reader.GetInt16(cnt++);
                this.Tooltip = reader.GetNullableString(cnt++);
                this.TileGroup = reader.GetNullableString(cnt++);
                this.Message = reader.GetNullableString(cnt++);
                this.ExecutionAccessDeniedMessage = reader.GetNullableString(cnt++);
                this.RefreshAndNotify = reader.GetBoolean(cnt++);
                this.AskConfirmation = reader.GetBoolean(cnt++);
                this.ConfirmationMessage = reader.GetNullableString(cnt++);
                this.ActionGrouping = reader.GetBoolean(cnt++);
                this.VisibilitySqlCondition = reader.GetNullableString(cnt++);
                this.ExecutionSqlCondition = reader.GetNullableString(cnt++);
                this.VisibilitySourceCondition = reader.GetNullableString(cnt++);
                this.ExecutionSourceCondition = reader.GetNullableString(cnt++);
                this.EventType = reader.GetNullableString(cnt++);
                this.AllowClientSideLaunch = reader.GetBoolean(cnt++);
                this.CheckRecalcRestrictions = reader.GetBoolean(cnt++);
                this.RunOnce = reader.GetBoolean(cnt++);
                this.ButtonHotkey = reader.GetNullableString(cnt);
            }

            public IKrPureProcess ToPureProcess()
            {
                return new KrPureProcess(
                    this.ID,
                    this.Name,
                    this.IsGlobal,
                    this.Async,
                    this.ExecutionAccessDeniedMessage,
                    false, // RunOnce актуально только для экшонов
                    this.ContextRolesIDs,
                    this.ExecutionSqlCondition,
                    this.ExecutionSourceCondition,
                    this.AllowClientSideLaunch,
                    this.CheckRecalcRestrictions);
            }

            public IKrAction ToAction()
            {
                return new KrAction(
                    this.ID,
                    this.Name,
                    this.IsGlobal,
                    this.Async,
                    this.ExecutionAccessDeniedMessage,
                    this.RunOnce,
                    this.ContextRolesIDs,
                    this.ExecutionSqlCondition,
                    this.ExecutionSourceCondition,
                    this.EventType);
            }

            public IKrProcessButton ToProcessButton()
            {
                return new KrProcessButton(
                    this.ID,
                    this.Name,
                    this.IsGlobal,
                    this.Async,
                    this.ExecutionAccessDeniedMessage,
                    false, // RunOnce актуально только для экшонов
                    this.ContextRolesIDs,
                    this.ExecutionSqlCondition,
                    this.ExecutionSourceCondition,
                    this.Caption,
                    this.Icon,
                    this.TileSize,
                    this.Tooltip,
                    this.TileGroup,
                    this.Message,
                    this.RefreshAndNotify,
                    this.AskConfirmation,
                    this.ConfirmationMessage,
                    this.ActionGrouping,
                    this.ButtonHotkey,
                    this.VisibilitySqlCondition,
                    this.VisibilitySourceCondition);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Запрос всех шаблонов из базы если ID == null
        /// Если ID не null, то выбирается шаблон только с указанным ID
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IKrStageTemplate> SelectStageTemplates(
            IDbScope dbScope,
            Guid? id = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C(null,
                        ID, Name, Order, StageGroupID, StageGroupName,
                        KrStageTemplates.GroupPositionID, KrStageTemplates.CanChangeOrder,
                        KrStageTemplates.IsStagesReadonly,
                        SqlCondition, SourceCondition, SourceBefore, SourceAfter)
                    .From(KrStageTemplates.Name).NoLock();
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C(ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }
                var stages = new List<IKrStageTemplate>();
                using (var reader = db
                    .LogCommand()
                    .ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stages.Add(new KrStageTemplate(
                            reader.GetGuid(0), // ID
                            reader.GetString(1), // Name
                            reader.GetInt32(2), // Order
                            reader.GetGuid(3), // StageGroupID
                            reader.GetNullableString(4), // StageGroupName
                            GroupPosition.GetByID(reader.GetNullableInt32(5)), // GroupPosition
                            reader.GetBoolean(6), // CanChangeOrder
                            reader.GetBoolean(7), // IsStagesReadonly
                            reader.GetNullableString(8), // SQLCondition
                            reader.GetNullableString(9), // SourceCondition
                            reader.GetNullableString(10), // SourceBefore
                            reader.GetNullableString(11) // SourceAfter
                        ));
                    }
                }

                return stages;
            }
        }

        /// <summary>
        /// Запрос всех виртуальных шаблонов по вторичным процессам из базы если ID == null
        /// Если ID не null, то выбирается шаблоны только с указанному ID (вторичного процесса)
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IKrStageTemplate> SelectVirtualStageTemplates(
            IDbScope dbScope,
            Guid? id = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C(null, ID, Name)
                    .From(KrSecondaryProcesses.Name).NoLock();
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C(ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }
                var stages = new List<IKrStageTemplate>();
                using (var reader = db
                    .LogCommand()
                    .ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rowID = reader.GetGuid(0);
                        var groupID = rowID;
                        var name = reader.GetString(1);

                        stages.Add(new KrStageTemplate(
                            rowID, // ID
                            name, // Name
                            DefaultSecondaryProcessTemplateOrder, // Order
                            groupID, // StageGroupID
                            name, // StageGroupName
                            GroupPosition.AtFirst, // GroupPosition
                            false, // CanChangeOrder
                            true, // IsStagesReadonly
                            string.Empty, // SQLCondition
                            string.Empty, // SourceCondition
                            string.Empty, // SourceBefore
                            string.Empty // SourceAfter
                        ));
                    }
                }

                return stages;
            }
        }

        /// <summary>
        /// Загрузка шаблонов для указанного типа карточки/документа, пользователя и группы этапов.
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="typeID"></param>
        /// <param name="userID"></param>
        /// <param name="stageGroupID"></param>
        /// <param name="secondaryProcessID"></param>
        /// <returns></returns>
        public static List<Guid> GetFilteredStageTemplates(
            IDbScope dbScope,
            Guid typeID,
            Guid userID,
            Guid stageGroupID,
            Guid? secondaryProcessID = null)
        {
            if (secondaryProcessID == stageGroupID)
            {
                return new List<Guid> { secondaryProcessID.Value };
            }

            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C("t", ID)
                    .From(KrStageTemplates.Name, "t").NoLock()
                    .LeftJoin(KrStageTypes.Name, "tt").NoLock()
                        .On().C("tt", ID).Equals().C("t", "ID")
                    .Where()
                        .C("t", StageGroupID).Equals().P("StageGroupID")
                        .And()
                        .E(w => w
                            .C("tt", TypeID).IsNull()
                            .Or()
                            .C("tt", TypeID).Equals().P("TypeID"))
                        .And()
                        .E(w => w
                            .NotExists(e => e
                                .Select().V(null)
                                .From(KrStageRoles.Name, "r").NoLock()
                                .Where().C("r", ID).Equals().C("t", ID))
                            .Or()
                            .Exists(e => e
                                .Select().V(null)
                                .From(KrStageRoles.Name, "r").NoLock()
                                .InnerJoin("RoleUsers", "ru").NoLock()
                                    .On().C("ru", ID).Equals().C("r", "RoleID")
                                .Where().C("r", ID).Equals().C("t", ID)
                                    .And().C("ru", "UserID").Equals().P("UserID")));


                return db
                    .SetCommand(
                        builder.Build(),
                        db.Parameter("TypeID", typeID),
                        db.Parameter("UserID", userID),
                        db.Parameter("StageGroupID", stageGroupID))
                    .LogCommand()
                    .ExecuteList<Guid>();
            }
        }

        /// <summary>
        /// Запрос из базы данных всех групп если ID == null
        /// Если ID не null, то выбирается группа только с указанным ID
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IKrStageGroup> SelectStageGroups(
            IDbScope dbScope,
            Guid? id = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C(null,
                        ID, Name, Order,
                        KrStageGroups.IsGroupReadonly, KrStageGroups.KrSecondaryProcessID, SqlCondition, RuntimeSqlCondition,
                        SourceCondition, SourceBefore, SourceAfter,
                        RuntimeSourceCondition, RuntimeSourceBefore, RuntimeSourceAfter)
                    .From(KrStageGroups.Name).NoLock();
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C(ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }var stages = new List<IKrStageGroup>();
                using (var reader = db
                    .LogCommand()
                    .ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stages.Add(new KrStageGroup(
                            reader.GetGuid(0), // ID
                            reader.GetString(1), // Name
                            reader.GetInt32(2), // Order
                            reader.GetBoolean(3), // IsGroupReadonly
                            reader.GetNullableGuid(4), // SecondaryProcessID
                            reader.GetNullableString(5), // SQLCondition
                            reader.GetNullableString(6), // RuntimeSQLCondition
                            reader.GetNullableString(7), // SourceCondition
                            reader.GetNullableString(8), // SourceBefore
                            reader.GetNullableString(9), // SourceAfter
                            reader.GetNullableString(10), // RuntimeSourceCondition
                            reader.GetNullableString(11), // RuntimeSourceBefore
                            reader.GetNullableString(12) // RuntimeSourceAfter
                        ));
                    }
                }

                return stages;
            }
        }

        /// <summary>
        /// Запрос всех виртуальных групп по вторичным процессам из базы если ID == null
        /// Если ID не null, то выбирается группа только с указанному ID (вторичного процесса)
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IKrStageGroup> SelectVirtualStageGroups(
            IDbScope dbScope,
            Guid? id = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select()
                    .C(null,ID, Name)
                    .From(KrSecondaryProcesses.Name).NoLock();
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C(ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }

                var stages = new List<IKrStageGroup>();
                using (var reader = db
                    .LogCommand()
                    .ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var spID = reader.GetGuid(0);
                        var groupID = spID;
                        stages.Add(new KrStageGroup(
                            groupID, // ID
                            reader.GetString(1), // Name
                            DefaultSecondaryProcessGroupOrder, // Order
                            true, // IsGroupReadonly
                            spID, // SecondaryProcessID
                            string.Empty, // SQLCondition
                            string.Empty, // RuntimeSQLCondition
                            string.Empty, // SourceCondition
                            string.Empty, // SourceBefore
                            string.Empty, // SourceAfter
                            string.Empty, // RuntimeSourceCondition
                            string.Empty, // RuntimeSourceBefore
                            string.Empty // RuntimeSourceAfter
                        ));
                    }
                }

                return stages;
            }
        }

        /// <summary>
        /// Загрузка групп этапов для карточки/документа и пользователя.
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="typeID"></param>
        /// <param name="userID"></param>
        /// <param name="orderFrom"></param>
        /// <param name="orderTo"></param>
        /// <param name="secondaryProcessID"></param>
        /// <returns></returns>
        public static List<Guid> SelectFilteredStageGroups(
            IDbScope dbScope,
            Guid typeID,
            Guid userID,
            int? orderFrom = null,
            int? orderTo = null,
            Guid? secondaryProcessID = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C("t", ID)
                    .From(KrStageGroups.Name, "t").NoLock()
                    .LeftJoin(KrStageTypes.Name, "tt").NoLock()
                    .On().C("tt", ID).Equals().C("t", ID)
                    .Where()
                    .C("t", KrStageGroups.Ignore).Equals().V(BooleanBoxes.False)
                    .And().E(w => w
                        .C("tt", TypeID).IsNull()
                        .Or()
                        .C("tt", TypeID).Equals().P("TypeID"))
                    .And()
                    .E(w => w
                        .NotExists(e => e
                            .Select().V(null)
                            .From(KrStageRoles.Name, "r").NoLock()
                            .Where().C("r", ID).Equals().C("t", ID))
                        .Or()
                        .Exists(e => e
                            .Select().V(null)
                            .From(KrStageRoles.Name, "r").NoLock()
                            .InnerJoin("RoleUsers", "ru").NoLock()
                            .On().C("ru", ID).Equals().C("r", RoleID)
                            .Where().C("r", ID).Equals().C("t", ID)
                            .And().C("ru", "UserID").Equals().P("UserID")));
                var parameters = new List<DataParameter>
                {
                    db.Parameter("TypeID", typeID),
                    db.Parameter("UserID", userID),
                };

                if (secondaryProcessID.HasValue)
                {
                    builder.And().C("t", KrStageGroups.KrSecondaryProcessID).Equals().P("processID");
                    parameters.Add(db.Parameter("processID", secondaryProcessID.Value));
                }
                else
                {
                    builder.And().C("t", KrStageGroups.KrSecondaryProcessID).IsNull();
                }

                if (orderFrom.HasValue)
                {
                    builder.And().C("t", Order).GreaterOrEquals().P("OrderFrom");
                    parameters.Add(db.Parameter("OrderFrom", orderFrom.Value));
                }

                if (orderTo.HasValue)
                {
                    builder.And().C("t", Order).LessOrEquals().P("OrderTo");
                    parameters.Add(db.Parameter("OrderTo", orderTo.Value));
                }

                builder.OrderBy("t", Order);

                var ids = db
                    .SetCommand(builder.Build(), parameters.ToArray())
                    .LogCommand()
                    .ExecuteList<Guid>();
                if (secondaryProcessID != null
                    && (orderFrom is null || orderFrom <= 0)
                    && (orderTo is null || orderTo >= 0))
                {
                    ids.Add(secondaryProcessID.Value);
                }

                return ids;
            }
        }

        /// <summary>
        /// Запрос из базы всех рантайм-скриптов для этапов.
        /// Если ID указывается, то выбираются только для карточки шаблона этапов с ID.
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="stageSerializer"></param>
        /// <param name="extraSourceSerializer"></param>
        /// <param name="id">ID карточки шаблона этапов.</param>
        /// <returns></returns>
        public static List<IKrRuntimeStage> SelectRuntimeStages(
            IDbScope dbScope,
            IKrStageSerializer stageSerializer,
            IExtraSourceSerializer extraSourceSerializer,
            Guid? id = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select()
                    .C("kst",
                        ID,
                        Name,
                        StageGroupID,
                        StageGroupName)
                    .C("ksg",
                        Order)
                    .C("ks",
                        RowID,
                        Name,
                        Order,
                        KrStages.TimeLimit,
                        KrStages.Planned,
                        KrStages.Hidden,
                        KrStages.StageTypeID,
                        KrStages.StageTypeCaption,
                        KrStages.SqlApproverRole,
                        KrStages.Settings,
                        KrStages.ExtraSources,
                        RuntimeSqlCondition,
                        RuntimeSourceCondition,
                        RuntimeSourceBefore,
                        RuntimeSourceAfter,
                        KrStages.ExtraSources,
                        KrStages.Skip,
                        KrStages.CanBeSkipped)
                    .From(KrStages.Name, "ks").NoLock()
                    .InnerJoin(KrStageTemplates.Name, "kst").NoLock()
                    .On().C("ks", ID).Equals().C("kst", "ID")
                    .LeftJoin(KrStageGroups.Name, "ksg").NoLock()
                    .On().C("kst", StageGroupID).Equals().C("ksg", "ID");
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C("kst", ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }

                return ReadRuntimeStages(db, stageSerializer, extraSourceSerializer);
            }
        }

        /// <summary>
        /// Запрос из базы этапов для вторичных процессов.
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="stageSerializer"></param>
        /// <param name="extraSourceSerializer"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IKrRuntimeStage> SelectSecondaryProcessRuntimeStages(
            IDbScope dbScope,
            IKrStageSerializer stageSerializer,
            IExtraSourceSerializer extraSourceSerializer,
            Guid? id = null)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select()
                    .C("ksp",
                        ID,
                        Name,
                        ID,
                        Name)
                    .V(KrConstants.DefaultSecondaryProcessGroupOrder) // StageGroupID
                    .C("ks",
                        RowID,
                        Name,
                        Order,
                        KrStages.TimeLimit,
                        KrStages.Planned,
                        KrStages.Hidden,
                        KrStages.StageTypeID,
                        KrStages.StageTypeCaption,
                        KrStages.SqlApproverRole,
                        KrStages.Settings,
                        KrStages.ExtraSources,
                        RuntimeSqlCondition,
                        RuntimeSourceCondition,
                        RuntimeSourceBefore,
                        RuntimeSourceAfter,
                        KrStages.ExtraSources,
                        KrStages.Skip,
                        KrStages.CanBeSkipped)
                    .From(KrStages.Name, "ks").NoLock()
                    .InnerJoin(KrSecondaryProcesses.Name, "ksp").NoLock()
                    .On().C("ks", ID).Equals().C("ksp", "ID");
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C("ksp", ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }

                return ReadRuntimeStages(db, stageSerializer, extraSourceSerializer);
            }
        }

        /// <summary>
        /// Получить все вторичные процессы из БД.
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="secondaryProcessID"></param>
        /// <param name="pureProcesses"></param>
        /// <param name="actions"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static void SelectKrSecondaryProcesses(
            IDbScope dbScope,
            Guid? secondaryProcessID,
            out List<IKrPureProcess> pureProcesses,
            out List<IKrAction> actions,
            out List<IKrProcessButton> buttons)
        {
            pureProcesses = new List<IKrPureProcess>();
            actions = new List<IKrAction>();
            buttons = new List<IKrProcessButton>();

            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var query = dbScope.BuilderFactory
                    .Select()
                    .C(null,
                        ID,
                        Name,
                        KrSecondaryProcesses.ModeID,
                        KrSecondaryProcesses.IsGlobal,
                        KrSecondaryProcesses.Async,
                        Caption,
                        KrSecondaryProcesses.Icon,
                        KrSecondaryProcesses.TileSizeID,
                        KrSecondaryProcesses.Tooltip,
                        KrSecondaryProcesses.TileGroup,
                        KrSecondaryProcesses.Message,
                        KrSecondaryProcesses.ExecutionAccessDeniedMessage,
                        KrSecondaryProcesses.RefreshAndNotify,
                        KrSecondaryProcesses.AskConfirmation,
                        KrSecondaryProcesses.ConfirmationMessage,
                        KrSecondaryProcesses.ActionGrouping,
                        KrSecondaryProcesses.VisibilitySqlCondition,
                        KrSecondaryProcesses.ExecutionSqlCondition,
                        KrSecondaryProcesses.VisibilitySourceCondition,
                        KrSecondaryProcesses.ExecutionSourceCondition,
                        KrSecondaryProcesses.ActionEventType,
                        KrSecondaryProcesses.AllowClientSideLaunch,
                        KrSecondaryProcesses.CheckRecalcRestrictions,
                        KrSecondaryProcesses.RunOnce,
                        KrSecondaryProcesses.ButtonHotkey)
                    .From(KrSecondaryProcesses.Name).NoLock();
                if (secondaryProcessID is null)
                {
                    db.SetCommand(query.Build());
                }
                else
                {
                    db.SetCommand(
                        query.Where().C(ID).Equals().P("ID").Build(),
                        db.Parameter("ID", secondaryProcessID));
                }

                db.LogCommand();

                var spData = new List<KrSecondaryProcessData>();
                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new KrSecondaryProcessData();
                        data.Read(reader);
                        spData.Add(data);
                    }
                }

                if (spData.Count == 0)
                {
                    return;
                }

                var processIDs = spData.Select(p => p.ID).ToArray();
                var contextRoles = ReadContextRoles(dbScope, processIDs);

                foreach (var secProcess in spData)
                {
                    var processContextRoles = contextRoles.TryGetValue(secProcess.ID, out var cr)
                        ? (ICollection<Guid>)cr
                        : EmptyHolder<Guid>.Collection;

                    secProcess.ContextRolesIDs = processContextRoles;

                    if (secProcess.Mode == KrSecondaryProcessModes.PureProcess.ID)
                    {
                        pureProcesses.Add(secProcess.ToPureProcess());
                    }
                    else if (secProcess.Mode == KrSecondaryProcessModes.Action.ID)
                    {
                        actions.Add(secProcess.ToAction());
                    }
                    else if(secProcess.Mode == KrSecondaryProcessModes.Button.ID)
                    {
                        buttons.Add(secProcess.ToProcessButton());
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Secondary process mode");
                    }
                }
            }
        }

        /// <summary>
        /// Запрос из базы списка всех базовых методов если ID == null
        /// Если ID указан, то выбирается один метод по ID
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IKrCommonMethod> SelectCommonMethods(
            IDbScope dbScope,
            Guid? id = null)
        {
            var methods = new List<IKrCommonMethod>();
            using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C(null, ID, Name, KrStageCommonMethods.Source)
                    .From(KrStageCommonMethods.Name).NoLock();
                if (id == null)
                {
                    db.SetCommand(
                        builder.Build());
                }
                else
                {
                    db.SetCommand(
                        builder
                            .Where().C(ID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id));
                }

                using var reader = db
                    .LogCommand()
                    .ExecuteReader();
                while (reader.Read())
                {
                    methods.Add(new KrCommonMethod(
                        reader.GetGuid(0),          // ID
                        reader.GetString(1),        // Name
                        reader.GetNullableString(2) // Source
                    ));
                }
            }

            return methods;
        }

        #endregion

        #region private

        private static Dictionary<Guid, List<Guid>> ReadContextRoles(
            IDbScope dbScope,
            Guid[] processIDs)
        {
            var contextRolesQuery = dbScope.BuilderFactory
                .Select().C(null, RoleID, ID)
                .From(KrSecondaryProcessContextRoles.Name).NoLock()
                .Where().C(ID).In(processIDs)
                .Build();
            dbScope.Db.SetCommand(contextRolesQuery).LogCommand();
            var contextRolesButtons = new Dictionary<Guid, List<Guid>>((int)(1.5 * processIDs.Length));
            using (var reader = dbScope.Db.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetGuid(1);
                    if (!contextRolesButtons.TryGetValue(id, out var contextRolesList))
                    {
                        contextRolesList = new List<Guid>();
                        contextRolesButtons.Add(id, contextRolesList);
                    }

                    contextRolesList.Add(reader.GetGuid(0));
                }
            }

            return contextRolesButtons;
        }

        private static List<IKrRuntimeStage> ReadRuntimeStages(
            DbManager db,
            IKrStageSerializer stageSerializer,
            IExtraSourceSerializer extraSourceSerializer)
        {
            var stages = new List<IKrRuntimeStage>();
            using (var reader = db
                .LogCommand()
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    var templateID = reader.GetGuid(0);
                    var groupID = reader.GetGuid(2);

                    stages.Add(new KrRuntimeStage(
                        templateID, // TemplateID
                        reader.GetNullableString(1), // TemplateName
                        groupID, // StageGroupID
                        reader.GetNullableString(3), // GroupName
                        reader.GetNullableInt32(4) ?? default, // GroupOrder
                        reader.GetGuid(5), // StageID
                        reader.GetNullableString(6), // StageName
                        reader.GetNullableInt32(7), // Order
                        reader.GetNullableDouble(8), // TimeLimit
                        reader.GetNullableDateTimeUtc(9), // Planned
                        reader.GetBoolean(10), // Hiden
                        reader.GetGuid(11), // StageTypeID
                        reader.GetNullableString(12), //StageTypeCaption
                        reader.GetNullableString(13), // SQLRoles
                        reader.GetNullableString(14), // Settings
                        reader.GetNullableString(15), // ExtraSources
                        reader.GetNullableString(16), // RuntimeSqlCondition
                        reader.GetNullableString(17), // SourceCondition
                        reader.GetNullableString(18), // SourceBefore
                        reader.GetNullableString(19), // SourceAfter
                        extraSourceSerializer,
                        stageSerializer,
                        skip: reader.GetBoolean(21),
                        canBeSkipped: reader.GetBoolean(22)
                    ));
                }
            }

            return stages;
        }

        #endregion

    }
}
