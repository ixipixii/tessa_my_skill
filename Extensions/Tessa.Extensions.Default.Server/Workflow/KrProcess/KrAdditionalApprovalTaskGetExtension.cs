using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrAdditionalApprovalCardGetExtension :
        CardGetExtension
    {
        #region ChildResolutionInfo Private Class

        private sealed class AdditionalApprovalInfo
        {
            #region Constructors

            public AdditionalApprovalInfo(
                Guid rowID,
                string comment,
                string answer,
                DateTime created,
                DateTime planned,
                Guid performerID,
                string performerName,
                DateTime? inProgress,
                Guid? userID,
                string userName,
                DateTime? completed,
                Guid? optionID,
                string optionCaption,
                bool isResponsible)
            {
                this.RowID = rowID;
                this.Comment = comment;
                this.Answer = answer;
                this.Created = created;
                this.Planned = planned;
                this.PerformerID = performerID;
                this.PerformerName = performerName;
                this.InProgress = inProgress;
                this.UserID = userID;
                this.UserName = userName;
                this.Completed = completed;
                this.OptionID = optionID;
                this.OptionCaption = optionCaption;
                this.IsResponsible = isResponsible;
            }

            #endregion

            #region Properties

            public Guid RowID { get; }

            public string Comment { get; }

            public string Answer { get; }

            public DateTime Created { get; }

            public DateTime Planned { get; }

            public Guid PerformerID { get; }

            public string PerformerName { get; }

            public DateTime? InProgress { get; }

            public Guid? UserID { get; }

            public string UserName { get; }

            public DateTime? Completed { get; }

            public Guid? OptionID { get; }

            public string OptionCaption { get; }

            public bool IsResponsible { get; }

            #endregion
        }

        #endregion

        #region Private Methods

        private static async Task<List<AdditionalApprovalInfo>> TryGetParentInfoAsync(
            CardTask task,
            DbManager db,
            IQueryBuilderFactory builderFactory,
            CancellationToken cancellationToken = default)
        {
            db
                .SetCommand(
                    builderFactory
                        .Select().C(null,
                            "RowID", "Comment", "Answer", "Created", "Planned", "PerformerID", "PerformerName",
                            "InProgress", "UserID", "UserName", "Completed", "OptionID", "OptionCaption", "IsResponsible")
                        .From("KrAdditionalApprovalInfo").NoLock()
                        .Where().C("ID").Equals().P("TaskRowID")
                        .Build(),
                    db.Parameter("TaskRowID", task.ParentRowID))
                .LogCommand();

            var result = new List<AdditionalApprovalInfo>();
            await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Add(new AdditionalApprovalInfo(
                        reader.GetGuid(0),
                        reader.GetValue<string>(1),
                        reader.GetValue<string>(2),
                        reader.GetDateTimeUtc(3),
                        reader.GetDateTimeUtc(4),
                        reader.GetGuid(5),
                        reader.GetValue<string>(6),
                        reader.GetNullableDateTimeUtc(7),
                        reader.GetNullableGuid(8),
                        reader.GetValue<string>(9),
                        reader.GetNullableDateTimeUtc(10),
                        reader.GetNullableGuid(11),
                        reader.GetValue<string>(12),
                        reader.GetBoolean(13)));
                }
            }

            return result;
        }

        private static List<AdditionalApprovalInfo> TryGetChildrenInfo(CardTask task)
        {
            Card taskCard = task.TryGetCard();
            StringDictionaryStorage<CardSection> taskSections;
            ListStorage<CardRow> childrenRows;
            if (taskCard == null
                || (taskSections = taskCard.TryGetSections()) == null
                || !taskSections.TryGetValue(nameof(KrAdditionalApprovalInfo), out CardSection childrenSection)
                || (childrenRows = childrenSection.TryGetRows()) == null
                || childrenRows.Count == 0)
            {
                return null;
            }

            var result = new List<AdditionalApprovalInfo>(childrenRows.Count);
            foreach (CardRow childrenRow in childrenRows)
            {
                result.Add(
                    new AdditionalApprovalInfo(
                        childrenRow.RowID,
                        childrenRow.Get<string>("Comment"),
                        childrenRow.Get<string>("Answer"),
                        childrenRow.Get<DateTime>("Created"),
                        childrenRow.Get<DateTime>("Planned"),
                        childrenRow.Get<Guid>("PerformerID"),
                        childrenRow.Get<string>("PerformerName"),
                        childrenRow.Get<DateTime?>("InProgress"),
                        childrenRow.Get<Guid?>("UserID"),
                        childrenRow.Get<string>("UserName"),
                        childrenRow.Get<DateTime?>("Completed"),
                        childrenRow.Get<Guid?>("OptionID"),
                        childrenRow.Get<string>("OptionCaption"),
                        childrenRow.Get<bool>("IsResponsible")));
            }

            return result;
        }

        private void SetAdditionalApprovalInfoToVirtual(CardTask task, List<AdditionalApprovalInfo> infoList)
        {
            if (infoList == null || infoList.Count == 0)
            {
                return;
            }

            CardSection virtualSection = task.Card.Sections.GetOrAdd(KrAdditionalApprovalInfo.Virtual);
            virtualSection.Type = CardSectionType.Table;
            virtualSection.TableType = CardTableType.Collection;

            ListStorage<CardRow> rows = virtualSection.Rows;
            rows.Clear();

            foreach (AdditionalApprovalInfo info in infoList.OrderBy(x => x.Created))
            {
                CardRow row = rows.Add();
                StoreAdditionalApprovalInfo(info, row);
                // по умолчанию уже установлено: row.State = CardRowState.None;
            }
        }


        private static void StoreAdditionalApprovalInfo(
            AdditionalApprovalInfo info,
            CardRow row)
        {
            row.RowID = info.RowID;
            row["Comment"] = info.Comment;
            row["Answer"] = LocalizationManager.Format(info.Answer);
            row["Created"] = info.Created;
            row["Planned"] = info.Planned;
            row["PerformerID"] = info.PerformerID;
            row["PerformerName"] = info.PerformerName;
            row["InProgress"] = info.InProgress;
            row["UserID"] = info.UserID;
            row["UserName"] = info.UserName;
            row["Completed"] = info.Completed;
            row["OptionID"] = info.OptionID;
            row["OptionCaption"] = info.OptionCaption;
            row["ColumnComment"] = GetColumnComment(info);
            row["ColumnState"] = GetColumnState(info);
            row["IsResponsible"] = info.IsResponsible;
        }


        private static string GetColumnComment(AdditionalApprovalInfo info)
        {
            return info.Comment
                .ReplaceLineEndingsAndTrim().NormalizeSpaces()
                .Limit(4000);
        }

        private static string GetColumnState(AdditionalApprovalInfo info)
        {
            return GetState(
                info.PerformerName,
                info.UserID.HasValue
                    ? info.UserName
                    : null,
                info.OptionID);
        }

        private static string GetState(
            string performerRoleName,
            string userName,
            Guid? completionOptionID)
        {
            string state;
            if (completionOptionID.HasValue)
            {
                state = completionOptionID.Value == DefaultCompletionOptions.Revoke
                    ? string.Format(LocalizationManager.GetString("WfResolution_State_Revoked"), LocalizationManager.Localize(userName))
                    : string.Format(LocalizationManager.GetString("Cards_TaskState_Completed"), LocalizationManager.Localize(userName));
            }
            else if (userName != null)
            {
                state = string.Format(LocalizationManager.GetString("Cards_TaskState_InProgress"), LocalizationManager.Localize(userName));
            }
            else
            {
                state = string.Format(LocalizationManager.GetString("Cards_TaskState_Created"), LocalizationManager.Localize(performerRoleName));
            }

            return state.Limit(4000);
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (!context.RequestIsSuccessful
                || context.CardType == null
                || context.CardType.Flags.HasNot(CardTypeFlags.AllowTasks)
                || context.Request.RestrictionFlags.Has(CardGetRestrictionFlags.RestrictTaskSections)
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }

            await using (context.DbScope.Create())
            {
                ListStorage<CardTask> tasks = card.TryGetTasks();

                var db = context.DbScope.Db;
                var builderFactory = context.DbScope.BuilderFactory;
                if (tasks != null && tasks.Count > 0)
                {
                    foreach (CardTask task in tasks)
                    {
                        if (task.TypeID == DefaultTaskTypes.KrAdditionalApprovalTypeID)
                        {
                            List<AdditionalApprovalInfo> infoList = await TryGetParentInfoAsync(task, db, builderFactory, context.CancellationToken);
                            this.SetAdditionalApprovalInfoToVirtual(task, infoList);
                        }
                        else if (task.TypeID == DefaultTaskTypes.KrApproveTypeID)
                        {
                            List<AdditionalApprovalInfo> infoList = TryGetChildrenInfo(task);
                            this.SetAdditionalApprovalInfoToVirtual(task, infoList);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
