namespace Tessa.Extensions.Default.Client.Workplaces
{
    public class StringValueModel
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        public StringValueModel()
        {
        }

        /// <inheritdoc />
        public StringValueModel(string value)
        {
            this.Value = value;
        }

        #endregion

        #region Public properties

        public string Value { get; set; }

        #endregion
    }
}