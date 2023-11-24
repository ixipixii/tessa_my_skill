using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;


namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public static class KrProcessSharedHelper
    {

        /// <summary>
        /// Указанная картчока является карточкой в которой, указывается (шаблоны).
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static bool DesignTimeCard(Guid typeID) =>
            typeID == DefaultCardTypes.KrStageTemplateTypeID
            || typeID == DefaultCardTypes.KrSecondaryProcessTypeID;

        /// <summary>
        /// Указанная карточка является карточкой, в которой маршрут выполняется
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static bool RuntimeCard(Guid typeID) => !DesignTimeCard(typeID);

        /// <summary>
        /// Получить ID типа документа или null явно из базы.
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="dbScope"></param>
        /// <returns></returns>
        public static Guid? GetDocTypeID(
            Guid cardID,
            IDbScope dbScope)
        {
            using (dbScope.Create())
            {
                var query = dbScope.BuilderFactory
                    .Select()
                    .C(DocumentCommonInfo.DocTypeID)
                    .From(DocumentCommonInfo.Name).NoLock()
                    .Where().C(ID).Equals().P("CardID")
                    .Build();
                return dbScope.Db.SetCommand(
                        query,
                        dbScope.Db.Parameter("CardID", cardID))
                    .LogCommand()
                    .Execute<Guid?>();
            }
        }

        /// <summary>
        /// Получить ID типа документа или null.
        /// Метод кэширует тип документа в Card.Info
        /// </summary>
        /// <param name="card"></param>
        /// <param name="dbScope"></param>
        /// <returns></returns>
        public static Guid? GetDocTypeID(
            Card card,
            IDbScope dbScope = null)
        {
            // Тип лежит в секции DocumentCommonInfo.DocTypeID
            if (card.Sections.TryGetValue(DocumentCommonInfo.Name, out var sec)
                && sec.Fields.TryGetValue(DocumentCommonInfo.DocTypeID, out object docTypeIDObj))
            {
                return docTypeIDObj as Guid?;
            }

            // Тип закэширован в Card.Info
            if (card.Info.TryGetValue(Keys.DocTypeID, out docTypeIDObj))
            {
                return docTypeIDObj as Guid?;
            }

            // В карточке ничего нет, придется лезть в базу
            Guid? docTypeFromDb = null;
            if(dbScope != null)
            {
                docTypeFromDb = GetDocTypeID(card.ID, dbScope);
                card.Info[Keys.DocTypeID] = docTypeFromDb;
            }

            return docTypeFromDb;
        }

        /// <summary>
        /// Получает запросом из базы состояние согласования для карточки
        /// </summary>
        /// <param name="dbScope">IDbScope</param>
        /// <param name="cardID">ИД основной карточки</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Состояние карточки указанное в сателлите (или Draft если сателлита нет)</returns>
        public static async Task<KrState?> GetKrStateAsync(
            Guid cardID,
            IDbScope dbScope,
            CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                //Получаем состояние карточки
                var db = dbScope.Db;
                var builderFactory = dbScope.BuilderFactory;
                var cardState = await db
                    .SetCommand(
                        builderFactory
                            .Select().Coalesce(b => b.C(StateID).V(0))
                            .From(KrApprovalCommonInfo.Name).NoLock()
                            .Where().C(KrProcessCommonInfo.MainCardID).Equals().P("CardID")
                            .Build(),
                        db.Parameter("CardID", cardID))
                    .LogCommand()
                    .ExecuteAsync<int?>(cancellationToken).ConfigureAwait(false);
                return cardState.HasValue
                    ? (KrState?)new KrState(cardState.Value)
                    : null;
            }
        }

        /// <summary>
        /// Получить состояние карточки из возможных источников:
        /// Секция KrApprovalCommonInfoVirtual
        /// Сателлит в Info
        /// БД (опционально)
        /// </summary>
        /// <param name="card"></param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns></returns>
        public static async ValueTask<KrState?> GetKrStateAsync(
            Card card,
            IDbScope dbScope = null,
            CancellationToken cancellationToken = default)
        {
            KrState? result = null;

            if (card.Sections.TryGetValue(KrApprovalCommonInfo.Virtual, out var section))
            {
                result = (KrState?)section.RawFields.TryGet<int?>(StateID);
            }

            if (!result.HasValue)
            {
                // возможно удалённая карточка
                Card satelliteCard = CardSatelliteHelper.TryGetSatelliteCard(card, KrSatelliteInfoKey);
                if (satelliteCard != null)
                {
                    result = satelliteCard.Sections[KrApprovalCommonInfo.Name].RawFields.Get<KrState?>(StateID);
                }
            }

            if (!result.HasValue
                && dbScope != null)
            {
                result = await GetKrStateAsync(card.ID, dbScope, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Возвращает эффективные настройки для типа карточки или типа документа <see cref="IKrType"/>
        /// по карточке <paramref name="card"/>, которая загружена со всеми секциями, или <c>null</c>, если настройки нельзя получить.
        /// </summary>
        /// <param name="krTypesCache">Кэш типов карточек.</param>
        /// <param name="card">Карточка, загруженная со всеми секциями.</param>
        /// <param name="cardTypeID">Идентификатор типа карточки.</param>
        /// <param name="validationResult">
        /// Объект, в который записываются сообщения об ошибках, или <c>null</c>, если сообщения никуда не записываются.
        /// </param>
        /// <param name="validationObject">
        /// Объект, информация о котором записывается в сообщениях об ошибках в <paramref name="validationResult"/>,
        /// или <c>null</c>, если информация об объекте не будет указана.
        /// </param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>
        /// Возвращает эффективные настройки для типа карточки или типа документа
        /// или <c>null</c>, если настройки нельзя получить.
        /// </returns>
        public static async ValueTask<IKrType> TryGetKrTypeAsync(
            IKrTypesCache krTypesCache,
            Card card,
            Guid cardTypeID,
            IValidationResultBuilder validationResult = null,
            object validationObject = null,
            CancellationToken cancellationToken = default)
        {
            KrCardType krCardType = (await krTypesCache.GetCardTypesAsync(cancellationToken).ConfigureAwait(false))
                .FirstOrDefault(x => x.ID == cardTypeID);

            if (krCardType == null)
            {
                // карточка может не входить в типовое решение, тогда возвращается null
                // при этом нельзя кидать ошибку в ValidationResult, иначе любое действие с такой карточкой будет неудачным
                return null;
            }

            IKrType result = krCardType;
            if (krCardType.UseDocTypes)
            {
                if (card.Sections.TryGetValue("DocumentCommonInfo", out CardSection section))
                {
                    if (section.RawFields.TryGetValue("DocTypeID", out object value))
                    {
                        if (value is Guid docTypeID)
                        {
                            result = (await krTypesCache.GetDocTypesAsync(cancellationToken).ConfigureAwait(false))
                                .FirstOrDefault(x => x.ID == docTypeID);

                            if (result == null)
                            {
                                if (validationResult != null)
                                {
                                    validationResult.AddError(validationObject, "$KrMessages_UnableToFindTypeWithID", docTypeID);
                                }

                                return null;
                            }
                        }
                        else
                        {
                            if (validationResult != null)
                            {
                                validationResult.AddError(validationObject, "$KrMessages_DocTypeNotSpecified");
                            }

                            return null;
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Восстановление порядка сортировки для списка строк настроек.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="orderField"></param>
        public static void RepairStorageRowsOrders(
            IList<object> rows,
            string orderField)
        {
            if (rows.Count == 0)
            {
                return;
            }

            if (rows.Count == 1)
            {
                // Если строка одна, то можно сэкономить
                var singleRow = (IDictionary<string, object>)rows[0];
                singleRow[orderField] = 0;
                return;
            }

            // Полученные из словаря ордеры кэшируются, чтобы не вычислять позицию в хэш-таблице несколько раз.
            // 0 - значение в кэше не заполнено. Признаком заполненности является значение старшего бита, равное 1.
            // Ордеры здесь и в словаре могут расходится. Только в этом массиве поддерживается отношение порядка
            // В словарях проводится восстановление последовательности ордеров 0..n-1
            var ordersCache = new int[rows.Count];

            // Выполняем сортировку вставками, т.к. в большинстве случаев считаем последовательность упорядоченной
            // В случае, когда последовательность упорядочена, сортировка выполняется за O(n).
            for (var i = 1; i < rows.Count; i++)
            {
                var iRow = (IDictionary<string, object>)rows[i];
                var iOrder = GetOrder(iRow, orderField, ordersCache, i);

                int j = i - 1;
                for (; j >= 0; j--)
                {
                    var jRow = (IDictionary<string, object>)rows[j];
                    int jOrder = GetOrder(jRow, orderField, ordersCache, j);
                    if (jOrder <= iOrder)
                    {
                        break;
                    }
                    // Сдвигаем на место перемещаемой назад.
                    rows[j + 1] = rows[j];
                    ordersCache[j + 1] = ordersCache[j];
                    // Фикс ордеров для сдвигаемых строк.
                    ((IDictionary<string, object>)rows[j + 1])[orderField] = j + 1;
                }

                // Элемент необходимо переместить назад.
                if (j + 1 != i)
                {
                    rows[j + 1] = iRow;
                    ordersCache[j + 1] = SetCachedMask(iOrder);
                }

                // Если ордер в строке несоответствует порядковому ордеру,
                // то в i-тую строку ставим j+1 ордер, на который i-тая строка перемещается.
                if (iOrder != j + 1)
                {
                    iRow[Order] = j + 1;
                }
            }
        }

        /// <summary>
        /// Определить порядок ручного этапа при вставке в маршрут.
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="groupOrder"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static int ComputeStageOrder(
            Guid groupID,
            int groupOrder,
            IReadOnlyList<CardRow> rows)
        {
            if (rows.Count == 0)
            {
                return 0;
            }

            var rowIndex = 0;

            Guid GetID() => rows[rowIndex].TryGet(StageGroupID, Guid.Empty);
            int GetOrder() => rows[rowIndex].TryGet(StageGroupOrder, int.MaxValue);
            bool NestedStage() => rows[rowIndex].TryGet<bool>(Keys.NestedStage);

            var cnt = rows.Count;

            // Достигаем начало требуемой группы
            while (rowIndex < cnt
                && (GetID() != groupID
                    && GetOrder() < groupOrder
                    || NestedStage()))
            {
                rowIndex++;
            }

            // На нестеде тут мы быть не можем, т.к. пропустили возможные нестеды выше
            // Проверим, что мы в конце или на другой группе
            if (rows.Count == rowIndex
                || (GetID() != groupID
                    && GetOrder() != groupOrder))
            {
                // Текущая группа последняя
                // В текущей группе нет этапов, просто добавляем на нужное место.
                return rowIndex;
            }

            var firstIndexInGroup = rowIndex;
            rowIndex++;

            // Спускаемся до конца группы
            while (rowIndex < cnt
                && (GetID() == groupID
                    && GetOrder() == groupOrder
                    || NestedStage()))
            {
                rowIndex++;
            }

            // Поднимаемся вверх до возможного места добавления
            var position = rowIndex;
            var sortedRows = rows.OrderBy(p => (int) p[Order]).ToArray();
            for (int i = rowIndex - 1; i >= firstIndexInGroup; i--)
            {
                var row = sortedRows[i];
                if (row.TryGet<bool>(Keys.NestedStage))
                {
                    continue;
                }

                if (row.Fields.TryGetValue(KrStages.BasedOnStageTemplateGroupPositionID, out var gpObj)
                    && GroupPosition.GetByID(gpObj) == GroupPosition.AtLast
                    && row.Fields.TryGetValue(KrStages.OrderChanged, out var orderChangedObj)
                    && orderChangedObj is bool orderChanged
                    && !orderChanged)
                {
                    position = i;
                }
            }

            return position;
        }

        #region private

        /// <summary>
        /// Маска, позволяющая установить в старшем бите Int32 признак того, что значение является инициализированным.
        /// </summary>
        private const int CachedMark = 0x40000000;

        /// <summary>
        /// Маска, позволяющая снять <see cref="CachedMark"/> для получения значения.
        /// </summary>
        private const int InvertCachedMark = 0x3FFFFFFF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SetCachedMask(
            int value) => CachedMark | value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int UnsetCachedMask(
            int value) => value & InvertCachedMark;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCached(
            int value) => (CachedMark & value) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetOrder(
            IDictionary<string, object> rowStorage,
            string orderField,
            int[] cache,
            int idx)
        {
            var cachedOrder = cache[idx];
            if (IsCached(cachedOrder))
            {
                return UnsetCachedMask(cachedOrder);
            }
            var order = rowStorage.TryGet<int?>(orderField) ?? int.MaxValue;
            cache[idx] = SetCachedMask(order);
            return order;
        }

        #endregion

    }
}