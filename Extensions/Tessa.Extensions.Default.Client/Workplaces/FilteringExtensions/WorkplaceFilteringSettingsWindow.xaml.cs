#region Usings

using System.Windows;

#endregion

namespace Tessa.Extensions.Default.Client.Workplaces
{
    /// <summary>
    ///     Interaction logic for WorkplaceFilteringSettingsWindow
    /// </summary>
    public partial class WorkplaceFilteringSettingsWindow
    {
        #region Constructors and Destructors

        public WorkplaceFilteringSettingsWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Other methods

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}