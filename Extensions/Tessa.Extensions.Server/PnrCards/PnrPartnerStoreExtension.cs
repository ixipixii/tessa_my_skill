using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    class PnrPartnerStoreExtension : CardStoreExtension
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
                await ValidatePartnerAsync(context.DbScope, card.ID, context.ValidationResult);
            }
        }

        /// <summary>
        /// Проверка заполненности полей КА. Вынесено на сервер, т.к. поля могут быть пустыми при интеграции.
        /// </summary>
        private async Task ValidatePartnerAsync(IDbScope dbScope, Guid partnerID, IValidationResultBuilder validationResult)
        {
            var partner = await PnrPartnerHelper.GetPartnerByID(dbScope, partnerID);

            if (partner== null)
            {
                validationResult.AddError($"Не удалось загрузить данные контрагента. ID = {partnerID}");
                return;
            }

            PnrCardFieldValidationHelper.Validate(validationResult, partner.Name, "Краткое наименование");
            PnrCardFieldValidationHelper.Validate(validationResult, partner.FullName, "Полное наименование");
            // PnrCardFieldValidationHelper.Validate(validationResult, partner.Validity, "Срок действия");
            PnrCardFieldValidationHelper.Validate(validationResult, partner.TypeID, "Тип контрагента");

            if (partner.TypeID == 2)
            {
                PnrCardFieldValidationHelper.Validate(validationResult, partner.IdentityDocument, "Документ, удостоверяющий личность");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.IdentityDocumentSeries, "Серия");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.IdentityDocumentNumber, "Номер");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.IdentityDocumentIssuedBy, "Кем выдан");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.IdentityDocumentIssueDate, "Когда выдан");

            }
            else if (partner.TypeID == 1 || partner.TypeID == 3)
            {
                PnrCardFieldValidationHelper.Validate(validationResult, partner.ContactAddress, "Адрес");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.INN, "ИНН");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.OGRN, "ОГРН");
                PnrCardFieldValidationHelper.Validate(validationResult, partner.OKVED, "ОКВЭД");

                if (partner.NonResident == false)
                {
                    PnrCardFieldValidationHelper.Validate(validationResult, partner.KPP, "КПП");
                }
            }
        }
    }
}
