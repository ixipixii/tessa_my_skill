using System;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public static class UserAPIKrProcessExecutionHelper
    {
        public static object NotSupportedAction() => throw new NotSupportedException("Generated class doesn't support specified action.");

        public static object DefaultAction() => BooleanBoxes.True;

        public static bool RunExecution(
            IKrScript script,
            IKrProcessExecutionScript execution)
        {
            script.KrScriptType = KrScriptType.ProcessExecution;
            return execution.Execution();
        }
    }
}