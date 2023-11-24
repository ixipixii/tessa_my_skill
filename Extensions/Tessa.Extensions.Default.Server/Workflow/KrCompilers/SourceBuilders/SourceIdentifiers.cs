using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders
{
    public static class SourceIdentifiers
    {
        public const string Void = "void";
        
        public static readonly string BaseClass = typeof(KrScript).FullName;

        public const string Namespace = "Tessa.Generated.Kr";

        public const string KrStageCommonClass = "KrStageCommon";

        public const string KrDesignTimeClass = "KrDesignTime";

        public const string KrRuntimeClass = "KrRuntime";

        public const string KrVisibilityClass = "KrVisibility";

        public const string KrExecutionClass = "KrExecution";

        public const string ConditionMethod = nameof(IKrScript.Condition);

        public const string BeforeMethod = nameof(IKrScript.Before);

        public const string AfterMethod = nameof(IKrScript.After);

        public const string VisibilityMethod = nameof(IKrScript.Visibility);

        public const string ExecutionMethod = nameof(IKrScript.Execution);

        public const string DefaultExtraMethodParameterName = "Context";

        public const string StageAlias = "Stage";

        public const string TemplateAlias = "Template";

        public const string GroupAlias = "Group";

        public const string SecondaryProcessAlias = "SecondaryProcess";

    }
}