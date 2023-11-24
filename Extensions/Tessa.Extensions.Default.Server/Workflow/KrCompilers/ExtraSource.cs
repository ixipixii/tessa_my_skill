using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    [Serializable]
    public sealed class ExtraSource: 
        StorageObject,
        IExtraSource
    {
        public ExtraSource()
            : this(null, null, null, null, null, null)
        {
        }
        
        public ExtraSource(
            string displayName,
            string name,
            string returnType,
            string parameterType,
            string parameterName,
            string source)
            : base(new Dictionary<string, object>(7))
        {
            this.Init(nameof(this.DisplayName), displayName);
            this.Init(nameof(this.Name), name);
            this.Init(nameof(this.ReturnType), returnType);
            this.Init(nameof(this.ParameterType), parameterType);
            this.Init(nameof(this.ParameterName), parameterName);
            this.Init(nameof(this.Source), source);
        }
        
        /// <inheritdoc />
        public ExtraSource(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        public ExtraSource(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public string DisplayName 
        {
            get => this.Get<string>(nameof(this.DisplayName));
            set => this.Set(nameof(this.DisplayName), value);
        }

        /// <inheritdoc />
        public string Name
        {
            get => this.Get<string>(nameof(this.Name));
            set => this.Set(nameof(this.Name), value);
        }

        /// <inheritdoc />
        public string ReturnType
        {
            get => this.Get<string>(nameof(this.ReturnType));
            set => this.Set(nameof(this.ReturnType), value);
        }

        /// <inheritdoc />
        public string ParameterType
        {
            get => this.Get<string>(nameof(this.ParameterType));
            set => this.Set(nameof(this.ParameterType), value);
        }

        /// <inheritdoc />
        public string ParameterName
        {
            get => this.TryGet<string>(nameof(this.ParameterName)) ?? SourceIdentifiers.DefaultExtraMethodParameterName;
            set => this.Set(nameof(this.ParameterName), value);
        }

        /// <inheritdoc />
        public string Source
        {
            get => this.Get<string>(nameof(this.Source));
            set => this.Set(nameof(this.Source), value);
        }

        /// <inheritdoc />
        public IExtraSource ToMutable() => this;

        /// <inheritdoc />
        public IExtraSource ToReadonly() => 
            new ReadonlyExtraSource(
                this.DisplayName,
                this.Name,
                this.ReturnType,
                this.ParameterType,
                this.ParameterName,
                this.Source);
    }
}