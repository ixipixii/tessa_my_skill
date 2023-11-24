using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Files;
using Tessa.Platform.Collections;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Properties.Resharper;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Controls;
using Tessa.UI.Files;
using Tessa.UI.Files.Controls;
using Tessa.UI.Views.Content;
using Tessa.Views;

namespace Tessa.Extensions.Default.Client.UI.CardFiles
{
    [PublicAPI]
    public class CardFilesUIExtension : CardUIExtension
    {
        [NotNull]
        private readonly Func<AddFileButtonViewModel> addFileButtonFactory;

        [NotNull]
        private readonly ICardMetadata cardMetadata;

        [NotNull]
        private readonly IExtensionContainer extensionContainer;

        [NotNull]
        private readonly ISession session;

        [NotNull]
        private readonly IViewService viewService;


        /// <inheritdoc />
        public CardFilesUIExtension(
            [NotNull] ViewCardControlInitializationEvents initializationEvents,
            [NotNull] ICardMetadata cardMetadata,
            [NotNull] ISession session,
            [NotNull] IExtensionContainer extensionContainer,
            [NotNull] IViewService viewService,
            [NotNull] Func<AddFileButtonViewModel> addFileButtonFactory)
        {
            if (initializationEvents is null)
            {
                throw new ArgumentNullException(nameof(initializationEvents));
            }

            this.cardMetadata = cardMetadata ?? throw new ArgumentNullException(nameof(cardMetadata));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.extensionContainer = extensionContainer ?? throw new ArgumentNullException(nameof(extensionContainer));
            this.viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            this.addFileButtonFactory = addFileButtonFactory ?? throw new ArgumentNullException(nameof(addFileButtonFactory));

            initializationEvents.DataProviderInitialization.GetInitializationOverrides += GetOverridesForFileListControl;
            initializationEvents.ContentInitialization.Initialized += this.ContentInitialized;
            initializationEvents.ContextMenuInitialization.Initialized += this.ContextMenuInitialized;
        }

        /// <inheritdoc />
        public override async Task Initializing(ICardUIExtensionContext context)
        {
            var categoriesView = await this.viewService.GetByNameAsync(CardFileCommonConsts.FileCategoriesViewAlias, context.CancellationToken);
            if (categoriesView is null)
            {
                context.ValidationResult.AddError(
                    $"Categories View:'{CardFileCommonConsts.FileCategoriesViewAlias}' isn't found'");
                return;
            }

            var model = context.Model;
            var cardMetadata = model.GeneralMetadata;
            var fileContainer = model.FileContainer;
            var fileTypes =
                CardHelper.GetCardFileTypes(
                    await CardHelper.GetFileCardTypesAsync(
                        cardMetadata,
                        this.session.User.IsAdministrator(),
                        context.CancellationToken));


            var fileControl = TryGetFileControl(model.Info);
            if (fileControl != null)
            {
                return;
            }

            fileControl = this.CreateFileControl(fileContainer, model, fileTypes, categoriesView);
            await fileControl.InitializeAsync(fileContainer.Files, cancellationToken: context.CancellationToken);
            model.Info[CardFileCommonConsts.SpecialFileControlInfoKey] = fileControl;
        }

