using System;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.UI;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.CommandInterpreter
{
    public sealed class OpenCardClientCommandHandler : ClientCommandHandlerBase
    {
        private readonly IUIHost uiHost;

        public OpenCardClientCommandHandler(
            IUIHost uiHost)
        {
            this.uiHost = uiHost;
        }

        public override void Handle(
            IClientCommandHandlerContext context)
        {
            var command = context.Command;
            if (command.Parameters.TryGetValue("cardID", out var cardIDObj)
                && cardIDObj is Guid cardID)
            {
                DispatcherHelper.InvokeInUI( async () =>
                {
                    await this.uiHost.OpenCardAsync(cardID);
                });
            }
        }
    }
}