// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeViewItemTestExtension.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// <summary>
//   The tree view item test extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    using Tessa.UI;
    using Tessa.UI.Controls;
    using Tessa.UI.Menu;
    using Tessa.UI.Views;
    using Tessa.UI.Views.Workplaces.Tree;

    /// <summary>
    /// Тестовое расширение для узлов дерева рабочего места.
    /// Добавляет в контекстное меню узла дерева РМ новый элемент.
    /// </summary>
    public sealed class TreeViewItemTestExtension : ITreeItemExtension
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
            model.ContextMenuGenerators.Add(ctx => 
                ctx.MenuActions.Add(
                    new MenuAction(
                        "custom", 
                        "custom menu item", 
                        ctx.MenuContext.Icons.Get("Thin1"), 
                        new DelegateCommand(o => TessaDialog.ShowMessage("Message")))));
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> перед отображении в UI
        /// </summary>
        /// <param name="model">
        /// Модель
        /// </param>
        public void Initialized(ITreeItem model)
        {
        }

        #endregion
    }
}