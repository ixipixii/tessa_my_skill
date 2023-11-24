using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Json;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public sealed class KrStageSerializer : IKrStageSerializer
    {
        #region nested types

        private sealed class SettingsSectionsVisitor : DescendantSectionsVisitor
        {
            private readonly Dictionary<Guid, IDictionary<string, object>> settingsStorages;

            private readonly IKrStageSerializer serializer;

            /// <inheritdoc />
            public SettingsSectionsVisitor(
                ICardMetadata cardMetadata,
                IKrStageSerializer serializer,
                Dictionary<Guid, IDictionary<string, object>> settingsStorages)
                : base(cardMetadata)
            {
                this.settingsStorages = settingsStorages;
                this.serializer = serializer;
            }

            /// <inheritdoc />
            protected override void VisitTopLevelSection(
                CardRow row,
                CardSection section,
                IDictionary<Guid, Guid> stageMapping)
            {
                var rowCopy = row.Clone();
                if (!this.settingsStorages.TryGetValue(rowCopy.RowID, out var rowStorage))
                {
                    rowStorage = new Dictionary<string, object>();
                    this.settingsStorages.Add(rowCopy.RowID, rowStorage);
                }

                PrepareUpdatedRow(rowCopy);
                var updatedRowStorage = rowCopy.GetStorage();
                foreach (var plainSetting in this.serializer.SettingsFieldNames)
                {
                    if (updatedRowStorage.TryGetValue(plainSetting, out var newValue))
                    {
                        rowStorage[plainSetting] = newValue;
                    }
                }
            }

            /// <inheritdoc />
            protected override void VisitNestedSection(
                CardRow row,
                CardSection section,
                Guid parentRowID,
                Guid topLevelRowID,
                IDictionary<Guid, Guid> stageMapping)
            {
                var rowCopy = row.Clone();
                if (!this.settingsStorages.TryGetValue(topLevelRowID, out var rowStorage))
                {
                    rowStorage = new Dictionary<string, object>();
                    this.settingsStorages.Add(rowCopy.RowID, rowStorage);
                }

                var sectionRows = rowStorage.TryGet<IList<object>>(section.Name);
                if (sectionRows == null)
                {
                    sectionRows = new List<object>();
                    rowStorage[section.Name] = sectionRows;
                }

                PrepareUpdatedRow(rowCopy);
                ProcessUpdatedRow(rowCopy, sectionRows);
                sectionRows.Add(rowCopy.GetStorage());
            }
        }

        private struct TemporaryHiddenRowInfo
        {
            public CardRow Row;
            public int AbsolutePosition;
        }

        private sealed class StageNode
        {
            public CardRow Row;
            public Guid? ParentStageRowID;
            public readonly List<StageNode> Children = new List<StageNode>();
            public int Level; // = 0

            public override string ToString() => $"{this.Row[KrConstants.Name]} {this.Row.RowID} Children = {this.Children.Count}";
        }

        #endregion

        #region fields

        // Это абсолютно случайный и ни на что не влияющий идентификатор
        // нужен только для удачного восстановления структуры
        private static readonly Guid fakeCardID =
            new Guid(0xCECE4CEE, 0x5B29, 0x4FE5, 0x8A, 0xE0, 0x43, 0x49, 0xDF, 0xF0, 0x01, 0x81);

        private readonly object lockObj = new object();

        private readonly ICardRepairManager repairManager;

        private readonly ICardMetadata cardMetadata;

        private readonly IStageTypeFormatterContainer formatterContainer;

        private readonly ISession session;

        private readonly IKrProcessContainer processContainer;

        private readonly IExtensionContainer extensionContainer;

        #endregion

        #region constructor

        public KrStageSerializer(
            ICardMetadata cardMetadata,
            [Unity.Dependency(CardRepairManagerNames.Default)]
            ICardRepairManager repairManager,
            IStageTypeFormatterContainer formatterContainer,
            ISession session,
            IKrProcessContainer processContainer,
            IExtensionContainer extensionContainer)
        {
            this.cardMetadata = cardMetadata;
            this.repairManager = repairManager;
            this.formatterContainer = formatterContainer;
            this.session = session;
            this.processContainer = processContainer;
            this.extensionContainer = extensionContainer;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IReadOnlyList<string> SettingsSectionNames { get; private set; }

        /// <inheritdoc />
        public IReadOnlyList<string> SettingsFieldNames { get; private set; }

        /// <inheritdoc />
        public IReadOnlyList<ReferenceToStage> ReferencesToStages { get; private set; }

        /// <inheritdoc />
        public IReadOnlyList<OrderColumn> OrderColumns { get; private set; }

        /// <inheritdoc />
        public void SetData(
            KrStageSerializerData data)
        {
            lock (this.lockObj)
            {
                var distinctSections = new HashSet<string>(data.SettingsSectionNames);
                var distinctFields = new HashSet<string>(data.SettingsFieldNames);
                var distinctReferences = new HashSet<ReferenceToStage>(data.ReferencesToStages);
                var distinctOrders = new HashSet<OrderColumn>(data.OrderColumns);

                this.SettingsSectionNames = distinctSections.ToList().AsReadOnly();
                this.SettingsFieldNames = distinctFields.ToList().AsReadOnly();
                this.ReferencesToStages = distinctReferences.ToList().AsReadOnly();
                this.OrderColumns = distinctOrders.ToList().AsReadOnly();
            }
        }

        /// <inheritdoc />
        public string Serialize(object value)
        {
            return StorageHelper.SerializeToJson(value, TessaSerializer.JsonTyped);
        }

        /// <inheritdoc />
        public T Deserialize<T>(
            string json)
        {
            return StorageHelper.DeserializeFromJson<T>(json, TessaSerializer.JsonTyped);
        }

        /// <inheritdoc />
        public IDictionary<Guid, IDictionary<string, object>> CreateStageSettings(
            StringDictionaryStorage<CardSection> allSections)
        {
            var settingsStorages = new Dictionary<Guid, IDictionary<string, object>>();
            new SettingsSectionsVisitor(this.cardMetadata, this, settingsStorages)
                .Visit(allSections, DefaultCardTypes.KrCardTypeID, KrConstants.KrStages.Virtual);

            foreach (var rowStorages in settingsStorages.Values)
            {
                foreach (var empty in this.SettingsSectionNames.Where(p => !rowStorages.ContainsKey(p)))
                {
                    rowStorages.Add(empty, EmptyHolder<object>.Array);
                }
            }

            return settingsStorages;
        }

        /// <inheritdoc />
        public async ValueTask<IDictionary<Guid, IDictionary<string, object>>> MergeStageSettingsAsync(
            CardSection krStagesSection,
            StringDictionaryStorage<CardSection> updatedSections,
            CancellationToken cancellationToken = default)
        {
            var stageStorages = new Dictionary<Guid, IDictionary<string, object>>();
            foreach (var sectionName in this.SettingsSectionNames)
            {
                if (!updatedSections.TryGetValue(sectionName, out var updatedSection))
                {
                    continue;
                }

                foreach (var updatedRowOriginal in updatedSection.Rows)
                {
                    // Копируем строку, чтобы не портить основную.
                    var updatedRow = updatedRowOriginal.Clone();

                    if (!updatedRow.TryGetValue(KrConstants.Keys.ParentStageRowID, out var stageRowIDObj)
                        || !(stageRowIDObj is Guid stageRowID))
                    {
                        // Не знаем, что это за строка. Пропускаем.
                        continue;
                    }

                    // Получаем storage для необходимого этапа.
                    // Если его нет(а точнее строки), то создадим новое хранилище.
                    if (!stageStorages.TryGetValue(stageRowID, out var stageStorage))
                    {
                        var originalRow = krStagesSection.Rows.FirstOrDefault(p => p.RowID == stageRowID);
                        stageStorage = originalRow != null
                            ? await this.DeserializeSettingsStorageAsync(originalRow, cancellationToken: cancellationToken)
                            : new Dictionary<string, object>();
                        stageStorages[stageRowID] = stageStorage;
                    }

                    if (sectionName == KrConstants.KrStages.Virtual)
                    {
                        // Поля из строки KrStagesVirtual располагаются на верхнем уровне
                        PrepareUpdatedRow(updatedRow);
                        var updatedRowStorage = updatedRow.GetStorage();
                        foreach (var plainSetting in this.SettingsFieldNames)
                        {
                            if (updatedRowStorage.TryGetValue(plainSetting, out var newValue))
                            {
                                stageStorage[plainSetting] = newValue;
                            }
                        }
                    }
                    else
                    {
                        // Получаем секцию из storage конкретного этапа
                        // Для упрощения - секция это сразу массив строк.
                        var sectionRows = stageStorage.TryGet<IList<object>>(sectionName);
                        if (sectionRows == null)
                        {
                            sectionRows = new List<object>();
                            stageStorage[sectionName] = sectionRows;
                        }

                        ProcessUpdatedRow(updatedRow, sectionRows);
                    }
                }
            }

            foreach (var rowStorages in stageStorages.Values)
            {
                foreach (var empty in this.SettingsSectionNames.Where(p => !rowStorages.ContainsKey(p)))
                {
                    rowStorages.Add(empty, EmptyHolder<object>.Array);
                }
            }

            return stageStorages;
        }

        /// <inheritdoc />
        public void SerializeSettingsStorage(
            CardRow stageRow,
            IDictionary<string, object> settingsStorage)
        {
            if (!(settingsStorage is Dictionary<string, object> settingsDict))
            {
                throw new InvalidOperationException();
            }

            foreach (var orderColumn in this.OrderColumns)
            {
                if (settingsDict.TryGetValue(orderColumn.SectionName, out var rowsObj)
                    && rowsObj is IList<object> rows)
                {
                    KrProcessSharedHelper.RepairStorageRowsOrders(rows, orderColumn.OrderFieldName);
                }
            }

            settingsDict.Remove(KrConstants.Keys.ParentStageRowID);
            stageRow.Fields[KrConstants.KrStages.Settings] = this.Serialize(settingsDict);
        }

        /// <inheritdoc />
        public async ValueTask<IDictionary<string, object>> DeserializeSettingsStorageAsync(
            string settings,
            Guid rowID,
            bool repairStorage = true,
            CancellationToken cancellationToken = default)
        {
            var stageStorage = this.Deserialize<Dictionary<string, object>>(settings);
            if (!repairStorage)
            {
                return stageStorage;
            }

            var doubleSize = 2 * stageStorage.Count;
            var redundantKeys = new List<string>(stageStorage.Count);
            var stageRowStorage = new Dictionary<string, object>(doubleSize) { [KrConstants.RowID] = rowID };
            var sectionsStorage = new Dictionary<string, object>(doubleSize);
            foreach (var pair in stageStorage)
            {
                if (pair.Key == KrConstants.KrStages.Virtual)
                {
                    continue;
                }

                if (pair.Value is IList<object> collectionRows
                    && this.SettingsSectionNames.Contains(pair.Key))
                {
                    var sectionStorage = new Dictionary<string, object>
                    {
                        [".table"] = (int) CardTableType.Collection,
                        ["Rows"] = collectionRows
                    };

                    sectionsStorage[pair.Key] = sectionStorage;
                }
                else if (this.SettingsFieldNames.Contains(pair.Key))
                {
                    stageRowStorage[pair.Key] = pair.Value;
                }
                else
                {
                    redundantKeys.Add(pair.Key);
                }
            }

            var stagesSection = new Dictionary<string, object>
            {
                [".table"] = (int) CardTableType.Collection,
                ["Rows"] = new List<object> { stageRowStorage },
            };
            sectionsStorage[KrConstants.KrStages.Virtual] = stagesSection;

            var cardToRepair = CreateFakeRepairableCard(null, sectionsStorage);

            await this.repairManager.RepairAsync(cardToRepair, cancellationToken: cancellationToken);

            foreach (var settingsFieldName in this.SettingsFieldNames)
            {
                stageStorage[settingsFieldName] = stageRowStorage[settingsFieldName];
            }

            foreach (var settingsSectionName in this.SettingsSectionNames)
            {
                if (!stageStorage.ContainsKey(settingsSectionName))
                {
                    stageStorage[settingsSectionName] = new List<object>();
                }
            }

            foreach (var redundantKey in redundantKeys)
            {
                stageStorage.Remove(redundantKey);
            }

            return stageStorage;
        }

        /// <inheritdoc />
        public ValueTask<IDictionary<string, object>> DeserializeSettingsStorageAsync(
            CardRow stageRow,
            bool repairStorage = true,
            CancellationToken cancellationToken = default)
        {
            if (!stageRow.TryGetValue(KrConstants.KrStages.Settings, out var settingsObj)
                || !(settingsObj is string settings)
                || string.IsNullOrWhiteSpace(settings))
            {
                return new ValueTask<IDictionary<string, object>>(new Dictionary<string, object>());
            }

            return this.DeserializeSettingsStorageAsync(settings, stageRow.RowID, repairStorage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task DeserializeSectionsAsync(
            Card sourceCard,
            Card destCard,
            IDictionary<Guid, IDictionary<string, object>> predeserializedSettings = null,
            KrProcessSerializerHiddenStageMode hiddenStageMode = KrProcessSerializerHiddenStageMode.Ignore,
            ICardExtensionContext cardExtensionContext = null,
            CancellationToken cancellationToken = default)
        {
            bool considerHiddenStages = hiddenStageMode != KrProcessSerializerHiddenStageMode.Ignore;

            if (!destCard.Sections.ContainsKey(KrConstants.KrStages.Virtual))
            {
                return;
            }

            var cardToRepair = CreateFakeRepairableCard(destCard, null);
            var sections = cardToRepair.Sections;
            var sourceRows = sourceCard.GetStagesSection().Rows;
            var stagePositions = new List<object>(sourceRows.Count);

            await this.ParseSettingsAsync(
                sourceRows, sections, stagePositions, predeserializedSettings,
                hiddenStageMode, considerHiddenStages, cancellationToken);

            var cardType = (await this.cardMetadata.GetCardTypesAsync(cancellationToken))[destCard.TypeID];
            var routeCardType = KrProcessSharedHelper.DesignTimeCard(cardType.ID)
                ? RouteCardType.Template
                : RouteCardType.Document;

            var context = new KrStageRowExtensionContext(
                null,
                cardToRepair,
                sourceCard,
                destCard,
                cardType,
                routeCardType,
                cardExtensionContext,
                cancellationToken);

            await using var executor = await this.extensionContainer.ResolveExecutorAsync<IKrStageRowExtension>(cancellationToken);
            await executor.ExecuteAsync(x => x.DeserializationBeforeRepair, context);
            await this.repairManager.RepairAsync(cardToRepair, cancellationToken: cancellationToken);

            var sectionsStorage = sections.GetStorage();
            foreach (var section in destCard.Sections)
            {
                if (!sectionsStorage.ContainsKey(section.Key)
                    || !this.SettingsSectionNames.Contains(section.Key))
                {
                    sectionsStorage[section.Key] = section.Value.GetStorage();
                }
            }

            destCard.Sections = new StringDictionaryStorage<CardSection>(
                sectionsStorage,
                CardComponentHelper.SectionFactory);

            if (considerHiddenStages)
            {
                destCard.SetStagePositions(stagePositions);
            }

            await executor.ExecuteAsync(x => x.DeserializationAfterRepair, context);
        }

        /// <inheritdoc />
        public async Task UpdateStageSettingsAsync(
            Card innerCard,
            Card outerCard,
            IDictionary<Guid, IDictionary<string, object>> stageStorages,
            IKrProcessCache krProcessCache,
            ICardExtensionContext cardExtensionContext = null,
            CancellationToken cancellationToken = default)
        {
            if (stageStorages == null)
            {
                return;
            }

            if (!innerCard.TryGetStagesSection(out var satelliteStages))
            {
                return;
            }

            var cardType = (await this.cardMetadata.GetCardTypesAsync(cancellationToken))[innerCard.TypeID];
            var routeCardType = KrProcessSharedHelper.DesignTimeCard(cardType.ID)
                ? RouteCardType.Template
                : RouteCardType.Document;

            var context = new KrStageRowExtensionContext(
                stageStorages,
                null,
                innerCard,
                outerCard,
                cardType,
                routeCardType,
                cardExtensionContext);

            await using (var executor = await this.extensionContainer.ResolveExecutorAsync<IKrStageRowExtension>(cancellationToken))
            {
                await executor.ExecuteAsync(x => x.BeforeSerialization, context);
            }

            foreach (var row in satelliteStages.Rows)
            {
                if (row.State == CardRowState.Deleted
                    || !stageStorages.TryGetValue(row.RowID, out IDictionary<string, object> storage))
                {
                    continue;
                }

                if (row.State == CardRowState.Modified)
                {
                    if (CanBeSkipped(row)
                        && row.IsChanged(KrConstants.KrStages.Skip)
                        && !row.Fields.TryGet<bool>(KrConstants.KrStages.Skip))
                    {
                        var runtimeStages = krProcessCache.GetRuntimeStagesForTemplate(row.TryGet<Guid>(KrConstants.KrStages.BasedOnStageTemplateID));

                        var runtimeStage = runtimeStages.Single(i => i.StageID == row.Fields.TryGet<Guid>(KrConstants.KrStages.BasedOnStageRowID));

                        row.SetIfDiffer(KrConstants.KrStages.Hidden, runtimeStage.Hidden);
                    }
                }

                this.FormatRow(row, innerCard, storage);
                this.SerializeSettingsStorage(row, storage);
            }
        }

        #endregion

        #region private

        private static void AppendLevelLabel(
            StageNode node,
            CardRow rowCopy,
            int[] stairs)
        {
            const int spacesPerLevel = 2;
            if (node.Level <= 0)
            {
                return;
            }

            var sb = StringBuilderHelper.Acquire();
            for (var i = 0; i < node.Level * spacesPerLevel; i++)
            {
                sb.Append(' ');
            }

            sb.Append('(');
            if (node.Level > 1)
            {
                sb.Append(stairs[1] + 1);
            }

            for (var i = 2; i < node.Level; i++)
            {
                sb.Append('.');
                sb.Append(stairs[i] + 1);
            }

            if (node.Level > 1)
            {
                sb.Append('.');
            }

            sb.Append((int) rowCopy[KrConstants.KrStages.NestedOrder] + 1);
            sb.Append(") ");
            sb.Append(rowCopy[KrConstants.Name]);
            rowCopy[KrConstants.Name] = sb.ToStringAndRelease();
        }

        private static void SetTreeStructureMark(
            StageNode node,
            CardRow rowCopy)
        {
            rowCopy[KrConstants.Keys.RootStage] = BooleanBoxes.Box(node.Level == 0 && node.Children.Count > 0);
            rowCopy[KrConstants.Keys.NestedStage] =
                BooleanBoxes.Box(rowCopy.TryGet<Guid?>(KrConstants.KrStages.NestedProcessID) != null);
        }

        private static IEnumerable<StageNode> CreateHierarchicalListOfNodes(ICollection<CardRow> rows)
        {
            var nodes = new List<StageNode>(rows.Count);
            var nodesTable = new HashSet<Guid, StageNode>(p => p.Row.RowID);
            var hasChild = false;
            foreach (var row in rows)
            {
                var node = new StageNode
                {
                    Row = row,
                    ParentStageRowID = row.TryGet<Guid?>(KrConstants.KrStages.ParentStageRowID),
                };
                nodes.Add(node);
                nodesTable.Add(node);

                if (node.ParentStageRowID != null)
                {
                    hasChild = true;
                }
            }

            if (!hasChild)
            {
                return nodes;
            }

            foreach (var node in nodes)
            {
                if (node.ParentStageRowID != null
                    && nodesTable.TryGetItem(node.ParentStageRowID.Value, out var parentNode))
                {
                    parentNode.Children.Add(node);
                }
            }

            foreach (var node in nodes)
            {
                if (node.Children.Count > 0)
                {
                    node.Children.Sort(CompareStageNodes);
                }
            }

            return nodes
                .Where(p => p.ParentStageRowID is null);
        }

        private static int CompareStageNodes(
            StageNode x,
            StageNode y)
        {
            var xNest = (int) x.Row[KrConstants.KrStages.NestedOrder];
            var yNest = (int) y.Row[KrConstants.KrStages.NestedOrder];
            var byNested = xNest.CompareTo(yNest);
            if (byNested != 0)
            {
                return byNested;
            }

            var xOrder = (int) x.Row[KrConstants.Order];
            var yOrder = (int) y.Row[KrConstants.Order];
            return xOrder.CompareTo(yOrder);
        }

        private static Card CreateFakeRepairableCard(Card card, Dictionary<string, object> sectionsStorage)
        {
            Card fakeCard;
            if (card?.TypeID == DefaultCardTypes.KrStageTemplateTypeID)
            {
                fakeCard = new Card
                {
                    ID = fakeCardID,
                    TypeID = DefaultCardTypes.KrStageTemplateTypeID,
                    TypeName = DefaultCardTypes.KrStageTemplateTypeName,
                    TypeCaption = DefaultCardTypes.KrStageTemplateTypeName,
                };
            }
            else if (card?.TypeID == DefaultCardTypes.KrSecondaryProcessTypeID)
            {
                fakeCard = new Card
                {
                    ID = fakeCardID,
                    TypeID = DefaultCardTypes.KrSecondaryProcessTypeID,
                    TypeName = DefaultCardTypes.KrSecondaryProcessTypeName,
                    TypeCaption = DefaultCardTypes.KrSecondaryProcessTypeName,
                };
            }
            else
            {
                fakeCard = new Card
                {
                    ID = fakeCardID,
                    TypeID = DefaultCardTypes.KrCardTypeID,
                    TypeName = DefaultCardTypes.KrCardTypeName,
                    TypeCaption = DefaultCardTypes.KrCardTypeName,
                };
            }

            if (sectionsStorage != null)
            {
                fakeCard.Sections =
                    new StringDictionaryStorage<CardSection>(sectionsStorage, CardComponentHelper.SectionFactory);
            }

            return fakeCard;
        }

        private static void PrepareUpdatedRow(
            CardRow updatedRow)
        {
            updatedRow.ClearChanges();
            updatedRow.GetStorage().Remove(CardHelper.SystemKeyPrefix + "state");
            updatedRow.GetStorage().Remove(KrConstants.Keys.ParentStageRowID);
        }

        private static bool DictContainsSameRowID(
            object dictObj,
            Guid rowID)
        {
            return dictObj is IDictionary<string, object> dict
                && dict.TryGetValue(KrConstants.RowID, out var rowIDObj)
                && (rowIDObj is Guid rid || rowIDObj is string rids && Guid.TryParse(rids, out rid))
                && rid == rowID;
        }

        private static void ProcessUpdatedRow(
            CardRow updatedRow,
            IList<object> sectionRows)
        {
            switch (updatedRow.State)
            {
                case CardRowState.Inserted:
                    PrepareUpdatedRow(updatedRow);
                    sectionRows.Add(updatedRow.GetStorage());
                    break;
                case CardRowState.Modified:
                    PrepareUpdatedRow(updatedRow);
                    var sectionRow = sectionRows
                        .FirstOrDefault(p => DictContainsSameRowID(p, updatedRow.RowID));
                    if (sectionRow is IDictionary<string, object> sectionRowStorage)
                    {
                        var settingsStorage = updatedRow.GetStorage();
                        StorageHelper.Merge(settingsStorage, sectionRowStorage);
                    }

                    break;
                case CardRowState.Deleted:
                    sectionRows.RemoveAll(p => DictContainsSameRowID(p, updatedRow.RowID));
                    break;
            }
        }

        private void FormatRow(
            CardRow innerRow,
            Card innerCard,
            IDictionary<string, object> settings)
        {
            if (!innerRow.TryGetValue(KrConstants.KrStages.StageTypeID, out var stageTypeIDObj)
                || !(stageTypeIDObj is Guid stageTypeID))
            {
                return;
            }

            var formatter = this.formatterContainer.ResolveFormatter(stageTypeID);
            if (formatter == null)
            {
                return;
            }

            var info = new Dictionary<string, object>();
            var ctx = new StageTypeFormatterContext(
                this.session,
                info,
                innerCard,
                innerRow,
                settings)
            {
                DisplayTimeLimit = innerRow.TryGet<string>(KrConstants.KrStages.DisplayTimeLimit) ?? string.Empty,
                DisplayParticipants = innerRow.TryGet<string>(KrConstants.KrStages.DisplayParticipants) ?? string.Empty,
                DisplaySettings = innerRow.TryGet<string>(KrConstants.KrStages.DisplaySettings) ?? string.Empty
            };
            formatter.FormatServer(ctx);

            innerRow.SetIfDiffer(KrConstants.KrStages.DisplayTimeLimit, ctx.DisplayTimeLimit);
            innerRow.SetIfDiffer(KrConstants.KrStages.DisplayParticipants, ctx.DisplayParticipants);
            innerRow.SetIfDiffer(KrConstants.KrStages.DisplaySettings, ctx.DisplaySettings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHidden(
            CardRow row) =>
            row.TryGet<bool?>(KrConstants.KrStages.Hidden) == true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanBeHidden(
            CardRow row,
            ICollection<StageTypeDescriptor> descriptors)
        {
            var stageTypeID = row.TryGet<Guid>(KrConstants.KrStages.StageTypeID);
            foreach (var descriptor in descriptors)
            {
                if (descriptor.ID == stageTypeID)
                {
                    return descriptor.CanBeHidden;
                }
            }

            return false;
        }

        /// <summary>
        /// Возвращает значение, показывающее, возможен ли пропуск указанного этапа.
        /// </summary>
        /// <param name="row">Строка (этап) проверяемая на возможность пропуска.</param>
        /// <returns>Значение, показывающее, возможен ли пропуск указанного этапа.</returns>
        public static bool CanBeSkipped(CardRow row)
        {
            return row.Fields.TryGetValue(KrConstants.KrStages.BasedOnStageTemplateID, out var basedOnStageTemplateIDObj)
                && basedOnStageTemplateIDObj != null
                && row.Fields.TryGet<bool>(KrConstants.KrStages.CanBeSkipped);
        }

        /// <summary>
        /// Возвращает значение, показывающее, пропущен ли этап.
        /// </summary>
        /// <param name="row">Строка - этап, для которого выполяется проверка.</param>
        /// <returns>Значение true, если этап пропущен, иначе - false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSkip(
            CardRow row) =>
            row.TryGet<bool>(KrConstants.KrStages.Skip) == true;

        private async ValueTask ParseSettingsAsync(
            ListStorage<CardRow> srcRows,
            StringDictionaryStorage<CardSection> destSections,
            List<object> stagePositions,
            IDictionary<Guid, IDictionary<string, object>> predeserializedSettings,
            KrProcessSerializerHiddenStageMode hiddenStageMode,
            bool considerHiddenStages,
            CancellationToken cancellationToken = default)
        {
            var descriptors = this.processContainer.GetHandlerDescriptors();
            bool saveRows = hiddenStageMode == KrProcessSerializerHiddenStageMode.ConsiderWithStoringCardRows;
            var showOnlySkipStages = hiddenStageMode == KrProcessSerializerHiddenStageMode.ConsiderOnlySkippedStages;
            var destStageRows = destSections.GetOrAddTable(KrConstants.KrStages.Virtual).Rows;
            var hiddenRows = new List<TemporaryHiddenRowInfo>(srcRows.Count);
            CardRow lastVisibleRow = null;
            var shiftedOrder = 0;

            var nodes = CreateHierarchicalListOfNodes(srcRows);
            var nodeStack = new Stack<StageNode>(srcRows.Count);
            // Стек необходимо заполнять с конца массива, чтобы выстроить в правильном порядке
            foreach (var node in nodes.OrderBy(p => (int)p.Row[KrConstants.Order]).Reverse())
            {
                nodeStack.Push(node);
            }

            // "Ступеньки" вложенности этапов
            // Верхний с индексом 0 - всегда -1, остальные это текущие этапы на определенном уровне
            var stairs = new int[srcRows.Count];
            while (nodeStack.Count != 0)
            {
                var node = nodeStack.Pop();
                stairs[node.Level] = node.Row.TryGet<int?>(KrConstants.KrStages.NestedOrder) ?? -1;
                if (node.Children.Count != 0)
                {
                    // Стек необходимо заполнять с конца массива, чтобы выстроить в правильном порядке
                    for (var i = node.Children.Count - 1; i >= 0; i--)
                    {
                        var child = node.Children[i];
                        child.Level = node.Level + 1;
                        nodeStack.Push(child);
                    }
                }

                var row = node.Row;
                var absoluteOrder = row[KrConstants.Order] as int? ?? 0;
                if (considerHiddenStages)
                {
                    var isSkip = IsSkip(row) && CanBeSkipped(row);
                    var isHidden = IsHidden(row) && CanBeHidden(row, descriptors);
                    var isHide = showOnlySkipStages
                        ? !isSkip && isHidden
                        : isHidden || isSkip;
                    if (isHide)
                    {
                        AddHiddenStagePositionInfo(row, absoluteOrder);
                        continue;
                    }

                    AddHiddenRows(lastVisibleRow, row);
                    lastVisibleRow = row;
                }

                var stageStorage = await DeserializeSettingsRowLocalAsync(row, cancellationToken);
                var rowCopy = row.Clone();
                AppendLevelLabel(node, rowCopy, stairs);
                SetTreeStructureMark(node, rowCopy);
                rowCopy.Remove(KrConstants.Info);
                rowCopy.Remove(KrConstants.KrStages.Settings);
                foreach (var settingsFieldName in this.SettingsFieldNames)
                {
                    if (stageStorage.TryGetValue(settingsFieldName, out var fieldValue))
                    {
                        rowCopy[settingsFieldName] = fieldValue;
                    }
                }

                // Ордер все равно нужно менять везде, т.к. нестедам нужно переприсвоить нормальный ордер
                if (considerHiddenStages)
                {
                    AddVisibleStagePositionInfo(row, absoluteOrder, shiftedOrder);
                }

                rowCopy[KrConstants.Order] = shiftedOrder++;

                destStageRows.Add(rowCopy);

                foreach (var settingsSectionName in this.SettingsSectionNames)
                {
                    if (!stageStorage.TryGetValue(settingsSectionName, out var srcSec)
                        || !(srcSec is IList<object> sRows))
                    {
                        continue;
                    }

                    var destSec = destSections.GetOrAddTable(settingsSectionName);

                    foreach (var srcRowObj in sRows)
                    {
                        if (srcRowObj is IDictionary<string, object> srcRow)
                        {
                            var destRow = destSec.Rows.Add();
                            StorageHelper.Merge(srcRow, destRow.GetStorage());
                        }
                    }
                }
            }

            AddHiddenRows(lastVisibleRow, null);

            ValueTask<IDictionary<string, object>> DeserializeSettingsRowLocalAsync(
                CardRow row,
                CancellationToken ct)
            {
                return predeserializedSettings != null && predeserializedSettings.TryGetValue(row.RowID, out var dict)
                    ? new ValueTask<IDictionary<string, object>>(dict)
                    : this.DeserializeSettingsStorageAsync(row, false, ct);
            }

            void AddHiddenRows(CardRow lastVisibleStage, CardRow aboveRow)
            {
                if (hiddenRows.Count == 0)
                {
                    return;
                }

                foreach (var hiddenRow in hiddenRows)
                {
                    stagePositions.Add(KrStagePositionInfo.CreateHidden(
                            hiddenRow.Row,
                            hiddenRow.AbsolutePosition,
                            saveRows,
                            lastVisibleStage,
                            aboveRow)
                        .GetStorage());
                }

                hiddenRows.Clear();
            }

            void AddVisibleStagePositionInfo(
                CardRow row,
                int absolutePosition,
                int? shiftedPosition)
            {
                stagePositions.Add(
                    KrStagePositionInfo.CreateVisible(row, absolutePosition, shiftedPosition).GetStorage());
            }

            void AddHiddenStagePositionInfo(
                CardRow row,
                int absolutePosition)
            {
                hiddenRows.Add(
                    new TemporaryHiddenRowInfo
                    {
                        Row = row,
                        AbsolutePosition = absolutePosition,
                    });
            }
        }

        #endregion
    }
}