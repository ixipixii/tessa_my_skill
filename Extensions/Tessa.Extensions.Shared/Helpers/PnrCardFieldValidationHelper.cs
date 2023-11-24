using System;
using System.Collections.Generic;
using System.Text;
using Tessa.Cards.Extensions;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Shared.Helpers
{
    public static class PnrCardFieldValidationHelper
    {
        private static bool IsNotEmpty(object value)
        {
            if (value == null)
                return false;

            if (value is string valueString)
            {
                return !string.IsNullOrWhiteSpace(valueString);
            }
            return true;
        }

        /// <summary>
        /// Проверяет поле на заполненность. Если оно пусто, добавляет в результат валидации ошибку вида "{fieldCaption} обязательно к заполнению.".
        /// </summary>
        public static void Validate(IValidationResultBuilder validationResult, object fieldValue, string fieldCaption)
        {
            if (!IsNotEmpty(fieldValue))
            {
                validationResult.AddError($"\"{fieldCaption}\" обязательно к заполнению.");
            }
        }

        /// <summary>
        /// Флаг "пропустить валидацию", например передается при интеграции, т.к. там приходят не все обязательные поля
        /// </summary>
        /// <param name="storeContext">Контекст расширения на сохранение карточки, где выполняется валидация</param>
        /// <returns></returns>
        public static bool IsPnrSkipCardFieldsCustomValidation(ICardStoreExtensionContext storeContext)
        {
            bool skipValidation = storeContext.Request.Info.ContainsKey(PnrInfoKeys.PnrSkipCardFieldsCustomValidation);
            if (!skipValidation)
            {
                // флаг также может быть прокинут из предыдущего запроса (в случае создания карточки и автозапуска процесса)
                var fdPrevRequestInfo = storeContext.Request.Info.TryGet<Dictionary<string, object>>("fd_prev_request_info", null);
                if (fdPrevRequestInfo != null)
                {
                    skipValidation = fdPrevRequestInfo.ContainsKey(PnrInfoKeys.PnrSkipCardFieldsCustomValidation);
                }
            }
            return skipValidation;
        }
    }
}
