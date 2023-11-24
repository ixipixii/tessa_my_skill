using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Compilation;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders
{
    public abstract class KrSourceBuilder<T>: IKrSourceBuilder<T>
    {
        protected sealed class ExtraSourceInfo
        {
            public string DisplayName;
            public string Name;
            public string ReturnType;
            public string ParameterType;
            public string ParameterName;
            public string Source;
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly string[] voids = { SourceIdentifiers.Void, "void", "Void", "System.Void", "global::System.Void" };
        
        protected Guid ClassID;

        protected string ClassAlias = string.Empty;
        
        protected string Location;

        protected Dictionary<Guid, string> AnchorsMap;

        protected readonly List<ExtraSourceInfo> ExtraSources = new List<ExtraSourceInfo>();
        
        protected readonly List<string> DefaultConstructorParts = new List<string>();
        
        protected readonly ICompilationSourceProvider CompileSourceProvider;
        protected readonly IKrPreprocessorProvider PreprocessorProvider;

        protected KrSourceBuilder(
            ICompilationSourceProvider compileSourceProvider,
            IKrPreprocessorProvider preprocessorProvider)
        {
            this.CompileSourceProvider = compileSourceProvider;
            this.PreprocessorProvider = preprocessorProvider;
        }

        protected abstract string FormatClassName();
        
        /// <inheritdoc />
        public IKrSourceBuilder<T> SetClassID(
            Guid id)
        {
            this.ClassID = id;
            return this;
        }

        /// <inheritdoc />
        public IKrSourceBuilder<T> SetClassAlias(
            string classAlias)
        {
            this.ClassAlias = classAlias;
            return this;
        }

        /// <inheritdoc />
        public IKrSourceBuilder<T> SetLocation(
            params string[] trace)
        {
            // name + template + group + process = 4
            // excess elements will be null
            Array.Resize(ref trace, 4);
            this.Location = KrErrorHelper.FormatErrorMessageTrace(
                trace[0],
                trace[1],
                trace[2],
                trace[3]);
            return this;
        }

        /// <inheritdoc />
        public abstract IKrSourceBuilder<T> SetSources(
            T source);

        /// <inheritdoc />
        public IKrSourceBuilder<T> FillAnchorsMap(
            Dictionary<Guid, string> anchorsMap)
        {
            this.AnchorsMap = anchorsMap;
            return this;
        }

        /// <inheritdoc />
        public IKrSourceBuilder<T> SetExtraSources(
            IExtraSources extraSources)
        {
            foreach (var source in extraSources.ExtraSources)
            {
                this.ExtraSources.Add(new ExtraSourceInfo
                {
                    DisplayName = source.DisplayName,
                    Name = source.Name,
                    ReturnType = source.ReturnType,
                    ParameterType = source.ParameterType,
                    ParameterName = source.ParameterName,
                    Source = source.Source,
                });
            }

            return this;
        }

        /// <inheritdoc />
        public abstract IList<ICompilationSource> BuildSources();

        protected ICompilationSource BuildDefaultConstructor()
        {
            if (this.DefaultConstructorParts.Count == 0)
            {
                return null;
            }
            
            var builder = this.CompileSourceProvider.AcquireSyntaxTree();
            var sb = StringBuilderHelper.Acquire(256);
            foreach (var part in this.DefaultConstructorParts)
            {
                sb.AppendLine(part);
            }

            var methodBody = this.PreprocessorProvider
                .AcquireProcedurePreprocessor()
                .Preprocess(sb.ToStringAndRelease());
            
            var trace = KrErrorHelper.FormatErrorMessageTrace("$KrProcess_Constructor", this.Location);

            return builder
                .SetName(trace)
                .Namespace(SourceIdentifiers.Namespace)
                .Class(
                    this.FormatClassName(),
                    AccessModifier.Public,
                    new[] { SourceIdentifiers.KrStageCommonClass },
                    isPartial: true)
                .AddConstructor(methodBody)
                .Build();
        }
        
        protected IList<ICompilationSource> BuildExtraSources()
        {
            if (this.ExtraSources.Count == 0)
            {
                return EmptyHolder<ICompilationSource>.Collection;
            }
            
            var extraCompilationSources = new List<ICompilationSource>(this.ExtraSources.Count);
            foreach (var source in this.ExtraSources)
            {
                var builder = this.CompileSourceProvider.AcquireSyntaxTree();
                var methodBody = !string.IsNullOrWhiteSpace(source.Source) 
                    ? this.PreprocessorProvider.AcquireProcedurePreprocessor().Preprocess(source.Source) 
                    : string.Empty;
                var trace = KrErrorHelper.FormatErrorMessageTrace(source.DisplayName, this.Location);

                var sourceID = Guid.NewGuid();
                this.AnchorsMap?.Add(sourceID, source.Name);
                builder
                    .SetID(sourceID)
                    .SetName(trace)
                    .Namespace(SourceIdentifiers.Namespace)
                    .Class(
                        this.FormatClassName(),
                        AccessModifier.Public,
                        new[] { SourceIdentifiers.KrStageCommonClass },
                        isPartial: true)
                    .AddMethod(
                        source.ReturnType,
                        source.Name,
                        new []
                        {
                            new Tuple<string, string>(source.ParameterType, source.ParameterName), 
                        },
                        methodBody);

                extraCompilationSources.Add(builder.Build());

                var invocationBody = voids.Contains(source.ReturnType)
                    ? $"{{ this.{source.Name}(({source.ParameterType})ctx); return null; }};"
                    : $"this.{source.Name}(({source.ParameterType})ctx);";
                this.DefaultConstructorParts.Add(
                    $"this.{KrScript.ExtraMethodsName}[\"{source.Name}\"] = ctx => {invocationBody}");
            }

            return extraCompilationSources;
        }
    }
}