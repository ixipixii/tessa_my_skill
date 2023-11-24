using System.Collections.Generic;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public interface IKrGlobalTileContainer
    {
        void Init(IEnumerable<KrTileInfo> globalTiles);

        IReadOnlyList<KrTileInfo> GetTileInfos();
    }
}