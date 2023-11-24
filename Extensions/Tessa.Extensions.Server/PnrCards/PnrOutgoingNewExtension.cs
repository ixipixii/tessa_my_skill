using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrOutgoingNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;

            card.Sections["PnrOutgoing"].Fields["DocumentKindID"] = PnrOutgoingTypes.OutgoingLetterID;
            card.Sections["PnrOutgoing"].Fields["DocumentKindIdx"] = PnrOutgoingTypes.OutgoingLetterIdx;
            card.Sections["PnrOutgoing"].Fields["DocumentKindName"] = PnrOutgoingTypes.OutgoingLetterName;

            await TransferFieldsFromBasedOnCard(context, card);
        }

        /// <summary>
        /// Создание Исходящего на основании Входящего
        /// </summary>
        private async Task TransferFieldsFromBasedOnCard(ICardNewExtensionContext context, Card card)
        {
            if (context.Request.Info.TryGetValue("KrCreateBasedOnCardID", out var basedOnCardID))
            {
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;

                    var incomingFields = await db.SetCommand(@"
                        SELECT
                        pi.DocumentKindID,
                        pi.ProjectID,
                        pi.ProjectName,
                        dci.Subject,
                        pi.DepartmentID,
                        pi.DepartmentIdx,
                        pi.DepartmentName,
                        pi.CorrespondentID,
                        pi.CorrespondentName,
                        pi.FullNameRefID,
                        pi.FullNameRefName,
                        pi.ComplaintKindID,
                        pi.ComplaintKindName,
                        pi.ComplaintFormatID,
                        pi.ComplaintFormatName,
                        pi.Contacts,
                        pi.ApartmentNumber
                        FROM PnrIncoming AS pi
                        JOIN DocumentCommonInfo AS dci
                        ON dci.ID = pi.ID
                        WHERE pi.ID = @CardID",
                        db.Parameter("@CardID", basedOnCardID))
                        .LogCommand()
                        .ExecuteAsync<PnrIncomingFields>();
                    
                    var pnrIncomingOrganizations = await db.SetCommand(@"
                        SELECT OrganizationID, OrganizationName
                        FROM PnrIncomingOrganizations
                        WHERE ID = @CardID",
                        db.Parameter("@CardID", basedOnCardID))
                        .LogCommand()
                        .ExecuteListAsync<PnrIncomingOrganization>();

                    if (pnrIncomingOrganizations.Count == 1 && incomingFields != null && card.Sections.ContainsKey("PnrOutgoing"))
                    {
                        var legalEntityIndexIdx = await db.SetCommand(@"
                        SELECT po.LegalEntityIndex
                        FROM PnrIncomingOrganizations AS pio
                        JOIN PnrOrganizations AS po ON po.id = pio.OrganizationID
                        WHERE pio.ID = @CardID",
                        db.Parameter("@CardID", basedOnCardID))
                        .LogCommand()
                        .ExecuteAsync<string>();

                        card.Sections["PnrOutgoing"].Fields["LegalEntityIndexIdx"] = legalEntityIndexIdx;
                    }

                    if (incomingFields != null && card.Sections.ContainsKey("PnrOutgoing"))
                    {
                        card.Sections["PnrOutgoing"].Fields["ProjectID"] = incomingFields.ProjectID;
                        card.Sections["PnrOutgoing"].Fields["ProjectName"] = incomingFields.ProjectName;
                        card.Sections["PnrOutgoing"].Fields["Subject"] = incomingFields.Subject;
                        card.Sections["PnrOutgoing"].Fields["DepartmentID"] = incomingFields.DepartmentID;
                        card.Sections["PnrOutgoing"].Fields["DepartmentIdx"] = incomingFields.DepartmentIdx;
                        card.Sections["PnrOutgoing"].Fields["DepartmentName"] = incomingFields.DepartmentName;

                        if (pnrIncomingOrganizations != null)
                        {
                            foreach (var incomingOrganization in pnrIncomingOrganizations)
                            {
                                CardRow outgoingOrganization = card.Sections["PnrOutgoingOrganizations"].Rows.Add();

                                outgoingOrganization["ID"] = basedOnCardID;
                                outgoingOrganization["RowID"] = Guid.NewGuid();
                                outgoingOrganization["OrganizationID"] = incomingOrganization.OrganizationID;
                                outgoingOrganization["OrganizationName"] = incomingOrganization.OrganizationName;

                                outgoingOrganization.State = CardRowState.Inserted;
                            }
                        }

                        if (incomingFields.DocumentKindID == PnrIncomingTypes.IncomingLetterID)
                        {
                            card.Sections["PnrOutgoing"].Fields["DocumentKindID"] = PnrOutgoingTypes.OutgoingLetterID;
                            card.Sections["PnrOutgoing"].Fields["DocumentKindIdx"] = PnrOutgoingTypes.OutgoingLetterIdx;
                            card.Sections["PnrOutgoing"].Fields["DocumentKindName"] = PnrOutgoingTypes.OutgoingLetterName;

                            card.Sections["PnrOutgoing"].Fields["DestinationID"] = incomingFields.CorrespondentID;
                            card.Sections["PnrOutgoing"].Fields["DestinationName"] = incomingFields.CorrespondentName;
                        }

                        if (incomingFields.DocumentKindID == PnrIncomingTypes.IncomingComplaintsID)
                        {
                            card.Sections["PnrOutgoing"].Fields["DocumentKindID"] = PnrOutgoingTypes.OutgoingComplaintsID;
                            card.Sections["PnrOutgoing"].Fields["DocumentKindIdx"] = PnrOutgoingTypes.OutgoingComplaintsIdx;
                            card.Sections["PnrOutgoing"].Fields["DocumentKindName"] = PnrOutgoingTypes.OutgoingComplaintsName;

                            card.Sections["PnrOutgoing"].Fields["FullNameRefID"] = incomingFields.FullNameRefID;
                            card.Sections["PnrOutgoing"].Fields["FullNameRefName"] = incomingFields.FullNameRefName;
                            card.Sections["PnrOutgoing"].Fields["ComplaintKindID"] = incomingFields.ComplaintKindID;
                            card.Sections["PnrOutgoing"].Fields["ComplaintKindName"] = incomingFields.ComplaintKindName;
                            card.Sections["PnrOutgoing"].Fields["ComplaintFormatID"] = incomingFields.ComplaintFormatID;
                            card.Sections["PnrOutgoing"].Fields["ComplaintFormatName"] = incomingFields.ComplaintFormatName;
                            card.Sections["PnrOutgoing"].Fields["Contacts"] = incomingFields.Contacts;
                            card.Sections["PnrOutgoing"].Fields["ApartmentNumber"] = incomingFields.ApartmentNumber;
                        }
                    }
                }
            }
        }

        private class PnrIncomingFields
        {
            public Guid? DocumentKindID { get; set; }
            public Guid? ProjectID { get; set; }
            public string ProjectName { get; set; }
            public string Subject { get; set; }
            public Guid? DepartmentID { get; set; }
            public string DepartmentIdx { get; set; }
            public string DepartmentName { get; set; }
            public Guid? CorrespondentID { get; set; }
            public string CorrespondentName { get; set; }
            public Guid? FullNameRefID { get; set; }
            public string FullNameRefName { get; set; }
            public Guid? ComplaintKindID { get; set; }
            public string ComplaintKindName { get; set; }
            public Guid? ComplaintFormatID { get; set; }
            public string ComplaintFormatName { get; set; }
            public string Contacts { get; set; }
            public string ApartmentNumber { get; set; }
        }

        private class PnrIncomingOrganization
        {
            public Guid? OrganizationID { get; set; }
            public string OrganizationName { get; set; }
        }
    }
}
