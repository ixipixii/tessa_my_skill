using System;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public static class UserAPIKrProcessItemHelper
    {
        public static object NotSupportedAction() => throw new NotSupportedException("Generated class doesn't support specified action.");

        public static object DefaultAction() => BooleanBoxes.True;

        public static void RunBefore(
            IKrScript script,
            IKrProcessItemScript item)
        {
            script.KrScriptType = KrScriptType.Before;
            item.Before();
        }
        
        public static void RunAfter(
            IKrScript script,
            IKrProcessItemScript item)
        {
            script.KrScriptType = KrScriptType.After;
            item.After();
        }

        public static bool RunCondition(
            IKrScript script,
            IKrProcessItemScript item)
        {
            script.KrScriptType = KrScriptType.Condition;
            return item.Condition();
        }
    }
}