namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class ReadonlyExtraSource : IExtraSource
    {
        public ReadonlyExtraSource(
            string displayName,
            string name,
            string returnType,
            string parameterType,
            string parameterName,
            string source)
        {
            this.DisplayName = displayName;
            this.Name = name;
            this.ReturnType = returnType;
            this.ParameterType = parameterType;
            this.ParameterName = parameterName;
            this.Source = source;
        }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string ReturnType { get; }

        /// <inheritdoc />
        public string ParameterType { get; }

        /// <inheritdoc />
        public string ParameterName { get; }

        /// <inheritdoc />
        public string Source { get; }

        /// <inheritdoc />
        public IExtraSource ToMutable() =>
            new ExtraSource(
                this.DisplayName,
                this.Name,
                this.ReturnType, 
                this.ParameterType,
                this.ParameterName, 
                this.Source);

        /// <inheritdoc />
        public IExtraSource ToReadonly() => this;
    }
}