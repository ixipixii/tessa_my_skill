using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tessa.Extensions.Default.Client.Views
{
    using Tessa.UI.Views;
    using Tessa.UI.Views.Workplaces.Tree;

    public sealed class CustomFolderViewExtension : ITreeItemExtension
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
        public void Clone(ITreeItem source, ITreeItem cloned, ICloneableContext context)
        {
            var folder = (IFolderTreeItem)cloned;
            folder.ContentProviderFactory = (item, viewModel, strategy) => new CustomViewContentProvider(folder);
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> 
        /// </summary>
        /// <param name="model">
        /// Инициализируемая модель
        /// </param>
        public void Initialize(ITreeItem model)
        {
            var folder = model as IFolderTreeItem;
            if (folder == null)
            {
                throw new ApplicationException("Данное расширение работает только с папками");
            }

            folder.SwitchExpandOnSingleClick = false;
            folder.ContentProviderFactory = (item, viewModel, strategy) => new CustomViewContentProvider(folder);
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> перед отображени в UI
        /// </summary>
        /// <param name="model">
        /// Модель
        /// </param>
        public void Initialized(ITreeItem model)
        {
            
        }
    }
}
