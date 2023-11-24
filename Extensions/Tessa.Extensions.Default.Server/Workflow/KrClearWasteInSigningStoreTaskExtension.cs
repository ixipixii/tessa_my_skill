using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow
{
    /// <summary>
    /// Расширение очищает поля с параметрами вариантов завершения у задачи подписания,
    /// когда задача завершается без удаления.
    /// </summary>
    public sealed class KrClearWasteInSigningStoreTaskExtension : CardStoreTaskExtension
    {
        #region Private Methods

        private static void MarkRowsAsDeleted(CardSection section)
        {
            ListStorage<CardRow> rows = section.TryGetRows();

            if (rows != null && rows.Count > 0)
            {
                foreach (CardRow row in rows)
                {
                    row.State = CardRowState.Deleted;
                }
            }
        }

        #endregion

        #region Base Overrides

        public override Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;

            if (!context.IsCompletion
                || context.State != CardRowState.Modified
                || (card = context.Task.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null)
            {
                return Task.CompletedTask;
            }

            if (sections.TryGetValue("KrTask", out var krTask))
            {
                // TODO: страшный костыль. В ближайше время подружить расширение с KrProcess1
                card.Info["Comment"] = krTask.RawFields.TryGet("Comment", string.Empty);
                krTask.Fields["Comment"] = null;
            }

            if (sections.TryGetValue("KrCommentators", out var krCommentators))
            {
                MarkRowsAsDeleted(krCommentators);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
