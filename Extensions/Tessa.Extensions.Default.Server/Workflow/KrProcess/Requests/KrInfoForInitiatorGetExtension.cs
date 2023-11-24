﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrInfoForInitiatorGetExtension : CardGetExtension
    {
        #region Fields

        private readonly ICardRepository defaultRepository;

        private readonly IKrTypesCache krCache;

        #endregion

        #region Constructors

        public KrInfoForInitiatorGetExtension(
            [Dependency(CardRepositoryNames.Default)] ICardRepository defaultRepository,
            IKrTypesCache krCache)
        {
            this.defaultRepository = defaultRepository;
            this.krCache = krCache;
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (!context.RequestIsSuccessful
                || context.CardType.Flags.HasNot(CardTypeFlags.AllowTasks)
                || context.Request.RestrictionFlags.HasAny(CardGetRestrictionFlags.RestrictTasks | CardGetRestrictionFlags.RestrictTaskSections)
                || !context.ValidationResult.IsSuccessful()
                || (card = context.Response.TryGetCard()) == null
                || KrComponentsHelper.GetKrComponents(card, this.krCache).HasNot(KrComponents.Routes)
                || !(await context.CardMetadata.GetCardTypesAsync(context.CancellationToken))
                    .Contains(DefaultTaskTypes.KrInfoForInitiatorTypeID))
            {
                return;
            }

            List<CardRow> rows = null;
            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;

                db
                    .SetCommand(
                        context.DbScope.BuilderFactory
                            .Select()
                            .C("t", "RowID", "RoleName", "UserName", "InProgress")
                            .From("KrActiveTasks", "at").NoLock()
                            .InnerJoin("KrApprovalCommonInfo", "kr").NoLock()
                            .On().C("kr", "ID").Equals().C("at", "ID")
                            .InnerJoin("Tasks", "t").NoLock()
                            .On().C("t", "RowID").Equals().C("at", "TaskID")
                            .Where().C("kr", "MainCardID").Equals().P("CardID")
                            .Build(),
                        db.Parameter("CardID", card.ID))
                    .LogCommand();

                await using DbDataReader reader = await db.ExecuteReaderAsync(context.CancellationToken);
                while (await reader.ReadAsync(context.CancellationToken))
                {
                    Guid rowID = reader.GetGuid(0);
                    string roleName = reader.GetString(1);
                    string userName = reader.GetValue<string>(2);
                    DateTime? inProgress = reader.GetNullableDateTimeUtc(3);

                    if (rows == null)
                    {
                        rows = new List<CardRow>();
                    }

                    rows.Add(new CardRow
                    {
                        RowID = rowID,
                        [KrConstants.KrInfoForInitiator.ApproverRole] = roleName,
                        [KrConstants.KrInfoForInitiator.ApproverUser] = userName,
                        [KrConstants.KrInfoForInitiator.InProgress] = inProgress,
                        State = CardRowState.None,
                    });
                }
            }

            if (rows != null)
            {
                var taskRequest = new CardNewRequest { CardTypeID = DefaultTaskTypes.KrInfoForInitiatorTypeID };
                CardNewResponse taskResponse = await this.defaultRepository.NewAsync(taskRequest, context.CancellationToken);

                ValidationResult taskResult = taskResponse.ValidationResult.Build();
                context.ValidationResult.Add(taskResult);

                if (!taskResult.IsSuccessful)
                {
                    return;
                }

                Card taskCard = taskResponse.Card;
                CardSection infoForInitiator = taskCard.Sections.GetOrAddTable(KrConstants.KrInfoForInitiator.Name);
                infoForInitiator.Rows.AddItems(rows);
                taskCard.ID = Guid.NewGuid();

                IUser user = context.Session.User;
                taskCard.CreatedByID = user.ID;
                taskCard.CreatedByName = user.Name;

                CardTask task = context.Response.Card.Tasks.Insert(0);
                task.SetCard(taskCard);

                //Оливковый
                //task.SetBackground(StorageColor.FromArgb(0x88, 145, 170, 58));
                task.SetBackground(StorageColor.FromArgb(0xDD, 229, 223, 200));
                task.SectionRows = taskResponse.SectionRows;
                task.RoleID = Session.SystemID;
                task.RoleName = Session.SystemName;
                task.State = CardRowState.None;
            }
        }

        #endregion
    }
}