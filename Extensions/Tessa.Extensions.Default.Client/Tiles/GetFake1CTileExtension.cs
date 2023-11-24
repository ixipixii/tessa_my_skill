using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Client.UI;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Notifications;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;

namespace Tessa.Extensions.Default.Client.Tiles
{
    public sealed class GetFake1CTileExtension :
        TileExtension
    {
        #region Constructors

        public GetFake1CTileExtension(
            ICardRepository cardRepository,
            IUIHost uiHost,
            INotificationUIManager notificationUIManager)
        {
            this.cardRepository = cardRepository;
            this.uiHost = uiHost;
            this.notificationUIManager = notificationUIManager;
        }

        #endregion

        #region Fields

        private readonly ICardRepository cardRepository;

        private readonly IUIHost uiHost;

        private readonly INotificationUIManager notificationUIManager;

        #endregion

        #region Private Methods

        private static void EnableForCarAndCanAdd(object sender, TileEvaluationEventArgs e)
        {
            ICardEditorModel editor = e.CurrentTile.Context.CardEditor;

            e.SetIsEnabledWithCollapsing(
                e.CurrentTile,
                editor != null
                && editor.CardModel != null
                && string.Equals(editor.CardModel.CardType.Name, "Car", StringComparison.Ordinal)
                && editor.CardModel.FileContainer.Permissions.CanAdd);
        }

        #endregion

        #region Command Actions

        private async void CommandActionAsync(object parameter) =>
            await Get1CHelper.RequestAndAddFileAsync(
                this.uiHost,
                this.cardRepository,
                this.notificationUIManager);

        #endregion

        #region Base Overrides

        public override Task InitializingGlobal(ITileGlobalExtensionContext context)
        {
            context.Workspace.LeftPanel.Tiles.Add(
                new Tile(
                    "Fake 1C",
                    "$CardTypes_Controls_GetFrom1C",
                    context.Icons.Get("Thin111"),
                    context.Workspace.LeftPanel,
                    new DelegateCommand(this.CommandActionAsync),
                    TileGroups.Cards,
                    order: 1000,
                    evaluating: EnableForCarAndCanAdd));

            return Task.CompletedTask;
        }

        #endregion
    }
}
