using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.Requests
{
    public sealed class KrClientCommandCustomExtension: CardRequestExtension
    {
        private readonly IClientCommandInterpreter interpreter;

        public KrClientCommandCustomExtension(
            IClientCommandInterpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        /// <inheritdoc />
        public override Task AfterRequest(
            ICardRequestExtensionContext context)
        {
            if (context.RequestIsSuccessful
                && context.Response.TryGetKrProcessClientCommands(out var clientCommands))
            {
                this.interpreter.Interpret(clientCommands, context);
            }

            return Task.CompletedTask;
        }
    }
}