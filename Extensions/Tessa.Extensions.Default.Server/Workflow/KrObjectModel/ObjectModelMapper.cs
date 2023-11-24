using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public sealed class ObjectModelMapper : IObjectModelMapper
    {
        #region nested types

        private sealed class ObjectModelToCardRowContext
        {
            public CardStoreMode CardStoreMode;

            public IList<Stage> Stages;
            public SealableObjectList<Stage> InitialStages;

            // CardRow как для последовательного, так и произвольного доступа.
            public ListStorage<CardRow> StageRows;
            public HashSet<Guid, CardRow> StageRowsTable;
            public Guid? NestedProcessID;
            public Guid? ParentStageRowID;
            public int? NestedOrder;
        }

        #endregion

        #region fields

        private static readonly List<RouteDiff> emptyDiffList = new List<RouteDiff>(0);

        private readonly IKrStageSerializer serializer;

        private readonly ICardMetadata cardMetadata;

        private readonly IKrScope krScope;

        #endregion

        #region constructor

        public ObjectModelMapper(
            IKrStageSerializer serializer,
            IStageSettingsConverter stageSettingsConverter,
            ICardMetadata cardMetadata,
            IKrScope krScope)
        {
            this.serializer = serializer;
            this.cardMetadata = cardMetadata;
            this.krScope = krScope;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public MainProcessCommonInfo GetMainProcessCommonInfo(
            Card processHolderSatellite,
            bool withInfo = true)
        {
            if (!processHolderSatellite.TryGetKrApprovalCommonInfoSection(out var commonInfo))
            {
                return null;
            }

            // Берем значение инфо процесса, которое могло быть ранее кешировано
            // Таким образом, из объектной модели и кэша будет ссылка на один объект
            // далее при сохранении кэш будет сброшен и единственное актуальное значение будет в секции
            var processStateRow = withInfo
                ? ProcessInfoCacheHelper.Get(this.serializer, processHolderSatellite)
                : null;

            return new MainProcessCommonInfo(
                (Guid?)commonInfo.Fields[KrProcessCommonInfo.CurrentApprovalStageRowID],
                processStateRow,
                commonInfo.Fields.TryGet<Guid?>(KrSecondaryProcessCommonInfo.SecondaryProcessID),
                commonInfo.Fields.TryGet<Guid?>(KrApprovalCommonInfo.AuthorID),
                commonInfo.Fields.TryGet<string>(KrApprovalCommonInfo.AuthorName),
                commonInfo.Fields.TryGet<string>(KrApprovalCommonInfo.AuthorComment),
                commonInfo.Fields.TryGet<int>(StateID)
                );
        }

        /// <inheritdoc />
        public void SetMainProcessCommonInfo(
            Guid mainCardID,
            Card processHolderSatellite,
            MainProcessCommonInfo processCommonInfo)
        {
            if (!processHolderSatellite.TryGetKrApprovalCommonInfoSection(out var aci))
            {
                return;
            }

            aci.SetIfDiffer(KrProcessCommonInfo.CurrentApprovalStageRowID, processCommonInfo.CurrentStageRowID);
            ProcessInfoCacheHelper.Update(this.serializer, processHolderSatellite);
            if (processHolderSatellite.TypeID == DefaultCardTypes.KrSecondarySatelliteTypeID)
            {
                // Для вторички и этого хватит.
                aci.SetIfDiffer(KrSecondaryProcessCommonInfo.SecondaryProcessID, processCommonInfo.SecondaryProcessID);
                return;
            }

            aci.SetIfDiffer(KrApprovalCommonInfo.AuthorID, processCommonInfo.AuthorID);
            aci.SetIfDiffer(KrApprovalCommonInfo.AuthorName, processCommonInfo.AuthorName);
            aci.SetIfDiffer(KrApprovalCommonInfo.AuthorComment, processCommonInfo.AuthorComment);
            var stateChanged = aci.SetIfDiffer(StateID, processCommonInfo.State);
            if (stateChanged)
            {
                aci.Fields[KrApprovalCommonInfo.StateChangedDateTimeUTC] = DateTime.UtcNow;
                aci.SetIfDiffer(StateName, this.cardMetadata.GetDocumentStateName((KrState)processCommonInfo.State));

                if (processCommonInfo.AffectMainCardVersionWhenStateChanged)
                {
                    this.krScope.ForceIncrementMainCardVersion(mainCardID);
                }
            }
        }

        /// <inheritdoc />
        public List<NestedProcessCommonInfo> GetNestedProcessCommonInfos(
            Card processHolderSatellite)
        {
            if (processHolderSatellite.TryGetKrApprovalCommonInfoSection(out var aci)
                && aci.Fields.TryGetValue(KrProcessCommonInfo.NestedWorkflowProcesses, out var nwpObj))
            {
                if (nwpObj is string nwp
                    && !string.IsNullOrWhiteSpace(nwp))
                {
                    return this.serializer.Deserialize<List<object>>(nwp)
                        .Select(p => new NestedProcessCommonInfo((Dictionary<string, object>) p))
                        .ToList();
                }

                return new List<NestedProcessCommonInfo>();
            }

            return null;

        }

        /// <inheritdoc />
        public void SetNestedProcessCommonInfos(
            Card processHolderSatellite,
            IReadOnlyCollection<NestedProcessCommonInfo> nestedProcessCommonInfos)
        {
            if (nestedProcessCommonInfos is null)
            {
                return;
            }

            // Выпиливаем завершенные процессы
            var activeInfos = new List<object>(nestedProcessCommonInfos.Count);
            foreach (var info in nestedProcessCommonInfos)
            {
                if (info.CurrentStageRowID != null)
                {
                    activeInfos.Add(info.GetStorage());
                }
            }

            var nwpJson = this.serializer.Serialize(activeInfos);
            processHolderSatellite
                .GetApprovalInfoSection()
                .Fields[KrProcessCommonInfo.NestedWorkflowProcesses] = nwpJson;
        }

        /// <inheritdoc />
        public void FillWorkflowProcessFromPci(
            WorkflowProcess workflowProcess,
            ProcessCommonInfo commonInfo,
            MainProcessCommonInfo primaryProcessCommonInfo)
        {
            if (primaryProcessCommonInfo?.AuthorID != null)
            {
                workflowProcess.SetAuthor(new Author(
                    primaryProcessCommonInfo.AuthorID.Value,
                    primaryProcessCommonInfo.AuthorName),
                    false);
            }
            workflowProcess.SetAuthorComment(primaryProcessCommonInfo?.AuthorComment, false);
            workflowProcess.SetState((KrState)(primaryProcessCommonInfo?.State ?? 0), false);
            workflowProcess.CurrentApprovalStageRowID = commonInfo?.CurrentStageRowID;
        }

        /// <inheritdoc />
        public WorkflowProcess CardRowsToObjectModel(
            IKrStageTemplate stageTemplate,
            IReadOnlyCollection<IKrRuntimeStage> runtimeStages,
            MainProcessCommonInfo primaryPci = null,
            bool initialStage = true,
            bool saveInitialStages = false)
        {
            var krStages = new SealableObjectList<Stage>();
            foreach (var stageRow in runtimeStages.OrderBy(p => p.Order))
            {
                var stage = new Stage(stageRow, stageTemplate);
                krStages.Add(stage);
            }

            return new WorkflowProcess(
                primaryPci?.Info,
                primaryPci?.Info,
                krStages,
                saveInitialStages: true,
                nestedProcessID: null);
        }

        /// <inheritdoc />
        public WorkflowProcess CardRowsToObjectModel(
            Card processHolder,
            ProcessCommonInfo pci,
            MainProcessCommonInfo mainPci,
            IReadOnlyDictionary<Guid, IKrStageTemplate> templates,
            IReadOnlyDictionary<Guid, IReadOnlyCollection<IKrRuntimeStage>> runtimeStages,
            bool initialStage = true,
            Guid? nestedProcessID = null)
        {
            var pciNested = pci as NestedProcessCommonInfo;
            var stagesRows = processHolder.GetStagesSection().Rows;
            var krStages = new SealableObjectList<Stage>(stagesRows.Count);
            foreach (var stageRow in stagesRows
                .Where(p => p.State != CardRowState.Deleted && Equals(p[KrStages.NestedProcessID], pciNested?.NestedProcessID))
                .OrderBy(row => row.Fields[Order]))
            {
                var basedOnIDObj = stageRow.Fields[KrStages.BasedOnStageTemplateID];
                IKrStageTemplate templateCard = null;
                IReadOnlyCollection<IKrRuntimeStage> stages = null;
                if (basedOnIDObj is Guid basedOnID)
                {
                    templates.TryGetValue(basedOnID, out templateCard);
                    runtimeStages.TryGetValue(basedOnID, out stages);
                }

                if (stages is null)
                {
                    stages = EmptyHolder<IKrRuntimeStage>.Collection;
                }

                var settings = this.serializer.DeserializeSettingsStorageAsync(stageRow).GetAwaiter().GetResult(); // TODO async
                var stageInfoStorage = this.ParseInfoStorage(stageRow);

                var stage = new Stage(
                    stageRow,
                    settings,
                    stageInfoStorage,
                    templateCard,
                    stages,
                    initialStage);

                krStages.Add(stage);
            }

            return new WorkflowProcess(
                pci.Info,
                mainPci.Info,
                krStages,
                saveInitialStages: true,
                nestedProcessID: nestedProcessID);
        }

        /// <inheritdoc />
        public void ObjectModelToPci(
            WorkflowProcess process,
            ProcessCommonInfo pci,
            MainProcessCommonInfo mainPci,
            MainProcessCommonInfo primaryPci)
        {
            pci.CurrentStageRowID = process.CurrentApprovalStageRowID;
            if (primaryPci != null)
            {
                if (primaryPci.AuthorTimestamp < process.AuthorTimestamp)
                {
                    primaryPci.AuthorID = process.Author?.AuthorID;
                    primaryPci.AuthorName = process.Author?.AuthorName;
                    primaryPci.AuthorTimestamp = process.AuthorTimestamp;
                }
                if (primaryPci.AuthorCommentTimestamp < process.AuthorCommentTimestamp)
                {
                    primaryPci.AuthorComment = process.AuthorComment;
                    primaryPci.AuthorCommentTimestamp = process.AuthorCommentTimestamp;
                }
                if (primaryPci.StateTimestamp < process.StateTimestamp)
                {
                    primaryPci.State = process.State.ID;
                    primaryPci.StateTimestamp = process.StateTimestamp;
                }

                if (primaryPci.AffectMainCardVersionWhenStateChangedTimestamp < process.AffectMainCardVersionWhenStateChangedTimestamp)
                {
                    primaryPci.AffectMainCardVersionWhenStateChanged = process.AffectMainCardVersionWhenStateChanged;
                    primaryPci.AffectMainCardVersionWhenStateChangedTimestamp = process.AffectMainCardVersionWhenStateChangedTimestamp;
                }
            }
            pci.Info = process.InfoStorage;
            if (!ReferenceEquals(pci, mainPci))
            {
                StorageHelper.Merge(process.MainProcessInfoStorage, mainPci.Info);
            }
        }

        /// <inheritdoc />
        public List<RouteDiff> ObjectModelToCardRows(
            WorkflowProcess process,
            Card baseCard,
            ProcessCommonInfo pci)
        {
            var ctx = new ObjectModelToCardRowContext
            {
                CardStoreMode = baseCard.StoreMode,
                Stages = process.Stages ?? throw new ArgumentNullException(nameof(process.Stages)),
                InitialStages = process.InitialWorkflowProcess?.Stages
                    ?? throw new ArgumentNullException(nameof(process.InitialWorkflowProcess.Stages)),
            };
            if (pci is NestedProcessCommonInfo npci)
            {
                ctx.NestedProcessID = npci.NestedProcessID;
                ctx.ParentStageRowID = npci.ParentStageRowID;
                ctx.NestedOrder = npci.NestedOrder;
            }

            // Будут использоваться только от текущего нестеда, но отдаем всю коллекцию, чтобы была
            // возможность модифицировать ее по ссылке.
            ctx.StageRows = baseCard.GetStagesSection().Rows;
            // Здесь строится хештаблица только по текущему нестеду.
            ctx.StageRowsTable = new HashSet<Guid, CardRow>(
                p => p.TryGet<Guid?>(KrStages.BasedOnStageRowID) ?? p.RowID,
                ctx.StageRows.Where(p => Equals(p[KrStages.NestedProcessID], ctx.NestedProcessID)));

            return this.MoveStagesIntoCardRows(ctx);

        }

        #endregion

        #region CardRowsToObjectModel

        /// <summary>
        /// Разобрать хранилище info из JSON, который хранится в fields по ключу Info.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private IDictionary<string, object> ParseInfoStorage(IDictionary<string, object> fields)
        {
            if (fields.TryGetValue(Info, out var stateObj)
                && stateObj is string state
                && !string.IsNullOrWhiteSpace(state))
            {
                return this.serializer.Deserialize<Dictionary<string, object>>(state);
            }

            return new Dictionary<string, object>();
        }

        #endregion

        #region ObjectModelToCardRows

        /// <summary>
        /// Исправить ссылки StageRowID в подставленных настройках, а также выставить порядок сортировки.
        /// </summary>
        /// <param name="stageRowID"></param>
        /// <param name="stage"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RepairSettings(Guid stageRowID, Stage stage)
        {
            var storage = stage.SettingsStorage;
            foreach (var referenceToStages in this.serializer.ReferencesToStages)
            {
                if (storage.TryGetValue(referenceToStages.SectionName, out var rowsObj)
                    && rowsObj is IList<object> rows)
                {
                    foreach (var row in rows)
                    {
                        if (row is IDictionary<string, object> rowStorage)
                        {
                            Guid? oldStageRowID;
                            if ((oldStageRowID = rowStorage.TryGet<Guid?>(referenceToStages.RowIDFieldName)) != null
                                && oldStageRowID != stageRowID)
                            {
                                rowStorage[referenceToStages.RowIDFieldName] = stageRowID;
                                break;
                            }
                        }
                    }
                }
            }

            if (stage.Performers != null)
            {
                var order = 0;
                foreach (var performer in stage.Performers)
                {
                    performer.GetStorage()[Order] = order++;
                }
            }
        }

        /// <summary>
        /// Заполнение полей строки коллекционной секции этапа согласования
        /// </summary>
        /// <param name="stage">Абстракция этапа(источник данных для полей)</param>
        /// <param name="initialStage"></param>
        /// <param name="stageRow">Строка этапа(куда записать данные)</param>
        /// <param name="ctx"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FillStageSections(
            Stage stage,
            Stage initialStage,
            CardRow stageRow,
            ObjectModelToCardRowContext ctx)
        {
            if (initialStage == null
                || !StorageHelper.Equals(stage.SettingsStorage, initialStage.SettingsStorage))
            {
                this.serializer.SerializeSettingsStorage(stageRow, stage.SettingsStorage);
            }

            stageRow.SetIfDiffer(Name, stage.Name);
            stageRow.SetIfDiffer(KrStages.TimeLimit, stage.TimeLimit);
            stageRow.SetIfDiffer(KrStages.Planned, stage.Planned);
            stageRow.SetIfDiffer(KrStages.Hidden, stage.Hidden);
            stageRow.SetIfDiffer(KrStages.Skip, stage.Skip);
            stageRow.SetIfDiffer(KrStages.CanBeSkipped, stage.CanBeSkipped);

            stageRow.SetIfDiffer(StageGroupID, stage.StageGroupID);
            stageRow.SetIfDiffer(StageGroupName, stage.StageGroupName);
            stageRow.SetIfDiffer(StageGroupOrder, stage.StageGroupOrder);

            stageRow.SetIfDiffer(KrStages.RowChanged, stage.RowChanged);
            stageRow.SetIfDiffer(KrStages.OrderChanged, stage.OrderChanged);

            stageRow.SetIfDiffer(KrStages.StageTypeID, stage.StageTypeID);
            stageRow.SetIfDiffer(KrStages.StageTypeCaption, stage.StageTypeCaption);

            stageRow.SetIfDiffer(KrStages.NestedProcessID, ctx.NestedProcessID);
            stageRow.SetIfDiffer(KrStages.ParentStageRowID, ctx.ParentStageRowID);
            stageRow.SetIfDiffer(KrStages.NestedOrder, ctx.NestedOrder);

            // Этап может быть не привязанным вообще, в случае для этапа, добавленного пользователем
            // Этап может быть привязан только к шаблону этапов, это этап, добавленный в пользовательских скриптах
            // Этап может быть привязан шаблону этапов и к конкретному этапу внутри шаблона.
            if (stage.BasedOnTemplate)
            {
                stageRow.Fields[KrStages.BasedOnStageTemplateID] = stage.TemplateID;
                stageRow.Fields[KrStages.BasedOnStageTemplateName] = stage.TemplateName;
                stageRow.Fields[KrStages.BasedOnStageTemplateOrder] = stage.TemplateOrder;
                stageRow.Fields[KrStages.BasedOnStageTemplateGroupPositionID] = stage.GroupPosition.ID;
            }
            if (stage.BasedOnTemplateStage)
            {
                stageRow.Fields[KrStages.BasedOnStageRowID] = stage.ID;
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<RouteDiff> MoveNewStagesIntoCardRows(
            ObjectModelToCardRowContext ctx)
        {
            var diffs = new List<RouteDiff>(ctx.Stages.Count);
            for (var stageIndex = 0; stageIndex < ctx.Stages.Count; stageIndex++)
            {
                var newStage = ctx.Stages[stageIndex];
                var cardRow = AddRow(ctx.StageRows, newStage.RowID);

                this.RepairSettings(cardRow.RowID, newStage);
                this.FillStageSections(newStage, null, cardRow, ctx);
                cardRow.Fields[Info] = this.serializer.Serialize(newStage.InfoStorage);
                var stateChanged = cardRow.SetIfDiffer(KrStages.StageStateID, (int)newStage.State);
                if (stateChanged)
                {
                    cardRow.SetIfDiffer(KrStages.StageStateName, this.cardMetadata.GetStageStateName(newStage.State));
                }

                UpdateRowOrder(cardRow, stageIndex, Order, onlyIfNeeded: false);
                diffs.Add(RouteDiff.NewStage(cardRow.RowID, newStage.Name, newStage.Hidden));
            }
            return diffs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<RouteDiff> DeleteAllStageRows(
            ObjectModelToCardRowContext ctx)
        {
            if (ctx.InitialStages != null)
            {
                var diffs = new List<RouteDiff>(ctx.InitialStages.Count);
                foreach (var oldStage in ctx.InitialStages)
                {
                    var cardRow = ctx.StageRowsTable[oldStage.ID];
                    cardRow.State = CardRowState.Deleted;
                    diffs.Add(RouteDiff.DeleteStage(cardRow.RowID, oldStage.Name, oldStage.Hidden));
                }
                return diffs;
            }
            return emptyDiffList;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<RouteDiff> DeleteRedundantStageRows(
            ObjectModelToCardRowContext ctx,
            HashSet<Guid, Stage> initialStagesTable)
        {
            var diffs = new List<RouteDiff>(ctx.InitialStages.Count);
            var redundantIDs = ctx.InitialStages
                .Select(x => x.ID)
                .Except(ctx.Stages.Select(x => x.ID));
            foreach (var redundantRowID in redundantIDs)
            {
                var cardRow = ctx.StageRowsTable[redundantRowID];
                cardRow.State = CardRowState.Deleted;
                var stage = initialStagesTable[redundantRowID];
                diffs.Add(RouteDiff.DeleteStage(cardRow.RowID, stage.Name, stage.Hidden));
            }
            return diffs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnbindUnconfirmedOrDeletedStage(CardRow stageRow)
        {
            stageRow.Fields[KrStages.BasedOnStageTemplateID] = null;
            stageRow.Fields[KrStages.BasedOnStageTemplateName] = null;
            stageRow.Fields[KrStages.BasedOnStageTemplateOrder] = null;
            stageRow.Fields[KrStages.BasedOnStageTemplateGroupPositionID] = null;
            stageRow.Fields[KrStages.BasedOnStageRowID] = null;
        }

        private List<RouteDiff> MoveStagesIntoCardRows(ObjectModelToCardRowContext ctx)
        {
            // Новых этапов нет, маршрут пустой, нужно удалить все старые этапы
            if (ctx.Stages.Count == 0)
            {
                return DeleteAllStageRows(ctx);
            }
            // Старых этапов нет, нужно просто создать новые
            if (ctx.InitialStages == null
                || ctx.InitialStages.Count == 0)
            {
                return this.MoveNewStagesIntoCardRows(ctx);
            }

            var initialStagesTable = new HashSet<Guid, Stage>(x => x.ID, ctx.InitialStages);
            var diffs = new List<RouteDiff>(ctx.Stages.Count + ctx.InitialStages.Count);
            for (var stageIndex = 0; stageIndex < ctx.Stages.Count; stageIndex++)
            {
                var stage = ctx.Stages[stageIndex];
                var oldStage = initialStagesTable.TryGetItem(stage.ID, out var os) ? os : null;
                var cardRow = ctx.StageRowsTable.TryGetItem(stage.ID, out var cr) ? cr : null;
                RouteDiff diff = null;
                // Для корректного восстановления и сравнения с существующими нужно восстановить
                // StageRowID конкретной карточки.
                if (cardRow != null)
                {
                    this.RepairSettings(cardRow.RowID, stage);
                }
                // Если текущий этап не совпадает с этапом до пересчета по значению или порядку
                // то этап можно считать добавленным или измененным,
                // в зависимости от того, был ли уже этап с таким ID
                if (ctx.InitialStages.Count <= stageIndex
                    || stage != ctx.InitialStages[stageIndex])
                {
                    if (oldStage != null
                        && cardRow != null)
                    {
                        // изменен этап oldStage -> stage
                        if (cardRow.State == CardRowState.None)
                        {
                            cardRow.State = CardRowState.Modified;
                        }
                        diff = RouteDiff.ModifyStage(
                            cardRow.RowID,
                            stage.Name,
                            stage.Hidden,
                            stage.Name != oldStage.Name ? oldStage.Name : null);
                    }
                    else
                    {
                        // добавлен этап stage
                        cardRow = AddRow(ctx.StageRows, stage.RowID);
                        diff = RouteDiff.NewStage(
                            cardRow.RowID,
                            stage.Name,
                            stage.Hidden);
                    }

                    this.FillStageSections(stage, oldStage, cardRow, ctx);
                }
                if (cardRow != null)
                {
                    // Шаблон с таким флагом может дойти в двух случаях:
                    // 1. Шаблон удален, но этап изменился пользователем, поэтому остался
                    // 2. Шаблон не подтвержден, но этап изменился пользователем, поэтому остался
                    // В обоих случаях нужно забыть, что этап связан с шаблоном.
                    if (stage.UnbindTemplate)
                    {
                        UnbindUnconfirmedOrDeletedStage(cardRow);
                    }

                    if (oldStage is null
                        || stage.IsInfoChanged(oldStage))
                    {
                        cardRow.Fields[Info] = this.serializer.Serialize(stage.InfoStorage);
                    }
                    var stateChanged = cardRow.SetIfDiffer(KrStages.StageStateID, (int)stage.State);
                    if (stateChanged)
                    {
                        cardRow.SetIfDiffer(KrStages.StageStateName, this.cardMetadata.GetStageStateName(stage.State));
                    }

                    UpdateRowOrder(cardRow, stageIndex, Order);
                }
                if (diff != null)
                {
                    diffs.Add(diff);
                }
            }
            // Пометка лишних на удаление.
            // Лишние определяются как разность множеств начальных и текущих этапов.
            var deleteDiffs = DeleteRedundantStageRows(ctx, initialStagesTable);
            diffs.AddRange(deleteDiffs);

            // Если возникает кейс, когда операция производится над еще не сохраненной карточкой,
            // Но в ней уже был какой-то маршрут, то после пересчета этапы могут пропасть.
            // Их нужно удалить из коллекции, а не отправить на сохранение с состоянием Deleted.
            // Возникает, например, при создании копии
            if (ctx.CardStoreMode == CardStoreMode.Insert)
            {
                ctx.StageRows.RemoveAll(p => p.State == CardRowState.Deleted);
            }

            return diffs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateRowOrder(CardRow row, int order, string alias, bool onlyIfNeeded = true)
        {
            if (!onlyIfNeeded)
            {
                row.Fields[alias] = order;
                return;
            }

            if (!row.Fields.TryGetValue(alias, out var oldOrderObj)
                || !(oldOrderObj is int oldOrder)
                || oldOrder != order)
            {
                row.Fields[alias] = order;
                if (row.State == CardRowState.None)
                {
                    row.State = CardRowState.Modified;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CardRow AddRow(ListStorage<CardRow> rows, Guid? rowID)
        {
            var row = rows.Add();
            row.State = CardRowState.Inserted;
            row.RowID = rowID ?? Guid.NewGuid();
            return row;
        }

        #endregion
    }
}