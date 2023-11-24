using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Placeholders;
using Tessa.Platform.Placeholders.Extensions;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using A = DocumentFormat.OpenXml.Drawing;
using Hyperlink = DocumentFormat.OpenXml.Spreadsheet.Hyperlink;
using Text = DocumentFormat.OpenXml.Spreadsheet.Text;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Объект, определяющий способы хранения и изменения текста с заменяемыми плейсхолдерами
    /// для документа Excel.
    /// </summary>
    public sealed class ExcelPlaceholderDocument :
        OpenXmlPlaceholderDocument
    {
        #region Nested Types

        /// <summary>
        /// Класс с вспомогательными методами для обработки Excel
        /// </summary>
        private static class ExcelHelper
        {
            #region Private Fields

            /// <summary>
            /// Regex шаблон для строк
            /// </summary>
            private static readonly Regex rowPattern = new Regex(@"[^\d+]");

            /// <summary>
            /// Regex шаблон для столбцов
            /// </summary>
            private static readonly Regex columnPattern = new Regex(@"[^A-Z]+");

            #endregion

            #region Public Fields and Constants

            /// <summary>
            /// Пустой список плейсхолдеров
            /// </summary>
            public static readonly IList<IPlaceholder> EmptyPlaceholders = EmptyHolder<IPlaceholder>.Collection;

            /// <summary>
            /// Настройки форматирования при переводе значения double в строку, обозначающюю дату/время в Excel
            /// </summary>
            public static readonly NumberFormatInfo DoubleExcelFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };

            /// <summary>
            /// Имя первой колонки в Excel
            /// </summary>
            public const string FirstColumn = "A";

            /// <summary>
            /// Имя последней колонки в Excel
            /// </summary>
            public const string LastColumn = "XFD";

            #endregion

            #region Static Methods

            /// <summary>
            /// Производит действие при отсутствии доступа к операции
            /// </summary>
            /// <param name="operation">Название операции или метода</param>
            /// <param name="objectType">Тип объекта</param>
            public static void NotSupported(string operation, string objectType)
            {
                throw new NotSupportedException(
                    string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_OperationNotSupported"),
                        operation,
                        objectType));
            }

            /// <summary>
            /// Производит сравнение значений колонок Excel между собой
            /// </summary>
            /// <param name="x">Имя первой колонки</param>
            /// <param name="y">Имя второй колонки</param>
            /// <returns>Возвращает положительное число, если x > y, отрицаительное, если y > x и 0, если они равны</returns>
            public static int Compare(string x, string y)
            {
                if (x.Length > y.Length)
                {
                    return 1;
                }
                if (x.Length < y.Length)
                {
                    return -1;
                }
                return string.CompareOrdinal(x, y);
            }

            /// <summary>
            /// Метод для получения номера строки из ссылки ячейки
            /// </summary>
            /// <param name="reference">Ссылка ячейки</param>
            /// <returns>Возвращает номер строки переданной ссылки</returns>
            public static int GetRowIndex(string reference)
            {
                return int.Parse(rowPattern.Replace(reference, string.Empty));
            }

            /// <summary>
            /// Метод для получения имени колонки из ссылки ячейки
            /// </summary>
            /// <param name="reference">Ссылка ячейки</param>
            /// <returns>Возвращает имя колонки переданной ссылки</returns>
            public static string GetColumnIndex(string reference)
            {
                return columnPattern.Replace(reference, string.Empty);
            }

            /// <summary>
            /// Возвращает имя колонки в Excel по отсчитываемому от нуля индексу.
            /// Например: <c>0 = A, 1 = B, 2 = C, ..., 26 = AA, 27 = AB, ...</c>
            /// </summary>
            /// <param name="index">Отсчитываемый от нуля индекс колонки.</param>
            /// <returns>Строка с именем колонки в Excel, соответствующая заданному индексу.</returns>
            public static string GetColumnName(int index)
            {
                const byte alphabetLength = 'Z' - 'A' + 1; // 26

                string name = string.Empty;

                while (index >= 0)
                {
                    name = Convert.ToChar('A' + index % alphabetLength) + name;
                    index = index / alphabetLength - 1;
                }

                return name;
            }

            #endregion
        }

        /// <summary>
        /// Класс, описывающий общие свойства хранилищ элементов Excel
        /// </summary>
        /// <typeparam name="TElement">Тип хранимого элемента Excel</typeparam>
        private abstract class ElementBase<TElement> where TElement : OpenXmlElement
        {
            #region Fields

            /// <summary>
            /// Хранимый элемент Excel
            /// </summary>
            private readonly TElement element;

            /// <summary>
            /// Параметр, определяющий на какое число нужно переместить элемент при обновлении
            /// </summary>
            protected int MoveBy;

            #endregion

            #region Constructors

            protected ElementBase(TElement element)
            {
                this.element = element;

                // ReSharper disable once VirtualMemberCallInConstructor
                this.ParseElement();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Хранимый элемент Excel
            /// </summary>
            public TElement Element { get { return element; } }

            #endregion

            #region Abstract And Virtual Methods

            /// <summary>
            /// Метод для клонирования хранимого элемента и вставки его перед передаваемым элементом, или перед клонируемым, если параметр отсутствует
            /// </summary>
            /// <param name="element">Элемент, перед которым производится вставка текущего элемента</param>
            /// <returns>Возвращает новый объект хранилища с клонируемым элементом Excel</returns>
            public abstract ElementBase<TElement> CloneAndInsertBefore(OpenXmlElement element = null);

            /// <summary>
            /// Метод для получения основных данных из хранимого элемента
            /// </summary>
            protected abstract void ParseElement();

            /// <summary>
            /// Метод для удаления объекта и его элемента
            /// </summary>
            public virtual void Remove()
            {
                if (Element.Parent != null)
                    Element.Remove();
            }

            /// <summary>
            /// Метод для обновления позиции элемента в документе Excel
            /// </summary>
            public abstract void Update();

            #endregion

            #region Protected Methods

            /// <summary>
            /// Метод для клонирования хранимого элемента
            /// </summary>
            /// <returns>Возвращает полный клон хранимого элемента</returns>
            protected TElement CloneElement()
            {
                return (TElement)Element.CloneNode(true);
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Метод для перемещения элемента внутри документа Excel на заданное число. Фактическое обновление позиции элемента в документе производится методом Update
            /// </summary>
            /// <param name="moveBy">Число, на которое требуется переместить хранимый элемент</param>
            public void Move(int moveBy)
            {
                this.MoveBy += moveBy;
            }

            #endregion
        }

        /// <summary>
        /// Класс-хранилище для упрощенной работы с элементои типа Worksheet
        /// </summary>
        private sealed class WorksheetElement : ElementBase<Worksheet>
        {
            #region Constructors

            public WorksheetElement(Worksheet worksheet, string name)
                : base(worksheet)
            {
                this.Name = name;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Имя элемента
            /// </summary>
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            public string Name { get; private set; }

            /// <summary>
            /// Список объектов смерженных ячеек, хранимых в данном элементе
            /// </summary>
            public List<MergeCellGroup> MergeCells { get; private set; }

            /// <summary>
            /// Список объектов гиперссылок, хранимых в данном элементе
            /// </summary>
            public List<HyperlinkCellGroup> Hyperlinks { get; private set; }

            /// <summary>
            /// Список объектов строк, хранимых в данном элементе
            /// </summary>
            public List<RowCellGroup> Rows { get; private set; }

            /// <summary>
            /// Список объектов-якорей, используемых для привязки надписей и картинок к данному элементу
            /// </summary>
            public List<AnchorCellGroup> Anchors { get; private set; }

            /// <summary>
            /// Список всех таблиц в текущем элементе
            /// </summary>
            public List<TableGroup> Tables { get; private set; }

            #endregion

            #region Base Overrides

            /// <summary>
            /// Метод для получения основных данных из элемента Worksheet
            /// </summary>
            protected override void ParseElement()
            {
                InitializeHyperlinks();
                InitializeMergeCells();
                InitializeRows();
                InitializeAnchors();
                InitializeTables();
            }

            /// <summary>
            /// Клонирование элемента типа Worksheet недоступно
            /// </summary>
            /// <param name="element"></param>
            /// <returns></returns>
            public override ElementBase<Worksheet> CloneAndInsertBefore(OpenXmlElement element = null)
            {
                ExcelHelper.NotSupported(nameof(CloneAndInsertBefore), nameof(WorksheetElement));
                return null;
            }

            /// <summary>
            /// Удаление элемента типа Worksheet недоступно
            /// </summary>
            public override void Remove()
            {
                ExcelHelper.NotSupported(nameof(Remove), nameof(WorksheetElement));
            }

            /// <summary>
            /// Производит обновление позиций всех строк, объединенных ячеек и гиперссылок в текущем Worksheet
            /// </summary>
            public override void Update()
            {
                Rows.ForEach(x => x.Update());
                MergeCells.ForEach(x => x.Update());
                Hyperlinks.ForEach(x => x.Update());
                Anchors.ForEach(x => x.Update());

                // Только строки добавляем в конце, для остальных элементов порядок не важен
                SheetData rowData = Element.Elements<SheetData>().FirstOrDefault();
                rowData.RemoveAllChildren();

                foreach(var row in Rows.OrderBy(x => x.Top))
                {
                    rowData.AppendChild(row.Element);
                }
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Производит инициализацию объектов гиперссылок
            /// </summary>
            private void InitializeHyperlinks()
            {
                Hyperlinks = new List<HyperlinkCellGroup>();
                Hyperlinks hyperlinks = Element.Elements<Hyperlinks>().FirstOrDefault();
                if (hyperlinks == null)
                {
                    return;
                }
                foreach (var element in hyperlinks)
                {
                    var hyperlink = (Hyperlink)element;

                    HyperlinkCellGroup group = new HyperlinkCellGroup(hyperlink, this);
                    Hyperlinks.Add(group);
                }
            }

            /// <summary>
            /// Производит инициализацию объектов смерженных ячеек
            /// </summary>
            private void InitializeMergeCells()
            {
                MergeCells = new List<MergeCellGroup>();
                MergeCells mergeCells = Element.Elements<MergeCells>().FirstOrDefault();
                if (mergeCells == null)
                {
                    return;
                }
                foreach (var element in mergeCells)
                {
                    var mergeCell = (MergeCell)element;

                    MergeCellGroup group = new MergeCellGroup(mergeCell, this);
                    MergeCells.Add(group);
                }
            }

            /// <summary>
            /// Производит инициализацию объектов строк
            /// </summary>
            private void InitializeRows()
            {
                Rows = new List<RowCellGroup>();
                SheetData rowData = Element.Elements<SheetData>().FirstOrDefault();
                if (rowData == null)
                {
                    return;
                }
                foreach (Row row in rowData.Elements<Row>())
                {
                    RowCellGroup rowElement = new RowCellGroup(row, this);
                    Rows.Add(rowElement);
                }
            }

            /// <summary>
            /// Производит инициализацию объектов якорей
            /// </summary>
            private void InitializeAnchors()
            {
                Anchors = new List<AnchorCellGroup>();

                var anchors = Element.WorksheetPart.DrawingsPart?.WorksheetDrawing?.Elements<Xdr.TwoCellAnchor>();
                if (anchors != null)
                {
                    foreach (Xdr.TwoCellAnchor anchor in anchors)
                    {
                        var group = new AnchorCellGroup(anchor, this);
                        Anchors.Add(group);
                    }
                }
            }

            /// <summary>
            /// Производит инициализацию списка таблиц
            /// </summary>
            private void InitializeTables()
            {
                Tables = new List<TableGroup>();
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Производит перемещение всех элементов данного Worksheet, начиная с moveFrom строки (не включая ее) на moveBy
            /// </summary>
            /// <param name="moveBy">Число, на которое необходимо переместить элементы</param>
            /// <param name="moveFrom">Номер строки, начиная с которой производится перемещение элементов</param>
            public void Move(int moveBy, int moveFrom)
            {
                foreach (RowCellGroup row in Rows.Where(x => x.Bottom > moveFrom))
                {
                    row.Move(moveBy);
                }
                foreach (HyperlinkCellGroup hyp in Hyperlinks.Where(x => x.Bottom > moveFrom))
                {
                    hyp.Move(moveBy);
                }
                foreach (MergeCellGroup mc in MergeCells.Where(x => x.Bottom > moveFrom))
                {
                    mc.Move(moveBy);
                }
                foreach (AnchorCellGroup anchor in Anchors.Where(x => x.Bottom > moveFrom))
                {
                    anchor.Move(moveBy);
                }
            }

            /// <summary>
            /// Метод для получения строки по ее номеру
            /// </summary>
            /// <param name="rowIndex">Номер строки</param>
            /// <returns>Возвращает первую найденную строку, или null, если таких строк нет</returns>
            public RowCellGroup GetRow(int rowIndex)
            {
                return Rows.FirstOrDefault(x => x.Top == rowIndex);
            }

            #endregion
        }

        /// <summary>
        /// Класс, определяющий общие свойства объектов, хранимых на базе Worksheet
        /// </summary>
        /// <typeparam name="TElement">Тип хранимого элемента Excel</typeparam>
        private abstract class WorksheetBase<TElement> : ElementBase<TElement> where TElement : OpenXmlElement
        {
            #region Fields

            /// <summary>
            /// Объект Worksheet, на базе которого хранится текущий объект
            /// </summary>
            private WorksheetElement worksheet;

            #endregion

            #region Constructors

            protected WorksheetBase(TElement element)
                : base(element)
            {
            }

            protected WorksheetBase(TElement element, WorksheetElement worksheet)
                : base(element)
            {
                this.worksheet = worksheet;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Объект Worksheet, на базе которого хранится текущий объект
            /// </summary>
            public WorksheetElement Worksheet
            {
                get { return worksheet; }
                protected set { worksheet = value; }
            }

            #endregion
        }

        /// <summary>
        /// Интерфейс для объектов, являющихся хранилищем для одной или нескольких ячеек
        /// </summary>
        private interface ICellsGroup
        {
            /// <summary>
            /// Свойство, определяющее, состоит ли объект из одной ячейки
            /// </summary>
            bool IsSingleCell { get; }

            /// <summary>
            /// Определяет верхнюю границу диапозона
            /// </summary>
            int Top { get; }

            /// <summary>
            /// Определяет нижнюю границу диапозона
            /// </summary>
            int Bottom { get; }

            /// <summary>
            /// Определяет левую границу диапозона
            /// </summary>
            string Left { get; }

            /// <summary>
            /// Определяет правую границу диапозона
            /// </summary>
            string Right { get; }

            /// <summary>
            /// Определяет высоту (количество строк) диапозона
            /// </summary>
            int Height { get; }

            /// <summary>
            /// Возвращает строковый вариант диапозона (например 'B2' для одной ячейки или 'B2:D4' для группы ячеек)
            /// </summary>
            string Reference { get; }
        }

        /// <summary>
        /// Класс для объектов, являющихся хранилищем для одной или нескольких ячеек, включающий в себя общие методы для работы с группой ячеек
        /// </summary>
        private abstract class CellsGroup<TElement> : WorksheetBase<TElement>, ICellsGroup where TElement : OpenXmlElement
        {
            #region Constructors

            protected CellsGroup(TElement element)
                : base(element)
            {
            }

            protected CellsGroup(TElement element, WorksheetElement worksheet)
                : base(element, worksheet)
            {
            }

            #endregion

            #region Properties

            /// <summary>
            /// Определяет верхнюю границу диапозона
            /// </summary>
            public int Top { get; protected set; }

            /// <summary>
            /// Определяет нижнюю границу диапозона
            /// </summary>
            public int Bottom { get; protected set; }

            /// <summary>
            /// Определяет левую границу диапозона
            /// </summary>
            public string Left { get; protected set; }

            /// <summary>
            /// Определяет правую границу диапозона
            /// </summary>
            public string Right { get; protected set; }

            /// <summary>
            /// Определяет высоту (количество строк) диапозона
            /// </summary>
            public int Height => Top - Bottom + 1;

            /// <summary>
            /// Свойство, определяющее, состоит ли объект из одной ячейки
            /// </summary>
            public bool IsSingleCell => Left == Right && Top == Bottom;

            /// <summary>
            /// Возвращает строковый вариант диапозона (например 'B2' для одной ячейки или 'B2:D4' для группы ячеек)
            /// </summary>
            public string Reference =>
                this.IsSingleCell
                    ? this.Left + this.Bottom
                    : this.Left + this.Bottom + ":" + this.Right + this.Top;

            #endregion

            #region Virtual Methods

            public virtual string GetDisplayString() => Reference;

            #endregion

            #region Methods

            /// <summary>
            /// Возвращает признак того, что среди наследников текущего элемента присутствует указанный элемент.
            /// </summary>
            /// <typeparam name="T">Тип искомого элемента. Объекты других типов игнорируются.</typeparam>
            /// <param name="element">Искомый элемент.</param>
            /// <returns>
            /// <c>true</c>, если среди наследников текущего элемента присутствует указанный элемент;
            /// <c>false</c> в противном случае.
            /// </returns>
            public bool HasChildElement<T>(T element)
                where T : OpenXmlElement
            {
                return Element.Descendants<T>().Contains(element);
            }

            /// <summary>
            /// Проверяет, включает ли данная группа в себя передаваемую ссылку.
            /// </summary>
            /// <param name="reference">Ссылка на ячейку или диапозон ячеек</param>
            /// <returns>Возвращает true, если текущая группа ячеек включает в себя группу ячеек по передаваемой ссылке</returns>
            public bool IsInclude(string reference)
            {
                if (reference.Contains(":"))
                {
                    return IsIncludeGroup(reference);
                }
                else
                {
                    string column = ExcelHelper.GetColumnIndex(reference);
                    int row = ExcelHelper.GetRowIndex(reference);
                    return IsIncludeCell(column, row);
                }
            }

            /// <summary>
            /// Проверяет, включает ли данная группа в себя передаваемую группу
            /// </summary>
            /// <param name="group">Передаваемая группа</param>
            /// <returns>Возвращает true, если текущая группа ячеек включает в себя передаваемую группу</returns>
            protected bool IsInclude(ICellsGroup group)
            {
                if (group.IsSingleCell)
                {
                    return IsIncludeCell(group.Left, group.Bottom);
                }
                else
                {
                    return IsIncludeGroup(group.Left, group.Right, group.Bottom, group.Top);
                }
            }

            /// <summary>
            /// Метод проверяет, входит ли передаваеммая группа ячеек в текущую группу
            /// </summary>
            /// <param name="reference">Ссылка на группу ячеек</param>
            /// <returns>Возвращает true, если передаваеммая группа ячеек входит в текущую группу</returns>
            private bool IsIncludeGroup(string reference)
            {
                string[] refs = reference.Split(':');
                if (refs.Length != 2)
                {
                    throw new ArgumentException("Wrong parameter '" + reference + "'", "reference");
                }
                string leftR = ExcelHelper.GetColumnIndex(refs[0]),
                    rightR = ExcelHelper.GetColumnIndex(refs[1]);
                int bottomR = ExcelHelper.GetRowIndex(refs[0]),
                    topR = ExcelHelper.GetRowIndex(refs[1]);

                return IsIncludeGroup(leftR, rightR, bottomR, topR);
            }

            /// <summary>
            /// Метод проверяет, входит ли передаваеммая группа ячеек в текущую группу
            /// </summary>
            /// <param name="leftR">Левая граница передаваемой группы</param>
            /// <param name="rightR">Правая граница передаваемой группы</param>
            /// <param name="bottomR">Нижняя граница передаваемой группы</param>
            /// <param name="topR">Верхняя граница передаваемой группы</param>
            /// <returns>Возвращает true, если передаваеммая группа ячеек входит в текущую группу</returns>
            private bool IsIncludeGroup(string leftR, string rightR, int bottomR, int topR)
            {
                return ExcelHelper.Compare(rightR, Right) <= 0
                    && ExcelHelper.Compare(leftR, Left) >= 0
                    && bottomR >= Bottom
                    && topR <= Top;
            }

            /// <summary>
            /// Метод проверяет, входит ли передаваеммая ячейка в текущую группу
            /// </summary>
            /// <param name="column">Имя колонки передаваемой ячейки</param>
            /// <param name="row">Номер строки передаваемой ячейки</param>
            /// <returns>Возвращает true, если передаваеммая ячейка входит в текущую группу</returns>
            private bool IsIncludeCell(string column, int row)
            {
                return ExcelHelper.Compare(column, Right) <= 0
                    && ExcelHelper.Compare(column, Left) >= 0
                    && row >= Bottom
                    && row <= Top;
                //column <= right && column >= left && row >= bottom && row <= top;
            }

            /// <summary>
            /// Метод проверяет, есть ли пересечения между текущей и передаваемой группами
            /// </summary>
            /// <param name="group">Группа ячеек</param>
            /// <returns>Возвращает true, если есть пересечение</returns>
            protected bool IsCrossed(ICellsGroup group)
            {
                return IsCrossed(group.Left, group.Right, group.Bottom, group.Top);
            }

            /// <summary>
            /// Метод проверяет, есть ли пересечения между текущей и передаваемой группами
            /// </summary>
            /// <param name="leftR">Левая граница передаваемой группы</param>
            /// <param name="rightR">Правая граница передаваемой группы</param>
            /// <param name="bottomR">Нижняя граница передаваемой группы</param>
            /// <param name="topR">Верхняя граница передаваемой группы</param>
            /// <returns>Возвращает true, если есть пересечение</returns>
            private bool IsCrossed(string leftR, string rightR, int bottomR, int topR)
            {
                return !(ExcelHelper.Compare(leftR, Right) > 0
                    || ExcelHelper.Compare(rightR, Left) < 0
                    || topR < Bottom
                    || bottomR > Top);
            }

            #endregion
        }

        /// <summary>
        /// Класс для работы с элементом строки Excel
        /// </summary>
        private sealed class RowCellGroup : CellsGroup<Row>
        {
            #region Constructors

            public RowCellGroup(Row row, WorksheetElement worksheet)
                : base(row, worksheet)
            {
            }

            #endregion

            #region Base Overrides

            /// <summary>
            /// Метод для получения основных данных из элемента строки
            /// </summary>
            protected override void ParseElement()
            {
                Top = Bottom = Convert.ToInt32(Element.RowIndex.Value);
                Left = ExcelHelper.FirstColumn;
                Right = ExcelHelper.LastColumn;
            }

            /// <summary>
            /// Метод для клонирования хранимого элемента и вставки его перед передаваемым элементом, или перед клонируемым, если параметр отсутствует
            /// </summary>
            /// <param name="element">Элемент, перед которым производится вставка текущего элемента</param>
            /// <returns></returns>
            public override ElementBase<Row> CloneAndInsertBefore(OpenXmlElement element = null)
            {
                RowCellGroup newRow = new RowCellGroup(CloneElement(), Worksheet);

                Worksheet.Rows.Add(newRow);
                return newRow;
            }

            /// <summary>
            /// Метод для удаления объекта и его элемента
            /// </summary>
            public override void Remove()
            {
                base.Remove();
                Worksheet.Rows.Remove(this);
            }

            /// <summary>
            /// Метод для обновления позиции элемента в документе Excel
            /// </summary>
            public override void Update()
            {
                if (this.MoveBy != 0)
                {
                    int oldIndex = Top;
                    int newIndex = oldIndex + this.MoveBy;
                    string newIndexS = newIndex.ToString(), oldIndexS = oldIndex.ToString();

                    foreach (Cell cell in Element.Elements<Cell>())
                    {
                        cell.CellReference.Value = cell.CellReference.Value.Replace(oldIndexS, newIndexS);
                    }
                    Element.RowIndex.Value = Convert.ToUInt32(newIndex);
                    Top = newIndex;
                    Bottom = newIndex;

                    this.MoveBy = 0;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Метод производит установку новой ячейки внутри строки. Если ячейка уже существуют, то он заменит ее на новую.
            /// </summary>
            /// <param name="newCell">Новый элемент ячейки</param>
            public void SetCell(Cell newCell)
            {
                string newCellColumn = ExcelHelper.GetColumnIndex(newCell.CellReference.Value);
                string newCellReference = newCellColumn + Top.ToString();
                newCell.CellReference = newCellReference;
                Cell oldCell;
                if ((oldCell = Element.Elements<Cell>().FirstOrDefault(x => x.CellReference.Value == newCellReference)) != null)
                {
                    Element.ReplaceChild(newCell, oldCell);
                }
                else
                {
                    Cell beforeCell = Element.Elements<Cell>().FirstOrDefault(x => ExcelHelper.Compare(newCellColumn, ExcelHelper.GetColumnIndex(x.CellReference.Value)) < 0);

                    if (beforeCell != null)
                    {
                        beforeCell.InsertBeforeSelf(newCell);
                    }
                    else
                    {
                        Element.AppendChild(newCell);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Класс для работы с элементом гиперссылки в Excel
        /// </summary>
        private sealed class HyperlinkCellGroup : CellsGroup<Hyperlink>
        {
            #region Constructors

            public HyperlinkCellGroup(Hyperlink hyperlink, WorksheetElement worksheet)
                : base(hyperlink, worksheet)
            {
            }

            #endregion

            #region Base Overrides

            /// <summary>
            /// Метод для получения основных данных элемента гиперссылка
            /// </summary>
            protected override void ParseElement()
            {
                string text = Element.Reference.Value;
                Left = ExcelHelper.GetColumnIndex(text);
                Right = Left;
                Bottom = ExcelHelper.GetRowIndex(text);
                Top = Bottom;
            }

            /// <summary>
            /// Метод для клонирования хранимого элемента и вставки его перед клонируемым элементом. Игнорирует передаваемый элемент
            /// </summary>
            /// <param name="element">Данный параметр игнорируется</param>
            /// <returns>Возвращает новый объект хранилища с клонируемым элементом Excel</returns>
            public override ElementBase<Hyperlink> CloneAndInsertBefore(OpenXmlElement element = null)
            {
                HyperlinkCellGroup newHyperlink = new HyperlinkCellGroup(CloneElement(), Worksheet);
                Worksheet.Hyperlinks.Add(newHyperlink);
                Element.Parent.AppendChild(newHyperlink.Element);
                return newHyperlink;
            }

            /// <summary>
            /// Метод для удаления объекта и его элемента
            /// </summary>
            public override void Remove()
            {
                base.Remove();
                Worksheet.Hyperlinks.Remove(this);
            }

            /// <summary>
            /// Метод для обновления позиции элемента в документе Excel
            /// </summary>
            public override void Update()
            {
                if (this.MoveBy != 0)
                {
                    Top = Top + this.MoveBy;
                    Bottom = Bottom + this.MoveBy;
                    Element.Reference.Value = Reference;
                    this.MoveBy = 0;
                }
            }

            #endregion
        }

        /// <summary>
        /// Класс для работы с элементом смерженные ячейкм в Excel
        /// </summary>
        private sealed class MergeCellGroup : CellsGroup<MergeCell>
        {
            #region Constructors

            public MergeCellGroup(MergeCell mergeCell, WorksheetElement worksheet)
                : base(mergeCell, worksheet)
            {
            }

            #endregion

            #region Base Overrides

            /// <summary>
            /// Метод для получения основных данных из элемента смерженные ячейкм
            /// </summary>
            protected override void ParseElement()
            {
                string text = Element.Reference.Value;
                string[] refs = text.Split(':');
                if (refs.Length != 2)
                {
                    throw new ArgumentException("Wrong parameter '" + text + "'", "reference");
                }
                Left = ExcelHelper.GetColumnIndex(refs[0]);
                Right = ExcelHelper.GetColumnIndex(refs[1]);
                Bottom = ExcelHelper.GetRowIndex(refs[0]);
                Top = ExcelHelper.GetRowIndex(refs[1]);
            }

            /// <summary>
            /// Метод для клонирования хранимого элемента и вставки его перед передаваемым элементом, или перед клонируемым, если параметр отсутствует
            /// </summary>
            /// <param name="element">Элемент, перед которым производится вставка текущего элемента</param>
            /// <returns>Возвращает новый объект хранилища с клонируемым элементом Excel</returns>
            public override ElementBase<MergeCell> CloneAndInsertBefore(OpenXmlElement element = null)
            {
                MergeCellGroup newMergeCellGroup = new MergeCellGroup(CloneElement(), Worksheet);
                Worksheet.MergeCells.Add(newMergeCellGroup);
                Element.Parent.AppendChild(newMergeCellGroup.Element);
                return newMergeCellGroup;
            }

            /// <summary>
            /// Метод для удаления объекта и его элемента
            /// </summary>
            public override void Remove()
            {
                base.Remove();
                Worksheet.MergeCells.Remove(this);
            }

            /// <summary>
            /// Метод для обновления позиции элемента в документе Excel
            /// </summary>
            public override void Update()
            {
                if (this.MoveBy != 0)
                {
                    Top = Top + this.MoveBy;
                    Bottom = Bottom + this.MoveBy;
                    Element.Reference.Value = Reference;
                    this.MoveBy = 0;
                }
            }

            #endregion
        }

        /// <summary>
        /// Якорь, используемый для привязки надписи или картинки к Worksheet
        /// </summary>
        private sealed class AnchorCellGroup : CellsGroup<Xdr.TwoCellAnchor>
        {
            #region Constructors

            public AnchorCellGroup(Xdr.TwoCellAnchor anchor, WorksheetElement worksheet)
                : base(anchor, worksheet)
            {
            }

            #endregion

            #region Base Overrides

            /// <summary>
            /// Метод для получения основных данных элемента "якорь"
            /// </summary>
            protected override void ParseElement()
            {
                // Top и Bottom должны отсчитываться от 1, а в маркере индекс отсчитывается от 0
                Xdr.FromMarker fromMarker = Element.FromMarker;
                Top = int.Parse(fromMarker.RowId.Text) + 1;
                Left = ExcelHelper.GetColumnName(int.Parse(fromMarker.ColumnId.Text));

                Xdr.ToMarker toMarker = Element.ToMarker;
                Bottom = int.Parse(toMarker.RowId.Text) + 1;
                Right = ExcelHelper.GetColumnName(int.Parse(toMarker.ColumnId.Text));
            }

            /// <summary>
            /// Метод для клонирования хранимого элемента и вставки его перед клонируемым элементом. Игнорирует передаваемый элемент
            /// </summary>
            /// <param name="element">Данный параметр игнорируется</param>
            /// <returns>Возвращает новый объект хранилища с клонируемым элементом Excel</returns>
            public override ElementBase<Xdr.TwoCellAnchor> CloneAndInsertBefore(OpenXmlElement element = null)
            {
                var newAnchor = new AnchorCellGroup(CloneElement(), Worksheet);
                Worksheet.Anchors.Add(newAnchor);
                Element.Parent.AppendChild(newAnchor.Element);
                return newAnchor;
            }

            /// <summary>
            /// Метод для удаления объекта и его элемента
            /// </summary>
            public override void Remove()
            {
                base.Remove();
                Worksheet.Anchors.Remove(this);
            }

            /// <summary>
            /// Метод для обновления позиции элемента в документе Excel
            /// </summary>
            public override void Update()
            {
                if (this.MoveBy != 0)
                {
                    Top += this.MoveBy;
                    Bottom += this.MoveBy;

                    // Top и Bottom должны отсчитываться от 1, а в маркере индекс отсчитывается от 0
                    Element.FromMarker.RowId.Text = (Top - 1).ToString();
                    Element.ToMarker.RowId.Text = (Bottom - 1).ToString();

                    this.MoveBy = 0;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Возвращает элемент, который является базовым для замены плейсхолдеров. Обычно это параграф.
            /// </summary>
            /// <returns>Элемент, который является базовым для замены плейсхолдеров.</returns>
            public OpenXmlElement GetPlaceholderBaseElement()
            {
                // если в замену изображения передать сам Element, то он обходит дочерние элементы,
                // берёт первый, и в нём не может найти "Shape" (т.к. это какой-нибудь FromMarker);
                // поэтому при замене по Element будет работать замена текста, но не картинок;
                // для картинок нам желательно передать то же самое, как и для элемента вне табличных групп, а именно, параграф

                A.Paragraph paragraph = Element
                    .GetFirstChild<Xdr.Shape>()
                    ?.GetFirstChild<Xdr.TextBody>()
                    ?.GetFirstChild<A.Paragraph>();

                return (OpenXmlElement)paragraph ?? Element;
            }

            #endregion
        }

        /// <summary>
        /// Класс для работы с именнованными группами в Excel, определяющими таблицы внутри шаблона Excel.
        /// </summary>
        private sealed class TableGroup : CellsGroup<DefinedName>
        {
            #region Fields

            /// <summary>
            /// Справочник с плейсхолдерами строк
            /// </summary>
            private readonly Dictionary<RowCellGroup, List<IPlaceholder>> rowsPlaceholders;

            /// <summary>
            /// Справочник с плейсхолдерами гиперссылок
            /// </summary>
            private readonly Dictionary<HyperlinkCellGroup, List<IPlaceholder>> hyperlinksPlaceholders;

            /// <summary>
            /// Справочник с плейсхолдерами в якорях (надписях)
            /// </summary>
            private readonly Dictionary<AnchorCellGroup, List<IPlaceholder>> anchorsPlaceholders;

            #endregion

            #region Constructors

            public TableGroup(DefinedName defineName, TableGroupType type)
                : base(defineName)
            {
                IsValid = true;
                ErrorText = null;
                Name = defineName.Name;
                Type = type;

                rowsPlaceholders = new Dictionary<RowCellGroup, List<IPlaceholder>>();
                hyperlinksPlaceholders = new Dictionary<HyperlinkCellGroup, List<IPlaceholder>>();
                anchorsPlaceholders = new Dictionary<AnchorCellGroup, List<IPlaceholder>>();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Список объектов смерженных ячеек, входящих в данную группу
            /// </summary>
            public List<MergeCellGroup> MergeCells { get; private set; }

            /// <summary>
            /// Список объектов гиперссылок, входящих в данную таблицу
            /// </summary>
            public List<HyperlinkCellGroup> Hyperlinks { get; private set; }

            /// <summary>
            /// Список объектов строк, входящих в данную таблицу
            /// </summary>
            public List<RowCellGroup> Rows { get; private set; }

            /// <summary>
            /// Список объектов якорей (надписей), входящих в данную таблицу
            /// </summary>
            public List<AnchorCellGroup> Anchors { get; private set; }

            /// <summary>
            /// Имя элемента Worksheet, к которому относится данная именованная область
            /// </summary>
            public string WorksheetName { get; private set; }

            /// <summary>
            /// Название именованной области
            /// </summary>
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            public string Name { get; private set; }

            /// <summary>
            /// Тип таблицы
            /// </summary>
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            public TableGroupType Type { get; private set; }

            /// <summary>
            /// Определяет, является ли данная таблица валидной
            /// </summary>
            public bool IsValid { get; private set; }

            /// <summary>
            /// Текст ошибки при ошибке валидации
            /// </summary>
            public string ErrorText { get; private set; }

            /// <summary>
            /// Подчиненная группа таблицы
            /// </summary>
            public TableGroup ChildGroup { get; private set; }

            /// <summary>
            /// Обозначает, что группа имеет родительскую группу
            /// </summary>
            public bool HasParent { get; private set; }

            #endregion

            #region Base Overrides

            /// <summary>
            /// Метод для получения основных данных из хранимого элемента
            /// </summary>
            protected override void ParseElement()
            {
                ParseDefineName(Element.InnerText);
            }

            /// <summary>
            /// Данный метод недоступен для текущего типа
            /// </summary>
            /// <param name="element"></param>
            /// <returns></returns>
            public override ElementBase<DefinedName> CloneAndInsertBefore(OpenXmlElement element = null)
            {
                ExcelHelper.NotSupported(nameof(CloneAndInsertBefore), nameof(TableGroup));
                return null;
            }

            /// <summary>
            /// Данный метод недоступен для текущего типа
            /// </summary>
            public override void Update()
            {
                ExcelHelper.NotSupported(nameof(Update), nameof(TableGroup));
            }

            public override string GetDisplayString()
            {
                return string.Format(
                    LocalizationManager.GetString("KrMessages_ExcelTemplate_DefineNameDisplayFormat"),
                    Name,
                    Worksheet.Name,
                    Reference);
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Парсит текст элемента DefineName на состовляющие (имя, координаты элемента). Если имя имеет 2 или более диапозона, то считаем, что данная группа не валидна.
            /// </summary>
            /// <param name="text">Текст элемента DefineName. Группы: 1 - worksheetName, 2 - left, 3 - bottom, 4 - right, 5 - top.</param>
            private void ParseDefineName(string text)
            {
                MatchCollection matches = Regex.Matches(text, @",?'?((?:[^']|'{2})+)'?!\$?([A-Z]+)?\$?(\d+)(?:\:\$?([A-Z]+)?\$?(\d+))?");
                if (matches.Count == 0)
                {
                    IsValid = false;
                    ErrorText = string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_DefineNameParsingError"), Name);
                    return;
                }
                if (matches.Count > 1)
                {
                    IsValid = false;
                    ErrorText = string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_DefineNameRangesError"), Name);
                    return;
                }

                Match match = matches[0];

                // Заменяем '' на ' в имени страницы
                WorksheetName = match.Groups[1].Value.Replace("''", "'");
                Left = match.Groups[2].Value;
                Bottom = int.Parse(match.Groups[3].Value);

                //Right и Top могут отсутствовать. В таком случае в них записывается значение Left и Bottom соответственно.
                Right = match.Groups[4].Success ? match.Groups[4].Value : Left;
                Top = match.Groups[5].Success ? int.Parse(match.Groups[5].Value) : Bottom;

                // Могут быть ситуации, когда не заданы колонки в диапозоне. В таком случае в Left ставим FirstColumn, а в Right LastColumn
                if (string.IsNullOrEmpty(Left))
                {
                    Left = ExcelHelper.FirstColumn;
                }
                if (string.IsNullOrEmpty(Right))
                {
                    Right = ExcelHelper.LastColumn;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Производит инициализацию группы
            /// </summary>
            /// <param name="worksheet">Объект Worksheet, к которому относится данная таблица</param>
            public void Initialize(WorksheetElement worksheet)
            {
                Check.ArgumentNotNull(worksheet, nameof(worksheet));

                Worksheet = worksheet;

                MergeCells = new List<MergeCellGroup>();
                Hyperlinks = new List<HyperlinkCellGroup>();
                Rows = new List<RowCellGroup>();
                Anchors = new List<AnchorCellGroup>();

                foreach (TableGroup table in Worksheet.Tables)
                {
                    // Если одна из групп - группирующая, вторая - Row, и группирующая содержит Row группу
                    if (((Type == TableGroupType.Group && table.Type == TableGroupType.Row)
                        || (Type == TableGroupType.Table
                            && (table.Type == TableGroupType.Row
                                || table.Type == TableGroupType.Group)))
                        && IsInclude(table))
                    {
                        if (!SetChildGroup(table))
                        {
                            IsValid = false;
                            ErrorText = string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_SetChildGroupFailed"),
                                GetDisplayString(),
                                table.GetDisplayString(),
                                ChildGroup.GetDisplayString());
                            return;
                        }
                        continue;
                    }

                    if (((table.Type == TableGroupType.Group && Type == TableGroupType.Row)
                        || (table.Type == TableGroupType.Table
                            && (Type == TableGroupType.Row
                                || Type == TableGroupType.Group)))
                        && table.IsInclude(this))
                    {
                        if (!table.SetChildGroup(this))
                        {
                            IsValid = false;
                            ErrorText = string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_SetChildGroupFailed"),
                                table.GetDisplayString(),
                                GetDisplayString(),
                                table.ChildGroup.GetDisplayString());
                            return;
                        }
                        continue;
                    }

                    // Если таблица имеет пересечение с текущей таблицей, то падаем по ошибке
                    if (IsCrossed(table))
                    {
                        IsValid = false;
                        ErrorText = string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_DefineNameCrossed"),
                            GetDisplayString(),
                            table.GetDisplayString());
                        return;
                    }

                    if (table.Type != TableGroupType.Jump || Type != TableGroupType.Jump)
                    {
                        if (table.Top >= Bottom && table.Bottom <= Bottom
                            || table.Bottom <= Top && table.Top >= Top)
                        {
                            IsValid = false;
                            ErrorText = string.Format(
                                LocalizationManager.GetString("KrMessages_ExcelTemplate_DefineNameCrossedRows"),
                                GetDisplayString(),
                                table.GetDisplayString());
                            return;
                        }
                    }
                }

                Worksheet.Tables.Add(this);

                foreach (HyperlinkCellGroup hyperlink in Worksheet.Hyperlinks)
                {
                    if (IsInclude(hyperlink)
                        && (ChildGroup == null || !ChildGroup.IsInclude(hyperlink)))
                    {
                        Hyperlinks.Add(hyperlink);
                    }
                }

                foreach (MergeCellGroup mergeCell in Worksheet.MergeCells)
                {
                    if (IsInclude(mergeCell))
                    {
                        if (ChildGroup == null || !ChildGroup.IsInclude(mergeCell))
                        {
                            MergeCells.Add(mergeCell);
                        }
                    }
                    else if (IsCrossed(mergeCell))
                    {
                        IsValid = false;
                        ErrorText = string.Format(LocalizationManager.GetString("KrMessages_ExcelTemplate_DefineNameCrossesMergeCell"),
                            GetDisplayString(),
                            mergeCell.GetDisplayString());
                        return;
                    }
                }

                foreach (RowCellGroup row in Worksheet.Rows)
                {
                    if (IsCrossed(row))
                    {
                        Rows.Add(row);
                    }
                }

                foreach (AnchorCellGroup anchor in Worksheet.Anchors)
                {
                    if (IsInclude(anchor)
                        && (ChildGroup == null || !ChildGroup.IsInclude(anchor)))
                    {
                        Anchors.Add(anchor);
                    }
                }
            }

            /// <summary>
            /// Производит установку дочерней группы. Возвращает true, если удалось установить дочернюю группу, и false, если уже существует дочерняя группа
            /// Метод очищает гиперссылки и смерженные ячейки из родительской группы, если те входят в дочернюю группу
            /// </summary>
            /// <param name="table">Группа, с которой создается связь</param>
            /// <returns>Возвращает true, если удалось установить или обновить дочернюю группу, и false, если уже существует дочерняя группа</returns>
            private bool SetChildGroup(TableGroup table)
            {
                if (ChildGroup != null)
                {
                    if (Type == TableGroupType.Table)
                    {
                        if (table.Type == TableGroupType.Group
                            && ChildGroup.Type == TableGroupType.Row
                            && table.IsInclude(table))
                        {
                            // Если в таблицу установлена дочерняя группа как строка, но в итоге находится дочерняя группа как группа, содержащая строку,
                            // То производим установку этой группы как новую дочернюю группу. Связь между строкой и группой производится в TableGroup.Initialize
                        }
                        else if (ChildGroup.Type == TableGroupType.Group
                            && table.Type == TableGroupType.Row
                            && ChildGroup.IsInclude(table))
                        {
                            // Если строка входит в дочернюю группу, то считаем установку дочерней группы успешной
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                ChildGroup = table;
                table.HasParent = true;

                for (int i = Hyperlinks.Count - 1; i >= 0; i--)
                {
                    if (table.IsInclude(Hyperlinks[i]))
                    {
                        Hyperlinks.RemoveAt(i);
                    }
                }

                for (int i = MergeCells.Count - 1; i >= 0; i--)
                {
                    if (table.IsInclude(MergeCells[i]))
                    {
                        MergeCells.RemoveAt(i);
                    }
                }

                for (int i = Anchors.Count - 1; i >= 0; i--)
                {
                    if (table.IsInclude(Anchors[i]))
                    {
                        Anchors.RemoveAt(i);
                    }
                }

                return true;
            }

            /// <summary>
            /// Производит очистку таблицы от старых объектов
            /// </summary>
            public void Clear()
            {
                if (Type == TableGroupType.Row || Type == TableGroupType.Group || Type == TableGroupType.Table)
                {
                    Rows.ForEach(x => x.Remove());
                    MergeCells.ForEach(x => x.Remove());
                }

                Hyperlinks.ForEach(x => x.Remove());
                Anchors.ForEach(x => x.Remove());

                ChildGroup?.Clear();
            }

            /// <summary>
            /// Удаление элемента из документа. Если есть дочерняя группа, она также удаляется из документа
            /// </summary>
            public override void Remove()
            {
                base.Remove();
                this.ChildGroup?.Remove();
            }

            /// <summary>
            /// Получает список плейсхолдеров, относящихся к переданной гиперссылке
            /// </summary>
            /// <param name="hyperlink">Объект гиперссылки, по которой ищутся плейсхолдеры</param>
            /// <returns>Возвращает список плейсхолдеров, относящихся к переданной гиперссылке</returns>
            public IList<IPlaceholder> GetHyperlinkPlaceholders(HyperlinkCellGroup hyperlink)
            {
                return hyperlinksPlaceholders.TryGetValue(hyperlink, out List<IPlaceholder> result)
                    ? result
                    : ExcelHelper.EmptyPlaceholders;
            }

            /// <summary>
            /// Получает список плейсхолдеров, относящихся к переданной строке
            /// </summary>
            /// <param name="row">Объект строки, по которой ищутся плейсхолдеры</param>
            /// <returns>Возвращает список плейсхолдеров, относящихся к переданной строке</returns>
            public IList<IPlaceholder> GetRowPlaceholders(RowCellGroup row)
            {
                return rowsPlaceholders.TryGetValue(row, out List<IPlaceholder> result)
                    ? result
                    : ExcelHelper.EmptyPlaceholders;
            }

            /// <summary>
            /// Получает список плейсхолдеров, относящихся к переданному якорю (надписи)
            /// </summary>
            /// <param name="anchor">Объект якоря (надписи), по которой ищутся плейсхолдеры</param>
            /// <returns>Возвращает список плейсхолдеров, относящихся к переданному якорю (надписи)</returns>
            public IList<IPlaceholder> GetAnchorPlaceholders(AnchorCellGroup anchor)
            {
                return anchorsPlaceholders.TryGetValue(anchor, out List<IPlaceholder> result)
                    ? result
                    : ExcelHelper.EmptyPlaceholders;
            }

            /// <summary>
            /// Метод для получения всех плейсхолдеров текущей таблицы
            /// </summary>
            /// <returns>Возвращает список всех плейсхолдеров таблицы</returns>
            public List<IPlaceholder> GetAllPlaceholders()
            {
                List<IPlaceholder> placeholders = new List<IPlaceholder>();
                foreach (List<IPlaceholder> rowPlaceholders in rowsPlaceholders.Values)
                {
                    placeholders.AddRange(rowPlaceholders);
                }
                foreach (List<IPlaceholder> hyperlinkPlaceholders in hyperlinksPlaceholders.Values)
                {
                    placeholders.AddRange(hyperlinkPlaceholders);
                }
                foreach (List<IPlaceholder> anchorPlaceholders in anchorsPlaceholders.Values)
                {
                    placeholders.AddRange(anchorPlaceholders);
                }

                return placeholders;
            }

            /// <summary>
            /// Метод для добавления плейсхолдера, связанного с объектом гиперссылки
            /// </summary>
            /// <param name="hyperlink">Объект гиперссылки, к которой принадлежит плейсхолдер</param>
            /// <param name="placeholder">Плейсхолдер</param>
            public void AddHyperlinkPlaceholder(HyperlinkCellGroup hyperlink, IPlaceholder placeholder)
            {
                if (hyperlinksPlaceholders.TryGetValue(hyperlink, out List<IPlaceholder> placeholders))
                {
                    placeholders.Add(placeholder);
                }
                else
                {
                    hyperlinksPlaceholders.Add(hyperlink, new List<IPlaceholder> { placeholder });
                }
            }

            /// <summary>
            /// Метод для добавления плейсхолдера, связанного с объектом строки.
            /// </summary>
            /// <param name="row">Объект строки, к которой принадлежит плейсхолдер</param>
            /// <param name="placeholder">Плейсхолдер</param>
            public void AddRowPlaceholder(RowCellGroup row, IPlaceholder placeholder)
            {
                if (rowsPlaceholders.TryGetValue(row, out List<IPlaceholder> placeholders))
                {
                    placeholders.Add(placeholder);
                }
                else
                {
                    rowsPlaceholders.Add(row, new List<IPlaceholder> { placeholder });
                }
            }

            /// <summary>
            /// Метод для добавления плейсхолдера, связанного с объектом якоря (надписи)
            /// </summary>
            /// <param name="anchor">Объект якоря (надписи), к которому принадлежит плейсхолдер</param>
            /// <param name="placeholder">Плейсхолдер</param>
            public void AddAnchorPlaceholder(AnchorCellGroup anchor, IPlaceholder placeholder)
            {
                if (anchorsPlaceholders.TryGetValue(anchor, out List<IPlaceholder> placeholders))
                {
                    placeholders.Add(placeholder);
                }
                else
                {
                    anchorsPlaceholders.Add(anchor, new List<IPlaceholder> { placeholder });
                }
            }

            #endregion
        }

        /// <summary>
        /// Список типов таблиц в Excel
        /// </summary>
        private enum TableGroupType
        {
            Row,
            Jump,
            Group,
            Table,
        }

        private sealed class SharedStringTableContainer
        {
            #region Fields

            private readonly SharedStringTable sharedStringTable;
            private readonly List<SharedStringItem> items;

            #endregion

            #region Constructors

            public SharedStringTableContainer(SharedStringTable sharedStringTable)
            {
                this.sharedStringTable = sharedStringTable;

                this.items = sharedStringTable.Elements<SharedStringItem>().ToList();
            }

            #endregion

            #region Properties

            public int Count => items.Count;

            #endregion

            #region Public Methods

            public SharedStringItem ElementAt(int index)
            {
                return items[index];
            }

            public void Add(SharedStringItem newItem)
            {
                items.Add(newItem);
            }

            public void Save()
            {
                for (int i = (int)sharedStringTable.UniqueCount.Value; i < items.Count; i++)
                {
                    sharedStringTable.AppendChild(items[i]);
                }

                sharedStringTable.Count.Value = (uint)items.Count;
                sharedStringTable.UniqueCount.Value = (uint)items.Count;
            }

            #endregion
        }

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
        public ExcelPlaceholderDocument(MemoryStream stream, Guid templateID)
            : base(stream, templateID)
        {
        }

        #endregion

        #region Fields

        /// <summary>
        /// Контекст расширений
        /// </summary>
        private ExcelPlaceholderReplaceExtensionContext extensionContext;

        /// <summary>
        /// Документ Excel
        /// </summary>
        private SpreadsheetDocument excelDocument;

        /// <summary>
        /// Элемент Stylesheet текущего документа. Хранит информацию о стилях ячеек
        /// </summary>
        private Stylesheet stylesheet;

        /// <summary>
        /// Элемент <see cref="SharedStringTableContainer"/>, являющийся оберткой над <see cref="SharedStringTable"/> текущего документа
        /// </summary>
        private SharedStringTableContainer sharedStringTable;

        /// <summary>
        /// Справочник со всеми таблицами, разбитый по Worksheet
        /// </summary>
        private Dictionary<WorksheetElement, List<TableGroup>> tableGroupsDictionary;

        /// <summary>
        /// Справочник со всеми объектами Worksheet
        /// </summary>
        private Dictionary<Worksheet, WorksheetElement> worksheetsDictionary;

        #endregion

        #region Private Methods

        /// <summary>
        /// Производит замену Replacement'ов в заданном элементе Cell
        /// </summary>
        /// <param name="cell">Cell, в котором производится замена плейсхолдеров</param>
        /// <param name="createNew">Определяет, создается ли ноая ячейка или заменется значение в существующей</param>
        /// <param name="replacements">Массив плейсхолдеров для замены</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task ReplaceElementsInCellAsync(
            Cell cell,
            bool createNew,
            CancellationToken cancellationToken = default,
            params IPlaceholderReplacement[] replacements)
        {
            if (WithExtensions)
            {
                extensionContext.Cell = cell;
            }

            if (cell.DataType != null
                && cell.DataType.HasValue
                && cell.DataType.Value == CellValues.SharedString)
            {
                int sharedStringID = int.Parse(cell.InnerText);

                IPlaceholderReplacement firstReplacement = replacements.First();
                Type valueType = firstReplacement.NewValue.NetType;

                // Если формат ячейки задан явно (в его стиле NumberFormat отличен от 0) и в ячейке находится только данный плейсхолдер, то устанавливаем полученное значение в value
                if (valueType != typeof(string)
                    && valueType != typeof(DBNull)
                    && GetCellNumberFormat(cell) != 0
                    && GetSharedString(sharedStringID).Length == firstReplacement.Placeholder.Text.Length)
                {
                    if (WithExtensions)
                    {
                        extensionContext.PlaceholderValue = firstReplacement.NewValue;
                        extensionContext.Placeholder = firstReplacement.Placeholder;
                        await BeforePlaceholderReplaceAsync(extensionContext.ReplacementContext);
                    }

                    ReadOnlyCollection<PlaceholderField> fields = firstReplacement.NewValue.Fields;
                    object value = fields.Count > 0 ? fields[0].Value : null;

                    string newCellValue;
                    if (value == null)
                    {
                        newCellValue = firstReplacement.NewValue.Text;
                    }
                    else if (valueType == typeof(DateTime))
                    {
                        newCellValue = ((DateTime)value).ToOADate().ToString(ExcelHelper.DoubleExcelFormat);
                    }
                    else if ((valueType == typeof(decimal) || valueType == typeof(double) || valueType == typeof(float))
                        && value is IFormattable formattable)
                    {
                        newCellValue = formattable.ToString(null, ExcelHelper.DoubleExcelFormat);
                    }
                    else
                    {
                        newCellValue = firstReplacement.NewValue.Text;
                    }

                    cell.DataType = null;

                    cell.CellValue.Text = RemoveInvalidChars(newCellValue);
                    if (WithExtensions)
                    {
                        extensionContext.PlaceholderTextElement = cell.CellValue;
                        extensionContext.PlaceholderElement = cell;
                        await AfterPlaceholderReplaceAsync(extensionContext.ReplacementContext);
                    }
                }
                else // Иначе заменяем значение в SharedString
                {
                    SharedStringItem sharedString;
                    if (createNew)
                    {
                        (sharedString, sharedStringID) = this.CopySharedString(sharedStringID);
                    }
                    else
                    {
                        sharedString = sharedStringTable.ElementAt(sharedStringID);
                    }

                    await ReplaceElementsInSharedStringAsync(sharedString, cancellationToken, replacements);

                    cell.CellValue.Text = RemoveInvalidChars(sharedStringID.ToString());
                }
            }
            else
            {
                await ReplaceElementsInCompositeElementAsync(cell, cancellationToken, replacements);
            }

            if (WithExtensions)
            {
                extensionContext.Cell = null;
            }
        }

        /// <summary>
        /// Метод для получения текста ячейки
        /// </summary>
        /// <param name="cell">Элемент ячейки</param>
        /// <returns>Возвращает текст ячейки</returns>
        private string GetCellText(Cell cell)
        {
            return cell.DataType != null
                && cell.DataType.HasValue
                && cell.DataType.Value == CellValues.SharedString
                    ? this.GetSharedString(int.Parse(cell.InnerText))
                    : cell.InnerText;
        }

        /// <summary>
        /// Возвращает все дочерние элементы с типом Text среди всех дочерних элементов объекта <c>baseElement</c>
        /// </summary>
        /// <param name="baseElement">Базовый элемент, начиная с которого производим поиск</param>
        /// <returns>Список элементов типа Text, или null, если <c>baseElement</c> не содержит дочерних элементов типа Text</returns>
        private static List<A.Text> GetTextElements(OpenXmlElement baseElement)
        {
            var result = new List<A.Text>();

            foreach (OpenXmlElement e in baseElement.ChildElements)
            {
                Type eType = e.GetType();
                if (eType == typeof(A.Text))
                {
                    result.Add((A.Text)e);
                }
                else if (eType != typeof(A.Hyperlink) && eType != typeof(A.Paragraph) && e.HasChildren)
                {
                    result.AddRange(GetTextElements(e));
                }
            }

            return result;
        }

        /// <summary>
        /// Метод производит инициализацию объектов Worksheets
        /// </summary>
        private void InitializeWorksheets()
        {
            worksheetsDictionary = new Dictionary<Worksheet, WorksheetElement>();

            WorkbookPart workboookPart = excelDocument.WorkbookPart;
            IEnumerable<WorksheetPart> worksheetParts = workboookPart.WorksheetParts;
            Sheet[] sheetCollection = excelDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToArray();

            foreach (WorksheetPart worksheetPart in worksheetParts)
            {
                string relID = workboookPart.GetIdOfPart(worksheetPart);
                Sheet sheet = sheetCollection.FirstOrDefault(x => x.Id == relID);

                if (sheet != null)
                {
                    WorksheetElement worksheet = new WorksheetElement(worksheetPart.Worksheet, sheet.Name);
                    worksheetsDictionary.Add(worksheet.Element, worksheet);
                }
            }
        }

        /// <summary>
        /// Метод производит обновление позиций всех внутренных объектов Worksheets
        /// </summary>
        private void UpdateWorksheets()
        {
            foreach (WorksheetElement worksheet in worksheetsDictionary.Values)
            {
                worksheet.Update();
            }
        }

        /// <summary>
        /// Метод производит поиск и инициализацию объектов групп для таблиц в Excel. Если группы настроенны некорректно, метод вернет ValidationResult с ошибкой.
        /// </summary>
        /// <returns></returns>
        private ValidationResultBuilder InitializeTableGroups()
        {
            ValidationResultBuilder validationResult = new ValidationResultBuilder();
            tableGroupsDictionary = new Dictionary<WorksheetElement, List<TableGroup>>();

            DefinedNames defNames = excelDocument.WorkbookPart.Workbook.DefinedNames;

            if (defNames != null && defNames.ChildElements.Count > 0)
            {
                foreach (DefinedName defName in defNames.Elements<DefinedName>())
                {
                    TableGroupType type;
                    string prefix = defName.Name.Value.Length > 1 ? defName.Name.Value.Substring(0, 2) : string.Empty;
                    switch (prefix)
                    {
                        case "r_":
                            type = TableGroupType.Row;
                            break;

                        case "j_":
                            type = TableGroupType.Jump;
                            break;

                        case "g_":
                            type = TableGroupType.Group;
                            break;

                        case "t_":
                            type = TableGroupType.Table;
                            break;

                        default:
                            continue;
                    }

                    TableGroup newTableGroup = new TableGroup(defName, type);
                    if (!newTableGroup.IsValid)
                    {
                        validationResult.AddError(this, newTableGroup.ErrorText);
                        break;
                    }

                    WorksheetElement worksheetElement = worksheetsDictionary.Values.FirstOrDefault(x => x.Name == newTableGroup.WorksheetName);
                    if (worksheetElement == null)
                    {
                        // Если по каким то причинам для группы невозможно найти страницу, то пишем ошибку
                        validationResult.AddError(this,
                            "$FileTemplate_Excel_ErrorFindingPageInGroup",
                            newTableGroup.Name, newTableGroup.WorksheetName);

                        break;
                    }

                    newTableGroup.Initialize(worksheetElement);

                    if (!newTableGroup.IsValid)
                    {
                        validationResult.AddError(this, newTableGroup.ErrorText);
                        break;
                    }

                    if (tableGroupsDictionary.TryGetValue(worksheetElement, out List<TableGroup> tableGroups))
                    {
                        tableGroups.Add(newTableGroup);
                    }
                    else
                    {
                        tableGroupsDictionary.Add(
                            worksheetElement,
                            new List<TableGroup> { newTableGroup });
                    }
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Метод производит привязку плейсхолдеров к таблицам
        /// </summary>
        /// <param name="placeholders">Плейсхолдеры</param>
        private void AttachPlaceholders(IList<IPlaceholder> placeholders)
        {
            foreach (IPlaceholder placeholder in placeholders)
            {
                WorksheetElement worksheet = GetWorksheetByPlaceholder(placeholder);
                OpenXmlElement baseElement = GetElementByPlaceholder(excelDocument.WorkbookPart, placeholder);

                // Если плейсхолдер относится к Relationship, то baseElement == null
                if (baseElement == null)
                {
                    string elementId = placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField).Last().ToString();

                    HyperlinkCellGroup hyperlink = worksheet.Hyperlinks.FirstOrDefault(x => x.Element.Id == elementId);
                    if (hyperlink == null)
                    {
                        throw new InvalidOperationException("Can't find Hyperlink with position " +
                            TextPosition(placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField)));
                    }

                    baseElement = hyperlink.Element;

                    if (tableGroupsDictionary.TryGetValue(worksheet, out var tableGroups))
                    {
                        TableGroup tableGroup =
                            tableGroups
                                .FirstOrDefault(x => x.IsInclude(hyperlink.Reference)
                                                    && (x.ChildGroup == null || !x.ChildGroup.IsInclude(hyperlink.Reference)));
                        if (tableGroup != null)
                        {
                            tableGroup.AddHyperlinkPlaceholder(hyperlink, placeholder);
                        }
                    }
                }
                else
                {
                    Type type = baseElement.GetType();
                    if (type == typeof(Cell))
                    {
                        // плейсхолдер в ячейке с текстом
                        Cell cell = (Cell)baseElement;

                        RowCellGroup row = worksheet.Rows.FirstOrDefault(x => x.IsInclude(cell.CellReference.Value));
                        if (row == null)
                        {
                            throw new InvalidOperationException("Can't find Row with position " +
                                TextPosition(placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField)));
                        }

                        if (tableGroupsDictionary.TryGetValue(worksheet, out var tableGroups))
                        {
                            TableGroup tableGroup =
                                tableGroups
                                    .FirstOrDefault(x => x.IsInclude(cell.CellReference.Value)
                                        && (x.ChildGroup == null || !x.ChildGroup.IsInclude(cell.CellReference.Value)));

                            if (tableGroup != null)
                            {
                                tableGroup.AddRowPlaceholder(row, placeholder);
                            }
                        }
                    }
                    else if (type == typeof(A.Paragraph))
                    {
                        // плейсхолдер в надписи
                        A.Paragraph paragraph = (A.Paragraph)baseElement;
                        AnchorCellGroup anchor = worksheet.Anchors.FirstOrDefault(x => x.HasChildElement(paragraph));
                        if (anchor == null)
                        {
                            throw new InvalidOperationException("Can't find Anchor with position " +
                                TextPosition(placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField)));
                        }

                        baseElement = anchor.Element;

                        if (tableGroupsDictionary.TryGetValue(worksheet, out var tableGroups))
                        {
                            TableGroup tableGroup =
                                tableGroups
                                    .FirstOrDefault(x => x.IsInclude(anchor.Reference)
                                        && (x.ChildGroup == null || !x.ChildGroup.IsInclude(anchor.Reference)));

                            if (tableGroup != null)
                            {
                                tableGroup.AddAnchorPlaceholder(anchor, placeholder);
                            }
                        }
                    }
                }

                placeholder.Info.Add(OpenXmlHelper.BaseElementField, baseElement);
            }
        }

        /// <summary>
        /// Метод получает объект Worksheet по плейсхолдеру
        /// </summary>
        /// <param name="placeholder">Плейсхолдер</param>
        /// <returns>Объект Worksheet, к которому принадлежит данный плейсхолдер</returns>
        private WorksheetElement GetWorksheetByPlaceholder(IPlaceholder placeholder)
        {
            if (this.worksheetsDictionary == null)
            {
                return null;
            }

            List<object> position = placeholder.Info.TryGet<List<object>>(OpenXmlHelper.PositionField);
            if (position == null)
            {
                return null;
            }

            OpenXmlPart mainPart = excelDocument.WorkbookPart;
            for (int i = 0; i < position.Count; i++)
            {
                if (position[i] is int index)
                {
                    if (index < 0)
                    {
                        mainPart = mainPart.Parts.ToArray()[~index].OpenXmlPart;

                        if (mainPart.RootElement is Worksheet worksheet)
                        {
                            return this.worksheetsDictionary[worksheet];
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        private WorksheetPart GetWorksheetPartByPlaceholder(IPlaceholder placeholder)
        {
            List<object> position = placeholder.Info.TryGet<List<object>>(OpenXmlHelper.PositionField);
            if (position == null)
            {
                return null;
            }

            OpenXmlPart mainPart = excelDocument.WorkbookPart;
            for (int i = 0; i < position.Count; i++)
            {
                if (position[i] is int index)
                {
                    if (index < 0)
                    {
                        mainPart = mainPart.Parts.ToArray()[~index].OpenXmlPart;

                        if (mainPart is WorksheetPart worksheetPart)
                        {
                            return worksheetPart;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        private async Task ReplacePlaceholdersInRowAsync(
            IPlaceholderReplacementContext context,
            TableGroup tableGroup,
            IPlaceholderRow row,
            int moveIndex,
            Func<RowCellGroup, Row, RowCellGroup> copyRowFunc = null)
        {
            Row prevRow = null;
            foreach (RowCellGroup copyRow in tableGroup.Rows.OrderByDescending(x => x.Top))
            {
                RowCellGroup newRow = copyRowFunc != null
                    ? copyRowFunc(copyRow, prevRow)
                    : (RowCellGroup)copyRow.CloneAndInsertBefore(prevRow);

                newRow.Move(moveIndex);
                prevRow = newRow.Element;

                if (WithExtensions)
                {
                    extensionContext.CurrentRowElement = prevRow;
                    extensionContext.RowElements.Add(prevRow);
                }

                // Берем все плейсхолдеры, относящиеся к текущей строке внутренней таблицы
                foreach (IGrouping<OpenXmlElement, IPlaceholder> placeholders in
                    tableGroup.GetRowPlaceholders(copyRow)
                        .GroupBy(x => x.Info.Get<OpenXmlElement>(OpenXmlHelper.BaseElementField)))
                {
                    OpenXmlElement baseElement = placeholders.Key;
                    if (baseElement == null)
                    {
                        continue;
                    }

                    Cell newCell = GetRelativeElement<Cell>(copyRow.Element, baseElement, newRow.Element);

                    var replacements = new List<IPlaceholderReplacement>();
                    foreach (IPlaceholder placeholder in placeholders.ToArray())
                    {
                        PlaceholderValue newValue = await ((ITablePlaceholderType)placeholder.Type)
                            .ReplaceAsync(context, placeholder, row, context.CancellationToken)
                            ?? PlaceholderValue.Empty;

                        replacements.Add(new PlaceholderReplacement(placeholder, newValue));
                    }

                    await ReplaceElementsInCellAsync(newCell, true, context.CancellationToken, replacements.ToArray());
                }
            }

            foreach (MergeCellGroup mergeCell in tableGroup.MergeCells)
            {
                MergeCellGroup newMergeCellGroup = (MergeCellGroup)mergeCell.CloneAndInsertBefore();
                newMergeCellGroup.Move(moveIndex);
            }

            await this.ReplaceRowPlaceholdersForHyperlinksAsync(context, tableGroup, moveIndex, row);
            await this.ReplaceRowPlaceholdersForAnchorsAsync(context, tableGroup, moveIndex, row);
        }

        /// <summary>
        /// Метод для замены плейсхолдеров в таблице типа Group
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tableGroup">Объект таблицы</param>
        /// <param name="table">Рассчитанные значения данной таблицы</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task ReplaceGroupTablePlaceholdersAsync(
            IPlaceholderReplacementContext context,
            TableGroup tableGroup,
            IEditablePlaceholderTable table)
        {
            int number = 0;
            int moveIndex = 0;

            // Выделяем плейсхолдеры групп
            foreach (IPlaceholder placeholder in tableGroup.GetAllPlaceholders())
            {
                table.AddHorizontalGroupPlaceholder(placeholder);
            }

            // Построчно обрабатываем группирующую таблицу
            // Для плейсхолдеров в группирующей таблице
            await table.FillHorizontalGroupsAsync(context, context.CancellationToken);

            foreach (IGrouping<PlaceholderValue, IPlaceholderRow> grouping
                        in table.Rows.GroupBy(x => x.HorizontalGroup))
            {
                IPlaceholderRow firstRow = grouping.First();
                Dictionary<RowCellGroup, RowCellGroup> newCopyRowsOrdered = new Dictionary<RowCellGroup, RowCellGroup>();
                List<RowCellGroup> afterMovingRows = new List<RowCellGroup>();
                bool needAfterMoving = true;

                await ReplacePlaceholdersInRowAsync(
                    context,
                    tableGroup,
                    firstRow,
                    moveIndex,
                    (copyRow, prevRow) =>
                    {
                        RowCellGroup newRow = (RowCellGroup)copyRow.CloneAndInsertBefore(prevRow);
                        if (tableGroup.ChildGroup != null && tableGroup.ChildGroup.Rows.Contains(copyRow))
                        {
                            needAfterMoving = false;
                            newCopyRowsOrdered.Add(copyRow, newRow);
                        }

                        if (needAfterMoving)
                        {
                            afterMovingRows.Add(newRow);
                        }
                        return newRow;
                    });

                if (WithExtensions)
                {
                    extensionContext.TableElements.AddRange(extensionContext.RowElements);
                    extensionContext.RowElements.Clear();
                }

                // Теперь обрабатываем элементы внутренней группы, если есть внутренняя группа
                if (tableGroup.ChildGroup != null)
                {
                    foreach (IPlaceholderRow row in grouping)
                    {
                        row.Number = ++number;

                        if (WithExtensions)
                        {
                            extensionContext.Row = row;
                            await BeforeRowReplaceAsync(context);
                        }

                        await ReplacePlaceholdersInRowAsync(
                            context,
                            tableGroup.ChildGroup,
                            row,
                            moveIndex,
                            (copyRow, prevRow) => (RowCellGroup)newCopyRowsOrdered[copyRow].CloneAndInsertBefore(prevRow));

                        moveIndex += tableGroup.ChildGroup.Height;

                        if (WithExtensions)
                        {
                            await AfterRowReplaceAsync(context);
                            extensionContext.TableElements.AddRange(extensionContext.RowElements);
                            extensionContext.RowElements.Clear();
                        }
                    }
                    moveIndex -= tableGroup.ChildGroup.Height;

                    // Очищаем новые строки для копирования
                    foreach (var newCopyRow in newCopyRowsOrdered.Values)
                    {
                        newCopyRow.Remove();
                    }
                }

                // Двигаем строчки группировки, которые находятся ниже внутренней группы
                foreach (RowCellGroup rowCellGroup in afterMovingRows)
                {
                    rowCellGroup.Move(moveIndex);
                }

                moveIndex += tableGroup.Height;
            }
            moveIndex -= tableGroup.Height;

            // Очитска старых строк, гиперссылок и смерженных клеток
            tableGroup.Clear();

            // Добавление всем строкам, гиперссылкам, группам и смерженным ячейкам ниже данной таблицы нового индекса строки
            tableGroup.Worksheet.Move(moveIndex, tableGroup.Top);
            tableGroup.Remove();
        }

        /// <summary>
        /// Метод для замены плейсхолдеров в таблице типа Jump
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tableGroup">Объект таблицы</param>
        /// <param name="table">Рассчитанные значения данной таблицы</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task ReplaceJumpTablePlaceholdersAsync(
            IPlaceholderReplacementContext context,
            TableGroup tableGroup,
            IEditablePlaceholderTable table)
        {
            int number = 0;
            int moveIndex = 0;
            List<Row> originalRows = new List<Row>();
            foreach (RowCellGroup row in tableGroup.Rows)
            {
                originalRows.Add((Row)row.Element.CloneNode(true));
            }
            // Если нет строк для замены, то заменяем текст ячеек с табличными плейсхолдерами на String.Empty
            if (table.Rows.Count > 0)
            {
                foreach (IPlaceholderRow row in table.Rows)
                {
                    row.Number = ++number;

                    if (WithExtensions)
                    {
                        extensionContext.Row = row;
                        await BeforeRowReplaceAsync(context);
                    }

                    // Обработку ведем по элементам строк

                    for (int i = 0; i < tableGroup.Rows.Count; i++)
                    {
                        RowCellGroup copyRow = tableGroup.Rows[i];
                        Row originalRowElement = originalRows[i];

                        RowCellGroup editedRow = tableGroup.Worksheet.GetRow(copyRow.Top + moveIndex);
                        // Если в таблице отсутствует элемент строки с данным индексом, то делаем копию оригинального элемента
                        if (editedRow == null)
                        {
                            editedRow = new RowCellGroup(
                                (Row)originalRowElement.CloneNode(false),
                                tableGroup.Worksheet);
                            editedRow.Move(moveIndex);
                        }

                        if (WithExtensions)
                        {
                            extensionContext.CurrentRowElement = editedRow.Element;
                            extensionContext.RowElements.Add(editedRow.Element);
                        }

                        // Берем все плейсхолдеры, относящиеся к текущей строке
                        foreach (var placeholders in
                            tableGroup.GetRowPlaceholders(copyRow)
                                .GroupBy(x => x.Info.Get<OpenXmlElement>(OpenXmlHelper.BaseElementField)))
                        {
                            OpenXmlElement baseElement = placeholders.Key;
                            if (baseElement == null)
                            {
                                continue;
                            }

                            var replacements = new List<IPlaceholderReplacement>();
                            foreach (IPlaceholder placeholder in placeholders.ToArray())
                            {
                                PlaceholderValue newValue = await ((ITablePlaceholderType)placeholder.Type)
                                    .ReplaceAsync(context, placeholder, row, context.CancellationToken)
                                    ?? PlaceholderValue.Empty;

                                replacements.Add(new PlaceholderReplacement(placeholder, newValue));
                            }

                            if (number == 1)
                            {
                                Cell newCell = (Cell)baseElement;
                                await ReplaceElementsInCellAsync(newCell, true, context.CancellationToken, replacements.ToArray());
                            }
                            else
                            {
                                Cell originalCell = GetRelativeElement<Cell>(copyRow.Element, baseElement, originalRowElement);
                                Cell newCell = (Cell)originalCell.CloneNode(true);
                                await ReplaceElementsInCellAsync(newCell, true, context.CancellationToken, replacements.ToArray());
                                editedRow.SetCell(newCell);
                            }
                        }

                        editedRow.Update();

                        if (WithExtensions)
                        {
                            await AfterRowReplaceAsync(context);
                            extensionContext.TableElements.AddRange(extensionContext.RowElements);
                            extensionContext.RowElements.Clear();
                        }
                    }

                    await this.ReplaceRowPlaceholdersForHyperlinksAsync(context, tableGroup, moveIndex, row);
                    await this.ReplaceRowPlaceholdersForAnchorsAsync(context, tableGroup, moveIndex, row);

                    moveIndex += tableGroup.Height;
                }
            }
            else
            {
                // Если нет строк для замены, то заменяем текст ячеек с табличными плейсхолдерами на String.Empty
                // Обработку ведем по элементам строк
                foreach (RowCellGroup row in tableGroup.Rows)
                {
                    foreach (IGrouping<OpenXmlElement, IPlaceholder> placeholders in
                            tableGroup.GetRowPlaceholders(row)
                                .GroupBy(x => x.Info.Get<OpenXmlElement>(OpenXmlHelper.BaseElementField)))
                    {
                        OpenXmlElement baseElement = placeholders.Key;
                        if (baseElement == null)
                        {
                            continue;
                        }

                        Cell cell = (Cell)baseElement;
                        (SharedStringItem sharedString, int sharedStringID) = this.CopySharedString(int.Parse(cell.InnerText));
                        ClearTextInSharedString(sharedString);
                        cell.CellValue.Text = sharedStringID.ToString();
                    }
                }
            }

            // Очистка старых гиперссылок, якорей и др.
            tableGroup.Clear();
            tableGroup.Remove();
        }

        private async Task ReplaceRowPlaceholdersForHyperlinksAsync(
            IPlaceholderReplacementContext context,
            TableGroup tableGroup,
            int moveIndex,
            IPlaceholderRow row)
        {
            foreach (HyperlinkCellGroup hyperlink in tableGroup.Hyperlinks)
            {
                HyperlinkCellGroup newHyperlink = (HyperlinkCellGroup)hyperlink.CloneAndInsertBefore();
                newHyperlink.Move(moveIndex);

                Hyperlink hypElem = hyperlink.Element;
                Hyperlink newHypElem = newHyperlink.Element;
                IList<IPlaceholder> placeholders = tableGroup.GetHyperlinkPlaceholders(hyperlink);

                if (placeholders.Count > 0)
                {
                    WorksheetPart worksheetPart = tableGroup.Worksheet.Element.WorksheetPart;
                    IPlaceholderReplacement[] replacements = await ReplaceRowPlaceholdersAsync(context, row, placeholders);
                    newHypElem.Id = await ReplaceElementsInRelationshipsWithCopyAsync(worksheetPart, hypElem.Id, context.CancellationToken, replacements);
                }
            }
        }

        private async Task ReplaceRowPlaceholdersForAnchorsAsync(
            IPlaceholderReplacementContext context,
            TableGroup tableGroup,
            int moveIndex,
            IPlaceholderRow row)
        {
            foreach (AnchorCellGroup anchor in tableGroup.Anchors)
            {
                AnchorCellGroup newAnchor = (AnchorCellGroup)anchor.CloneAndInsertBefore();
                newAnchor.Move(moveIndex);

                IList<IPlaceholder> placeholders = tableGroup.GetAnchorPlaceholders(anchor);

                if (placeholders.Count > 0)
                {
                    IPlaceholderReplacement[] replacements = await ReplaceRowPlaceholdersAsync(context, row, placeholders);
                    await this.ReplaceElementsInCompositeElementAsync(newAnchor.GetPlaceholderBaseElement(), context.CancellationToken, replacements);
                }
            }
        }

        private static async Task<IPlaceholderReplacement[]> ReplaceRowPlaceholdersAsync(
            IPlaceholderReplacementContext context,
            IPlaceholderRow row,
            IList<IPlaceholder> placeholders)
        {
            var result = new List<IPlaceholderReplacement>(placeholders.Count);
            foreach (IPlaceholder placeholder in placeholders)
            {
                result.Add(
                    new PlaceholderReplacement(
                        placeholder,
                        await ((ITablePlaceholderType) placeholder.Type)
                            .ReplaceAsync(context, placeholder, row, context.CancellationToken)
                        ?? PlaceholderValue.Empty));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Метод для замены плейсхолдеров в таблице типа Row
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tableGroup">Объект таблицы</param>
        /// <param name="table">Рассчитанные значения данной таблицы</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task ReplaceRowTablePlaceholdersAsync(
            IPlaceholderReplacementContext context,
            TableGroup tableGroup,
            IEditablePlaceholderTable table)
        {
            int number = 0;
            int moveIndex = 0;
            foreach (IPlaceholderRow row in table.Rows)
            {
                row.Number = ++number;
                if (WithExtensions)
                {
                    extensionContext.Row = row;
                    await BeforeRowReplaceAsync(context);
                }

                await ReplacePlaceholdersInRowAsync(
                    context,
                    tableGroup,
                    row,
                    moveIndex);

                moveIndex += tableGroup.Height;

                if (WithExtensions)
                {
                    await AfterRowReplaceAsync(context);
                    extensionContext.TableElements.AddRange(extensionContext.RowElements);
                    extensionContext.RowElements.Clear();
                }
            }
            moveIndex -= tableGroup.Height;

            // Очитска старых строк, гиперссылок и смерженных клеток
            tableGroup.Clear();

            // Добавление всем строкам, гиперссылкам, группам и смерженным ячейкам ниже данной таблицы нового индекса строки
            tableGroup.Worksheet.Move(moveIndex, tableGroup.Top);
            tableGroup.Remove();
        }

        private uint GetCellNumberFormat(Cell cell)
        {
            if (cell.StyleIndex == null)
            {
                return 0;
            }

            return stylesheet.CellFormats.Elements<CellFormat>().ElementAt(Convert.ToInt32(cell.StyleIndex.Value)).NumberFormatId ?? 0;
        }

        /// <summary>
        /// Создать объект Picture, который оборачивает. Бинарные данные при этом добавляются в страницу Excel.
        /// </summary>
        /// <param name="worksheetPart">Объект, который содержит страницу Excel (вкладка документа).</param>
        /// <param name="imageBytes">Содержимое изображения в байтах.</param>
        /// <param name="imagePartType">Тип изображения.</param>
        /// <param name="existingProperties">
        /// Существующие настройки формата фигуры, которые надо перенести, или <c>null</c>, если таких настроек нет.
        /// Обычно это настройки объекта "Надпись".
        /// </param>
        /// <param name="width">Ширина изображения в пикселях для 96dpi, заданная в плейсхолдере.</param>
        /// <param name="height">Высота изображения в пикселях для 96dpi, заданная в плейсолдере.</param>
        /// <param name="reformat">
        /// Признак, заданный в плейсхолдере, который указывает на то, что исходные настройки форматирования
        /// <paramref name="existingProperties"/> придётся по большей части выбросить.
        /// </param>
        /// <param name="alternativeText">Замещающий текст.</param>
        private static Xdr.Picture CreatePictureFromImage(
            WorksheetPart worksheetPart,
            byte[] imageBytes,
            ImagePartType imagePartType,
            Xdr.ShapeProperties existingProperties,
            double width,
            double height,
            bool reformat,
            string alternativeText)
        {
            // добавляем изображение в документ и получаем его relationshipId
            DrawingsPart drawingsPart = worksheetPart.DrawingsPart ?? worksheetPart.AddNewPart<DrawingsPart>();
            if (!worksheetPart.Worksheet.ChildElements.OfType<Drawing>().Any())
            {
                worksheetPart.Worksheet.AppendChild(new Drawing { Id = worksheetPart.GetIdOfPart(drawingsPart) });
            }

            Xdr.WorksheetDrawing worksheetDrawing = drawingsPart.WorksheetDrawing;
            if (worksheetDrawing == null)
            {
                worksheetDrawing = new Xdr.WorksheetDrawing();
                drawingsPart.WorksheetDrawing = worksheetDrawing;
            }

            var imagePart = drawingsPart.AddImagePart(imagePartType);
            imagePart.FeedData(new MemoryStream(imageBytes));

            // берём из надписи или генерим свойства фигуры для картинки
            Xdr.ShapeProperties actualProperties;
            if (!reformat && existingProperties != null)
            {
                actualProperties = (Xdr.ShapeProperties)existingProperties.CloneNode(deep: true);
            }
            else
            {
                actualProperties = new Xdr.ShapeProperties(
                    new A.Transform2D(
                        new A.Offset { X = 0L, Y = 0L },
                        new A.Extents { Cx = OpenXmlHelper.DefaultExtentsCx, Cy = OpenXmlHelper.DefaultExtentsCy }
                    ),
                    new A.PresetGeometry { Preset = A.ShapeTypeValues.Rectangle });
            }

            // определяем размеры картинки
            A.Extents extents = actualProperties.Transform2D?.Extents;
            if (extents != null)
            {
                if (width > 0.0)
                {
                    extents.Cx = OpenXmlHelper.PixelsToEmu(width);
                }

                if (height > 0.0)
                {
                    extents.Cy = OpenXmlHelper.PixelsToEmu(height);
                }

                if (reformat && (width <= 0.0 || height <= 0.0))
                {
                    // нужно вычислить актуальные размеры изображения
                    using Image image = Image.FromStream(new MemoryStream(imageBytes));
                    if (width <= 0.0)
                    {
                        extents.Cx = OpenXmlHelper.GetImageCx(image);
                    }

                    if (height <= 0.0)
                    {
                        extents.Cy = OpenXmlHelper.GetImageCy(image);
                    }
                }
            }

            // определяем уникальный идентификатор картинки
            Xdr.NonVisualDrawingProperties[] nvps = worksheetDrawing
                .Descendants<Xdr.NonVisualDrawingProperties>()
                .ToArray();

            uint nvpId = nvps.Length > 0 ? nvps.Max(p => p.Id.Value) + 1u : 1u;

            // генерим объект Picture, в который картинка обёрнута
            return new Xdr.Picture(
                new Xdr.NonVisualPictureProperties(
                    new Xdr.NonVisualDrawingProperties { Id = nvpId, Name = "Image " + nvpId, Title = LocalizationManager.Format(alternativeText) },
                    new Xdr.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true })
                ),
                new Xdr.BlipFill(
                    new A.Blip
                    {
                        Embed = drawingsPart.GetIdOfPart(imagePart),
                        CompressionState = A.BlipCompressionValues.Print
                    },
                    new A.Stretch(new A.FillRectangle())
                ),
                actualProperties);
        }

        #endregion

        #region SharedStrings Private Methods

        /// <summary>
        /// Метод для получения значения SharedString по его ID
        /// </summary>
        /// <param name="id">ID искомой SharedString</param>
        /// <returns></returns>
        private string GetSharedString(int id)
        {
            return sharedStringTable.ElementAt(id).InnerText;
        }

        /// <summary>
        /// Метод для замены плейсхолдеров в SharedString
        /// </summary>
        /// <param name="sharedString">Объект SharedString</param>
        /// <param name="replacements">Плейсхолдеры для замены</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        private Task ReplaceElementsInSharedStringAsync(
            SharedStringItem sharedString,
            CancellationToken cancellationToken = default,
            params IPlaceholderReplacement[] replacements)
        {
            return ReplaceElementsInCompositeElementAsync(sharedString, cancellationToken, replacements);
        }

        /// <summary>
        /// Метод для очистки текста в в SharedString
        /// </summary>
        /// <param name="sharedString">SharedString, в которой очищается текст</param>
        private static void ClearTextInSharedString(SharedStringItem sharedString)
        {
            foreach (Text text in sharedString.Descendants<Text>())
            {
                text.Text = string.Empty;
            }
        }

        /// <summary>
        /// Метод производит копирование SharedString с указанным ID и возвращает новый объект SharedString с его ID
        /// </summary>
        /// <param name="id">ID копируемой SharedString</param>
        /// <returns></returns>
        private Tuple<SharedStringItem, int> CopySharedString(int id)
        {
            SharedStringItem oldItem = sharedStringTable.ElementAt(id);
            SharedStringItem newItem = oldItem.CloneNode(true) as SharedStringItem;
            sharedStringTable.Add(newItem);

            return new Tuple<SharedStringItem, int>(newItem, sharedStringTable.Count - 1);
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="PlaceholderDocument" and @item="ExtensionContext"]'/>
        protected override IPlaceholderReplaceExtensionContext ExtensionContext => extensionContext;

        /// <doc path='info[@type="PlaceholderDocument" and @item="CreateExtensionContext"]'/>
        protected override IPlaceholderReplaceExtensionContext CreateExtensionContext(IPlaceholderReplacementContext context) =>
            extensionContext = new ExcelPlaceholderReplaceExtensionContext(context, context.CancellationToken);

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
                    if (dictionary.TryGetValue(nameof(ExcelPlaceholderDocument), out var listObj)
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
                                newPlaceholder.Info[OpenXmlHelper.OrderField] = values.TryGet<int>(OpenXmlHelper.OrderField);

                                placeholders.Add(newPlaceholder);
                            }
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
            excelDocument = SpreadsheetDocument.Open(Stream, true);

            var sharedStringTablePart = this.excelDocument.WorkbookPart.SharedStringTablePart ??
                this.excelDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();

            sharedStringTable = new SharedStringTableContainer(sharedStringTablePart.SharedStringTable ??
                (excelDocument.WorkbookPart.SharedStringTablePart.SharedStringTable = new SharedStringTable()));

            stylesheet = excelDocument.WorkbookPart.WorkbookStylesPart.Stylesheet ??
                (excelDocument.WorkbookPart.WorkbookStylesPart.Stylesheet = new Stylesheet());

            if (WithExtensions)
            {
                extensionContext.Document = excelDocument;
            }

            return excelDocument;
        }

        /// <summary>
        /// Метод для получения плейсхолдеров из объекта документа
        /// </summary>
        /// <returns>Возвращает список плейсхолдеров, найденных в документе</returns>
        protected override List<IPlaceholderText> GetPlaceholdersFromDocument() =>
            this.GetPlaceholdersFromPart(this.excelDocument.WorkbookPart);

        /// <summary>
        /// Метод для подготовки документа к сохранению
        /// </summary>
        protected override void PrepareDocumentForSave() => sharedStringTable.Save();

        /// <summary>
        /// Метод для сохранения инициализированного документа
        /// </summary>
        protected override void SaveDocument()
        {
            excelDocument.Close();
            excelDocument = null;
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
                Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [nameof(ExcelPlaceholderDocument)] = list,
                };

                foreach (IPlaceholderText placeholder in placeholders)
                {
                    var pDictionary = new Dictionary<string, object>(StringComparer.Ordinal)
                        {
                            { placeholder.Text, placeholder.Info[OpenXmlHelper.PositionField] },
                            { OpenXmlHelper.OrderField, placeholder.Info.TryGet<int>(OpenXmlHelper.OrderField) }
                        };

                    list.Add(pDictionary);
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
        /// Производит замену плейсхолдеров типа Field в документе
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
                OpenXmlElement element = excelDocument.WorkbookPart.Workbook;
                OpenXmlPart part = excelDocument.WorkbookPart;
                List<object> position = replacements.First().Placeholder.Info.Get<List<object>>(OpenXmlHelper.PositionField);

                for (int i = 0; i < position.Count; i++)
                {
                    if (position[i].ToString() == Hyperlink)
                    {
                        if (position.Count > i + 1)
                        {
                            await ReplaceElementsInRelationshipsAsync(part, position[i + 1] as string, context.CancellationToken, replacements.ToArray());
                        }
                        break;
                    }

                    int index = (int)position[i];
                    if (index < 0)
                    {
                        part = part.Parts.ToArray()[~index].OpenXmlPart;
                        element = part.RootElement;
                    }
                    else
                    {
                        element = element.ChildElements[index];

                        if (element is Worksheet worksheet
                            && WithExtensions)
                        {
                            extensionContext.Worksheet = worksheet;
                        }
                    }
                }

                if (element != null)
                {
                    Type elementType = element.GetType();
                    if (elementType == typeof(Cell))
                    {
                        // ReSharper disable once PossibleInvalidCastException
                        await ReplaceElementsInCellAsync((Cell)element, false, context.CancellationToken, replacements.ToArray());
                    }
                    else if (elementType == typeof(A.Paragraph))
                    {
                        await ReplaceElementsInCompositeElementAsync(element, context.CancellationToken, replacements.ToArray());
                    }
                }

                hasChanges = true;
            }

            return hasChanges;
        }

        /// <summary>
        /// Производит замену плейсхолдеров типа Table в документе
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

            InitializeWorksheets();
            context.ValidationResult.Add(InitializeTableGroups());

            if (!context.ValidationResult.IsSuccessful())
            {
                return false;
            }

            bool hasChanges = false;
            AttachPlaceholders(tablePlaceholders);

            foreach (var worksheetTables in tableGroupsDictionary)
            {
                foreach (TableGroup tableGroup in
                    worksheetTables.Value
                        .OrderByDescending(x => x.Top))
                {
                    hasChanges = true;
                    var childGroup = tableGroup.ChildGroup;
                    List<IPlaceholder> rowPlaceholders = tableGroup.GetAllPlaceholders();

                    if (rowPlaceholders.Count == 0
                        && childGroup == null)
                    {
                        continue;
                    }

                    //Если есть родительская группа, то обработку этой таблицы производим в родительской группе
                    if (tableGroup.HasParent)
                    {
                        continue;
                    }

                    while(childGroup != null)
                    {
                        rowPlaceholders.AddRange(childGroup.GetAllPlaceholders());
                        childGroup = childGroup.ChildGroup;
                    }

                    // Сортируем плейсхолдеры по позициям параграфов (слева направо). Если позиции равны, то сортируем по Order внутри параграфа
                    rowPlaceholders.Sort((x, y) =>
                    {
                        int result = string.Compare(
                            TextPosition(x.Info.Get<List<object>>(OpenXmlHelper.PositionField)),
                            TextPosition(y.Info.Get<List<object>>(OpenXmlHelper.PositionField)),
                            StringComparison.Ordinal);

                        return result == 0
                            ? x.Info.TryGet<int>(OpenXmlHelper.OrderField).CompareTo(y.Info.TryGet<int>(OpenXmlHelper.OrderField))
                            : result;
                    });

                    // загружаем данные из базы данных
                    IEditablePlaceholderTable table = null;
                    foreach (var placeholder in rowPlaceholders)
                    {
                        table = await ((ITablePlaceholderType)placeholder.Type)
                            .FillTableAsync(context, placeholder, table, cancellationToken: context.CancellationToken);
                    }

                    if (table == null)
                    {
                        // таблица с данными плейсхолдеров не найдена, значит плейсхолдеры были некорректно заданы в документе
                        // (они не соответствуют карточке или представлению)

                        throw new InvalidOperationException(string.Format(
                            LocalizationManager.GetString("KrMessages_ExcelTemplate_PlaceholdersWithoutTable"),
                            string.Join(", ", rowPlaceholders.Select(x => x.Text))));
                    }

                    if (WithExtensions)
                    {
                        extensionContext.Worksheet = tableGroup.Worksheet.Element;
                        extensionContext.Table = table;
                        await BeforeTableReplaceAsync(context);
                    }

                    // Производим замену значений в плейсхолдерах
                    switch (tableGroup.Type)
                    {
                        case TableGroupType.Row:
                            await ReplaceRowTablePlaceholdersAsync(context, tableGroup, table);
                            break;

                        case TableGroupType.Jump:
                            await ReplaceJumpTablePlaceholdersAsync(context, tableGroup, table);
                            break;

                        case TableGroupType.Group:
                            await ReplaceGroupTablePlaceholdersAsync(context, tableGroup, table);
                            break;

                        case TableGroupType.Table:
                            if (table.Rows.Count > 0)
                            {
                                var childGroupType = tableGroup.ChildGroup?.Type;
                                if (childGroupType == TableGroupType.Row)
                                {
                                    await ReplaceRowTablePlaceholdersAsync(context, tableGroup.ChildGroup, table);
                                }
                                else if (childGroupType == TableGroupType.Group)
                                {
                                    await ReplaceGroupTablePlaceholdersAsync(context, tableGroup.ChildGroup, table);
                                }
                            }
                            else
                            {
                                // Если нет строк с результатом, то все строки в шаблоне Excel удаляем и перемещаем остальные строки на  высоту таблицы
                                tableGroup.Clear();
                                tableGroup.Worksheet.Move(-tableGroup.Height, tableGroup.Bottom);
                            }
                            break;
                    }

                    foreach (HyperlinkCellGroup hyperlink in tableGroup.Hyperlinks)
                    {
                        ClearOldRelationships(tableGroup.Worksheet.Element.WorksheetPart, tableGroup.GetHyperlinkPlaceholders(hyperlink));
                    }

                    if (WithExtensions)
                    {
                        await AfterTableReplaceAsync(context);
                        extensionContext.TableElements.Clear();
                    }
                }
            }

            UpdateWorksheets();
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
            List<IPlaceholderText> result = new List<IPlaceholderText>();

            Type elementType = baseElement.GetType();
            if (elementType == typeof(Cell))
            {
                var excelCell = (Cell)baseElement;

                string allTextString = GetCellText(excelCell);
                OpenXmlHelper.AddPlaceholdersFromText(result, allTextString, position);
            }
            else if (elementType == typeof(A.Paragraph))
            {
                StringBuilder allText = StringBuilderHelper.Acquire();

                List<A.Text> textElements = GetTextElements(baseElement);
                if (textElements != null)
                {
                    foreach (A.Text element in textElements)
                    {
                        allText.Append(element.Text);
                    }
                }

                string allTextString = allText.ToStringAndRelease();
                OpenXmlHelper.AddPlaceholdersFromText(result, allTextString, position);
            }

            return result;
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
            // Text (Spreadsheet.Text): текст в ячейке Excel
            // A.Text (Drawing.Text): текст в объекте "Надпись"
            newText = RemoveInvalidChars(newText);

            Type type = firstTextElement.GetType();
            if (type == typeof(Text))
            {
                var text = (Text)firstTextElement;
                text.Text = newText.Replace(Environment.NewLine, "\n");
                text.Space = SpaceProcessingModeValues.Preserve;

                if (lastTextElement != null)
                {
                    var lastText = (Text)lastTextElement;
                    if (string.IsNullOrEmpty(lastText.Text))
                    {
                        lastBaseElement.Remove();
                    }
                    else
                    {
                        lastText.Space = SpaceProcessingModeValues.Preserve;
                    }
                }
            }
            else if (type == typeof(A.Text))
            {
                var text = (A.Text)firstTextElement;
                text.Text = newText.Replace(Environment.NewLine, "\n");

                if (lastTextElement != null)
                {
                    var lastText = (A.Text)lastTextElement;
                    if (string.IsNullOrEmpty(lastText.Text))
                    {
                        lastBaseElement.Remove();
                    }
                }
            }

            if (WithExtensions)
            {
                extensionContext.PlaceholderElement = firstBaseElement;
                extensionContext.PlaceholderTextElement = firstTextElement;
                await AfterPlaceholderReplaceAsync(extensionContext.ReplacementContext);
            }

            // Если в текстовом элементе текст пустой, то удаляем базовый элемент, в котором хранится этот текст (обычно это Run или сам Text)
            if (string.IsNullOrEmpty(firstTextElement.Text))
            {
                firstBaseElement.Remove();
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
        protected override Task<bool> ReplaceImageAsync(
            OpenXmlElement baseElement,
            IPlaceholderReplacement replacement,
            CancellationToken cancellationToken = default)
        {
            OpenXmlElement shapeBase;

            var existingShape = OpenXmlHelper.FindParent<Xdr.Shape>(baseElement);
            if (existingShape == null
                || (shapeBase = existingShape.Parent) == null)
            {
                return TaskBoxes.False;
            }

            PlaceholderValue value = replacement.NewValue;
            byte[] data = value.Data;

            if (data == null || data.Length == 0)
            {
                // пустое изображение при замене удаляет надпись
                var anchor = OpenXmlHelper.FindParent<Xdr.TwoCellAnchor>(existingShape);
                if (anchor != null)
                {
                    WorksheetElement element = this.GetWorksheetByPlaceholder(replacement.Placeholder);

                    AnchorCellGroup elementAnchor;
                    if (element != null && (elementAnchor = element.Anchors.FirstOrDefault(x => ReferenceEquals(x.Element, anchor))) != null)
                    {
                        // удаляем "надпись" из строки таблицы плейсхолдеров: {t:...}, {tv:...}, ...
                        elementAnchor.Remove();
                    }
                    else
                    {
                        // удаляем "надпись" снаружи таблицы плейсхолдеров: {f:...}, {fv:...}, ....
                        anchor.Remove();
                    }

                    return TaskBoxes.True;
                }

                return TaskBoxes.False;
            }

            WorksheetPart worksheetPart = this.GetWorksheetPartByPlaceholder(replacement.Placeholder);
            if (worksheetPart == null)
            {
                return TaskBoxes.False;
            }

            IPlaceholderImageParameters imageParameters = value.FormatResult.GetImageParameters();
            ImagePartType imagePartType = OpenXmlHelper.GetImagePartType(imageParameters);

            var existingProperties = existingShape.Descendants<Xdr.ShapeProperties>().FirstOrDefault();

            var picture = CreatePictureFromImage(
                worksheetPart,
                data,
                imagePartType,
                existingProperties,
                imageParameters.Width,
                imageParameters.Height,
                imageParameters.Reformat,
                imageParameters.AlternativeText);

            // мы должны расположить элемент Picture в ту же позицию, где был Shape,
            // здесь особенно важно положение относительно ClientData
            int index = shapeBase.IndexOf(existingShape);

            existingShape.Remove();
            shapeBase.InsertAt(picture, index);

            return TaskBoxes.True;
        }

        #endregion
    }
}