        private FileListControlViewModel CreateFileControl(IFileUIContainer fileContainer, ICardModel model,
            ICollection<IFileType> fileTypes, ITessaView categoriesView)
        {
            var fileControl =
                new FileListControlViewModel(
                    fileContainer,
                    this.extensionContainer,
                    model.MenuContext,
                    fileTypes,
                    false,
                    false,
                    false,
                    false,
                    false,
                    this.session,
                    name: CardFileCommonConsts.SpecialFileControlName)
                {
                    CategoryFilterAsync = async (categories, ct) =>
                    {
                        ITessaViewResult result = null;
                        var request = new TessaViewRequest(categoriesView.Metadata);

                        IList<object> categoriesViewMapping = new List<object>();
                        var parameters =
                            ViewMappingHelper.AddRequestParameters(
                                categoriesViewMapping,
                                model,
                                this.session,
                                categoriesView);
                        if (parameters != null)
                        {
                            request.Values.AddRange(parameters);
                        }

                        await model.ExecuteInContextAsync(
                            async (c, ct2) =>
                            {
                                result = await categoriesView.GetDataAsync(request, ct2).ConfigureAwait(false);
                            },
                            ct).ConfigureAwait(false);

                        var rows = (result != null ? result.Rows : null)
                            ?? EmptyHolder<object>.Array;

                        // категории из представления в порядке, в котором их вернуло представление (кроме строчек null)
                        var viewCategories = rows
                            .Cast<IList<object>>()
                            .Where(x => x.Count > 0 && x[0] != null)
                            .Select(x => (IFileCategory) new FileCategory((Guid) x[0], (string) x[1]))
                            .ToArray();

                        // категории из представления плюс вручную добавленные или другие присутствующие в карточке категории, кроме null
                        var mainCategories = viewCategories
                            .Union(categories)
                            .Where(x => x != null)
                            .ToArray();

                        // добавляем наверх "Без категории" и возвращаем результирующий список
                        return new List<IFileCategory> { null }
                            .Union(mainCategories);
                    }
                };
            return fileControl;
        }

        private void ContextMenuInitialized(object sender, InitializationViewCardControlEventArgs e)
        {
            var isFileControl = FilesViewMetadata.IsFilesViewMetadata(e.Context.ControlViewModel.ViewMetadata);
            if (isFileControl)
            {
                e.Context.ControlViewModel.ContextMenuGenerators.Add(this.GenerateFileContextMenuItems);
            }
        }

        private void GenerateFileContextMenuItems(ViewControlMenuContext context)
        {
            var fileControl = TryGetFileControl(context);
            var uploadFileMenuAction = CardFilesMenuItems.CreateUploadFileMenuItem(fileControl,
                context.MenuContext.Icons,
                this.session.User,
                this.cardMetadata,
                context.ViewModel.CardModel.FileContainer,
                context.ViewModel.RefreshAsync);
            context.MenuActions.Add(uploadFileMenuAction);
        }


        [CanBeNull]
        private static IFileControl TryGetFileControl([NotNull] ViewControlMenuContext context)
        {
            return TryGetFileControl(context.ViewModel.CardModel.Info);
        }

        [CanBeNull]
        private static IFileControl TryGetFileControl([NotNull] ISerializableObject info)
        {
            return info.TryGetValue(CardFileCommonConsts.SpecialFileControlInfoKey, out var o)
                ? o as IFileControl
                : null;
        }

        private void ContentInitialized(object sender, InitializationViewCardControlEventArgs e)
        {
            var isFileControl = FilesViewMetadata.IsFilesViewMetadata(e.Context.ControlViewModel.ViewMetadata);
            if (!isFileControl)
            {
                return;
            }

            this.OverrideDrop(e);
            this.OverrideFileDoubleClickAction(e);
            this.OverrideEnterKey(e);
            this.CreateUploadFileButton(e);
        }

        private void OverrideDrop(InitializationViewCardControlEventArgs e)
        {
            e.Context.ControlViewModel.AllowDrop = true;
            e.Context.ControlViewModel.DragDrop = new FilesDragDrop(this.cardMetadata, this.session,
                e.Context.Model.FileContainer, e.Context.Model,
                e.Context.ControlViewModel.RefreshAsync);
        }


        private void OverrideEnterKey(InitializationViewCardControlEventArgs e)
        {
            e.Context.ControlViewModel.KeyDownHandlers.Add(
                async (item, data, eventArgs) =>
                {
                    if (data is TableRowViewModel row)
                    {
                        await this.OpenFile(e.Context.Model.Info, row.Data, e.Context.Model.FileContainer.Files);
                    }
                }
            );
        }

