using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Server.Helpers
{
    public static class OrderRegistrationNumber
    {
        private static string GetDocumentsGroupIndex(Guid? documentKindID)
        {
            string documentsGroupIndex = "";
            switch (documentKindID)
            {
                case Guid standard when standard == new Guid(PnrDocumentKinds.OrderAdministrativeActivity.ToString()):
                    documentsGroupIndex = "3-01";
                    break;
                case Guid standard when standard == new Guid(PnrDocumentKinds.OrderMainActivity.ToString()):
                    documentsGroupIndex = "3-02";
                    break;
                case Guid standard when standard == new Guid(PnrDocumentKinds.OrderImplementation.ToString()):
                    documentsGroupIndex = "3-03";
                    break;
                case Guid standard when standard == new Guid(PnrDocumentKinds.OrderMobileCommunications.ToString()):
                    documentsGroupIndex = "3-04";
                    break;
                case Guid standard when standard == new Guid(PnrDocumentKinds.Disposal.ToString()):
                    documentsGroupIndex = "3-05";
                    break;
            }
            return documentsGroupIndex;
        }
        //public static string GenerateOrderRegistrationNumber(Guid? documentKindID, string organizationLegalEntityIndex, long number)
        //{
        //    string documentsGroupIndex = GetDocumentsGroupIndex(documentKindID);

        //    return $"{documentsGroupIndex}-{organizationLegalEntityIndex}-{number.ToString().PadLeft(4, '0')}";
        //}

        public static async Task SetOrderRegistrationNumber(IDbScope dbScope, Card card)
        {
            long number = await PnrDataHelper.GetActualFieldValueAsync<long>(dbScope, card, "DocumentCommonInfo", "Number");

            Guid? documentKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(dbScope, card, "PnrOrder", "DocumentKindID");
            string documentsGroupIndex = GetDocumentsGroupIndex(documentKindID);
            string organizationLegalEntityIndex = await PnrDataHelper.GetActualFieldValueAsync<string>(dbScope, card, "PnrOrder", "OrganizationLegalEntityIndex");

            string newFullNumber = $"{documentsGroupIndex}-{organizationLegalEntityIndex}-{number.ToString().PadLeft(4, '0')}";
            
            await ServerHelper.UpdateDocumentNumber(dbScope, card.ID, newFullNumber);
        }

        public static async Task<string> GetDocumentsGroupIndex(IDbScope dbScope, Card card)
        {
            Guid? documentKindID = await PnrDataHelper.GetActualFieldValueAsync<Guid?>(dbScope, card, "PnrOrder", "DocumentKindID");
            return GetDocumentsGroupIndex(documentKindID);
        }
    }
}
