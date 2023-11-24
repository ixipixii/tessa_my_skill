using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public static class UserAPIContextChangeableHelper
    {
        public static void RunNextStageInContext(
            IKrScript script,
            IContextChangeableScript contextChangeable,
            Guid cardID,
            bool wholeCurrentGroup = false,
            IDictionary<string, object> processInfo = null)
        {
            if (script.CardObject is null
                || cardID != script.CardID)
            {
                contextChangeable.DifferentContextCardID = cardID;
                contextChangeable.DifferentContextWholeCurrentGroup = wholeCurrentGroup;
                contextChangeable.DifferentContextSetupScriptType = script.KrScriptType;
                contextChangeable.DifferentContextProcessInfo = processInfo;
            }
        }

        public static bool ContextChangePending(IContextChangeableScript contextChangeable) => contextChangeable.DifferentContextCardID.HasValue;

        public static void DoNotChangeContext(IContextChangeableScript contextChangeable)
        {
            contextChangeable.DifferentContextCardID = null;
            contextChangeable.DifferentContextWholeCurrentGroup = false;
            contextChangeable.DifferentContextSetupScriptType = null;
            contextChangeable.DifferentContextProcessInfo = null;
        }
    }
}