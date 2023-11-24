using System;
using System.Collections.Generic;
using Tessa.Compilation;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrCompiler : IKrCompiler
    {
        #region fields

        private readonly ICompiler compiler;

        private readonly IKrSourceBuilderFactory builderFactory;

        #endregion

        #region constructor

        public KrCompiler(
            ICompiler compiler,
            IKrSourceBuilderFactory builderFactory)
        {
            this.compiler = compiler;
            this.builderFactory = builderFactory;
        }

        #endregion

        #region private

        /// <summary>
        /// Получение исходных кодов и подготовка их к компиляции
        /// </summary>
        /// <param name="krContext"></param>
        /// <param name="context"></param>
        /// <param name="anchorsMap"></param>
        private void SetSources(IKrCompilationContext krContext, ICompilationContext context, out Dictionary<Guid, string> anchorsMap)
        {
            anchorsMap = new Dictionary<Guid, string>();
            var commonMethodBuilder = this.builderFactory.GetKrCommonMethodBuilder();
            commonMethodBuilder.SetSources(krContext.CommonMethods);
            commonMethodBuilder.FillAnchorsMap(anchorsMap);
            context.Sources.AddRange(commonMethodBuilder.BuildSources());

            var stages = krContext.Stages;
            foreach (var stage in stages)
            {
                var sources = this.builderFactory.GetKrRuntimeScriptBuilder()
                    .SetClassID(stage.StageID)
                    .SetClassAlias(SourceIdentifiers.StageAlias)
                    .SetLocation(stage.StageName, stage.TemplateName, stage.GroupName)
                    .SetSources(stage)
                    .SetExtraSources(stage)
                    .FillAnchorsMap(anchorsMap)
                    .BuildSources()
                    ;
                context.Sources.AddRange(sources);
            }

            var templates = krContext.StageTemplates;
            foreach (var template in templates)
            {
                var sources = this.builderFactory.GetKrDesignScriptBuilder()
                        .SetClassID(template.ID)
                        .SetClassAlias(SourceIdentifiers.TemplateAlias)
                        .SetLocation(template.Name, template.StageGroupName)
                        .SetSources(template)
                        .FillAnchorsMap(anchorsMap)
                        .BuildSources()
                    ;
                context.Sources.AddRange(sources);
            }

            var stageGroups = krContext.StageGroups;
            foreach (var stageGroup in stageGroups)
            {
                var sources = this.builderFactory.GetKrDesignScriptBuilder()
                        .SetClassID(stageGroup.ID)
                        
                        .SetClassAlias(SourceIdentifiers.GroupAlias)
                        .SetLocation(stageGroup.Name)
                        .SetSources(stageGroup)
                        .FillAnchorsMap(anchorsMap)
                        .BuildSources()
                    ;
                context.Sources.AddRange(sources);
                sources = this.builderFactory.GetKrRuntimeScriptBuilder()
                        .SetClassID(stageGroup.ID)
                        .SetClassAlias(SourceIdentifiers.GroupAlias)
                        .SetLocation(stageGroup.Name)
                        .SetSources(stageGroup)
                        .FillAnchorsMap(anchorsMap)
                        .BuildSources()
                    ;
                context.Sources.AddRange(sources);
            }

            var secondaryProcesses = krContext.SecondaryProcesses;
            foreach (var secondaryProcess in secondaryProcesses)
            {
                var sources = this.builderFactory.GetKrExecutionScriptBuilder()
                        .SetClassID(secondaryProcess.ID)
                        .SetClassAlias(SourceIdentifiers.SecondaryProcessAlias)
                        .SetLocation(secondaryProcess.Name)
                        .SetSources(secondaryProcess)
                        .FillAnchorsMap(anchorsMap)
                        .BuildSources()
                    ;
                context.Sources.AddRange(sources);

                if (secondaryProcess is IKrProcessButton button)
                {
                    sources = this.builderFactory.GetKrVisibilityScriptBuilder()
                            .SetClassID(button.ID)
                            .SetClassAlias(SourceIdentifiers.SecondaryProcessAlias)
                            .SetLocation(button.Name)
                            .SetSources(button)
                            .FillAnchorsMap(anchorsMap)
                            .BuildSources()
                        ;
                    context.Sources.AddRange(sources);
                }
            }
        }

        /// <summary>
        /// Заполнение ValidationResult по выводу компилятора.
        /// </summary>
        /// <param name="compilationResult"></param>
        /// <param name="anchorsMap"></param>
        /// <returns></returns>
        private static ValidationResult FillValidationResult(
            ICompilationResult compilationResult,
            Dictionary<Guid, string> anchorsMap)
        {
            var result = new ValidationResultBuilder();
            foreach (var compilerOutputItem in compilationResult.CompilerOutput)
            {
                var source = compilerOutputItem.Source;
                var name = source?.Name ?? string.Empty;
                var sourceCode = source != null
                    ? CompilationHelper.FormatErrorIntoMember(source, compilerOutputItem, anchorsMap[source.ID])
                    : string.Empty;
                string errorText = string.IsNullOrWhiteSpace(name)
                    ? string.Empty + compilerOutputItem.ErrorText
                    : name + Environment.NewLine + compilerOutputItem.ErrorText;

                var validator = ValidationSequence
                    .Begin(result)
                    .SetObjectName(name);
                if (compilerOutputItem.IsWarning)
                {
                    validator.WarningDetails(errorText, sourceCode);
                }
                else
                {
                    validator.ErrorDetails(errorText, sourceCode);
                }
                validator.End();
            }

            if (result.Count != 0)
            {
                result.AddWarning(nameof(IKrCompiler), "$KrProcess_ErrorMessage_FullScriptInDetailsWithErrorPointer");
            }

            return result.Build();
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IList<string> DefaultUsings { get; } = new List<string>
        {
            "System.Linq",
            "System.Text",
            "System.Collections.Generic",
            "Tessa.Platform",
            "Tessa.Platform.Data", // reader.GetValue<int>("Column")
            "Tessa.Platform.Runtime",
            "Tessa.Platform.Storage",
            "Tessa.Platform.Collections",
            "Tessa.Platform.Validation",
            "Tessa.Cards",
            "Tessa.Cards.Extensions",
            "Tessa.Files",
            "Tessa.Localization",
            "Tessa.Extensions.Default.Shared", // DefaultCardTypes.SomeTypeID
            "Tessa.Extensions.Default.Shared.Workflow.KrProcess",
            "Tessa.Extensions.Default.Server.Workflow.KrObjectModel",
            "Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI",
            "Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow",
            "Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers",
            "Unity",
            "Unity.Injection",
            "Unity.Lifetime"
        };

        /// <inheritdoc />
        public IList<string> DefaultReferences { get; } = new List<string>
        {
            "linq2db",
            "DocumentFormat.OpenXml",
            "NLog",
            "Unity.Abstractions",
            "Unity.Container",
            "Tessa",
            "Tessa.Extensions.Default.Server",
            "Tessa.Extensions.Default.Shared",
            "Tessa.Extensions.Server",
            "Tessa.Extensions.Shared",
        };
        
        /// <inheritdoc />
        public IKrCompilationResult Compile(IKrCompilationContext krContext)
        {
            var context = this.compiler.CreateContext();
            this.SetSources(krContext, context, out var anchorsMap);
            context.DefaultUsings.UnionWith(krContext.Usings);
            context.DefaultUsings.UnionWith(this.DefaultUsings);
            context.References.UnionWith(krContext.References);
            context.References.UnionWith(this.DefaultReferences);
            var result = this.compiler.Compile(context);
            return new KrCompilationResult(result, FillValidationResult(result, anchorsMap));
        }

        #endregion
    }
}
