using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrSecondaryProcessStoreExtension : KrTemplateStoreExtension
    {
        private readonly ICardTransactionStrategy transactionStrategyWt;

        private readonly ICardGetStrategy getStrategy;

        /// <inheritdoc />
        public KrSecondaryProcessStoreExtension(
            IKrStageSerializer serializer,
            ICardStoreStrategy cardStoreStrategy,
            ICardMetadata cardMetadata,
            [Dependency(CardTransactionStrategyNames.WithoutTransaction)]ICardTransactionStrategy transactionStrategyWt,
            ICardGetStrategy getStrategy,
            IKrProcessCache krProcessCache)
            : base(serializer, cardStoreStrategy, cardMetadata, krProcessCache)
        {
            this.transactionStrategyWt = transactionStrategyWt;
            this.getStrategy = getStrategy;
        }

        /// <inheritdoc />
        protected override async Task<Card> GetInnerCardAsync(
            ICardStoreExtensionContext context)
        {
            var cardID = context.Request.Card.ID;
            var validationResult = context.ValidationResult;
            Card card = null;

            await this.transactionStrategyWt.ExecuteInReaderLockAsync(
                cardID,
                validationResult,
                async p =>
                {
                    var getContext = await this.getStrategy
                        .TryLoadCardInstanceAsync(
                            cardID,
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
                        await this.GetKrSecondaryProcessSectionsToExcludeAsync(p.CancellationToken));

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
                context.CancellationToken);

            if (!validationResult.IsSuccessful())
            {
                return null;
            }

            await this.Serializer.DeserializeSectionsAsync(card, card, cancellationToken: context.CancellationToken);
            return card;
        }

        private async ValueTask<Guid[]> GetKrSecondaryProcessSectionsToExcludeAsync(CancellationToken cancellationToken = default)
        {
            var stageTemplateSectionsIDs = new HashSet<Guid>(
                (await this.CardMetadata.GetCardTypesAsync(cancellationToken))[DefaultCardTypes.KrSecondaryProcessTypeID]
                    .SchemeItems
                    .Select(x => x.SectionID));

            return (await this.CardMetadata.GetSectionsAsync(cancellationToken))
                .Select(x => x.ID)
                .Where(id => stageTemplateSectionsIDs.Contains(id)
                    && id != DefaultSchemeHelper.KrStages
                    && id != DefaultSchemeHelper.KrSecondaryProcesses)
                .ToArray();
        }
    }
}