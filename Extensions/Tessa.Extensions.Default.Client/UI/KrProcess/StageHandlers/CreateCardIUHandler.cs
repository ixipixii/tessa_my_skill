using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class CreateCardUIHandler : StageTypeUIHandlerBase
    {
        private CardRow settings;

        public override Task Validate(IKrStageTypeUIHandlerContext context)
        {
            var template = context.Row.TryGet<Guid?>(KrConstants.KrCreateCardStageSettingsVirtual.TemplateID);
            var type = context.Row.TryGet<Guid?>(KrConstants.KrCreateCardStageSettingsVirtual.TypeID);
            var mode = context.Row.TryGet<int?>(KrConstants.KrCreateCardStageSettingsVirtual.ModeID);
            if (template is null && type is null)
            {
                context.ValidationResult.AddWarning(this, "$KrStages_CreateCard_TemplateAndModeRequired");
            }
            else if (template != null && type != null)
            {
                context.ValidationResult.AddWarning(this, "$KrStages_CreateCard_TemplateAndTypeSelected");
            }
            if (mode == null)
            {
                context.ValidationResult.AddError(this, "$KrStages_CreateCard_ModeRequired");
            }

            return Task.CompletedTask;
        }

        public override Task Initialize(IKrStageTypeUIHandlerContext context)
        {
            this.settings = context.Row;
            this.settings.FieldChanged += this.OnSettingsFieldChanged;

            return Task.CompletedTask;
        }

        public override Task Finalize(IKrStageTypeUIHandlerContext context)
        {
            this.settings.FieldChanged -= this.OnSettingsFieldChanged;
            this.settings = null;

            return Task.CompletedTask;
        }

        private void OnSettingsFieldChanged(object sender, CardFieldChangedEventArgs e)
        {
            if (e.FieldName == KrConstants.KrCreateCardStageSettingsVirtual.TypeID)
            {
                if (e.FieldValue != null)
                {
                    this.settings.Fields[KrConstants.KrCreateCardStageSettingsVirtual.TemplateID] = null;
                    this.settings.Fields[KrConstants.KrCreateCardStageSettingsVirtual.TemplateCaption] = null;
                }
            }
            else if (e.FieldName == KrConstants.KrCreateCardStageSettingsVirtual.TemplateID)
            {
                if (e.FieldValue != null)
                {
                    this.settings.Fields[KrConstants.KrCreateCardStageSettingsVirtual.TypeID] = null;
                    this.settings.Fields[KrConstants.KrCreateCardStageSettingsVirtual.TypeCaption] = null;
                }
            }
        }
    }
}