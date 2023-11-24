using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Forums;

namespace Tessa.Extensions.Default.Server.Forum
{
    public class ForumGetExtension :CardGetExtension
    {
        private readonly IForumProvider forumProvider;
        private readonly IKrTypesCache krTypesCache;


        public ForumGetExtension(IForumProvider forumProvider, IKrTypesCache krTypesCache)
        {
            this.forumProvider = forumProvider;
            this.krTypesCache = krTypesCache;
        }

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (!context.RequestIsSuccessful
                || context.CardType == null
                || context.CardType?.Flags.HasAny(CardTypeFlags.Hidden | CardTypeFlags.Administrative) == true
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.Request.ServiceType == CardServiceType.Default
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }

            if (KrComponentsHelper.HasBase(context.CardType.ID, this.krTypesCache))
            {
                KrComponents usedComponents = KrComponentsHelper.GetKrComponents(card, this.krTypesCache);
                var krType = await KrProcessSharedHelper.TryGetKrTypeAsync(
                    this.krTypesCache, card, card.TypeID, cancellationToken: context.CancellationToken);
                
                if (usedComponents.Has(KrComponents.UseForum) || krType != null && krType.UseForum)
                {
                    var response = await this.forumProvider.GetTopicsWithMessagesAsync(
                        card.ID,
                        false,
                        ForumHelper.MessagesInTopicСount,
                        ForumHelper.TopicsCount,
                        ForumHelper.FromDate,
                        context.CancellationToken);

                    if (response.ValidationResult.IsSuccessful())
                    {
                        //TODO: удалить лишние поля, поля которые не нужны для формирования топиков в области заданий
                        if (ForumHelper.IsExistDefaultMessages(response.Topics))
                        {
                            card.Topics = response.Topics;
                        }
                        card.Info.Add(ForumHelper.ForumSettingsKey, response.ForumSettings);
                    }
                }
            }
        }
    }
}
