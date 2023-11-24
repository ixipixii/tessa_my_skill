using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Server.Notices
{
    public sealed class MessageProcessor :
        IMessageProcessor
    {
        #region Fields

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IDbScope dbScope;
        private readonly ISession session;
        private readonly ICardRepository extendedRepositoryWithoutTransaction;
        private readonly ICardMetadata cardMetadata;
        private readonly ICardGetStrategy cardGetStrategy;
        private readonly ICardServerPermissionsProvider permissionsProvider;
        private readonly ICardTransactionStrategy cardTransactionStrategy;
        private readonly SetProcessorTokenAction setSessionToken;
        private readonly Func<IMessageHandler>[] resolveHandlersArray;

        #endregion

        #region Constructors

        public MessageProcessor(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]
            ICardRepository extendedRepositoryWithoutTransaction,
            ICardTransactionStrategy cardTransactionStrategy,
            ICardGetStrategy cardGetStrategy,
            ICardMetadata cardMetadata,
            IDbScope dbScope,
            ISession session,
            ICardServerPermissionsProvider permissionsProvider,
            SetProcessorTokenAction setSessionToken,
            IUnityContainer unityContainer)
        {
            this.extendedRepositoryWithoutTransaction = extendedRepositoryWithoutTransaction;
            this.cardTransactionStrategy = cardTransactionStrategy;
            this.cardGetStrategy = cardGetStrategy;
            this.cardMetadata = cardMetadata;
            this.dbScope = dbScope;
            this.session = session;
            this.permissionsProvider = permissionsProvider;
            this.setSessionToken = setSessionToken;
            this.resolveHandlersArray = unityContainer.ResolveAll<Func<IMessageHandler>>().ToArray();
        }

        #endregion

        #region Private Methods

        private static async Task<ISessionToken> TryFindUserByEmailAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            string email,
            CancellationToken cancellationToken = default)
        {
            db
                .SetCommand(
                    builderFactory
                        .Select().Top(1)
                        .C("pr", "ID", "Name")
                        .Coalesce(b => b.C("s", "LanguageCode").C("kr", "NotificationsDefaultLanguageCode"))
                        .From("PersonalRoles", "pr").NoLock()
                        .LeftJoin("PersonalRoleSatellite", "s").NoLock()
                        .On().C("s", "MainCardID").Equals().C("pr", "ID")
                        .CrossJoin("KrSettings", "kr").NoLock()
                        .Where().LowerC("pr", "Email").Equals().LowerP("Email")
                        .Limit(1)
                        .Build(),
                    db.Parameter("Email", SqlHelper.NotNull(email, RoleHelper.UserEmailMaxLength)))
                .LogCommand();

            await using DbDataReader reader = await db.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                logger.Error("User not found. Email: {0}", email);
                return null;
            }

            Guid userID = reader.GetGuid(0);
            string userName = reader.GetValue<string>(1);

            string languageCode = reader.GetValue<string>(2);
            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = LocalizationManager.EnglishLanguageCode;
            }

            CultureInfo userCulture = CultureInfo.GetCultureInfo(languageCode);

            return new SessionToken(
                userID,
                userName,
                UserAccessLevel.Regular,
                culture: userCulture,
                uiCulture: userCulture);
        }


        private static Task<Guid?> GetCardIDAsync(
            DbManager db,
            IQueryBuilderFactory builderFactory,
            Guid taskID,
            CancellationToken cancellationToken = default) =>
            db
                .SetCommand(
                    builderFactory
                        .Select().C("ID")
                        .From("Tasks").NoLock()
                        .Where().C("RowID").Equals().P("RowID")
                        .Build(),
                    db.Parameter("RowID", taskID))
                .ExecuteAsync<Guid?>(cancellationToken);


        private async Task CompleteTaskAsync(
            Guid taskRowID,
            DbManager db,
            IQueryBuilderFactory builderFactory,
            IMessageInfo info,
            NoticeMessage message,
            IMessageHandler handler,
            CancellationToken cancellationToken = default)
        {
            Guid? cardID = await GetCardIDAsync(db, builderFactory, taskRowID, cancellationToken);
            if (!cardID.HasValue)
            {
                logger.Error("Could not find task by id. TaskID='{0}'", taskRowID.ToString());
                return;
            }

            var validationResult = new ValidationResultBuilder();

            await this.cardTransactionStrategy
                .ExecuteInWriterLockAsync(
                    (Guid) cardID,
                    CardComponentHelper.DoNotCheckVersion,
                    validationResult,
                    async p =>
                    {
                        // загружаем карточку с нужным заданием
                        // потом помечаем задание как завершённое с вариантом "Согласовать" или "Не согласовать"
                        // сохраняем карточку, не забыв вызвать card.RemoveAllButChanged()

                        // здесь нельзя делать RestrictTasks, т.к. от наличия видимых заданий могут зависеть права на карточку;
                        // также нельзя делать RestrictTaskSections, т.к. некоторые расширения требуют секций у заданий (такие как WfTasksServerGetExtension)

                        var getRequest = new CardGetRequest
                        {
                            CardID = cardID,
                            GetMode = CardGetMode.ReadOnly,
                            RestrictionFlags =
                                CardGetRestrictionFlags.RestrictFiles
                                | CardGetRestrictionFlags.RestrictTaskCalendar
                                | CardGetRestrictionFlags.RestrictTaskHistory
                        };

                        getRequest.IgnoreButtons();

                        CardGetResponse getResponse = await this.extendedRepositoryWithoutTransaction.GetAsync(getRequest, p.CancellationToken);
                        ValidationResult getResult = getResponse.ValidationResult.Build();
                        logger.LogResult(getResult);

                        if (!getResult.IsSuccessful)
                        {
                            p.ReportError = true;
                            return;
                        }

                        Card card = getResponse.Card;
                        card.Tasks.Clear();

                        IList<CardGetContext> taskContextList =
                            await this.cardGetStrategy.TryLoadTaskInstancesAsync(
                                (Guid) cardID,
                                card,
                                p.DbScope.Db,
                                this.cardMetadata,
                                p.ValidationResult,
                                this.session.User.ID,
                                loadCalendarInfo: false,
                                taskRowIDList: new[] { taskRowID },
                                cancellationToken: p.CancellationToken);

                        if (taskContextList.Count == 0)
                        {
                            logger.Error(
                                "User UserName='{0}', Email='{1}', UserID='{2}', doesn't have permissions to complete task"
                                + " TaskID='{3}' for card CardID='{4}', or task has been completed already.",
                                this.session.User.Name,
                                message.From,
                                this.session.User.ID,
                                taskRowID,
                                cardID);

                            p.ReportError = true;
                            return;
                        }

                        CardTask taskToComplete = card.Tasks[0];

                        // Получаем секции задания
                        CardGetContext taskContext = taskContextList[0];
                        await this.cardGetStrategy.LoadSectionsAsync(taskContext, p.CancellationToken);

                        try
                        {
                            var context = new MessageHandlerContext(
                                info,
                                message,
                                this.session,
                                p.DbScope.Db,
                                p.DbScope.BuilderFactory,
                                card,
                                taskToComplete,
                                cancellationToken);

                            await handler.HandleAsync(context);

                            if (context.Cancel)
                            {
                                p.ReportError = true;
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);

                            logger.Error(
                                "Message handling has failed. From: '{0}', Subject: '{1}'.",
                                message.From,
                                message.Subject);

                            p.ReportError = true;
                            return;
                        }

                        string cardDigest = await this.extendedRepositoryWithoutTransaction.GetDigestAsync(
                            card, CardDigestEventNames.ActionHistoryMobileApproval, p.CancellationToken);

                        card.RemoveAllButChanged();

                        CardStoreRequest storeRequest = new CardStoreRequest { Card = card };
                        storeRequest.SetDigest(cardDigest);
                        this.permissionsProvider.SetFullPermissions(storeRequest);

                        CardStoreResponse storeResponse = await this.extendedRepositoryWithoutTransaction.StoreAsync(storeRequest, p.CancellationToken);
                        ValidationResult storeResult = storeResponse.ValidationResult.Build();
                        logger.LogResult(storeResult);

                        if (!storeResult.IsSuccessful)
                        {
                            p.ReportError = true;
                        }
                    },
                    cancellationToken: cancellationToken);

            logger.LogResult(validationResult);
        }

        #endregion

        #region IMessageProcessor Methods

        public async Task ProcessMessageAsync(NoticeMessage message, CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builderFactory = this.dbScope.BuilderFactory;

                ISessionToken user = await TryFindUserByEmailAsync(db, builderFactory, message.From, cancellationToken);
                if (user != null)
                {
                    LocalizationManager.CurrentUICulture = user.UICulture;
                    LocalizationManager.CurrentCulture = user.Culture;
                    this.setSessionToken(user);

                    IMessageInfo info = null;
                    IMessageHandler handler = null;

                    try
                    {
                        for (int i = 0; i < this.resolveHandlersArray.Length; i++)
                        {
                            handler = this.resolveHandlersArray[i]();

                            info = await handler.TryParseAsync(message, cancellationToken);
                            if (info != null)
                            {
                                break;
                            }
                        }

                        if (info == null)
                        {
                            logger.Error(
                                "Can't find parser for the message. From: '{0}', Subject: '{1}'.",
                                message.From,
                                message.Subject);
                        }
                    }
                    catch (Exception ex)
                    {
                        info = null;
                        logger.LogException(ex);

                        logger.Error(
                            "Message parsing has failed. From: '{0}', Subject: '{1}'.",
                            message.From,
                            message.Subject);
                    }

                    if (info != null)
                    {
                        try
                        {
                            Guid? taskRowID = info.TaskRowID;
                            if (taskRowID.HasValue)
                            {
                                logger.Trace(
                                    "Completing task. From: '{0}', Subject: '{1}', TaskID: '{2}', Handler: '{3}'.",
                                    message.From,
                                    message.Subject,
                                    taskRowID.Value,
                                    handler.GetType().FullName);

                                await this.CompleteTaskAsync(taskRowID.Value, db, builderFactory, info, message, handler, cancellationToken);
                            }
                            else
                            {
                                logger.Trace(
                                    "Processing message. From: '{0}', Subject: '{1}', Handler: '{2}'.",
                                    message.From,
                                    message.Subject,
                                    handler.GetType().FullName);

                                var context = new MessageHandlerContext(
                                    info,
                                    message,
                                    this.session,
                                    db,
                                    builderFactory,
                                    cancellationToken: cancellationToken);

                                await handler.HandleAsync(context);
                            }

                            logger.Trace(
                                "Message is processed. From: '{0}', Subject: '{1}', Handler: '{2}'.",
                                message.From,
                                message.Subject,
                                handler.GetType().FullName);
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);

                            logger.Error(
                                "Message handling has failed. From: '{0}', Subject: '{1}'.",
                                message.From,
                                message.Subject);
                        }
                    }
                }
            }
        }

        #endregion
    }
}