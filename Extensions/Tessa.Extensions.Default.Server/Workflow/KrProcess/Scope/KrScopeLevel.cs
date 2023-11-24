using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Numbers;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Scopes;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope
{
    public sealed class KrScopeLevel : IDisposable
    {
        #region fields

        private readonly ICardRepository cardRepository;
        private readonly IKrTokenProvider tokenProvider;
        private readonly IKrTypesCache krTypesCache;
        private readonly IKrStageSerializer serializer;
        private readonly ICardGetStrategy getStrategy;
        private readonly ICardStreamServerRepository streamServerRepository;

        private readonly IInheritableScopeInstance<KrScopeContext> scope;
        private readonly IDbScope dbScope;
        private readonly ICardMetadata cardMetadata;
        private readonly IValidationResultBuilder validationResult;


        #endregion

        #region constructor

        public KrScopeLevel(
            ICardRepository cardRepository,
            IKrTokenProvider tokenProvider,
            IKrTypesCache krTypesCache,
            IKrStageSerializer serializer,
            ICardGetStrategy getStrategy,
            ICardTransactionStrategy cardCardTransactionStrategy,
            IDbScope dbScope,
            ICardMetadata cardMetadata,
            ICardStreamServerRepository streamServerRepository,
            IValidationResultBuilder validationResult,
            bool withReaderLocks)
        {
            this.cardRepository = cardRepository;
            this.tokenProvider = tokenProvider;
            this.krTypesCache = krTypesCache;
            this.serializer = serializer;
            this.getStrategy = getStrategy;
            this.CardTransactionStrategy = cardCardTransactionStrategy;
            this.dbScope = dbScope;
            this.cardMetadata = cardMetadata;
            this.streamServerRepository = streamServerRepository;
            this.validationResult = validationResult;
            this.WithReaderLocks = withReaderLocks;

            this.scope = KrScopeContext.Create();
            this.scope.Value.LevelStack.Push(this);
        }

        #endregion

        #region properties

        public Guid LevelID { get; } = Guid.NewGuid();

        public bool Exited { get; private set; } // = false;

        public ICardTransactionStrategy CardTransactionStrategy { get; }

        public bool WithReaderLocks { get; }

        #endregion

        #region public

        public void ApplyChanges(
            Guid mainCardID,
            IValidationResultBuilder overridenValidationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            if (scopeContext == null)
            {
                return;
            }

            var locks = scopeContext.Locks;

            if (scopeContext.MainKrSatellites.TryGetItem(mainCardID, out var satellite)
                && !locks.Contains(satellite.ID))
            {
                ProcessInfoCacheHelper.Update(this.serializer, satellite);
                if (satellite.StoreMode == CardStoreMode.Insert || satellite.HasChanges())
                {
                    this.StoreCard(satellite, null, this.GetSuitableValidationResult(overridenValidationResult));
                }
            }

            var forceAffectVersion = scopeContext.ForceIncrementCardVersion.Remove(mainCardID);
            if (scopeContext.Cards.TryGetValue(mainCardID, out var mainCard)
                && !locks.Contains(mainCard.ID))
            {
                if (mainCard.HasChanges()
                    || mainCard.TryGetWorkflowQueue()?.Items?.Count > 0
                    || mainCard.HasNumberQueueToProcess()
                    || forceAffectVersion)
                {
                    scopeContext.CardFileContainers.TryGetValue(mainCardID, out var container);
                    this.StoreCard(
                        mainCard,
                        container,
                        this.GetSuitableValidationResult(overridenValidationResult),
                        forceAffectVersion: forceAffectVersion);
                    container?.Dispose();
                    scopeContext.CardFileContainers.Remove(mainCardID);
                }
            }
            else if (forceAffectVersion)
            {
                // Карточка не загружена, но нужно обязательно увеличить ее версию
                this.ForceIncrementVersion(mainCardID, this.GetSuitableValidationResult(overridenValidationResult));
            }

            var deletedSecondarySatellites = new List<Guid>(scopeContext.SecondaryKrSatellites.Count);
            foreach (var secondarySatellitePair in scopeContext.SecondaryKrSatellites)
            {
                var secondarySatellite = secondarySatellitePair.Value;
                var secondaryProcessMainCardID = secondarySatellite
                    .GetApprovalInfoSection()
                    .Fields[KrConstants.KrProcessCommonInfo.MainCardID];
                if (locks.Contains(secondarySatellite.ID)
                    || !secondaryProcessMainCardID.Equals(mainCardID))
                {
                    continue;
                }

                var currentRowID = secondarySatellite
                    .GetApprovalInfoSection()
                    .Fields[KrConstants.KrProcessCommonInfo.CurrentApprovalStageRowID];
                // Процесс по сателлиту закончился, сателлит больше не нужен.
                if (currentRowID is null)
                {
                    // Если карточка уже создана, ее нужно удалить
                    if (secondarySatellite.StoreMode == CardStoreMode.Update)
                    {
                        this.DeleteCard(secondarySatellite, this.GetSuitableValidationResult(overridenValidationResult));
                    }
                    // Если карточка только создана, но не сохранялась, можно просто забыть про нее.

                    deletedSecondarySatellites.Add(secondarySatellitePair.Key);
                }
                else
                {
                    ProcessInfoCacheHelper.Update(this.serializer, secondarySatellite);
                    if (secondarySatellite.StoreMode == CardStoreMode.Insert
                        || secondarySatellite.HasChanges())
                    {
                        this.StoreCard(secondarySatellite, null, this.GetSuitableValidationResult(overridenValidationResult));
                    }
                }
            }

            foreach (var id in deletedSecondarySatellites)
            {
                scopeContext.SecondaryKrSatellites.Remove(id);
            }
        }

        public void Exit(bool throwIfExited = true, bool throwErrors = true)
        {
            if (this.Exited)
            {
                if (throwIfExited)
                {
                    throw new InvalidOperationException("Current scope has already been disposed.");
                }
                return;
            }

            this.Exited = true;
            if (this.scope == null)
            {
                return;
            }
            var stackTop = this.scope.Value.LevelStack.Pop();
            if (stackTop != this)
            {
                const string text = "Trying to exit from non-top level.";
                if (throwErrors)
                {
                    throw new InvalidOperationException(text);
                }

                this.validationResult?.AddError(nameof(KrScopeLevel), text);
            }

            var ctx = KrScopeContext.Current;
            var locks = ctx.Locks;
            var cardFileContainers = ctx.CardFileContainers;
            this.scope.Dispose();
            if (!ctx.IsDisposed)
            {
                return;
            }

            if (cardFileContainers?.Count > 0)
            {
                foreach (var container in cardFileContainers)
                {
                    container.Value?.Dispose();
                }
            }

            if (locks.Count != 0)
            {
                var text = $"Disposed KrScope contains locks ({string.Join(", ", locks)}). " +
                    "All cards inside scope must be saved at the end (no card should be locked).";
                if (throwErrors)
                {
                    throw new InvalidOperationException(text);
                }

                this.validationResult?.AddError(nameof(KrScopeLevel), text);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!this.Exited)
            {
                this.Exit(false, false);
            }
        }

        #endregion

        #region private

        private IValidationResultBuilder GetSuitableValidationResult(
            IValidationResultBuilder overriden) =>
            overriden ?? this.validationResult;

        private void StoreCard(
            Card card,
            ICardFileContainer fileContainer,
            IValidationResultBuilder result,
            bool forceAffectVersion = false)
        {
            var copy = card.Clone();
            card.RemoveChanges(CardRemoveChangesDeletedHandling.Remove);
            card.RemoveWorkflowQueue();
            card.RemoveNumberQueue();

            copy.RemoveAllButChanged(copy.StoreMode);
            var request = new CardStoreRequest { Card = copy, AffectVersion = forceAffectVersion };

            if (KrComponentsHelper.HasBase(card.TypeID, this.krTypesCache))
            {
                this.tokenProvider.CreateToken(copy).Set(copy.Info);
            }

            string digest = this.cardRepository.GetDigestAsync(card, CardDigestEventNames.ActionHistoryStoreRouteProcess).GetAwaiter().GetResult(); // TODO async
            if (digest != null)
            {
                request.SetDigest(digest);
            }

            var response = CardHelper.StoreAsync(request, fileContainer?.FileContainer, this.cardRepository, this.streamServerRepository).GetAwaiter().GetResult(); // TODO async
            result.Add(response.ValidationResult);
            card.Version = response.CardVersion;
        }

        private void DeleteCard(
            Card card,
            IValidationResultBuilder result)
        {
            var req = new CardDeleteRequest
            {
                CardID = card.ID,
                CardTypeID = card.TypeID,
                DeletionMode = CardDeletionMode.WithoutBackup,
            };
            var resp = this.cardRepository.DeleteAsync(req).GetAwaiter().GetResult(); // TODO async
            result.Add(resp.ValidationResult);
        }

        private void ForceIncrementVersion(
            Guid mainCardID,
            IValidationResultBuilder result)
        {
            // TODO async
            this.CardTransactionStrategy.ExecuteInReaderLockAsync(
                mainCardID,
                result,
                async p => this.ForceIncrementVersionInternal(mainCardID, result)
            ).GetAwaiter().GetResult();
        }

        private void ForceIncrementVersionInternal(
            Guid mainCardID,
            IValidationResultBuilder result)
        {
            var getContext = this.getStrategy.TryLoadCardInstanceAsync(
                mainCardID,
                this.dbScope.Db,
                this.cardMetadata,
                result).GetAwaiter().GetResult(); // TODO async

            var card = getContext.Card;
            var token = this.tokenProvider.CreateToken(card);
            token.Set(card.Info);

            string digest;
            CardStoreRequest workflowRequest = WorkflowScopeContext.Current.StoreContext?.Request;
            if (workflowRequest != null)
            {
                digest = workflowRequest.TryGetDigest();
            }
            else if (this.getStrategy.LoadSectionsAsync(getContext).GetAwaiter().GetResult()) // TODO async
            {
                digest = this.cardRepository.GetDigestAsync(card, CardDigestEventNames.ActionHistoryStoreRouteProcess).GetAwaiter().GetResult(); // TODO async
                card.RemoveAllButChanged();
            }
            else
            {
                digest = null;
            }

            var storeRequest = new CardStoreRequest { Card = card, AffectVersion = true, };
            storeRequest.SetDigest(digest);

            var storeResponse = this.cardRepository.StoreAsync(storeRequest).GetAwaiter().GetResult(); // TODO async
            result.Add(storeResponse.ValidationResult);
        }

        #endregion
    }
}