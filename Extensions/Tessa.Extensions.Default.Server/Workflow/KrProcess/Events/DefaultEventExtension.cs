using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    public class DefaultEventExtension: KrEventExtension
    {
        public override Task HandleEvent(IKrEventExtensionContext context) => Task.CompletedTask;
    }
}