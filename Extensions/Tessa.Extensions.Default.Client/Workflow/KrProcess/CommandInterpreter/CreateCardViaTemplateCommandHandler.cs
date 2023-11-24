using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.UI;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.CommandInterpreter
{
    public sealed class CreateCardViaTemplateCommandHandler : ClientCommandHandlerBase
    {
        private readonly IUIHost uiHost;

        public CreateCardViaTemplateCommandHandler(
            IUIHost uiHost)
        {
            this.uiHost = uiHost;
        }

        public override void Handle(
            IClientCommandHandlerContext context)
        {
            var command = context.Command;
            if (command.Parameters.TryGetValue(KrConstants.Keys.TemplateID, out var templateIDObj)
                && templateIDObj is Guid templateID)
            {
                DispatcherHelper.InvokeInUI(async () =>
                {
                    using ISplash splash = TessaSplash.Create(TessaSplashMessage.CreatingCard);
                    await this.uiHost.CreateFromTemplateAsync(
                        templateID,
                        templateInfo: new Dictionary<string, object>
                        {
                            [CardHelper.NewCardBilletKey] = command.Parameters.TryGet<byte[]>(KrConstants.Keys.NewCard),
                            [CardHelper.NewCardBilletSignatureKey]  = command.Parameters.TryGet<byte[]>(KrConstants.Keys.NewCardSignature),
                        },
                        options: new OpenCardOptions
                        {
                            Splash = splash,
                        });
                });
            }
        }
    }
}