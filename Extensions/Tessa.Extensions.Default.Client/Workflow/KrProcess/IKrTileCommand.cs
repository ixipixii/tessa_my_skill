using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.UI;
using Tessa.UI.Tiles;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess
{
    public interface IKrTileCommand
    {
        Task OnClickAsync(
            IUIContext context,
            ITile tile,
            KrTileInfo tileInfo);
    }
}