using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public abstract class StageTypeUIHandlerBase : IStageTypeUIHandler
    {
        /// <inheritdoc />
        public virtual Task Initialize(IKrStageTypeUIHandlerContext context) => Task.CompletedTask;

        /// <inheritdoc />
        public virtual Task Validate(IKrStageTypeUIHandlerContext context) => Task.CompletedTask;

        /// <inheritdoc />
        public virtual Task Finalize(IKrStageTypeUIHandlerContext context) => Task.CompletedTask;
    }
}