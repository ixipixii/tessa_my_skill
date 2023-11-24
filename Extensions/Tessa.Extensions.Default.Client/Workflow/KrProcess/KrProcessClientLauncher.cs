using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public sealed class KrProcessClientLauncher : IKrProcessLauncher
    {
        #region nested types

        public sealed class SpecificParameters : IKrProcessLauncherSpecificParameters
        {
            /// <summary>
            /// Использовать текущий CardEditor. Приоритет выше, чем у <see cref="CardEditor"/>.
            /// </summary>
            public bool UseCurrentCardEditor { get; set; } = false;

            /// <summary>
            /// Использовать указанный CardEditor. Приоритет ниже, чем у <see cref="UseCurrentCardEditor"/>.
            /// </summary>
            public ICardEditorModel CardEditor { get; set; } = null;

            /// <summary>
            /// Info реквеста.
            /// </summary>
            public IDictionary<string, object> RequestInfo { get; set; } = null;
        }

        #endregion

        #region fields

        private readonly ICardRepository cardRepository;

        #endregion

        #region constructor

        public KrProcessClientLauncher(
            ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IKrProcessLaunchResult Launch(
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext = null,
            IKrProcessLauncherSpecificParameters specificParameters = null)
        {
            return this.LaunchAsync(krProcess, cardContext, specificParameters).GetAwaiter().GetResult(); // TODO async
        }

        /// <inheritdoc />
        public async Task<IKrProcessLaunchResult> LaunchAsync(
            KrProcessInstance krProcess,
            ICardExtensionContext cardContext = null,
            IKrProcessLauncherSpecificParameters specificParameters = null)
        {
            var withinCardEditor = (specificParameters as SpecificParameters)?.UseCurrentCardEditor ?? false;
            var specificCardEditor = (specificParameters as SpecificParameters)?.CardEditor;
            var requestInfo = (specificParameters as SpecificParameters)?.RequestInfo;

            ICardEditorModel cardEditor = null;
            IUIContext uiContext = null;
            if (withinCardEditor)
            {
                var context = UIContext.Current;
                var currentEditor = context?.CardEditor;
                if (currentEditor is null)
                {
                    throw new InvalidOperationException("Can't use current card editor because it's null.");
                }

                cardEditor = currentEditor;
                uiContext = context;
            }
            else if (specificCardEditor != null)
            {
                cardEditor = specificCardEditor;
                uiContext = cardEditor.Context;
            }
                
            if (cardEditor != null)
            {
                var info = new Dictionary<string, object>();
                info.SetKrProcessInstance(krProcess);
                if (requestInfo != null)
                {
                    StorageHelper.Merge(requestInfo, info);
                }
                
                await cardEditor.SaveCardAsync(uiContext, info);
                var storeResponse = cardEditor.LastData.StoreResponse;
                var storeResult = storeResponse?.GetKrProcessLaunchResult();
                if (storeResult != null)
                {
                    return new KrProcessLaunchResult(
                        storeResult.LaunchStatus,
                        storeResult.ProcessID,
                        storeResult.ValidationResult.Build(),
                        storeResult.ProcessInfo,
                        storeResponse,
                        null);
                }

                return null;
            }

            var req = new CardRequest
            {
                RequestType = KrConstants.LaunchProcessRequestType,
            };
            req.SetKrProcessInstance(krProcess);
            if (requestInfo != null)
            {
                StorageHelper.Merge(requestInfo, req.Info);
            }
            var resp = await this.cardRepository.RequestAsync(req);
            var result = resp.GetKrProcessLaunchResult();
            return new KrProcessLaunchResult(
                result?.LaunchStatus ?? KrProcessLaunchStatus.Undefined,
                result?.ProcessID,
                resp.ValidationResult.Build(),
                result?.ProcessInfo,
                null,
                resp);
        }

        #endregion
    }
}