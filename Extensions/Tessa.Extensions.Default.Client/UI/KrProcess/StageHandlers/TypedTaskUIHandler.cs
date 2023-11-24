using System;
using System.Threading.Tasks;

using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class TypedTaskUIHandler : StageTypeUIHandlerBase
    {
        /// <inheritdoc />
        public override Task Validate(IKrStageTypeUIHandlerContext context)
        {
            if (context.Row.TryGet<Guid?>(KrTypedTaskSettingsVirtual.TaskTypeID) is null)
            {
                context.ValidationResult.AddError(this, "$KrStages_TypedTask_TaskType");
            }

            return Task.CompletedTask;
        }
    }
}