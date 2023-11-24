using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Metadata;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow
{
    /// <summary>
    /// Расширение очищает поля с параметрами вариантов завершения у задачи согласования,
    /// когда задача завершается без удаления.
    /// </summary>
    public sealed class KrClearWasteInApprovalStoreTaskExtension : CardStoreTaskExtension
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

        public override async Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;

            if (!context.IsCompletion
                || context.State != CardRowState.Modified
                || (card = context.Task.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null)
            {
                return;
            }

            if (sections.TryGetValue("KrTask", out var krTask))
            {
                // TODO: страшный костыль. В ближайшее время подружить расширение с KrProcess
                card.Info["Comment"] = krTask.RawFields.TryGet("Comment", string.Empty);
                krTask.Fields["Comment"] = null;
            }

            if (sections.TryGetValue("KrCommentators", out var krCommentators))
            {
                MarkRowsAsDeleted(krCommentators);
            }

            if (sections.TryGetValue("KrAdditionalApprovalUsers", out var krAdditionalApprovalUsers))
            {
                MarkRowsAsDeleted(krAdditionalApprovalUsers);
            }

            if (sections.TryGetValue("KrAdditionalApproval", out var krAdditionalApproval))
            {
                CardMetadataColumnCollection columns = (await context.CardMetadata.GetSectionsAsync(context.CancellationToken))
                    ["KrAdditionalApproval"].Columns;

                IDictionary<string, object> fields = krAdditionalApproval.Fields;

                // TODO: страшный костыль. В ближайшее время подружить расширение с KrProcess
                var comment = fields["Comment"];
                if (comment != null)
                    card.Info["Comment"] = fields["Comment"];

                card.Info["TimeLimitation"] = fields["TimeLimitation"];
                card.Info["FirstIsResponsible"] = fields["FirstIsResponsible"];

                fields["Comment"] = columns["Comment"].DefaultValidValue;
                fields["TimeLimitation"] = columns["TimeLimitation"].DefaultValidValue;
                fields["FirstIsResponsible"] = columns["FirstIsResponsible"].DefaultValidValue;
            }
        }

        #endregion
    }
}
