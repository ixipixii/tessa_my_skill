using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Settings;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Settings;

namespace Tessa.Extensions.Shared.Settings
{
    public sealed class PnrSettings : SettingsExtension
    {
        public override Task Initialize(ISettingsExtensionContext context)
        {
            KrSettings settings = context.Settings.Get<KrSettings>();

            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrIncomingTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrOutgoingTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrServiceNoteTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrOrderTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrErrandTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrRegulationTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrPowerAttorneyTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrContractTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrSupplementaryAgreementTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrActTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrTenderTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrTemplateTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrPartnerRequestTypeID);

            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrContractUKTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrIncomingUKTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrOrderUKTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrOutgoingUKTypeID);
            settings.CreateBasedOnTypes.Add(PnrCardTypes.PnrSupplementaryAgreementUKTypeID);

            return Task.CompletedTask;
        }
    }
}
