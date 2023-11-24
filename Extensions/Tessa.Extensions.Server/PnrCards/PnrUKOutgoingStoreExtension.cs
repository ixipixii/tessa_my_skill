using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrUKOutgoingStoreExtension : CardStoreExtension
    {
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

            if (!skipValidation)
            {
                // валидация полей
                await ValidateUKOutgoingAsync(context, card);
            }
        }

        private async Task ValidateUKOutgoingAsync(ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrOutgoingUK = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrOutgoingUK", out pnrOutgoingUK)
                    & !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }

            var validationResult = context.ValidationResult;

            //var uKOutgoingDocumentKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid>(context.DbScope, card, "PnrOutgoingUK", "DocumentKindID");
            //var uKOutgoingSignatoryId = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "SignatoryId");

            //if (uKOutgoingDocumentKindID == PnrOutgoingUKTypes.OutgoingUKLetterID)
            //{
            //    PnrCardFieldValidationHelper.Validate(validationResult, uKOutgoingSignatoryId, "Подписант");
            //}

            var approverID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoingUK", "ApproverID");
            PnrCardFieldValidationHelper.Validate(validationResult, approverID, "Утверждающий");

            // Индекс ЮЛ 
            string organizationLegalEntityIndex = null;
            Guid? organizationID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoingUK", "OrganizationID");
            if (organizationID != null)
            {
                organizationLegalEntityIndex = (await PnrOrganizationHelper.GetOrganizationByID(context.DbScope, organizationID.Value))?.LegalEntityIndex;
                if (String.IsNullOrEmpty(organizationLegalEntityIndex))
                {
                    string organizationName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "OrganizationName");
                    context.ValidationResult.AddError(this, $"В организации {(String.IsNullOrEmpty(organizationName) ? "" : organizationName)} не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ\" обязательно к заполнению.");
                }
                else if (pnrOutgoingUK != null && pnrOutgoingUK.RawFields.ContainsKey("LegalEntityIndexIdx"))
                    pnrOutgoingUK.RawFields["LegalEntityIndexIdx"] = organizationLegalEntityIndex;  // убрать при рефакторинге
            }

            // при редактировании карточки формируем ее номер (в случае успешной валидации)
            if (card.StoreMode == CardStoreMode.Update && context.ValidationResult.Count == 0)
            {
                if (pnrOutgoingUK != null)
                {
                    // в формировании номера: Вид входящего документа, Подразделение, Индекс ЮЛ
                    // если поля или часть полей присутствуют(изменялись) - переформируем номер
                    if (pnrOutgoingUK.RawFields.ContainsKey("DocumentKindIdx") ||
                        pnrOutgoingUK.RawFields.ContainsKey("DepartmentIdx") ||
                        pnrOutgoingUK.RawFields.ContainsKey("LegalEntityIndexIdx")
                        )
                    {
                        string documentKindIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "DocumentKindIdx");
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "LegalEntityIndexIdx");

                        string currentFullNumber = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "FullNumber");
                        string prefix = currentFullNumber.StartsWith("Пр-") ? "Пр-" : "";

                        int number = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        // номер выделен, сформируем полный номер
                        //{ f: PnrOutgoingUK.DocumentKindIdx}-{ f: PnrOutgoingUK.DepartmentIdx}-{ f: PnrOutgoingUK.LegalEntityIndexIdx}/{ 00n}
                        string fullNumber = $"{prefix}{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(number.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, fullNumber);
                    }
                }
            }

            // Редактирование номера (при редактировании карточки)
            if (card.StoreMode == CardStoreMode.Update && documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if (editFullNumber != null)
                {
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, editFullNumber.ToString());
                }
                else if (editNumber != null)
                {
                    string documentKindIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "DocumentKindIdx");
                    string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "DepartmentIdx");
                    string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoingUK", "LegalEntityIndexIdx");

                    string currentFullNumber = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "FullNumber");
                    string prefix = currentFullNumber.StartsWith("Пр-") ? "Пр-" : "";

                    //{ f: PnrOutgoingUK.DocumentKindIdx}-{ f: PnrOutgoingUK.DepartmentIdx}-{ f: PnrOutgoingUK.LegalEntityIndexIdx}/{ 00n}
                    string newFullNumber = $"{prefix}{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(editNumber.ToString()).PadLeft(3, '0')}";

                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                }
            }
        }
    }
}
