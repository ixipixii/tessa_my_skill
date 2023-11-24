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
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrRegulationStoreExtension : CardStoreExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrRegulationStoreExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null
                || !sections.TryGetValue("PnrRegulations", out CardSection pnrRegulations))
            {
                return;
            }

            if (card.StoreMode == CardStoreMode.Insert)
            {
                if (sections.TryGetValue("DocumentCommonInfo", out CardSection documentCommonInfo))
                {
                    Dictionary<string, object> dciFields = documentCommonInfo.RawFields;
                    bool hasAuthor = dciFields.TryGetValue("AuthorID", out object authorID);
                    // есть автор - подчитаем индекс его подразделения для включения в Номер
                    // (получилось дублирование с клиентом, там тоже обращение за данными подразделения, требуется рефакторинг)
                    if (hasAuthor)
                    {
                        dciFields.TryGetValue("AuthorName", out object authorName);
                        CardRequest request = new CardRequest
                        {
                            RequestType = Shared.PnrRequestTypes.GetUserDepartmentInfoRequestTypeID,
                            Info =
                            {
                                { "authorID", (Guid)authorID }
                            }
                        };

                        CardResponse response = await cardRepository.RequestAsync(request);
                        ValidationResult result = response.ValidationResult.Build();
                        if (result.IsSuccessful)
                        {
                            string index = response.Info.Get<string>("Index");

                            if(index == null || index == "")
                                context.ValidationResult.AddWarning(this, $"В подразделении автора \"{authorName}\" не заполнен Индекс, входящий в номер.");

                            // индекс подчитан - сформируем номер
                            bool hasNumber = dciFields.TryGetValue("Number", out object number);
                            if (hasNumber)
                            {
                                // номер выделен, сформируем полный номер
                                string fullNumber = $"4-01-{index ?? ""}-{(number.ToString()).PadLeft(4, '0')}";
                                documentCommonInfo.RawFields["FullNumber"] = fullNumber;
                            }
                        }
                    }
                }
            }
        }


        private async Task ValidateContractAsync(IValidationResultBuilder validationResult, ICardStoreExtensionContext context, Card card)
        {
            StringDictionaryStorage<CardSection> sections;
            CardSection pnrRegulations = null,
                documentCommonInfo = null;
            if (context.CardType == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrRegulations", out pnrRegulations)
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
                } else if (editNumber != null)
                {
                    string oldFullNumber = await PnrDataHelper.GetActualFieldValueAsync<string>(context.DbScope, card, "DocumentCommonInfo", "FullNumber");
                    string newFullNumber = oldFullNumber.Substring(0, oldFullNumber.Length - 4) + $"{(editNumber.ToString()).PadLeft(4, '0')}";
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
                await ValidateContractAsync(context.ValidationResult, context, card);
            }
        }
    }
}
