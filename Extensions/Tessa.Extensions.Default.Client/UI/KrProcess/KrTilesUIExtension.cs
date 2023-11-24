using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tessa.Cards;
using Tessa.Extensions.Default.Client.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Settings;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Tiles;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    public sealed class KrTilesUIExtension : CardUIExtension
    {
        #region fields

        private readonly IKrTileInflater tileInflater;

        private const string tileCommandKey = CardHelper.SystemKeyPrefix + "TileCommand";
        private const string gestureKey = CardHelper.SystemKeyPrefix + "Gesture";

        #endregion

        #region constructors

        public KrTilesUIExtension(
            IKrTileInflater tileInflater)
        {
            this.tileInflater = tileInflater;
        }

        #endregion

        #region public

        public override async Task ContextInitialized(
            ICardUIExtensionContext context)
        {
            var card = context.Card;
            ITilePanel tilesPanel;
            if (card == null
                || context.Model.Flags.InSpecialMode()
                || (tilesPanel = context.UIContext.Tiles?.LeftPanel) == null)
            {
                return;
            }

            tilesPanel.Tiles.RemoveAll(p =>
            {
                if (p.SharedInfo.ContainsKey(KrConstants.Ui.TileInfo))
                {
                    RemoveHotkey(p);
                    return true;
                }
                return false;
            });

            if (!card.TryGetLocalTiles(out var tileInfos))
            {
                return;
            }

            var tiles = this.tileInflater.Inflate(tilesPanel, tileInfos);

            var actionGroupingTiles = tiles
                .Where(p => p.SharedInfo.TryGet<KrTileInfo>(KrConstants.Ui.TileInfo)?.ActionGrouping == true)
                .Select(p => p.Clone())
                .ToList();
            if (actionGroupingTiles.Count > 0)
            {
                var groupTile = tilesPanel.Tiles.TryGet(TileNames.ActionsGrouping);
                if (groupTile is null)
                {
                    groupTile = new Tile(
                        TileNames.ActionsGrouping,
                        "$UI_Tiles_ActionsGrouping",
                        context.Icons.Get("Thin258"),
                        tilesPanel,
                        DelegateCommand.Empty,
                        TileGroups.Cards,
                        order: 10);
                    tilesPanel.Tiles.Add(groupTile);
                }
                groupTile.Tiles.RemoveAll(p => p.SharedInfo.ContainsKey(KrConstants.Ui.TileInfo));

                groupTile.Info[nameof(ISettings.MinActionsGroupingCount)] = 1;
                groupTile.Tiles.AddRange(actionGroupingTiles);
            }

            tilesPanel.Tiles.AddRange(tiles);
            
            SetHotkeys(tiles, context);
        }

        #endregion

        #region private

        private static void SetHotkeys(ICollection<ITile> tiles, ICardUIExtensionContext context)
        {
            var keyGestureConverter = (TypeConverter) new KeyGestureConverter();
            foreach (var tile in tiles)
            {
                if (tile.Tiles != null
                    && tile.Tiles.Count > 0)
                {
                    SetHotkeys(tile.Tiles, context);
                }

                var tileInfo = tile.SharedInfo.TryGet<KrTileInfo>(KrConstants.Ui.TileInfo);
                if (tileInfo != null
                    && !string.IsNullOrWhiteSpace(tileInfo.ButtonHotkey))
                {
                    try
                    {
                        var gesture = (KeyGesture)keyGestureConverter.ConvertFromString(tileInfo.ButtonHotkey);
                        var tileCommand = new DelegateCommand(async p => await tile.ExecuteCommandWithCheckAsync(TileCommandEventType.KeyPressed));

                        tile.Context.InputBindings.AddCombined(
                            tileCommand,
                            gesture);

                        tile.Info[tileCommandKey] = tileCommand;
                        tile.Info[gestureKey] = gesture;
                    }
                    catch (ArgumentException)
                    {
                        context.ValidationResult.AddWarning($"Invalid hotkey {tileInfo.ButtonHotkey} in KrSecondaryProcess {tileInfo.Name} (ID = {tileInfo.ID}).");
                    }
                }
            }
        }

        private static void RemoveHotkey(ITile tile)
        {
            if (tile.Info.TryGetValue(tileCommandKey, out var tileCommandObj)
                && tile.Info.TryGetValue(gestureKey, out var gestureObj))
            {
                tile.Context.InputBindings.RemoveCombined(
                    (ICommand)tileCommandObj,
                    (InputGesture)gestureObj,
                    (c1, c2) => c1 == c2);
            }

            if (tile.Tiles != null
                && tile.Tiles.Count > 0)
            {
                foreach (var innerTile in tile.Tiles)
                {
                    RemoveHotkey(innerTile);
                }
            }
        }

        #endregion

    }
}