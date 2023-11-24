using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Web.Models
{
    public abstract class PnrBaseRequest
    {
        public abstract Task<Guid?> GetCardIDAsync(IDbScope dbScope);

        public abstract string GetDescription();

        /// <summary>
        /// Получение ID типа карточки
        /// </summary>
        public abstract Task<Guid?> GetCardTypeID(IDbScope dbScope);

        public abstract Task FillCardDataAsync(IDbScope dbScope, Card card, IValidationResultBuilder validationResult);

        /// <summary>
        /// Постобработка после сохранения карточки. Например тут можно обновить связанные карточки.
        /// </summary>
        public virtual Task AfterCardStoreActionAsync(Logger logger, IDbScope dbScope, ICardRepository cardRepository, Card card, IValidationResultBuilder validationResult)
        {
            return Task.CompletedTask;
        }

        protected Guid? GetGuidFromString(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return null;
            }
            return Guid.Parse(uid);
        }

        /// <summary>
        /// Удаляет пробелы в строке, если они есть
        /// </summary>
        protected string GetTrimmedString(string value)
        {
            return value?.Trim();
        }

        protected DateTime GetUtcDate(DateTime date)
        {
            return new DateTime(date.Ticks, DateTimeKind.Utc);
        }

        protected DateTime? GetDateFromString(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            // будем пробовать считать несколько форматов
            string[] formats = { "dd.MM.yyyy", "dd/MM/yyyy", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-ddThh:mm:ss" };

            var parsedData = DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);

            return GetUtcDate(parsedData);
        }

        /// <summary>
        /// Возвращает "Да"/"Нет"
        /// </summary>
        protected string GetStringFromBool(bool value)
        {
            return value ? "Да" : "Нет";
        }

        /// <summary>
        /// Возвращает bool из строки (1 или 0)
        /// </summary>
        protected bool GetBoolFromString(string value)
        {
            return value == "1";
        }

        /// <summary>
        /// Вовзращает ID карточки по строковому значению MDM-ключа
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="mdmKey">Значение MDM-ключа, по которому выполняется поиск</param>
        /// <param name="mdmKeyTableName">Название таблицы в БД, в которой хранится значение MDM-ключа</param>
        /// <param name="mdmKeyColumnName">Название колонки MDM-ключа в таблице</param>
        /// <returns>ID карточки</returns>
        protected async Task<Guid?> TryGetCardIDFromMdmKeyAsync(IDbScope dbScope, string mdmKey, string mdmKeyTableName, string mdmKeyColumnName = "MDMKey")
        {
            Guid? result = null;

            if (mdmKey != null)
            {
                using (dbScope.Create())
                {
                    var db = dbScope.Db;

                    result = await db
                        .SetCommand($@"select top 1 ID
                                    from {mdmKeyTableName} with(nolock)
                                    where {mdmKeyColumnName} = @mdmKey",
                                    db.Parameter("@mdmKey", mdmKey))
                        .ExecuteAsync<Guid?>();
                }
            }

            return result;
        }

        public virtual PnrBaseResponse GetSuccessResult(Logger logger, string message, Card card, ISession session)
        {
            if (!string.IsNullOrEmpty(message))
            {
                logger.Info(message);
            }
            return new PnrBaseResponse(PnrBaseResponseStatusCode.Success, message);
        }

        public virtual PnrBaseResponse GetErrorResult(Logger logger, string message, string prefix = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                logger.Error(prefix + message);
            }
            return new PnrBaseResponse(PnrBaseResponseStatusCode.Error, message);
        }
    }
}
