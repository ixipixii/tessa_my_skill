using System;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public static class UserAPIKrProcessVisibilityHelper
    {
        public static object NotSupportedAction() => throw new NotSupportedException("Generated class doesn't support specified action.");

        public static object DefaultAction() => BooleanBoxes.True;

        public static bool RunVisibility(
            IKrScript script,
            IKrProcessVisibilityScript visibility)
        {
            script.KrScriptType = KrScriptType.ProcessVisibility;
            return visibility.Visibility();
        }
    }
}