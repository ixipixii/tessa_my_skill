using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrContractStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null
                || !sections.TryGetValue("PnrContracts", out var pnrContracts))
                return;

            // Подразделение автора
            var authorID = (Guid) card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
            var userDepartment = await PnrUserDepartmentHelper.GetDepartmentByUserID(context.DbScope, authorID);
            if (userDepartment != null && userDepartment.ID != null)
            {
                card.Sections["PnrContracts"].Fields["DepartmentID"] = userDepartment.ID;
                card.Sections["PnrContracts"].Fields["DepartmentIdx"] = userDepartment.Idx;
                card.Sections["PnrContracts"].Fields["DepartmentName"] = userDepartment.Name;
            }

            var typeID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "TypeID");
            var implementationStageID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "ImplementationStageID");
            var kindDUPID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "KindDUPID");
            var formID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "FormID");
            var isProjectInArchive = await PnrDataHelper.GetActualFieldValueAsync<bool>(context.DbScope, card, "PnrContracts", "IsProjectInArchive");

            var isHeadEstimateDepartmentInStage = kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID &&
                                                  formID != PnrContractForms.PnrWithMonopolistID;

            var isProjectEconomistInStage = !isProjectInArchive;

            var isHeadDepartmentMTOInStage =
                (typeID == PnrContractTypes.PnrDeliveryContractID ||
                 typeID == PnrContractTypes.PnrFinishingID ||
                 typeID == PnrContractTypes.PnrLandscapingID ||
                 implementationStageID == PnrImplementationStages.PnrLandscapingID ||
                 implementationStageID == PnrImplementationStages.PnrFinishingID) &&
                kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID &&
                kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID &&
                formID != PnrContractForms.PnrWithMonopolistID;

            var isHeadDepartmentSVISInStage =
                (typeID == PnrContractTypes.PnrConstructionEngineeringID ||
                 typeID == PnrContractTypes.PnrGeneralContractID ||
                 typeID == PnrContractTypes.PnrGeneralDesignAgreementID ||
                 implementationStageID == PnrImplementationStages.PnrConceptID ||
                 implementationStageID == PnrImplementationStages.PnrGPID ||
                 implementationStageID == PnrImplementationStages.PnrEngineeringID) &&
                kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID &&
                kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID;

            var isHeadDepartmentSPIAInStage =
                (typeID == PnrContractTypes.PnrFinishingID ||
                 typeID == PnrContractTypes.PnrLandscapingID ||
                 typeID == PnrContractTypes.PnrGeneralContractID ||
                 typeID == PnrContractTypes.PnrGeneralDesignAgreementID ||
                 implementationStageID == PnrImplementationStages.PnrLandscapingID ||
                 implementationStageID == PnrImplementationStages.PnrFinishingID ||
                 implementationStageID == PnrImplementationStages.PnrConceptID ||
                 implementationStageID == PnrImplementationStages.PnrGPID ||
                 implementationStageID == PnrImplementationStages.PnrPDID) &&
                kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID &&
                kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID;

            var isProjectGIPInStage =
                (typeID == PnrContractTypes.PnrConstructionWorksID ||
                 typeID == PnrContractTypes.PnrConstructionEngineeringID ||
                 implementationStageID == PnrImplementationStages.PnrEngineeringID ||
                 implementationStageID == PnrImplementationStages.PnrOtherID ||
                 implementationStageID == PnrImplementationStages.PnrLandscapingID ||
                 implementationStageID == PnrImplementationStages.PnrGPID ||
                 implementationStageID == PnrImplementationStages.PnrFinishingID) &&
                kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID &&
                kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID &&
                formID != PnrContractForms.PnrWithMonopolistID && !isProjectInArchive;

            var isConstructionManagerInStage =
                (typeID == PnrContractTypes.PnrConstructionWorksID ||
                 implementationStageID == PnrImplementationStages.PnrLandscapingID ||
                 implementationStageID == PnrImplementationStages.PnrFinishingID ||
                 implementationStageID == PnrImplementationStages.PnrEngineeringID ||
                 implementationStageID == PnrImplementationStages.PnrOtherID ||
                 implementationStageID == PnrImplementationStages.PnrGPID) &&
                kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID &&
                kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID &&
                formID != PnrContractForms.PnrWithMonopolistID && !isProjectInArchive;

            var approvingPersons =
                await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrApprovingPersons");
            var isApprovingPersonsInStage =
                approvingPersons.Count > 0 && kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID;

            var isInternalApprovalStage = isHeadEstimateDepartmentInStage || isProjectEconomistInStage ||
                                          isHeadDepartmentMTOInStage || isHeadDepartmentSVISInStage ||
                                          isHeadDepartmentSPIAInStage || isProjectGIPInStage ||
                                          isConstructionManagerInStage || isApprovingPersonsInStage;

            pnrContracts.RawFields["IsHeadDepartmentMTOInStage"] = isHeadDepartmentMTOInStage;
            pnrContracts.RawFields["IsHeadDepartmentSVISInStage"] = isHeadDepartmentSVISInStage;
            pnrContracts.RawFields["IsHeadDepartmentSPIAInStage"] = isHeadDepartmentSPIAInStage;
            pnrContracts.RawFields["IsProjectGIPInStage"] = isProjectGIPInStage;
            pnrContracts.RawFields["IsConstructionManagerInStage"] = isConstructionManagerInStage;
            pnrContracts.RawFields["IsInternalApprovalStage"] = isInternalApprovalStage;
        }


        /// <summary>
        ///     Проверка заполненности полей договора. Вынесено на сервер, т.к. поля могут быть пустыми при интеграции.
        /// </summary>
        private async Task ValidateContractAsync(IValidationResultBuilder validationResult,
            ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrContracts = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || !sections.TryGetValue("PnrContracts", out pnrContracts)
                && !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
            )
                return;

            var kindID =
                await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "KindID");
            PnrCardFieldValidationHelper.Validate(validationResult, kindID, "Вид договора");

            var projectID =
                await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "ProjectID");
            PnrCardFieldValidationHelper.Validate(validationResult, projectID, "Проект");

            var organizationID =
                await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts",
                    "OrganizationID");
            PnrCardFieldValidationHelper.Validate(validationResult, organizationID, "Организация ГК Пионер");

            // Индекс ЮЛ 
            string organizationLegalEntityIndex = null;
            if (organizationID != null)
            {
                organizationLegalEntityIndex =
                    (await PnrOrganizationHelper.GetOrganizationByID(context.DbScope, organizationID.Value))
                    ?.LegalEntityIndex;
                if (string.IsNullOrEmpty(organizationLegalEntityIndex))
                {
                    var organizationName =
                        await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrContracts",
                            "OrganizationName");
                    context.ValidationResult.AddError(this,
                        $"В организации {(string.IsNullOrEmpty(organizationName) ? "" : organizationName)} не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ\" обязательно к заполнению.");
                }
                else if (pnrContracts != null && pnrContracts.RawFields.ContainsKey("LegalEntityIndexIdx"))
                {
                    pnrContracts.RawFields["LegalEntityIndexIdx"] =
                        organizationLegalEntityIndex; // убрать при рефакторинге
                }
            }

            if (kindID != null)
            {
                // общая валидация для видов договора, исключая "С покупателями"
                if (kindID == PnrContractKinds.PnrContractDUPID || kindID == PnrContractKinds.PnrContractCFOID ||
                    kindID == PnrContractKinds.PnrContractIntercompanyID)
                {
                    var projectDate =
                        await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrContracts",
                            "ProjectDate");
                    PnrCardFieldValidationHelper.Validate(validationResult, projectDate, "Дата заключения");

                    var subject =
                        await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrContracts",
                            "Subject");
                    PnrCardFieldValidationHelper.Validate(validationResult, subject, "Предмет договора");

                    var partnerID =
                        await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts",
                            "PartnerID");
                    PnrCardFieldValidationHelper.Validate(validationResult, partnerID, "Контрагент");

                    var cfoID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card,
                        "PnrContracts", "CFOID");
                    PnrCardFieldValidationHelper.Validate(validationResult, cfoID, "ЦФО");

                    var costItemID =
                        await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts",
                            "CostItemID");
                    PnrCardFieldValidationHelper.Validate(validationResult, costItemID, "Наименование статьи затрат");

                    var amount =
                        await PnrDataHelper.GetActualFieldValueAsync<decimal?>(context.DbScope, card, "PnrContracts",
                            "Amount");
                    PnrCardFieldValidationHelper.Validate(validationResult, amount, "Сумма договора");

                    var plannedActDate =
                        await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrContracts",
                            "PlannedActDate");
                    PnrCardFieldValidationHelper.Validate(validationResult, plannedActDate,
                        "Планируемая дата актирования");

                    var formID =
                        await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts",
                            "FormID");
                    PnrCardFieldValidationHelper.Validate(validationResult, formID, "Форма договора");

                    var typeID =
                        await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts",
                            "TypeID");
                    PnrCardFieldValidationHelper.Validate(validationResult, typeID, "Тип договора");

                    //Guid? signatoryID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts", "SignatoryID");
                    //PnrCardFieldValidationHelper.Validate(validationResult, signatoryID, "Подписант");
                }

                // Валидация полей, зависящих от Вида договора
                switch (kindID)
                {
                    // Договор ДУП
                    case Guid standard when standard == new Guid(PnrContractKinds.PnrContractDUPID.ToString()):
                        var phasedImplementationID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope,
                            card, "PnrContracts", "PhasedImplementationID");
                        PnrCardFieldValidationHelper.Validate(validationResult, phasedImplementationID,
                            "Предусмотрено поэтапное выполнение");

                        var implementationStageID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope,
                            card, "PnrContracts", "ImplementationStageID");
                        PnrCardFieldValidationHelper.Validate(validationResult, implementationStageID,
                            "Стадия реализации");

                        var kindDUPID =
                            await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrContracts",
                                "KindDUPID");
                        PnrCardFieldValidationHelper.Validate(validationResult, kindDUPID, "Вид договора (ДУП)");

                        break;
                    // Договор ЦФО
                    case Guid standard when standard == new Guid(PnrContractKinds.PnrContractCFOID.ToString()):
                        //int? developmentID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrContracts", "DevelopmentID");
                        //PnrCardFieldValidationHelper.Validate(validationResult, developmentID, "Разработка договора");

                        break;
                }
            }

            // если в текущем сеансе работы с карточкой менялась Организация - необходимо переформирование номера.
            if (pnrContracts != null && context.ValidationResult.Count == 0 &&
                pnrContracts.RawFields.TryGetValue("OrganizationID", out var organizationField)
            )
            {
                var number =
                    await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo",
                        "Number");
                var newFullNumber = $"7-01-{organizationLegalEntityIndex}/{number.ToString().PadLeft(4, '0')}";

                await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
            }

            // Редактирование номера (может и при создании, и при редактировании карточки)
            if (documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out var editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out var editFullNumber);

                if (editNumber != null || editFullNumber != null)
                {
                    // Шаблонная часть номера при редактировании номера не заполнена - формируем ее по старой схеме
                    if (editFullNumber == null)
                    {
                        var newFullNumber =
                            $"7-01-{organizationLegalEntityIndex}/{editNumber.ToString().PadLeft(4, '0')}";
                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                    }
#if false // Закомментирован в связи с тем, что при редактировании номера вручную добавляются лишние символы в конец номера
                    if (editNumber == null)
                    {
                        // Порядковый номер при редактировании номера не заполнен - берем выделенный ранее
                        int number =
 await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        string newFullNumber = $"{editFullNumber}/{(number.ToString()).PadLeft(4, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                    }
#endif
                }
            }
        }

        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (card = context.Request.TryGetCard()) == null)
                return;

            // флаг "пропустить валидацию", передается при интеграции, т.к. там приходят не все обязательные поля
            var skipValidation = PnrCardFieldValidationHelper.IsPnrSkipCardFieldsCustomValidation(context);

            if (!skipValidation)
                // валидация полей
                await ValidateContractAsync(context.ValidationResult, context, card);
        }
    }
}