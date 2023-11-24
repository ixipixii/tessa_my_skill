using System.Collections.Generic;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Tiles;
using Unity;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public sealed class KrTileInflater : IKrTileInflater
    {
        private readonly IIconContainer iconContainer;

        private readonly IKrTileCommand localCommand;

        private readonly IKrTileCommand globalCommand;

        public KrTileInflater(
            IIconContainer iconContainer,
            [Dependency(KrTileCommandNames.Local)] IKrTileCommand localCommand,
            [Dependency(KrTileCommandNames.Global)] IKrTileCommand globalCommand)
        {
            this.iconContainer = iconContainer;
            this.localCommand = localCommand;
            this.globalCommand = globalCommand;
        }

        /// <inheritdoc />
        public List<ITile> Inflate(
            ITileContextSource contextSource,
            IReadOnlyCollection<KrTileInfo> tileInfos)
        {
            var tiles = new List<ITile>(tileInfos.Count);

            var order = 0;
            foreach (var tileInfo in tileInfos)
            {
                tiles.Add(this.InflateTile(contextSource, tileInfo, order++));
            }

            return tiles;
        }

        private ITile InflateTile(
            ITileContextSource contextSource,
            KrTileInfo tileInfo,
            int order)
        {
            IIcon icon;
            try
            {
                icon = this.iconContainer.Get(tileInfo.Icon);
            }
            catch (KeyNotFoundException)
            {
                icon = Icon.Empty;
            }

            var tileCollection = new TileCollection();
            if (tileInfo.NestedTiles.Count != 0)
            {
                var nestedTiles = new List<ITile>(tileInfo.NestedTiles.Count);
                var nestedOrder = 0;
                foreach (var nestedTileInfo in tileInfo.NestedTiles)
                {
                    nestedTiles.Add(this.InflateTile(contextSource, nestedTileInfo, nestedOrder++));
                }

                tileCollection.AddRange(nestedTiles);

                // Тайл группы.
                var grouptile = new Tile(
                    tileInfo.Caption,
                    TileHelper.SplitCaption(tileInfo.Caption),
                    icon,
                    contextSource,
                    DelegateCommand.Empty,
                    order: order,
                    toolTip: tileInfo.Tooltip,
                    tiles: tileCollection,
                    size: tileInfo.TileSize,
                    sharedInfo: CreateInfo(tileInfo));
                if (tileInfo.ActionGrouping)
                {
                    grouptile.SetActionsGrouping(tileInfo.ActionGrouping);
                    grouptile.Evaluating += TileGroupingEvaluation;
                }
                return grouptile;
            }
            var tile = new Tile(
                tileInfo.Name,
                TileHelper.SplitCaption(tileInfo.Caption),
                icon,
                contextSource,
                new DelegateCommand(this.OnClickAsync),
                order: order,
                toolTip: TileHelper.GetToolTip(tileInfo.Tooltip, tileInfo.ButtonHotkey),
                tiles: tileCollection,
                size: tileInfo.TileSize,
                sharedInfo: CreateInfo(tileInfo));
            if (tileInfo.ActionGrouping)
            {
                tile.SetActionsGrouping(tileInfo.ActionGrouping);
                tile.Evaluating += TileGroupingEvaluation;
            }

            return tile;
        }

        private async void OnClickAsync(object tileObj)
        {
            KrTileInfo tileInfo;

            if (tileObj is ITile tile
                && (tileInfo = tile.SharedInfo.TryGet<KrTileInfo>(KrConstants.Ui.TileInfo)) != null)
            {
                var context = tile.Context;
                var command = tileInfo.IsGlobal
                    ? this.globalCommand
                    : this.localCommand;

                await command.OnClickAsync(context, tile, tileInfo);
            }
        }

        private static SerializableObject CreateInfo(
            KrTileInfo tileInfo)
        {
            return new SerializableObject
            {
                [KrConstants.Ui.TileInfo] = tileInfo
            };
        }

        private static void TileGroupingEvaluation(object sender, TileEvaluationEventArgs e)
        {
            e.SetIsEnabledWithCollapsing(e.CurrentTile, true);
        }
    }
}