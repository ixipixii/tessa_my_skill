using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IExtraSources
    {
        /// <summary>
        /// Список дополнительных исходных кодов.
        /// </summary>
        IReadOnlyList<IExtraSource> ExtraSources { get; }
    }
}