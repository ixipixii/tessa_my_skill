using System;
using System.Collections.Generic;
using Tessa.Extensions.Server.Helpers;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Validation;
using Tessa.Extensions.Server.DataHelpers;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    /// <summary>
    /// Класс проверки на приложенный файл при завершении заданий по процессу
    /// </summary>
    public sealed class PnrCheckingForAnAttachedFileTaskExtension : CardStoreTaskExtension
    {

        private void AddCategoryFileToErrorMessage(ref string message, string categoryFileName)
        {
            if (String.IsNullOrEmpty(message))
                message += $"'{categoryFileName}'";
            else message += $",\n'{categoryFileName}'";
        }

        private async Task CheckFiles(int? typeID, ICardStoreTaskExtensionContext context, Card card, List<Guid> fileCategories)
        {
            string missingFilesMessage = "";
            // Категории документов с типом контрагента ЮЛ
            if (typeID == 1)
            {
                if (!fileCategories.Contains(PnrFileCategories.PnrAccountCardCompanyID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrAccountCardCompanyName);
                if (!fileCategories.Contains(PnrFileCategories.PnrBalanceSheetID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrBalanceSheetName);
                if (!fileCategories.Contains(PnrFileCategories.PnrLeaseContractID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrLeaseContractName);
                if (!fileCategories.Contains(PnrFileCategories.PnrDecisionCreateLegalEntityID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrDecisionCreateLegalEntityName);
                if (!fileCategories.Contains(PnrFileCategories.PnrOrderAppointmentGeneralDirectorID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrOrderAppointmentGeneralDirectorName);
                if (!fileCategories.Contains(PnrFileCategories.PnrRSV1LastReportingDateID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrRSV1LastReportingDateName);
                if (!fileCategories.Contains(PnrFileCategories.PnrTINCertificateID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrTINCertificateName);
                if (!fileCategories.Contains(PnrFileCategories.PnrExtractUnifiedStateRegisterLegalEntitiesID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrExtractUnifiedStateRegisterLegalEntitiesName);
                if (!fileCategories.Contains(PnrFileCategories.PnrCertificateTerritorialInspectorateID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrCertificateTerritorialInspectorateName);
            }
            else if (typeID == 3)
            // Категории документов с типом контрагента ИП
            {
                if (!fileCategories.Contains(PnrFileCategories.PnrAccountCardPartnerID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrAccountCardPartnerName);
                if (!fileCategories.Contains(PnrFileCategories.PnrCertificateRegistrationIndividualID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrCertificateRegistrationIndividualName);
                if (!fileCategories.Contains(PnrFileCategories.PnrTINCertificateIndividualID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrTINCertificateIndividualName);
                if (!fileCategories.Contains(PnrFileCategories.PnrPassportID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrPassportName);
            }
            else if (typeID == 2)
            // Категории документов с типом контрагента ФЛ
            {
                if (!fileCategories.Contains(PnrFileCategories.PnrAccountCardPartnerID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrAccountCardPartnerName);
                //if (!fileCategories.Contains(PnrFileCategories.PnrCertificateRegistrationIndividualID))
                //    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrCertificateRegistrationIndividualName);
                if (!fileCategories.Contains(PnrFileCategories.PnrTINCertificateIndividualID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrTINCertificateIndividualName);
                if (!fileCategories.Contains(PnrFileCategories.PnrPassportID))
                    AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrPassportName);
            }
            if (!String.IsNullOrEmpty(missingFilesMessage))
                context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы:\n{missingFilesMessage}");
        }

        public override async Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            CardTask tasks;
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (tasks = context.Task) == null
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }
            await CheckingForAnAttachedFile(context, tasks, card);
        }

        /// <summary>
        /// Проверка приложенного скана-копии при регистрации ВХ
        /// </summary>
        public async Task CheckingForAnAttachedFile(ICardStoreTaskExtensionContext context, CardTask tasks, Card card)
        {
            var fileCategories = await ServerHelper.GetFileCategories(context.DbScope, card);

            // Проверка приложенного скана-копии при регистрации ВХ
            if (card.TypeID == PnrCardTypes.PnrIncomingTypeID && tasks.OptionID == PnrCompletionOptions.PnrRegistration)
            {
                // Пока нет категорий файлов по входящим, данная проверка не позволяет провести процесс регистрации
                //if (!fileCategories.Contains(PnrFileCategories.PnrScanCopyID))
                //    context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrScanCopyName}");

                // Пока нет категорий файлов по входящим, проверим наличие приложенного скана-копии по количеству файлов в карточке
                var numberOfFiles = await ServerHelper.GetNumberOfFiles(context.DbScope, card.ID);                
                if (!(numberOfFiles > 0))
                { 
                    context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrScanCopyName}"); 
                }
            }

            // Проверка приложенного Проекта доверенности к Доверенности
            if (card.TypeID == PnrCardTypes.PnrPowerAttorneyTypeID && tasks.OptionID == PnrCompletionOptions.FdStartProcess)
            {
                // Пока нет категорий файлов по доверенностям, данная проверка не позволяет запустить процесс
                //if (!fileCategories.Contains(PnrFileCategories.PnrDraftPowerOfAttorneyID))
                //    context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrDraftPowerOfAttorneyName}");

                // Пока нет категорий файлов по доверенностям, проверим наличие приложенного Проекта доверенности по количеству файлов в карточке
                var numberOfFiles = await ServerHelper.GetNumberOfFiles(context.DbScope, card.ID);
                if (!(numberOfFiles > 0))
                {
                    context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrDraftPowerOfAttorneyName}");
                }
            }

            // Проверка приложенного Проекта служебной записки к СЗ
            if (card.TypeID == PnrCardTypes.PnrServiceNoteTypeID && tasks.OptionID == PnrCompletionOptions.FdStartProcess)
            {
                // Пока нет категорий файлов по Служебным запискам, данная проверка не позволяет запустить процесс
                //if (!fileCategories.Contains(PnrFileCategories.PnrProjectServiceNoteID))
                //    context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrProjectServiceNoteName}");

                // Пока нет категорий файлов по Служебным запискам, проверим наличие приложенного Проекта служебной записки по количеству файлов в карточке
                var numberOfFiles = await ServerHelper.GetNumberOfFiles(context.DbScope, card.ID);
                if (!(numberOfFiles > 0))
                {
                    context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrProjectServiceNoteName}");
                }

            }

            // Проверка приложенного Файла приказа к Приказу
            //if (card.TypeID == PnrCardTypes.PnrOrderUKTypeID && tasks.OptionID == PnrCompletionOptions.FdStartProcess)
            //{
            //    if (!fileCategories.Contains(PnrFileCategories.PnrOrderID))
            //        context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrOrderName}");
            //}

            // Проверка приложенных файлов к Заявке на КА при запуске процесса
            if (card.TypeID == PnrCardTypes.PnrPartnerRequestTypeID && 
                (tasks.OptionID == PnrCompletionOptions.FdStartProcess || tasks.OptionID == PnrCompletionOptions.SendToPerformer))
            {
                // файлы валидируем при: известен Тип КА, Особый признак контрагента != Монополист, Требует согласование КА == true
                // Тип контрагента
                int? typeID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "TypeID");
                // Особый признак контрагента (обязателен и при Согласовании(он теперь открыт), и при Создании)
                int? specialSignID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "SpecialSignID");
                // Тип заявки
                int? requestTypeID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "RequestTypeID");
                // Требует согласование КА
                // bool? requiresApprovalCA = await PnrDataHelper.GetActualFieldValueAsync<bool?>(context.DbScope, card, "PnrPartnerRequests", "RequiresApprovalCA");
                // bool isRequiresApprovalCA = requiresApprovalCA.HasValue ? requiresApprovalCA.Value : false;

                if (typeID != null && specialSignID != 1 && requestTypeID == PnrPartnerRequestsTypes.ApproveID)
                {
                    await CheckFiles(typeID, context, card, fileCategories);
                }
            }
        }
    }
}
