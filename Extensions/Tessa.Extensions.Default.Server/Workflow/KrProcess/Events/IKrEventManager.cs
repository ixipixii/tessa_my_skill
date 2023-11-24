using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    public interface IKrEventManager
    {
        Task RaiseAsync(IKrEventExtensionContext context);
    }
}