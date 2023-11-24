using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Initialization;
using Unity;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.Initialization
{
    public class GlobalButtonsInitalizationExtension :
        ClientInitializationExtension
    {
        #region Fields

        private readonly IKrGlobalTileContainer tileContainer;

        #endregion

        #region Constructors

        public GlobalButtonsInitalizationExtension(
            [OptionalDependency] IKrGlobalTileContainer tileContainer)
        {
            this.tileContainer = tileContainer;
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(IClientInitializationExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || this.tileContainer == null
                || !context.Response.TryGetGlobalTiles(out List<KrTileInfo> globalTiles))
            {
                return;
            }

            this.tileContainer.Init(globalTiles);
        }

        #endregion
    }
}