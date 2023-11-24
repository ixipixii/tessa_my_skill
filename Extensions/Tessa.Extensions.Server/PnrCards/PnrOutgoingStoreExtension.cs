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
    public sealed class PnrOutgoingStoreExtension : CardStoreExtension
    {
        /// <summary>
        /// Проверка заполненности полей Входящего
        /// </summary>
        private async Task ValidateContractAsync(ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrOutgoing = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrOutgoing", out pnrOutgoing)
                    && !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }

            IValidationResultBuilder validationResult = context.ValidationResult;

            Guid? documentKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "DocumentKindID");
            PnrCardFieldValidationHelper.Validate(validationResult, documentKindID, "Вид исходящего документа");
            string documentKindIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "DocumentKindIdx");
            //PnrCardFieldValidationHelper.Validate(validationResult, documentKindIdx, "Вид исходящего документа");

            string subject = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "Subject");
            PnrCardFieldValidationHelper.Validate(validationResult, subject, "Заголовок");

            Guid? departmentID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "DepartmentID");
            PnrCardFieldValidationHelper.Validate(validationResult, departmentID, "Подразделение");

            // Валидация полей, зависящих от Вида исходящего документа
            if (documentKindID != null && !String.IsNullOrEmpty(documentKindIdx))
            {
                switch (documentKindIdx)
                {
                    case "2-01":    // исходящее письмо
                        Guid? destinationID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "DestinationID");
                        PnrCardFieldValidationHelper.Validate(validationResult, destinationID, "Адресат (контрагент/организация)");

                        //var performers = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "Performers");
                        //if (performers.Count < 1)
                        //{
                        //    context.ValidationResult.AddError(this, $"\"Доп. согласующие\" обязательно к заполнению");
                        //}

                        Guid? signatoryID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "SignatoryID");
                        PnrCardFieldValidationHelper.Validate(validationResult, signatoryID, "Подписант");

                        break;

                    case "2-02":    // рекламации
                        Guid? complaintKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "ComplaintKindID");
                        PnrCardFieldValidationHelper.Validate(validationResult, complaintKindID, "Вид рекламации");

                        Guid? complaintFormatID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "ComplaintFormatID");
                        PnrCardFieldValidationHelper.Validate(validationResult, complaintFormatID, "Формат рекламации");

                        //string fullName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "FullName");
                        //PnrCardFieldValidationHelper.Validate(validationResult, fullName, "Ф.И.О.");

                        Guid? fullNameRefID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrOutgoing", "FullNameRefID");
                        PnrCardFieldValidationHelper.Validate(validationResult, fullNameRefID, "Ф.И.О.");

                        break;
                }
            }

            // для формирования номера обязательно поле Индекс ЮЛ в Организация
            var organizations = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrOutgoingOrganizations");
            if (organizations.Count < 1)
            {
                context.ValidationResult.AddError(this, $"\"Организация ГК Пионер\" обязательно к заполнению");
            }
            else if (organizations.Count == 1)
            {
                // для формирования номера обязательно поле Индекс ЮЛ в Организация
                string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "LegalEntityIndexIdx");
                PnrCardFieldValidationHelper.Validate(validationResult, legalEntityIndexIdx, $"В Организации ГК не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ");
            }

            // при редактировании карточки формируем ее номер (в случае успешной валидации)
            if (card.StoreMode == CardStoreMode.Update && context.ValidationResult.Count == 0)
            {
                if (pnrOutgoing != null)
                {
                    // в формировании номера: Вид входящего документа, Подразделение, Индекс ЮЛ
                    // если поля или часть полей присутствуют(изменялись) - переформируем номер
                    if (pnrOutgoing.RawFields.TryGetValue("DocumentKindIdx", out object dki) ||
                        pnrOutgoing.RawFields.TryGetValue("DepartmentIdx", out object depIdx) ||
                        pnrOutgoing.RawFields.TryGetValue("LegalEntityIndexIdx", out object legalIdx)
                        )
                    {
                        // DocumentKindIdx проверялось выше в процессе валидации, поэтому значение уже доступно в documentKindIdx
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "LegalEntityIndexIdx");
                        string oldFullNumber = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "FullNumber");
                        string prefixFullNumber = oldFullNumber.StartsWith("Пр-") ? "Пр-" : "";

                        int number = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        // номер выделен, сформируем полный номер
                        //{ f: PnrOutgoing.DocumentKindIdx}-{ f: PnrOutgoing.DepartmentIdx}-{ f: PnrOutgoing.LegalEntityIndexIdx}/{00n}
                        string fullNumber = $"{prefixFullNumber}{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(number.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, fullNumber);
                    }
                }
            }

            // Редактирование номера (может и при создании, и при редактировании карточки)
            if (documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if (editNumber != null || editFullNumber != null)
                {
                    // Шаблонная часть номера при редактировании номера не заполнена - формируем ее по старой схеме
                    if (editFullNumber == null)
                    {
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrOutgoing", "LegalEntityIndexIdx");
                        string fullNumber = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "FullNumber");
                        string prefixFullNumber = fullNumber.StartsWith("Пр-") ? "Пр-" : "";

                        //{ f: PnrOutgoing.DocumentKindIdx}-{ f: PnrOutgoing.DepartmentIdx}-{ f: PnrOutgoing.LegalEntityIndexIdx}/{00n}
                        string newFullNumber = $"{prefixFullNumber}{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(editNumber.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                    }
                    #if false // Закомментирован в связи с тем, что при редактировании номера вручную добавляются лишние символы в конец номера
                    if (editNumber == null)
                    {
                        // Порядковый номер при редактировании номера не заполнен - берем выделенный ранее
                        int number = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        string newFullNumber = $"{editFullNumber}/{(number.ToString()).PadLeft(3, '0')}";

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

            // флаг "пропустить валидацию", передается при интеграции, т.к. там приходят не все обязательные поля
            bool skipValidation = PnrCardFieldValidationHelper.IsPnrSkipCardFieldsCustomValidation(context);

            if (!skipValidation)
            {
                // валидация полей
                await ValidateContractAsync(context, card);
            }
        }
    }
}
