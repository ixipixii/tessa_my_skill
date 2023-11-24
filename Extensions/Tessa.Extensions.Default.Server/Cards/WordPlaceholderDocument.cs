using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Placeholders;
using Tessa.Platform.Placeholders.Extensions;
using Tessa.Platform.Storage;
using A = DocumentFormat.OpenXml.Drawing;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Hyperlink = DocumentFormat.OpenXml.Wordprocessing.Hyperlink;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Wp14 = DocumentFormat.OpenXml.Office2010.Word.Drawing;
using WPS = DocumentFormat.OpenXml.Office2010.Word.DrawingShape;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Объект, определяющий способы хранения и изменения текста с заменяемыми плейсхолдерами
    /// для документа Word.
    /// </summary>
    public sealed class WordPlaceholderDocument :
        OpenXmlPlaceholderDocument
    {
        #region Nested Types

        private enum TableGroupType
        {
            Row,
            Group,
            Table,
        }

        private sealed class TableGroup : StorageObject
        {
            #region Fields

            private List<OpenXmlElement> baseElements;
            private List<IPlaceholder> tablePlaceholders;

            #endregion

            #region Constructors

            public TableGroup(Dictionary<string, object> storage = null)
                : base(storage ?? new Dictionary<string, object>(StringComparer.Ordinal))
            {
                Init(nameof(ID), string.Empty);
                Init(nameof(Name), string.Empty);
                Init(nameof(StartIndex), Int32Boxes.Zero);
                Init(nameof(EndIndex), Int32Boxes.Zero);
                Init(nameof(StartPosition), null);
                Init(nameof(EndPosition), null);
                Init(nameof(GroupType), 0);
            }

            #endregion

            #region Storage Properties

            /// <summary>
            /// Идентификатор закладки в Word
            /// </summary>
            public string ID
            {
                get { return Get<string>(nameof(ID)); }
                set { Set(nameof(ID), value); }
            }

            /// <summary>
            /// Имя закладки в Word
            /// </summary>
            public string Name
            {
                get { return Get<string>(nameof(Name)); }
                set { Set(nameof(Name), value); }
            }

            /// <summary>
            /// Индекс метки начала закладки в тексте ее родительского элемента
            /// </summary>
            public int StartIndex
            {
                get { return Get<int>(nameof(StartIndex)); }
                set { Set(nameof(StartIndex), value); }
            }

            /// <summary>
            /// Индекс метки конца закладки в тексте ее родительского элемента
            /// </summary>
            public int EndIndex
            {
                get { return Get<int>(nameof(EndIndex)); }
                set { Set(nameof(EndIndex), value); }
            }

            /// <summary>
            /// Позиция метки начала закладки в структуре документа
            /// </summary>
            public List<object> StartPosition
            {
                get { return Get<List<object>>(nameof(StartPosition)); }
                set { Set(nameof(StartPosition), value); }
            }

            /// <summary>
            /// Позиция метки конца закладки в структуре документа
            /// </summary>
            public List<object> EndPosition
            {
                get { return Get<List<object>>(nameof(EndPosition)); }
                set { Set(nameof(EndPosition), value); }
            }

            /// <summary>
            /// Тип группы
            /// </summary>
            public TableGroupType GroupType
            {
                get { return (TableGroupType)Get<int>(nameof(GroupType)); }
                set { Set(nameof(GroupType), value.GetHashCode()); }
            }

            #endregion

            #region Properties

            /// <summary>
            /// Внутренняя таблица
            /// </summary>
            public TableGroup InnerTableGroup { get; set; }

            /// <summary>
            /// Определяет, что вся закладка находится внутри параграфа
            /// </summary>
            public bool InParagraph { get; set; }

            /// <summary>
            /// Родительский элемент по отношению ко всем <see cref="BaseElements"/>
            /// </summary>
            public OpenXmlElement TableElement { get; set; }

            /// <summary>
            /// Позиция начала элемента таблицы
            /// </summary>
            public List<object> TableStartPosition { get; set; }

            /// <summary>
            /// Позиция окончания элемента таблицы
            /// </summary>
            public List<object> TableEndPosition { get; set; }

            /// <summary>
            /// Те элементы, что копируются, в которых потом производится замена плейсхолдеров
            /// </summary>
            public List<OpenXmlElement> BaseElements => this.baseElements ??= new List<OpenXmlElement>();

            /// <summary>
            /// Табличные плейсхолдеры
            /// </summary>
            public List<IPlaceholder> TablePlaceholders => this.tablePlaceholders ??= new List<IPlaceholder>();

            #endregion

            #region Public Methods

            public bool Contains(TableGroup info)
            {
                return
                    (!info.InParagraph || !InParagraph || (StartIndex <= info.StartIndex && EndIndex >= info.EndIndex))
                    && OpenXmlHelper.IsLessOrEquals(TableStartPosition, info.TableStartPosition)
                    && OpenXmlHelper.IsLessOrEquals(info.TableEndPosition, TableEndPosition, true);
            }

            public bool Contains(List<object> position)
            {
                return
                    OpenXmlHelper.IsLessOrEquals(TableStartPosition, position)
                    && OpenXmlHelper.IsLessOrEquals(position, TableEndPosition, true);
            }

            public bool ContainsPlaceholder(IPlaceholder placeholder)
            {
                var placeholderIndex = placeholder.Info.Get<int>(OpenXmlHelper.IndexField);
                return placeholderIndex >= StartIndex && placeholderIndex < EndIndex;
            }

            #endregion
        }

        #endregion

        #region Consts

        /// <summary>
        /// Определяет константу для хранения в Info табличного плейсхолдера элемента строки таблицы, в которой расположен плейсхолдер
        /// </summary>
        private const string TableRowField = "TableRow";

        #endregion

        #region Fields

        /// <summary>
        /// Контекст расширений
        /// </summary>
        private WordPlaceholderReplaceExtensionContext extensionContext;

        /// <summary>
        /// Документ Word
        /// </summary>
        private WordprocessingDocument wordDocument;

        /// <summary>
        /// Идентификатор очередного объекта docPr, который должен быть уникален в пределах документа
        /// </summary>
        private uint docPropertiesNextId = 1u;

        /// <summary>
        /// Информация о таблицах, созданных закладками
        /// </summary>
        private List<TableGroup> tableGroups = new List<TableGroup>();


        #endregion

        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класс с указанием потока файла документа, в котором должны быть заменены плейсхолдеры.
        /// </summary>
        /// <param name="stream">
        /// Поток файла документа, в котором должны быть заменены плейсхолдеры.
        /// Не может быть равен <c>null</c>.
        /// </param>
        /// <param name="templateID">
        /// ID карточки шаблона файла.
        /// </param>
        public WordPlaceholderDocument(MemoryStream stream, Guid templateID)
            : base(stream, templateID)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Возвращает все дочерние элементы с типом Text среди всех дочерних элементов объекта <c>baseElement</c>
        /// </summary>
        /// <param name="baseElement">Базовый элемент, начиная с которого производим поиск</param>
        /// <returns>Список элементов типа Text, или null, если <c>baseElement</c> не содержит дочерних элементов типа Text</returns>
        private static List<Text> GetTextElements(OpenXmlElement baseElement)
        {
            List<Text> result = new List<Text>();
            foreach (OpenXmlElement e in baseElement.ChildElements)
            {
                Type eType = e.GetType();
                if (eType == typeof(Text))
                {
                    result.Add((Text)e);
                }
                else if (eType != typeof(Hyperlink) && eType != typeof(Paragraph) && e.HasChildren)
                {
                    result.AddRange(GetTextElements(e));
                }
            }

            return result;
        }

        /// <summary>
        /// Метод для получаения ближайшего родительского элемента типа строка таблицы или строка списка, или null, если такого элемента нет
        /// </summary>
        /// <param name="element">Элемент, относительно которого ведется поиск строки таблицы или списка</param>
        /// <returns>Возвращает ближайший родительский элемент типа строка таблицы или строку списка, или null, если такого элемента нет</returns>
        private static OpenXmlElement GetTableRowElement(OpenXmlElement element)
        {
            if (element == null)
            {
                return null;
            }

            ParagraphProperties prop;
            OpenXmlElement tableRowElement = element;

            while (tableRowElement != null
                && (tableRowElement.GetType() != typeof(TableRow))
                && (tableRowElement.GetType() != typeof(Paragraph) || (prop = tableRowElement.GetFirstChild<ParagraphProperties>()) == null || prop.GetFirstChild<NumberingProperties>() == null))
            {
                tableRowElement = tableRowElement.Parent;
            }

            return tableRowElement;
        }

        /// <summary>
        /// Метод возвращает все плейсхолдеры из Relationships, относящиеся к данной строке таблицы.
        /// </summary>
        /// <param name="tableRow">Элемент строки таблицы/списка, в котором проихводится поиск гиперссылок</param>
        /// <param name="relationshipsPlaceholders">Список табличных плейсхолдеров из Relationships</param>
        /// <returns></returns>
        private static IEnumerable<IPlaceholder> CheckForRelationshipsPlaceholders(OpenXmlElement tableRow, IList<IPlaceholder> relationshipsPlaceholders)
        {
            List<IPlaceholder> result = new List<IPlaceholder>();
            if (tableRow == null || relationshipsPlaceholders == null || relationshipsPlaceholders.Count == 0)
            {
                return result;
            }

            foreach (Hyperlink hyperlinkElement in tableRow.Descendants<Hyperlink>())
            {
                IEnumerable<IPlaceholder> placeholders = relationshipsPlaceholders
                    .Where(x => (string)x.Info.Get<List<object>>(OpenXmlHelper.PositionField)[1] == hyperlinkElement.Id)
                    .ToArray();

                foreach (IPlaceholder placeholder in placeholders)
                {
                    placeholder.Info[OpenXmlHelper.BaseElementField] = hyperlinkElement;
                }

                result.AddRange(placeholders);
            }

            return result;
        }

        /// <summary>
        /// Производит замену Replacement'ов в заданном элементе FieldCode
        /// </summary>
        /// <param name="fieldCode">FieldCode, в котором производится замена плейсхолдеров</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <param name="replacements">Массив плейсхолдеров для замены</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task ReplaceElementsInFieldCodeAsync(
            FieldCode fieldCode,
            CancellationToken cancellationToken = default,
            params IPlaceholderReplacement[] replacements)
        {
            string codeText = fieldCode.Text;

            StringBuilder decodedText = new StringBuilder(Uri.UnescapeDataString(codeText));
            decodedText.Replace(HyperlinkRemoveHead, string.Empty, 0, Math.Min(HyperlinkRemoveHeadLength, decodedText.Length));

            foreach (IPlaceholderReplacement replacement in replacements)
            {
                var newValue = replacement.NewValue;
                string newText;
                if (WithExtensions)
                {
                    extensionContext.Placeholder = replacement.Placeholder;
                    extensionContext.PlaceholderValue = replacement.NewValue;
                    await BeforePlaceholderReplaceAsync(extensionContext.ReplacementContext);
                    newValue = extensionContext.PlaceholderValue;

                    extensionContext.PlaceholderText = newValue.Text;
                    await AfterPlaceholderReplaceAsync(extensionContext.ReplacementContext);
                    newText = extensionContext.PlaceholderText;
                }
                else
                {
                    newText = newValue.Text;
                }

                decodedText.Replace(replacement.Placeholder.Text, newText);
            }

            fieldCode.Text = decodedText.ToString();
        }

        /// <summary>
        /// Получает дочерний элемент newParent, соответствующий элементу baseChild относительно baseParent. Элемент newParent должен быть полной копией элемента baseParent
        /// </summary>
        /// <param name="baseChildType">Тип искомого элемента</param>
        /// <param name="baseParent">Родительский элемент, относительно которого ведется поиск</param>
        /// <param name="baseChild">Дочерний элемент, соответствие которого мы ищем в newParent</param>
        /// <param name="newParent">Копия родительского элемента, дочерний элемент которого мы ищем</param>
        /// <returns></returns>
        private static OpenXmlElement GetRelativeElement(
            OpenXmlElement baseParent,
            OpenXmlElement baseChild,
            Type baseChildType,
            OpenXmlElement newParent)
        {
            if (baseChildType == typeof(Paragraph))
            {
                return GetRelativeElement<Paragraph>(baseParent, baseChild, newParent);
            }
            if (baseChildType == typeof(Hyperlink))
            {
                return GetRelativeElement<Hyperlink>(baseParent, baseChild, newParent);
            }
            if (baseChildType == typeof(FieldCode))
            {
                return GetRelativeElement<FieldCode>(baseParent, baseChild, newParent);
            }
            if (baseChildType == typeof(Run))
            {
                return GetRelativeElement<Run>(baseParent, baseChild, newParent);
            }
            if (baseChildType == typeof(TableRow))
            {
                return GetRelativeElement<TableRow>(baseParent, baseChild, newParent);
            }
            if (baseChildType == typeof(TableCell))
            {
                return GetRelativeElement<TableCell>(baseParent, baseChild, newParent);
            }
            return null;
        }

        private A.Graphic CreateGraphicFromImage(
            byte[] imageBytes,
            ImagePartType imagePartType,
            WPS.ShapeProperties existingProperties,
            DW.Extent existingExtent,
            double width,
            double height,
            bool reformat)
        {
            // добавляем изображение в документ и получаем его relationshipId
            MainDocumentPart mainPart = this.wordDocument.MainDocumentPart;

            ImagePart imagePart = mainPart.AddImagePart(imagePartType);
            imagePart.FeedData(new MemoryStream(imageBytes));

            string relationshipId = mainPart.GetIdOfPart(imagePart);

            // берём из надписи или генерим свойства фигуры для картинки
            PIC.ShapeProperties actualProperties;
            if (!reformat && existingProperties != null)
            {
                OpenXmlElement[] childElements = existingProperties.ChildElements
                    .Select(x => x.CloneNode(deep: true))
                    .ToArray();

                var transform2D = (A.Transform2D)existingProperties.Transform2D.CloneNode(deep: true);

                actualProperties = new PIC.ShapeProperties(childElements)
                {
                    BlackWhiteMode = existingProperties.BlackWhiteMode,
                    Transform2D = transform2D,
                };
            }
            else
            {
                actualProperties = new PIC.ShapeProperties(
                    new A.Transform2D(
                        new A.Offset { X = 0L, Y = 0L },
                        new A.Extents { Cx = OpenXmlHelper.DefaultExtentsCx, Cy = OpenXmlHelper.DefaultExtentsCy }),
                    new A.PresetGeometry(new A.AdjustValueList())
                    { Preset = A.ShapeTypeValues.Rectangle });
            }

            // определяем и проставляем размеры картинки
            A.Extents extents = actualProperties.Transform2D?.Extents;
            if (extents != null || existingExtent != null)
            {
                Int64Value cx = OpenXmlHelper.DefaultExtentsCx;
                Int64Value cy = OpenXmlHelper.DefaultExtentsCy;

                if (width > 0.0)
                {
                    cx = OpenXmlHelper.PixelsToEmu(width);
                }

                if (height > 0.0)
                {
                    cy = OpenXmlHelper.PixelsToEmu(height);
                }

                if (reformat && (width <= 0.0 || height <= 0.0))
                {
                    // нужно вычислить актуальные размеры изображения
                    using Image image = Image.FromStream(new MemoryStream(imageBytes));
                    if (width <= 0.0)
                    {
                        cx = OpenXmlHelper.GetImageCx(image);
                    }

                    if (height <= 0.0)
                    {
                        cy = OpenXmlHelper.GetImageCy(image);
                    }
                }

                if (extents != null)
                {
                    extents.Cx = cx;
                    extents.Cy = cy;
                }

                if (existingExtent != null)
                {
                    existingExtent.Cx = cx;
                    existingExtent.Cy = cy;
                }
            }

            // генерим объект Graphic, в который картинка обёрнута
            return new A.Graphic(
                new A.GraphicData(
                    new PIC.Picture(
                        new PIC.NonVisualPictureProperties(
                            new PIC.NonVisualDrawingProperties { Id = 0U, Name = "Image" },
                            new PIC.NonVisualPictureDrawingProperties()),
                        new PIC.BlipFill(
                            new A.Blip(
                                new A.BlipExtensionList(
                                    new A.BlipExtension { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }))
                            {
                                Embed = relationshipId,
                                CompressionState = A.BlipCompressionValues.Print
                            },
                            new A.Stretch(new A.FillRectangle())),
                        actualProperties))
                {
                    Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
                });
        }

        /// <summary>
        /// Метод для заполнения таблиц из <see cref="tableGroups"/> элементами документа
        /// </summary>
        private void FiilTableGroups()
        {
            // Заполняем таблицы-по закладкам элементами документа
            // Закладка может целиком лежать внутри одного параграфа, тогда базовыми элементами будут являться объекты внутри параграфа.
            // Если закладка лежит между несколькими параграфами, то базовыми элементами считаем все параграфы и таблицы от первого до последнего
            // Поиском ведем на самом верхнем уровне среди этих двух элементов
            foreach (var table in tableGroups)
            {
                var startPosition = table.StartPosition;
                var endPosition = table.EndPosition;
                var minCount = Math.Min(startPosition.Count, endPosition.Count);
                int depth = 0;
                // Берем глубину, как индекс первого различия в позициях начала и конца позиции
                while (depth < minCount
                    && (int)startPosition[depth] == (int)endPosition[depth])
                {
                    depth++;
                }

                depthRecalc:
                table.TableStartPosition = startPosition.Take(depth + 1).ToList();
                table.TableEndPosition = endPosition.Take(depth + 1).ToList();

                var firstElement = GetElementByPosition(wordDocument.MainDocumentPart, table.TableStartPosition);

                if (startPosition.Count == depth + 1
                    && endPosition.Count == depth + 1
                    && firstElement.Parent is Paragraph)
                {
                    table.InParagraph = true;
                    table.TableStartPosition.RemoveAt(table.TableStartPosition.Count - 1);
                    table.TableEndPosition.RemoveAt(table.TableEndPosition.Count - 1);

                    var startBaseElement = firstElement.Parent;
                    table.TableElement = startBaseElement;

                    bool needAdd = false;
                    for (int i = 0; i < startBaseElement.ChildElements.Count; i++)
                    {
                        var element = startBaseElement.ChildElements[i];
                        if (element is BookmarkStart start)
                        {
                            if (start.Id.Value == table.ID)
                            {
                                needAdd = true;
                            }
                        }
                        else if (element is BookmarkEnd end)
                        {
                            if (end.Id.Value == table.ID)
                            {
                                break;
                            }
                        }
                        else if (needAdd)
                        {
                            table.BaseElements.Add(element);
                        }
                    }
                }
                else
                {
                    // Число дополнительных элементов, из которых состоит строка таблицы, помимо firstElement
                    var nextElementsCount = (int)endPosition[depth] - (int)startPosition[depth];

                    // Последний элемент нужно добавлять только в ситуации, когда перед окончанием закладки есть что-нибудь полезное
                    var lastElementIsValueable = false;
                    var endIndex = (int)endPosition[endPosition.Count - 1];
                    var endBaseElement = GetElementByPosition(wordDocument.MainDocumentPart, endPosition.Take(endPosition.Count - 1));

                    // Если первый элемент входит в коллекцию базового последнего элемента, то начинаем отсчет от следующего элемента
                    int i = endBaseElement.IndexOf(firstElement) + 1;

                    for (; i < endIndex; i++)
                    {
                        var childElement = endBaseElement.ChildElements[i];
                        if (!(childElement is TableProperties
                            || childElement is ParagraphProperties
                            || childElement is BookmarkStart
                            || childElement is BookmarkEnd))
                        {
                            lastElementIsValueable = true;
                            break;
                        }
                    }

                    if (!lastElementIsValueable)
                    {
                        // Если нет значимых элементов, есть 2 варианта развития событий:
                        // 1) Просто уменьшаем позицию окончания таблицы на 1. Возникает, когда выделяется параграф целиком. Метка в таком случае ставится начале в следующего параграфа.
                        // 2) Word очень странно размещает конец метки внутри таблицы в ситуации, если при сохранении курсор был указан на абзац сразу после таблицы.
                        // или в последнем абзаце есть текст. В первом случае метка будет внутри параграфа сразу за таблицей. Во втором случае будет как отдельный элемент за таблицей,
                        // но перед параграфом с текстом
                        // Из-за этих особенностей, расчет глубины метки некорректен, т.к. общим родителем начала и конца всегда будет именно элемент Body.
                        // Поэтому при такой ситуации мы увеличиваем depth на 1, обновляем позицию окончания метки и перерасчитываем положение таблицы.

                        bool isBody = endBaseElement is Body;

                        // Если первый элемент таблица целиком, а всего их 2 (последний элемент - это внешний параграф или внешняя метка), то ситуация (2)
                        if ((isBody || nextElementsCount == 1) && firstElement is Table)
                        {
                            depth++;
                            if (!isBody)
                            {
                                endPosition.RemoveAt(endPosition.Count - 1);
                            }
                            endPosition[endPosition.Count - 1] = (int)endPosition[endPosition.Count - 1] - nextElementsCount;
                            endPosition.Add(firstElement.ChildElements.Count - 1);
                            goto depthRecalc;
                        }
                        else // иначе ситуация (1)
                        {
                            table.TableEndPosition[table.TableEndPosition.Count - 1] = (int)table.TableEndPosition[table.TableEndPosition.Count - 1] - 1;
                            nextElementsCount--;
                        }
                    }

                    table.TableElement = firstElement.Parent;
                    table.BaseElements.Add(firstElement);

                    while (nextElementsCount-- > 0)
                    {
                        firstElement = firstElement.NextSibling();
                        if (firstElement is BookmarkStart
                            || firstElement is BookmarkEnd)
                        {
                            continue;
                        }

                        table.BaseElements.Add(firstElement);
                    }
                }
            }
        }

        /// <summary>
        /// Метод для формирования связей между таблицами. Таблицы разных типов могут быть вложены друг в друга.
        /// </summary>
        /// <param name="newTables">Добавляемые таблицы</param>
        /// <param name="tables">Уже существующий список таблиц, если он уже есть</param>
        /// <returns>Возвращает список таблиц со связями</returns>
        private List<TableGroup> FillTableRelationships(
            IEnumerable<TableGroup> newTables,
            List<TableGroup> tables = null)
        {
            if (tables == null)
            {
                tables = new List<TableGroup>();
            }
            foreach (var info in newTables)
            {
                bool isInnerTable = false;
                foreach (var table in tables)
                {
                    if (table.InnerTableGroup != null
                        && table.InnerTableGroup.Contains(info))
                    {
                        table.InnerTableGroup.InnerTableGroup = info;
                        isInnerTable = true;
                        break;
                    }
                    else if (table.Contains(info))
                    {
                        table.InnerTableGroup = info;
                        isInnerTable = true;
                        break;
                    }
                    // Обратную проверку не делаем, т.к. их добавление производится по дереву
                }

                if (!isInnerTable)
                {
                    tables.Add(info);
                }
            }

            return tables;
        }

        /// <summary>
        /// Метод для распределения табличных плейсхолдеров по таблицам.
        /// Может создавать типовые таблиц без групп, если плейсхолдер записан в строке таблицы или в перечислении
        /// </summary>
        /// <param name="tables">Таблицы</param>
        /// <param name="tablePlaceholders">Табличные плейсхолдеры</param>
        /// <param name="relationshipsPlaceholders">Плейсхолдеры из объектов Relationships</param>
        private void PrepareTablePlaceholders(List<TableGroup> tables, IList<IPlaceholder> tablePlaceholders, out IList<IPlaceholder> relationshipsPlaceholders)
        {
            relationshipsPlaceholders = new List<IPlaceholder>();
            // Определяем таблицу/список, к которому относится плейсхолдер
            foreach (IPlaceholder placeholder in tablePlaceholders)
            {
                if (IsRelationship(placeholder))
                {
                    relationshipsPlaceholders.Add(placeholder);
                    continue;
                }

                OpenXmlElement baseElement = GetElementByPlaceholder(wordDocument.MainDocumentPart, placeholder);
                placeholder.Info.Add(OpenXmlHelper.BaseElementField, baseElement);

                var placeholderPosition = placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField);
                bool tableFounded = false;

                foreach (var table in tables)
                {
                    var checkTable = table;
                    TableGroup prevTable = null;

                    while (!tableFounded)
                    {
                        if (checkTable.Contains(placeholderPosition)
                            && (!checkTable.InParagraph || checkTable.ContainsPlaceholder(placeholder)))
                        {
                            if (checkTable.InnerTableGroup != null)
                            {
                                prevTable = checkTable;
                                checkTable = checkTable.InnerTableGroup;
                            }
                            else if (checkTable.GroupType != TableGroupType.Table)
                            {
                                checkTable.TablePlaceholders.Add(placeholder);
                                tableFounded = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (prevTable != null
                                && prevTable.GroupType != TableGroupType.Table)
                        {
                            prevTable.TablePlaceholders.Add(placeholder);
                            tableFounded = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (tableFounded)
                    {
                        break;
                    }
                }

                if (!tableFounded)
                {
                    var rowElement = GetTableRowElement(baseElement);
                    if (rowElement != null)
                    {
                        var startPosition = new List<object>();
                        OpenXmlElement root = wordDocument.MainDocumentPart.RootElement;

                        for (int i = 0; i < placeholderPosition.Count; i++)
                        {
                            var index = (int)placeholderPosition[i];
                            if (index < 0)
                            {
                                startPosition.Add(Int32Boxes.Box(index));
                            }
                            else
                            {
                                startPosition.Add(Int32Boxes.Box(index));
                                root = root.ChildElements[index];
                                if (root == rowElement)
                                {
                                    break;
                                }
                            }
                        }
                        var endPosition = startPosition.ToList();

                        // Если не нашли таблицу, пытаемся создать новую
                        var newTable = new TableGroup()
                        {
                            GroupType = TableGroupType.Row,
                            TableElement = rowElement.Parent,
                            StartPosition = startPosition,
                            EndPosition = endPosition,
                            TableStartPosition = startPosition,
                            TableEndPosition = endPosition,
                        };

                        newTable.BaseElements.Add(rowElement);
                        newTable.TablePlaceholders.Add(placeholder);

                        FillTableRelationships(
                            new[] { newTable },
                            tables);
                    }
                }

                placeholder.Info.Add(TableRowField, GetTableRowElement(baseElement));
            }
        }

        /// <summary>
        /// Метод для создания и добавления новых элементов, созданных из копий элементов таблицы <paramref name="currentTableGroup"/>
        /// </summary>
        /// <param name="currentTableGroup">Таблица</param>
        /// <param name="insertBeforeElement">Объект, перед которым происходит вставка новых элементов</param>
        /// <param name="parentElement">
        /// Родительский объект, в который производится вставка новых элементов. Испрользуется, если <paramref name="insertBeforeElement"/> равен <c>null</c>
        /// </param>
        /// <returns>Возвращает список новых элементов</returns>
        private static List<OpenXmlElement> CreateNewElements(
            TableGroup currentTableGroup,
            OpenXmlElement insertBeforeElement,
            OpenXmlElement parentElement)
        {
            var newTableRows = currentTableGroup.BaseElements.Select(x => x.CloneNode(true)).ToList();

            foreach (OpenXmlElement newTableRow in newTableRows)
            {
                // Добавляем новые строки в таблицу
                if (insertBeforeElement != null)
                {
                    insertBeforeElement.InsertBeforeSelf(newTableRow);
                }
                else
                {
                    // ReSharper disable once PossiblyMistakenUseOfParamsMethod
                    parentElement.Append(newTableRow);
                }
            }

            return newTableRows;
        }

        /// <summary>
        /// Метод для замены плейсхолдеров строки для заданной таблицы
        /// </summary>
        /// <param name="context">Контекст замены плейсхолдеров</param>
        /// <param name="currentTableGroup">Таблица</param>
        /// <param name="newTableRows">Новые элементы строки таблицы, где производится замена</param>
        /// <param name="row">Строка с данными расчета плейсхолдеров</param>
        /// <param name="isGroup">Определяет, производится ли замена группы или строки</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task ReplaceInNewElementsAsync(
            IPlaceholderReplacementContext context,
            TableGroup currentTableGroup,
            IReadOnlyCollection<OpenXmlElement> newTableRows,
            IPlaceholderRow row,
            bool isGroup = false)
        {
            var rowPlaceholders = currentTableGroup.TablePlaceholders;
            if (!isGroup
                && WithExtensions)
            {
                extensionContext.Row = row;
                extensionContext.RowElements.Clear();
                extensionContext.RowElements.AddRange(newTableRows);
                await BeforeRowReplaceAsync(context);
            }

            (OpenXmlElement baseElement, OpenXmlElement rowElement)[] newBaseElementArray = rowPlaceholders
                .Select(placeholder =>
                {
                    OpenXmlElement baseElement = placeholder.Info.Get<OpenXmlElement>(OpenXmlHelper.BaseElementField);
                    if (baseElement == null)
                    {
                        return default;
                    }

                    Type baseElementType = baseElement.GetType();

                    if (currentTableGroup.InParagraph)
                    {
                        return (currentTableGroup.TableElement, currentTableGroup.TableElement);
                    }
                    var i = 0;
                    foreach (var newTableRow in newTableRows)
                    {
                        var relativeElement = GetRelativeElement(currentTableGroup.BaseElements[i++], baseElement, baseElementType, newTableRow);
                        if (relativeElement != null)
                        {
                            return (relativeElement, newTableRow);
                        }
                    }
                    return default;
                })
                .ToArray();

            int index = 0;
            foreach (IPlaceholder placeholder in rowPlaceholders)
            {
                (OpenXmlElement newBaseElement, OpenXmlElement currentTableRow) = newBaseElementArray[index++];
                if (newBaseElement == null)
                {
                    continue;
                }

                if (WithExtensions)
                {
                    extensionContext.CurrentRowElement = currentTableRow;
                }

                // заменяем текст, который может быть равен null в результате замены
                PlaceholderValue newValue = await ((ITablePlaceholderType)placeholder.Type)
                    .ReplaceAsync(context, placeholder, row, context.CancellationToken)
                    ?? PlaceholderValue.Empty;

                if (IsRelationship(placeholder))
                {
                    Hyperlink hyperlink = (Hyperlink)newBaseElement;
                    hyperlink.Id = await ReplaceElementsInRelationshipsWithCopyAsync(
                        wordDocument.MainDocumentPart,
                        hyperlink.Id,
                        context.CancellationToken,
                        new PlaceholderReplacement(placeholder, newValue));
                }
                else
                {
                    // Заменяем плейсхолдер в новом базовом элементе
                    Type baseElementType = newBaseElement.GetType();
                    if (baseElementType == typeof(Paragraph)
                        || baseElementType == typeof(Hyperlink))
                    {
                        await ReplaceElementsInCompositeElementAsync(newBaseElement, context.CancellationToken,
                            new PlaceholderReplacement(placeholder, newValue));
                    }
                    else if (baseElementType == typeof(FieldCode))
                    {
                        await ReplaceElementsInFieldCodeAsync((FieldCode)newBaseElement, context.CancellationToken,
                            new PlaceholderReplacement(placeholder, newValue));
                    }
                }
            }

            if (!isGroup && WithExtensions)
            {
                await AfterRowReplaceAsync(context);
            }
        }

        #endregion

        #region Base Overrides

        protected override IPlaceholderReplaceExtensionContext ExtensionContext => extensionContext;

        protected override IPlaceholderReplaceExtensionContext CreateExtensionContext(IPlaceholderReplacementContext context)
        {
            return extensionContext = new WordPlaceholderReplaceExtensionContext(context, context.CancellationToken);
        }

        /// <summary>
        /// Метод для получения информации о плейсхолдерах документа из базы данных
        /// </summary>
        /// <param name="dbScope">Объект dbScope текущего подключения к базе данных</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Возвращает список плейсхолдеров документа из базы данных</returns>
        protected override async Task<List<IPlaceholderText>> GetPlaceholdersFromDatabaseAsync(
            IDbScope dbScope,
            CancellationToken cancellationToken = default)
        {
            List<IPlaceholderText> placeholders = new List<IPlaceholderText>();
            // Пытаемся получить плейсхолдеры из базы
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builderFactory = dbScope.BuilderFactory;

                db
                    .SetCommand(
                        builderFactory
                            .Select().Top(1)
                                .If(Dbms.SqlServer, b => b.Q("DATALENGTH("))
                                .ElseIf(Dbms.PostgreSql, b => b.Q("length("))
                                .ElseThrow()
                                    .C("PlaceholdersInfo").Q(")").RequireComma()
                                .C("PlaceholdersInfo")
                            .From("FileTemplates").NoLock()
                            .Where().C("ID").Equals().P("ID")
                                .And().C("PlaceholdersInfo").IsNotNull()
                            .Limit(1)
                            .Build(),
                        db.Parameter("ID", this.TemplateID))
                    .LogCommand();

                await using DbDataReader reader = await db.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    int length = (int)reader.GetInt64(0);

                    byte[] data = new byte[length];
                    reader.GetBytes(1, 0, data, 0, length);

                    Dictionary<string, object> dictionary = data.ToSerializable().GetStorage();
                    if (dictionary.TryGetValue(nameof(WordPlaceholderDocument), out var listObj)
                        && listObj is List<object> list)
                    {
                        foreach (Dictionary<string, object> values in list)
                        {
                            if (values.Count == 0)
                            {
                                continue;
                            }

                            KeyValuePair<string, object> firstValue = values.First();
                            string text = firstValue.Key;
                            string value = PlaceholderHelper.TryGetValue(text);
                            if (value != null && firstValue.Value != null)
                            {
                                var newPlaceholder = new PlaceholderText(text, value);
                                newPlaceholder.Info[OpenXmlHelper.PositionField] = firstValue.Value;
                                newPlaceholder.Info[OpenXmlHelper.IndexField] = values.TryGet<int>(OpenXmlHelper.IndexField);
                                newPlaceholder.Info[OpenXmlHelper.OrderField] = values.TryGet<int>(OpenXmlHelper.OrderField);

                                placeholders.Add(newPlaceholder);
                            }
                        }
                    }

                    if (dictionary.TryGetValue(nameof(TableGroup), out var bookmarkListObj)
                        && bookmarkListObj is List<object> bookmarkList)
                    {
                        foreach (Dictionary<string, object> bookmarkStorage in bookmarkList)
                        {
                            var bookmarkTable = new TableGroup(bookmarkStorage);
                            this.tableGroups.Add(bookmarkTable);
                        }
                    }
                }
            }

            return placeholders;
        }

        /// <summary>
        /// Метод для инициализации документа
        /// </summary>
        /// <returns>Возвращает инициализированный документ</returns>
        protected override OpenXmlPackage InitDocument()
        {
            wordDocument = WordprocessingDocument.Open(Stream, true);

            if (WithExtensions)
            {
                extensionContext.Document = wordDocument;
            }

            return wordDocument;
        }

        /// <summary>
        /// Метод для получения плейсхолдеров из объекта документа
        /// </summary>
        /// <returns>Возвращает список плейсхолдеров, найденных в документе</returns>
        protected override List<IPlaceholderText> GetPlaceholdersFromDocument()
        {
            var result = GetPlaceholdersFromPart(wordDocument.MainDocumentPart);

            return result;
        }
        /// <summary>
        /// Метод для подготовки документа к сохранению
        /// </summary>
        protected override void PrepareDocumentForSave()
        {
        }

        /// <summary>
        /// Метод для сохранения инициализированного документа
        /// </summary>
        protected override void SaveDocument()
        {
            wordDocument.Close();
            wordDocument = null;
        }

        /// <summary>
        /// Метод для сохранения информации о плейсхолдерах документа в базу данных
        /// </summary>
        /// <param name="dbScope">Объект dbScope текущего подключения к базе данных</param>
        /// <param name="placeholders">Список плейсхолдеров для сохранения</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        protected override async Task SavePlaceholdersInDatabaseAsync(
            IDbScope dbScope,
            List<IPlaceholderText> placeholders,
            CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                List<object> list = new List<object>(placeholders.Count);
                List<object> bookmarkList = new List<object>(tableGroups.Count);
                Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [nameof(WordPlaceholderDocument)] = list,
                    [nameof(TableGroup)] = bookmarkList,
                };

                foreach (IPlaceholderText placeholder in placeholders)
                {
                    var pDictionary = new Dictionary<string, object>(StringComparer.Ordinal)
                        {
                            { placeholder.Text, placeholder.Info[OpenXmlHelper.PositionField] },
                            { OpenXmlHelper.IndexField, placeholder.Info[OpenXmlHelper.IndexField] },
                            { OpenXmlHelper.OrderField, placeholder.Info.TryGet<int>(OpenXmlHelper.OrderField) }
                        };

                    list.Add(pDictionary);
                }
                tableGroups.RemoveAll(info => info.EndPosition == null);
                foreach (var info in tableGroups)
                {
                    if (info.EndPosition != null)
                    {
                        bookmarkList.Add(info.GetStorage());
                    }
                }

                byte[] data = dictionary.ToSerializable().Serialize();

                var executor = dbScope.Executor;
                var builderFactory = dbScope.BuilderFactory;

                await executor
                    .ExecuteNonQueryAsync(
                        builderFactory
                            .Update("FileTemplates").C("PlaceholdersInfo").Assign().P("Data")
                            .Where().C("ID").Equals().P("ID")
                            .Build(),
                        cancellationToken,
                        executor.Parameter("ID", this.TemplateID),
                        executor.Parameter("Data", data));
            }
        }

        /// <summary>
        /// Производит замену плейсхолдеров типа Field в Word документе
        /// </summary>
        /// <param name="context">Контекст операции замены плейсхолдеров</param>
        /// <returns>Возвращает true, если в документе производились изменения</returns>
        protected override async Task<bool> ReplaceFieldPlaceholdersAsync(IPlaceholderReplacementContext context)
        {
            bool hasChanges = false;

            foreach (IGrouping<string, IPlaceholderReplacement> replacements in
                context.Replacements
                    .GroupBy(x => TextPosition(x.Placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField)))
                    .OrderByDescending(x => x.Key))
            {
                OpenXmlElement paragraph = wordDocument.MainDocumentPart.Document;
                OpenXmlPart part = wordDocument.MainDocumentPart;
                List<object> position = replacements.First().Placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField);

                if (position[0].ToString() == Hyperlink)
                {
                    if (position.Count > 1)
                    {
                        await ReplaceElementsInRelationshipsAsync(part, position[1] as string, context.CancellationToken, replacements.ToArray());
                    }
                }
                else
                {
                    for (int i = 0; i < position.Count; i++)
                    {
                        int index = (int)position[i];
                        if (index < 0)
                        {
                            part = part.Parts.ToArray()[~index].OpenXmlPart;
                            paragraph = part.RootElement;
                        }
                        else
                        {
                            paragraph = paragraph.ChildElements[index];
                        }
                    }

                    Type elemType = paragraph.GetType();
                    if (elemType == typeof(Paragraph) || elemType == typeof(Hyperlink))
                    {
                        await ReplaceElementsInCompositeElementAsync(paragraph, context.CancellationToken, replacements.ToArray());
                    }
                    if (elemType == typeof(FieldCode))
                    {
                        // ReSharper disable once PossibleInvalidCastException
                        await ReplaceElementsInFieldCodeAsync((FieldCode)paragraph, context.CancellationToken, replacements.ToArray());
                    }
                }
                hasChanges = true;
            }

            return hasChanges;
        }

        /// <summary>
        /// Производит замену плейсхолдеров типа Table в Word документе
        /// </summary>
        /// <param name="context">Контекст операции замены плейсхолдеров</param>
        /// <returns>Возвращает true, если в документе производились изменения</returns>
        protected override async Task<bool> ReplaceTablePlaceholdersAsync(IPlaceholderReplacementContext context)
        {
            IList<IPlaceholder> tablePlaceholders = context.TablePlaceholders;
            if (tablePlaceholders.Count == 0)
            {
                return false;
            }
            bool hasChanges = false;

            FiilTableGroups();
            var tables = FillTableRelationships(tableGroups);

            PrepareTablePlaceholders(
                tables,
                tablePlaceholders,
                out IList<IPlaceholder> relationshipsPlaceholders);

            // Для каждой таблицы организуем получение данных и производим замену
            foreach (var tableGroup in tables)
            {
                hasChanges = true;
                var currentTableGroup = tableGroup;

                // Если тип группы - таблица, то в качестве обрабатываемой группы берем вложенную в нее.
                if (currentTableGroup.GroupType == TableGroupType.Table)
                {
                    currentTableGroup = currentTableGroup.InnerTableGroup;

                    // Если вложенной группы нет, то удаляем таблицо целиком
                    if (currentTableGroup == null)
                    {
                        foreach (var element in tableGroup.BaseElements)
                        {
                            element.SafeRemove();
                        }
                        continue;
                    }
                }

                // Наполняем таблицу данными
                List<IPlaceholder> rowPlaceholders = currentTableGroup.TablePlaceholders.ToList();

                var tableElement = currentTableGroup.TableElement;

                foreach (var element in currentTableGroup.BaseElements)
                {
                    // Дополняем список плейсхолдеров строки плейсхолдерами из Relationships
                    rowPlaceholders.AddRange(CheckForRelationshipsPlaceholders(element, relationshipsPlaceholders));
                }

                if (currentTableGroup.GroupType == TableGroupType.Group
                    && currentTableGroup.InnerTableGroup != null
                    && currentTableGroup.InnerTableGroup.GroupType == TableGroupType.Row)
                {
                    rowPlaceholders.AddRange(currentTableGroup.InnerTableGroup.TablePlaceholders);

                    foreach (var element in currentTableGroup.InnerTableGroup.BaseElements)
                    {
                        // Дополняем список плейсхолдеров строки плейсхолдерами из Relationships
                        rowPlaceholders.AddRange(CheckForRelationshipsPlaceholders(element, relationshipsPlaceholders));
                    }
                }

                // Сортируем плейсхолдеры по позициям параграфов (слева направо). Если позиции равны, то сортируем по Order внутри параграфа
                rowPlaceholders.Sort((x, y) =>
                {
                    int result = string.Compare(
                        TextPosition(x.Info.Get<List<object>>(OpenXmlHelper.PositionField)),
                        TextPosition(y.Info.Get<List<object>>(OpenXmlHelper.PositionField)),
                        StringComparison.Ordinal);

                    if (result == 0)
                    {
                        return x.Info.TryGet<int>(OpenXmlHelper.OrderField).CompareTo(y.Info.TryGet<int>(OpenXmlHelper.OrderField));
                    }

                    return result;
                });

                // загружаем данные из базы данных
                IEditablePlaceholderTable table = null;
                foreach (var placeholder in rowPlaceholders)
                {
                    table = await ((ITablePlaceholderType)placeholder.Type)
                        .FillTableAsync(context, placeholder, table, cancellationToken: context.CancellationToken);
                }

                // если в таблице нет строк, то переходим к следующему шаблону строки
                if (table == null || table.Rows.Count == 0)
                {
                    foreach (var element in tableGroup.BaseElements)
                    {
                        element.SafeRemove();
                    }
                    continue;
                }

                if (WithExtensions)
                {
                    extensionContext.Table = table;
                    extensionContext.TableElement = tableElement;
                    await BeforeTableReplaceAsync(context);
                }

                if (currentTableGroup.GroupType == TableGroupType.Group)
                {
                    // Производим замену значений в плейсхолдерах
                    int number = 0;

                    var rowTableGroup = currentTableGroup.InnerTableGroup;
                    var groupPlaceholders = currentTableGroup.TablePlaceholders;

                    foreach (var placeholder in groupPlaceholders)
                    {
                        table.AddHorizontalGroupPlaceholder(placeholder);
                    }
                    // Построчно обрабатываем группирующую таблицу
                    // Для плейсхолдеров в группирующей таблице
                    await table.FillHorizontalGroupsAsync(context, context.CancellationToken);

                    var lastGroupElement = currentTableGroup.BaseElements[currentTableGroup.BaseElements.Count - 1];
                    var insertGroupBeforeElement = lastGroupElement.NextSibling();
                    var parentGroupElement = lastGroupElement.Parent;

                    var lastElementOriginal = rowTableGroup?.BaseElements[rowTableGroup.BaseElements.Count - 1];
                    var lastElementOriginalType = lastElementOriginal?.GetType();

                    // Удаляем старые строки таблицы
                    foreach (var tableRow in currentTableGroup.BaseElements)
                    {
                        tableRow.SafeRemove();
                    }

                    foreach (IGrouping<PlaceholderValue, IPlaceholderRow> grouping
                        in table.Rows.GroupBy(x => x.HorizontalGroup))
                    {
                        OpenXmlElement insertBeforeElement = null;
                        OpenXmlElement parentElement = null;

                        var groupRow = grouping.First();

                        var groupRowElements =
                            CreateNewElements(
                                currentTableGroup,
                                insertGroupBeforeElement,
                                parentGroupElement);

                        if (rowTableGroup != null)
                        {
                            // Определяем, куда вставлять строки
                            for (int i = 0; i < groupRowElements.Count; i++)
                            {
                                var baseElement = currentTableGroup.BaseElements[i];
                                var newElement = groupRowElements[i];

                                var lastElement = GetRelativeElement(baseElement, lastElementOriginal, lastElementOriginalType, newElement);
                                if (lastElement != null)
                                {
                                    insertBeforeElement = lastElement.NextSibling();
                                    parentElement = lastElement.Parent;
                                    break;
                                }
                            }

                            // Удаляем оригинальные строки
                            foreach (var rowElement in rowTableGroup.BaseElements)
                            {
                                var rowElementType = rowElement.GetType();
                                for (int i = 0; i < groupRowElements.Count; i++)
                                {
                                    var baseElement = currentTableGroup.BaseElements[i];
                                    var newElement = groupRowElements[i];

                                    var relativeElement = GetRelativeElement(baseElement, rowElement, rowElementType, newElement);
                                    if (relativeElement != null)
                                    {
                                        relativeElement.SafeRemove();
                                        break;
                                    }
                                }
                            }
                        }

                        await ReplaceInNewElementsAsync(
                            context,
                            currentTableGroup,
                            groupRowElements,
                            groupRow,
                            true);

                        if (rowTableGroup != null
                            && (insertBeforeElement != null || parentElement != null))
                        {
                            foreach (IPlaceholderRow row in grouping)
                            {
                                row.Number = ++number;

                                await ReplaceInNewElementsAsync(
                                    context,
                                    rowTableGroup,
                                    CreateNewElements(
                                        rowTableGroup,
                                        insertBeforeElement,
                                        parentElement),
                                    row);
                            }
                        }
                    }

                    if (WithExtensions)
                    {
                        await AfterTableReplaceAsync(context);
                    }
                }
                else
                {
                    // Производим замену значений в плейсхолдерах
                    int number = 0;

                    var lastElement = currentTableGroup.BaseElements[currentTableGroup.BaseElements.Count - 1];
                    var insertBeforeElement = lastElement.NextSibling();
                    var parentElement = lastElement.Parent;

                    // Удаляем старые строки таблицы
                    foreach (var tableRow in currentTableGroup.BaseElements)
                    {
                        tableRow.SafeRemove();
                    }

                    // для каждой строки заменяем плейсхолдеры в строке-шаблоне rowText
                    foreach (IPlaceholderRow row in table.Rows)
                    {
                        row.Number = ++number;

                        await ReplaceInNewElementsAsync(
                            context,
                            currentTableGroup,
                            CreateNewElements(
                                currentTableGroup,
                                insertBeforeElement,
                                parentElement),
                            row);
                    }

                    if (WithExtensions)
                    {
                        await AfterTableReplaceAsync(context);
                    }
                }
            }

            ClearOldRelationships(wordDocument.MainDocumentPart, relationshipsPlaceholders);
            return hasChanges;
        }

        /// <summary>
        /// Метод для поиска плейсхолдеров внутри элемента документа
        /// </summary>
        /// <param name="baseElement">Элемент, в котором производится поиск плейсхолдеров</param>
        /// <param name="position">Позиция элемента в документе</param>
        /// <returns>Возвращает список плейсхолдеров, найденных в текущем элементе</returns>
        protected override IEnumerable<IPlaceholderText> GetPlaceholdersFromElementOverride(OpenXmlElement baseElement, List<object> position)
        {
            Type elementType = baseElement.GetType();
            if (elementType == typeof(Paragraph) || elementType == typeof(Hyperlink))
            {
                List<IPlaceholderText> result = new List<IPlaceholderText>();

                StringBuilder allText = StringBuilderHelper.Acquire();

                List<Text> textElements = GetTextElements(baseElement);
                if (textElements != null)
                {
                    foreach (Text element in textElements)
                    {
                        allText.Append(element.Text);
                    }
                }

                string allTextString = allText.ToStringAndRelease();
                OpenXmlHelper.AddPlaceholdersFromText(result, allTextString, position);
                return result;
            }
            else if (elementType == typeof(FieldCode))
            {
                List<IPlaceholderText> result = new List<IPlaceholderText>();
                string codeText = ((FieldCode)baseElement).Text;
                string decodedText = Uri.UnescapeDataString(codeText);

                OpenXmlHelper.AddPlaceholdersFromText(result, decodedText, position, hasOrder: false);
                return result;
            }
            else if (baseElement is BookmarkStart bookStart)
            {
                var bookName = bookStart.Name.Value;
                if (bookName.Length < 3)
                {
                    return EmptyHolder<IPlaceholderText>.Array;
                }

                TableGroupType? type = null;
                switch (bookName.Substring(0, 2))
                {
                    case "r_":
                        type = TableGroupType.Row;
                        break;
                    case "g_":
                        type = TableGroupType.Group;
                        break;
                    case "t_":
                        type = TableGroupType.Table;
                        break;
                }

                if (type.HasValue)
                {
                    tableGroups.Add(new TableGroup()
                    {
                        GroupType = type.Value,
                        StartIndex = OpenXmlHelper.GetIndex(bookStart),
                        StartPosition = position.ToList(),
                        ID = bookStart.Id,
                        Name = bookStart.Name,
                    });
                }
            }
            else if (baseElement is BookmarkEnd bookEnd)
            {
                var info = tableGroups.FirstOrDefault(x => bookEnd.Id.Value == x.ID);
                if (info != null)
                {
                    info.EndPosition = position.ToList();
                    info.EndIndex = OpenXmlHelper.GetIndex(bookEnd);
                }
            }

            return EmptyHolder<IPlaceholderText>.Array;
        }

        /// <summary>
        /// Метод, определяющий правила замены текста в элементе документа
        /// </summary>
        /// <param name="firstBaseElement">Первый базовый элемент, в котором производится замена текста. Обычно это Run или сам Text</param>
        /// <param name="firstTextElement">Первый объект текста для замены</param>
        /// <param name="lastBaseElement">Последний базовый элемент, в котором производится замена текста. Обычно это Run или сам Text</param>
        /// <param name="lastTextElement">Последний элемент текста для замены</param>
        /// <param name="newText">Текст, на который выполняется замена</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        protected override async Task ReplaceTextAsync(
            OpenXmlElement firstBaseElement,
            OpenXmlLeafTextElement firstTextElement,
            OpenXmlElement lastBaseElement,
            OpenXmlLeafTextElement lastTextElement,
            string newText,
            CancellationToken cancellationToken = default)
        {
            newText = RemoveInvalidChars(newText);

            string[] textRows = newText.Replace(Environment.NewLine, "\n").Split('\n');

            var text = (Text)firstTextElement;
            text.Text = textRows[0];
            text.Space = SpaceProcessingModeValues.Preserve;

            if (lastTextElement != null)
            {
                var lastText = (Text)lastTextElement;
                if (string.IsNullOrEmpty(lastText.Text))
                {
                    lastBaseElement.SafeRemove();
                }
                else
                {
                    lastText.Space = SpaceProcessingModeValues.Preserve;
                }
            }

            if (WithExtensions
                && firstBaseElement is Run run)
            {
                extensionContext.PlaceholderElements.Add(run);
            }

            for (int i = textRows.Length - 1; i > 0; i--)
            {
                if (string.IsNullOrEmpty(textRows[i]))
                {
                    var nextRow = new Run(new Break());
                    if (WithExtensions)
                    {
                        extensionContext.PlaceholderElements.Add(nextRow);
                    }

                    firstBaseElement.InsertAfterSelf(nextRow);
                }
                else
                {
                    OpenXmlElement nextElement = firstBaseElement.CloneNode(true);

                    Text nextTextElement = (Text)GetTextElement(nextElement);
                    nextTextElement.Text = textRows[i];
                    firstBaseElement.InsertAfterSelf(nextElement);

                    if (WithExtensions
                        && nextElement is Run nextRun)
                    {
                        extensionContext.PlaceholderElements.Add(nextRun);
                    }

                    // Т.к. внутри объекта Run может хранится и Text и Break одновременно, то при наличии Break, добавлять новый Break не нужно
                    if (!firstBaseElement.Descendants<Break>().Any())
                    {
                        var nextRow = new Run(new Break());
                        if (WithExtensions)
                        {
                            extensionContext.PlaceholderElements.Add(nextRow);
                        }

                        firstBaseElement.InsertAfterSelf(nextRow);
                    }
                }
            }

            if (WithExtensions)
            {
                await AfterPlaceholderReplaceAsync(extensionContext.ReplacementContext);
                extensionContext.PlaceholderElements.Clear();
            }
        }

        /// <summary>
        /// Метод, определяющий правила замены изображения в элементе документа.
        /// Возвращает признак того, что замена выполнена успешно.
        /// </summary>
        /// <param name="baseElement">Базовый элемент, в котором производится замена текста</param>
        /// <param name="replacement">Значение с изображением, которое требуется заменить</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Признак того, что замена выполнена успешно.</returns>
        protected override async Task<bool> ReplaceImageAsync(
            OpenXmlElement baseElement,
            IPlaceholderReplacement replacement,
            CancellationToken cancellationToken = default)
        {
            var graphicBase = OpenXmlHelper.FindInParentChildren<A.Graphic>(baseElement)?.Parent;
            if (graphicBase == null)
            {
                return false;
            }

            PlaceholderValue value = replacement.NewValue;
            byte[] data = value.Data;

            if (data == null || data.Length == 0)
            {
                // пустое изображение при замене удаляет надпись
                var run = OpenXmlHelper.FindParent<Run>(graphicBase);
                if (run != null)
                {
                    run.SafeRemove();
                    return true;
                }

                return false;
            }

            IPlaceholderImageParameters imageParameters = value.FormatResult.GetImageParameters();
            ImagePartType imagePartType = OpenXmlHelper.GetImagePartType(imageParameters);

            var existingGraphic = graphicBase.GetFirstChild<A.Graphic>();
            var existingExtent = graphicBase.GetFirstChild<DW.Extent>();
            var existingProperties = existingGraphic?.GraphicData?.Descendants<WPS.ShapeProperties>().FirstOrDefault();

            A.Graphic graphic = this.CreateGraphicFromImage(
                data,
                imagePartType,
                existingProperties,
                existingExtent,
                imageParameters.Width,
                imageParameters.Height,
                imageParameters.Reformat);

            if (existingGraphic != null)
            {
                existingGraphic.Remove();
            }

            graphicBase.AppendChild(graphic);

            // замещающий текст
            var docProperties = graphicBase.GetFirstChild<DW.DocProperties>();
            if (docProperties != null)
            {
                docProperties.Title = LocalizationManager.Format(imageParameters.AlternativeText);
            }

            // теперь магия, если надпись была на якоре
            if (graphicBase is DW.Anchor anchor)
            {
                // обнуляем свойства distT="0" distB="0" distL="0" distR="0"
                anchor.DistanceFromTop = 0u;
                anchor.DistanceFromBottom = 0u;
                anchor.DistanceFromLeft = 0u;
                anchor.DistanceFromRight = 0u;

                // удаляем детей <wp14:sizeRelH/> и <wp14:sizeRelV/>
                anchor.RemoveAllChildren<Wp14.RelativeWidth>();
                anchor.RemoveAllChildren<Wp14.RelativeHeight>();
            }

            var alternateContent = OpenXmlHelper.FindParent<AlternateContent>(graphicBase);
            if (alternateContent != null)
            {
                // мы внутри т.н. альтернативного контента, где есть наша надпись/картинка,
                // а ещё некое Fallback-значение, которое мы хотим выпилить, потому что для картинок оно бесполезно,
                // и Word по умолчанию его не генерит

                OpenXmlElement alternateContentBase = alternateContent.Parent;
                OpenXmlElement choiceChild;

                if (alternateContentBase != null
                    && (choiceChild = alternateContent.GetFirstChild<AlternateContentChoice>()?.FirstChild) != null)
                {
                    // было: alternateContentBase -> alternateContent -> choice -> choiceChild -> ... -> graphicBase -> graphic
                    // нужно: alternateContentBase -> choiceChild -> ... -> graphicBase -> graphic

                    choiceChild.Remove();
                    alternateContent.Remove();

                    alternateContentBase.AppendChild(choiceChild);
                }
            }

            return true;
        }

        protected override void CleanAfterChanges()
        {
            // исправляем объекты "Надпись": идентификаторы DocProperties для всех строк должны быть уникальны
            foreach (DW.DocProperties docProperties
                in this.wordDocument.MainDocumentPart.Document.Descendants<DW.DocProperties>())
            {
                docProperties.Id = this.docPropertiesNextId++;
            }
        }

        protected override bool CheckTextElement(OpenXmlLeafTextElement baseElementText)
        {
            return !(baseElementText is FieldCode);
        }

        #endregion
    }
}
