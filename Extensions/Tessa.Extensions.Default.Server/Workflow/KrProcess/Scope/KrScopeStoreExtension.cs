using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope
{
    public sealed class KrScopeStoreExtension : CardStoreExtension
    {
        #region fields

        private readonly IKrTypesCache krTypesCache;

        private List<KrProcessClientCommand> clientCommands;

        private IValidationResultBuilder scopeValidationResult;

        #endregion

        #region constructor

        public KrScopeStoreExtension(
            IKrTypesCache krTypesCache)
        {
            this.krTypesCache = krTypesCache;
        }

        #endregion

        #region base overrides

        public override Task BeforeCommitTransaction(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.Request.Card.TypeID, this.krTypesCache))
            {
                return Task.CompletedTask;
            }

            var scopeContext = KrScopeContext.Current;
            if (scopeContext != null)
            {
                scopeContext.LevelStack.Peek().ApplyChanges(context.Request.Card.ID, context.ValidationResult);
                this.clientCommands = scopeContext.GetKrProcessClientCommands();
                this.scopeValidationResult = scopeContext.ValidationResult;
            }

            return Task.CompletedTask;
        }

        public override Task AfterRequest(
            ICardStoreExtensionContext context)
        {
            if (KrScopeContext.HasCurrent)
            {
                return Task.CompletedTask;
            }

            if (this.clientCommands != null)
            {
                context.Response.AddKrProcessClientCommands(this.clientCommands);
            }

            if (this.scopeValidationResult != null)
            {
                context.ValidationResult.Add(this.scopeValidationResult);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}