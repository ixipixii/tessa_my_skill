using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Client.Tiles;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Platform.Collections;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Tiles;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    public sealed class KrEditModeToolbarUIExtension : CardUIExtension
    {
        #region Command Actions

        private static async void OpenForEditingAsync(object parameter)
        {
            await KrTileHelper.OpenMarkedCardAsync(KrPermissionsHelper.CalculatePermissionsMark,
                null, //Не требуем подтверждения дейтсвия если не было изменений
                () => TessaDialog.ConfirmWithCancel("$KrTiles_EditModeConfirmation"));
        }
        
        private static bool CanOpenForEditing(
            object parameter) => UIContext.Current?.CardEditor?.CardModel?.TileIsVisible(DefaultTileNames.KrEditMode) == true;

        private static bool CanOpenForEditing(
            Card card) => card.TileIsVisible(DefaultTileNames.KrEditMode);
            

        #endregion

        #region Base Overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            var actions = context.ToolbarActions;
            var editModeItem = actions.TryGet(DefaultTileNames.KrEditMode);
            var containsEditMode = editModeItem != null;
            if (!containsEditMode && CanOpenForEditing(context.Card))
            {
                var item = new CardToolbarAction(
                    DefaultTileNames.KrEditMode,
                    "$KrTiles_EditMode",
                    context.UIContext.CardEditor.Toolbar.CreateIcon("Thin2"),
                    new DelegateCommand(OpenForEditingAsync, CanOpenForEditing),
                    tooltip: TileHelper.GetToolTip("$KrTiles_EditModeTooltip", KrTileKeys.Edit),
                    order: 11
                );
                actions.Add(item);
                actions.AddOrReplaceInputBinding(item, KrTileKeys.Edit);
            }
            if (containsEditMode && !CanOpenForEditing(context.Card))
            {
                actions.RemoveAll(p => p.Name == DefaultTileNames.KrEditMode);
                actions.RemoveInputBinding(editModeItem, KrTileKeys.Edit);
            }
        }

        #endregion
    }
}