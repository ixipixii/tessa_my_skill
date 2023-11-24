using Tessa.UI.Views;
using Tessa.UI.Views.Workplaces.Tree;

namespace Tessa.Extensions.Client.Views
{
    /// <summary>
    /// Тестовое расширение для узлов дерева рабочего места.
    /// Добавляет в контекстное меню узла дерева РМ новый элемент.
    /// </summary>
    public sealed class PnrRefreshTreeViews : ITreeItemExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// Вызывается при клонировании модели <paramref name="source"/> в контексте <paramref name="context"/>
        /// </summary>
        /// <param name="source">
        /// Исходная модель
        /// </param>
        /// <param name="cloned">
        /// Клонированная модель
        /// </param>
        /// <param name="context">
        /// Контекст клонирования
        /// </param>
        public void Clone(ITreeItem source, ITreeItem cloned, ICloneableContext context)
        {
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> 
        /// для инициализации в UI
        /// </summary>
        /// <param name="model">
        /// Инициализируемая модель
        /// </param>
        public void Initialize(ITreeItem model)
        {
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> перед отображении в UI
        /// </summary>
        /// <param name="model">
        /// Модель
        /// </param>
        public void Initialized(ITreeItem model)
        {
            IWorkplaceViewModel view;
            if ((view = model.Workplace) == null) return;
            view.Activated += (sender, args) =>
            {
                model.RefreshNodeAsync();
            };
            #endregion
        }

    }
}