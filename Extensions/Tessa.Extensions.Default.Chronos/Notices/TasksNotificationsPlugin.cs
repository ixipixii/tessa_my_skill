﻿using Chronos.Contracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Roles;
using Unity;
#pragma warning disable 1998

namespace Tessa.Extensions.Default.Chronos.Notices
{
    /// <summary>
    /// Плагин, выполняющий добавление уведомлений о текущих заданиях пользователя.
    /// </summary>
    [Plugin(
        Name = "Tasks notifications plugin",
        Description = "Plugin starts at configured time, collects info about users tasks and send emails to users.",
        Version = 1,
        ConfigFile = ConfigFilePath)]
    public sealed class TasksNotificationsPlugin :
        Plugin
    {
        #region Constants

        /// <summary>
        /// Относительный путь к конфигурационному файлу плагина.
        /// </summary>
        public const string ConfigFilePath = "configuration/TasksNotifications.xml";

        #endregion

        #region Fields

        private IDbScope dbScope;

        private ISession session;

        private INotificationManager notificationManager;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Private Methods

        private static async Task<string> GetNormalizedWebAddressAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            string webAddress = await db
                .SetCommand(
                    builderFactory
                        .Select().Top(1).C("WebAddress")
                        .From("ServerInstances").NoLock()
                        .Limit(1)
                        .Build())
                .LogCommand()
                .ExecuteAsync<string>(cancellationToken);

            return LinkHelper.NormalizeWebAddress(webAddress);
        }


