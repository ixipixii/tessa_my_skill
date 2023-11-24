using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Platform.Client.Tiles;
using Tessa.Platform;
using Tessa.Platform.Runtime;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;
using Tessa.UI.Views;

namespace Tessa.Extensions.Default.Client.Tiles
{
    public sealed class KrDocStateTileExtension :
        TileExtension
    {
        #region Constructors

        public KrDocStateTileExtension(
            ICardRepository cardRepository,
            ICardMetadata cardMetadata,
            ISession session)
        {
            this.cardRepository = cardRepository;
            this.cardMetadata = cardMetadata;
            this.session = session;
        }

        #endregion

        #region Fields

        private readonly ICardRepository cardRepository;

        private readonly ICardMetadata cardMetadata;

        private readonly ISession session;

        #endregion

        #region Private Methods

        private async void DeleteKrDocStateFromViewAsync(object parameter)
        {
            IViewContext viewContext = UIContext.Current.ViewContext;
            if (viewContext != null)
            {
                // не включаем режим принудительного удаления без возможности восстановления, т.к. виртуальная карточка
                // и так удаляется без восстановления, но режим принудительного удаления отображает другие предупреждения перед удалением
                const bool withoutBackupOnly = false;

                var operation = new DeleteIntegerCardOperation(
                    (request, item) => request.Info[DefaultExtensionHelper.StateIDKey] = (int) item.CardID,
                    async (item, ct) => DefaultCardTypes.KrDocStateTypeID,
                    withoutBackupOnly,
                    this.cardRepository,
                    this.cardMetadata);

                await operation.StartAsync(viewContext);
            }
        }


        private static void DeleteKrDocStateFromViewEvaluating(object sender, TileEvaluationEventArgs e)
        {
            IViewContext viewContext = e.CurrentTile.Context.ViewContext;

            e.SetIsEnabledWithCollapsing(
                e.CurrentTile,
                viewContext != null
                && viewContext.View?.Metadata.Alias == DefaultViewAliases.KrDocStateCards);
        }

        #endregion

        #region Base Overrides

        public override Task InitializingGlobal(ITileGlobalExtensionContext context)
        {
            ITilePanel panel = context.Workspace.LeftPanel;

            ITile viewsOther = panel.Tiles.TryGet(TileNames.ViewsOther);
            if (viewsOther != null)
            {
                ITile deleteCardFromView = viewsOther.Tiles.TryGet(TileNames.DeleteCardFromView);

                viewsOther.Tiles.Add(
                    new Tile(
                        DefaultTileNames.DeleteKrDocStateFromView,
                        "$UI_Tiles_DeleteCardFromView",
                        context.Icons.Get("Thin59"),
                        panel,
                        new DelegateCommand(this.DeleteKrDocStateFromViewAsync),
                        TileGroups.Views,
                        deleteCardFromView?.Order ?? 100,
                        toolTip: TileHelper.GetToolTip("$UI_Tiles_DeleteCardFromView_ToolTip", TileKeys.DeleteCardFromView),
                        evaluating: DeleteKrDocStateFromViewEvaluating));
            }

            return Task.CompletedTask;
        }


        public override Task InitializingLocal(ITileLocalExtensionContext context)
        {
            ITilePanel panel = context.Workspace.LeftPanel;
            if (panel.Context.ViewContext != null)
            {
                ITile deleteCard = panel.Tiles.TryGet(TileNames.ViewsOther)
                    ?.Tiles.TryGet(DefaultTileNames.DeleteKrDocStateFromView);

                if (deleteCard != null)
                {
                    deleteCard.Context.AddInputBinding(deleteCard, TileKeys.DeleteCardFromView);

                    if (this.session.User.IsAdministrator())
                    {
                        // удаление с зажатым Alt должно быть в режиме "всегда без возможности восстановления",
                        // но виртуальная карточка и так удаляется без возможности восстановления,
                        // поэтому просто добавим горячую клавишу, чтобы диалог отображался и с Alt, и без него
                        deleteCard.Context.AddInputBinding(deleteCard, TileKeys.DeleteCardFromViewAlt);
                    }
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}