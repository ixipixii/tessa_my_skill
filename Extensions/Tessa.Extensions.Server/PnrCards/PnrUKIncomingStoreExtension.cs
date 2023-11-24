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
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrUKIncomingStoreExtension : CardStoreExtension
    {
        private async Task ValidateContractAsync(IValidationResultBuilder validationResult, ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrIncomingUK = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrIncomingUK", out pnrIncomingUK)
                    & !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }

            string documentKindIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncomingUK", "DocumentKindIdx");

            // Валидация полей, зависящих от Вида входящего документа
            switch (documentKindIdx)
            {
                case "1-01":    // входящие ук

                    Guid? departmentID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncomingUK", "DepartmentID");
                    PnrCardFieldValidationHelper.Validate(validationResult, departmentID, "Подразделение");
                    
                    break;
            }

            // Индекс ЮЛ 
            string organizationLegalEntityIndex = null;
            Guid? organizationID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncomingUK", "OrganizationID");
            if (organizationID != null)
            {
                organizationLegalEntityIndex = (await PnrOrganizationHelper.GetOrganizationByID(context.DbScope, organizationID.Value))?.LegalEntityIndex;
                if (String.IsNullOrEmpty(organizationLegalEntityIndex))
                {
                    string organizationName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncomingUK", "OrganizationName");
                    context.ValidationResult.AddError(this, $"В организации {(String.IsNullOrEmpty(organizationName) ? "" : organizationName)} не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ\" обязательно к заполнению.");
                }
                else if (pnrIncomingUK != null && pnrIncomingUK.RawFields.ContainsKey("LegalEntityIndexIdx"))
                    pnrIncomingUK.RawFields["LegalEntityIndexIdx"] = organizationLegalEntityIndex;  // убрать при рефакторинге
            }

            // при редактировании карточки формируем ее номер (в случае успешной валидации)
            if (card.StoreMode == CardStoreMode.Update && context.ValidationResult.Count == 0)
            {
                if (pnrIncomingUK != null)
                {
                    // в формировании номера: Вид входящего документа, Подразделение, Индекс ЮЛ
                    // если поля или часть полей присутствуют(изменялись) - переформируем номер
                    if (pnrIncomingUK.RawFields.ContainsKey("DocumentKindIdx") ||
                        pnrIncomingUK.RawFields.ContainsKey("DepartmentIdx") ||
                        pnrIncomingUK.RawFields.ContainsKey("LegalEntityIndexIdx")
                        )
                    {
                        // DocumentKindIdx проверялось выше в процессе валидации, поэтому значение уже доступно в documentKindIdx
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncomingUK", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncomingUK", "LegalEntityIndexIdx");

                        int number = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        // номер выделен, сформируем полный номер
                        //{ f: PnrIncomingUK.DocumentKindIdx}-{ f: PnrIncomingUK.DepartmentIdx}-{ f: PnrIncomingUK.LegalEntityIndexIdx}/{ 00n}
                        string fullNumber = $"{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(number.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, fullNumber);
                    }
                }
            }

            // Редактирование номера (при редактировании карточки)
            if (card.StoreMode == CardStoreMode.Update && documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if (editNumber != null || editFullNumber != null)
                {
                    if (editFullNumber != null)
                    {
                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, editFullNumber.ToString());
                    } else if (editNumber != null)
                    {
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncomingUK", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncomingUK", "LegalEntityIndexIdx");

                        //{ f: PnrIncomingUK.DocumentKindIdx}-{ f: PnrIncomingUK.DepartmentIdx}-{ f: PnrIncomingUK.LegalEntityIndexIdx}/{ 00n}
                        string newFullNumber = $"{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(editNumber.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                    }
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

            bool skipValidation = PnrCardFieldValidationHelper.IsPnrSkipCardFieldsCustomValidation(context);
            if (!skipValidation)
            {
                await ValidateContractAsync(context.ValidationResult, context, card);
            }
        }
    }
}