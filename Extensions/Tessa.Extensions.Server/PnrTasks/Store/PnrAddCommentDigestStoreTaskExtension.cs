using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    public sealed class PnrAddCommentDigestStoreTaskExtension : CardStoreTaskExtension
    {
        public override async Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            Card card;
            if (!context.ValidationResult.IsSuccessful()
                || (card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            // при создании задания по любому документу
            if (!context.IsCompletion && context.Task.State == CardRowState.Inserted)
            {
                // в дайджест положим комментарий с предыдущего этапа
                string lastComment = card.Tasks.LastOrDefault(t => t.Result != null)?.Result;

                if (!string.IsNullOrEmpty(lastComment))
                {
                    context.Task.Digest = context.Task.Digest + "\n" + "Комментарий: " + lastComment;
                }
            }
        }
    }
}
