
namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    /// <summary>
    /// Представляет абстрактный обработчкик этапа.
    /// </summary>
    public abstract class StageTypeHandlerBase : IStageTypeHandler
    {
        #region implementation

        /// <inheritdoc />
        public virtual void BeforeInitialization(
            IStageTypeHandlerContext context)
        {
        }

        /// <inheritdoc />
        public virtual void AfterPostprocessing(
            IStageTypeHandlerContext context)
        {
        }

        /// <inheritdoc />
        public virtual StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            return StageHandlerResult.EmptyResult;
        }

        /// <inheritdoc />
        public virtual StageHandlerResult HandleTaskCompletion(IStageTypeHandlerContext context)
        {
            return StageHandlerResult.EmptyResult;
        }

        /// <inheritdoc />
        public virtual StageHandlerResult HandleTaskReinstate(IStageTypeHandlerContext context)
        {
            return StageHandlerResult.EmptyResult;
        }

        /// <inheritdoc />
        public virtual StageHandlerResult HandleSignal(IStageTypeHandlerContext context)
        {
            return StageHandlerResult.EmptyResult;
        }

        /// <inheritdoc />
        public virtual StageHandlerResult HandleResurrection(IStageTypeHandlerContext context)
        {
            return StageHandlerResult.EmptyResult;
        }

        /// <inheritdoc />
        public virtual bool HandleStageInterrupt(
            IStageTypeHandlerContext context) => true;

        #endregion
    }
}