using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrSuppAgrStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null
                || !sections.TryGetValue("PnrSupplementaryAgreements", out CardSection pnrSupplementaryAgreements))
            {
                return;
            }

            var authorID = (Guid) card.Sections["DocumentCommonInfo"].Fields["AuthorID"];
            var userDepartment = await PnrUserDepartmentHelper.GetDepartmentByUserID(context.DbScope, authorID);
            if (userDepartment != null && userDepartment.ID != null)
            {
                pnrSupplementaryAgreements.Fields["DepartmentID"] = userDepartment.ID;
                pnrSupplementaryAgreements.Fields["DepartmentIdx"] = userDepartment.Idx;
                pnrSupplementaryAgreements.Fields["DepartmentName"] = userDepartment.Name;
            }

            Dictionary<string, object> pnrSupplementaryAgreementsFields = pnrSupplementaryAgreements.RawFields;

            var typeID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrSupplementaryAgreements", "TypeID");
            var implementationStageID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrSupplementaryAgreements", "ImplementationStageID");
            var kindDUPID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrSupplementaryAgreements", "KindDUPID");
            var formID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrSupplementaryAgreements", "FormID");
            bool isProjectInArchive = await PnrDataHelper.GetActualFieldValueAsync<bool>(context.DbScope, card, "PnrSupplementaryAgreements", "IsProjectInArchive");

            bool isHeadEstimateDepartmentInStage = (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID) &&
                (formID != PnrContractForms.PnrWithMonopolistID);

            bool isProjectEconomistInStage = !isProjectInArchive;

            bool isHeadDepartmentMTOInStage =
                ((typeID == PnrContractTypes.PnrDeliveryContractID || typeID == PnrContractTypes.PnrFinishingID || typeID == PnrContractTypes.PnrLandscapingID) ||
                (implementationStageID == PnrImplementationStages.PnrLandscapingID || implementationStageID == PnrImplementationStages.PnrFinishingID)) &&
                (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID && kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID) &&
                (formID != PnrContractForms.PnrWithMonopolistID);

            bool isHeadDepartmentSVISInStage =
                ((typeID == PnrContractTypes.PnrConstructionEngineeringID || typeID == PnrContractTypes.PnrGeneralContractID || typeID == PnrContractTypes.PnrGeneralDesignAgreementID) ||
                (implementationStageID == PnrImplementationStages.PnrConceptID || implementationStageID == PnrImplementationStages.PnrGPID || implementationStageID == PnrImplementationStages.PnrEngineeringID)) &&
                (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID && kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID);

            bool isHeadDepartmentSPIAInStage =
                ((typeID == PnrContractTypes.PnrFinishingID || typeID == PnrContractTypes.PnrLandscapingID ||
                    typeID == PnrContractTypes.PnrGeneralContractID || typeID == PnrContractTypes.PnrGeneralDesignAgreementID) ||
                (implementationStageID == PnrImplementationStages.PnrLandscapingID || implementationStageID == PnrImplementationStages.PnrFinishingID ||
                 implementationStageID == PnrImplementationStages.PnrConceptID || implementationStageID == PnrImplementationStages.PnrGPID ||
                 implementationStageID == PnrImplementationStages.PnrPDID)) &&
                (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID && kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID);

            bool isProjectGIPInStage =
                ((typeID == PnrContractTypes.PnrConstructionWorksID || typeID == PnrContractTypes.PnrConstructionEngineeringID) ||
                (implementationStageID == PnrImplementationStages.PnrEngineeringID || implementationStageID == PnrImplementationStages.PnrOtherID ||
                 implementationStageID == PnrImplementationStages.PnrLandscapingID || implementationStageID == PnrImplementationStages.PnrGPID ||
                 implementationStageID == PnrImplementationStages.PnrFinishingID)) &&
                (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID && kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID) &&
                (formID != PnrContractForms.PnrWithMonopolistID) &&
                !isProjectInArchive;

            bool isConstructionManagerInStage =
                ((typeID == PnrContractTypes.PnrConstructionWorksID) ||
                (implementationStageID == PnrImplementationStages.PnrLandscapingID || implementationStageID == PnrImplementationStages.PnrFinishingID ||
                implementationStageID == PnrImplementationStages.PnrEngineeringID || implementationStageID == PnrImplementationStages.PnrOtherID ||
                implementationStageID == PnrImplementationStages.PnrGPID)) &&
                (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID && kindDUPID != PnrContractKinds.PnrContractDUPIntragroupID) &&
                (formID != PnrContractForms.PnrWithMonopolistID) &&
                !isProjectInArchive;

            var approvingPersons = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrApprovingPersons");
            bool isApprovingPersonsInStage = approvingPersons.Count > 0 && (kindDUPID != PnrContractKinds.PnrContractDUPNotBuildingID);

            bool isInternalApprovalStage = isHeadEstimateDepartmentInStage || isProjectEconomistInStage || isHeadDepartmentMTOInStage || isHeadDepartmentSVISInStage ||
                isHeadDepartmentSPIAInStage || isProjectGIPInStage || isConstructionManagerInStage || isApprovingPersonsInStage;

            pnrSupplementaryAgreements.RawFields["IsHeadDepartmentMTOInStage"] = isHeadDepartmentMTOInStage;
            pnrSupplementaryAgreements.RawFields["IsHeadDepartmentSVISInStage"] = isHeadDepartmentSVISInStage;
            pnrSupplementaryAgreements.RawFields["IsHeadDepartmentSPIAInStage"] = isHeadDepartmentSPIAInStage;
            pnrSupplementaryAgreements.RawFields["IsProjectGIPInStage"] = isProjectGIPInStage;
            pnrSupplementaryAgreements.RawFields["IsConstructionManagerInStage"] = isConstructionManagerInStage;
            pnrSupplementaryAgreements.RawFields["IsInternalApprovalStage"] = isInternalApprovalStage;

            //РАСЧЕТ СУММЫ ДС

            //decimal amountContract = 0, amountSA = 0;

            //if (pnrSupplementaryAgreements.Fields.ContainsKey("MainContractID") && card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"] != null)
            //{
            //    var contractID = (Guid)card.Sections["PnrSupplementaryAgreements"].Fields["MainContractID"];

            //    await using (context.DbScope.Create())
            //    {
            //        var db = context.DbScope.Db;
            //        amountContract = await db.SetCommand(
            //                "SELECT Amount FROM PnrContracts AS C "
            //                + "WHERE C.ID = @contractID;",
            //                db.Parameter("contractID", contractID))
            //            .LogCommand()
            //            .ExecuteAsync<decimal>();
            //    }
            //}

            //if (pnrSupplementaryAgreements.Fields.ContainsKey("AmountSA") && card.Sections["PnrSupplementaryAgreements"].Fields["AmountSA"] != null)
            //{
            //    amountSA = (decimal)card.Sections["PnrSupplementaryAgreements"].Fields["AmountSA"];
            //}

            //card.Sections["PnrSupplementaryAgreements"].Fields["Amount"] = amountSA + amountContract;
            //card.Sections["PnrSupplementaryAgreements"].SetChanged("Amount", false);
            
        }


        /// <summary>
        /// Проверка заполненности полей ДС. Вынесено на сервер, т.к. поля могут быть пустыми при интеграции.
        /// </summary>
        private async Task ValidateSuppAgrAsync(IDbScope dbScope, Guid suppAgrID, IValidationResultBuilder validationResult, Card card)
        {
            var suppAgr = await PnrSuppAgrHelper.GetSuppAgrByID(dbScope, suppAgrID);

            if (suppAgr == null)
            {
                validationResult.AddError($"Не удалось загрузить данные ДС. ID = {suppAgrID}");
                return;
            }

            PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.PartnerID, "Контрагент");

            PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.OrganizationID, "Организация ГК Пионер");

            PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.ProjectID, "Проект");

            //PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.TypeID, "Тип договора");

            // -------------------------------------------
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrSupplementaryAgreements = null,
                documentCommonInfo = null;
            if ((sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrSupplementaryAgreements", out pnrSupplementaryAgreements)
                    && !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }

            // Индекс ЮЛ - не участвует в формировании номера, исключим из валидации
            //Guid? organizationID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(dbScope, card, "PnrSupplementaryAgreements", "OrganizationID");
            //if (organizationID != null)
            //{
            //    string organizationLegalEntityIndex = (await PnrOrganizationHelper.GetOrganizationByID(dbScope, organizationID.Value))?.LegalEntityIndex;
            //    if (String.IsNullOrEmpty(organizationLegalEntityIndex))
            //    {
            //        string organizationName = await PnrDataHelper.GetActualFieldValueAsync<string>(dbScope, card, "PnrSupplementaryAgreements", "OrganizationName");
            //        validationResult.AddError(this, $"В организации {(String.IsNullOrEmpty(organizationName) ? "" : organizationName)} не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ\" обязательно к заполнению.");
            //    }
            //}

            // Валидация полей, зависящих от Вида документа
            Guid? kindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(dbScope, card, "PnrSupplementaryAgreements", "KindID");

            if (!Guid.Equals(kindID, PnrContractKinds.PnrContractWithBuyersID))
            {
                PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.CostItemID, "Наименование статьи затрат");

                PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.FormID, "Форма договора");

                PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.SignatoryID, "Подписант");

                PnrCardFieldValidationHelper.Validate(validationResult, suppAgr.AmountSA, "Сумма ДС");
            }

            switch (kindID)
            {
                case Guid standard when standard == new Guid(PnrContractKinds.PnrContractDUPID.ToString()):
                    int? phasedImplementationID = await PnrDataHelper.GetActualFieldValueAsync<int?>(dbScope, card, "PnrSupplementaryAgreements", "PhasedImplementationID");
                    PnrCardFieldValidationHelper.Validate(validationResult, phasedImplementationID, "Предусмотрено поэтапное выполнение");

                    DateTime? plannedActDate = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(dbScope, card, "PnrSupplementaryAgreements", "PlannedActDate");
                    PnrCardFieldValidationHelper.Validate(validationResult, plannedActDate, "Планируемая дата актирования");

                    Guid? kindDUPID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(dbScope, card, "PnrSupplementaryAgreements", "KindDUPID");
                    PnrCardFieldValidationHelper.Validate(validationResult, kindDUPID, "Вид договора ДУП");

                    break;
                case Guid standard when standard == new Guid(PnrContractKinds.PnrContractCFOID.ToString()):
                    //int? developmentID = await PnrDataHelper.GetActualFieldValueAsync<int?>(dbScope, card, "PnrSupplementaryAgreements", "DevelopmentID");
                    //PnrCardFieldValidationHelper.Validate(validationResult, developmentID, "Разработка договора");
                    
                    break;
            }

            bool isContractWithBuyers = Guid.Equals(kindID, PnrContractKinds.PnrContractWithBuyersID);
            bool isUntil2019 = await PnrDataHelper.GetActualFieldValueAsync<bool>(dbScope, card, "PnrSupplementaryAgreements", "IsUntil2019");
            if (!isContractWithBuyers)
            {
                if (!isUntil2019)
                {
                    Guid? mainContractID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(dbScope, card, "PnrSupplementaryAgreements", "MainContractID");
                    PnrCardFieldValidationHelper.Validate(validationResult, mainContractID, "Основной договор");
                }
            }

            // Редактирование номера (может и при создании, и при редактировании карточки)
            if (documentCommonInfo != null && validationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if (editFullNumber != null)
                {
                    await ServerHelper.UpdateDocumentNumber(dbScope, card.ID, editFullNumber.ToString());
                }
                else if (editNumber != null)
                {
                    string newFullNumber = $"Д-{(editNumber.ToString()).PadLeft(6, '0')}";
                    await ServerHelper.UpdateDocumentNumber(dbScope, card.ID, newFullNumber);
                }
            }
        }

        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            // флаг "пропустить валидацию", передается при интеграции, т.к. там приходят не все обязательные поля
            bool skipValidation = PnrCardFieldValidationHelper.IsPnrSkipCardFieldsCustomValidation(context);

            // пропустить валидацию если тип ДС - С покупателями (т.к. пользователи не могут редактировать файлы существующих карточек)
            // skipValidation = (Guid)card.Sections["PnrSupplementaryAgreements"].Fields["KindID"] == PnrContractKinds.PnrContractWithBuyersID;

            if (!skipValidation)
            {
                // валидация полей
                await ValidateSuppAgrAsync(context.DbScope, card.ID, context.ValidationResult, card);
            }
        }
    }
}
