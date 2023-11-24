using System;
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
    public sealed class PnrUKContractStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;

            if ((card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            var startDate = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrContractsUK", "StartDate");
            var endDate = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrContractsUK", "EndDate");
            var validationResult = context.ValidationResult;

            if (startDate > endDate)
            {
                context.ValidationResult.AddError(this, "Дата начала договора превышает дату окончания");
            }

            var amount = await PnrDataHelper.GetActualFieldValueAsync<decimal?>(context.DbScope, card, "PnrContractsUK", "Amount");
            PnrCardFieldValidationHelper.Validate(validationResult, amount, "Сумма");
        }

        private async Task ValidateUKContractAsync(ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrContractsUK = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrContractsUK", out pnrContractsUK)
                    & !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }

            var validationResult = context.ValidationResult;

            // Редактирование номера (может и при создании, и при редактировании карточки)
            if (card.StoreMode == CardStoreMode.Update && documentCommonInfo != null && validationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if (editFullNumber != null)
                {
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, editFullNumber.ToString());
                } else if (editNumber != null)
                {
                    string newFullNumber = $"Д-{(editNumber.ToString()).PadLeft(6, '0')}";
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
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

            if (!skipValidation)
            {
                // валидация полей
                await ValidateUKContractAsync(context, card);
            }
        }
    }
}
