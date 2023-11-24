using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrPowerAttorneyStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;

            if ((card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            var startDate = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrPowerAttorney", "StartDate");
            var endDate = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrPowerAttorney", "EndDate");

            if (startDate > endDate)
            {
                context.ValidationResult.AddError(this, "Дата начала доверенности превышает дату окончания");
            }
        }



        private async Task ValidatePowerAttorneyAsync(IValidationResultBuilder validationResult, ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrPowerAttorney = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrPowerAttorney", out pnrPowerAttorney)
                    & !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    & !sections.ContainsKey("PnrConfidants")
                    )
                )
            {
                return;
            }

            int? employeeID = await PnrDataHelper.GetActualFieldValueAsync<int?>(context.DbScope, card, "PnrPowerAttorney", "EmployeeID");
            if(employeeID != null)
            {
                switch (employeeID)
                {
                    case 0:
                        string confidantNotEmployee = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrPowerAttorney", "ConfidantNotEmployee");
                        PnrCardFieldValidationHelper.Validate(validationResult, confidantNotEmployee, "Доверенное лицо (не сотрудник компании)");
                        break;
                    case 1:
                        // здесь теперь коллекционное поле
                        var confidants = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrConfidants");
                        if (confidants.Count < 1)
                        {
                            context.ValidationResult.AddError(this, $"\"Доверенное лицо\" обязательно к заполнению");
                        }
                        break;
                }
            }

            // есть изменения в секции Доверенные лица
            if (sections.ContainsKey("PnrConfidants") && context.ValidationResult.Count == 0)
            {
                // удаление сохраненных ранее записей руководителей дирекций, относящихся к данной карточке
                await ServerHelper.DeleteConfidantsDepartmentsHeadsByCardID(context.DbScope, card.ID);

                // Подчитывание и запись в 'скрытое' поле руководителей дирекций доверенных лиц (для использования в процессе)
                List<Dictionary<string, object>> confidants = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrConfidants");

                await ServerHelper.SetConfidantsDepartmentsHeadsByCardID(context.DbScope, card.ID, confidants);
            }

            // Редактирование номера(при редактировании карточки)
            if (card.StoreMode == CardStoreMode.Update && documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if (editNumber != null || editFullNumber != null)
                {
                    // Шаблонная часть номера при редактировании номера не заполнена - формируем ее по старой схеме
                    if (editFullNumber == null)
                    {
                        string newFullNumber = $"{(editNumber.ToString()).PadLeft(6, '0')}";
                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                    }
                    #if false // Закомментирован в связи с тем, что при редактировании номера вручную добавляются лишние символы в конец номера
                    else if (editNumber == null)
                    {
                        // Порядковый номер при редактировании номера не заполнен - берем выделенный ранее
                        int number = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        string newFullNumber = $"{(editFullNumber.ToString()).PadLeft(6, '0')}";

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
            {
                return;
            }

            bool skipValidation = PnrCardFieldValidationHelper.IsPnrSkipCardFieldsCustomValidation(context);
            if (!skipValidation)
            {
                await ValidatePowerAttorneyAsync(context.ValidationResult, context, card);
            }
        }
    }
}
