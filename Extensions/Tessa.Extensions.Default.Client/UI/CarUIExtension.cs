using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Tessa.Cards;
using Tessa.FileConverters;
using Tessa.Files;
using Tessa.Platform.Collections;
using Tessa.Platform.IO;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Cards.Controls.AutoComplete;
using Tessa.UI.Controls.AutoCompleteCtrl;
using Tessa.UI.Controls.TessaGrid;
using Tessa.UI.Files;
using Tessa.UI.Files.Controls;
using Tessa.UI.Menu;
using Tessa.UI.Notifications;

namespace Tessa.Extensions.Default.Client.UI
{
    public sealed class CarUIExtension :
        CardUIExtension
    {
        #region Constructors

        public CarUIExtension(
            ICardRepository cardRepository,
            IUIHost uiHost,
            INotificationUIManager notificationUIManager,
            IAdvancedCardDialogManager cardDialogManager)
        {
            this.cardRepository = cardRepository;
            this.uiHost = uiHost;
            this.notificationUIManager = notificationUIManager;
            this.cardDialogManager = cardDialogManager;
        }

        #endregion

        #region Fields

        private readonly ICardRepository cardRepository;

        private readonly IUIHost uiHost;

        private readonly INotificationUIManager notificationUIManager;

        private readonly IAdvancedCardDialogManager cardDialogManager;

        /// <summary>
        /// Список расширений файлов, из которых можно выполнить преобразование в PDF,
        /// но для которых недоступен встроенный предпросмотр средствами системы <see cref="FileControlHelper.PreviewTypesByExtensions"/>.
        ///
        /// Каждое расширение указано с ведущей точкой и в нижнем регистре.
        /// </summary>
        private static readonly HashSet<string> pdfConverterExtensions =
            new HashSet<string>(
                FileConverterFormat.GetSupportedInputFormats(FileConverterFormat.Pdf)
                    .Select(x => "." + x.ToLowerInvariant())
                    .Except(FileControlHelper.PreviewTypesByExtensions.Keys.Select(x => x.ToLowerInvariant())),
                StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Command Actions

        private async void Get1CButtonActionAsync(object parameter) =>
            await Get1CHelper.RequestAndAddFileAsync(
                this.uiHost,
                this.cardRepository,
                this.notificationUIManager);

        #endregion

        #region Base Overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            if (context.Model.Controls.TryGet("DriverName2", out IControlViewModel driver2Control))
            {
                var autoComplete = (AutoCompleteEntryViewModel) driver2Control;
                autoComplete.ValueSelected +=
                    async (sender, args) =>
                    {
                        await this.notificationUIManager.ShowTextOrMessageBoxAsync(
                            "Demo for event ValueSelected." + Environment.NewLine
                            + "Selected item: " + args.Item.DisplayText);
                    };

                autoComplete.ValueDeleted +=
                    async (sender, args) =>
                    {
                        await this.notificationUIManager.ShowTextOrMessageBoxAsync(
                            "Demo for event ValueDeleted." + Environment.NewLine
                            + "Deleted item: " + args.Item.DisplayText);
                    };

                // Выполняем открытие по двойному клику в модальном диалоге, а не в вкладке основного окна.
                autoComplete.OpenCardCommandClosure.Execute = async p =>
                {
                    if (p is AutoCompleteItem autoCompleteItem)
                    {
                        var id = (Guid) autoCompleteItem.Reference;
                        await this.cardDialogManager.OpenCardAsync(id);
                    }
                };
            }

            if (context.Model.Controls.TryGet("Owners2", out IControlViewModel owner2Control))
            {
                var autoComplete = (AutoCompleteTableViewModel) owner2Control;
                autoComplete.ValueSelected +=
                    async (sender, args) =>
                    {
                        await this.notificationUIManager.ShowTextOrMessageBoxAsync(
                            "Demo for event ValueSelected." + Environment.NewLine
                            + "Selected item: " + args.Item.DisplayText + "[" + args.Row.RowID + "]");
                    };
                autoComplete.ValueDeleted +=
                    async (sender, args) =>
                    {
                        await this.notificationUIManager.ShowTextOrMessageBoxAsync(
                            "Demo for event ValueDeleted." + Environment.NewLine
                            + "Deleted item: " + args.Item.DisplayText + "[" + args.Row.RowID + "]");
                    };
            }

            // определяем обработчик нажатия на кнопку Get1CButton
            if (context.Model.Controls.TryGet("Get1CButton", out IControlViewModel control))
            {
                var button = (ButtonViewModel) control;
                button.CommandClosure.Execute = this.Get1CButtonActionAsync;
                button.IsReadOnly = !context.Model.FileContainer.Permissions.CanAdd;
            }

            // добавляем валидацию (красную рамку) на текстовый контрол CarName
            if (context.Model.Controls.TryGet("CarName", out control))
            {
                control.HasActiveValidation = true;
                control.ValidationFunc = c =>
                    ((TextBoxViewModelBase) c).Text == "42"
                        ? "Can't enter magic number here"
                        : null;
            }

            // скрываем файлы категории "Image" в одном контроле и показываем в другом
            IFileControl allFilesControl = ((FileListViewModel) context.Model.Controls["AllFilesControl"]).FileControl;
            foreach (IFile file in allFilesControl.Files.ToArray())
            {
                // разрешены все файлы, кроме категории "Изображения"
                if (file.Category != null && file.Category.Caption == "Image")
                {
                    allFilesControl.Files.Remove(file);
                }
            }

            IFileControl imagesFilesControl = ((FileListViewModel) context.Model.Controls["ImageFilesControl"]).FileControl;
            foreach (IFile file in imagesFilesControl.Files.ToArray())
            {
                // разрешены только файлы с категорией "Изображения"
                if (file.Category == null || file.Category.Caption != "Image")
                {
                    imagesFilesControl.Files.Remove(file);
                }
            }

            // в карточке "Автомобиль" файлы на клиенте не будут отмечаться как большие, независимо от настроек сервера;
            // после сохранения их отметит как большие серверное расширение
            context.FileContainer.SetNewPhysicalFileAction(async (ctx, ct) => { ctx.Tags.Remove(FileTag.Large); });

            context.FileContainer.ContainerFileAdding += (s, e) =>
            {
                switch (e.Control.Name)
                {
                    case "AllFilesControl":
                        // разрешены все файлы, кроме категории "Изображения"
                        if (e.File.Category != null && e.File.Category.Caption == "Image")
                        {
                            e.Cancel = true;
                        }

                        break;

                    case "ImageFilesControl":
                        // разрешены только файлы с категорией "Изображения"
                        if (e.File.Category == null || e.File.Category.Caption != "Image")
                        {
                            e.Cancel = true;
                        }

                        break;
                }
            };

            // запрещаем добавлять файлы с расширением .exe
            context.FileContainer.Files.ItemChecking += (s, e) =>
            {
                if (e.Action == ControllableItemAction.Add
                    && string.Equals(FileHelper.GetExtension(e.Item.Name), ".exe", StringComparison.OrdinalIgnoreCase))
                {
                    TessaDialog.ShowMessage("Can't add file: " + e.Item.Name);
                    e.Cancel = true;
                }
            };

            // если предпросмотр инициируется через контролы файлов на вкладке "Сравнение файлов", то независимо от глобальных настроек для файлов,
            // которые можно сконвертировать в PDF, но нельзя отобразить встроенным предпросмотром, - будем запрашивать конвертацию
            Func<IFilePreviewContext, CancellationToken, Task> prevFilePreviewAction =
                context.FileContainer.TryGetFilePreviewAction();

            context.FileContainer.SetFilePreviewAction(
                async (ctx, ct) =>
                {
                    switch (ctx.FileControl.Name)
                    {
                        case "CompareFiles1":
                        case "CompareFiles2":
                            if (pdfConverterExtensions.Contains(FileHelper.GetExtension(ctx.File.Name) ?? string.Empty))
                            {
                                IFileContent previewContent = ctx.AllocateAdditionalLocalContent();
                                previewContent.RequestInfo.SetConverterFormat(FileConverterFormat.Pdf);

                                ctx.PreviewContent = previewContent;
                                ctx.LoadingText = "$UI_Controls_Preview_ConvertingFile";
                                ctx.LoadingExtraText = "$UI_Controls_Preview_ConvertingFile_FullText";
                                return;
                            }

                            break;
                    }

                    // если файловый контрол в другом месте или файл не преобразуется в PDF, то используем стандартный обработчик
                    if (prevFilePreviewAction != null)
                    {
                        await prevFilePreviewAction(ctx, ct);
                    }
                });

            // в контроле-таблице "Список акций"
            if (context.Model.Controls.TryGet("ShareList", out control))
            {
                var grid = (GridViewModel) control;

                // добавляем валидацию при сохранении редактируемой строки
                grid.RowValidating += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(e.Row.Get<string>("Name")))
                    {
                        e.ValidationResult.AddError(this, "Share's name is empty (from RowValidating).");
                    }
                };

                // при клике по ячейке из первой колонки "Акция" в открытом окне будет поставлен фокус
                // на первый контрол типа "Строка", в котором также будет выделен весь текст
                grid.RowInvoked += (s, e) =>
                {
                    if (e.Action == GridRowAction.Opening && e.ColumnIndex == 0)
                    {
                        IControlViewModel textBox = e.RowModel.ControlBag.FirstOrDefault(x => x is TextBoxViewModel);
                        if (textBox != null)
                        {
                            textBox.SelectAllWhenFocused(oneTime: true);
                            textBox.Focus();
                        }
                    }
                };

                // при открытии окна добавления/редактирования строки добавляем горячую клавишу
                grid.RowInitializing += (s, e) =>
                {
                    e.Window.InputBindings.Add(
                        new KeyBinding(
                            new DelegateCommand(p => TessaDialog.ShowMessage("F5 key is pressed")),
                            new KeyGesture(Key.F5)));
                };

                // контекстное меню таблицы, зависит от кликнутой ячейки
                grid.ContextMenuGenerators.Insert(0, ctx =>
                {
                    string text = $"Name={ctx.Row.Model.Get<string>("Name")}, Count={ctx.Control.SelectedRows.Count}, Cell=\"{(ctx.Cell?.Value)}\"";

                    ctx.MenuActions.Add(
                        new MenuAction(
                            "Name",
                            text,
                            Icon.Empty,
                            new DelegateCommand(p => { TessaDialog.ShowMessage("Share name is " + ctx.Row.Model.Get<string>("Name")); })));

                    ctx.MenuActions.Add(
                        new MenuAction(
                            "EditRow",
                            "Edit row",
                            ctx.MenuContext.Icons.Get("Thin2"),
                            new DelegateCommand(p =>
                            {
                                var column = ctx.ColumnIndex >= 0 ? grid.Columns[ctx.ColumnIndex] : null;
                                var cellParam = new TessaGridCellParameter(ctx.Row, column);

                                if (grid.EditRowCommand.CanExecute(cellParam))
                                {
                                    grid.EditRowCommand.Execute(cellParam);
                                }
                            })));
                });

                // нажатие Ctrl+Enter показывает окно для выбранной строки в фокусе
                grid.KeyDownHandlers.Add((row, e) =>
                {
                    if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers.Has(ModifierKeys.Control))
                    {
                        e.Handled = true;
                        TessaDialog.ShowMessage($"Share name is {row.Model.Get<string>("Name")}");
                    }
                });
            }

            // когда страница листается в левой области предпросмотра - она также листается в правой
            if (context.Model.Controls.TryGet("Preview1", out control)
                && control is FilePreviewViewModel preview1
                && context.Model.Controls.TryGet("Preview2", out control)
                && control is FilePreviewViewModel preview2)
            {
                preview1.FilePreview.PagingControlPropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(IFilePagingControlModel.CurrentPage))
                    {
                        int currentPage = ((IFilePagingControlModel) s).CurrentPage;
                        if (currentPage <= 0)
                        {
                            return;
                        }

                        IFilePagingControlModel otherControl = preview2.FilePreview.PagingControl;

                        if (otherControl != null
                            && currentPage <= otherControl.TotalPages
                            && currentPage != otherControl.CurrentPage)
                        {
                            otherControl.BeginMove(currentPage);
                        }
                    }
                };
            }
        }


        public override async Task Saving(ICardUIExtensionContext context)
        {
            // удаляем файл с определённым именем
            IFile fileToRemove = context.FileContainer.Files.FirstOrDefault(x => x.Name == "remove me.txt");
            if (fileToRemove != null)
            {
                bool removed = await context.FileContainer.Files.RemoveWithNotificationAsync(fileToRemove, context.CancellationToken);
                if (removed)
                {
                    // и вместо него добавляем другой файл с таким же контентом и категорией
                    await context.FileContainer
                        .BuildFile("file was removed.txt")
                        .SetContent(ct => Task.FromResult(fileToRemove.Content))
                        .SetCategory(fileToRemove.Category)
                        .AddWithNotificationAsync(cancellationToken: context.CancellationToken);
                }
            }
        }

        #endregion
    }
}