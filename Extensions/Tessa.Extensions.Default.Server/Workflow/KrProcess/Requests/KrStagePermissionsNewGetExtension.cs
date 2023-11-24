using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrStagePermissionsNewGetExtension : CardNewGetExtension
    {
        #region nested types

        /// <summary>
        /// Тип, несущий в себе информацию о разрешениях на определенные действия со строками
        /// этапов, подставляемых из шаблонов этапов
        /// </summary>
        public struct KrTemplatePermissionsInfo
        {
            /// <summary>
            /// ID карточки шаблона
            /// </summary>
            public Guid TemplateID;

            /// <summary>
            /// ID группы этапов.
            /// </summary>
            public Guid StageGroupID;

            /// <summary>
            /// Можно ли менять порядок этапа
            /// </summary>
            public bool CanChangeOrder;

            /// <summary>
            /// Можно ли изменять содержимое этапа
            /// </summary>
            public bool IsReadonly;

            /// <summary>
            /// Положение этапа относительно вручную добавленных
            /// </summary>
            public GroupPosition GroupPosition;
        }

        #endregion

        #region fields

        private readonly IKrTypesCache typesCache;

        private readonly IKrProcessCache processCache;

        #endregion

        #region constructor

        public KrStagePermissionsNewGetExtension(
            IKrTypesCache typesCache,
            IKrProcessCache processCache)
        {
            this.typesCache = typesCache;
            this.processCache = processCache;
        }

        #endregion

        #region base overrides

        public override async Task AfterRequest(
            ICardNewExtensionContext context)
        {
            Card card;
            if (context.CardType == null
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) == null
                || !KrProcessHelper.CardSupportsRoutes(
                    context.Response.Card,
                    context.DbScope,
                    this.typesCache))
            {
                return;
            }

            await this.SetStageRowsPermissionsAsync(card, context.DbScope, context.CancellationToken);
        }


        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (context.CardType == null
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) == null
                || !KrProcessHelper.CardSupportsRoutes(
                        context.Response.Card,
                        context.DbScope,
                        this.typesCache))
            {
                return;
            }

            await this.SetStageRowsPermissionsAsync(card, context.DbScope, context.CancellationToken);
        }

        #endregion

        #region private

        /// <summary>
        /// Установка прав на этапы согласования, сгенерированные по шаблону.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task SetStageRowsPermissionsAsync(Card card, IDbScope dbScope, CancellationToken cancellationToken = default)
        {
            // Здесь возможны дубли, однако через Sql они отфильтруются
            var requiredTemplates = card.GetStagesSection()
                .Rows
                .Where(p => p.Fields[KrConstants.KrStages.BasedOnStageTemplateID] != null)
                .Select(p => (Guid)p.Fields[KrConstants.KrStages.BasedOnStageTemplateID])
                .ToList();
            var allTemplatesPermissions = await GetStageTemplatePermissionsAsync(dbScope, requiredTemplates, cancellationToken);
            var groupIDs = new HashSet<Guid>(allTemplatesPermissions.Select(p => p.StageGroupID));
            var stageGroups = this.processCache.GetStageGroups(groupIDs);

            var stagesSecPermissions =
                card.Permissions.Sections.GetOrAddTable(KrConstants.KrStages.Virtual);

            foreach (var row in card.GetStagesSection().Rows)
            {
                if (row.TryGet<bool>(KrConstants.Keys.RootStage)
                    || row.TryGet<bool>(KrConstants.Keys.NestedStage))
                {
                    // Для форка и его нестед строк все просто - они запрещены к редактированию в любом случае
                    var perm = stagesSecPermissions.Rows.GetOrAdd(row.RowID);
                    perm.SetRowPermissions(CardPermissionFlags.ProhibitDeleteRow);
                    perm.SetRowPermissions(CardPermissionFlags.ProhibitModify);
                }
                else if (row.Fields[KrConstants.KrStages.BasedOnStageTemplateID] != null
                    && allTemplatesPermissions.TryGetItem((Guid)row.Fields[KrConstants.KrStages.BasedOnStageTemplateID], out var templatePermissions))
                {
                    if (row.Fields.TryGetValue(KrConstants.KrStages.BasedOnStageTemplateGroupPositionID, out var grPosObj)
                        && (grPosObj as int?) != templatePermissions.GroupPosition.ID)
                    {
                        templatePermissions.GroupPosition = GroupPosition.GetByID(grPosObj as int?);
                    }

                    var group = stageGroups.FirstOrDefault(p => p.ID == templatePermissions.StageGroupID);
                    if (group != null)
                    {
                        SetRowPermissions(stagesSecPermissions.Rows.GetOrAdd(row.RowID), row, templatePermissions, group);
                    }
                }
            }
        }

        /// <summary>
        /// Установить права на строку этапа.
        /// </summary>
        /// <param name="stageRowPermissionsInfo">Права доступа на отдельную строку коллекционной секции - этапа.</param>
        /// <param name="row">Этап для которого выполняется настройка прав.</param>
        /// <param name="templatePermissions">Информация о разрешениях задаваемых через карточку шаблона этапов.</param>
        /// <param name="group">Информация о группе этапов.</param>
        private static void SetRowPermissions(
            CardRowPermissionInfo stageRowPermissionsInfo,
            CardRow row,
            KrTemplatePermissionsInfo templatePermissions,
            IKrStageGroup group)
        {
            if (stageRowPermissionsInfo.RowPermissions.HasNot(CardPermissionFlags.ProhibitModify))
            {
                stageRowPermissionsInfo.SetRowPermissions(
                    stageRowPermissionsInfo.RowPermissions.Has(CardPermissionFlags.AllowDeleteRow)
                    && KrStageSerializer.CanBeSkipped(row)
                    && !row.TryGet<bool>(KrConstants.KrStages.Skip)
                    ? CardPermissionFlags.AllowDeleteRow
                    : CardPermissionFlags.ProhibitDeleteRow);
            }

            stageRowPermissionsInfo.SetFieldPermissions(KrConstants.Name, CardPermissionFlags.ProhibitModify);
            if (group.IsGroupReadonly || !templatePermissions.CanChangeOrder)
            {
                stageRowPermissionsInfo.SetFieldPermissions(KrConstants.Order, CardPermissionFlags.ProhibitModify);
            }

            if ((group.IsGroupReadonly || templatePermissions.IsReadonly)
                && stageRowPermissionsInfo.RowPermissions.HasNot(CardPermissionFlags.ProhibitModify))
            {
                var fieldPermissions = stageRowPermissionsInfo.TryGetFieldPermissions();
                var prohibitChangeOrder = fieldPermissions != null
                    && fieldPermissions.TryGetValue(KrConstants.Order, out var orderPermissions)
                    && orderPermissions.Has(CardPermissionFlags.ProhibitModify);

                stageRowPermissionsInfo.SetRowPermissions(CardPermissionFlags.ProhibitModify);

                // Если до установки прав на всю строку не было запрета на смену порядка,
                // то запрет на смену порядка через запрет на изменение всей строки идет лесом
                if (!prohibitChangeOrder)
                {
                    stageRowPermissionsInfo.SetFieldPermissions(KrConstants.Order, CardPermissionFlags.AllowModify);
                }

            }
        }

        /// <summary>
        /// Получение информации о правах на работу со строками этапов для этапов,
        /// подставленных из шаблонов этапов.
        /// </summary>
        /// <param name="dbScope"></param>
        /// <param name="stageTemplateIDs"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns></returns>
        private static async Task<HashSet<Guid, KrTemplatePermissionsInfo>> GetStageTemplatePermissionsAsync(
            IDbScope dbScope,
            List<Guid> stageTemplateIDs,
            CancellationToken cancellationToken = default)
        {
            var result = new HashSet<Guid, KrTemplatePermissionsInfo>(x => x.TemplateID, stageTemplateIDs.Count);
            if (stageTemplateIDs.Count == 0)
            {
                return result;
            }
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory
                    .Select().C(null,
                        "ID",
                        "StageGroupID",
                        "CanChangeOrder",
                        "IsStagesReadonly",
                        "GroupPositionID")
                    .From("KrStageTemplates").NoLock()
                    .Where().C("ID").In(stageTemplateIDs.ToArray());

                await using var reader = await db
                    .SetCommand(builder.Build())
                    .LogCommand()
                    .ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Add(new KrTemplatePermissionsInfo
                    {
                        TemplateID = reader.GetGuid(0),
                        StageGroupID = reader.GetGuid(1),
                        CanChangeOrder = reader.GetBoolean(2),
                        IsReadonly = reader.GetBoolean(3),
                        GroupPosition = GroupPosition.GetByID(reader.GetNullableInt32(4))
                    });
                }
            }
            return result;
        }

        #endregion
    }
}