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
    class PnrOrganizationStoreExtension : CardStoreExtension
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
        /// Проверка заполненности полей организации. Вынесено на сервер, т.к. поля могут быть пустыми при интеграции.
        /// </summary>
        private async Task ValidatePartnerAsync(IDbScope dbScope, Guid organizationID, IValidationResultBuilder validationResult)
        {
            var organization = await PnrOrganizationHelper.GetOrganizationByID(dbScope, organizationID);

            if (organization == null)
            {
                validationResult.AddError($"Не удалось загрузить данные организации. ID = {organizationID}");
                return;
            }

            // Индекс юридического лица
            PnrCardFieldValidationHelper.Validate(validationResult, organization.LegalEntityIndex, "Индекс юридического лица");

            // Адрес
            PnrCardFieldValidationHelper.Validate(validationResult, organization.Address, "Адрес");

            // Должность руководителя ЮЛ
            PnrCardFieldValidationHelper.Validate(validationResult, organization.PositionHeadLegalEntity, "Должность руководителя ЮЛ");

            // Главный бухгалтер для процессов
            PnrCardFieldValidationHelper.Validate(validationResult, organization.ChiefAccountantProcessID, "Главный бухгалтер для процессов");

            // Руководитель ЮЛ
            PnrCardFieldValidationHelper.Validate(validationResult, organization.HeadLegalEntityID, "Руководитель ЮЛ");

            // Банк
            //PnrCardFieldValidationHelper.Validate(validationResult, organization.Bank, "Банк");

            // ИНН
            //PnrCardFieldValidationHelper.Validate(validationResult, organization.INNBank, "ИНН");

            // БИК
            //PnrCardFieldValidationHelper.Validate(validationResult, organization.BIK, "БИК");

            // Расчетный счет
            //PnrCardFieldValidationHelper.Validate(validationResult, organization.SettlementAccount, "Расчетный счет");

            // Корр. счет №
            //PnrCardFieldValidationHelper.Validate(validationResult, organization.CorrespondentAccount, "Корр. счет №");
        }
    }
}
