// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerSettingsDialog.xaml.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces.Manager
{
    #region

    using System.Windows;

    using Tessa.Extensions.Default.Client.Views;
    using Tessa.UI.Windows;

    #endregion

    /// <summary>
    ///     Interaction logic for ChartDesignerWindow.xaml
    /// </summary>
    public partial class ManagerSettingsDialog : TessaWindow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ManagerSettingsDialog" /> class.
        /// </summary>
        public ManagerSettingsDialog()
        {
            this.InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}