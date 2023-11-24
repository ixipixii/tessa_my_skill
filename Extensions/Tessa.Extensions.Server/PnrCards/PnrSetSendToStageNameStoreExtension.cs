using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.FdProcesses.Shared.Fd;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrSetSendToStageNameStoreExtension : CardStoreExtension
    {
        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;

            if ((card = context.Request.TryGetCard()) == null)
            {
                return;
            }

            foreach (var cardTaskHistoryItem in card.TaskHistory)
            {
                if (cardTaskHistoryItem.Result.Contains("Процесс принудительно отправлен на этап"))
                {
                    cardTaskHistoryItem.TypeID = new System.Guid("22222222-2222-2222-2222-222222222222");
                    cardTaskHistoryItem.TypeCaption = "Отправка на этап";
                }
            }
        }
    }
}
