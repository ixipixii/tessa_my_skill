using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;

namespace Tessa.Extensions.Client.Tiles
{
    /// <summary>
    /// Тайл "Создать заявку на согласование" в карточке контрагента
    /// </summary>
    class PnrCreatePartnerRequestTileExtension : TileExtension
    {
        private readonly IUIHost host;

        public PnrCreatePartnerRequestTileExtension(IUIHost host)
        {
            this.host = host;
        }

        private async void CreatePartnerRequestCommandActionAsync(object parameter)
        {
            var tile = (ITile)parameter;

            IUIContext uiContext = tile.Context;
            ICardEditorModel editor = uiContext.CardEditor;
            ICardModel model = editor?.CardModel;
            if (model == null)
            {
                return;
            }

            using ISplash splash = TessaSplash.Create(TessaSplashMessage.CreatingCard);
            await this.host.CreateCardAsync(
                PnrCardTypes.PnrPartnerRequestTypeID,
                options: new CreateCardOptions
                {
                    UIContext = tile.Context,
                    Splash = splash,
                    Info = new Dictionary<string, object>
                    {
                        { PnrInfoKeys.PnrCreatePartnerRequestPartnerID, model.Card.ID },
                    },
                });
        }

        private static void EnableForPartnerCard(object sender, TileEvaluationEventArgs e)
        {
            ICardEditorModel editor = e.CurrentTile.Context.CardEditor;

            e.SetIsEnabledWithCollapsing(
                e.CurrentTile,
                editor != null
                && editor.CardModel != null
                && editor.CardModel.CardType.ID == DefaultCardTypes.PartnerTypeID
                && editor.CardModel.Card.StoreMode == Tessa.Cards.CardStoreMode.Update);
        }

        public override Task InitializingGlobal(ITileGlobalExtensionContext context)
        {
            // Тайл создания заявки на согласование КА
            ITile createPartnerRequestTile = new Tile(
                "PnrCreatePartnerRequestTile",
                TileHelper.SplitCaption("Создать заявку на согласование"),
                Icon.Empty,
                context.Workspace.LeftPanel,
                //Создание карточки указаного поддтипа
                new DelegateCommand(this.CreatePartnerRequestCommandActionAsync),
                TileGroups.Cards,
                order: 100,
                evaluating: EnableForPartnerCard);

            context.Workspace.LeftPanel.Tiles.Add(createPartnerRequestTile);

            return Task.CompletedTask;
        }
    }
}
