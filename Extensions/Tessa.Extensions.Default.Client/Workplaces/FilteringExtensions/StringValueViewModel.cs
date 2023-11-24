#region Usings

using Tessa.Properties.Resharper;
using Tessa.UI;

#endregion

namespace Tessa.Extensions.Default.Client.Workplaces
{
    public sealed class StringValueViewModel : SelectableViewModel<StringValueModel>
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        public StringValueViewModel(StringValueModel model)
            : base(model)
        {
        }

        #endregion

        #region Public properties

        public string Value
        {
            get => this.Model.Value;
            [UsedImplicitly]
            set
            {
                if (this.Value == value)
                {
                    return;
                }


                this.Model.Value = value;
                this.OnPropertyChanged("Value");
            }
        }

        #endregion

        #region Public methods and operators

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Model.Value;
        }

        #endregion
    }
}