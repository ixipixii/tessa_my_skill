using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public struct KrProcessSectionMapper
    {
        private sealed class StagePosition
        {
            // ao - скрытый
            // no - новый
            // ao so - измененный, но не передвинутый
            // ao so no - измененный и передвинутый
            // co - (no ?? so ?? minValue)

            public StagePosition(
                KrStagePositionInfo posInfo,
                CardRow row)
            {
                this.RowID = posInfo?.RowID ?? row.RowID;
                this.GroupOrder = posInfo?.GroupOrder ?? (int)row[KrConstants.StageGroupOrder];
                this.AbsoluteOrder = posInfo?.AbsoluteOrder;
                this.ShiftedOrder = posInfo?.ShiftedOrder;
                this.NewOrder = row?.TryGet<int?>(KrConstants.Order);
                this.CombinedOrder = this.NewOrder ?? this.ShiftedOrder ?? int.MinValue;
                this.IsHidden = this.CombinedOrder == int.MinValue;
                this.IsNew = this.NewOrder.HasValue && !this.AbsoluteOrder.HasValue;
                this.GroupPos = posInfo?.GroupPosition;
                this.ResultOrder = -1;
                this.Row = row;
                this.HiddenRow = posInfo?.CardRow;
            }

            /// <summary>
            /// Идентификатор строки
            /// </summary>
            public readonly Guid RowID;

            /// <summary>
            /// go
            /// </summary>
            public readonly int GroupOrder;

            /// <summary>
            /// ao
            /// </summary>
            public readonly int? AbsoluteOrder;

            /// <summary>
            /// so
            /// </summary>
            public readonly int? ShiftedOrder;

            /// <summary>
            /// no
            /// </summary>
            public readonly int? NewOrder;

            /// <summary>
            /// co
            /// </summary>
            public readonly int CombinedOrder;

            /// <summary>
            /// Этап является скрытым
            /// </summary>
            public readonly bool IsHidden;

            /// <summary>
            /// Этап добавлен пользователем
            /// </summary>
            public readonly bool IsNew;

            /// <summary>
            /// Позиция в группе.
            /// </summary>
            public readonly int? GroupPos;

            /// <summary>
            /// Строка из основной карточки
            /// </summary>
            public readonly CardRow Row;

            /// <summary>
            /// Скрытая строка из карточки.
            /// Присутствует, если с клиента приходит скрытая строка на добавление.
            /// </summary>
            public readonly CardRow HiddenRow;

            /// <summary>
            /// Результирующий порядок
            /// </summary>
            public readonly int ResultOrder;

            /// <inheritdoc />
            public override string ToString()
            {
                return (this.Row?.TryGet<string>(KrConstants.Name) ?? this.HiddenRow?.TryGet<string>(KrConstants.Name) ?? string.Empty)
                    + $", go={this.GroupOrder}" 
                    + $", ao={this.AbsoluteOrder}" 
                    + $", so={this.ShiftedOrder}"
                    + $", no={this.NewOrder}" 
                    + $", co={this.CombinedOrder}"
                    + $", GroupPos={this.GroupPos?.ToString() ?? "null"}"
                    + $", ro={this.ResultOrder}";
            }

        }

        private sealed class GroupOrderComparer : IComparer<StagePosition>
        {
            /// <inheritdoc />
            public int Compare(
                StagePosition x,
                StagePosition y) => x.GroupOrder.CompareTo(y.GroupOrder);
        }

        private sealed class CombinedOrderComparer : IComparer<StagePosition>
        {
            /// <inheritdoc />
            public int Compare(
                StagePosition x,
                StagePosition y) =>
                x.CombinedOrder.CompareTo(y.CombinedOrder);
        }


        private static readonly string[] serviceFields =
        {
            KrConstants.KrProcessCommonInfo.CurrentApprovalStageRowID,
            KrConstants.KrApprovalCommonInfo.StateChangedDateTimeUTC,
            KrConstants.StateID,
            KrConstants.StateName,
            KrConstants.KrApprovalCommonInfo.DisapprovedBy,
            KrConstants.KrApprovalCommonInfo.ApprovedBy,
            KrConstants.KrProcessCommonInfo.MainCardID,
        };

        private readonly Card source;
        private readonly Card dest;
        private readonly bool defaultAddDestinationSection;

        private static readonly GroupOrderComparer groupOrderComparer = new GroupOrderComparer();
        private static readonly CombinedOrderComparer combinedOrderComparer = new CombinedOrderComparer();

        public KrProcessSectionMapper(
            Card source,
            Card dest,
            bool addDestinationSection = false)
        {
            this.source = source;
            this.dest = dest;
            this.defaultAddDestinationSection = addDestinationSection;
        }

        public KrProcessSectionMapper Map(
            string sourceSectionAlias,
            string destSectionAlias,
            Action<CardSection, IDictionary<string, object>> modifyAction = null,
            bool? addDestinationSection = null)
        {
            if (!this.source.Sections.TryGetValue(sourceSectionAlias, out var sourceSection))
            {
                return this;
            }

            CardSection destSection = null;
            if (addDestinationSection ?? this.defaultAddDestinationSection)
            {
                destSection = this.dest.Sections.GetOrAdd(destSectionAlias);
            }

            if (destSection != null 
                || this.dest.Sections.TryGetValue(destSectionAlias, out destSection))
            {
                StorageHelper.Merge(sourceSection.GetStorage(), destSection.GetStorage());

                if (destSection.Type == CardSectionType.Entry)
                {
                    modifyAction?.Invoke(destSection, destSection.RawFields);
                }
                else if (destSection.Type == CardSectionType.Table)
                {
                    foreach (var row in destSection.Rows)
                    {
                        modifyAction?.Invoke(destSection, row.GetStorage());
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Выполнить перенос несерализуемых данных из виртуальной секции KrApprovalCommonInfoVirtual в KrApprovalCommonInfo
        /// </summary>
        public KrProcessSectionMapper MapApprovalCommonInfo()
        {
            if (!this.source.TryGetKrApprovalCommonInfoSection(out var aci))
            {
                return this;
            }

            var satelliteAci = this.dest.GetApprovalInfoSection();
            foreach (var field in aci.RawFields)
            {
                var skip = false;
                for (int i = 0; i < serviceFields.Length; i++)
                {
                    if (field.Key == serviceFields[i])
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }

                satelliteAci.Fields[field.Key] = field.Value;
            }
            return this;
        }

        /// <summary>
        /// Выполнить перенос несериализуемых данных из виртуальной секции KrStagesVirtual в физическую KrStages.
        /// </summary>
        public KrProcessSectionMapper MapKrStages()
        {
            if (!this.source.TryGetStagesSection(out var sourceSec, true)
                || !this.dest.TryGetStagesSection(out var destSec))
            {
                return this;
            }

            if (HasOrderChanges(sourceSec)
                && this.source.TryGetStagePositions(out var stagePositions)
                && stagePositions.Count > 0)
            {
                MapKrStagesWithHidden(sourceSec, destSec, stagePositions);
            }
            else
            {
                MapKrStagesSimple(sourceSec, destSec);
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasOrderChanges(
            CardSection sec) => sec.Rows.Any(p => p.ContainsKey(KrConstants.Order));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MapKrStagesWithHidden(
            CardSection sourceSec,
            CardSection destSec,
            List<KrStagePositionInfo> stagePositionsInfo)
        {
            var stagePositions = TransformToPositionsArray(sourceSec, stagePositionsInfo, out var size);
            if (stagePositions.Length == 0)
            {
                return;
            }

            SortStagePositions(stagePositions, size);
            
            var rows = destSec.Rows;
            var order = 0;
            for (var i = 0; i < size; i++)
            {
                var pos = stagePositions[i];
                var srcRow = pos.Row;
                if (srcRow is null)
                {
                    if (pos.HiddenRow != null)
                    {
                        // Новый скрытый этап с клиента
                        var row = rows.Add();
                        InsertInternal(pos.HiddenRow, row);
                        row.Fields[KrConstants.Order] = order;
                        row.Fields[KrConstants.KrStages.Hidden] = BooleanBoxes.True;
                    }
                    else
                    {
                        // Скрытый этап, в нем нужно только ордер пофиксать
                        var destRow = rows.FirstOrDefault(p => p.RowID == pos.RowID);
                        if (destRow != null)
                        {
                            destRow.Fields[KrConstants.Order] = order;
                        }
                    }
                }
                else
                {
                    switch (srcRow.State)
                    {
                        case CardRowState.Modified:
                            var destRow = rows.FirstOrDefault(p => p.RowID == pos.RowID);
                            if (destRow != null)
                            {
                                ModifyInternal(srcRow, destRow);
                                if (destRow.TryGetValue(KrConstants.Order, out var oldOrder)
                                    && !order.Equals(oldOrder))
                                {
                                    destRow.Fields[KrConstants.Order] = order;
                                }
                            }
                            break;
                        case CardRowState.Inserted:
                            var row = rows.Add();
                            InsertInternal(srcRow, row);
                            row.Fields[KrConstants.Order] = order;
                            row.Fields[KrConstants.KrStages.Hidden] = BooleanBoxes.False;
                            
                            break;
                        case CardRowState.Deleted:
                            var rowToDelete = rows.FirstOrDefault(p => p.RowID == srcRow.RowID);
                            var isDeleted = false;
                            if (rowToDelete == null)
                            {
                                isDeleted = true;
                            }
                            else
                            {
                                isDeleted = DeleteInternal(srcRow, rowToDelete);
                            }

                            if (isDeleted)
                            {
                                // Удаление не меняет порядок
                                order--;
                            }
                            break;
                    }
                }

                order++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SortStagePositions(
            StagePosition[] stagePositions,
            int size)
        {
            // Сортируем по go весь массив
            Array.Sort(stagePositions, 0, size, groupOrderComparer);

            // Выходит n неупорядоченных подмножеств, которые между собой упорядочены по go
            var currStage = stagePositions[0];
            var currGroupID = currStage.GroupOrder;
            var groupFirstIdx = 0;
            var hiddenCount = currStage.IsHidden ? 1 : 0;
            for (var currIdx = 1; currIdx < size; currIdx++)
            {
                currStage = stagePositions[currIdx];
                if (currStage.GroupOrder != currGroupID)
                {
                    MapGroupStages(stagePositions, groupFirstIdx, currIdx, hiddenCount);
                    hiddenCount = 0;
                    groupFirstIdx = currIdx;
                    currGroupID = currStage.GroupOrder;
                }
                if (currStage.IsHidden)
                {
                    hiddenCount++;
                }
            }
            // Дообработочка последней группы
            MapGroupStages(stagePositions, groupFirstIdx, size, hiddenCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MapGroupStages(
            StagePosition[] stagePositions,
            int groupFirstIdx,
            int currIdx,
            int hiddenCount)
        {
            if (currIdx - groupFirstIdx <= 1)
            {
                return;
            }

            // Берем каждое из таких множеств и дополнительно сортируем по (no ?? so), скрытые этапы 
            Array.Sort(stagePositions, groupFirstIdx, currIdx - groupFirstIdx, combinedOrderComparer);

            if (hiddenCount == 0)
            {
                // Нет скрытых этапов в группе, а значит в ней все замечательно.
                return;
            }

            // Теперь необходимо разобраться, где должны быть во всем этом скрытые этапы с помощью ao
            // Все скрытые этапы, которые содержат только ao, будут в начале т.к. co == int.MinValue
            var firstFixedIndex = groupFirstIdx + hiddenCount;
            for (var i = firstFixedIndex - 1; i >= groupFirstIdx; i--, firstFixedIndex--)
            {
                // Частичная сортировка (модификация selection sort) для скрытых элементов
                var hiddenStage = stagePositions[i];
                var newIndex = WindowSearch(stagePositions, hiddenStage, i, currIdx - 1);
                if (newIndex != i)
                {
                    MoveStageToTheRight(stagePositions, i, newIndex);
                }
                // newIndex == firstFixedIndex оставляет скрытый этап в начале.
            }
            // CombinedOrder + Offset = new absolute order

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WindowSearch(
            StagePosition[] array,
            StagePosition item,
            int from,
            int to)
        {
            // from - индекс текущего скрытого этапа item
            // начинаем
            var i = from + 1;
            // Находим первый не новый (с ао != null)
            for (; i <= to && array[i].IsNew; i++)
            {
            }

            // Все видимые этапы новые 
            if (i > to)
            {
                return item.GroupPos == GroupPosition.AtFirst.ID
                    ? from
                    : to;
            }
            var prev = array[i];

            if (item.AbsoluteOrder < prev.AbsoluteOrder)
            {
                // Место перед prev
                return item.GroupPos == GroupPosition.AtFirst.ID
                    ? from // в самом начале, т.е. оставляем на месте
                    : i - 1; // Внизу, где находятся "в конце группы".
                             // Берем i - 1, т.к. на i-той позиции уже с большим ao, т.е. "в конце группы"
            }

            var newCount = 0;
            for (; i <= to; i++)
            {
                var curr = array[i];
                if (curr.IsNew)
                {
                    newCount++;
                    continue;
                }

                if (prev.AbsoluteOrder < item.AbsoluteOrder
                    && item.AbsoluteOrder < curr.AbsoluteOrder)
                {
                    break;
                }

                prev = curr;
            }
            
            return item.GroupPos == GroupPosition.AtFirst.ID 
                ? i - newCount - 1
                : i - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MoveStageToTheRight(
            StagePosition[] array,
            int startIndex,
            int destIndex)
        {
            var tmp = array[startIndex];
            // Согласно документации операция безопасна даже при наложении областей.
            Array.Copy(array, startIndex + 1, array, startIndex, destIndex - startIndex);
            array[destIndex] = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static StagePosition[] TransformToPositionsArray(
            CardSection sourceSec,
            List<KrStagePositionInfo> stagePositionsInfo,
            out int size)
        {
            var rows = sourceSec.Rows;
            size = 0;
            // Такой размер позволит точно превысить возможное количество этапов
            var stagePositions = new StagePosition[rows.Count + stagePositionsInfo.Count];

            var modifiedRows = rows
                .OrderBy(p => p.RowID)
                .GetEnumerator();
            var oldRows = stagePositionsInfo
                .OrderBy(p => p.RowID)
                .GetEnumerator();
            
            try
            {
                var hasModifiedRows = modifiedRows.MoveNext();
                var hasOldRows = oldRows.MoveNext();
                while (true)
                {
                    if (!hasModifiedRows)
                    {
                        if (!hasOldRows)
                        {
                            // Ничего не осталось
                            break;
                        }

                        // Осталось только старенькое
                        do
                        {
                            var tailOldRow = oldRows.Current;
                            stagePositions[size++] = new StagePosition(tailOldRow, null);
                        } while (oldRows.MoveNext());
                        break;
                    }

                    if (!hasOldRows)
                    {
                        // Остались новенькие
                        do
                        {
                            var tailModifiedRow = modifiedRows.Current;
                            stagePositions[size++] = new StagePosition(null, tailModifiedRow);
                        } while (modifiedRows.MoveNext());
                        break;
                    }

                    var modifiedRow = modifiedRows.Current;
                    var oldRow = oldRows.Current;
                    var comparisonResult = modifiedRow.RowID.CompareTo(oldRow.RowID);
                    switch (comparisonResult)
                    {
                        case 1:
                            // modifiedRow > oldRow
                            // Для старой строки нет модифицированной
                            stagePositions[size++] = new StagePosition(oldRow, null);
                            hasOldRows = oldRows.MoveNext();
                            break;
                        case 0:
                            // Строка на вставку есть и в списке старых этапов,
                            // И присутствует в измененных
                            // Возможна ситуация, когда modifiedRow.State == CardRowState.Inserted
                            // Это специфичный кейс, когда новый этап вставляется, 
                            // но о нем есть информация как о старом.
                            // Это создание копии или по шаблону.
                            stagePositions[size++] = new StagePosition(oldRow, modifiedRow);
                            hasModifiedRows = modifiedRows.MoveNext();
                            hasOldRows = oldRows.MoveNext();
                            break;
                        case -1:
                            // modifiedRow < oldRow
                            // Для модифицированной строки нет старой - это явная вставка новой
                            stagePositions[size++] = new StagePosition(null, modifiedRow);
                            hasModifiedRows = modifiedRows.MoveNext();
                            break;
                    }
                }
            }
            finally
            {
                modifiedRows.Dispose();
                oldRows.Dispose();
            }

            return stagePositions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MapKrStagesSimple(CardSection sourceSec, CardSection destSec)
        {
            var rows = destSec.Rows;
            foreach (var stSecRow in sourceSec.Rows)
            {
                switch (stSecRow.State)
                {
                    case CardRowState.Modified:
                        {
                            var row = rows.FirstOrDefault(p => p.RowID == stSecRow.RowID);

                            if (row != null)
                            {
                                ModifyInternal(stSecRow, row);
                                if (stSecRow.TryGetValue(KrConstants.Order, out var order))
                                {
                                    row.Fields[KrConstants.Order] = order;
                                }
                            }

                            break;
                        }
                    case CardRowState.Inserted:
                        {
                            var row = rows.Add();
                            InsertInternal(stSecRow, row);
                            if (stSecRow.TryGetValue(KrConstants.Order, out var order))
                            {
                                row.Fields[KrConstants.Order] = order;
                            }
                            break;
                        }
                    case CardRowState.Deleted:
                        var rowToDelete = rows.FirstOrDefault(p => p.RowID == stSecRow.RowID);
                        if (rowToDelete != null)
                        {
                            DeleteInternal(stSecRow, rowToDelete);
                        }
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertInternal(
            CardRow sourceRow,
            CardRow destRow)
        {
            destRow.RowID = sourceRow.RowID;
            destRow.State = CardRowState.Inserted;
            foreach (var pair in sourceRow
                .Where(k => !k.Key.StartsWith(CardHelper.SystemKeyPrefix)
                    && !k.Key.StartsWith(CardHelper.UserKeyPrefix)
                    && !StageTypeSettingsNaming.IsPlainName(k.Key)
                    && !StageTypeSettingsNaming.IsSectionName(k.Key)
                    && k.Key != KrConstants.Order))
            {
                destRow.Fields[pair.Key] = pair.Value;
            }

            destRow.Fields[KrConstants.KrStages.ExtraSources] = null;
            destRow.Fields[KrConstants.KrStages.NestedProcessID] = null;
            destRow.Fields[KrConstants.KrStages.ParentStageRowID] = null;
            destRow.Fields[KrConstants.KrStages.NestedOrder] = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ModifyInternal(
            CardRow sourceRow,
            CardRow destRow)
        {
            destRow.State = CardRowState.Modified;
            foreach (var pair in sourceRow
                .Where(k => !k.Key.StartsWith(CardHelper.SystemKeyPrefix)
                    && !k.Key.StartsWith(CardHelper.UserKeyPrefix)
                    && destRow.ContainsKey(k.Key)
                    && !StageTypeSettingsNaming.IsPlainName(k.Key)
                    && !StageTypeSettingsNaming.IsSectionName(k.Key)
                    && k.Key != KrConstants.Order))
            {
                destRow.Fields[pair.Key] = pair.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DeleteInternal(
            CardRow sourceRow,
            CardRow destRow)
        {
            if (KrStageSerializer.CanBeSkipped(destRow))
            {
                destRow.State = CardRowState.Modified;

                destRow.Fields[KrConstants.KrStages.Skip] = BooleanBoxes.True;
                destRow.Fields[KrConstants.KrStages.Hidden] = BooleanBoxes.True;
                return false;
            }

            destRow.State = CardRowState.Deleted;
            return true;
        }
    }
}