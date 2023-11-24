using Tessa.UI.Views;

namespace Tessa.Extensions.Client.Views
{
    /// <summary>
    ///     Расширение позволяющее обновлять данные при активации панели рабочего пространства
    /// </summary>
    public sealed class PnrRefreshWorkplace : IWorkplaceViewComponentExtension
    {

        /// <summary>
        /// Вызывается при клонировании модели <paramref name="source"/> в контексте <paramref name="context"/>
        /// </summary>
        /// <param name="source">
        /// Исходная модель
        /// </param>
        /// <param name="cloned">
        /// Клориованная модель
        /// </param>
        /// <param name="context">
        /// Контекст клонирования
        /// </param>
        public void Clone(IWorkplaceViewComponent source, IWorkplaceViewComponent cloned, ICloneableContext context)
        {
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/>
        /// </summary>
        /// <param name="model">
        /// Инициализируемая модель
        /// </param>
        public void Initialize(IWorkplaceViewComponent model)
        {
            IWorkplaceViewModel view;
            if ((view = model.Workplace) == null) return;
            view.Activated += (sender, args) =>
            {
                model.RefreshViewAsync();
                model.Refresh();
            };
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> перед отображени в UI
        /// </summary>
        /// <param name="model">
        /// Модель
        /// </param>
        public void Initialized(IWorkplaceViewComponent model)
        {
        }
    }
}