using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrLocalTilesNewGetExtension : CardNewGetExtension
    {
        #region fields

        private readonly IKrTypesCache typesCache;

        private readonly IKrProcessButtonVisibilityEvaluator buttonVisibilityEvaluator;

        #endregion

        #region constructor

        public KrLocalTilesNewGetExtension(
            IKrTypesCache typesCache,
            IKrProcessButtonVisibilityEvaluator buttonVisibilityEvaluator)
        {
            this.typesCache = typesCache;
            this.buttonVisibilityEvaluator = buttonVisibilityEvaluator;
        }

        #endregion

        #region base overrides

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (context.CardType == null
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.ValidationResult.IsSuccessful()
                || context.Request.AreButtonsIgnored()
                || !context.Response.Card.Sections.TryGetValue(DocumentCommonInfo.Name, out var dci))
            {
                return;
            }

            var components = KrComponentsHelper.GetKrComponents(
                context.Response.Card.TypeID,
                dci.RawFields.TryGet<Guid?>(DocumentCommonInfo.DocTypeID),
                this.typesCache);
            if (KrComponentsHelper.GetKrComponents(context.Response.Card.TypeID, this.typesCache).HasNot(KrComponents.Base))
            {
                return;
            }

            var tiles = (await this.LoadButtonsAsync(context.Response.Card, components, context))
                .OrderByLocalized(p => p.Caption)
                .ToList();

            context.Response.Card.SetLocalTiles(tiles);
        }

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            if (context.CardType == null
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.ValidationResult.IsSuccessful()
                || context.Request.AreButtonsIgnored())
            {
                return;
            }

            var components = KrComponentsHelper.GetKrComponents(context.Response.Card, this.typesCache);
            if (KrComponentsHelper.GetKrComponents(context.Response.Card.TypeID, this.typesCache).HasNot(KrComponents.Base))
            {
                return;
            }

            var tiles = (await this.LoadButtonsAsync(context.Response.Card, components, context))
                .OrderByLocalized(p => p.Caption)
                .ToList();

            context.Response.Card.SetLocalTiles(tiles);
        }

        #endregion

        #region private

        private async Task<List<KrTileInfo>> LoadButtonsAsync(
            Card card,
            KrComponents components,
            ICardExtensionContext context)
        {
            var dbScope = context.DbScope;
            await using (dbScope.Create())
            {
                var docTypeID = KrProcessSharedHelper.GetDocTypeID(card, dbScope);
                var typeID = docTypeID ?? card.TypeID;

                KrState state;
                if (card.Sections.TryGetValue(KrApprovalCommonInfo.Virtual, out var ksSec)
                    && ksSec.Fields.TryGetValue(StateID, out var stateIDObj)
                    && stateIDObj is int sid)
                {
                    state = (KrState) sid;
                }
                else
                {
                    state = KrState.Draft;
                }

                var visibilityEvaluatorContext = new KrProcessButtonVisibilityEvaluatorContext(
                    context.ValidationResult,
                    new ObviousMainCardAccessStrategy(card),
                    components,
                    typeID,
                    state,
                    context);
                var evaluatedButtons = await this.buttonVisibilityEvaluator.EvaluateLocalButtonsAsync(visibilityEvaluatorContext, context.CancellationToken);
                var groups = evaluatedButtons.GroupBy(p => p.TileGroup);

                var tileInfos = new List<KrTileInfo>(evaluatedButtons.Count);
                foreach (var group in groups)
                {
                    if (string.IsNullOrWhiteSpace(group.Key))
                    {
                        tileInfos
                            .AddRange(group.Select(p => ConvertToTileInfo(p, true)));
                    }
                    else
                    {
                        var tiles = new List<KrTileInfo>();
                        var actionGrouping = false;
                        foreach (var button in group)
                        {
                            if (button.ActionGrouping)
                            {
                                actionGrouping = true;
                            }

                            tiles.Add(ConvertToTileInfo(button, false));
                        }

                        var tileSize = actionGrouping
                            ? TileSize.Full
                            : TileSize.Half;

                        var globalGroupTile = new KrTileInfo(
                            Guid.Empty,
                            string.Empty,
                            group.Key,
                            Ui.DefaultTileGroupIcon,
                            tileSize,
                            string.Empty,
                            true,
                            false,
                            string.Empty,
                            actionGrouping,
                            null,
                            nestedTiles: tiles.OrderByLocalized(p => p.Caption));
                        tileInfos.Add(globalGroupTile);
                    }
                }


                return tileInfos;
            }
        }

        private static KrTileInfo ConvertToTileInfo(
            IKrProcessButton button,
            bool considerGrouping)
        {
            return new KrTileInfo(
                button.ID,
                button.Name,
                button.Caption,
                button.Icon,
                button.TileSize,
                button.Tooltip,
                button.IsGlobal,
                button.AskConfirmation,
                button.ConfirmationMessage,
                considerGrouping && button.ActionGrouping,
                button.ButtonHotkey,
                EmptyHolder<KrTileInfo>.Collection);
        }

        #endregion
    }
}