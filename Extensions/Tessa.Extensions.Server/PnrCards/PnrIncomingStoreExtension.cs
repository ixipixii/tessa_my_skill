using DocumentFormat.OpenXml.Office.Word;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Server.Helpers;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrIncomingStoreExtension : CardStoreExtension
    {

        private async Task CheckSimilarIncomingCard(ICardStoreExtensionContext context, Card card)
        {
            if (context.ValidationResult.IsSuccessful())
            {
                    /// проверим есть ли входящие с аналогичные данными?
                Guid? kindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming", "DocumentKindID");
                if (kindID.Equals(PnrIncomingTypes.IncomingLetterID))
                {
                    Guid? CorrespondentID =
                        await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming",
                            "CorrespondentID");
                    string ExternalNumber =
                        await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming",
                            "ExternalNumber");
                    DateTime? ExternalDate =
                        await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrIncoming",
                            "ExternalDate");
                    if (CorrespondentID != null && !string.IsNullOrWhiteSpace(ExternalNumber) && ExternalDate != null)
                    {
                        Guid similarIncominGuid = await PnrDataHelper.CheckIncomingDataAsync(
                            context.DbScope,
                            (Guid) CorrespondentID,
                            (DateTime) ExternalDate,
                            ExternalNumber,
                            card.ID
                        );
                        if (similarIncominGuid == null || similarIncominGuid.Equals(Guid.Empty))
                        {
                            return;
                        }

                        context.Response.Info.Add("similarIncominGuid", similarIncominGuid);
                    }
                }
            }
        }

        /// <summary>
        /// Проверка заполненности полей Входящего
        /// </summary>
        private async Task ValidateContractAsync(ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrIncoming = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrIncoming", out pnrIncoming)
                    && !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    && !sections.TryGetValue("PnrIncomingOrganizations", out CardSection pnrIncomingOrganizations)
                    )
                )
            {
                return;
            }

            IValidationResultBuilder validationResult = context.ValidationResult;

            string documentKindIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "DocumentKindIdx");
            PnrCardFieldValidationHelper.Validate(validationResult, documentKindIdx, "Вид входящего документа");

            string subject = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "Subject");
            PnrCardFieldValidationHelper.Validate(validationResult, subject, "Заголовок");

            DateTime? registrationDate = await PnrDataHelper.GetActualFieldValueAsync<DateTime?>(context.DbScope, card, "PnrIncoming", "RegistrationDate");
            PnrCardFieldValidationHelper.Validate(validationResult, registrationDate, "Дата регистрации");

            var organizations = await PnrDataHelper.GetActualCollectionValuesAsync(context.DbScope, card, "PnrIncomingOrganizations");
            if (organizations.Count < 1)
            {
                context.ValidationResult.AddError(this, $"\"Организация ГК Пионер\" обязательно к заполнению");
            } else if (organizations.Count == 1)
            {
                // для формирования номера обязательно поле Индекс ЮЛ в Организация
                string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "LegalEntityIndexIdx");
                PnrCardFieldValidationHelper.Validate(validationResult, legalEntityIndexIdx, $"В Организации ГК не задан Индекс ЮЛ - укажите его в карточке организации. \"Индекс ЮЛ");
            }

            // Валидация полей, зависящих от Вида входящего документа
            if (!String.IsNullOrEmpty(documentKindIdx))
            {
                switch (documentKindIdx)
                {
                    case "1-01":    // входящие
                        Guid? correspondentID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming", "CorrespondentID");
                        PnrCardFieldValidationHelper.Validate(validationResult, correspondentID, "Корреспондент");

                        Guid? departmentID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming", "DepartmentID");
                        PnrCardFieldValidationHelper.Validate(validationResult, departmentID, "Подразделение");
                        break;
                    case "1-02":    // рекламации
                        Guid? complaintKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming", "ComplaintKindID");
                        PnrCardFieldValidationHelper.Validate(validationResult, complaintKindID, "Вид рекламации");

                        Guid? complaintFormatID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming", "ComplaintFormatID");
                        PnrCardFieldValidationHelper.Validate(validationResult, complaintFormatID, "Формат рекламации");

                        //string fullName = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "FullName");
                        //PnrCardFieldValidationHelper.Validate(validationResult, fullName, "Ф.И.О.");

                        Guid? fullNameRef = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(context.DbScope, card, "PnrIncoming", "FullNameRefID");
                        PnrCardFieldValidationHelper.Validate(validationResult, fullNameRef, "Ф.И.О.");
                        break;
                }
            }

            // при редактировании карточки формируем ее номер (в случае успешной валидации)
            if (card.StoreMode == CardStoreMode.Update && context.ValidationResult.Count == 0)
            {
                if (pnrIncoming != null)
                {
                    // в формировании номера: Вид входящего документа, Подразделение, Индекс ЮЛ
                    // если поля или часть полей присутствуют(изменялись) - переформируем номер
                    if (pnrIncoming.RawFields.TryGetValue("DocumentKindIdx", out object dki) ||
                        pnrIncoming.RawFields.TryGetValue("DepartmentIdx", out object depIdx) ||
                        pnrIncoming.RawFields.TryGetValue("LegalEntityIndexIdx", out object legalIdx)
                        )
                    {
                        // DocumentKindIdx проверялось выше в процессе валидации, поэтому значение уже доступно в documentKindIdx
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "LegalEntityIndexIdx");

                        int number = await PnrDataHelper.GetActualFieldValueAsync<int>(context.DbScope, card, "DocumentCommonInfo", "Number");
                        // номер выделен, сформируем полный номер
                        //{ f: PnrIncoming.DocumentKindIdx}-{ f: PnrIncoming.DepartmentIdx}-{ f: PnrIncoming.LegalEntityIndexIdx}/{00n}
                        string fullNumber = $"{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(number.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, fullNumber);
                    }
                }
            }

            // Редактирование номера (может и при создании, и при редактировании карточки)
            if(documentCommonInfo != null && context.ValidationResult.Count == 0)
            {
                documentCommonInfo.RawFields.TryGetValue("Number", out object editNumber);
                documentCommonInfo.RawFields.TryGetValue("FullNumber", out object editFullNumber);

                if(editNumber != null || editFullNumber != null)
                {
                    // Шаблонная часть номера при редактировании номера не заполнена - формируем ее по старой схеме
                    if (editFullNumber == null)
                    {
                        string departmentIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "DepartmentIdx");
                        string legalEntityIndexIdx = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "PnrIncoming", "LegalEntityIndexIdx");

                        //{ f: PnrIncoming.DocumentKindIdx}-{ f: PnrIncoming.DepartmentIdx}-{ f: PnrIncoming.LegalEntityIndexIdx}/{00n}
                        string newFullNumber = $"{documentKindIdx}-{departmentIdx}-{legalEntityIndexIdx}/{(editNumber.ToString()).PadLeft(3, '0')}";

                        await ServerHelper.UpdateDocumentNumber(context.DbScope, card.ID, newFullNumber);
                    }
                    #if false // Закомментирован в связи с тем, что при редактировании номера вручную добавляются лишние символы в конец номера
                    if(editNumber == null)
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

        public override async Task AfterRequest(ICardStoreExtensionContext context)
        {
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }
            
            await CheckSimilarIncomingCard(context, card);
        }
    }
}
