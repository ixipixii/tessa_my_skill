using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrValidateStageFieldsStoreExtension : CardStoreExtension
    {
        public override Task BeforeRequest(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !context.Request.Card.Sections.TryGetValue(KrConstants.KrStages.Virtual, out var stagesSec))
            {
                return Task.CompletedTask;
            }

            var rows = stagesSec.Rows;

            if (rows.Any(p =>
                p.TryGetValue(KrConstants.KrStages.StageTypeID, out var typeID)
                && typeID.Equals(StageTypeDescriptors.EditDescriptor.ID)
                && p.TryGetValue(KrConstants.KrSinglePerformerVirtual.PerformerID, out var perfID)
                && perfID is null))
            {
                context.ValidationResult.AddError(this, "$KrStages_Edit_PerformerMissed");
            }

            if (rows.Any(p =>
                p.TryGetValue(KrConstants.KrStages.StageTypeID, out var createTypeID)
                && createTypeID.Equals(StageTypeDescriptors.CreateCardDescriptor.ID)
                && (p.TryGetValue(KrConstants.KrCreateCardStageSettingsVirtual.TemplateID, out var tempID) && tempID is null
                && p.TryGetValue(KrConstants.KrCreateCardStageSettingsVirtual.TypeID, out var typeID) && typeID is null
                && p.TryGetValue(KrConstants.KrCreateCardStageSettingsVirtual.TemplateID, out var tempID2) && tempID2 != null
                && p.TryGetValue(KrConstants.KrCreateCardStageSettingsVirtual.TypeID, out var typeID2) && typeID2 != null
                || p.TryGetValue(KrConstants.KrCreateCardStageSettingsVirtual.ModeID, out var modeID) && modeID is null)))
            {
                context.ValidationResult.AddError(this, "$KrStages_CreateCard_TemplateAndModeRequired");
            }

            return Task.CompletedTask;
        }
    }
}