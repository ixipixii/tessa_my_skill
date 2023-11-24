using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public interface IStageSettingsConverter
    {
        IDictionary<string, object> ToPlain(
            IDictionary<string, object> treeSettings);

        IDictionary<string, object> ToTree(
            Guid topLevelRowID,
            IDictionary<string, object> plainSettings);
    }
}