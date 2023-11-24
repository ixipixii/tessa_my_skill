using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrIncomingNewExtension : CardNewExtension
    {
        private readonly Tessa.Platform.Runtime.ISession session;
        private readonly ICardRepository cardRepository;

        public PnrIncomingNewExtension(Tessa.Platform.Runtime.ISession session, ICardRepository cardRepository)
        {
            this.session = session;
            this.cardRepository = cardRepository;
        }

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;
            var currentUserID = session.User.ID;

            //установим подразделение автора по умолчанию. Убрал - заполняем на клиенте при выборе вида "рекламация"
            //SetUserDepartmentInfo(currentUserID, card);

            var isCurrentUserClerk = await PnrIsUserInRole.GetIsUserInRole(context.DbScope, currentUserID, PnrRoles.Clerk);
            var isCurrentUserOfficeManager = await PnrIsUserInRole.GetIsUserInRole(context.DbScope, currentUserID, PnrRoles.OfficeManager);
            var isCurrentUserAdmin = session.User.AccessLevel == Tessa.Platform.Runtime.UserAccessLevel.Administrator;

            if (isCurrentUserAdmin || (isCurrentUserClerk && isCurrentUserOfficeManager))
            {
                card.Sections["PnrIncoming"].Fields["DocumentKindID"] = PnrIncomingTypes.IncomingLetterID;
                card.Sections["PnrIncoming"].Fields["DocumentKindIdx"] = PnrIncomingTypes.IncomingLetterIdx;
                card.Sections["PnrIncoming"].Fields["DocumentKindName"] = PnrIncomingTypes.IncomingLetterName;
            }
            else
            {
                if (isCurrentUserClerk)
                {
                    card.Sections["PnrIncoming"].Fields["DocumentKindID"] = PnrIncomingTypes.IncomingLetterID;
                    card.Sections["PnrIncoming"].Fields["DocumentKindIdx"] = PnrIncomingTypes.IncomingLetterIdx;
                    card.Sections["PnrIncoming"].Fields["DocumentKindName"] = PnrIncomingTypes.IncomingLetterName;
                }
                if (isCurrentUserOfficeManager)
                {
                    card.Sections["PnrIncoming"].Fields["DocumentKindID"] = PnrIncomingTypes.IncomingComplaintsID;
                    card.Sections["PnrIncoming"].Fields["DocumentKindIdx"] = PnrIncomingTypes.IncomingComplaintsIdx;
                    card.Sections["PnrIncoming"].Fields["DocumentKindName"] = PnrIncomingTypes.IncomingComplaintsName;
                }
            }

            // return Task.CompletedTask;
        }
        private async void SetUserDepartmentInfo(Guid? authorID, Card card)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetUserDepartmentInfoRequestTypeID,
                Info =
                {
                    { "authorID", authorID }
                }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            var result = response.ValidationResult.Build();
            if (result.IsSuccessful)
            {
                card.Sections["PnrIncoming"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                card.Sections["PnrIncoming"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                card.Sections["PnrIncoming"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
            }
        }
    }
}