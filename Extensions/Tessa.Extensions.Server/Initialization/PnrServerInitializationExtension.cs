using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.Platform.Initialization;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.Initialization
{
    public sealed class PnrServerInitializationExtension : ServerInitializationExtension
    {
        private readonly Tessa.Platform.Runtime.ISession session;
        private readonly ICardRepository cardRepository;

        public PnrServerInitializationExtension(Tessa.Platform.Runtime.ISession session, ICardRepository cardRepository)
        {
            this.session = session;
            this.cardRepository = cardRepository;
        }

        public override async Task AfterRequest(IServerInitializationExtensionContext context)
        {
            if (!context.RequestIsSuccessful
             || context.ConfigurationIsCached
             || context.Request.Info.TryGet<bool>("JustCards"))
            {
                return;
            }

            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetIsUserInRoleExtension,
                Info =
                    {
                        { "userID", this.session.User.ID},
                        { "roleID", PnrRoles.EmployeeUkID }
                    }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            var isUserInRole = response.Info.Get<bool>("isUserInRole");

            context.Response.Info["isUserInRole"] = BooleanBoxes.Box(isUserInRole);

            var currentUserID = session.User.ID;

            var isCurrentUserClerk = await PnrIsUserInRole.GetIsUserInRole(context.DbScope, currentUserID, PnrRoles.Clerk);
            context.Response.Info["isCurrentUserClerk"] = BooleanBoxes.Box(isCurrentUserClerk);

            var isCurrentUserOfficeManager = await PnrIsUserInRole.GetIsUserInRole(context.DbScope, currentUserID, PnrRoles.OfficeManager);
            context.Response.Info["isCurrentUserOfficeManager"] = BooleanBoxes.Box(isCurrentUserOfficeManager);
        }
    }
}
