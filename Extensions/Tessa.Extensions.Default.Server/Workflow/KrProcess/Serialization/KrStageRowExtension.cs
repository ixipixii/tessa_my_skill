using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public abstract class KrStageRowExtension: IKrStageRowExtension
    {
        /// <inheritdoc />
        public virtual Task BeforeSerialization(IKrStageRowExtensionContext context) => Task.CompletedTask;

        /// <inheritdoc />
        public virtual Task DeserializationBeforeRepair(IKrStageRowExtensionContext context) => Task.CompletedTask;

        /// <inheritdoc />
        public virtual Task DeserializationAfterRepair(IKrStageRowExtensionContext context) => Task.CompletedTask;
    }
}