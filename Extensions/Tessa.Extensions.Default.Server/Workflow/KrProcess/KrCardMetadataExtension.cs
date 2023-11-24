using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Metadata;
using Tessa.Cards.Validation;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public sealed class KrCardMetadataExtension : KrCardMetadataExtensionBase
    {
        #region nested types

        /// <summary>
        /// Единичная связь между существующим и сгенеренным элементом метаданных
        /// Может хранить связь
        /// ID существующей секции -- ID новой секции
        /// ID существующей колонки -- ID новой колонки
        /// </summary>
        private sealed class CardMetadataLink
        {
            public CardMetadataLink(Guid originalID, Guid newID)
            {
                this.OriginalID = originalID;
                this.NewID = newID;
            }

            public Guid OriginalID { get; }
            public Guid NewID { get; }
        }

        /// <summary>
        /// Связь между существующей и сгенеренной секцией метаданных
        /// с учетом маппиннга полей.
        /// </summary>
        private sealed class CardMetadataContainerLink : IReadOnlyCollection<CardMetadataContainerLink>
        {
            private readonly HashSet<Guid, CardMetadataContainerLink> forwardMapping =
                new HashSet<Guid, CardMetadataContainerLink>(x => x.link.OriginalID);

            private readonly CardMetadataLink link;

            public CardMetadataContainerLink(Guid originContainerID, Guid newContainerID)
            {
                this.link = new CardMetadataLink(originContainerID, newContainerID);
            }

            public Guid OriginalID => this.link.OriginalID;
            public Guid NewID => this.link.NewID;

            public int Count => this.forwardMapping.Count;

            public CardMetadataContainerLink GetLink(Guid originalID) =>
                this.forwardMapping[originalID];

            public bool TryGetLink(Guid originalID, out CardMetadataContainerLink l) =>
                this.forwardMapping.TryGetItem(originalID, out l);

            public bool ContainsLink(Guid originalID) =>
                this.forwardMapping.ContainsKey(originalID);

            public CardMetadataContainerLink AddLink(Guid originalColumnID, Guid newColumnID)
            {
                var l = new CardMetadataContainerLink(originalColumnID, newColumnID);
                this.forwardMapping.Replace(l);
                return l;
            }

            /// <inheritdoc />
            public IEnumerator<CardMetadataContainerLink> GetEnumerator() =>
                this.forwardMapping.GetEnumerator();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() =>
                this.GetEnumerator();
        }

        private sealed class ReplaceFieldControlVisitor : TagParserCardTypeVisitor
        {
            private readonly CardMetadataContainerLink mapping;
            private readonly HashSet<Guid> mergedIntoKrStages;
            private readonly ICardMetadata metadata;
            private readonly KrStageSerializerData data;

            /// <inheritdoc />
            public ReplaceFieldControlVisitor(
                IValidationResultBuilder validationResult,
                CardMetadataContainerLink mapping,
                HashSet<Guid> mergedIntoKrStages,
                ICardMetadata metadata,
                KrStageSerializerData data)
                : base(validationResult)
            {
                this.mapping = mapping;
                this.mergedIntoKrStages = mergedIntoKrStages;
                this.metadata = metadata;
                this.data = data;
            }

            public override async ValueTask VisitControlAsync(
                CardTypeControl control,
                CardTypeBlock block,
                CardTypeForm form,
                CardType type,
                CancellationToken cancellationToken = default)
            {
                await base.VisitControlAsync(control, block, form, type, cancellationToken);
                this.ReplaceMappingIfNeeded(control);

                CardMetadataContainerLink link;
                switch (control)
                {
                    case CardTypeEntryControl entryControl:
                        link = this.mapping.GetLink(entryControl.SectionID);
                        this.MoveEntryControl(link, entryControl);
                        this.MoveEntryControlSettings(link, entryControl);
                        break;
                    case CardTypeTableControl tableControl:
                        link = this.mapping.GetLink(tableControl.SectionID);
                        this.MoveTableControl(link, tableControl);
                        await this.MoveTableSettingsAsync(link, tableControl, cancellationToken);
                        break;
                    case CardTypeTabControl _:
                        // В табах нечего исправлять
                        break;
                    case CardTypeCustomControl customControl:
                        if (control.Type == CardControlTypes.Numerator)
                        {
                            this.ReplaceNumeratorSettings(customControl);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(control)} type {control.GetType().Name} is unknown.");
                }
            }

            private void MoveEntryControl(CardMetadataContainerLink link, CardTypeEntryControl entryControl)
            {
                entryControl.SectionID = this.mergedIntoKrStages.Contains(link.NewID)
                    ? DefaultSchemeHelper.KrStagesVirtual
                    : link.NewID;
                if (entryControl.ComplexColumnID.HasValue)
                {
                    entryControl.ComplexColumnID = link.GetLink(entryControl.ComplexColumnID.Value).NewID;
                }

                entryControl.PhysicalColumnIDList = entryControl
                    .PhysicalColumnIDList
                    .Select(p => link.GetLink(p).NewID)
                    .ToSealableList();
            }

            private void MoveEntryControlSettings(
                CardMetadataContainerLink link,
                CardTypeEntryControl entryControl)
            {
                var manualInputColumnID =
                    entryControl.ControlSettings.TryGet<Guid?>(CardControlSettings.ManualInputColumnIDSetting);
                if (manualInputColumnID.HasValue)
                {
                    var manualInputLink = link.GetLink(manualInputColumnID.Value);
                    entryControl.ControlSettings[CardControlSettings.ManualInputColumnIDSetting] =
                        manualInputLink.NewID;
                }
            }

            private void MoveTableControl(CardMetadataContainerLink link, CardTypeTableControl tableControl)
            {
                tableControl.SectionID = link.NewID;
                foreach (var column in tableControl.Columns)
                {
                    if (column.ComplexColumnID.HasValue)
                    {
                        column.ComplexColumnID = link.GetLink(column.ComplexColumnID.Value).NewID;
                    }

                    if (column.OwnedSectionID.HasValue)
                    {
                        var ownedSectionLink = this.mapping.GetLink(column.OwnedSectionID.Value);
                        column.OwnedSectionID = ownedSectionLink.NewID;
                        if (column.OwnedComplexColumnID.HasValue)
                        {
                            column.OwnedComplexColumnID =
                                ownedSectionLink.GetLink(column.OwnedComplexColumnID.Value).NewID;
                        }

                        if (column.OwnedOrderColumnID.HasValue)
                        {
                            column.OwnedOrderColumnID =
                                ownedSectionLink.GetLink(column.OwnedOrderColumnID.Value).NewID;
                        }

                        column.OwnedPhysicalColumnIDList = column
                            .OwnedPhysicalColumnIDList
                            .Select(p => ownedSectionLink.GetLink(p).NewID)
                            .ToSealableList();
                    }

                    column.PhysicalColumnIDList = column
                        .PhysicalColumnIDList
                        .Select(p => link.GetLink(p).NewID)
                        .ToSealableList();
                }
            }

            private async ValueTask MoveTableSettingsAsync(
                CardMetadataContainerLink link,
                CardTypeTableControl tableControl,
                CancellationToken cancellationToken = default)
            {
                var orderID =
                    tableControl.ControlSettings.TryGet<Guid?>(CardControlSettings.OrderColumnIDSetting);
                if (orderID.HasValue)
                {
                    var orderLink = link.GetLink(orderID.Value);
                    tableControl.ControlSettings[CardControlSettings.OrderColumnIDSetting] =
                        orderLink.NewID;
                    // Необходимо запомнить ордер, чтобы затем восстанавливать его.
                    var section = (await this.metadata.GetSectionsAsync(cancellationToken))[link.NewID];
                    var sectionName = section.Name;
                    var orderFieldName = section.Columns[orderLink.NewID].Name;
                    this.data.OrderColumns.Add(new OrderColumn(sectionName, orderFieldName));
                }

                var manualInputColumnID =
                    tableControl.ControlSettings.TryGet<Guid?>(CardControlSettings.ManualInputColumnIDSetting);
                if (manualInputColumnID.HasValue)
                {
                    var manualInputLink = link.GetLink(manualInputColumnID.Value);
                    tableControl.ControlSettings[CardControlSettings.ManualInputColumnIDSetting] =
                        manualInputLink.NewID;
                }
            }

            private void ReplaceNumeratorSettings(CardTypeCustomControl numeratorControl)
            {
                var ctrlSettings = numeratorControl.ControlSettings;
                if (!ctrlSettings.TryGetValue(CardControlSettings.SectionIDSetting, out var secIDObj)
                    || !(secIDObj is Guid sectionID))
                {
                    return;
                }

                var link = this.mapping.GetLink(sectionID);
                ctrlSettings[CardControlSettings.SectionIDSetting] = this.mergedIntoKrStages.Contains(link.NewID)
                    ? DefaultSchemeHelper.KrStagesVirtual
                    : link.NewID;

                if (ctrlSettings.TryGetValue(CardControlSettings.SequenceColumnIDSetting, out var seqIDObj)
                    && seqIDObj is Guid sequenceColumnID)
                {
                    var seqLink = link.GetLink(sequenceColumnID);
                    ctrlSettings[CardControlSettings.SequenceColumnIDSetting] = seqLink.NewID;
                }

                if (ctrlSettings.TryGetValue(CardControlSettings.NumberColumnIDSetting, out var numberColIDObj)
                    && numberColIDObj is Guid numberColumnID)
                {
                    var numberLink = link.GetLink(numberColumnID);
                    ctrlSettings[CardControlSettings.NumberColumnIDSetting] = numberLink.NewID;
                }

                if (ctrlSettings.TryGetValue(CardControlSettings.FullNumberColumnIDSetting, out var fullNumberColIDObj)
                    && fullNumberColIDObj is Guid fullNumberColumnID)
                {
                    var fullNumberLink = link.GetLink(fullNumberColumnID);
                    ctrlSettings[CardControlSettings.FullNumberColumnIDSetting] = fullNumberLink.NewID;
                }
            }

            private void ReplaceMappingIfNeeded(CardTypeControl control)
            {
                var mapSettingsList = control.ControlSettings.TryGet<List<object>>(CardControlSettings.ViewMapSetting);
                if (mapSettingsList == null)
                {
                    return;
                }

                foreach (var mappingSettings in mapSettingsList.Cast<Dictionary<string, object>>())
                {
                    var mappingColumnType = mappingSettings.Get<int?>(CardControlSettings.MappingColumnTypeSetting);
                    switch (mappingColumnType)
                    {
                        case (int?) ViewMapColumnType.CardColumn:
                            var mappingCardSection = mappingSettings.Get<Guid?>(CardControlSettings.MappingCardSectionSetting);
                            var mappingCardColumn = mappingSettings.Get<Guid?>(CardControlSettings.MappingCardColumnSetting);
                            if (mappingCardSection.HasValue
                                && mappingCardColumn.HasValue)
                            {
                                var link = this.mapping.GetLink(mappingCardSection.Value);
                                var columnLink = link.GetLink(mappingCardColumn.Value);
                                mappingSettings[CardControlSettings.MappingCardSectionSetting] =
                                    this.mergedIntoKrStages.Contains(link.NewID)
                                        ? DefaultSchemeHelper.KrStagesVirtual
                                        : link.NewID;
                                mappingSettings[CardControlSettings.MappingCardColumnSetting] = columnLink.NewID;
                            }

                            break;
                        case (int?) ViewMapColumnType.CardID:
                        case (int?) ViewMapColumnType.CardType:
                        case (int?) ViewMapColumnType.CardTypeAlias:
                        case (int?) ViewMapColumnType.CurrentUser:
                        case (int?) ViewMapColumnType.ConstantValue:
                            // В данных случаях ничего не меняем, т.к. не затронуты секции.
                            break;
                        case null:
                            // Контрол не хочет, чтобы его маппили.
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Unknown ViewMapColumnType.{mappingColumnType.ToString()}");
                    }
                }
            }
        }

        private sealed class VisibilityContext
        {
            public VisibilityContext(
                int cnt)
            {
                this.UseDefaultTimeLimit = new List<Guid>(cnt);
                this.UseDefaultPlanned = new List<Guid>(cnt);
                this.CanBeHidden = new List<Guid>(cnt);
                this.SinglePerformers = new List<Guid>(cnt);
                this.MultiplePerformers = new List<Guid>(cnt);
                this.PerformerCaptions = new Dictionary<string, object>();
                this.CheckPerformers = new List<Guid>(cnt);
                this.CanOverrideAuthor = new List<Guid>(cnt);
                this.CanOverrideTaskHistoryGroup = new List<Guid>(cnt);
                this.UseTaskKind = new List<Guid>(cnt);
                this.CanBeSkipped = new List<Guid>(cnt);
            }

            public List<Guid> UseDefaultTimeLimit { get; }
            public List<Guid> UseDefaultPlanned { get; }
            public List<Guid> CanBeHidden { get; }
            public List<Guid> SinglePerformers { get; }
            public List<Guid> MultiplePerformers { get; }
            public Dictionary<string, object> PerformerCaptions { get; }
            public List<Guid> CheckPerformers { get; }
            public List<Guid> CanOverrideAuthor { get; }
            public List<Guid> CanOverrideTaskHistoryGroup { get; }

            public List<Guid> UseTaskKind { get; }

            public List<Guid> CanBeSkipped { get; }

            public void Init(ICollection<StageTypeDescriptor> handlerDescriptors)
            {
                foreach (var descriptor in handlerDescriptors)
                {
                    if (descriptor.UseTimeLimit)
                    {
                        this.UseDefaultTimeLimit.Add(descriptor.ID);
                    }

                    if (descriptor.UsePlanned)
                    {
                        this.UseDefaultPlanned.Add(descriptor.ID);
                    }

                    if (descriptor.CanBeHidden)
                    {
                        this.CanBeHidden.Add(descriptor.ID);
                    }

                    switch (descriptor.PerformerUsageMode)
                    {
                        case PerformerUsageMode.Single:
                            this.SinglePerformers.Add(descriptor.ID);
                            break;
                        case PerformerUsageMode.Multiple:
                            this.MultiplePerformers.Add(descriptor.ID);
                            break;
                    }

                    if (descriptor.PerformerUsageMode != PerformerUsageMode.None
                        && !string.IsNullOrWhiteSpace(descriptor.PerformerCaption))
                    {
                        this.PerformerCaptions.Add(descriptor.ID.ToString("D"), descriptor.PerformerCaption);
                    }

                    if (descriptor.PerformerUsageMode != PerformerUsageMode.None
                        && descriptor.PerformerIsRequired)
                    {
                        this.CheckPerformers.Add(descriptor.ID);
                    }

                    if (descriptor.CanOverrideAuthor)
                    {
                        this.CanOverrideAuthor.Add(descriptor.ID);
                    }

                    if (descriptor.CanOverrideTaskHistoryGroup)
                    {
                        this.CanOverrideTaskHistoryGroup.Add(descriptor.ID);
                    }

                    if (descriptor.UseTaskKind)
                    {
                        this.UseTaskKind.Add(descriptor.ID);
                    }

                    if (descriptor.CanBeSkipped)
                    {
                        this.CanBeSkipped.Add(descriptor.ID);
                    }
                }
            }
        }

        private struct InjectCardTypeInfo
        {
            public CardType CardType { get; set; }

            public Action<CardType, CardType, VisibilityContext> ModificationAction { get; set; }
        }

        #endregion

        #region fields

        // Расширение регистрируется как PerResolve,
        // поэтому данные от одного выполнения хранятся в полях.

        private readonly IDbScope dbScope;

        private readonly IKrProcessContainer processContainer;

        private readonly IKrStageSerializer serializer;

        /// <summary>
        /// Связь ID существующих секций и добавленных в KrCard
        /// </summary>
        private readonly CardMetadataContainerLink sectionMapping =
            new CardMetadataContainerLink(Guid.Empty, Guid.Empty);

        /// <summary>
        /// ID секций, попавших в sectionMapping как NewID, но влитые в KrStagesVirtual
        /// </summary>
        private readonly HashSet<Guid> mergedIntoKrStages =
            new HashSet<Guid>();

        /// <summary>
        /// Новые имена для сгенеренных секций
        /// </summary>
        private readonly Dictionary<Guid, string> newSectionNames =
            new Dictionary<Guid, string>();

        /// <summary>
        /// Класс для обхода UI типов и подмены секций
        /// </summary>
        private ReplaceFieldControlVisitor visitor;

        private Dictionary<Guid, StageTypeDescriptor> stageSettingsWithDescriptors;

        private ICardMetadata metadata;
        private CardMetadataSection krStageSection;
        private short krStageComplexColumnIndex;

        private IList<CardType> krTypes;
        private IList<CardTypeSchemeItem> krStagesVirtualSchemeItem;

        private readonly KrStageSerializerData stageSerializerData = new KrStageSerializerData();

        #endregion

        #region constructors

        /// <inheritdoc />
        public KrCardMetadataExtension(
            ICardMetadata clientCardMetadata)
            : base(clientCardMetadata)
        {
            throw new InvalidOperationException("This constructor only for client-side.");
        }

        /// <inheritdoc />
        public KrCardMetadataExtension(
            IDbScope dbScope,
            IKrProcessContainer processContainer,
            IKrStageSerializer serializer)
            : base()
        {
            this.dbScope = dbScope;
            this.processContainer = processContainer;
            this.serializer = serializer;
        }

        #endregion

        #region base override

        /// <inheritdoc />
        protected override async Task ExtendKrTypesAsync(
            IList<CardType> types,
            ICardMetadataExtensionContext context)
        {
            this.visitor = new ReplaceFieldControlVisitor(
                new ValidationResultBuilder(),
                this.sectionMapping,
                this.mergedIntoKrStages,
                context.CardMetadata,
                this.stageSerializerData);
            this.krTypes = types;
            this.krStagesVirtualSchemeItem = new List<CardTypeSchemeItem>(this.krTypes.Count);
            foreach (var krType in this.krTypes)
            {
                this.krStagesVirtualSchemeItem.Add(krType.SchemeItems.First(p => p.SectionID == DefaultSchemeHelper.KrStagesVirtual));
            }

            this.metadata = context.CardMetadata;
            var metadataSections = await this.metadata.GetSectionsAsync(context.CancellationToken);
            this.krStageSection = metadataSections[DefaultSchemeHelper.KrStagesVirtual];
            this.krStageComplexColumnIndex = (short) Math.Min(
                this.krStageSection.Columns.Max(p => p.ComplexColumnIndex) + 1,
                short.MaxValue);

            this.stageSettingsWithDescriptors = new Dictionary<Guid, StageTypeDescriptor>();

            foreach (var p in this.processContainer.GetHandlerDescriptors())
            {
                if (p.SettingsCardTypeID.HasValue)
                {
                    this.stageSettingsWithDescriptors[p.SettingsCardTypeID.Value] = p;
                }
            }

            var cardTypes = context
                .CardTypes
                .Where(p => this.stageSettingsWithDescriptors.ContainsKey(p.ID))
                .Select(p => p.DeepClone())
                .ToList();
            this.BuildMapping(cardTypes);

            var injectableInfo = this.GetInjectableCards(context);

            // Прегенерация имен для корректного восстановления связей
            // А также можно заранее сохранить секции, чтобы не лезть дважды в хэш таблицы.
            var sections = new CardMetadataSection[this.sectionMapping.Count];
            var sectionsIndex = 0;
            foreach (var link in this.sectionMapping)
            {
                var section = metadataSections[link.OriginalID];
                sections[sectionsIndex++] = section;
                this.newSectionNames.Add(
                    link.NewID,
                    section.SectionType == CardSectionType.Entry
                        ? KrConstants.KrStages.Virtual
                        : NewSectionName(section));
            }

            sectionsIndex = 0;
            foreach (var link in this.sectionMapping)
            {
                var section = sections[sectionsIndex++];
                switch (section.SectionType)
                {
                    case CardSectionType.Entry:
                        this.ExtendKrStagesVirtual(link, section);
                        break;
                    case CardSectionType.Table:
                        await this.CreateCollectionAsync(link, section, context.CancellationToken);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            this.stageSerializerData.SettingsSectionNames.Add(this.krStageSection.Name);
            foreach (var settingsCardType in cardTypes)
            {
                await settingsCardType.VisitAsync(this.visitor, context.CancellationToken);
                foreach (var krType in this.krTypes)
                {
                    var copy = settingsCardType.DeepClone();
                    this.MoveForms(krType, copy);
                    this.MoveValidators(krType, copy);
                    this.MoveExtensions(krType, copy);
                }
            }

            foreach (var info in injectableInfo)
            {
                await info.CardType.VisitAsync(this.visitor, context.CancellationToken);
            }

            this.SetOptionalControlsVisibility(injectableInfo);
            this.serializer.SetData(this.stageSerializerData);
        }

        /// <inheritdoc />
        protected override async Task<List<Guid>> GetCardTypeIDsAsync(CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                return await db
                    .SetCommand(
                        this.dbScope.BuilderFactory
                            .Select()
                            .C(KrConstants.KrSettingsCardTypes.CardTypeID)
                            .From(KrConstants.KrSettingsCardTypes.Name).NoLock()
                            .Where().C(KrConstants.KrSettingsCardTypes.CardTypeID).NotEquals().P("krCardTypeID")
                            .Build(),
                        db.Parameter("krCardTypeID", DefaultCardTypes.KrCardTypeID))
                    .LogCommand()
                    .ExecuteListAsync<Guid>(cancellationToken);
            }
        }

        protected override ValueTask<CardMetadataSectionCollection> GetAllSectionsAsync(ICardMetadataExtensionContext context) =>
            context.CardMetadata.GetSectionsAsync(context.CancellationToken);

        #endregion

        #region private

        private static CardTypeBlock GetBlock(CardType cardType, string blockName)
        {
            try
            {
                var tableControl = (CardTypeTableControl) cardType
                    .Forms
                    .First(p => p.Name == KrConstants.Ui.KrApprovalProcessFormAlias)
                    .Blocks
                    .First(p => p.Name == KrConstants.Ui.KrApprovalStagesBlockAlias)
                    .Controls
                    .First(p => p.Name == KrConstants.Ui.KrApprovalStagesControlAlias);

                return tableControl
                    .Form
                    .Blocks
                    .FirstOrDefault(p => p.Name == blockName);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private void BuildMapping(IEnumerable<CardType> cardTypes)
        {
            foreach (var cardType in cardTypes)
            {
                foreach (var schemeItem in cardType.SchemeItems)
                {
                    if (!this.sectionMapping.TryGetLink(schemeItem.SectionID, out var link))
                    {
                        link = this.sectionMapping.AddLink(schemeItem.SectionID, Guid.NewGuid());
                    }

                    foreach (var columnID in schemeItem.ColumnIDList)
                    {
                        if (!link.ContainsLink(columnID))
                        {
                            link.AddLink(columnID, Guid.NewGuid());
                        }
                    }
                }
            }
        }

        private void ExtendKrStagesVirtual(
            CardMetadataContainerLink sectionLink,
            CardMetadataSection originalSection)
        {
            this.mergedIntoKrStages.Add(sectionLink.NewID);
            this.MoveColumns(sectionLink,
                originalSection,
                this.krStageSection,
                this.krStagesVirtualSchemeItem,
                /*addToSerializer: */true,
                ref this.krStageComplexColumnIndex,
                out var _);
        }

        private async Task CreateCollectionAsync(
            CardMetadataContainerLink sectionLink,
            CardMetadataSection originalSection,
            CancellationToken cancellationToken = default)
        {
            var newSchemeItems = new List<CardTypeSchemeItem>(this.krTypes.Count);
            for (var i = 0; i < this.krTypes.Count; i++)
            {
                newSchemeItems.Add(new CardTypeSchemeItem { SectionID = sectionLink.NewID });
            }

            var newSection = new CardMetadataSection
            {
                ID = sectionLink.NewID,
                Name = this.newSectionNames[sectionLink.NewID],
                SectionType = CardSectionType.Table,
                TableType = SchemeTableContentType.Collections,
                SectionTableType = CardTableType.Collection,
                Order = originalSection.Order,
                Flags = originalSection.Flags | CardMetadataSectionFlags.Virtual,
            };

            short complexColumnIndex = 0;
            this.MoveColumns(
                sectionLink,
                originalSection,
                newSection,
                newSchemeItems,
                /*addToSerializer: */false,
                ref complexColumnIndex,
                out var hasReferenceToOwner);
            if (!hasReferenceToOwner)
            {
                this.AddReferenceToOwner(newSection, newSchemeItems, ref complexColumnIndex);
            }

            this.stageSerializerData.SettingsSectionNames.Add(newSection.Name);
            (await this.metadata.GetSectionsAsync(cancellationToken)).Add(newSection);
            for (var i = 0; i < this.krTypes.Count; i++)
            {
                this.krTypes[i].SchemeItems.Add(newSchemeItems[i]);
            }
        }

        private CardMetadataSectionReference CopyMetadataReference(CardMetadataSectionReference referencedSection)
        {
            var reference = new CardMetadataSectionReference
            {
                ID = referencedSection.ID,
                Name = referencedSection.Name,
            };
            if (this.sectionMapping.TryGetLink(referencedSection.ID, out var link))
            {
                reference.ID = link.NewID;
                if (this.newSectionNames.TryGetValue(link.NewID, out var newName))
                {
                    reference.Name = newName;
                }
            }

            return reference;
        }

        private void MoveColumns(
            CardMetadataContainerLink sectionLink,
            CardMetadataSection originalSection,
            CardMetadataSection newSection,
            IList<CardTypeSchemeItem> newSchemeItems,
            bool addToSerializer,
            ref short complexColumnIndex,
            out bool hasReferenceToOwner)
        {
            void AddToSerializer(CardMetadataColumn newColumn)
            {
                if (addToSerializer)
                {
                    this.stageSerializerData.SettingsFieldNames.Add(newColumn.Name);
                }
            }

            hasReferenceToOwner = false;
            for (var columnIndex = 0; columnIndex < originalSection.Columns.Count; columnIndex++)
            {
                var column = originalSection.Columns[columnIndex];
                var newColumn = this.CreateColumn(sectionLink, originalSection, column, newSection);
                newSection.Columns.Add(newColumn);

                if (column.ColumnType == CardMetadataColumnType.Complex)
                {
                    foreach (var newSchemeItem in newSchemeItems)
                    {
                        newSchemeItem.ColumnIDList.Add(newColumn.ID);
                    }

                    newColumn.ComplexColumnIndex = complexColumnIndex;
                    CardMetadataColumn physColumn;
                    while (columnIndex + 1 < originalSection.Columns.Count
                        && (physColumn = originalSection.Columns[columnIndex + 1]).ComplexColumnIndex == column.ComplexColumnIndex)
                    {
                        columnIndex++;
                        sectionLink.AddLink(physColumn.ID, Guid.NewGuid());
                        var newPhysColumn = this.CreateColumn(
                            sectionLink,
                            originalSection,
                            physColumn,
                            newSection,
                            complexColumnIndex);
                        newSection.Columns.Add(newPhysColumn);
                        AddToSerializer(newPhysColumn);
                    }

                    complexColumnIndex++;
                }
                else
                {
                    AddToSerializer(newColumn);

                    foreach (var newSchemeItem in newSchemeItems)
                    {
                        newSchemeItem.ColumnIDList.Add(newColumn.ID);
                    }
                }

                if (newColumn.ParentRowSection != null
                    && !hasReferenceToOwner)
                {
                    hasReferenceToOwner = true;
                }
            }
        }

        private CardMetadataColumn CreateColumn(
            CardMetadataContainerLink sectionLink,
            CardMetadataSection originalSection,
            CardMetadataColumn originalColumn,
            CardMetadataSection newSection,
            short complexColumnIndex = -1)
        {
            var newColumn = new CardMetadataColumn
            {
                ID = sectionLink.GetLink(originalColumn.ID).NewID,
                Name = NewColumnName(originalSection, originalColumn, newSection),
                CardTypeIDList = this.krTypes.Select(p => p.ID).ToSealableList(),
                ColumnType = originalColumn.ColumnType,
                DefaultValidValue = originalColumn.DefaultValidValue,
                DefaultValue = originalColumn.DefaultValue,
                IsReference = originalColumn.IsReference,
                MetadataType = originalColumn.MetadataType,
                ComplexColumnIndex = complexColumnIndex,
            };
            if (originalColumn.ReferencedSection != null)
            {
                newColumn.ReferencedSection = this.CopyMetadataReference(originalColumn.ReferencedSection);
            }

            if (originalColumn.ParentRowSection != null)
            {
                newColumn.ParentRowSection = this.CopyMetadataReference(originalColumn.ParentRowSection);
                if (originalColumn.ParentRowSection.ID == DefaultSchemeHelper.KrStagesVirtual
                    && originalColumn.ColumnType == CardMetadataColumnType.Physical)
                {
                    this.stageSerializerData.ReferencesToStages.Add(new ReferenceToStage(newSection.Name, newColumn.Name));
                }
            }

            return newColumn;
        }

        private void AddReferenceToOwner(
            CardMetadataSection newSection,
            IList<CardTypeSchemeItem> newSchemeItems,
            ref short maxComplexColumnIndex)
        {
            var stageColumn = new CardMetadataColumn
            {
                ID = Guid.NewGuid(),
                Name = KrConstants.StageReferenceToOwner,
                CardTypeIDList = this.krTypes.Select(p => p.ID).ToSealableList(),
                ColumnType = CardMetadataColumnType.Complex,
                ComplexColumnIndex = maxComplexColumnIndex,
                DefaultValidValue = null,
                DefaultValue = null,
                IsReference = false,
                MetadataType = new CardMetadataType(SchemeType.ReferenceTypified),
                ReferencedSection = new CardMetadataSectionReference
                {
                    ID = this.krStageSection.ID,
                    Name = this.krStageSection.Name,
                },
                ParentRowSection = new CardMetadataSectionReference
                {
                    ID = this.krStageSection.ID,
                    Name = this.krStageSection.Name,
                },
            };
            newSection.Columns.Add(stageColumn);
            foreach (var newSchemeItem in newSchemeItems)
            {
                newSchemeItem.ColumnIDList.Add(stageColumn.ID);
            }

            var stageRowIDColumn = new CardMetadataColumn
            {
                ID = Guid.NewGuid(),
                Name = KrConstants.StageRowIDReferenceToOwner,
                CardTypeIDList = this.krTypes.Select(p => p.ID).ToSealableList(),
                ColumnType = CardMetadataColumnType.Physical,
                ComplexColumnIndex = maxComplexColumnIndex,
                DefaultValidValue = Guid.Empty,
                DefaultValue = null,
                IsReference = true,
                MetadataType = CardMetadataTypes.Guid,
                ReferencedSection = null,
                ParentRowSection = new CardMetadataSectionReference
                {
                    ID = this.krStageSection.ID,
                    Name = this.krStageSection.Name,
                },
            };
            newSection.Columns.Add(stageRowIDColumn);

            this.stageSerializerData.ReferencesToStages.Add(
                new ReferenceToStage(newSection.Name, stageRowIDColumn.Name));
        }

        private void MoveForms(CardType krType, CardType settingsCardType)
        {
            var settingsBlock = GetBlock(krType, KrConstants.Ui.KrStageSettingsBlockAlias);
            if (settingsBlock == null)
            {
                return;
            }

            var tabControl = new CardTypeTabControl();
            settingsBlock.Controls.Add(tabControl);
            tabControl.ControlSettings[KrConstants.Ui.StageHandlerDescriptorIDSetting] =
                this.stageSettingsWithDescriptors[settingsCardType.ID].ID;
            tabControl.Type = settingsCardType.Forms.Count != 0
                ? CardControlTypes.TabControl
                : CardControlTypes.Container;
            var tab = new CardTypeTabControlForm();
            tab.TabCaption = settingsCardType.Caption;
            tab.Blocks.AddRange(settingsCardType.Blocks);
            tabControl.Forms.Add(tab);

            foreach (var form in settingsCardType.Forms)
            {
                tab = new CardTypeTabControlForm();
                tab.TabCaption = form.TabCaption;
                tab.Blocks.AddRange(form.Blocks);
                tabControl.Forms.Add(tab);
            }
        }

        private void MoveValidators(CardType krType, CardType settingsCardType)
        {
            foreach (var validatorOrig in settingsCardType.Validators)
            {
                var validator = validatorOrig.DeepClone();
                var sectionLink = this.MoveIfPresent(
                    validator.ValidatorSettings, this.sectionMapping, CardValidatorSettings.SectionIDSetting);
                this.MoveIfPresent(validator.ValidatorSettings, sectionLink, CardValidatorSettings.ColumnIDSetting);
                this.MoveIfPresent(validator.ValidatorSettings, sectionLink, CardValidatorSettings.OrderColumnIDSetting);
                var parentLink = this.MoveIfPresent(
                    validator.ValidatorSettings, this.sectionMapping, CardValidatorSettings.ParentSectionIDSetting);
                this.MoveIfPresent(validator.ValidatorSettings, parentLink, CardValidatorSettings.ParentColumnIDSetting);

                // Проверка для обнаружения неизвестных типов валидаторов
                // Чтобы ошибка вскрылась раньше.
                if (validator.Type != CardValidatorTypes.NotNullField
                    && validator.Type != CardValidatorTypes.NotNullTable
                    && validator.Type != CardValidatorTypes.Unique)
                {
                    throw new ArgumentException($"Unknown validator type {validator.Type}");
                }

                krType.Validators.Add(validator);
            }
        }

        private void MoveExtensions(CardType krType, CardType settingsCardType)
        {
            foreach (var extensionOrig in settingsCardType.Extensions)
            {
                var extension = extensionOrig.DeepClone();
                var sectionLink = this.MoveIfPresent(
                    extension.ExtensionSettings, this.sectionMapping, CardTypeExtensionSettings.SectionIDSetting);
                this.MoveIfPresent(extension.ExtensionSettings, sectionLink, CardTypeExtensionSettings.ColumnIDSetting);
                if (extension.Type != CardTypeExtensionTypes.SortRows)
                {
                    throw new ArgumentException($"Unknown extension type {extension.Type}");
                }

                krType.Extensions.Add(extension);
            }
        }

        private static string NewSectionName(CardMetadataSection originalSection) =>
            StageTypeSettingsNaming.SectionName(originalSection.Name);

        private static string NewColumnName(
            CardMetadataSection originalSection,
            CardMetadataColumn originalColumn,
            CardMetadataSection newSection)
        {
            return newSection.ID == DefaultSchemeHelper.KrStagesVirtual
                ? StageTypeSettingsNaming.PlainColumnName(originalSection.Name, originalColumn.Name)
                : originalColumn.Name;
        }

        private CardMetadataContainerLink MoveIfPresent(
            ISerializableObject dict,
            CardMetadataContainerLink link,
            string key)
        {
            if (dict.TryGetValue(key, out var objValue)
                && objValue is Guid value)
            {
                var embeddedLink = link.GetLink(value);
                var newID = this.mergedIntoKrStages.Contains(embeddedLink.NewID)
                    ? DefaultSchemeHelper.KrStagesVirtual
                    : embeddedLink.NewID;
                dict[key] = newID;
                return embeddedLink;
            }

            return null;
        }

        private void SetOptionalControlsVisibility(InjectCardTypeInfo[] injectableCardTypes)
        {
            var handlerDescriptors = this.processContainer.GetHandlerDescriptors();

            var visibilityCtx = new VisibilityContext(handlerDescriptors.Count);
            visibilityCtx.Init(handlerDescriptors);

            foreach (var krType in this.krTypes)
            {
                var commonBlock = GetBlock(krType, KrConstants.Ui.KrStageCommonInfoBlock);
                var timeLimitControl =
                    commonBlock.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.KrTimeLimitInputAlias);
                if (timeLimitControl != null)
                {
                    timeLimitControl.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] = visibilityCtx.UseDefaultTimeLimit;
                }

                var plannedControl =
                    commonBlock.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.KrPlannedInputAlias);
                if (plannedControl != null)
                {
                    plannedControl.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] = visibilityCtx.UseDefaultPlanned;
                }

                var canBeHiddenCheckbox =
                    commonBlock.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.KrHiddenStageCheckboxAlias);
                if (canBeHiddenCheckbox != null)
                {
                    canBeHiddenCheckbox.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] = visibilityCtx.CanBeHidden;
                }

                foreach (var type in injectableCardTypes)
                {
                    type.ModificationAction(krType, type.CardType, visibilityCtx);
                }

                var canBeSkippedCheckbox =
                    commonBlock.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.KrCanBeSkippedCheckboxAlias);
                if (canBeSkippedCheckbox != null)
                {
                    canBeSkippedCheckbox.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] = visibilityCtx.CanBeSkipped;
                }
            }
        }

        private static void SetPerformersVisibility(CardType krType, CardType perfCardType, VisibilityContext vCtx)
        {
            var performersBlock = GetBlock(krType, KrConstants.Ui.KrPerformersBlockAlias);

            // Блок быть должен, как и тип.
            var performersControls = perfCardType.Blocks[0].Controls;
            var singlePerformerControl =
                performersControls.FirstOrDefault(p => p.Name == KrConstants.Ui.KrSinglePerformerEntryAcAlias);
            if (singlePerformerControl != null)
            {
                singlePerformerControl.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] =
                    vCtx.SinglePerformers;
                singlePerformerControl.ControlSettings[KrConstants.Ui.RequiredForTypesSetting] =
                    vCtx.CheckPerformers;
                singlePerformerControl.ControlSettings[KrConstants.Ui.ControlCaptionsSetting] =
                    vCtx.PerformerCaptions;
            }

            var multiplePerformersControl =
                performersControls.FirstOrDefault(p => p.Name == KrConstants.Ui.KrMultiplePerformersTableAcAlias);
            if (multiplePerformersControl != null)
            {
                multiplePerformersControl.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] =
                    vCtx.MultiplePerformers;
                multiplePerformersControl.ControlSettings[KrConstants.Ui.ControlCaptionsSetting] =
                    vCtx.PerformerCaptions;
            }

            performersBlock.Controls.AddRange(performersControls);
        }

        private static void SetAuthorVisibility(
            CardType krType,
            CardType authorCardType,
            VisibilityContext vCtx)
        {
            var authorBlock = GetBlock(krType, KrConstants.Ui.AuthorBlockAlias);
            var control = authorCardType
                .Blocks[0]
                .Controls
                .FirstOrDefault(p => p.Name == KrConstants.Ui.AuthorEntryAlias);
            if (control != null)
            {
                control.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] =
                    vCtx.CanOverrideAuthor;
            }

            authorBlock.Controls.AddRange(control);
        }

        private static void SetTaskKindVisibility(
            CardType krType,
            CardType taskKindCardType,
            VisibilityContext vCtx)
        {
            var taskKindBlock = GetBlock(krType, KrConstants.Ui.TaskKindBlockAlias);
            var taskKindControl = taskKindCardType
                .Blocks[0]
                .Controls
                .FirstOrDefault(p => p.Name == KrConstants.Ui.TaskKindEntryAlias);
            if (taskKindControl != null)
            {
                taskKindControl.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] = vCtx.UseTaskKind;
            }

            taskKindBlock.Controls.AddRange(taskKindControl);
        }

        private static void SetTaskHistoryGroupVisibility(
            CardType krType,
            CardType taskHistoryType,
            VisibilityContext vCtx)
        {
            var historyBlock = GetBlock(krType, KrConstants.Ui.KrTaskHistoryBlockAlias);
            var controls = taskHistoryType.Blocks[0].Controls;

            foreach (var controlName in
                new[]
                {
                    KrConstants.Ui.KrTaskHistoryGroupTypeControlAlias,
                    KrConstants.Ui.KrParentTaskHistoryGroupTypeControlAlias,
                    KrConstants.Ui.KrTaskHistoryGroupNewIterationControlAlias,
                })
            {
                var control = controls.FirstOrDefault(p => p.Name == controlName);
                if (control != null)
                {
                    control.ControlSettings[KrConstants.Ui.VisibleForTypesSetting] =
                        vCtx.CanOverrideTaskHistoryGroup;
                }
            }

            historyBlock.Controls.AddRange(controls);
        }

        private InjectCardTypeInfo[] GetInjectableCards(ICardMetadataExtensionContext context)
        {
            var perfCardType = this.PrepareInjectableCardType(DefaultCardTypes.KrPerformersSettingsTypeID, context);
            var authorCardType = this.PrepareInjectableCardType(DefaultCardTypes.KrAuthorSettingsTypeID, context);
            var taskKindCardType = this.PrepareInjectableCardType(DefaultCardTypes.KrTaskKindSettingsTypeID, context);
            var historyCardType = this.PrepareInjectableCardType(DefaultCardTypes.KrHistoryManagementTypeID, context);
            return new[]
            {
                new InjectCardTypeInfo { CardType = perfCardType, ModificationAction = SetPerformersVisibility },
                new InjectCardTypeInfo { CardType = authorCardType, ModificationAction = SetAuthorVisibility },
                new InjectCardTypeInfo { CardType = taskKindCardType, ModificationAction = SetTaskKindVisibility },
                new InjectCardTypeInfo { CardType = historyCardType, ModificationAction = SetTaskHistoryGroupVisibility },
            };
        }

        private CardType PrepareInjectableCardType(
            Guid typeID,
            ICardMetadataExtensionContext context)
        {
            CardType type = null;
            if (context.CardTypes.TryGetValue(typeID, out var t))
            {
                type = t.DeepClone();
                this.BuildMapping(new[] { type });
            }

            return type;
        }

        #endregion
    }
}