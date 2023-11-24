using Tessa.Extensions.Default.Shared;
using Tessa.Views;

namespace Tessa.Extensions.Default.Client.Tiles
{
    /// <summary>
    /// Запрещает системные плитки "Удалить", "Экспорт" и "Показать структуру" для типовых представлений,
    /// перечисленных в <see cref="DefaultViewAliases"/>.
    /// </summary>
    public sealed class DefaultProhibitTilesInViewsTileExtension :
        ProhibitTilesInViewsTileExtension
    {
        #region Base Overrides

        protected override bool CanDeleteCard(ITessaView view) =>
            DefaultViewAliases.CanDeleteCard(view.Metadata.Alias);

        protected override bool CanExportCard(ITessaView view) =>
            DefaultViewAliases.CanExportCard(view.Metadata.Alias);

        protected override bool CanViewCardStorage(ITessaView view) =>
            DefaultViewAliases.CanViewCardStorage(view.Metadata.Alias);

        #endregion
    }
}