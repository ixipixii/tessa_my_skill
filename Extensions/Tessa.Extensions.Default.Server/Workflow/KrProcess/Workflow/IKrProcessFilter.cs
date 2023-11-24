using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IKrProcessFilter<out T>
    {
        IReadOnlyCollection<T> Excluded { get; }
    }
}