        private async Task<IList<ITaskNotificationInfo>> GetNotificationsInfoAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            string normalizedWebAddress,
            CancellationToken cancellationToken = default)
        {
            var result = new List<ITaskNotificationInfo>();

            // Обработка обычных заданий

            await using (var reader = await db
                .SetCommand(
                    builderFactory
                        .Select()
                            .C("tsk", "ID") // 0
                            .C("dco", "FullNumber", "Subject") // 1-2
                            .C("rus", "UserID", "UserName") // 3-4
                            .C("tsk", "StateID", "Created", "Planned", "Digest", "AuthorName", "TypeID") // 5-10
                            .Coalesce(b => b
                                .C("tco", "KindCaption").C("tsk", "TypeCaption")) // 11
                            .C("tsk", "RowID") // 12
                            .C("sct", "UseDocTypes") // 13
                            .C("kdt", "UseApproving", "UseAutoApprove") // 14-15
                            .Function("CalendarAddWorkingDaysToDate", b => b
                                .If(Dbms.SqlServer,
                                    v => v.Q(" DATEADD(minute, ").C("tsk", "TimeZoneUtcOffsetMinutes").Q(", ")
                                        .P("CurrentDate").Q(")"))
                                .ElseIf(Dbms.PostgreSql,
                                    v => v.Q("(").P("CurrentDate").Add().C("tsk", "TimeZoneUtcOffsetMinutes")
                                        .Q(" * interval '1 minute')"))
                                .ElseThrow()
                                .RequireComma()
                                .C("kdt", "NotifyBefore")) // 16
                            .Function("CalendarAddWorkingDaysToDate", b => b
                                .If(Dbms.SqlServer,
                                    v => v.Q(" DATEADD(minute, ").C("tsk", "TimeZoneUtcOffsetMinutes").Q(", ")
                                        .C("tsk", "Planned").Q(")"))
                                .ElseIf(Dbms.PostgreSql,
                                    v => v.Q("(").C("tsk", "Planned").Add().C("tsk", "TimeZoneUtcOffsetMinutes")
                                        .Q(" * interval '1 minute')"))
                                .ElseThrow()
                                .RequireComma()
                                .C("kdt", "ExceededDays")) // 17
                            .C("sct", "UseApproving", "UseAutoApprove") // 18-19
                            .Function("CalendarAddWorkingDaysToDate", b => b
                                .If(Dbms.SqlServer,
                                    v => v.Q(" DATEADD(minute, ").C("tsk", "TimeZoneUtcOffsetMinutes").Q(", ")
                                        .P("CurrentDate").Q(")"))
                                .ElseIf(Dbms.PostgreSql,
                                    v => v.Q("(").P("CurrentDate").Add().C("tsk", "TimeZoneUtcOffsetMinutes")
                                        .Q(" * interval '1 minute')"))
                                .ElseThrow()
                                .RequireComma()
                                .C("sct", "NotifyBefore")) // 20
                            .Function("CalendarAddWorkingDaysToDate", b => b
                                .If(Dbms.SqlServer,
                                    v => v.Q(" DATEADD(minute, ").C("tsk", "TimeZoneUtcOffsetMinutes").Q(", ")
                                        .C("tsk", "Planned").Q(")"))
                                .ElseIf(Dbms.PostgreSql,
                                    v => v.Q("(").C("tsk", "Planned").Add().C("tsk", "TimeZoneUtcOffsetMinutes")
                                        .Q(" * interval '1 minute')"))
                                .ElseThrow()
                                .RequireComma()
                                .C("sct", "ExceededDays")) // 21
                        .From("Tasks", "tsk").NoLock()
                        .InnerJoin(RoleStrings.RoleUsers, "rus").NoLock()
                            .On().C("rus", "ID").Equals().C("tsk", "RoleID")
                        .InnerJoin(RoleStrings.PersonalRoles, "pro").NoLock()
                            .On().C("pro", "ID").Equals().C("rus", "UserID")
                        .LeftJoin("DocumentCommonInfo", "dco").NoLock()
                            .On().C("dco", "ID").Equals().C("tsk", "ID")
                        .LeftJoin("TaskCommonInfo", "tco").NoLock()
                            .On().C("tco", "ID").Equals().C("tsk", "RowID")
                        .InnerJoin("Instances", "ins").NoLock()
                            .On().C("ins", "ID").Equals().C("tsk", "ID")
                        .InnerJoin("KrSettingsCardTypes", "sct").NoLock()
                            .On().C("sct", "CardTypeID").Equals().C("ins", "TypeID")
                        .LeftJoin("KrDocType", "kdt").NoLock()
                            .On().C("kdt", "ID").Equals().C("dco", "DocTypeID")
                        .Where(
                            b => b
                                .C("tsk", "UserID").IsNull().Or()
                                .C("tsk", "UserID").Equals().C("rus", "UserID").Or()
                                .Exists(b1 => b1
                                    .Select().V(null)
                                    .From("RoleDeputies", "rd").NoLock()
                                    .Where().C("rd", "ID").Equals().C("tsk", "RoleID")
                                        .And().C("rd", "DeputyID").Equals().C("rus", "UserID")
                                        .And().C("rd", "DeputizedID").Equals().C("tsk", "UserID")
                                        .And().C("rd", "IsActive").Equals().V(true)))
                            .And().C("pro", "Email").IsNotNull()
                            .And().C("pro", "Email").NotEquals().V(string.Empty)
                        .OrderBy("rus", "UserID").By("tsk", "StateID").By("tsk", "Planned")
                        .Build(),
                    db.Parameter("CurrentDate", SqlHelper.NotNull(DateTime.UtcNow)))
                .LogCommand()
                .WithoutTimeout()
                .ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var info = new TaskNotificationInfo
                    {
                        CardID = reader.GetGuid(0),
                        CardNumber = reader.GetValue<string>(1),
                        CardSubject = reader.GetValue<string>(2),
                        RoleID = reader.GetGuid(3),
                        RoleName = reader.GetValue<string>(4),
                        InProgress = reader.GetInt16(5),
                        Created = reader.GetDateTimeUtc(6),
                        Planned = reader.GetDateTimeUtc(7),
                        TaskInfo = reader.GetValue<string>(8),
                        AuthorRole = reader.GetValue<string>(9),
                        TypeCaption = reader.GetValue<string>(11),
                        TaskID = reader.GetGuid(12),
                    };

                    info.LinkText =
                        Shared.Notices.NotificationHelper.GetNameForLink(
                            !string.IsNullOrEmpty(info.CardNumber) &&
                            !string.IsNullOrEmpty(info.CardSubject)
                                ? info.CardNumber + ", " + info.CardSubject
                                : null,
                            info.CardNumber,
                            null);
                    info.Link = CardHelper.GetLink(this.session, info.CardID);

                    info.WebLink = CardHelper.GetWebLink(normalizedWebAddress, info.CardID, normalize: false);

                    var taskTypeID = reader.GetGuid(10);
                    var useDocTypes = reader.GetBoolean(13);
                    var dtUseApproving = reader.GetValue<bool?>(14);
                    var dtUseAutoApprove = reader.GetValue<bool?>(15);
                    var dtNotifyBeforePlanned = reader.GetNullableDateTimeUtc(16);
                    var dtAutoApprovePlanned = reader.GetNullableDateTimeUtc(17);
                    var ctUseApproving = reader.GetValue<bool?>(18);
                    var ctUseAutoApprove = reader.GetValue<bool?>(19);
                    var ctNotifyBeforePlanned = reader.GetNullableDateTimeUtc(20);
                    var ctAutoApprovePlanned = reader.GetNullableDateTimeUtc(21);

                    if (taskTypeID == DefaultTaskTypes.KrApproveTypeID)
                    {
                        if (useDocTypes)
                        {
                            if (dtUseApproving.Value && dtUseAutoApprove.Value)
                            {
                                if (dtNotifyBeforePlanned.HasValue &&
                                    dtAutoApprovePlanned.HasValue &&
                                    dtNotifyBeforePlanned.Value >= dtAutoApprovePlanned.Value)
                                {
                                    info.AutoApproveString = "{$UI_Tasks_AutoApproveNotice} ";
                                    info.AutoApproveDate = dtAutoApprovePlanned.Value;
                                }
                                else
                                {
                                    info.AutoApproveString = null;
                                }
                            }
                            else
                            {
                                info.AutoApproveString = null;
                            }
                        }
                        else
                        {
                            if (ctUseApproving.Value && ctUseAutoApprove.Value)
                            {
                                if (ctNotifyBeforePlanned.HasValue &&
                                    ctAutoApprovePlanned.HasValue &&
                                    ctNotifyBeforePlanned.Value >= ctAutoApprovePlanned.Value)
                                {
                                    info.AutoApproveString = "{$UI_Tasks_AutoApproveNotice} ";
                                    info.AutoApproveDate = ctAutoApprovePlanned.Value;
                                }
                                else
                                {
                                    info.AutoApproveString = null;
                                }
                            }
                            else
                            {
                                info.AutoApproveString = null;
                            }

                        }
                    }
                    else
                    {
                        info.AutoApproveString = null;
                    }

                    result.Add(info);
                }
            }

            // Обработка автозавершённых заданий

            await using (var reader = await db
                .SetCommand(
                    builderFactory
                        .Select()
                            .C("aah", "CardID", "CardDigest", "UserID") // 0 - 2
                            .C("pr", "Name") // 3
                            .C("aah", "Date", "ID", "Comment") // 4-6
                        .From("AutoApproveHistory", "aah").NoLock()
                        .InnerJoin(RoleStrings.Roles, "r").NoLock()
                            .On().C("r", "ID").Equals().C("aah", "UserID")
                        .InnerJoin(RoleStrings.PersonalRoles, "pr").NoLock()
                            .On().C("pr", "ID").Equals().C("aah", "UserID")
                        .LeftJoin(RoleStrings.PersonalRoleSatellite, "prs").NoLock()
                            .On().C("prs", "MainCardID").Equals().C("aah", "UserID")
                        .CrossJoin("KrSettings", "krs").NoLock()
                        .Build())
                .LogCommand()
                .WithoutTimeout()
                .ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var info = new AutoAprovedTaskNotificationInfo
                    {
                        CardID = reader.GetGuid(0),
                        RoleID = reader.GetGuid(2),
                        RoleName = reader.GetValue<string>(3),
                        Date = reader.GetDateTimeUtc(4),
                        ID = reader.GetGuid(5),
                        Comment = reader.GetString(6),
                    };

                    var cardDigest = reader.GetValue<string>(1);

                    info.LinkText =
                        !string.IsNullOrEmpty(cardDigest)
                            ? cardDigest
                            : null;
                    info.Link = CardHelper.GetLink(this.session, info.CardID);
                    info.WebLink = CardHelper.GetWebLink(normalizedWebAddress, info.CardID, normalize: false);

                    result.Add(info);
                }
            }

            return result.OrderBy(p => p.RoleID).ToList();
        }

        private static void ProceedUserNotificationRow(
            ITaskNotificationInfo info,
            UserNotification currentUserNotification,
            ref List<(Guid TaskID, Guid UserID)> idsToRemove)
        {
            var normalInfo = info as TaskNotificationInfo;
            var autoApprovedInfo = info as AutoAprovedTaskNotificationInfo;

            if (normalInfo != null)
            {
                if (normalInfo.Planned <= DateTime.UtcNow)
                {
                    if (normalInfo.InProgress == 1)
                    {
                        currentUserNotification.OutdatedTasksInProgress.Add(normalInfo);
                    }
                    else
                    {
                        currentUserNotification.OutdatedTasks.Add(normalInfo);
                    }
                }
                else
                {
                    if (normalInfo.InProgress == 1)
                    {
                        currentUserNotification.TasksInProgress.Add(normalInfo);
                    }
                    else
                    {
                        currentUserNotification.Tasks.Add(normalInfo);
                    }
                }
            }

            if (autoApprovedInfo != null)
            {
                idsToRemove.Add((autoApprovedInfo.ID, autoApprovedInfo.RoleID));
                currentUserNotification.AutoApprovedTasks.Add(autoApprovedInfo);
            }
        }

        /// <summary>
        /// Удаляет из AutoApproveHistory записи по автоматически завершённым заданиям для указанного пользователя.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="builderFactory"></param>
        /// <param name="idsToRemove">Идентификаторы записей, который нужно удалить</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        private static async Task DropAutoApprovedTaskRowsForUserAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            ICollection<(Guid TaskID, Guid UserID)> idsToRemove,
            CancellationToken cancellationToken = default)
        {
            if (idsToRemove.Count == 0)
            {
                return;
            }

            foreach ((Guid TaskID, Guid UserID) idsSet in idsToRemove)
            {
                await db
                    .SetCommand(
                        builderFactory
                            .DeleteFrom("AutoApproveHistory")
                            .Where()
                                .C("ID").Equals().P("TaskID")
                                .And()
                            .C("UserID").Equals().P("UserID")
                            .Build(),
                        db.Parameter("TaskID", idsSet.TaskID),
                        db.Parameter("UserID", idsSet.UserID))
                    .LogCommand()
                    .ExecuteNonQueryAsync(cancellationToken);
            }
        }

        #endregion

        #region Base Overrides

        public override async Task EntryPointAsync(CancellationToken cancellationToken = default)
        {
            logger.Trace("Starting task notifications plugin.");

            TessaPlatform.InitializeFromConfiguration();

            IUnityContainer container = await new UnityContainer().RegisterServerForPluginAsync();

            this.dbScope = container.Resolve<IDbScope>();
            this.session = container.Resolve<ISession>();
            this.notificationManager = container.Resolve<INotificationManager>();

            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builderFactory = this.dbScope.BuilderFactory;

                string normalizedWebAddress = await GetNormalizedWebAddressAsync(db, builderFactory, cancellationToken);

                logger.Trace("Getting notifications.");

                // Инфо упорядочена по пользователям
                IList<ITaskNotificationInfo> notificationList = await this.GetNotificationsInfoAsync(db, builderFactory, normalizedWebAddress, cancellationToken);
                if (notificationList.Count == 0)
                {
                    return;
                }

                logger.Trace("Processing notifications.");

                // Получаем вычисленную локаль - в порядке приоритета - локаль пользователя, локаль типового решения, англ

                // Берем первого пользователя
                UserNotification currentUserNotification =
                    new UserNotification(
                        notificationList[0].RoleID,
                        normalizedWebAddress);

                var idsToRemove = new List<(Guid TaskID, Guid UserID)>();

                foreach (ITaskNotificationInfo notification in notificationList)
                {
                    //Тут короче мы видим что закончился наш пользователь и должны перейти к следующему
                    if (currentUserNotification.UserID != notification.RoleID)
                    {
                        var sendResult = await notificationManager.SendAsync(
                            DefaultNotifications.TasksNotification,
                            new[] { currentUserNotification.UserID },
                            new NotificationSendContext
                            {
                                ExcludeDeputies = true,
                                MainCardID = currentUserNotification.UserID,
                                Info = currentUserNotification.GetInfo(),
                            },
                            cancellationToken);

                        logger.LogResult(sendResult);
                        if (sendResult.IsSuccessful)
                        {
                            await DropAutoApprovedTaskRowsForUserAsync(db, builderFactory, idsToRemove, cancellationToken);
                        }

                        // Начинаем обработку следующего пользователя
                        currentUserNotification =
                            new UserNotification(
                                notification.RoleID,
                                normalizedWebAddress);

                        // Обрабатываем первую запись
                        idsToRemove.Clear();
                        ProceedUserNotificationRow(notification, currentUserNotification, ref idsToRemove);
                    }
                    // Пользователь еще не законченный - обрабатываем его дальше
                    else
                    {
                        ProceedUserNotificationRow(notification, currentUserNotification, ref idsToRemove);
                    }
                }

                var lastSendResult = await notificationManager.SendAsync(
                    DefaultNotifications.TasksNotification,
                    new[] { currentUserNotification.UserID },
                    new NotificationSendContext
                    {
                        ExcludeDeputies = true,
                        MainCardID = currentUserNotification.UserID,
                        Info = currentUserNotification.GetInfo(),
                    },
                    cancellationToken);

                logger.LogResult(lastSendResult);
                if (lastSendResult.IsSuccessful)
                {
                    await DropAutoApprovedTaskRowsForUserAsync(db, builderFactory, idsToRemove, cancellationToken);
                }
            }

            logger.Trace("Notifications were successfully processed.");
        }

        #endregion
    }
}
