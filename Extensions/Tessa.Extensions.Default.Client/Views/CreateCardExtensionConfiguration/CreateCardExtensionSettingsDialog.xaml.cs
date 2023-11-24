// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerSettingsDialog.xaml.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Views
{
    #region

    using System.Windows;

    using Tessa.UI.Windows;

    #endregion

    /// <summary>
    /// The create card extension settings dialog.
    /// </summary>
    public partial class CreateCardExtensionSettingsDialog : TessaWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCardExtensionSettingsDialog"/> class.
        /// </summary>
        public CreateCardExtensionSettingsDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The ok button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}