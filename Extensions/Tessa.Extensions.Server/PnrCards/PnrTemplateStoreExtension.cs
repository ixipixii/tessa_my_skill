using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrTemplateStoreExtension : CardStoreExtension
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

            // Редактирование номера(при редактировании карточки)
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
