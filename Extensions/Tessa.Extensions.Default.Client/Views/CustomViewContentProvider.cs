// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomViewContentProvider.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// <summary>
//   The custom view content provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Tessa.Extensions.Default.Client.Views
{
    using System;
    using System.Collections.Generic;

    using Tessa.UI;
    using Tessa.UI.Views;
    using Tessa.UI.Views.Content;
    using Tessa.UI.Views.Workplaces.Tree;

    /// <summary>
    /// The custom view content provider.
    /// </summary>
    internal sealed class CustomViewContentProvider : ViewModel<EmptyModel>, IContentProvider
    {
        #region Fields

        /// <summary>
        /// The content.
        /// </summary>
        public object content;

        /// <summary>
        /// The components.
        /// </summary>
        private IDictionary<Guid, IWorkplaceViewComponent> components = new Dictionary<Guid, IWorkplaceViewComponent>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomViewContentProvider"/> class.
        /// </summary>
        public CustomViewContentProvider(IFolderTreeItem folder)
        {
            this.content = new CustomFolderView(folder);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Список компонент предоставляемых поставщиком
        /// </summary>
        public IDictionary<Guid, IWorkplaceViewComponent> Components
        {
            get
            {
                return this.components;
            }
        }

        /// <summary>
        ///     Gets содержимое объекта
        /// </summary>
        public object Content
        {
            get
            {
                return this.content;
            }
        }

        /// <summary>
        ///     Gets контекст представления
        /// </summary>
        public IViewContext ViewContext
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Осуществляет обновление представлений
        /// </summary>
        public void Refresh()
        {
            this.OnPropertyChanged("Content");
        }

        #endregion
    }
}