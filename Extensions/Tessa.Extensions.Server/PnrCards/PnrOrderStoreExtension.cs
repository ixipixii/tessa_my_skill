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
    public sealed class PnrOrderStoreExtension : CardStoreExtension
    {
        private async Task ValidateOrderAsync(IValidationResultBuilder validationResult, ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection documentCommonInfo = null,
                pnrOrder = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrOrder", out pnrOrder) && !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo))
                )
            {
                return;
            }

            // если есть секция карточки("PnrOrder") или секция с номерами("DocumentCommonInfo") - валидируем все(не выбирая что конкретно поменялось при редактировании)
            Guid? documentKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrder", "DocumentKindID");
            PnrCardFieldValidationHelper.Validate(validationResult, documentKindID, "Вид документа");

            string subject = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "Subject");
            PnrCardFieldValidationHelper.Validate(validationResult, subject, "Заголовок");

            Guid? organizationID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrder", "OrganizationID");
            PnrCardFieldValidationHelper.Validate(validationResult, organizationID, "Организация ГК Пионер");
            string organizationLegalEntityIndex = "";
            if (organizationID != null)
            {
                string organizationName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOrder", "OrganizationName");
                organizationLegalEntityIndex = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOrder", "OrganizationLegalEntityIndex");
                PnrCardFieldValidationHelper.Validate(validationResult, organizationLegalEntityIndex, $"В организации {(String.IsNullOrEmpty(organizationName) ? "" : organizationName)} не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ");
            }

            Guid? projectID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrder", "ProjectID");
            PnrCardFieldValidationHelper.Validate(validationResult, projectID, "Проект");

            var performers = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "Performers");
            if (performers.Count < 1)
            {
                context.ValidationResult.AddError(this, $"\"Исполнители\" обязательно к заполнению");
            }

            // При первом сохранении проектный номер назначится за счет настроек в Типовом решении.
            // При редактирование карточки:
            // при наличии изменений в полях Number или FullNumber(вручную поменяли Номер), или/и 
            // при изменении Вида документа или/и Организации - формируем новый номер
            if (card.StoreMode == CardStoreMode.Update && validationResult.Count == 0 && 
                ((documentCommonInfo != null && (documentCommonInfo.RawFields.ContainsKey("Number") || documentCommonInfo.RawFields.ContainsKey("FullNumber"))) ||
                (pnrOrder != null && (pnrOrder.RawFields.ContainsKey("DocumentKindID") || pnrOrder.RawFields.ContainsKey("OrganizationID")))
                ))
            {
                object modifiedNumber = null,
                 modifiedFullNumber = null;
                if (documentCommonInfo != null)
                {
                    documentCommonInfo.RawFields.TryGetValue("Number", out modifiedNumber);             // Порядковый номер
                    documentCommonInfo.RawFields.TryGetValue("FullNumber", out modifiedFullNumber);     // Номер
                }
                string fullNumber = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "FullNumber");
                bool isProjectNumber = fullNumber != null ? fullNumber.StartsWith("Проект") : false;

                if (modifiedFullNumber != null)
                {
                    // в контроле "Номер" изменен Номер (FullNumber)
                    // FullNumber содержит полностью сформированный, ни от чего не зависящий номер - просто пишем его в базу
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, $"{modifiedFullNumber}");
                }
                else if (modifiedNumber != null && isProjectNumber)
                {
                    // в контроле "Номер" изменен Порядковый номер (Number), при этом у документа в данный момент Проектный номер
                    // формируем и пишем в базу простой номер("Проект-XXX")
                    await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, $"Проект-{modifiedNumber}");
                }
                else
                {
                    // все прочие случаи не обрабатываем отдельно, формируем и пишем в базу новый номер по шаблону {DocumentsGroupIndex}-{LegalEntityIndexIdx}-{000n}
                    await OrderRegistrationNumber.SetOrderRegistrationNumber(context.DbScope, card);
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
                // валидация полей карточки и сохранение номера в базе при изменении
                await ValidateOrderAsync(context.ValidationResult, context, card);
            }
        }
    }
}
