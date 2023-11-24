using System;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public class AddFromTemplateUIHandler : StageTypeUIHandlerBase
    {
        public override Task Validate(IKrStageTypeUIHandlerContext context)
        {
            var template = context.Row.TryGet<Guid?>(KrConstants.KrAddFromTemplateSettingsVirtual.FileTemplateID);
            if (template is null)
            {
                context.ValidationResult.AddWarning(this, "$KrStages_AddFromTemplate_TemplateIsRequiredWarning");
            }

            return Task.CompletedTask;
        }
    }
}