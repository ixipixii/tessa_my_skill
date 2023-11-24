using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;

namespace Tessa.Extensions.Client.Tiles
{
    public class PnrRegulationSetCanceledTileExtension : TileExtension
    {
        private readonly IUIHost uiHost;
        private readonly Tessa.Platform.Runtime.ISession session;
        private readonly ICardRepository cardRepository;

        public PnrRegulationSetCanceledTileExtension(IUIHost uiHost, Tessa.Platform.Runtime.ISession session, ICardRepository cardRepository)
        {
            this.uiHost = uiHost;
            this.session = session;
            this.cardRepository = cardRepository;
        }

        private async Task<bool> GetIsUserInRole(Guid userID, Guid roleID)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetIsUserInRoleExtension,
                Info =
                    {
                        { "userID", userID },
                        { "roleID", roleID }
                    }
            };
            CardResponse response = await cardRepository.RequestAsync(request);
            return response.Info.Get<bool>("isUserInRole");
        }

        // в parameter всегда передается плитка, по которой щелкаем
        private async void ChangeStatus(object parameter)
        {
            Tile changeStatusTile = (Tile)parameter;
            var editor = changeStatusTile.Context.CardEditor;
            var model = editor != null ? editor.CardModel : null;

            if (model != null)
            {
                var stateID = changeStatusTile.Name == "SetWorks" ? PnrFdCardStates.PnrRegulationWorks.ID : PnrFdCardStates.PnrRegulationCanceled.ID;
                var stateName = changeStatusTile.Name == "SetWorks" ? PnrFdCardStates.PnrRegulationWorks.Name : PnrFdCardStates.PnrRegulationCanceled.Name;

                CardRequest request = new CardRequest
                {
                    RequestType = Shared.PnrRequestTypes.SetFdStateCard,
                    Info =
                    {
                        { "mainCardID", model.Card.ID },
                        { "stateID", stateID },
                        { "stateName", stateName }
                    }
                };
                CardResponse response = await cardRepository.RequestAsync(request);
                Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
                TessaDialog.ShowNotEmpty(result);
                if (result.IsSuccessful)
                {
                    await editor.RefreshCardAsync(editor.Context);
                }
            }
        }

        private static void SetWorksEvaluating(object sender, TileEvaluationEventArgs e)
        {
            var editor = UIContext.Current.CardEditor;
            var model = editor != null ? editor.CardModel : null;

            e.SetIsEnabledWithCollapsing(
                e.CurrentTile,
                model != null &&
                    model.CardType.ID == PnrCardTypes.PnrRegulationTypeID &&
                    (Guid)model.Card.Sections["FdSatelliteCommonInfoVirtual"].Fields["StateID"] == PnrFdCardStates.PnrRegulationCanceled.ID
            );
        }

        private static void SetCanceledEvaluating(object sender, TileEvaluationEventArgs e)
        {
            var editor = UIContext.Current.CardEditor;
            var model = editor != null ? editor.CardModel : null;

            e.SetIsEnabledWithCollapsing(
                e.CurrentTile,
                model != null &&
                    model.CardType.ID == PnrCardTypes.PnrRegulationTypeID &&
                    (Guid)model.Card.Sections["FdSatelliteCommonInfoVirtual"].Fields["StateID"] == PnrFdCardStates.PnrRegulationWorks.ID
            );
        }

        public override async Task InitializingGlobal(ITileGlobalExtensionContext context)
        {
            ITilePanel panel = context.Workspace.LeftPanel;

            //var IsCurrentUserGK = await GetIsUserInRole(this.session.User.ID, PnrRoles.EmployeeGkID);

            panel.Tiles.Add(
                new Tile(
                    "SetCanceled",
                    "Установить статус Отменен",
                    context.Icons.Get("Int438"),
                    context.Workspace.LeftPanel,
                    new DelegateCommand(this.ChangeStatus),
                    TileGroups.Top,
                    1,
                    evaluating: SetCanceledEvaluating
                    )
                );
            panel.Tiles.Add(
                new Tile(
                    "SetWorks",
                    "Установить статус Действует",
                    context.Icons.Get("Int430"),
                    context.Workspace.LeftPanel,
                    new DelegateCommand(this.ChangeStatus),
                    TileGroups.Top,
                    1,
                    evaluating: SetWorksEvaluating
                    )
                );
        }
    }
}
