using System;
using System.Collections.Generic;
using System.Text;
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
    public sealed class PnrUKOrderStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null
                || !sections.TryGetValue("PnrOrderUK", out CardSection pnrOrderUK))
            {
                // нет секции "PnrOrderUK" - нет полей для валидации
                return;
            }

            if (card.StoreMode == CardStoreMode.Insert && await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrderUK", "OrganizationID") != null)
            {
                // для формирования номера обязательно поле Индекс ЮЛ в Организация
                if (pnrOrderUK.Fields["LegalEntityIndexIdx"] == null)
                {
                    string legalEntityIndexErrorMessage = $"В организации \"{pnrOrderUK.Fields["OrganizationName"]}\" "
                        + "не заполнен Индекс ЮЛ. Заполните Индекс ЮЛ в карточке указанной выше организации и сделайте ее повторный выбор в поле \"Организация ГК Пионер\".";
                    context.ValidationResult.AddWarning(this, legalEntityIndexErrorMessage);
                }
            }

            var validationResult = context.ValidationResult;

            var subject = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "Subject");
            PnrCardFieldValidationHelper.Validate(validationResult, subject, "Заголовок");

            var organization = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrderUK", "OrganizationID");
            PnrCardFieldValidationHelper.Validate(validationResult, organization, "Организация ");

            int? performers = card.Sections["Performers"].Rows.Count;
            if (performers < 1)
                validationResult.AddError($"\"Согласующие\" обязательно к заполнению.");

            var department = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrderUK", "DepartmentID");
            PnrCardFieldValidationHelper.Validate(validationResult, department, "Подразделение");

            var approver = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOrderUK", "ApproverID");
            PnrCardFieldValidationHelper.Validate(validationResult, approver, "Утверждающий ");

            var fileCategories = await ServerHelper.GetFileCategories(context.DbScope, card);

            if (!fileCategories.Contains(PnrFileCategories.PnrOrderID))
                context.ValidationResult.AddError($"К документу должны быть приложены следующие файлы: {PnrFileCategories.PnrOrderName}");
        }
    }
}
