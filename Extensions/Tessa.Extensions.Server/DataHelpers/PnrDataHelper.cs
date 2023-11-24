using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.DataHelpers
{
    public static class PnrDataHelper
    {
        /// <summary>
        /// Приведение к типу
        /// </summary>
        public static T ConvertTo<T>(object x)
        {
            if (x == null) return default;

            if (x is IConvertible)
            {
                var type = typeof(T);
                var underlyingType = Nullable.GetUnderlyingType(type);
                return (T)Convert.ChangeType(x, underlyingType ?? type);
            }

            return (T)x;
        }

        /// <summary>
        /// Получение значения поля указаной таблички из БД по ID карточки.
        /// </summary>
        public static async Task<T> GetDataBaseFieldValueByIDAsync<T>(
            IDbScope dbScope,
            Guid id,
            string tableName,
            string fieldName,
            bool isTableSection = false)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;

                var value = await db.SetCommand(
                        $@"SELECT TOP 1 {fieldName}
                        FROM {tableName} WITH(NOLOCK)
                        WHERE {(isTableSection ? "[RowID]" : "[ID]")} = @ID",
                        db.Parameter("@ID", id))
                    .LogCommand()
                    .ExecuteAsync<T>();

                return ConvertTo<T>(value);
            }
        }

        public static async Task<Guid> CheckIncomingDataAsync(
            IDbScope dbScope,
            Guid CorrespondentID,
            DateTime ExternalDate,
            string ExternalNumber,
            Guid newCardID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;

                var value = await db.SetCommand(
                        $@"SELECT TOP 1 i.ID
                        FROM PnrIncoming i WITH(NOLOCK)
                        INNER JOIN DocumentCommonInfo d WITH(NOLOCK)
                        ON d.ID = i.ID
                        WHERE i.ID <> @newCardID AND
                        i.CorrespondentID = @CorrespondentID AND
                        i.ExternalNumber = @ExternalNumber AND
                        i.ExternalDate = @ExternalDate
                        ORDER BY d.CreationDate",
                        db.Parameter("@CorrespondentID", CorrespondentID),
                        db.Parameter("@ExternalNumber", ExternalNumber),
                        db.Parameter("@ExternalDate", ExternalDate),
                        db.Parameter("@newCardID", newCardID)
                        )
                    .LogCommand()
                    .ExecuteAsync<Guid>();

                return ConvertTo<Guid>(value);
            }
        }

        public static T ConvertFromDbVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // returns the default value for the type
            }
            return (T)obj;
        }

        public static async Task<IEnumerable<Dictionary<string, object>>> ExecReaderToListOfDictionaryAsync(DbManager db)
        {
            var result = new List<Dictionary<string, object>>();
            await using (var reader = await db.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var dict = new Dictionary<string, object>();
                    for (var lp = 0; lp < reader.FieldCount; lp++)
                    {
                        dict.Add(reader.GetName(lp), ConvertFromDbVal<object>(reader.GetValue(lp)));
                    }
                    result.Add(dict);
                }
            }

            return result;
        }

        /// <summary>
        /// Метод для получения действительного значения поля (значение берется из карточки или из базы)
        /// </summary>
        public static async Task<T> GetActualFieldValueAsync<T>(IDbScope dbScope, Card card, string sectionName, string fieldName)
        {
            if (string.IsNullOrEmpty(sectionName) ||
                string.IsNullOrEmpty(fieldName) ||
                card == null)
            {
                return default;
            }

            // Поле изменялось в контексте сохранения - берем значение из контекста
            if (card.Sections.TryGetValue(sectionName, out var section)
                && section.Fields.ContainsKey(fieldName))
            {
                var value = section.Fields[fieldName];
                return ConvertTo<T>(value);
            }

            // Иначе - берем значение из базы
            return await GetDataBaseFieldValueByIDAsync<T>(dbScope, card.ID, sectionName, fieldName);
        }

        /// <summary>
        /// Получить смерженную коллекцию
        /// </summary>
        public static async Task<List<Dictionary<string, object>>> GetActualCollectionValuesAsync(IDbScope dbScope, Card card, string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName) ||
                card == null)
            {
                return default;
            }
            var mergedRows = new List<Dictionary<string, object>>();

            card.Sections.TryGetValue(sectionName, out var section);
            section =
                section
                ?? new CardSection(sectionName, new Dictionary<string, object>()) { Type = CardSectionType.Table };

            var actualRows = section.Rows.Where(r => r.State != CardRowState.Deleted).ToList();
            // Получаем строки, которые отсутствуют в карточке
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var query = $@"SELECT * FROM {sectionName} WITH(NOLOCK) WHERE id = @cardID";

                db.SetCommand(query, db.Parameter("@cardID", card.ID));

                // получаем все записи из БД
                var rowsWithColumnsToMerge = (List<Dictionary<string, object>>)await ExecReaderToListOfDictionaryAsync(db);

                // убираем удаленные строки
                rowsWithColumnsToMerge.RemoveAll(
                    x => section.Rows.Where(r => r.State == CardRowState.Deleted)
                        .Any(r => x.TryGet<Guid>("RowID") == r.RowID));

                // мержим поля в строках с одним RowID
                foreach (var r in rowsWithColumnsToMerge)
                {
                    var source = actualRows.FirstOrDefault(x => x.RowID == r.TryGet<Guid>("RowID"));
                    if (source == null)
                    {
                        continue;
                    }
                    StorageHelper.Merge(source, r);
                }

                mergedRows.AddRange(rowsWithColumnsToMerge);
            }

            var list = actualRows.Select(s => (Dictionary<string, object>)s.GetStorage()).ToList();
            // Оставляем только новые строки
            list.RemoveAll(x => mergedRows.Any(c => c.TryGet<Guid>("RowID") == x.TryGet<Guid>("RowID")));
            mergedRows.AddRange(list);
            return mergedRows;
        }
    }
}
