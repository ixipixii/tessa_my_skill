using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrCardHelper
    {
        /// <summary>
        /// Загружает карточку. Если были ошибки, добавляет в validationResult
        /// </summary>
        /// <returns></returns>
        public static async Task<Card> LoadCardAsync(ICardRepository cardRepository, IValidationResultBuilder validationResult, Guid cardID)
        {
            var getResponse = await cardRepository.GetAsync(new CardGetRequest()
            {
                CardID = cardID
            });

            if (!getResponse.ValidationResult.IsSuccessful())
            {
                validationResult.AddError($"Не удалось загрузить карточку {cardID}:{Environment.NewLine}{getResponse.ValidationResult.Build().ToString()}");
                return null;
            }
            return getResponse.Card;
        }

        /// <summary>
        /// Получить тип карточки по GUID карточки.
        /// </summary>
        public static async Task<Guid?> GetCardTypeIDByCardID(IDbScope dbScope, Guid id)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                            @"SELECT TOP 1 [TypeID]
                            FROM [Instances] with(nolock)
                            WHERE ID = @ID",
                        db.Parameter("@ID", id))
                    .ExecuteAsync<Guid?>();
            }
        }
    }
}
