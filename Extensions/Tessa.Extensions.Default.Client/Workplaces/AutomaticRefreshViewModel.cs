// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutomaticRefreshViewModel.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Workplaces
{
    using Tessa.Properties.Resharper;
    using Tessa.UI;
    using Tessa.Extensions.Default.Shared.Workplaces;

    /// <summary>
    ///     Модель-представление редактирования настроек автоматического обновления
    /// </summary>
    public class AutomaticRefreshViewModel : ViewModel<IAutomaticNodeRefreshSettings>
    {
        /// <inheritdoc />
        public AutomaticRefreshViewModel([NotNull] IAutomaticNodeRefreshSettings model)
            : base(model)
        {
        }

        /// <summary>
        ///     Gets or sets интервал автоматического обновления в секундах
        /// </summary>
        public int Interval
        {
            get
            {
                return this.Model.RefreshInterval;
            }

            set
            {
                if (value == this.Interval)
                {
                    return;
                }

                this.Model.RefreshInterval = value;
                this.OnPropertyChanged("Interval");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether Признак необходимости обновления табличной части
        /// </summary>
        public bool WithContentDataRefreshing
        {
            get
            {
                return this.Model.WithContentDataRefreshing;
            }

            set
            {
                if (this.WithContentDataRefreshing == value)
                {
                    return;
                }

                this.Model.WithContentDataRefreshing = value;
                this.OnPropertyChanged("WithContentDataRefreshing");
            }
        }
    }
}