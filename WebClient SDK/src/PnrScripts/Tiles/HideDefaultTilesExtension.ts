import { TileExtension, ITileGlobalExtensionContext } from 'tessa/ui/tiles';

export class HideDefaultTilesExtension extends TileExtension {
    public async initializingGlobal(context: ITileGlobalExtensionContext) {
        const leftPanel = context.workspace.leftPanel;

        // Скрыть дефолтный тайл ознакомления
        const acquaintance = leftPanel.tryGetTile('AcquaintanceGroup');
        if (acquaintance) {
            acquaintance.isCollapsed = true;
            acquaintance.isEnabled = false;
            acquaintance.isHidden = true;
        }
        // * Данный тайл включается системными веб-расширениями после того как отрабатывает данное расширение,
        // * поэтому скрывать его тут бесполезно. Реальное отключение дефолтного тайла ознакомления произведено
        // * в уже скомпилированном файле системных расширений (C:\inetpub\wwwroot\tessa\web\wwwroot\extensions\default.ffa554ecf99cfe081009.js)
        // * путём закомментирования строк с 3812 по 3843 (можно найти по ключевому слову 'AcquaintanceGroup').
    
        // Скрыть тайл ознакомления служебного процесса параллельного ознакомления
        for (let tile of leftPanel.tiles)
        {
            if (tile.caption == "Параллельное ознакомление")
            {
                tile.isCollapsed = true;
                tile.isEnabled = false;
                tile.isHidden = true;
            }
        }
    }
}
