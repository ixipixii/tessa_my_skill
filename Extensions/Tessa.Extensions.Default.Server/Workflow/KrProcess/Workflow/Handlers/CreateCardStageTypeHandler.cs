using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Numbers;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants.KrCreateCardStageSettingsVirtual;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class CreateCardStageTypeHandler : StageTypeHandlerBase
    {
        #region Nested Types

        public enum CreateCardMode
        {
            Open = 0,
            StoreAndOpen = 1,
            StartProcess = 2,
            StartProcessAndOpen = 3,
            Store = 4
        }

        protected struct CreateCardStageLocalContext
        {
            public Guid? TemplateID;
            public Guid? TypeID;
            public string TypeCaption;
            public CreateCardMode Mode;
            public bool Valid;
        }

        #endregion

        #region Constructors

        public CreateCardStageTypeHandler(
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository cardRepositoryEwt,
            [Dependency(CardRepositoryNames.DefaultWithoutTransaction)] ICardRepository cardRepositoryDwt,
            ICardMetadata cardMetadata,
            ICardManager cardManager,
            IKrTokenProvider tokenProvider,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardFileManager fileManager,
            IDbScope dbScope,
            IKrScope krScope,
            IKrTypesCache typesCache,
            ISignatureProvider signatureProvider)
        {
            this.CardRepositoryEwt = cardRepositoryEwt;
            this.CardRepositoryDwt = cardRepositoryDwt;
            this.TokenProvider = tokenProvider;
            this.FileManager = fileManager;
            this.DbScope = dbScope;
            this.KrScope = krScope;
            this.TypesCache = typesCache;
            this.SignatureProvider = signatureProvider;
        }

        #endregion

        #region Properties

        protected ICardRepository CardRepositoryEwt { get; set; }

        protected ICardRepository CardRepositoryDwt { get; set; }

        protected IKrTokenProvider TokenProvider { get; set; }

        protected ICardFileManager FileManager { get; set; }

        protected IDbScope DbScope { get; set; }

        protected IKrScope KrScope { get; set; }

        protected IKrTypesCache TypesCache { get; set; }

        protected ISignatureProvider SignatureProvider { get; set; }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var localCtx = this.GetLocalContext(context);
            if (!localCtx.Valid)
            {
                return StageHandlerResult.CompleteResult;
            }

            if (localCtx.Mode == CreateCardMode.Open)
            {
                this.CreateEmptyCard(context, localCtx);
                return StageHandlerResult.CompleteResult;
            }

            var card = this.CreateCard(context, localCtx);
            if (card is null)
            {
                return StageHandlerResult.CompleteResult;
            }
            context.Stage.InfoStorage[KrConstants.Keys.NewCardID] = card.ID;
            if (localCtx.Mode == CreateCardMode.StoreAndOpen
                || localCtx.Mode == CreateCardMode.StartProcessAndOpen)
            {
                this.KrScope.TryAddClientCommand(new KrProcessClientCommand(
                    DefaultCommandTypes.OpenCard,
                    new Dictionary<string, object>
                    {
                        [KrConstants.Keys.NewCardID] = card.ID,
                    }));
            }

            if (localCtx.Mode == CreateCardMode.StartProcess
                || localCtx.Mode == CreateCardMode.StartProcessAndOpen)
            {
                this.StartProcess(context, card);
            }

            return StageHandlerResult.CompleteResult;
        }


        /// <inheritdoc />
        public override void AfterPostprocessing(
            IStageTypeHandlerContext context)
        {
            var localCtx = this.GetLocalContext(context);
            if (!localCtx.Valid)
            {
                return;
            }

            if (localCtx.Mode == CreateCardMode.Open)
            {
                var newCardAccessStrategy = context.Stage.InfoStorage.TryGet<IMainCardAccessStrategy>(KrConstants.Keys.NewCard);
                var newCard = newCardAccessStrategy.GetCard();
                context.Stage.InfoStorage.Remove(KrConstants.Keys.NewCard);
                newCard.RemoveAllButChanged();
                var serialized = newCard.ToSerializable().Serialize();
                var sign = this.SignatureProvider.Sign(serialized);

                var info = new Dictionary<string, object>
                {
                    [KrConstants.Keys.NewCard] = serialized,
                    [KrConstants.Keys.NewCardSignature] = sign,
                };
                if (localCtx.TemplateID.HasValue)
                {
                    info[KrConstants.Keys.TemplateID] = localCtx.TemplateID;
                    this.KrScope.TryAddClientCommand(
                        new KrProcessClientCommand(DefaultCommandTypes.CreateCardViaTemplate, info));
                }
                else
                {
                    var docType = this.TypesCache.GetDocTypesAsync().GetAwaiter().GetResult() // TODO async
                        .FirstOrDefault(x => x.ID == localCtx.TypeID);

                    if (docType != null)
                    {
                        info.Add(KrConstants.Keys.TypeID, docType.CardTypeID);
                        info.Add(KrConstants.Keys.DocTypeID, localCtx.TypeID);
                        info.Add(KrConstants.Keys.DocTypeTitle, localCtx.TypeCaption);
                    }
                    else
                    {
                        info.Add(KrConstants.Keys.TypeID, localCtx.TypeID);
                        info.Add(KrConstants.Keys.TypeCaption, localCtx.TypeCaption);
                    }
                    this.KrScope.TryAddClientCommand(new KrProcessClientCommand(
                        DefaultCommandTypes.CreateCardViaDocType,
                        info));
                }
            }
        }

        #endregion

        #region protected

        protected CreateCardStageLocalContext GetLocalContext(
            IStageTypeHandlerContext context)
        {
            var modeID = context.Stage.SettingsStorage.TryGet<int?>(ModeID);
            var localCtx = new CreateCardStageLocalContext
            {
                TemplateID = context.Stage.SettingsStorage.TryGet<Guid?>(TemplateID),
                TypeID = context.Stage.SettingsStorage.TryGet<Guid?>(TypeID),
                TypeCaption = context.Stage.SettingsStorage.TryGet<string>(TypeCaption),
            };

            if (!localCtx.TemplateID.HasValue && !localCtx.TypeID.HasValue)
            {
                context.ValidationResult.AddError(this, "$KrStages_CreateCard_TemplateAndTypeRequired");
                return localCtx;
            }

            if (localCtx.TemplateID.HasValue && localCtx.TypeID.HasValue)
            {
                context.ValidationResult.AddError(this, "$KrStages_CreateCard_TemplateAndTypeSelected");
                return localCtx;
            }

            if (!modeID.HasValue)
            {
                context.ValidationResult.AddError(this, "$KrStages_CreateCard_ModeRequired");
                return localCtx;
            }

            localCtx.Mode = (CreateCardMode)modeID;
            localCtx.Valid = true;
            return localCtx;
        }

        protected void CreateEmptyCard(
            IStageTypeHandlerContext context,
            CreateCardStageLocalContext localCtx)
        {
            Guid typeID;
            if (localCtx.TemplateID.HasValue)
            {
                var nullableTypeID = KrProcessHelper.GetTemplateCardTypeAsync(localCtx.TemplateID.Value, this.DbScope).GetAwaiter().GetResult(); // TODO async
                if (!nullableTypeID.HasValue)
                {
                    context.ValidationResult.AddError(this, "$KrStages_CreateCard_TemplateNotFound");
                    return;
                }

                typeID = nullableTypeID.Value;
            }
            else if (localCtx.TypeID.HasValue)
            {
                typeID = this.GetCardType(localCtx.TypeID.Value);
            }
            else
            {
                context.ValidationResult.AddError(this, "$KrStages_CreateCard_TemplateAndTypeNotSpecified");
                return;
            }

            var newRequest = new CardNewRequest { CardTypeID = typeID };
            var newResponse = this.CardRepositoryDwt.NewAsync(newRequest).GetAwaiter().GetResult(); // TODO async
            context.ValidationResult.Add(newResponse.ValidationResult);
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }
            context.Stage.InfoStorage[KrConstants.Keys.NewCard] = new ObviousMainCardAccessStrategy(newResponse.Card);
        }

        protected Card CreateCard(
            IStageTypeHandlerContext context,
            CreateCardStageLocalContext localCtx)
        {
            var newRequest = new CardNewRequest();
            if (localCtx.TemplateID.HasValue)
            {
                var docTypeID = KrProcessHelper.GetTemplateDocTypeAsync(localCtx.TemplateID.Value, this.DbScope).GetAwaiter().GetResult(); // TODO async
                if (!docTypeID.HasValue)
                {
                    return null;
                }

                newRequest.CardTypeID = CardHelper.TemplateTypeID;
                this.TokenProvider.CreateToken(docTypeID.Value).Set(newRequest.Info);
                newRequest.SetTemplateCardID(localCtx.TemplateID);
            }
            else
            {
                var docType = this.TypesCache.GetDocTypesAsync().GetAwaiter().GetResult()
                    .FirstOrDefault(x => x.ID == localCtx.TypeID);

                KrToken token;
                if (docType != null)
                {
                    newRequest.CardTypeID = docType.CardTypeID;
                    newRequest.Info[KrConstants.Keys.DocTypeID] = localCtx.TypeID;
                    newRequest.Info[KrConstants.Keys.DocTypeTitle] = localCtx.TypeCaption;
                    token = this.TokenProvider.CreateToken(docType.ID);
                }
                else
                {
                    newRequest.CardTypeID = localCtx.TypeID;
                    // ReSharper disable once PossibleInvalidOperationException
                    token = this.TokenProvider.CreateToken(newRequest.CardTypeID.Value);
                }

                token.Set(newRequest.Info);
            }
            var newResponse = this.CardRepositoryEwt.NewAsync(newRequest).GetAwaiter().GetResult(); // TODO async
            context.ValidationResult.Add(newResponse.ValidationResult);
            if (!context.ValidationResult.IsSuccessful())
            {
                return null;
            }
            var card = newResponse.Card;
            var cardID = Guid.NewGuid();
            card.ID = cardID;
            int version;
            using (var container = this.FileManager.CreateContainerAsync(card).GetAwaiter().GetResult()) // TODO async
            {
                var storeResponse = container.StoreAsync(async (c, request, ct) =>
                {
                    var token = this.TokenProvider.CreateToken(request.Card.ID);
                    token.Set(request.Card.Info);
                    var digest = await this.CardRepositoryEwt.GetDigestAsync(request.Card, CardDigestEventNames.ActionHistoryStoreRouteCreateCard, ct);
                    if (digest != null)
                    {
                        request.SetDigest(digest);
                    }
                }).GetAwaiter().GetResult(); // TODO async
                version = storeResponse.CardVersion;
                context.ValidationResult.Add(storeResponse.ValidationResult);
                if (!storeResponse.ValidationResult.IsSuccessful())
                {
                    return null;
                }
            }

            card.Version = version;
            return card;
        }

        protected void StartProcess(
            IStageTypeHandlerContext context,
            Card card)
        {
            card.RemoveChanges(CardRemoveChangesDeletedHandling.Remove);
            card.RemoveNumberQueue();
            card.RemoveWorkflowQueue();

            card.RemoveAllButChanged(card.StoreMode);
            var storeInfo = new Dictionary<string, object>(StringComparer.Ordinal);
            var token = this.TokenProvider.CreateToken(card);
            token.Set(card.Info);
            storeInfo.SetStartingProcessName(KrConstants.KrProcessName);
            var startProcessRequest = new CardStoreRequest { Card = card, Info = storeInfo };
            startProcessRequest.SetIgnorePermissionsWarning();
            var startProcessResponse = this.CardRepositoryEwt.StoreAsync(startProcessRequest).GetAwaiter().GetResult(); // TODO async
            context.ValidationResult.Add(startProcessResponse.ValidationResult.Build());
        }

        protected Guid GetCardType(Guid docOrCardTypeID)
        {
            var docType = this.TypesCache.GetDocTypesAsync().GetAwaiter().GetResult()
                .FirstOrDefault(x => x.ID == docOrCardTypeID);

            return docType?.CardTypeID ?? docOrCardTypeID;
        }

        #endregion

    }
}