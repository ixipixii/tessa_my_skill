using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrActStoreExtension : CardStoreExtension
    {
        /// <summary>
        /// Проверка заполненности полей акта.
        /// </summary>
        private async Task ValidateActAsync(IValidationResultBuilder validationResult, ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.ContainsKey("PnrActs")
                    && !sections.ContainsKey("DocumentCommonInfo")
                    )
                )
            {
                return;
            }

            Guid? typeID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrActs", "TypeID");
            PnrCardFieldValidationHelper.Validate(validationResult, typeID, "Тип акта");

            Guid? projectID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrActs", "ProjectID");
            PnrCardFieldValidationHelper.Validate(validationResult, projectID, "Проект");

            Guid? organizationID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrActs", "OrganizationID");
            PnrCardFieldValidationHelper.Validate(validationResult, organizationID, "Организация ГК Пионер");

            string subject = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "Subject");
            PnrCardFieldValidationHelper.Validate(validationResult, subject, "Наименование");

            if (typeID != null && typeID == PnrActTypes.PnrAcceptanceCertificateTypeID)
            {
                Guid? implementationStageID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrActs", "ImplementationStageID");
                PnrCardFieldValidationHelper.Validate(validationResult, implementationStageID, "Стадия реализации");
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
                await ValidateActAsync(context.ValidationResult, context, card);
            }
        }
    }
}
