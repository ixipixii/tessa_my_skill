using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrRecalcStagesStoreExtension: CardStoreExtension
    {
        #region constants

        private const string UnknownLiteral = "unknown";

        #endregion

        #region fields

        private readonly IKrTypesCache typesCache;
        private readonly IKrExecutor executor;
        private readonly IKrScope krScope;
        private readonly IObjectModelMapper mapper;
        private readonly IKrProcessCache processCache;

        private bool? hasChangesInfo;

        private IList<RouteDiff> diffsInfo;

        #endregion

        #region constructor

        public KrRecalcStagesStoreExtension(
            IKrTypesCache typesCache,
            [Unity.Dependency(KrExecutorNames.CacheExecutor)]IKrExecutor executor,
            IKrScope krScope,
            IObjectModelMapper mapper,
            IKrProcessCache processCache)
        {
            this.typesCache = typesCache;
            this.executor = executor;
            this.krScope = krScope;
            this.mapper = mapper;
            this.processCache = processCache;
        }

        #endregion

        #region private

        private static InfoAboutChanges ExtractInfoAboutChanges(
            ICardStoreExtensionContext context) => context.Request.GetInfoAboutChanges() ?? InfoAboutChanges.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatActualName(RouteDiff diff) =>
            $"\"{LocalizationManager.Localize(diff.ActualName ?? diff.OldName ?? UnknownLiteral)}\"";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatOldName(RouteDiff diff)
        {
            if (diff.ActualName != diff.OldName
                && !string.IsNullOrWhiteSpace(diff.OldName)
                && (diff.ActualName ?? diff.OldName) != diff.OldName)
            {
                var renamedFrom = LocalizationManager.GetString("KrCompilation_RouteElementRenamedFrom");
                return $" ({renamedFrom} \"{LocalizationManager.Localize(diff.OldName)}\")";
            }

            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatHidden(RouteDiff diff) =>
            diff.HiddenStage
                ? " (" + LocalizationManager.GetString("KrCompilation_RecalcChanges_HiddenStage") + ")"
                : string.Empty;

        private void ChangesToResponse(
            InfoAboutChanges infoAboutChanges,
            bool? hasChanges,
            IList<RouteDiff> diffs,
            IValidationResultBuilder validationResult)
        {
            if (infoAboutChanges == InfoAboutChanges.None
                || !hasChanges.HasValue)
            {
                return;
            }
            // Были ли изменения в Info просто по требованию
            if (infoAboutChanges.Has(InfoAboutChanges.HasChangesToInfo))
            {
                this.hasChangesInfo = hasChanges.Value;
            }
            // В ValidationResult положительный результат о наличии изменений помещается
            // только если изменения были и полная инфа об изменениях не нужна
            // Если запрошено полная инфа, отдельное сообщение "Изменение есть" лишнее
            if (hasChanges == true
                && infoAboutChanges.Has(InfoAboutChanges.HasChangesToValidationResult)
                && infoAboutChanges.HasNot(InfoAboutChanges.ChangesListToValidationResult))
            {
                ValidationSequence
                    .Begin(validationResult)
                    .Warning(DefaultValidationKeys.RecalcWithChanges)
                    .End();
            }
            // Изменений нет, надо сообщить об этом в ValidationResult
            // Если были запрошены полная инфа об изменениях, все равно необходимо сообщить,
            // что изменений нет
            else if (hasChanges == false
                && infoAboutChanges.HasAny(InfoAboutChanges.ToValidationResult))
            {
                ValidationSequence
                    .Begin(validationResult)
                    .Warning(DefaultValidationKeys.RecalcWithoutChanges)
                    .End();
            }

            if (diffs != null)
            {
                if (infoAboutChanges.Has(InfoAboutChanges.ChangesListToInfo))
                {
                    this.diffsInfo = diffs;
                }
                if (infoAboutChanges.Has(InfoAboutChanges.ChangesListToValidationResult))
                {
                    this.ChangesListToValidationResult(diffs, validationResult);
                }
            }
        }

        private void ChangesListToValidationResult(
            IList<RouteDiff> diffs,
            IValidationResultBuilder validationResult)
        {
            foreach (var diff in diffs)
            {
                var validator = ValidationSequence
                    .Begin(validationResult)
                    .SetObjectName(this);

                switch (diff.Action)
                {
                    case RouteDiffAction.Insert:
                        validator.Warning(DefaultValidationKeys.StageAdded, FormatActualName(diff), FormatHidden(diff));
                        break;
                    case RouteDiffAction.Delete:
                        validator.Warning(DefaultValidationKeys.StageDeleted, FormatActualName(diff), FormatHidden(diff));
                        break;
                    case RouteDiffAction.Modify:
                        validator.Warning(DefaultValidationKeys.StageModified, FormatActualName(diff), FormatOldName(diff), FormatHidden(diff));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                validator.End();
            }
        }

        #endregion

        #region base overrides

        public override Task BeforeRequest(
            ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !context.Request.GetRecalcFlag())
            {
                return Task.CompletedTask;
            }

            context.Request.ForceTransaction = true;
            return Task.CompletedTask;
        }

        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful()
                || !context.Request.GetRecalcFlag())
            {
                return;
            }

            if(!KrProcessHelper.CardSupportsRoutes(
                context.Request.Card,
                context.DbScope,
                this.typesCache))
            {
                return;
            }

            var docTypeID = KrProcessSharedHelper.GetDocTypeID(context.Request.Card);

            var satellite = this.krScope.GetKrSatellite(context.Request.Card.ID);
            if (satellite == null)
            {
                return;
            }
            if (satellite.IsMainProcessStarted())
            {
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .Error(DefaultValidationKeys.MainProcessStarted)
                    .End();
                return;
            }

            var card = context.Request.Card;
            var (templates, stages) = this.processCache.GetRelatedTemplates(satellite);
            var pci = this.mapper.GetMainProcessCommonInfo(satellite);

            NestedStagesCleaner.ClearAll(satellite);

            var objectModel = this.mapper.CardRowsToObjectModel(
                satellite,
                pci,
                pci,
                templates,
                stages,
                initialStage: true);
            this.mapper.FillWorkflowProcessFromPci(
                objectModel,
                pci,
                pci);

            var components = KrComponentsHelper.GetKrComponents(card.TypeID, docTypeID, this.typesCache);
            var ctx = new KrExecutionContext(
                cardContext: context,
                mainCardAccessStrategy: new KrScopeMainCardAccessStrategy(card.ID, this.krScope),
                cardID: card.ID,
                cardTypeID: card.TypeID,
                cardTypeName: card.TypeName,
                cardTypeCaption: card.TypeCaption,
                docTypeID: docTypeID,
                krComponents: components,
                workflowProcess: objectModel,
                compilationResult: null);
            var result = this.executor.Execute(ctx);

            var diffs = this.mapper.ObjectModelToCardRows( ctx.WorkflowProcess, satellite, pci);
            this.mapper.ObjectModelToPci(ctx.WorkflowProcess, pci, pci, pci);
            this.mapper.SetMainProcessCommonInfo(card.ID, satellite, pci);

            context.ValidationResult.Add(result.Result);
            if (context.ValidationResult.IsSuccessful())
            {
                var mode = ExtractInfoAboutChanges(context);
                if (mode.HasNot(InfoAboutChanges.ChangesInHiddenStages))
                {
                    diffs = diffs
                        .Where(p => !p.HiddenStage)
                        .ToList();
                }

                this.ChangesToResponse(
                    mode,
                    diffs.Any(),
                    diffs,
                    context.ValidationResult);
            }
            else
            {
                this.ChangesToResponse(
                    InfoAboutChanges.None,
                    false,
                    EmptyHolder<RouteDiff>.Collection,
                    context.ValidationResult);
            }
        }

        public override Task AfterRequest(ICardStoreExtensionContext context)
        {
            if (this.hasChangesInfo != null)
            {
                context.Response.SetHasRecalcChanges(this.hasChangesInfo.Value);
            }

            if (this.diffsInfo != null)
            {
                 context.Response.SetRecalcChanges(this.diffsInfo);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
