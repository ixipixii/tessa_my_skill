using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope
{
    public sealed class KrScope : IKrScope
    {
        #region fields

        private const int MaxDepth = 20;

        private readonly ICardRepository cardRepositoryExt;

        private readonly ICardRepository cardRepositoryEwt;

        private readonly ICardTransactionStrategy transactionStrategy;

        private readonly ICardTransactionStrategy transactionStrategyWt;

        private readonly ICardGetStrategy getStrategy;

        private readonly IDbScope dbScope;

        private readonly ICardMetadata cardMetadata;

        private readonly IKrTokenProvider tokenProvider;

        private readonly IKrStageSerializer serializer;

        private readonly IKrTypesCache typesCache;

        private readonly ICardFileManager fileManager;

        private readonly ICardStreamServerRepository streamServerRepositoryExt;

        private readonly ICardStreamServerRepository streamServerRepositoryEwt;

        #endregion

        #region constructor

        public KrScope(
            [Unity.Dependency(CardRepositoryNames.Extended)] ICardRepository cardRepositoryExt,
            [Unity.Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository cardRepositoryEwt,
            [Unity.Dependency(CardTransactionStrategyNames.Default)] ICardTransactionStrategy transactionStrategy,
            [Unity.Dependency(CardTransactionStrategyNames.WithoutTransaction)] ICardTransactionStrategy transactionStrategyWt,
            ICardGetStrategy getStrategy,
            IDbScope dbScope,
            ICardMetadata cardMetadata,
            IKrTokenProvider tokenProvider,
            IKrStageSerializer serializer,
            IKrTypesCache typesCache,
            ICardFileManager fileManager,
            [Unity.Dependency(CardRepositoryNames.Extended)] ICardStreamServerRepository streamServerRepositoryExt,
            [Unity.Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardStreamServerRepository streamServerRepositoryEwt)
        {
            this.cardRepositoryExt = cardRepositoryExt;
            this.cardRepositoryEwt = cardRepositoryEwt;
            this.transactionStrategy = transactionStrategy;
            this.transactionStrategyWt = transactionStrategyWt;
            this.getStrategy = getStrategy;
            this.dbScope = dbScope;
            this.cardMetadata = cardMetadata;
            this.tokenProvider = tokenProvider;
            this.serializer = serializer;
            this.typesCache = typesCache;
            this.fileManager = fileManager;
            this.streamServerRepositoryExt = streamServerRepositoryExt;
            this.streamServerRepositoryEwt = streamServerRepositoryEwt;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public bool Exists => KrScopeContext.HasCurrent;

        /// <inheritdoc />
        public Dictionary<string, object> Info => KrScopeContext.HasCurrent
            ? KrScopeContext.Current.Info
            : throw new InvalidOperationException("KrScopeContext is unknown");

        /// <inheritdoc />
        public int Depth => KrScopeContext.Current?.LevelStack.Count ?? 0;

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult
        {
            get
            {
                var scopeContext = KrScopeContext.Current;
                AssertNonEmptyKrScopeContext(scopeContext);
                return scopeContext.ValidationResult;
            }
        }

        /// <inheritdoc />
        public KrScopeLevel CurrentLevel => KrScopeContext.Current?.LevelStack.Peek();

        /// <inheritdoc />
        public KrScopeLevel EnterNewLevel(
            IValidationResultBuilder levelValidationResult,
            bool withReaderLocks = true)
        {
            ICardRepository suitableCardRepo;
            ICardStreamServerRepository suitableStreamRepo;
            if (KrProcessHelper.IsTransactionOpened(this.dbScope))
            {
                suitableCardRepo = this.cardRepositoryEwt;
                suitableStreamRepo = this.streamServerRepositoryEwt;
            }
            else
            {
                suitableCardRepo = this.cardRepositoryExt;
                suitableStreamRepo = this.streamServerRepositoryExt;
            }

            var suitableTransactionStrategy = withReaderLocks
                ? this.transactionStrategy
                : this.transactionStrategyWt;

            var level = new KrScopeLevel(
                    suitableCardRepo,
                    this.tokenProvider,
                    this.typesCache,
                    this.serializer,
                    this.getStrategy,
                    suitableTransactionStrategy,
                    this.dbScope,
                    this.cardMetadata,
                    suitableStreamRepo,
                    levelValidationResult,
                    withReaderLocks);
            var scopeContext = KrScopeContext.Current;
            if (MaxDepth < scopeContext.LevelStack.Count)
            {
                scopeContext.LevelStack.Pop();
                var text = LocalizationManager.Format("$KrProcess_MaximumKrScopeDepth", "$CardTypes_Controls_RunOnce");
                throw new InvalidOperationException(text);
            }
            return level;
        }

        /// <inheritdoc />
        public Card GetMainCard(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null,
            bool withoutTransaction = false)
        {
            var scopeContext = KrScopeContext.Current;
            if (scopeContext == null)
            {
                // в случае KrScope основная карточка загружается
                return this.GetMainCardInternal(
                    mainCardID,
                    validationResult,
                    withoutTransaction ? this.cardRepositoryEwt : this.cardRepositoryExt);
            }

            if (scopeContext.Cards.TryGetValue(mainCardID, out var card))
            {
                return card;
            }

            card = this.GetMainCardInternal(mainCardID, validationResult ?? scopeContext.ValidationResult, this.cardRepositoryEwt);
            if (card != null)
            {
                scopeContext.Cards[mainCardID] = card;
            }
            return card;
        }

        /// <inheritdoc />
        public ICardFileContainer GetMainCardFileContainer(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            if (scopeContext.CardFileContainers.TryGetValue(mainCardID, out var container))
            {
                return container;
            }

            var card = this.GetMainCard(mainCardID, validationResult ?? scopeContext.ValidationResult);
            if (card != null)
            {
                container = this.GetFileContainerInternal(card, validationResult ?? scopeContext.ValidationResult);
                if (container != null)
                {
                    scopeContext.CardFileContainers[mainCardID] = container;
                }
            }

            return container;
        }

        /// <inheritdoc />
        public void ForceIncrementMainCardVersion(
            Guid mainCardID)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);
            scopeContext.ForceIncrementCardVersion.Add(mainCardID);
        }

        /// <inheritdoc />
        public void EnsureMainCardHasTaskHistory(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            if (scopeContext.CardsWithTaskHistory.Contains(mainCardID))
            {
                return;
            }

            if (!scopeContext.Cards.TryGetValue(mainCardID, out var card))
            {
                throw new InvalidOperationException($"Card {mainCardID} not found in KrScope");
            }

            this.LoadTaskHistory(card, validationResult ?? scopeContext.ValidationResult);
            scopeContext.CardsWithTaskHistory.Add(mainCardID);
        }

        /// <inheritdoc />
        public Card GetKrSatellite(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            if (scopeContext == null)
            {
                return this.GetSatellite(
                    mainCardID,
                    validationResult ?? new ValidationResultBuilder(),
                    this.cardRepositoryExt);
            }

            if (scopeContext.MainKrSatellites.TryGetItem(mainCardID, out var satellite))
            {
                return satellite;
            }

            satellite = this.GetSatellite(
                mainCardID,
                validationResult ?? scopeContext.ValidationResult,
                this.cardRepositoryEwt);
            if (satellite != null)
            {
                scopeContext.MainKrSatellites.Add(satellite);
            }

            return satellite;
        }

        /// <inheritdoc />
        public void StoreSatelliteExplicitly(
            Guid mainCardID,
            ValidationResultBuilder validationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            var currentSatellite = this.GetKrSatellite(mainCardID, validationResult);
            var copy = currentSatellite.Clone();
            copy.RemoveAllButChanged(copy.StoreMode);
            var request = new CardStoreRequest { Card = copy };
            var response = this.cardRepositoryEwt.StoreAsync(request).GetAwaiter().GetResult(); // TODO async

            if (validationResult != null)
            {
                validationResult.Add(response.ValidationResult);
            }
            else
            {
                scopeContext.ValidationResult.Add(response.ValidationResult);
            }

            currentSatellite.RemoveChanges(CardRemoveChangesDeletedHandling.Remove);
            currentSatellite.Version = response.CardVersion;
        }

        /// <inheritdoc />
        public Guid? GetCurrentHistoryGroup(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            var satellite = this.GetKrSatellite(mainCardID, validationResult);
            if (satellite is null)
            {
                return null;
            }

            return satellite.TryGetKrApprovalCommonInfoSection(out var aci)
                && aci.RawFields.TryGetValue(KrConstants.KrApprovalCommonInfo.CurrentHistoryGroup, out var chgObj)
                    ? chgObj as Guid?
                    : null;
        }

        /// <inheritdoc />
        public void SetCurrentHistoryGroup(
            Guid mainCardID,
            Guid? newGroupHistoryID,
            IValidationResultBuilder validationResult = null)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            var satellite = this.GetKrSatellite(mainCardID, validationResult);
            if (satellite != null
                && satellite.TryGetKrApprovalCommonInfoSection(out var aci))
            {
                aci.Fields[KrConstants.KrApprovalCommonInfo.CurrentHistoryGroup] = newGroupHistoryID;
            }
        }

        /// <inheritdoc />
        public Card CreateSecondaryKrSatellite(
            Guid mainCardID,
            Guid processID)
        {
            var scopeContext = KrScopeContext.Current;
            if (scopeContext == null)
            {
                throw new InvalidOperationException("KrScopeContext == null");
            }

            if (scopeContext.SecondaryKrSatellites.ContainsKey(processID))
            {
                throw new InvalidOperationException("Satellite already exists.");
            }

            var newSatelliteRequest = new CardNewRequest { CardTypeID = DefaultCardTypes.KrSecondarySatelliteTypeID };
            var response = this.cardRepositoryEwt.NewAsync(newSatelliteRequest).GetAwaiter().GetResult(); // TODO async
            response.Card.ID = Guid.NewGuid();
            scopeContext.ValidationResult.Add(response.ValidationResult.Build());
            var satellite = response.Card;
            if (response.ValidationResult.IsSuccessful())
            {
                SetMainCardID(response.Card, mainCardID);
            }
            else
            {
                return null;
            }
            var copy = satellite.Clone();
            copy.RemoveAllButChanged(copy.StoreMode);
            var storeRequest = new CardStoreRequest { Card = copy };
            var storeResponse = this.cardRepositoryEwt.StoreAsync(storeRequest).GetAwaiter().GetResult(); // TODO async
            scopeContext.ValidationResult.Add(storeResponse.ValidationResult.Build());
            satellite.RemoveChanges(CardRemoveChangesDeletedHandling.Remove);
            satellite.Version = storeResponse.CardVersion;
            scopeContext.SecondaryKrSatellites[processID] = satellite;

            return satellite;
        }

        /// <inheritdoc />
        public Card GetSecondaryKrSatellite(
            Guid processID)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            if (scopeContext.SecondaryKrSatellites.TryGetValue(processID, out var satellite))
            {
                return satellite;
            }

            var db = this.dbScope.Db;
            var getCardIDQuery = this.dbScope.BuilderFactory
                .Select()
                .C("ID")
                .From("WorkflowProcesses").NoLock()
                .Where().C("RowID").Equals().P("processID")
                .Build();
            var id = db
                .SetCommand(getCardIDQuery, db.Parameter("processID", processID))
                .LogCommand()
                .Execute<Guid>();

            var request = new CardGetRequest
            {
                CardID = id,
                CardTypeID = DefaultCardTypes.KrSecondarySatelliteTypeID,
                GetMode = CardGetMode.ReadOnly,
                RestrictionFlags = CardGetRestrictionValues.Satellite,
            };
            var response = this.cardRepositoryEwt.GetAsync(request).GetAwaiter().GetResult(); // TODO async
            scopeContext.ValidationResult.Add(response.ValidationResult);

            satellite = response.ValidationResult.IsSuccessful()
                ? response.Card
                : null;
            scopeContext.SecondaryKrSatellites[processID] = satellite;

            return satellite;
        }

        /// <inheritdoc />
        public Guid? LockCard(
            Guid cardID)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            if (scopeContext.Locks.Contains(cardID))
            {
                return null;
            }

            var key = Guid.NewGuid();
            scopeContext.Locks.Add(cardID);
            scopeContext.LockKeys[cardID] = key;
            return key;
        }

        /// <inheritdoc />
        public bool IsCardLocked(
            Guid cardID)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            return scopeContext.Locks.Contains(cardID);
        }

        /// <inheritdoc />
        public bool ReleaseCard(
            Guid cardID,
            Guid? key)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            if (!scopeContext.Locks.Contains(cardID)
                || scopeContext.LockKeys.TryGetValue(cardID, out var cardKey) && cardKey != key)
            {
                return false;
            }

            scopeContext.Locks.Remove(cardID);
            scopeContext.LockKeys.Remove(cardID);
            return true;
        }

        /// <inheritdoc />
        public void AddProcessHolder(
            ProcessHolder processHolder)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            scopeContext.ProcessHolders.Add(processHolder);
        }

        /// <inheritdoc />
        public ProcessHolder GetProcessHolder(
            Guid processHolderID)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            return scopeContext.ProcessHolders.TryGetItem(processHolderID, out var holder)
                ? holder
                : null;
        }

        /// <inheritdoc />
        public void RemoveProcessHolder(
            Guid processHolderID)
        {
            var scopeContext = KrScopeContext.Current;
            AssertNonEmptyKrScopeContext(scopeContext);

            scopeContext.ProcessHolders.RemoveByKey(processHolderID);
        }

        #endregion

        #region private

        private Card GetSatellite(
            Guid cardID,
            IValidationResultBuilder validationResult,
            ICardRepository cardRepository)
        {
            // На сателлит права не используются, поэтому токен ему не нужен.

            var satelliteExists = KrProcessHelper.SatelliteExistsAsync(cardID, this.dbScope).GetAwaiter().GetResult(); // TODO async
            Card satellite;
            if (satelliteExists)
            {
                var request = new CardGetRequest
                {
                    CardID = cardID,
                    CardTypeID = DefaultCardTypes.KrSatelliteTypeID,
                    GetMode = CardGetMode.ReadOnly,
                    RestrictionFlags = CardGetRestrictionValues.Satellite,
                };
                var response = cardRepository.GetAsync(request).GetAwaiter().GetResult(); // TODO async
                validationResult.Add(response.ValidationResult);

                satellite = response.ValidationResult.IsSuccessful()
                    ? response.Card
                    : null;
            }
            else
            {
                var newSatelliteRequest = new CardNewRequest { CardTypeID = DefaultCardTypes.KrSatelliteTypeID };
                var response = cardRepository.NewAsync(newSatelliteRequest).GetAwaiter().GetResult(); // TODO async
                validationResult.Add(response.ValidationResult.Build());
                if (response.ValidationResult.IsSuccessful())
                {
                    response.Card.ID = Guid.NewGuid();
                    SetMainCardID(response.Card, cardID);
                    satellite = response.Card;
                }
                else
                {
                    satellite = null;
                }
            }
            return satellite;
        }

        private static void SetMainCardID(Card satelliteCard, Guid mainCardID)
        {
            if (!satelliteCard.TryGetKrApprovalCommonInfoSection(out var satelliteInfoSection))
            {
                return;
            }
            satelliteInfoSection.SetIfDiffer(KrConstants.KrProcessCommonInfo.MainCardID, mainCardID);
        }

        private Card GetMainCardInternal(
            Guid mainCardID,
            IValidationResultBuilder validationResult,
            ICardRepository cardRepository)
        {
            var request = new CardGetRequest
            {
                CardID = mainCardID,
                GetMode = CardGetMode.ReadOnly,
                RestrictionFlags = CardGetRestrictionFlags.RestrictTasks | CardGetRestrictionFlags.RestrictTaskHistory,
            };
            request.IgnoreButtons();
            request.IgnoreKrSatellite();
            request.SetForbidStoringHistory(true);
            // Основной карточке создаем токен, чтобы процесс мог к ней обращаться
            // вне зависимости от прав юзера
            var token = this.tokenProvider.CreateToken(mainCardID);
            token.Set(request.Info);

            var response = cardRepository.GetAsync(request).GetAwaiter().GetResult(); // TODO async
            validationResult.Add(response.ValidationResult);

            return response.ValidationResult.IsSuccessful()
                ? response.Card
                : null;
        }

        private ICardFileContainer GetFileContainerInternal(
            Card card,
            IValidationResultBuilder validationResult)
        {
            ICardFileContainer container = null;

            try
            {
                container = this.fileManager.CreateContainerAsync(card).GetAwaiter().GetResult(); // TODO async
                if (!container.CreationResult.IsSuccessful)
                {
                    return null;
                }

                ICardFileContainer result = container;
                container = null;

                return result;
            }
            finally
            {
                container?.Dispose();
            }
        }

        private void LoadTaskHistory(Card card, IValidationResultBuilder validationResult)
        {
            var insertedHistoryItems = card
                .TaskHistory
                .Where(p => p.State == CardTaskHistoryState.Inserted)
                .ToList();
            var insertedGroupItems = card
                .TaskHistoryGroups
                .Where(p => p.State == CardTaskHistoryState.Inserted)
                .ToList();

            card.TaskHistory.Clear();
            card.TaskHistoryGroups.Clear();

            using (this.dbScope.Create())
            {
                this.getStrategy.LoadTaskHistoryAsync(
                    card.ID,
                    card,
                    this.dbScope.Db,
                    this.cardMetadata,
                    validationResult,
                    new Dictionary<Guid, CardTask>()).GetAwaiter().GetResult(); // TODO async
            }

            card.TaskHistory.AddRange(insertedHistoryItems);
            card.TaskHistoryGroups.AddRange(insertedGroupItems);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void AssertNonEmptyKrScopeContext(KrScopeContext scopeContext)
        {
            if (scopeContext is null)
            {
                throw new InvalidOperationException("KrScopeContext == null");
            }
        }

        #endregion
    }
}