        private async Task OpenFile(ISerializableObject info, IDictionary<string, object> data,
            IFileCollection fileCollection)
        {
            var fileControl = TryGetFileControl(info);
            var selectedFileID = (Guid) data["ID"];
            var file = fileCollection.FirstOrDefault(f => f.ID == selectedFileID);
            if (file == null)
            {
                return;
            }

            await FileControlHelper.OpenAsync(fileControl, new[] { file }, FileOpeningMode.ForRead);
        }

        private void OverrideFileDoubleClickAction(InitializationViewCardControlEventArgs e)
        {
            e.Context.ControlViewModel.DoubleClickCommand =
                new DelegateCommand(async o =>
                {
                    var clickInfo = (ViewDoubleClickInfo) o;
                    await this.OpenFile(e.Context.Model.Info, clickInfo.SelectedObject,
                        e.Context.Model.FileContainer.Files);
                });
        }

        private void CreateUploadFileButton(InitializationViewCardControlEventArgs e)
        {
            var addFileButton = this.addFileButtonFactory();
            addFileButton.FileControl = TryGetFileControl(e.Context.Model.Info);
            addFileButton.ViewModel = e.Context.ControlViewModel;
            var refreshButtonIndex = e.Context.ControlViewModel.TopItems.Items.IndexOf(i => i is RefreshButton);
            if (refreshButtonIndex == -1)
            {
                e.Context.ControlViewModel.TopItems.Add(addFileButton);
            }
            else
            {
                e.Context.ControlViewModel.TopItems.Items.Insert(refreshButtonIndex, addFileButton);
            }
        }

        private static void GetOverridesForFileListControl(object sender,
            InitializationViewCardControlOverrideEventArgs e)
        {
            var settings = e.Context.CardTypeControl.ControlSettings;
            var viewAlias = ViewCardControlSettingsHelper.GetViewAlias(settings);
            var isFileControl = FilesViewMetadata.IsFilesViewMetadata(viewAlias);
            if (!isFileControl)
            {
                return;
            }

            e.Handlers.Add(InitializeDataProvider);
        }

        private static void InitializeDataProvider(CardViewControlInitializationContext context)
        {
            context.ControlViewModel.ViewMetadata = FilesViewMetadata.Create();
            context.ControlViewModel.DataProvider =
                new CardFilesDataProvider(new FileViewModelCollection(context.Model.FileContainer.Files));
        }


        private sealed class FilesDragDrop : DefaultDragDrop
        {
            [NotNull]
            private readonly ICardMetadata cardMetadata;

            private readonly ICardModel cardModel;

            private readonly IFileContainer fileContainer;

            [NotNull]
            private readonly Func<Task, Task> refreshFunc;

            [NotNull]
            private readonly ISession session;

            /// <inheritdoc />
            public FilesDragDrop([NotNull] ICardMetadata cardMetadata,
                [NotNull] ISession session,
                [NotNull] IFileContainer fileContainer,
                [NotNull] ICardModel cardModel,
                [NotNull] Func<Task, Task> refreshFunc)
            {
                this.cardMetadata = cardMetadata ?? throw new ArgumentNullException(nameof(cardMetadata));
                this.session = session ?? throw new ArgumentNullException(nameof(session));
                this.fileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
                this.cardModel = cardModel ?? throw new ArgumentNullException(nameof(cardModel));
                this.refreshFunc = refreshFunc ?? throw new ArgumentNullException(nameof(refreshFunc));
            }

            /// <inheritdoc />
            public override async void OnDrop(object sender, DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var fileControl = TryGetFileControl(this.cardModel.Info);
                    var filePaths = (string[]) e.Data.GetData(DataFormats.FileDrop);
                    var user = this.session.User;
                    await FileControlHelper.AddFilesAsync(
                        fileControl,
                        CardHelper.GetCardFileTypes(
                            await CardHelper.GetFileCardTypesAsync(this.cardMetadata,
                                user.IsAdministrator())), this.fileContainer, this.fileContainer.Source,
                        user,
                        filePaths);
                    await this.refreshFunc(null);
                }

                base.OnDrop(sender, e);
            }
        }
    }
}