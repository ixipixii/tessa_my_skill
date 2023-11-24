// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomFolderView.xaml.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// <summary>
//   Interaction logic for CustomFolderView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    using System.Windows.Controls;

    using Tessa.Properties.Resharper;
    using Tessa.UI;
    using Tessa.UI.Views.Workplaces.Tree;

    /// <summary>
    /// Interaction logic for CustomFolderView.xaml
    /// </summary>
    public partial class CustomFolderView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFolderView"/> class.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        public CustomFolderView([NotNull] IFolderTreeItem folder)
        {
            InitializeComponent();
            this.DataContext = folder;
            this.Resources = UIHelper.Generic;
        }

        #endregion
    }
}