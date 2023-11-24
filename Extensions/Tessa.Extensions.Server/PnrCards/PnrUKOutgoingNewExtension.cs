using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrUKOutgoingNewExtension : CardNewExtension
    {
        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;

            card.Sections["PnrOutgoingUK"].Fields["DocumentKindID"] = PnrOutgoingUKTypes.OutgoingUKLetterID;
            card.Sections["PnrOutgoingUK"].Fields["DocumentKindIdx"] = PnrOutgoingUKTypes.OutgoingUKLetterIdx;
            card.Sections["PnrOutgoingUK"].Fields["DocumentKindName"] = PnrOutgoingUKTypes.OutgoingUKLetterName;

            await TransferFieldsFromBasedOnCard(context, card);
        }

        /// <summary>
        /// Создание Исходящего УК ПС на основании Входящего УК ПС. Тип Исходящего УК ПС в зависимости от типа Иходящего УК ПС 
        /// </summary>
        private async Task TransferFieldsFromBasedOnCard(ICardNewExtensionContext context, Card card)
        {
            if (context.Request.Info.TryGetValue("KrCreateBasedOnCardID", out var basedOnCardID))
            {
                await using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;
                    var outgoingUkFields = await db.SetCommand(@"
                        SELECT
                            piuk.DocumentKindID,
                            dci.Subject,
                            piuk.OrganizationID,
                            piuk.OrganizationName,
                            piuk.CorrespondentID,
                            piuk.CorrespondentName,
                            piuk.DepartmentID,
                            piuk.DepartmentIdx,
                            piuk.DepartmentName,
                            piuk.OriginalID,
                            piuk.OriginalName,
                            piuk.DeliveryTypeID,
                            piuk.DeliveryTypeName,
                            piuk.Contacts,
                            piuk.Housing,
                            piuk.ApartmentNumber,
                            piuk.ComplaintFormatID,
                            piuk.ComplaintFormatName,
                            piuk.ComplaintKindID,
                            piuk.ComplaintKindName
                        FROM DocumentCommonInfo AS dci
                        JOIN PnrIncomingUK AS piuk ON piuk.ID = dci.ID
                        WHERE dci.ID = @CardID",
                            db.Parameter("@CardID", basedOnCardID))
                        .LogCommand()
                        .ExecuteAsync<PnrOutgoingUkFieldsFields>();

                    if (outgoingUkFields != null && card.Sections.ContainsKey("PnrOutgoingUK") && card.Sections.ContainsKey("DocumentCommonInfo"))
                    {
                        card.Sections["PnrOutgoingUK"].Fields["Subject"] = outgoingUkFields.Subject;
                        card.Sections["PnrOutgoingUK"].Fields["OrganizationID"] = outgoingUkFields.OrganizationID;
                        card.Sections["PnrOutgoingUK"].Fields["OrganizationName"] = outgoingUkFields.OrganizationName;
                        card.Sections["PnrOutgoingUK"].Fields["CorrespondentID"] = outgoingUkFields.CorrespondentID;
                        card.Sections["PnrOutgoingUK"].Fields["CorrespondentName"] = outgoingUkFields.CorrespondentName;
                        card.Sections["PnrOutgoingUK"].Fields["DepartmentID"] = outgoingUkFields.DepartmentID;
                        card.Sections["PnrOutgoingUK"].Fields["DepartmentIdx"] = outgoingUkFields.DepartmentIdx;
                        card.Sections["PnrOutgoingUK"].Fields["DepartmentName"] = outgoingUkFields.DepartmentName;

                        if (outgoingUkFields.DocumentKindID == PnrIncomingUKTypes.IncomingUKLetterID)
                        {
                            card.Sections["PnrOutgoingUK"].Fields["OriginalID"] = outgoingUkFields.OriginalID;
                            card.Sections["PnrOutgoingUK"].Fields["OriginalName"] = outgoingUkFields.OriginalName;
                            card.Sections["PnrOutgoingUK"].Fields["DeliveryTypeID"] = outgoingUkFields.DeliveryTypeID;
                            card.Sections["PnrOutgoingUK"].Fields["DeliveryTypeName"] = outgoingUkFields.DeliveryTypeName;

                            card.Sections["PnrOutgoingUK"].Fields["DocumentKindID"] = PnrOutgoingUKTypes.OutgoingUKLetterID;
                            card.Sections["PnrOutgoingUK"].Fields["DocumentKindIdx"] = PnrOutgoingUKTypes.OutgoingUKLetterIdx;
                            card.Sections["PnrOutgoingUK"].Fields["DocumentKindName"] = PnrOutgoingUKTypes.OutgoingUKLetterName;
                        }

                        if (outgoingUkFields.DocumentKindID == PnrIncomingUKTypes.IncomingUKComplaintsID)
                        {
                            card.Sections["PnrOutgoingUK"].Fields["Contacts"] = outgoingUkFields.Contacts;
                            card.Sections["PnrOutgoingUK"].Fields["Housing"] = outgoingUkFields.Housing;
                            card.Sections["PnrOutgoingUK"].Fields["ApartmentNumber"] = outgoingUkFields.ApartmentNumber;
                            card.Sections["PnrOutgoingUK"].Fields["ComplaintFormatID"] = outgoingUkFields.ComplaintFormatID;
                            card.Sections["PnrOutgoingUK"].Fields["ComplaintFormatName"] = outgoingUkFields.ComplaintFormatName;
                            card.Sections["PnrOutgoingUK"].Fields["ComplaintKindID"] = outgoingUkFields.ComplaintKindID;
                            card.Sections["PnrOutgoingUK"].Fields["ComplaintKindName"] = outgoingUkFields.ComplaintKindName;

                            card.Sections["PnrOutgoingUK"].Fields["DocumentKindID"] = PnrOutgoingUKTypes.OutgoingUKComplaintsID;
                            card.Sections["PnrOutgoingUK"].Fields["DocumentKindIdx"] = PnrOutgoingUKTypes.OutgoingUKComplaintsIdx;
                            card.Sections["PnrOutgoingUK"].Fields["DocumentKindName"] = PnrOutgoingUKTypes.OutgoingUKComplaintsName;
                        }
                    }
                }
            }
        }

        private class PnrOutgoingUkFieldsFields
        {
            public Guid? DocumentKindID { get; set; }
            public string Subject { get; set; }
            public Guid? OrganizationID { get; set; }
            public string OrganizationName { get; set; }
            public Guid? CorrespondentID { get; set; }
            public string CorrespondentName { get; set; }
            public Guid? DepartmentID { get; set; }
            public string DepartmentIdx { get; set; }
            public string DepartmentName { get; set; }
            public Guid? OriginalID { get; set; }
            public string OriginalName { get; set; }
            public Guid? DeliveryTypeID { get; set; }
            public string DeliveryTypeName { get; set; }
            public string Contacts { get; set; }
            public string Housing { get; set; }
            public string ApartmentNumber { get; set; }
            public Guid? ComplaintFormatID { get; set; }
            public string ComplaintFormatName { get; set; }
            public Guid? ComplaintKindID { get; set; }
            public string ComplaintKindName { get; set; }
        }
    }
}
