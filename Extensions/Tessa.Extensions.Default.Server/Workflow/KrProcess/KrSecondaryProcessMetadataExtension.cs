using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrSecondaryProcessMetadataExtension : CardTypeMetadataExtension
    {
        /// <inheritdoc />
        public override async Task ModifyTypes(
            ICardMetadataExtensionContext context)
        {
            var templateType = await this.TryGetCardTypeAsync(context, DefaultCardTypes.KrTemplateCardTypeID);
            if (templateType is null)
            {
                return;
            }

            var targetTypes = new[] { DefaultCardTypes.KrStageTemplateTypeID, DefaultCardTypes.KrSecondaryProcessTypeID, };
            foreach (var target in targetTypes)
            {
                var targetType = await this.TryGetCardTypeAsync(context, target);
                if (targetType is null)
                {
                    continue;
                }
                var templateClone = templateType.DeepClone();

                targetType.SchemeItems.AddRange(templateClone.SchemeItems);
                targetType.Validators.AddRange(templateClone.Validators);
                targetType.Extensions.AddRange(templateClone.Extensions);

                targetType.Forms.InsertRange(0, templateClone.Forms);
                for (int i = 0; i < targetType.Forms.Count; i++)
                {
                    targetType.Forms[i].TabOrder = i;
                }
            }
        }
    }
}