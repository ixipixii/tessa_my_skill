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
    public sealed class PnrErrandStoreExtension : CardStoreExtension
    {
        private async Task ValidateErrandAsync(ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrErrands = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrErrands", out pnrErrands)
                    & !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }


            var validationResult = context.ValidationResult;

            var subject = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "Subject");
            if (string.IsNullOrEmpty(subject))
            {
                context.ValidationResult.AddError(this, $"\"Тема\" обязательно к заполнению");
            }

            var performers = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "Performers");
            if (performers.Count < 1)
            {
                context.ValidationResult.AddError(this, $"\"Исолнители\" обязательно к заполнению");
            }

            var periodExecution = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrErrands", "PeriodExecution");
            if (periodExecution == null)
            {
                context.ValidationResult.AddError(this, $"\"Срок исполнения\" обязательно к заполнению");
            }

            var controllers = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrErrandsControllers");
            if (controllers.Count < 1)
            {
                context.ValidationResult.AddError(this, $"\"Контролеры\" обязательно к заполнению");
            }

            // Редактирование номера(при редактировании карточки)
            if (card.StoreMode == CardStoreMode.Update && documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);
                if (editFullNumber != null)
                {
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, editFullNumber.ToString());
                } else if (editNumber != null)
                {
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, $"{(editNumber.ToString()).PadLeft(6, '0')}");
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
                await ValidateErrandAsync(context, card);
            }
        }
    }
}
