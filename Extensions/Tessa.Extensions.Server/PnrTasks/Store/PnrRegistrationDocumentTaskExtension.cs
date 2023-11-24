using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Notices;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    /// <summary>
    /// Регистрация документа при task PnrCompletionOptions.PnrRegistration
    /// </summary>
    public sealed class PnrRegistrationDocumentTaskExtension : CardStoreTaskExtension
    {
        private readonly ICardRepository extendedCardRepositoryWithoutTransaction;
        private readonly IKrTokenProvider krTokenProvider;
        private readonly IKrProcessLauncher launcher;

        private readonly ICardTransactionStrategy transactionStrategy;
        private readonly INotificationManager notificationManager;
        private readonly INotificationRoleAggregator roleAggregator;

        public PnrRegistrationDocumentTaskExtension(ICardRepository extendedCardRepositoryWithoutTransaction,
            IKrTokenProvider krTokenProvider,
            IKrProcessLauncher launcher,

            ICardTransactionStrategy transactionStrategy,
            INotificationManager notificationManager,
            INotificationRoleAggregator roleAggregator)
        {
            this.extendedCardRepositoryWithoutTransaction = extendedCardRepositoryWithoutTransaction;
            this.krTokenProvider = krTokenProvider;
            this.launcher = launcher;

            this.transactionStrategy = transactionStrategy;
            this.notificationManager = notificationManager;
            this.roleAggregator = roleAggregator;
        }

        public async Task StartRegistrationProcess(ICardStoreTaskExtensionContext context, Card card)
        {
            var process = KrProcessBuilder
                .CreateProcess()
                .SetProcess(Guid.Parse("79b58276-e886-41dd-9267-646124a621fc"))
                .SetCard(card.ID)
                .Build();

            var result = this.launcher.Launch(process, context);

            if (!result.ValidationResult.IsSuccessful())
            {
                if (result.ValidationResult[0].Message != "Запуск вторичного процесса запрещен ограничениями.")
                    throw new Exception(($"Не удалось зарегистрировать документ. Возможно отсутствуют права на регистрацию, обратитесь к администратору системы. {result.ValidationResult.Build()}").Replace("Error: ", ""));
            }
            else
            {
                // если Приказ и распоряжение - нужно сгенерировать и записать в базу регистрационный номер (в настройках Типового решения этот шаблон невозможно прописать)
                if (card.TypeID == PnrCardTypes.PnrOrderTypeID)
                    await OrderRegistrationNumber.SetOrderRegistrationNumber(context.DbScope, card);
            }
        }

        //public override async Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        //{
        //    CardTask tasks;
        //    Card card;
        //    if (!context.ValidationResult.IsSuccessful()
        //        || (tasks = context.Task) == null
        //        || (card = context.Request.TryGetCard()) == null)
        //    {
        //        return;
        //    }
        //    await RegisterDocument(context, tasks, card);
        //}

        public override async Task StoreTaskBeforeCommitTransaction(ICardStoreTaskExtensionContext context)
        {
            CardTask tasks;
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (tasks = context.Task) == null
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }
            await RegisterDocument(context, tasks, card);
        }

        /// <summary>
        /// Регистрация документа
        /// </summary>
        public async Task RegisterDocument(ICardStoreTaskExtensionContext context, CardTask tasks, Card card)
        {
            // Автоматическая регистрация Входящего после сохранения карточки
            if(card.TypeID == PnrCardTypes.PnrIncomingTypeID && tasks.OptionID == null)
            {
                await StartRegistrationProcess(context, card);
            }

            if (card.TypeID == PnrCardTypes.PnrOutgoingTypeID && tasks.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                await StartRegistrationProcess(context, card);
            }

            // Исходящие - рекламации, Исходящие УК - рекламации
            // Присвоение номера после отправки На исполнение
            if ((card.TypeID == PnrCardTypes.PnrOutgoingTypeID || card.TypeID == PnrCardTypes.PnrOutgoingUKTypeID)
                && tasks.OptionID == PnrCompletionOptions.PnrToExecution)
            {
                await StartRegistrationProcess(context, card);

                Guid cardID = card.ID;
                DateTime? registrationDate = DateTime.Now;
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;

                    await db.SetCommand(
                        "UPDATE [dbo].[PnrOutgoing] "
                        + "SET RegistrationDate = @registrationDate "
                        + "WHERE ID = @cardID;",
                    db.Parameter("cardID", cardID),
                    db.Parameter("registrationDate", registrationDate))
                    .LogCommand()
                    .ExecuteNonQueryAsync();
                }
            }

            // Исходящие УК, Приказ и распоряжение УК
            // Присвоение номера после Согласовать
            if ((card.TypeID == PnrCardTypes.PnrOutgoingUKTypeID || card.TypeID == PnrCardTypes.PnrOrderUKTypeID)
                && tasks.OptionID == PnrCompletionOptions.PnrAgreed)
            {
                await StartRegistrationProcess(context, card);
            }

            // Приказ и распоряжение
            // для Распоряжение, Приказа по основной деятельности, Приказа АХО и корп.мо.связи
            // присвоение номера после Проверка оформления - Проверено
            // для Приказа по реализации
            // присвоение номера после Согласование руководителем проекта, директором по маркетингу, директором по продажам - Согласовать
            // (фактически на начале этапа Подписание)
            //if (card.TypeID == PnrCardTypes.PnrOrderTypeID)
            //{
            //    string documentsGroupIndex = await OrderRegistrationNumber.GetDocumentsGroupIndex(context.DbScope, card);
            //    // "3-01" по АХО
            //    // "3-02" по осн.деят-ти
            //    // "3-04" по обеспечению КМС
            //    // "3-05" распоряжения
            //    if ((documentsGroupIndex == "3-01" || documentsGroupIndex == "3-02" || documentsGroupIndex == "3-04" || documentsGroupIndex == "3-05")
            //        && tasks.OptionID == PnrCompletionOptions.PnrCheck)
            //        await StartRegistrationProcess(context, card);
            //    // "3-03" по реализации (регистрацию запускаем перед началом этапа Подписание (т.к. 3 Согласования - то это показатель, что все 3 Согласовать отработаны положительно))
            //    if ((documentsGroupIndex == "3-03")
            //        && tasks.TypeID == PnrTaskTypes.PnrTaskSigningTypeID
            //        && tasks.Action == CardTaskAction.None)
            //        await StartRegistrationProcess(context, card);
            //}

            //if (card.TypeID == PnrCardTypes.PnrOrderTypeID && tasks.OptionID == PnrCompletionOptions.PnrCheck)
            // Приказы: Этап Регистрация, нажатие кнопки В работу
            if  (card.TypeID == PnrCardTypes.PnrOrderTypeID &&
                tasks.TypeID == PnrTaskTypes.PnrRegistrationTypeID &&
                tasks.Action == CardTaskAction.Progress)
            {
                await StartRegistrationProcess(context, card);
                return;
            }

            // Приказы по реализация автостарт регистрации при запуске процесса
            if (card.TypeID == PnrCardTypes.PnrOrderTypeID &&
                tasks.TypeID == PnrTaskTypes.PnrRegistrationAutoStartTypeID &&
                tasks.Action == CardTaskAction.Complete)
            {
                await StartRegistrationProcess(context, card);
                return;
            }


            // Доверенность
            if (card.TypeID == PnrCardTypes.PnrPowerAttorneyTypeID && tasks.OptionID == PnrCompletionOptions.PnrCheck)
            {
                await StartRegistrationProcess(context, card);
            }

            // Исходящий
            if (card.TypeID == PnrCardTypes.PnrOutgoingTypeID && tasks.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                await StartRegistrationProcess(context, card);
            }
            
            // Входящий УК
            if (card.TypeID == PnrCardTypes.PnrIncomingUKTypeID && tasks.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                await StartRegistrationProcess(context, card);
            }
        }

        public async Task SetOrderRegistrationNumber(ICardStoreTaskExtensionContext context, Card card)
        {
            
        }
    }
}
