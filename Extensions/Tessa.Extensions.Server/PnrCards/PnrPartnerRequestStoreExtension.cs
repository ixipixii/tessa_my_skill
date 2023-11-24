using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Server.Integration;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Extensions.Shared.Settings;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    class PnrPartnerRequestStoreExtension : CardStoreExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrPartnerRequestStoreExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        //private void AddCategoryFileToErrorMessage(ref string message, string categoryFileName)
        //{
        //    if (String.IsNullOrEmpty(message))
        //        message += $"'{categoryFileName}'";
        //    else message += $",\n'{categoryFileName}'";
        //}

        //private async Task CheckFiles(int? typeID, ICardStoreExtensionContext context, Card card)
        //{
        //    var fileCategories = await ServerHelper.GetFileCategories(context.DbScope, card);

        //    string missingFilesMessage = "";
        //    // Категории документов с типом контрагента ЮЛ
        //    if (typeID == 1)
        //    {
        //        if (!fileCategories.Contains(PnrFileCategories.PnrAccountCardCompanyID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrAccountCardCompanyName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrBalanceSheetID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrBalanceSheetName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrLeaseContractID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrLeaseContractName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrDecisionCreateLegalEntityID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrDecisionCreateLegalEntityName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrOrderAppointmentGeneralDirectorID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrOrderAppointmentGeneralDirectorName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrRSV1LastReportingDateID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrRSV1LastReportingDateName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrTINCertificateID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrTINCertificateName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrExtractUnifiedStateRegisterLegalEntitiesID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrExtractUnifiedStateRegisterLegalEntitiesName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrCertificateTerritorialInspectorateID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrCertificateTerritorialInspectorateName);
        //    }
        //    else if (typeID == 3 || typeID == 2)
        //    // Категории документов с типом контрагента ИП, ФЛ
        //    {
        //        if (!fileCategories.Contains(PnrFileCategories.PnrAccountCardPartnerID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrAccountCardPartnerName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrCertificateRegistrationIndividualID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrCertificateRegistrationIndividualName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrTINCertificateIndividualID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrTINCertificateIndividualName);
        //        if (!fileCategories.Contains(PnrFileCategories.PnrPassportID))
        //            AddCategoryFileToErrorMessage(ref missingFilesMessage, PnrFileCategories.PnrPassportName);
        //    }
        //    if (!String.IsNullOrEmpty(missingFilesMessage))
        //        context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы:\n{missingFilesMessage}");
        //}

        /// <summary>
        /// Проверка заполненности полей Заявки контрагента. Вынесено на сервер, т.к. поля могут быть пустыми при интеграции.
        /// </summary>
        private async Task ValidatePartnerRequestAsync(IValidationResultBuilder validationResult, ICardStoreExtensionContext context, Card card)
        {
            int requestTypeID = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "PnrPartnerRequests", "RequestTypeID");

            int? directionID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "DirectionID");
            PnrCardFieldValidationHelper.Validate(validationResult, directionID, "Направление");

            // Особый признак контрагента (обязателен и при Согласовании(он теперь открыт), и при Создании)
            int? specialSignID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "SpecialSignID");
            PnrCardFieldValidationHelper.Validate(validationResult, specialSignID, "Особый признак контрагента");

            if (requestTypeID == 1)
            {
                // Согласование КА
                
                // КА
                Guid? partnerID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrPartnerRequests", "PartnerID");
                PnrCardFieldValidationHelper.Validate(validationResult, partnerID, "Контрагент");

                // Приложенные файлы (если известен Тип КА и Особый признак контрагента != Монополист)
                // Тип контрагента
                //int? typeID = null;
                //// -------------------------
                //CardRequest request = new CardRequest
                //{
                //    RequestType = Shared.PnrRequestTypes.PartnerInfo,
                //    Info =
                //    {
                //        { "partnerID", partnerID }
                //    }
                //};
                //CardResponse response = await cardRepository.RequestAsync(request);
                //ValidationResult result = response.ValidationResult.Build();
                //if (result.IsSuccessful)
                //{
                //    typeID = response.Info.Get<int>("TypeID");
                //}
                
                //if(typeID != null && specialSignID != 1)
                //{
                //    await CheckFiles(typeID, context, card);
                //}
                // -------------------------
            }
            else if (requestTypeID == 0)
            {
                // Создание нового КА, валидируется группа полей

                // typeID: 1 - ЮЛ, 2 - ФЛ, 3 - ИП
                // specialSignID: 0 - Нет, 1 - Монополист, 2 - Гос.органы
                // Тип контрагента
                int? typeID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "TypeID");
                PnrCardFieldValidationHelper.Validate(validationResult, typeID, "Тип контрагента");

                // Нерезидент (0 - Нет, 1 - Да)
                int? nonResidentID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPartnerRequests", "NonResidentID");

                // ИНН
                // Обязательно, если «Тип контрагента» = «ЮЛ»/ «ИП». 
                // Необязательно, если «Особый признак контрагента» = «Гос.органы» либо если признак Нерезидент = Да.
                if ((typeID == 1 || typeID == 3) && 
                    (specialSignID != 2) &&
                    (nonResidentID != 1)
                    )
                {
                    string inn = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrPartnerRequests", "INN");
                    PnrCardFieldValidationHelper.Validate(validationResult, inn, "ИНН");
                }

                // КПП
                // Обязательно, если «Тип контрагента» = «ЮЛ». 
                // Необязательно, если «Особый признак контрагента» = «Гос.органы», признак Нерезидент = Да
                if (typeID == 1 &&
                    (specialSignID != 2) &&
                    (nonResidentID != 1)
                    )
                {
                    string kpp = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrPartnerRequests", "KPP");
                    PnrCardFieldValidationHelper.Validate(validationResult, kpp, "КПП");
                }

                if (typeID != null && (typeID == 1 || typeID == 2))
                {
                    Guid? countryRegistration = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrPartnerRequests", "CountryRegistrationID");
                    PnrCardFieldValidationHelper.Validate(validationResult, countryRegistration, "Страна регистрации");
                }

                string fullName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrPartnerRequests", "FullName");
                PnrCardFieldValidationHelper.Validate(validationResult, fullName, "Полное наименование контрагента");

                string shortName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrPartnerRequests", "ShortName");
                PnrCardFieldValidationHelper.Validate(validationResult, shortName, "Краткое наименование контагента");

                // приложенные файлы
                //bool? requiresApprovalCA = await PnrDataHelper.GetActualFieldValueAsync<bool?>(context.DbScope, card, "PnrPartnerRequests", "RequiresApprovalCA");
                //// файлы валидируем при: известен Тип КА, установлен Требует согласование КА, Особый признак контрагента != Монополист
                //if (typeID != null && requiresApprovalCA != null && requiresApprovalCA == true && specialSignID != 1)
                //    await CheckFiles(typeID, context, card);
            }
        }

        private async Task ValidateCreatePartnerRequest(ICardStoreExtensionContext context, Card card)
        {
            // Тип заявки
            int requestTypeID = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "PnrPartnerRequests", "RequestTypeID");

            // КА
            Guid? partnerID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrPartnerRequests", "PartnerID");

            // заявка на создание, но при этом задан КА
            // такое может быть:
            // 1. после ответа из НСИ, происходит проставление связи между заявкой и КА
            // 2. баг, например создали заявку на согласование, указали КА, а потом поменяли на заявку на создание
            // в любом случае такая проверка обязательна
            if (requestTypeID == 0
                && partnerID != null)
            {
                using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;
                    var createPartnerRequestIDs =
                        await db
                            .SetCommand(@"select distinct ID
                                        from PnrPartnerRequests p with(nolock)
                                        where p.PartnerID = @partnerID
                                        and p.RequestTypeID = @requestTypeID",
                                        // заявка привязана к указанному КА
                                        db.Parameter("@partnerID", partnerID.Value),
                                        // заявка на создание
                                        db.Parameter("@requestTypeID", 0, LinqToDB.DataType.Int16))
                            .ExecuteListAsync<Guid>();

                    // убедимся, что для указанного КА нет других заявок на создание
                    if (createPartnerRequestIDs.Any(x => x != card.ID))
                    {
                        context.ValidationResult.AddError("Для указанного контрагента уже существует заявка на создание!");
                    }
                }
            }
        }

        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            // выполним валидацию типа заявки на создание
            // не будем игнорировать эту проверку даже при интеграции
            await ValidateCreatePartnerRequest(context, card);

            bool skipValidation = PnrCardFieldValidationHelper.IsPnrSkipCardFieldsCustomValidation(context);
            if (!skipValidation)
            {
                // валидация полей
                await ValidatePartnerRequestAsync(context.ValidationResult, context, card);
            }
        }
    }
}
