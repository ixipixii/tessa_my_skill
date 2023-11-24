using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public abstract class KrTemplateStoreExtension : CardStoreExtension
    {
        #region fields

        private const string HasStageChangesKey =
            nameof(KrStageTemplateStoreExtension) + "." + nameof(HasStageChangesKey);

        protected readonly IKrStageSerializer Serializer;

        protected readonly ICardStoreStrategy CardStoreStrategy;

        protected readonly ICardMetadata CardMetadata;

        protected readonly IKrProcessCache krProcessCache;

        #endregion

        #region constructor

        protected KrTemplateStoreExtension(
            IKrStageSerializer serializer,
            ICardStoreStrategy cardStoreStrategy,
            ICardMetadata cardMetadata,
            IKrProcessCache krProcessCache)
        {
            this.Serializer = serializer;
            this.CardStoreStrategy = cardStoreStrategy;
            this.CardMetadata = cardMetadata;
            this.krProcessCache = krProcessCache;
        }

        #endregion

        #region virtual

        protected virtual void OnInsert(ICardStoreExtensionContext context)
        {
        }

        protected virtual bool DoInsert(
            ICardStoreExtensionContext context) => true;

        protected virtual void OnUpdate(ICardStoreExtensionContext context, Card innerCard)
        {
        }

        protected virtual bool DoUpdate(
            ICardStoreExtensionContext context) => true;

        protected abstract Task<Card> GetInnerCardAsync(
            ICardStoreExtensionContext context);

        #endregion

        #region base overrides

        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            var card = context.Request.Card;

            var hasStageChanges = this.HasStageChanges(card);

            context.Info[HasStageChangesKey] = hasStageChanges;

            if (!hasStageChanges && !this.DoInsert(context))
            {
                return;
            }

            if (card.StoreMode == CardStoreMode.Insert)
            {
                this.OnInsert(context);

                if (hasStageChanges)
                {
                    await this.UpdateStagesAsync(card, card, context, krProcessCache);
                }
            }
            else
            {
                context.Request.ForceTransaction = true;
            }
        }

        public override async Task AfterBeginTransaction(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }
            var hasStageChanges = context.Info.TryGet<bool>(HasStageChangesKey);

            var outerCard = context.Request.Card;
            if (outerCard.StoreMode == CardStoreMode.Update
                && (hasStageChanges || this.DoUpdate(context)))
            {
                var innerCard = await this.GetInnerCardAsync(context);
                if (!context.ValidationResult.IsSuccessful())
                {
                    return;
                }

                this.OnUpdate(context, innerCard);

                if (hasStageChanges)
                {
                    await this.UpdateStagesAsync(outerCard, innerCard, context, krProcessCache);
                }

                innerCard.RemoveAllButChanged(innerCard.StoreMode);
                var storeContext =
                    await CardStoreContext.CreateAsync(
                        innerCard,
                        DateTime.UtcNow,
                        context.Session,
                        this.CardMetadata,
                        context.ValidationResult,
                        context.DbScope.Executor,
                        context.DbScope.BuilderFactory,
                        cancellationToken: context.CancellationToken);
                var extraSourcesChanged = innerCard.TryGetStagesSection(out var stagesSec)
                        && stagesSec.Rows.Any(p => p.ContainsKey(KrConstants.KrStages.ExtraSources));
                outerCard.Info[KrConstants.Keys.ExtraSourcesChanged] = BooleanBoxes.Box(extraSourcesChanged);

                await this.CardStoreStrategy.StoreAsync(storeContext);
            }
        }

        #endregion

        #region private

        private bool HasStageChanges(Card card)
        {
            var storeMode = card.StoreMode;
            switch (storeMode)
            {
                case CardStoreMode.Insert:
                    // Интересует исключительно виртуальная версия для main карточек
                    return this.Serializer
                        .SettingsSectionNames
                        .Any(p => card.Sections.TryGetValue(p, out var sec) && sec.HasChanges());
                // Интересует исключительно виртуальная версия для main карточек
                case CardStoreMode.Update:
                    return this.Serializer
                        .SettingsSectionNames
                        .Any(p => card.Sections.TryGetValue(p, out _));
                default:
                    throw new InvalidOperationException($"Unknown CardStoreMode.{storeMode.ToString()}");
            }
        }

        private async Task UpdateStagesAsync(Card outerCard, Card innerCard, ICardStoreExtensionContext context, IKrProcessCache krProcessCache)
        {
            IDictionary<Guid, IDictionary<string, object>> stageStorages = null;
            StringDictionaryStorage<CardSection> rows;
            if (innerCard.Sections.TryGetValue(KrConstants.KrStages.Name, out var krStagesSec)
                && (rows = outerCard.TryGetSections()) != null)
            {
                if (outerCard.StoreMode == CardStoreMode.Insert
                    && innerCard.StoreMode == CardStoreMode.Insert)
                {
                    stageStorages = this.Serializer.CreateStageSettings(rows);
                }
                else
                {
                    stageStorages = await this.Serializer.MergeStageSettingsAsync(krStagesSec, rows, context.CancellationToken);
                }
            }

            new KrProcessSectionMapper(outerCard, innerCard)
                .MapKrStages();

            await this.Serializer.UpdateStageSettingsAsync(innerCard, outerCard, stageStorages, krProcessCache, context, context.CancellationToken);
        }

        #endregion

    }
}