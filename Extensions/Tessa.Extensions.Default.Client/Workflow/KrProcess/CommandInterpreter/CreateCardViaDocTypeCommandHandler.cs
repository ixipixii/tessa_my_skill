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
    public sealed class CreateCardViaDocTypeCommandHandler : ClientCommandHandlerBase
    {
        private readonly IUIHost uiHost;

        public CreateCardViaDocTypeCommandHandler(
            IUIHost uiHost)
        {
            this.uiHost = uiHost;
        }

        public override void Handle(
            IClientCommandHandlerContext context)
        {
            var command = context.Command;
            if (command.Parameters.TryGetValue(KrConstants.Keys.TypeID, out var typeIDObj)
                && typeIDObj is Guid typeID)
            {
                var info = new Dictionary<string, object>
                {
                    [CardHelper.NewCardBilletKey] = command.Parameters.TryGet<byte[]>(KrConstants.Keys.NewCard),
                    [CardHelper.NewCardBilletSignatureKey] = command.Parameters.TryGet<byte[]>(KrConstants.Keys.NewCardSignature),
                };
                
                if (command.Parameters.TryGetValue(KrConstants.Keys.DocTypeID, out var doctypeIDObj)
                    && command.Parameters.TryGetValue(KrConstants.Keys.DocTypeTitle, out var doctypeTitleObj)
                    && doctypeIDObj is Guid doctypeID
                    && doctypeTitleObj is string doctypeTitle)
                {
                    info[KrConstants.Keys.DocTypeID] = doctypeID;
                    info[KrConstants.Keys.DocTypeTitle] = doctypeTitle;
                }
                
                DispatcherHelper.InvokeInUI(async () =>
                {
                    using ISplash splash = TessaSplash.Create(TessaSplashMessage.CreatingCard);
                    await this.uiHost.CreateCardAsync(
                        typeID,
                        options: new CreateCardOptions
                        {
                            Splash = splash,
                            Info = info,
                        });
                });
            }
        }
    }
}