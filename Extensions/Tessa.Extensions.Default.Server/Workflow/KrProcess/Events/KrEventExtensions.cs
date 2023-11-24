using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    public static class KrEventExtensions
    {
        public static IExtensionContainer RegisterKrEventExtensionTypes(
            this IExtensionContainer extensionContainer)
        {
            return extensionContainer
                .RegisterType<IKrEventExtension>(x => x
                    .MethodAsync<IKrEventExtensionContext>(y => y.HandleEvent),
                    x => x.Register(KrEventFilterPolicy.Instance));
        }

        public static IExtensionPolicyContainer WhenEventType(
            this IExtensionPolicyContainer policyContainer,
            params string[] eventTypes)
        {
            if (policyContainer is null)
            {
                throw new ArgumentNullException(nameof(policyContainer));
            }

            return policyContainer
                .Register(new KrEventPolicy(eventTypes));
        }


        public static Task RaiseAsync(
            this IKrEventManager manager,
            string eventType,
            Stage currentStage,
            KrProcessRunnerMode runnerMode,
            IKrProcessRunnerContext runnerContext,
            IDictionary<string, object> info = null,
            CancellationToken cancellationToken = default)
        {
            var context = new KrEventExtensionContext(
                eventType,
                info ?? new Dictionary<string, object>(),
                runnerContext.CardID,
                runnerContext.CardTypeID,
                runnerContext.CardTypeName,
                runnerContext.CardTypeCaption,
                runnerContext.DocTypeID,
                runnerContext.MainCardAccessStrategy,
                runnerContext.SecondaryProcess,
                runnerContext.ContextualSatellite,
                runnerContext.ProcessHolderSatellite,
                runnerContext.CardContext,
                runnerContext.ValidationResult,
                currentStage,
                runnerContext.WorkflowProcess,
                runnerContext.ProcessHolder,
                runnerContext.ProcessInfo,
                runnerMode,
                runnerContext.InitiationCause,
                cancellationToken);
            return manager.RaiseAsync(context);
        }

        public static Task RaiseAsync(
            this IKrEventManager manager,
            string eventType,
            IStageTypeHandlerContext handlerContext,
            IDictionary<string, object> info = null,
            CancellationToken cancellationToken = default)
        {
            var context = new KrEventExtensionContext(
                eventType,
                info ?? new Dictionary<string, object>(),
                handlerContext.MainCardID,
                handlerContext.MainCardTypeID,
                handlerContext.MainCardTypeName,
                handlerContext.MainCardTypeCaption,
                handlerContext.MainCardDocTypeID,
                handlerContext.MainCardAccessStrategy,
                handlerContext.SecondaryProcess,
                handlerContext.ContextualSatellite,
                handlerContext.ProcessHolderSatellite,
                handlerContext.CardExtensionContext,
                handlerContext.ValidationResult,
                handlerContext.Stage,
                handlerContext.WorkflowProcess,
                handlerContext.ProcessHolder,
                handlerContext.ProcessInfo,
                handlerContext.RunnerMode,
                handlerContext.InitiationCause,
                cancellationToken);
            return manager.RaiseAsync(context);
        }

        public static Task RaiseAsync(
            this IKrEventManager manager,
            string eventType,
            IGlobalSignalHandlerContext stateHandler,
            IDictionary<string, object> info = null,
            CancellationToken cancellationToken = default)
        {
            var runnerContext = stateHandler.RunnerContext;
            var context = new KrEventExtensionContext(
                eventType,
                info ?? new Dictionary<string, object>(),
                runnerContext.CardID,
                runnerContext.CardTypeID,
                runnerContext.CardTypeName,
                runnerContext.CardTypeCaption,
                runnerContext.DocTypeID,
                runnerContext.MainCardAccessStrategy,
                runnerContext.SecondaryProcess,
                runnerContext.ContextualSatellite,
                runnerContext.ProcessHolderSatellite,
                runnerContext.CardContext,
                runnerContext.ValidationResult,
                stateHandler.Stage,
                runnerContext.WorkflowProcess,
                runnerContext.ProcessHolder,
                runnerContext.ProcessInfo,
                stateHandler.RunnerMode,
                runnerContext.InitiationCause,
                cancellationToken);
            return manager.RaiseAsync(context);
        }

        public static Task RaiseAsync(
            this IKrEventManager manager,
            string eventType,
            IStateHandlerContext stateHandler,
            IDictionary<string, object> info = null,
            CancellationToken cancellationToken = default)
        {
            var runnerContext = stateHandler.RunnerContext;
            var context = new KrEventExtensionContext(
                eventType,
                info ?? new Dictionary<string, object>(),
                runnerContext.CardID,
                runnerContext.CardTypeID,
                runnerContext.CardTypeName,
                runnerContext.CardTypeCaption,
                runnerContext.DocTypeID,
                runnerContext.MainCardAccessStrategy,
                runnerContext.SecondaryProcess,
                runnerContext.ContextualSatellite,
                runnerContext.ProcessHolderSatellite,
                runnerContext.CardContext,
                runnerContext.ValidationResult,
                stateHandler.Stage,
                runnerContext.WorkflowProcess,
                runnerContext.ProcessHolder,
                runnerContext.ProcessInfo,
                stateHandler.RunnerMode,
                runnerContext.InitiationCause,
                cancellationToken);
            return manager.RaiseAsync(context);
        }

        public static Task RaiseAsync(
            this IKrEventManager manager,
            string eventType,
            Guid cardID,
            IMainCardAccessStrategy cardAccessStrategy,
            ICardExtensionContext extensionContext,
            IValidationResultBuilder validationResult,
            IDictionary<string, object> info = null,
            CancellationToken cancellationToken = default)
        {
            var context = new KrEventExtensionContext(
                eventType: eventType,
                info: info ?? new Dictionary<string, object>(),
                mainCardID: cardID,
                mainCardTypeID: null,
                mainCardTypeName: null,
                mainCardTypeCaption: null,
                mainCardDocTypeID: null,
                mainCardAccessStrategy: cardAccessStrategy,
                secondaryProcess: null,
                contextualSatellite: null,
                processHolderSatellite: null,
                cardExtensionContext: extensionContext,
                validationResult: validationResult,
                stage: null,
                workflowProcess: null,
                processHolder: null,
                processInfo: null,
                runnerMode: null,
                initiationCause: null,
                cancellationToken: cancellationToken);
            return manager.RaiseAsync(context);
        }

    }
}