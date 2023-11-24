using System;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrUniversalTaskStoreExtension : CardStoreTaskExtension
    {
        #region Fields

        public const string OptionIDKey = CardHelper.SystemKeyPrefix + "universalOptionID";

        #endregion

        #region Base Overrides

        public override Task StoreTaskBeforeRequest(ICardStoreTaskExtensionContext context)
        {
            if (context.Task.OptionID.HasValue)
            {
                Guid optionID = context.Task.OptionID.Value;
                if (optionID == DefaultCompletionOptions.Cancel)
                {
                    // HandleInterrupt отзывает задание с вариантом Cancel, см. UniversalTaskStageTypeHandler
                    return Task.CompletedTask;
                }

                var optionRow = context
                    .Task
                    .Card
                    .Sections
                    .GetOrAddTable(nameof(KrConstants.KrUniversalTaskOptions))
                    .Rows
                    .FirstOrDefault(x => x.Get<Guid?>(KrConstants.KrUniversalTaskOptions.OptionID) == optionID);

                if (optionRow == null)
                {
                    context.ValidationResult.AddError(this, $"Invalid completion option ID={optionID:B} when completing universal task.");
                    return Task.CompletedTask;
                }

                var showComment = optionRow.TryGet<bool>(KrConstants.KrUniversalTaskOptionsSettingsVirtual.ShowComment);
                if (showComment)
                {
                    context.Task.Result = context.Task.Card.Sections.GetOrAdd(nameof(KrConstants.KrTask)).RawFields.TryGet<string>(KrConstants.KrTask.Comment);
                }

                context.StoreContext.SetTaskAccessCheckIsIgnored(context.Task.RowID);
                context.Task.Info[OptionIDKey] = context.Task.OptionID;
                context.Task.OptionID = DefaultCompletionOptions.Approve;
            }

            return Task.CompletedTask;
        }


        public override async Task StoreTaskBeforeCommitTransaction(ICardStoreTaskExtensionContext context)
        {
            if (context.Task.Info.TryGetValue(OptionIDKey, out var optionIDObj)
                && optionIDObj is Guid optionID)
            {
                var optionRow = context
                    .Task
                    .Card
                    .Sections
                    .GetOrAddTable(nameof(KrConstants.KrUniversalTaskOptions))
                    .Rows
                    .FirstOrDefault(x => x.Get<Guid?>(KrConstants.KrUniversalTaskOptions.OptionID) == optionID);

                if (optionRow == null)
                {
                    context.ValidationResult.AddError(this, "Wrong option ID");
                    return;
                }

                var executor = context.DbScope.Executor;

                await executor.ExecuteNonQueryAsync(
                    context.DbScope.BuilderFactory
                        .Update("TaskHistory")
                            .C("OptionID").Equals().P("OptionID")
                            .C("OptionCaption").Equals().P("Caption")
                        .Where().C("RowID").Equals().P("TaskID")
                        .Build(),
                    context.CancellationToken,
                    executor.Parameter("TaskID", context.Task.RowID),
                    executor.Parameter("Caption", optionRow.Get<string>(KrConstants.KrUniversalTaskOptions.Caption)),
                    executor.Parameter("OptionID", optionID));
            }
        }

        #endregion
    }
}
