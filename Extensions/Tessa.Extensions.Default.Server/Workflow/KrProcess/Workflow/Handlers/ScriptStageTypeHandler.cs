namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class ScriptStageTypeHandler: StageTypeHandlerBase
    {
        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext ctx) => 
            StageHandlerResult.CompleteResult;

        #endregion
    }
}