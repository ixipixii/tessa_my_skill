using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.Extensions.Default.Shared.Workflow.WorkflowEngine;
using Tessa.Platform.Validation;
using Tessa.Workflow;
using Tessa.Workflow.Actions;
using Tessa.Workflow.Compilation;
using Tessa.Workflow.Helpful;
using Tessa.Workflow.Signals;
using Tessa.Workflow.Storage;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.WorkflowEngine
{
    /// <summary>
    /// Класс для действия Создать карточку
    /// </summary>
    public class WorkflowCreateCardAction : WorkflowActionBase
    {
        #region Fields

        private readonly IKrTypesCache typesCache;
        private readonly ICardRepository cardRepository;
        private readonly ICardStreamServerRepository cardStreamServerRepository;
        private readonly IWorkflowEngineCardRequestExtender requestExtender;
        private readonly ICardFileManager cardFileManager;

        public const string MainSection = "KrCreateCardAction";
        private const string InitMethodName = "InitCard";

        #endregion

        #region Constructors

        public WorkflowCreateCardAction(
            IKrTypesCache typesCache,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository cardRepository,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardStreamServerRepository cardStreamServerRepository,
            IWorkflowEngineCardRequestExtender requestExtender,
            ICardFileManager cardFileManager)
            : base(KrDescriptors.CreateCardDescriptor)
        {
            this.typesCache = typesCache;
            this.cardRepository = cardRepository;
            this.cardStreamServerRepository = cardStreamServerRepository;
            this.requestExtender = requestExtender;
            this.cardFileManager = cardFileManager;
        }

        #endregion

        #region Base Overrides

        public override void PrepareForExecute(
            WorkflowActionStateStorage actionState,
            IWorkflowEngineContext context)
        {
            var mainSection = WorkflowEngineHelper.Get<Dictionary<string, object>>(actionState.Hash, MainSection);
            if (mainSection != null)
            {
                mainSection.Remove("Script");
            }
        }

        protected override async Task ExecuteAsync(
            IWorkflowEngineContext context,
            IWorkflowEngineCompiled scriptObject)
        {
            if (context.Signal.Type != WorkflowSignalTypes.Default)
            {
                return;
            }

            var templateID = await context.GetAsync<Guid?>(MainSection, "Template", "ID");
            var typeID = await context.GetAsync<Guid?>(MainSection, "Type", "ID");
            var openCard = await context.GetAsync<bool?>(MainSection, "OpenCard") ?? false;
            var setAsMainCard = await context.GetAsync<bool?>(MainSection, "SetAsMainCard") ?? false;

            Card newCard;
            if (templateID.HasValue)
            {
                newCard = await CreateCardFromTemplateAsync(templateID.Value, context.ValidationResult, context.CancellationToken);
            }
            else if (typeID.HasValue)
            {
                newCard = await CreateCardFromTypeAsync(typeID.Value, context.ValidationResult, context.CancellationToken);
            }
            else
            {
                return;
            }

            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            newCard.ID = Guid.NewGuid();
            if (openCard)
            {
                OpenCard(context, newCard.ID);
            }

            using var fileContainer = await this.cardFileManager.CreateContainerAsync(newCard);

            if (scriptObject != null)
            {
                await scriptObject.ExecuteActionAsync(
                    InitMethodName,
                    newCard.DynamicEntries,
                    newCard.DynamicTables,
                    newCard,
                    fileContainer.FileContainer);
            }

            await this.StoreNewCardAsync(newCard, fileContainer, context.ValidationResult);
            if (setAsMainCard)
            {
                context.SetMainCard(newCard.ID);
            }
        }

        #endregion

        #region Private Methods

        private static void OpenCard(IWorkflowEngineContext context, Guid cardID)
        {
            if (!(context.ResponseInfo.TryGetValue(KrProcessSharedExtensions.KrProcessClientCommandInfoMark, out var commandsObj)
                && commandsObj is List<object> commands))
            {
                commands = new List<object>();
                context.ResponseInfo[KrProcessSharedExtensions.KrProcessClientCommandInfoMark] = commands;
            }

            commands.Add(
                new KrProcessClientCommand(
                    DefaultCommandTypes.OpenCard,
                    new Dictionary<string, object>
                    {
                        ["cardID"] = cardID,
                    }).GetStorage());
        }

        private async Task StoreNewCardAsync(
            Card newCard, 
            ICardFileContainer fileContainer, 
            IValidationResultBuilder validationResult, 
            CancellationToken cancellationToken = default)
        {
            var storeRequest = new CardStoreRequest()
            {
                Card = newCard,
            };
            requestExtender.ExtendStoreRequest(storeRequest);

            var response = await CardHelper.StoreAsync(storeRequest, fileContainer.FileContainer, cardRepository, cardStreamServerRepository, cancellationToken);
            validationResult.Add(response.ValidationResult);
        }

        private async Task<Card> CreateCardFromTypeAsync(Guid typeID, IValidationResultBuilder validationResult, CancellationToken cancellationToken = default)
        {
            var newRequest = new CardNewRequest();
            var docType = (await typesCache.GetDocTypesAsync(cancellationToken))
                .FirstOrDefault(x => x.ID == typeID);

            if (docType != null)
            {
                newRequest.CardTypeID = docType.CardTypeID;
                newRequest.Info[KrConstants.Keys.DocTypeID] = typeID;
                newRequest.Info[KrConstants.Keys.DocTypeTitle] = docType.Caption;
            }
            else
            {
                newRequest.CardTypeID = typeID;
            }

            requestExtender.ExtendNewRequest(newRequest);

            var response = await cardRepository.NewAsync(newRequest, cancellationToken);
            validationResult.Add(response.ValidationResult);

            return response.Card;
        }

        private async Task<Card> CreateCardFromTemplateAsync(Guid templateID, IValidationResultBuilder validationResult, CancellationToken cancellationToken = default)
        {
            var newRequest = new CardNewRequest()
            {
                CardTypeID = CardHelper.TemplateTypeID,
            };
            newRequest.SetTemplateCardID(templateID);

            var response = await cardRepository.NewAsync(newRequest, cancellationToken);
            validationResult.Add(response.ValidationResult);

            return response.Card;
        }

        #endregion
    }
}
