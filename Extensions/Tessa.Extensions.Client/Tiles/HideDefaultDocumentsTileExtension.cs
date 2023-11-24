using System.Threading.Tasks;
using Tessa.Platform;
using Tessa.UI;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;

namespace Tessa.Extensions.Client.Tiles
{
    public sealed class HideDefaultDocumentsTileExtension : TileExtension
    {
        /// <summary>
        /// Расширение, скрывающее отображение дефолтных тайлов
        /// </summary>
        public override async Task InitializingLocal(ITileLocalExtensionContext context)
        {
            // Левое меню
            ITilePanel leftPanel = context.Workspace.LeftPanel;

            // Скрыть дефолтный тайл ознакомления
            ITile acquaintance = leftPanel.Tiles.TryGet(TileNames.AcquaintanceGroup);
            acquaintance.DisableWithCollapsing();

            // Скрыть тайл ознакомления служебного процесса параллельного ознакомления
            foreach (var tile in leftPanel.Tiles)
            {
                if (tile.Caption == "Параллельное\r\nознакомление")
                {
                    tile.DisableWithCollapsing();
                }
            }

            // Правое меню
            ITilePanel panel = context.Workspace.RightPanel;

            // Тайл создать карточку
            ITile createCard = panel.Tiles.TryGet(TileNames.CreateCard);

            if (createCard != null)
            {
                // Скрыть дочерний тайл Документы путём скрытия всех вложенных тайлов 
                // (если скрыть лишь сам тайл Документы, то остаётся зарезервированное под тайл пустое место)
                ITile documents = createCard.Tiles.TryGet(TileNames.DocumentTypes);

                if (documents != null)
                {
                    foreach (var tile in documents.Tiles)
                    {
                        tile.IsHidden = true;
                    }
                }
            }
        }
    }
}
