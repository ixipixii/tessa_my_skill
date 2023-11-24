using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    /// <summary>
    /// Расширение проверяет, изменилась ли строка с этапом согласования в карточке.
    /// Если изменилась, делает соответствующую отметку в поле RowChanged и OrderChanged
    /// </summary>
    public sealed class KrCheckStageRowModifiedStoreExtension : CardStoreExtension
    {
        #region constants

        private const string ChangedOrderInfoKey = nameof(ChangedOrderInfoKey);

        private const string ChangedRowInfoKey = nameof(ChangedRowInfoKey);

        /// <summary>
        /// Поля в строке этапа согласования, игнорируемые при отслеживании изменений
        /// </summary>
        private static readonly List<string> serviceFields = new List<string>
        {
            KrConstants.Keys.ParentStageRowID,
            $"{CardHelper.SystemKeyPrefix}state",
            $"{CardHelper.SystemKeyPrefix}changed",
            KrConstants.RowID,
            KrConstants.Order,
            KrConstants.KrStages.BasedOnStageRowID,
            KrConstants.KrStages.BasedOnStageTemplateID,
            KrConstants.KrStages.OrderChanged,
            KrConstants.KrStages.RowChanged,
            KrConstants.KrStages.StageStateID,
            KrConstants.KrStages.StageStateName,
            KrConstants.KrStages.DisplayTimeLimit,
            KrConstants.KrStages.DisplayParticipants,
            KrConstants.KrStages.DisplaySettings,
            KrConstants.KrStages.Skip,
            KrConstants.Keys.NestedStage,
            KrConstants.Keys.RootStage
        };

        #endregion

        #region fields

        private readonly IKrStageSerializer serializer;

        private readonly IKrTypesCache typesCache;

        private readonly IKrScope scope;

        private readonly ISignatureProvider signatureProvider;

        #endregion

        #region constructors

        public KrCheckStageRowModifiedStoreExtension(
            IKrStageSerializer serializer,
            IKrTypesCache typesCache,
            IKrScope scope,
            ISignatureProvider signatureProvider)
        {
            this.serializer = serializer;
            this.typesCache = typesCache;
            this.scope = scope;
            this.signatureProvider = signatureProvider;
        }

        #endregion

        #region private

        private static void CheckOrderSections(
            CardSection mainStagesSection,
            CardSection satelliteStagesSection,
            HashSet<Guid> changedOrders)
        {
            // 1. Получить все исходные этапы отсортированными
            var originalStages = satelliteStagesSection
                .Rows
                .Where(p => p.Fields.ContainsKey(KrConstants.Order))
                .OrderBy(p => p.Fields[KrConstants.Order])
                .ToArray();
            var originalLength = originalStages.Length;

            // 2. Будем вычеркивать исходные этапы, которые отсутствуют в измененном наборе
            var crossedOutItems = new bool[originalLength];

            // 3. Получить измененные этапы (откидываем удаленные и добавленные)
            var changedStages = mainStagesSection
                .Rows
                .Where(p => p.State == CardRowState.Modified && p.ContainsKey(KrConstants.Order))
                .OrderBy(p => p[KrConstants.Order])
                .ToArray();

            // 4. Спроецировать измененные на исходные - получаем измененные, но в исходном порядке
            for (int originalStageIDIndex = 0;
                originalStageIDIndex < originalLength;
                originalStageIDIndex++)
            {
                bool hasStage = changedStages
                    .Any(changedStage => changedStage.RowID == originalStages[originalStageIDIndex].RowID);

                if (!hasStage)
                {
                    // Для простоты лишние элементы не удаляются, а лишь помечаются
                    crossedOutItems[originalStageIDIndex] = true;
                }
            }

            // 5. Проходимся попарно по массивам - несовпадающие пары свидетельствуют об изменении порядка
            int changedStageIndex = 0;
            for (int originalStageIDIndex = 0;
                originalStageIDIndex < originalLength;
                originalStageIDIndex++)
            {
                if (!crossedOutItems[originalStageIDIndex])
                {
                    if (originalStages[originalStageIDIndex].RowID != changedStages[changedStageIndex].RowID)
                    {
                        changedOrders.Add(changedStages[changedStageIndex].RowID);
                    }
                    changedStageIndex++;
                }
            }
        }

        private static void CheckPlainRowChanges(
            CardSection mainStageSection,
            CardSection satelliteStageSection,
            HashSet<Guid> changedStages)
        {
            // Определяем изменения для секции KrStages
            var modifiedStages = mainStageSection
                .Rows
                .Where(p => p.State == CardRowState.Modified && p.Fields.Keys.Except(serviceFields).Any());

            foreach (var modifiedStage in modifiedStages)
            {
                changedStages.Add(modifiedStage.RowID);
            }
        }

        private void CheckChildRowsChanges(
            Card mainCard,
            CardSection satelliteStageSection,
            HashSet<Guid> changedStages)
        {
            foreach (var settingsSectionName in this.serializer.SettingsSectionNames)
            {
                if (settingsSectionName == KrConstants.KrStages.Virtual)
                {
                    continue;
                }

                ListStorage<CardRow> rows;
                if (mainCard.Sections.TryGetValue(settingsSectionName, out var settingsSec)
                    && (rows = settingsSec.TryGetRows()) != null)
                {
                    foreach (var row in rows)
                    {
                        var parentID = row.TryGet<Guid?>(KrConstants.Keys.ParentStageRowID);
                        if (parentID.HasValue)
                        {
                            changedStages.Add(parentID.Value);
                        }
                    }
                }
            }
        }

        private bool HasAnySettingsSection(
            Card mainCard)
        {
            var settingSectionNames = this.serializer.SettingsSectionNames;
            var mainCardSections = mainCard.Sections;
            // Там есть KrStagesVirtual
            return settingSectionNames.Any(settingSectionName => mainCardSections.ContainsKey(settingSectionName));
        }

        private static void SetOrderChanged(CardRow row)
        {
            row.Fields[KrConstants.KrStages.OrderChanged] = BooleanBoxes.True;
            row.Fields[KrConstants.KrStages.BasedOnStageTemplateGroupPositionID] =
                GroupPosition.Unspecified.ID;
            row.Fields[KrConstants.KrStages.BasedOnStageTemplateOrder] = null;
        }

        #endregion

        #region base overrides

        public override Task AfterBeginTransaction(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || context.CardType == null
                || !KrComponentsHelper.HasBase(context.CardType.ID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var card = context.Request.TryGetCard();
            if (card == null
                || !this.HasAnySettingsSection(card))
            {
                return Task.CompletedTask;
            }

            var satellite = this.scope.GetKrSatellite(card.ID);
            if (!satellite.TryGetStagesSection(out var satellliteStagesSection))
            {
                return Task.CompletedTask;
            }

            var changedRows = new HashSet<Guid>();
            var changedOrders = new HashSet<Guid>();
            if (card.TryGetStagesSection(out var mainCardStages))
            {
                CheckOrderSections(mainCardStages, satellliteStagesSection, changedOrders);
                CheckPlainRowChanges(mainCardStages, satellliteStagesSection, changedRows);
            }

            this.CheckChildRowsChanges(card, satellliteStagesSection, changedRows);
            context.Info[ChangedOrderInfoKey] = changedOrders;
            context.Info[ChangedRowInfoKey] = changedRows;

            return Task.CompletedTask;
        }


        public override Task BeforeCommitTransaction(
            ICardStoreExtensionContext context)
        {
            if( !context.ValidationResult.IsSuccessful()
                || !context.Info.TryGetValue(ChangedRowInfoKey, out var crObj)
                || !(crObj is HashSet<Guid> changedRows)
                || !context.Info.TryGetValue(ChangedOrderInfoKey, out var coObj)
                || !(coObj is HashSet<Guid> changedOrders))
            {
                return Task.CompletedTask;
            }

            var card = context.Request.Card;
            var satellite = this.scope.GetKrSatellite(card.ID);
            if (!satellite.TryGetStagesSection(out var satellliteStagesSection))
            {
                return Task.CompletedTask;
            }

            var signatures = StageRowMigrationHelper.GetSignatures(card.Info);
            var orders = StageRowMigrationHelper.GetOrders(card.Info);

            foreach (var row in satellliteStagesSection.Rows)
            {
                if (row.State == CardRowState.Inserted)
                {
                    StageRowMigrationHelper.VerifyRow(
                        row,
                        signatures,
                        orders,
                        out var rowChanged,
                        out var orderChaged,
                        this.serializer,
                        this.signatureProvider);

                    if (orderChaged)
                    {
                        SetOrderChanged(row);
                    }

                    if (rowChanged)
                    {
                        row.Fields[KrConstants.KrStages.RowChanged] = BooleanBoxes.True;
                    }
                }
                else
                {
                    if (changedRows.Contains(row.RowID))
                    {
                        row.Fields[KrConstants.KrStages.RowChanged] = BooleanBoxes.True;
                    }
                    if (changedOrders.Contains(row.RowID))
                    {
                        SetOrderChanged(row);
                    }
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}