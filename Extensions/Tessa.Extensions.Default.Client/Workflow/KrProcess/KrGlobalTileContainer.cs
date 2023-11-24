using System.Collections.Generic;
using System.Linq;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public sealed class KrGlobalTileContainer : IKrGlobalTileContainer
    {
        private volatile IReadOnlyList<KrTileInfo> infos;

        /// <inheritdoc />
        public void Init(
            IEnumerable<KrTileInfo> globalTiles)
        {
            this.infos = globalTiles.ToList().AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyList<KrTileInfo> GetTileInfos() => this.infos;
    }
}