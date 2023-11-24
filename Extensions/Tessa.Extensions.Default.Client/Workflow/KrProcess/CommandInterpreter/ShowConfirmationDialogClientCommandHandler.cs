using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.Platform.Validation;
using Tessa.UI;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.CommandInterpreter
{
    public sealed class ShowConfirmationDialogClientCommandHandler: ClientCommandHandlerBase
    {
        public override void Handle(
            IClientCommandHandlerContext context)
        {
            var command = context.Command;
            if (command.Parameters.TryGetValue("text", out var textObj)
                && textObj is string text
                && !string.IsNullOrWhiteSpace(text))
            {
                TessaDialog.ShowNotEmpty(ValidationResult.FromText(text));
            }
        }
    }
}