﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrStageTemplateStoreExtension : KrTemplateStoreExtension
    {
        #region fields

        private const string CanChangeOrderWasSwitchedToEnabledKey =
            nameof(KrStageTemplateStoreExtension) + "." + nameof(CanChangeOrderWasSwitchedToEnabledKey);

        private readonly ICardTransactionStrategy transactionStrategy;

        private readonly ICardGetStrategy getStrategy;

        #endregion

        #region constructor

        public KrStageTemplateStoreExtension(
            IKrStageSerializer serializer,
            ICardStoreStrategy cardStoreStrategy,
            ICardMetadata cardMetadata,
            [Unity.Dependency(CardTransactionStrategyNames.WithoutTransaction)] ICardTransactionStrategy transactionStrategy,
            ICardGetStrategy getStrategy,
            IKrProcessCache krProcessCache) : base(serializer, cardStoreStrategy, cardMetadata, krProcessCache)
        {
            this.transactionStrategy = transactionStrategy;
            this.getStrategy = getStrategy;
        }

        #endregion

        #region private

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanChangeOrderWasSwitchedToEnabled(
            Card card) => card.Sections.TryGetValue(KrConstants.KrStageTemplates.Name, out var sec)
            && sec.RawFields.TryGetValue(KrConstants.KrStageTemplates.CanChangeOrder, out var canChangeOrder)
            && (canChangeOrder as bool?) == true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MakeAllStagesVisible(Card card)
        {
            if (!card.TryGetStagesSection(out var stages))
            {
                return;
            }

            foreach (var row in stages.Rows)
            {
                row.Fields[KrConstants.KrStages.Hidden] = BooleanBoxes.False;
            }
        }

        private async Task<Card> GetKrStageTemplateAsync(
            Guid templateID,
            IValidationResultBuilder validationResult,
            CancellationToken cancellationToken = default)
        {
            Card card = null;

            await this.transactionStrategy.ExecuteInReaderLockAsync(
                templateID,
                validationResult,
                async p =>
                {
                    var getContext = await this.getStrategy
                        .TryLoadCardInstanceAsync(
                            templateID,
                            p.DbScope.Db,
                            this.CardMetadata,
                            p.ValidationResult,
                            cancellationToken: p.CancellationToken);

                    if (getContext == null)
                    {
                        p.ReportError = true;
                        return;
                    }

                    getContext.SectionsToExclude.AddRange(
                        await GetKrStageTemplateSectionsToExcludeAsync(p.CancellationToken));

                    await this.getStrategy.LoadSectionsAsync(getContext, p.CancellationToken);

                    if (getContext.Card != null)
                    {
                        card = getContext.Card;
                    }
                    else
                    {
                        p.ReportError = true;
                    }
                },
                cancellationToken);

            if (!validationResult.IsSuccessful())
            {
                return null;
            }

            await this.Serializer.DeserializeSectionsAsync(card, card, cancellationToken: cancellationToken);
            return card;
        }

        private async ValueTask<Guid[]> GetKrStageTemplateSectionsToExcludeAsync(CancellationToken cancellationToken = default)
        {
            var stageTemplateSectionsIDs = new HashSet<Guid>(
                (await this.CardMetadata.GetCardTypesAsync(cancellationToken))[DefaultCardTypes.KrStageTemplateTypeID]
                    .SchemeItems
                    .Select(x => x.SectionID));

            return (await this.CardMetadata.GetSectionsAsync(cancellationToken))
                .Select(x => x.ID)
                .Where(id => stageTemplateSectionsIDs.Contains(id)
                    && id != DefaultSchemeHelper.KrStages
                    && id != DefaultSchemeHelper.KrStageTemplates)
                .ToArray();
        }

        #endregion

        #region base overrides

        protected override void OnInsert(ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;

            var canChangeOrderWasSwitchedToEnabled = context.Info.TryGet<bool>(CanChangeOrderWasSwitchedToEnabledKey);
            if (canChangeOrderWasSwitchedToEnabled)
            {
                MakeAllStagesVisible(card);
            }
        }

        protected override bool DoInsert(
            ICardStoreExtensionContext context) => context.Info.TryGet<bool>(CanChangeOrderWasSwitchedToEnabledKey);

        protected override void OnUpdate(ICardStoreExtensionContext context, Card innerCard)
        {
            var canChangeOrderWasSwitchedToEnabled = context.Info.TryGet<bool>(CanChangeOrderWasSwitchedToEnabledKey);
            if (canChangeOrderWasSwitchedToEnabled)
            {
                MakeAllStagesVisible(innerCard);
            }
        }

        protected override bool DoUpdate(ICardStoreExtensionContext context) =>
            context.Info.TryGet<bool>(CanChangeOrderWasSwitchedToEnabledKey);

        protected override Task<Card> GetInnerCardAsync(
            ICardStoreExtensionContext context)
        {
            return this.GetKrStageTemplateAsync(context.Request.Card.ID, context.ValidationResult, context.CancellationToken);
        }

        public override Task BeforeRequest(ICardStoreExtensionContext context)
        {
            if (context.ValidationResult.IsSuccessful())
            {
                var card = context.Request.Card;
                var canChangeOrderWasSwitchedToEnabled = CanChangeOrderWasSwitchedToEnabled(card);
                context.Info[CanChangeOrderWasSwitchedToEnabledKey] = canChangeOrderWasSwitchedToEnabled;
            }

            return base.BeforeRequest(context);
        }

        #endregion

    }
}