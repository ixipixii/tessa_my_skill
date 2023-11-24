using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Server.PnrTasks.Store
{
    public sealed class PnrChangeTaskResultStoreTaskExtension : CardStoreTaskExtension
    {
        /// <summary>
        /// Сохранение карточки задания и передача комментария в Digest
        /// </summary>
        public override async Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            if (context.Task.Action == CardTaskAction.Complete)
            {
                if (context.Task.Card.Sections.TryGetValue("FdTask", out CardSection fdTask) && fdTask.Fields.ContainsKey("Comment"))
                {
                    context.Task.Result = fdTask.Fields["Comment"] as string;
                }

                if (context.Task.Card.Sections.TryGetValue("KrTask", out CardSection krTask) && krTask.Fields.ContainsKey("Comment"))
                {
                    context.Task.Result = krTask.Fields["Comment"] as string;
                }
            }
        }
    }
}
