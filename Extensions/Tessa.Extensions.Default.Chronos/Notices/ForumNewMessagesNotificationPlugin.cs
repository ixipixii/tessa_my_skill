using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Chronos.Contracts;
using NLog;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Forums;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Operations;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    /// <summary>
    /// Плагин, выполняющий добавление уведомлений о текущих заданиях пользователя.
    /// </summary>
    [Plugin(
        Name = "Forum new messages notifications plugin",
        Description = "Plugin starts at configured time and send emails to users with information about new unread messages in forums.",
        Version = 1,
        ConfigFile = ConfigFilePath)]
    public class ForumNewMessagesNotificationPlugin :
        Plugin

    {
        #region Constants

        /// <summary>
        /// Относительный путь к конфигурационному файлу плагина.
        /// </summary>
        public const string ConfigFilePath = "configuration/ForumNewMessagesNotification.xml";

        public const string FmMessagesPluginTable = nameof(FmMessagesPluginTable);

        private const string TimeoutTemplate =
            "Forums messages lock: failed to acquire, timeout in {0} seconds. Please, check whether forums messages lock is stuck" +
            " in active operations table \"Operations\". Remove the row if that`s the case.";

        #endregion

        #region Fields

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IDbScope dbScope;
        private ISession session;
        private INotificationManager notificationManager;
        private IOperationRepository operationRepository;
        private ITransactionStrategy transactionStrategy;

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

        /// <summary>
        /// Получает дату последнего запуска плагина.
        /// До этой даты "включительно" рассылались сообщения при прошлом запуске.
        /// </summary>
        /// <param name="db">DbManager</param>
        /// <param name="builderFactory">IQueryBuilderFactory</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Дату последнего запуска плагина</returns>
        private static async Task<DateTime> GetLastPluginRunDateAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            var lastPluginRunDate = await db
                .SetCommand(
                    builderFactory
                        .Select().Top(1).C("LastPluginRunDate")
                        .From(FmMessagesPluginTable).NoLock()
                        .Limit(1)
                        .Build())
                .LogCommand()
                .ExecuteAsync<DateTime>(cancellationToken);

            return DateTime.SpecifyKind(lastPluginRunDate, DateTimeKind.Utc);
        }

        /// <summary>
        /// Обновляет дату последнего запуска плагина.
        /// До этой даты "включительно" рассылались сообщения при текущем запуске.
        /// </summary>
        /// <param name="db">DbManager</param>
        /// <param name="builderFactory">IQueryBuilderFactory</param>
        /// <param name="newLastPluginRunDate">Новая дата последнего запуска плагина.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Асинхронная задача.</returns>
        private static async Task UpdateLastPluginRunDateAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            DateTime newLastPluginRunDate,
            CancellationToken cancellationToken = default)
        {
            int updated = await db
                .SetCommand(
                    builderFactory
                        .Update(FmMessagesPluginTable)
                        .C("LastPluginRunDate").Equals().P("NewLastPluginRunDate")
                        .Build(),
                    db.Parameter("NewLastPluginRunDate", newLastPluginRunDate))
                .LogCommand()
                .ExecuteNonQueryAsync(cancellationToken);

            if (updated == 0)
            {
                await db
                    .SetCommand(
                        builderFactory
                            .InsertInto(FmMessagesPluginTable, "LastPluginRunDate")
                            .Values(v => v.P("NewLastPluginRunDate"))
                            .Build(),
                        db.Parameter("NewLastPluginRunDate", newLastPluginRunDate))
                    .LogCommand()
                    .ExecuteNonQueryAsync(cancellationToken);
            }
        }

        private async Task<IList<TopicNotificationInfo>> GetNotificationsInfoAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            string normalizedWebAddress,
            DateTime currentPluginRunDateTime,
            DateTime lastPluginRunDateTime,
            CancellationToken cancellationToken = default)
        {
            var result = new List<TopicNotificationInfo>();

            await using (var reader = await db
                .SetCommand(
                    builderFactory
                        .Select()
                        .C("usr", "UserID")
                        .C("fs", "MainCardID")
                        .C("msg", "TopicRowID")
                        .C("tp", "Title")
                        .C("msg", "Created", "AuthorName", "Body")
                        .C("tp", "Description")
                        .From("FmMessages", "msg").NoLock()
                        .InnerJoin("FmTopics", "tp").NoLock()
                        .On().C("msg", "TopicRowID").Equals().C("tp", "RowID")
                        .InnerJoin("FmForumSatellite", "fs").NoLock()
                        .On().C("tp", "ID").Equals().C("fs", "ID")
                        .InnerJoinLateral(q => q
                                .SelectDistinct()
                                .C("t", "TopicRowID", "UserID")
                                .From(t => t
                                        .Select()
                                        .C("tp", "TopicRowID", "UserID")
                                        .From("FmTopicParticipants", "tp").NoLock()
                                        .Where()
                                        .C("tp", "TopicRowID").Equals().C("msg", "TopicRowID")
                                        .UnionAll()
                                        .Select()
                                        .C("tpr", "TopicRowID")
                                        .C("ru", "UserID")
                                        .From("FmTopicParticipantRoles", "tpr").NoLock()
                                        .InnerJoin(RoleStrings.RoleUsers, "ru").NoLock()
                                        .On().C("tpr", "RoleID").Equals().C("ru", "ID")
                                        .Where()
                                        .C("tpr", "TopicRowID").Equals().C("msg", "TopicRowID"),
                                    "t")
                            , "usr")
                        .InnerJoin("FmUserStat", "fst").NoLock()
                        .On()
                        .C("tp", "RowID").Equals().C("fst", "TopicRowID")
                        .And()
                        .C("usr", "UserID").Equals().C("fst", "UserID")
                        .Where()
                        .C("msg", "Created").LessOrEquals().P("CurrentPluginRunDateTime")
                        .And()
                        .C("msg", "Created").Greater().P("LastPluginRunDateTime")
                        .And()
                        .C("msg", "Created").Greater().C("fst", "LastReadMessageTime")
                        .OrderBy("usr", "UserID").By("usr", "TopicRowID").By("msg", "Created")
                        .Build(),
                    db.Parameter("CurrentPluginRunDateTime", currentPluginRunDateTime),
                    db.Parameter("LastPluginRunDateTime", lastPluginRunDateTime))
                .LogCommand()
                .WithoutTimeout()
                .ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var htmlText = ForumSerializeHelper.DeserializeMessageBody(reader.GetValue<string>(6)).Text;
                    htmlText = Regex.Replace(htmlText, "color:#[0-9a-fA-F]{8}", p => p.Value.Substring(0, p.Value.Length - 2));

                    var info = new TopicNotificationInfo()
                    {
                        UserID = reader.GetGuid(0),
                        CardID = reader.GetGuid(1),
                        TopicID = reader.GetGuid(2),
                        TopicTitle = HttpUtility.HtmlEncode(reader.GetValue<string>(3)),
                        MessageDate = reader.GetValue<DateTime>(4),
                        AuthorName = reader.GetValue<string>(5),
                        HtmlText = htmlText,
                        TopicDescription = reader.GetValue<string>(7),
                    };

                    info.Link = CardHelper.GetLink(this.session, info.CardID);

                    info.WebLink = CardHelper.GetWebLink(normalizedWebAddress, info.CardID, normalize: false);

                    result.Add(info);
                }
            }

            return result.OrderBy(p => p.UserID).ToList();
        }

        private static void ProceedNotificationRow(
            TopicNotificationInfo info,
            ForumMessagesNotification currentNotification)
        {
            if (info != null)
            {
                info.HtmlText = Regex.Replace(info.HtmlText, "<img ", "<img alt=\"[image]\" ");
                currentNotification.TopicsNotifications.Add(info);
            }
        }

        private async Task ProcessOperationAsync(CancellationToken cancellationToken = default)
        {
            if (this.StopRequested)
            {
                return;
            }

            logger.Trace("Forums new messages notifications processing: started");

            await using (this.dbScope.Create())
            {
                await this.operationRepository.ExecuteInLockAsync(
                    "$Forums_Operation_NewMessagesNotificationsSending",
                    lockOperationTypeID: OperationTypes.ForumNewMessagesProcessingNotificationsOperationID,
                    timeoutSeconds: 300,
                    timeoutMessage: TimeoutTemplate,
                    lockName: "Forums messages lock",
                    logger: logger,
                    cancellationToken: cancellationToken,
                    actionFunc: async ct =>
                    {
                        var db = this.dbScope.Db;
                        var builderFactory = this.dbScope.BuilderFactory;

                        try
                        {
                            if (this.StopRequested)
                            {
                                return;
                            }

                            // Записываем текущую дату/время запуска в переменную
                            // и считываем из базы дату/время предыдущего запуска
                            DateTime currentPluginRunDateTime = DateTime.UtcNow;
                            DateTime lastPluginRunDateTime = await GetLastPluginRunDateAsync(db, builderFactory, cancellationToken)
                                ;

                            // Получаем кусочек с web адресом для ссылки на ЛК
                            string normalizedWebAddress =
                                    await GetNormalizedWebAddressAsync(db, builderFactory, cancellationToken)
                                ;

                            logger.Trace("Getting notifications.");

                            // Инфо упорядочена по пользователям
                            IList<TopicNotificationInfo> notificationList =
                                    await this.GetNotificationsInfoAsync(
                                        db,
                                        builderFactory,
                                        normalizedWebAddress,
                                        currentPluginRunDateTime,
                                        lastPluginRunDateTime,
                                        cancellationToken)
                                ;

                            if (this.StopRequested || notificationList.Count == 0)
                            {
                                return;
                            }

                            logger.Trace("Processing notifications.");

                            // Открываем транзакцию, чтобы, если что,
                            // всегда можно было откатиться на момент до отправки первого уведомления
                            var validationResult = new ValidationResultBuilder();
                            bool success = await this.transactionStrategy.ExecuteInTransactionAsync(
                                validationResult,
                                async p =>
                                {
                                    ForumMessagesNotification currentNotification =
                                        new ForumMessagesNotification(
                                            notificationList[0].UserID,
                                            normalizedWebAddress);

                                    // Парсим уведомления
                                    foreach (TopicNotificationInfo notification in notificationList)
                                    {
                                        if (currentNotification.UserID != notification.UserID)
                                        {
                                            var sendResult = await notificationManager.SendAsync(
                                                DefaultNotifications.ForumNewMessagesNotification,
                                                new[] { currentNotification.UserID },
                                                new NotificationSendContext
                                                {
                                                    ExcludeDeputies = true,
                                                    MainCardID = currentNotification.UserID,
                                                    Info = currentNotification.GetInfo(),
                                                },
                                                cancellationToken);

                                            logger.LogResult(sendResult);

                                            // Начинаем обработку следующего пользователя
                                            currentNotification =
                                                new ForumMessagesNotification(
                                                    notification.UserID,
                                                    normalizedWebAddress);

                                            ProceedNotificationRow(notification, currentNotification);
                                        }
                                        else
                                        {
                                            ProceedNotificationRow(notification, currentNotification);
                                        }
                                    }

                                    var lastSendResult = await notificationManager.SendAsync(
                                        DefaultNotifications.ForumNewMessagesNotification,
                                        new[] { currentNotification.UserID },
                                        new NotificationSendContext
                                        {
                                            ExcludeDeputies = true,
                                            MainCardID = currentNotification.UserID,
                                            Info = currentNotification.GetInfo(),
                                        },
                                        cancellationToken);

                                    logger.LogResult(lastSendResult);

                                    // Обновляем дату последнего запуска плагина (LastPluginRunDate) из даты СurrentPluginRunDateTime
                                    logger.Trace("Updating LastPluginRunDate.");
                                    await UpdateLastPluginRunDateAsync(db, builderFactory, currentPluginRunDateTime, cancellationToken);
                                },
                                cancellationToken);

                            if (!success)
                            {
                                throw new ValidationException(validationResult.Build());
                            }
                            
                            logger.Trace("Notifications were successfully processed.");
                        }
                        catch (Exception ex)
                        {
                            // исключения игнорируются
                            logger.LogException(ex);

                            if (db.Transaction != null)
                            {
                                try
                                {
                                    await db.RollbackTransactionAsync(ct);
                                }
                                catch (Exception ex2)
                                {
                                    // исключения игнорируются
                                    logger.LogException(ex2);
                                }
                            }
                        }
                    });
            }

            logger.Trace("Forums new messages notifications processing: completed");
        }

        #endregion

        #region Base Overrides

        public override async Task EntryPointAsync(CancellationToken cancellationToken = default)
        {
            logger.Trace("Starting forum new messages notifications plugin.");

            TessaPlatform.InitializeFromConfiguration();

            IUnityContainer container = await new UnityContainer().RegisterServerForPluginAsync();

            this.dbScope = container.Resolve<IDbScope>();
            this.session = container.Resolve<ISession>();
            this.operationRepository = container.Resolve<IOperationRepository>();
            this.transactionStrategy = container.Resolve<ITransactionStrategy>();
            this.notificationManager = container.Resolve<INotificationManager>();

            await this.ProcessOperationAsync(cancellationToken);
        }

        #endregion
    }
}