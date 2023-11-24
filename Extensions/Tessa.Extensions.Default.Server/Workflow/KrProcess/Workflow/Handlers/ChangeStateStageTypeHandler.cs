using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class ChangeStateStageTypeHandler : StageTypeHandlerBase
    {
        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var state = context.Stage.SettingsStorage.TryGet<int?>(KrConstants.KrChangeStateSettingsVirtual.StateID);
            if (state.HasValue)
            {
                context.WorkflowProcess.State = (KrState)state;
            }

            return StageHandlerResult.CompleteResult;
        }

        #endregion
    }
}