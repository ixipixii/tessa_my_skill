using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrServiceNoteStoreExtension : CardStoreExtension
    {
        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            StringDictionaryStorage<CardSection> sections;
            Card card;
            CardSection pnrServiceNote,
                documentCommonInfo = null;
            if (!context.ValidationResult.IsSuccessful()
                || (card = context.Request.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null
                || (!sections.TryGetValue("PnrServiceNote", out pnrServiceNote)
                    && !sections.TryGetValue("DocumentCommonInfo", out documentCommonInfo)
                    )
                )
            {
                return;
            }

            var SNtype = await PnrDataHelper.GetActualFieldValueAsync<Guid?>
                (
                    context.DbScope,
                    card,
                    "PnrServiceNote",
                    "ServiceNoteTypeID"
                );

            if (SNtype == PnrServiceNoteTypes.ConclusionContracts || SNtype == PnrServiceNoteTypes.FinancialActivities)
            {
                var SNThemeID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>
                    (
                        context.DbScope,
                        card,
                        "PnrServiceNote",
                        "ServiceNoteThemeID"
                    );
                if (SNThemeID == null)
                {
                    context.ValidationResult.AddError(this, $"\"Тематика СЗ\" обязательна к заполнению");
                }
            }

            // флаг "пропустить валидацию", передается при интеграции, т.к. там приходят не все обязательные поля
            //bool skipValidation = context.Request.Info.ContainsKey(PnrInfoKeys.PnrSkipCardFieldsCustomValidation);
            //if (!skipValidation)
            //{
            //    // валидация полей
            //    await ValidateContractAsync(context.ValidationResult, context, card);
            //}


        }
    }
}
