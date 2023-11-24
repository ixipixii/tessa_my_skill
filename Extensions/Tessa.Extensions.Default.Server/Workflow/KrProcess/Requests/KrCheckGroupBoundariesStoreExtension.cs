using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrCheckGroupBoundariesStoreExtension : CardStoreExtension
    {
        private readonly IKrScope krScope;

        private readonly IKrTypesCache typesCache;

        private readonly IKrProcessCache processCache;

        public KrCheckGroupBoundariesStoreExtension(
            IKrScope krScope,
            IKrTypesCache typesCache,
            IKrProcessCache processCache)
        {
            this.krScope = krScope;
            this.typesCache = typesCache;
            this.processCache = processCache;
        }

        public override Task BeforeRequest(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.CardType.ID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var card = context.Request.Card;
            if (!card.TryGetStagesSection(out var mainCardStagesSection)
                || !card.TryGetStagePositions(out var stagesPositions))
            {
                return Task.CompletedTask;
            }

            this.CheckMainCardBoundaries(mainCardStagesSection, stagesPositions, context.ValidationResult);
            return Task.CompletedTask;
        }

        public override Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !KrComponentsHelper.HasBase(context.CardType.ID, this.typesCache))
            {
                return Task.CompletedTask;
            }

            var card = context.Request.Card;
            var satellite = this.krScope.GetKrSatellite(card.ID);

            if (!satellite.TryGetStagesSection(out var satellliteStagesSection))
            {
                return Task.CompletedTask;
            }

            this.CheckStageGroupBounds(satellliteStagesSection, context.ValidationResult);
            return Task.CompletedTask;
        }

        private void CheckStageGroupBounds(
            CardSection satelliteStageSection,
            IValidationResultBuilder validationResult)
        {
            var groups = this.processCache.GetOrderedStageGroups();
            var groupsHashSet = this.processCache.GetAllStageGroups();

            var rows = satelliteStageSection
                .Rows
                // Исключаем как удаленные этапы, так и этапы с удаленными группами
                // А также нестеды, т.к. они по факту относятся к другим процессам
                .Where(p => p.State != CardRowState.Deleted
                    && groupsHashSet.ContainsKey(p.RowID)
                    && p.TryGet<Guid?>(KrConstants.KrStages.ParentStageRowID) == null)
                .OrderBy(p => p[KrConstants.Order])
                .ToArray();
            var rowIndex = 0;
            foreach (var currentGroup in groups)
            {
                while (rowIndex < rows.Length
                    && rows[rowIndex][KrConstants.StageGroupID].Equals(currentGroup.ID))
                {
                    rowIndex++;
                }
            }

            if (rowIndex != rows.Length)
            {
                var row = rows[rowIndex];
                var stageName = (string)row[KrConstants.Name];
                var stageGroupName = (string)row[KrConstants.StageGroupName];
                validationResult.AddError(this, "$KrMessages_ViolationOfGroupBoundaries", LocalizationManager.Localize(stageName), LocalizationManager.Localize(stageGroupName));
            }
        }

        private void CheckMainCardBoundaries(
            CardSection mainCardStagesSection,
            List<KrStagePositionInfo> stagesPositions,
            IValidationResultBuilder validationResult)
        {
            var rows = mainCardStagesSection
                .Rows
                // Исключаем как удаленные этапы, так и этапы с удаленными группами
                .Where(p => p.State != CardRowState.Deleted && p.ContainsKey(KrConstants.Order))
                .OrderBy(p => p[KrConstants.Order]);

            var currentOrder = int.MinValue;
            var groupsHashSet = this.processCache.GetAllStageGroups();
            foreach (var row in rows)
            {
                if ((row.TryGet<bool>(KrConstants.Keys.RootStage)
                    || row.TryGet<bool>(KrConstants.Keys.NestedStage))
                    && !row.All(p => p.Key.StartsWith(CardHelper.SystemKeyPrefix)
                        || p.Key.StartsWith(CardHelper.UserKeyPrefix)
                        || p.Key == KrConstants.RowID
                        || p.Key == KrConstants.Order))
                {
                    // Кто-то каким-то образом модифицировал нестед, что делать мы не разрешаем вообще.
                    validationResult.AddError(this, "$KrProcess_Error_TreeStructureStageModified");
                    return;
                }

                var rowID = row.RowID;
                foreach (var position in stagesPositions)
                {
                    if (position.RowID == rowID)
                    {
                        if (position.GroupOrder < currentOrder)
                        {
                            var stageName = position.Name;
                            var stageGroupName = groupsHashSet.TryGetValue(position.StageGroupID, out var group)
                                ? group.Name
                                : "unknown";
                            validationResult.AddWarning(
                                this,
                                "$KrMessages_ViolationOfGroupBoundaries",
                                LocalizationManager.Localize(stageName),
                                LocalizationManager.Localize(stageGroupName));
                            return;
                        }

                        currentOrder = position.GroupOrder;
                        break;
                    }
                }
            }
        }
    }
}