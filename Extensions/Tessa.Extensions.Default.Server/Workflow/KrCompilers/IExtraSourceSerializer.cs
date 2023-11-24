using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IExtraSourceSerializer
    {
        string Serialize(
            IList<IExtraSource> list);
        
        IList<IExtraSource> Deserialize(
            string json);
    }
}