using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow
{
    /// <summary>
    /// Расширение очищает поля с параметрами вариантов завершения у задачи доп. согласования,
    /// когда задача завершается без удаления.
    /// </summary>
    public sealed class KrClearWasteInAdditionalApprovalStoreTaskExtension : CardStoreTaskExtension
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

            if (sections.TryGetValue("KrCommentators", out CardSection krCommentators))
            {
                MarkRowsAsDeleted(krCommentators);
            }

            if (sections.TryGetValue("KrAdditionalApprovalTaskInfo", out CardSection krAdditionalApprovalTaskInfo))
            {
                // TODO: страшный костыль. В ближайшее время подружить расширение с KrProcess
                card.Info["Comment"] = krAdditionalApprovalTaskInfo.RawFields.TryGet("Comment", string.Empty);
                krAdditionalApprovalTaskInfo.Fields["Comment"] = null;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
