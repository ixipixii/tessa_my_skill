using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    public abstract class KrEventExtension: IKrEventExtension
    {
        /// <inheritdoc />
        public virtual Task HandleEvent(IKrEventExtensionContext context) => Task.CompletedTask;
    }
}