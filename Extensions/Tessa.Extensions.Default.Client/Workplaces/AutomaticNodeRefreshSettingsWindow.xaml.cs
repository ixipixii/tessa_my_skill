// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutomaticNodeRefreshSettingsWindow.xaml.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces
{
    using System;
    using System.Windows;

    /// <summary>
    ///     Interaction logic for ChartDesignerWindow.xaml
    /// </summary>
    public partial class AutomaticNodeRefreshSettingsWindow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AutomaticNodeRefreshSettingsWindow" /> class.
        /// </summary>
        public AutomaticNodeRefreshSettingsWindow()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.Dispatcher.BeginInvoke(new Action(() => this.IntervalBox.Focus()));
            this.IntervalBox.GotKeyboardFocus += (s, e) => this.IntervalBox.SelectAll();
        }

        /// <summary>
        /// The ok button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}