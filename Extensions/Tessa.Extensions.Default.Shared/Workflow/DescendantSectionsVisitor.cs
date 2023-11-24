using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Cards.Metadata;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow
{
    public abstract class DescendantSectionsVisitor
    {
        #region fields

        protected readonly ICardMetadata CardMetadata;

        #endregion

        #region constructor

        protected DescendantSectionsVisitor(
            ICardMetadata cardMetadata)
        {
            this.CardMetadata = cardMetadata;
        }

        #endregion

        #region abstract

        protected abstract void VisitTopLevelSection(
            CardRow row,
            CardSection section,
            IDictionary<Guid, Guid> stageMapping);

        protected abstract void VisitNestedSection(
            CardRow row,
            CardSection section,
            Guid parentRowID,
            Guid topLevelRowID,
            IDictionary<Guid, Guid> stageMapping);

        #endregion

        #region public

        /// <summary>
        /// Обход всех коллекционных секций, для которых предком является строка из секции topLevelSectionName.
        /// </summary>
        /// <param name="cardSections"></param>
        /// <param name="typeID"></param>
        /// <param name="topLevelSectionName"></param>
        public void Visit(
            StringDictionaryStorage<CardSection> cardSections,
            Guid typeID,
            string topLevelSectionName)
        {
            var cardTypeMetadata = this.CardMetadata.GetMetadataForTypeAsync(typeID).GetAwaiter().GetResult(); // TODO async
            var cardType = cardTypeMetadata.GetCardTypesAsync().GetAwaiter().GetResult()[typeID]; // TODO async
            var schemeItems = new HashSet<Guid, CardTypeSchemeItem>(
                p => p.SectionID,
                cardType.SchemeItems);

            var topLevelSecMetadata = this.CardMetadata.GetSectionsAsync().GetAwaiter().GetResult()[topLevelSectionName]; // TODO async
            // Связь RowID строки в подчиненной коллекционной секции произвольной вложенности
            // <->
            // RowID строки верхнего уровня.
            var stagesMapping = new Dictionary<Guid, Guid>();
            var topLevelSection = cardSections[topLevelSectionName];
            foreach (var topLevelRow in topLevelSection.Rows)
            {
                stagesMapping[topLevelRow.RowID] = topLevelRow.RowID;
                this.VisitTopLevelSection(topLevelRow, topLevelSection, stagesMapping);
            }

            var previousLayer = new HashSet<Guid> { topLevelSecMetadata.ID };
            var currentLayer = new HashSet<Guid>();

            // Обход зависимостей проводится в "ширину"
            // Вершиной является переданная через параметр секция
            // В первый слой входят все секции, имеющие столбец с указанием на родителя, т.е. на вершину
            // Вторым слоем будут все секции, у которых "ссылка на родителя" указывает на секции первого слоя
            // и т.д. до тех пор, пока очередной слой не станет пустым.
            while (previousLayer.Count != 0)
            {
                foreach (var secMetadata in cardTypeMetadata.GetSectionsAsync().GetAwaiter().GetResult()) // TODO async
                {
                    // Секция не используется в карточке, а значит в обработке не участвует.
                    if (!schemeItems.TryGetItem(secMetadata.ID, out var schemeItem))
                    {
                        continue;
                    }

                    // Получаем комплексный столбец с ссылкой на родителя.
                    var refSecTuple = GetParentColumnSec(secMetadata, previousLayer);
                    var parentComplexColumn = refSecTuple.Item1;
                    var parentRowIDColumn = refSecTuple.Item2;
                    if (parentComplexColumn == null
                        || parentRowIDColumn == null)
                    {
                        continue;
                    }

                    // Комплексный столбец используется в карточке.
                    if (!schemeItem.ColumnIDList.Contains(parentComplexColumn.ID))
                    {
                        continue;
                    }

                    ListStorage<CardRow> rows;
                    if (cardSections.TryGetValue(secMetadata.Name, out var section)
                        && (rows = section.TryGetRows()) != null
                        && rows.Count != 0)
                    {
                        currentLayer.Add(secMetadata.ID);
                        // Проставляем каждой строке ссылку на этап, ориентируясь по
                        // ссылке на непосредственного родителя.
                        foreach (var row in rows)
                        {
                            if (row.TryGetValue(parentRowIDColumn.Name, out var parentIDObj)
                                && parentIDObj is Guid parentID
                                && stagesMapping.TryGetValue(parentID, out var topLevelRowID))
                            {
                                stagesMapping[row.RowID] = topLevelRowID;
                                this.VisitNestedSection(row, section, parentID, topLevelRowID, stagesMapping);
                            }
                        }
                    }
                }
                SwapLayers(ref previousLayer, ref currentLayer);
            }
        }

        #endregion

        #region private

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Tuple<CardMetadataColumn, CardMetadataColumn> GetParentColumnSec(
            CardMetadataSection secMetadata,
            HashSet<Guid> previousLayer)
        {
            CardMetadataColumn complex = null;
            CardMetadataColumn rowID = null;
            foreach (var column in secMetadata.Columns)
            {
                if (column.ParentRowSection != null
                    && column.ColumnType == CardMetadataColumnType.Complex
                    && previousLayer.Contains(column.ParentRowSection.ID))
                {
                    complex = column;
                }
                else if (complex != null
                    && column.ColumnType == CardMetadataColumnType.Physical
                    && column.ParentRowSection?.ID == complex.ParentRowSection.ID
                    && column.ComplexColumnIndex == complex.ComplexColumnIndex)
                {
                    rowID = column;
                    break;
                }
            }

            return new Tuple<CardMetadataColumn, CardMetadataColumn>(complex, rowID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapLayers(ref HashSet<Guid> previousLayer, ref HashSet<Guid> currentLayer)
        {
            var tmp = previousLayer;
            tmp.Clear();
            previousLayer = currentLayer;
            currentLayer = tmp;
        }

        #endregion

    }
}