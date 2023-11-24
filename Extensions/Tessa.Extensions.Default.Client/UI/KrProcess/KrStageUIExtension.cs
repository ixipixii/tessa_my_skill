using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tessa.Cards;
using Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers;
using Tessa.Extensions.Default.Shared.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Controls.StageSelector;
using Tessa.UI.Menu;
using Tessa.UI.Windows;
using Tessa.Views;
using Tessa.Views.Metadata;
using Tessa.Views.Metadata.Criteria;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    public sealed class KrStageUIExtension : CardUIExtension
    {
        #region nested types

        private sealed class VisibilityViaTagsVisitor : BreadthFirstControlVisitor
        {
            private readonly Guid typeID;

            public VisibilityViaTagsVisitor(
                Guid typeID)
            {
                this.typeID = typeID;
            }

            /// <inheritdoc />
            protected override void VisitControl(
                IControlViewModel controlViewModel)
            {
                SetControlSettings(this.typeID, controlViewModel);
            }

            /// <inheritdoc />
            protected override void VisitBlock(
                IBlockViewModel blockViewModel)
            {
                SetBlockControlsSettings(this.typeID, blockViewModel);
            }
        }

        #endregion

        #region fields

        private readonly IKrTypesCache typesCache;

        private readonly IStageTypeFormatterContainer formatterContainer;

        private readonly ISession session;

        private readonly IViewService viewService;

        private readonly IKrProcessUIContainer uiContainer;

        private readonly ICardMetadata cardMetadata;

        private readonly Dictionary<Guid, List<IStageTypeUIHandler>> handlersCache;

        private CardRow currentRow;

        #endregion

        #region constructor

        public KrStageUIExtension(
            IKrTypesCache typesCache,
            IStageTypeFormatterContainer formatterContainer,
            ISession session,
            IViewService viewService,
            IKrProcessUIContainer uiContainer,
            ICardMetadata cardMetadata)
        {
            this.typesCache = typesCache;
            this.formatterContainer = formatterContainer;
            this.session = session;
            this.uiContainer = uiContainer;
            this.cardMetadata = cardMetadata;
            this.viewService = viewService;
            this.handlersCache = new Dictionary<Guid, List<IStageTypeUIHandler>>();
        }

        #endregion

        #region private

        private async void AddStageDialogHandlerAsync(object sender, GridRowAddingEventArgs e)
        {
            Card card = e.CardModel.Card;
            StageGroupViewModel group = null; //фильтруем если диалог вызван из шаблона этапа

            CardSection krSecondaryProcess = null;
            bool hasGroup = card.Sections.TryGetValue(KrConstants.KrStageTemplates.Name, out CardSection krStageTemplates)
                || card.Sections.TryGetValue(KrConstants.KrSecondaryProcesses.Name, out krSecondaryProcess);
            if (hasGroup)
            {
                if (krStageTemplates != null)
                {
                    Guid? groupID = krStageTemplates.RawFields.TryGet<Guid?>(KrConstants.StageGroupID);
                    if (groupID.HasValue)
                    {
                        group = new StageGroupViewModel(
                            groupID.Value,
                            krStageTemplates.RawFields.TryGet<string>(KrConstants.StageGroupName),
                            order: 0);
                    }
                }
                else if (krSecondaryProcess != null)
                {
                    group = new StageGroupViewModel(
                        card.ID,
                        krSecondaryProcess.RawFields.TryGet<string>(KrConstants.Name),
                        order: 0);
                }
            }

            var type = (card.Sections.TryGetValue("DocumentCommonInfo", out var commonInfo) ? commonInfo.RawFields.TryGet<Guid?>("DocTypeID") : null) ?? card.TypeID;
            var viewModel = new StageSelectorViewModel(group, card.ID, type)
            {
                GetGroupTypesFuncAsync = async (groupVm, cardId, typeId, ct) =>
                {
                    if (hasGroup && groupVm is null)
                    {
                        // группа не выбрана в шаблоне этапов
                        return new List<StageGroupViewModel>();
                    }

                    if (groupVm != null)
                    {
                        return new List<StageGroupViewModel> { groupVm };
                    }

                    ITessaView stageGroupsView = await viewService.GetByNameAsync(KrConstants.Views.KrStageGroups, ct);
                    if (stageGroupsView is null)
                    {
                        return new List<StageGroupViewModel>();
                    }

                    var request = new TessaViewRequest(stageGroupsView.Metadata);

                    var cardIdParam = new RequestParameterBuilder()
                        .WithMetadata(stageGroupsView.Metadata.Parameters.FindByName("CardId"))
                        .AddCriteria(new EqualsCriteriaOperator(), string.Empty, cardId)
                        .AsRequestParameter();

                    request.Values.Add(cardIdParam);

                    var typeIdParam = new RequestParameterBuilder()
                        .WithMetadata(stageGroupsView.Metadata.Parameters.FindByName("TypeId"))
                        .AddCriteria(new EqualsCriteriaOperator(), string.Empty, typeId)
                        .AsRequestParameter();

                    request.Values.Add(typeIdParam);

                    ITessaViewResult result = await stageGroupsView.GetDataAsync(request, ct).ConfigureAwait(false);
                    return result.Rows?.Cast<IList<object>>().Select(row => new StageGroupViewModel((Guid) row[0], (string) row[1], (int) row[4])).ToList()
                        ?? new List<StageGroupViewModel>();
                },
                GetStageTypesFuncAsync = async (groupType, cardId, typeId, ct) =>
                {
                    if (hasGroup && group is null)
                    {
                        // группа не выбрана в шаблоне этапов
                        return new List<StageTypeViewModel>();
                    }

                    ITessaView stageTypesView = await viewService.GetByNameAsync(KrConstants.Views.KrProcessStageTypes, ct);
                    if (stageTypesView is null)
                    {
                        return new List<StageTypeViewModel>();
                    }

                    var request = new TessaViewRequest(stageTypesView.Metadata);
                    if (KrProcessSharedHelper.DesignTimeCard(typeId))
                    {
                        var isTemplateParam = new RequestParameterBuilder()
                            .WithMetadata(stageTypesView.Metadata.Parameters.FindByName("IsTemplate"))
                            .AddCriteria(new EqualsCriteriaOperator(), string.Empty, true)
                            .AsRequestParameter();

                        request.Values.Add(isTemplateParam);
                    }

                    var param = new RequestParameterBuilder()
                        .WithMetadata(stageTypesView.Metadata.Parameters.FindByName("StageGroupIDParam"))
                        .AddCriteria(new EqualsCriteriaOperator(), string.Empty, groupType)
                        .AsRequestParameter();

                    request.Values.Add(param);

                    var cardIdParam = new RequestParameterBuilder()
                        .WithMetadata(stageTypesView.Metadata.Parameters.FindByName("CardId"))
                        .AddCriteria(new EqualsCriteriaOperator(), string.Empty, cardId)
                        .AsRequestParameter();

                    request.Values.Add(cardIdParam);

                    var typeIdMetadata = stageTypesView.Metadata.Parameters.FindByName("TypeId");
                    RequestParameter typeIdParam;
                    if (KrProcessSharedHelper.DesignTimeCard(typeId))
                    {
                        typeIdParam = null;

                        ListStorage<CardRow> typeRows = card.Sections["KrStageTypes"].Rows;
                        if (typeRows.Count > 0)
                        {
                            HashSet<Guid> typeIdSet = null;

                            foreach (CardRow row in typeRows)
                            {
                                if (row.State != CardRowState.Deleted)
                                {
                                    Guid? templateTypeId = row.TryGet<Guid?>("TypeID");
                                    if (templateTypeId.HasValue)
                                    {
                                        if (typeIdSet is null)
                                        {
                                            typeIdSet = new HashSet<Guid>();
                                        }

                                        typeIdSet.Add(templateTypeId.Value);
                                    }
                                }
                            }

                            if (typeIdSet != null)
                            {
                                var typeIdBuilder = new RequestParameterBuilder()
                                    .WithMetadata(typeIdMetadata);

                                foreach (Guid id in typeIdSet)
                                {
                                    typeIdBuilder
                                        .AddCriteria(new EqualsCriteriaOperator(), string.Empty, id);
                                }

                                typeIdParam = typeIdBuilder.AsRequestParameter();
                            }
                        }
                    }
                    else
                    {
                        typeIdParam = new RequestParameterBuilder()
                            .WithMetadata(typeIdMetadata)
                            .AddCriteria(new EqualsCriteriaOperator(), string.Empty, typeId)
                            .AsRequestParameter();
                    }

                    if (typeIdParam != null)
                    {
                        request.Values.Add(typeIdParam);
                    }

                    var result = await stageTypesView.GetDataAsync(request, ct).ConfigureAwait(false);
                    return result.Rows?.Cast<IList<object>>().Select(row => new StageTypeViewModel((Guid) row[0], (string) row[1], (string) row[2])).ToList()
                        ?? new List<StageTypeViewModel>();
                }
            };

            using (e.Defer())
            {
                await viewModel.RefreshAsync();

                if (viewModel.Groups.Count == 0)
                {
                    TessaDialog.ShowNotEmpty(ValidationResult.FromText("$UI_Error_NoAvailableStageGroups", ValidationResultType.Warning));
                    e.Cancel = true;
                    return;
                }

                var window = new TessaWindow
                {
                    Content = viewModel,
                    Title = LocalizationManager.GetString("UI_Cards_SelectGroupAndType"),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CloseKey = new KeyGesture(Key.Escape),
                    Width = 700.0,
                    Height = 470.0,
                };
                // поскольку типов групп и этапов может быть несколько сотен, надо ограничить вертикальный размер диалога
                window.ResolveOwnerAsActiveWindow();
                window.RestrictSizeToWorkingArea(restrictWidth: false);
                window.CloseOnPreviewMiddleButtonDown();

                window.ShowDialog();

                if (viewModel.SelectedType is null || viewModel.SelectedGroup is null)
                {
                    e.Cancel = true;
                    return;
                }

                e.Row[KrConstants.StageGroupID] = viewModel.SelectedGroup.ID;
                e.Row[KrConstants.StageGroupName] = viewModel.SelectedGroup.Name;
                e.Row[KrConstants.StageGroupOrder] = viewModel.SelectedGroup.Order;
                e.Row[KrConstants.KrStages.StageTypeID] = viewModel.SelectedType.ID;
                e.Row[KrConstants.KrStages.StageTypeCaption] = viewModel.SelectedType.Caption;
                e.Row[KrConstants.Name] = LocalizationManager.Localize(viewModel.SelectedType.DefaultStage);

                if (!KrProcessSharedHelper.DesignTimeCard(e.CardModel.CardType.ID))
                {
                    e.RowIndex = KrProcessSharedHelper.ComputeStageOrder(
                        viewModel.SelectedGroup.ID,
                        viewModel.SelectedGroup.Order,
                        e.Rows);
                }
            }
        }

        private static void SetStageTypeTitle(
            object sender,
            GridRowEventArgs e)
        {
            if (e.Row.TryGetValue(KrConstants.KrStages.StageTypeCaption, out var titleObj)
                && titleObj is string title)
            {
                e.Window.Title = LocalizationManager.Localize(title);
            }
        }

        private static Visibility? SetOptionalControlVisibility(
            IBlockViewModel block,
            string controlAlias,
            GridRowEventArgs e)
        {
            var inputControl = block.Controls.FirstOrDefault(p => p.Name == controlAlias);
            if (inputControl is null)
            {
                return null;
            }

            var controlSettings = inputControl.CardTypeControl.ControlSettings;
            if (!controlSettings.TryGetValue(KrConstants.Ui.VisibleForTypesSetting, out var obj) ||
                !(obj is List<object> list))
            {
                return null;
            }

            inputControl.ControlVisibility = Visibility.Collapsed;
            foreach (var id in list)
            {
                if (id.Equals(e.Row[KrConstants.KrStages.StageTypeID]))
                {
                    inputControl.ControlVisibility = Visibility.Visible;
                    break;
                }
            }

            inputControl.IsRequired = false;
            if (controlSettings.TryGetValue(KrConstants.Ui.RequiredForTypesSetting, out var reqObj)
                && reqObj is List<object> requiredList)
            {
                var stageTypeID = e.Row[KrConstants.KrStages.StageTypeID];
                foreach (var id in requiredList)
                {
                    if (id.Equals(stageTypeID))
                    {
                        inputControl.IsRequired = true;
                        break;
                    }
                }
            }

            SetControlSettings(e.CardModel.CardType.ID, inputControl);
            var visitor = new VisibilityViaTagsVisitor(e.CardModel.CardType.ID);
            visitor.Visit(inputControl);
            return inputControl.ControlVisibility;
        }

        private static void SetControlCaption(
            IBlockViewModel block,
            string controlAlias,
            GridRowEventArgs e)
        {
            var inputControl = block.Controls.FirstOrDefault(p => p.Name == controlAlias);
            if (inputControl is null)
            {
                return;
            }

            var controlSettings = inputControl.CardTypeControl.ControlSettings;
            if (!controlSettings.TryGetValue(KrConstants.Ui.ControlCaptionsSetting, out var obj) ||
                !(obj is Dictionary<string, object> captions))
            {
                return;
            }

            var stageTypeID = (Guid) e.Row[KrConstants.KrStages.StageTypeID];
            if (captions.TryGetValue(stageTypeID.ToString("D"), out var caption))
            {
                inputControl.Caption = caption.ToString();
            }
        }

        private static void ShowSettingsBlock(object sender, GridRowEventArgs e)
        {
            if (e.RowModel.Blocks.TryGet(KrConstants.Ui.KrStageCommonInfoBlock, out var commonBlock))
            {
                SetOptionalControlVisibility(commonBlock, KrConstants.Ui.KrTimeLimitInputAlias, e);
                SetOptionalControlVisibility(commonBlock, KrConstants.Ui.KrPlannedInputAlias, e);
                SetOptionalControlVisibility(commonBlock, KrConstants.Ui.KrHiddenStageCheckboxAlias, e);
                SetOptionalControlVisibility(commonBlock, KrConstants.Ui.KrCanBeSkippedCheckboxAlias, e);
            }

            if (e.RowModel.Blocks.TryGet(KrConstants.Ui.KrPerformersBlockAlias, out var performersBlock))
            {
                var singleVisibility = SetOptionalControlVisibility(performersBlock, KrConstants.Ui.KrSinglePerformerEntryAcAlias, e);
                var multipleVisibility = SetOptionalControlVisibility(performersBlock, KrConstants.Ui.KrMultiplePerformersTableAcAlias, e);

                if (e.RowModel.Blocks.TryGet(KrConstants.Ui.KrSqlPerformersLinkBlock, out var sqlPerformersLinkBlock)
                    && multipleVisibility == Visibility.Visible)
                {
                    sqlPerformersLinkBlock.BlockVisibility = Visibility.Visible;
                }

                if (singleVisibility == Visibility.Visible)
                {
                    SetControlCaption(performersBlock, KrConstants.Ui.KrSinglePerformerEntryAcAlias, e);
                }

                if (multipleVisibility == Visibility.Visible)
                {
                    SetControlCaption(performersBlock, KrConstants.Ui.KrMultiplePerformersTableAcAlias, e);
                }
            }

            if (e.RowModel.Blocks.TryGet(KrConstants.Ui.AuthorBlockAlias, out var authorBlock))
            {
                SetOptionalControlVisibility(authorBlock, KrConstants.Ui.AuthorEntryAlias, e);
            }

            if (e.RowModel.Blocks.TryGet(KrConstants.Ui.TaskKindBlockAlias, out var taskKindBlock))
            {
                SetOptionalControlVisibility(taskKindBlock, KrConstants.Ui.TaskKindEntryAlias, e);
            }

            if (e.RowModel.Blocks.TryGet(KrConstants.Ui.KrTaskHistoryBlockAlias, out var historyBlock))
            {
                var v = SetOptionalControlVisibility(historyBlock, KrConstants.Ui.KrTaskHistoryGroupTypeControlAlias, e);
                SetOptionalControlVisibility(historyBlock, KrConstants.Ui.KrParentTaskHistoryGroupTypeControlAlias, e);
                SetOptionalControlVisibility(historyBlock, KrConstants.Ui.KrTaskHistoryGroupNewIterationControlAlias, e);
                // Если показывается контрол (а показываются они все по одному правилу), то показывается и заголовок
                historyBlock.CaptionVisibility = v ?? Visibility.Collapsed;
            }

            if (e.RowModel.Blocks.TryGet(KrConstants.Ui.KrStageSettingsBlockAlias, out var block))
            {
                var visitor = new VisibilityViaTagsVisitor(e.CardModel.CardType.ID);

                foreach (var control in block.Controls)
                {
                    var controlSettings = control.CardTypeControl.ControlSettings;
                    if (controlSettings.TryGetValue(KrConstants.Ui.StageHandlerDescriptorIDSetting, out var stageHandlerID))
                    {
                        if (e.Row[KrConstants.KrStages.StageTypeID].Equals(stageHandlerID))
                        {
                            control.ControlVisibility = Visibility.Visible;
                            visitor.Visit(control);
                        }
                        else
                        {
                            control.ControlVisibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            e.RowModel.MainForm.Rearrange();
        }

        private static void SetControlSettings(
            Guid typeID,
            IControlViewModel controlViewModel)
        {
            if (controlViewModel.ControlVisibility != Visibility.Visible)
            {
                // Если контрол скрыт, нет смысла в нем копаться.
                return;
            }

            var controlSettings = controlViewModel.CardTypeControl.ControlSettings;
            if (!controlSettings.TryGetValue(KrConstants.Ui.TagsListSetting, out var tagsObj)
                || !(tagsObj is List<object> tags))
            {
                return;
            }

            var hasRuntime = false;
            var hasDesign = false;
            var hasRuntimeReadonly = false;
            var hasDesignReadonly = false;

            foreach (var tag in tags)
            {
                switch (tag)
                {
                    case "Runtime":
                        hasRuntime = true;
                        break;
                    case "DesignTime":
                        hasDesign = true;
                        break;
                    case "RuntimeReadonly":
                        hasRuntimeReadonly = true;
                        break;
                    case "DesignTimeReadonly":
                        hasDesignReadonly = true;
                        break;
                }
            }

            if (!hasDesign
                && !hasRuntime)
            {
                controlViewModel.ControlVisibility = Visibility.Visible;
            }
            else if (hasDesign
                && KrProcessSharedHelper.DesignTimeCard(typeID))
            {
                controlViewModel.ControlVisibility = Visibility.Visible;
            }
            else if (hasRuntime
                && KrProcessSharedHelper.RuntimeCard(typeID))
            {
                controlViewModel.ControlVisibility = Visibility.Visible;
            }
            else
            {
                controlViewModel.ControlVisibility = Visibility.Collapsed;
            }

            if (controlViewModel.ControlVisibility == Visibility.Visible)
            {
                if (hasDesignReadonly
                    && KrProcessSharedHelper.DesignTimeCard(typeID))
                {
                    controlViewModel.IsReadOnly = true;
                }
                else if (hasRuntimeReadonly
                    && KrProcessSharedHelper.RuntimeCard(typeID))
                {
                    controlViewModel.IsReadOnly = true;
                }
            }
        }

        private static void SetBlockControlsSettings(
            Guid typeID,
            IBlockViewModel blockViewModel)
        {
            if (blockViewModel.BlockVisibility != Visibility.Visible)
            {
                // Если блок скрыт, нет смысла в нем копаться.
                return;
            }

            var blockSettings = blockViewModel.CardTypeBlock.BlockSettings;
            if (!blockSettings.TryGetValue(KrConstants.Ui.TagsListSetting, out var tagsObj)
                || !(tagsObj is List<object> tags))
            {
                return;
            }

            var hasRuntime = false;
            var hasDesign = false;

            foreach (var tag in tags)
            {
                switch (tag)
                {
                    case "Runtime":
                        hasRuntime = true;
                        break;
                    case "DesignTime":
                        hasDesign = true;
                        break;
                }
            }

            if (!hasDesign
                && !hasRuntime)
            {
                blockViewModel.BlockVisibility = Visibility.Visible;
            }
            else if (hasDesign
                && KrProcessSharedHelper.DesignTimeCard(typeID))
            {
                blockViewModel.BlockVisibility = Visibility.Visible;
            }
            else if (hasRuntime
                && KrProcessSharedHelper.RuntimeCard(typeID))
            {
                blockViewModel.BlockVisibility = Visibility.Visible;
            }
            else
            {
                blockViewModel.BlockVisibility = Visibility.Collapsed;
            }
        }

        private async void BindUIHandlers(
            object sender,
            GridRowEventArgs args)
        {
            using (args.Defer())
            {
                await this.RunHandlersAsync(args, (h, ctx) => h.Initialize(ctx));
            }
        }

        private async void ValidateViaHandlers(
            object sender,
            GridRowValidationEventArgs args)
        {
            using (args.Defer())
            {
                await this.RunHandlersAsync(args, (h, ctx) => h.Validate(ctx));
            }
        }

        private async void UnbindUIHandlers(
            object sender,
            GridRowEventArgs args)
        {
            using (args.Defer())
            {
                await this.RunHandlersAsync(args, (h, ctx) => h.Finalize(ctx));
            }
        }

        private async Task RunHandlersAsync(
            EventArgs args,
            Func<IStageTypeUIHandler, IKrStageTypeUIHandlerContext, Task> handleAsync,
            CancellationToken cancellationToken = default)
        {
            GridRowAction? action;
            GridViewModel control;
            CardRow row;
            ICardModel rowModel;
            ICardModel cardModel;
            IValidationResultBuilder validationResult;

            switch (args)
            {
                case GridRowEventArgs gridRowEventArgs:
                    if (gridRowEventArgs.Action == GridRowAction.Deleting)
                    {
                        return;
                    }

                    action = gridRowEventArgs.Action;
                    control = gridRowEventArgs.Control;
                    row = gridRowEventArgs.Row;
                    rowModel = gridRowEventArgs.RowModel;
                    cardModel = gridRowEventArgs.CardModel;
                    validationResult = null;
                    break;
                case GridRowValidationEventArgs gridRowValidationEventArgs:
                    action = null;
                    control = null;
                    row = gridRowValidationEventArgs.Row;
                    rowModel = gridRowValidationEventArgs.RowModel;
                    cardModel = gridRowValidationEventArgs.CardModel;
                    validationResult = gridRowValidationEventArgs.ValidationResult;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var stageTypeID = row.TryGet<Guid?>(KrConstants.KrStages.StageTypeID);
            if (!stageTypeID.HasValue)
            {
                return;
            }

            if (!this.handlersCache.TryGetValue(stageTypeID.Value, out var handlers))
            {
                handlers = this.uiContainer.ResolveUIHandlers(stageTypeID.Value);
                this.handlersCache.Add(stageTypeID.Value, handlers);
            }

            if (handlers?.Count > 0 != true)
            {
                return;
            }

            var context = new KrStageTypeUIHandlerContext(
                stageTypeID.Value,
                action,
                control,
                row,
                rowModel,
                cardModel,
                validationResult,
                cancellationToken);

            foreach (var handler in handlers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await handleAsync(handler, context);
                }
                catch (Exception e)
                {
                    await TessaDialog.ShowExceptionAsync(e);
                }
            }
        }

        private async void FormatRowHandler(object sender, GridRowEventArgs e)
        {
            using (e.Defer())
            {
                await this.FormatRowAsync(e.Row, e.CardModel, withChanges: true);
            }
        }

        private async Task FormatRowAsync(
            CardRow row,
            ICardModel cardModel,
            bool withChanges,
            CancellationToken cancellationToken = default)
        {
            if (!row.TryGetValue(KrConstants.KrStages.StageTypeID, out var stageTypeIDObj)
                || !(stageTypeIDObj is Guid stageTypeID))
            {
                return;
            }

            var formatter = this.formatterContainer.ResolveFormatter(stageTypeID);
            if (formatter is null)
            {
                return;
            }

            var info = new Dictionary<string, object>();
            var ctx = new StageTypeFormatterContext(
                this.session,
                info,
                cardModel.Card,
                row,
                null)
            {
                DisplayTimeLimit = row.TryGet<string>(KrConstants.KrStages.DisplayTimeLimit) ?? string.Empty,
                DisplayParticipants = row.TryGet<string>(KrConstants.KrStages.DisplayParticipants) ?? string.Empty,
                DisplaySettings = row.TryGet<string>(KrConstants.KrStages.DisplaySettings) ?? string.Empty
            };

            await cardModel.ExecuteInContextAsync(
                async (ui, ct) => formatter.FormatClient(ctx),
                cancellationToken).ConfigureAwait(false);

            if (withChanges)
            {
                row.SetIfDiffer(KrConstants.KrStages.DisplayTimeLimit, ctx.DisplayTimeLimit);
                row.SetIfDiffer(KrConstants.KrStages.DisplayParticipants, ctx.DisplayParticipants);
                row.SetIfDiffer(KrConstants.KrStages.DisplaySettings, ctx.DisplaySettings);
            }
            else
            {
                row[KrConstants.KrStages.DisplayTimeLimit] = ctx.DisplayTimeLimit;
                row[KrConstants.KrStages.DisplayParticipants] = ctx.DisplayParticipants;
                row[KrConstants.KrStages.DisplaySettings] = ctx.DisplaySettings;
            }
        }

        private void BindTimeFieldsRadio(
            object sender,
            GridRowEventArgs args) => args.Row.FieldChanged += this.OnTimeFieldChanged;

        private void UnbindTimeFieldsRadio(
            object sender,
            GridRowEventArgs args) => args.Row.FieldChanged -= this.OnTimeFieldChanged;

        private void OnTimeFieldChanged(
            object sender,
            CardFieldChangedEventArgs e)
        {
            if (e.FieldValue is null)
            {
                return;
            }

            switch (e.FieldName)
            {
                case KrConstants.KrStages.Planned:
                    this.currentRow.Fields[KrConstants.KrStages.TimeLimit] = null;
                    break;
                case KrConstants.KrStages.TimeLimit:
                    this.currentRow.Fields[KrConstants.KrStages.Planned] = null;
                    break;
            }
        }

        private void ValidateTimeLimit(
            object sender,
            GridRowValidationEventArgs args)
        {
            var row = args.Row;
            var stageTypeID = row.TryGet<Guid?>(KrConstants.KrStages.StageTypeID);
            var timeLimit = row.TryGet<double?>(KrConstants.KrStages.TimeLimit);
            var planned = row.TryGet<DateTime?>(KrConstants.KrStages.Planned);
            var controls = args.RowModel.Controls;
            var checkTimeLimit = CheckField(KrConstants.Ui.KrTimeLimitInputAlias);
            var checkPlanned = CheckField(KrConstants.Ui.KrPlannedInputAlias);
            if (checkTimeLimit
                && checkPlanned)
            {
                if (timeLimit is null
                    && planned is null)
                {
                    args.ValidationResult.AddWarning(this, "$UI_Error_TimeLimitOrPlannedNotSpecifiedWarn");
                }
            }
            else if (checkTimeLimit
                && timeLimit is null)
            {
                args.ValidationResult.AddWarning(this, "$UI_Error_TimeLimitNotSpecifiedWarn");
            }
            else if (checkPlanned
                && planned is null)
            {
                args.ValidationResult.AddWarning(this, "$UI_Error_PlannedNotSpecifiedWarn");
            }

            bool CheckField(string inputAlias) =>
                controls.TryGet(inputAlias, out var control)
                && control.CardTypeControl.ControlSettings.TryGetValue(KrConstants.Ui.VisibleForTypesSetting, out var obj)
                && obj is List<object> list
                && list.Any(p => p.Equals(stageTypeID));
        }

        private void ValidatePerformers(
            object sender,
            GridRowValidationEventArgs args)
        {
            var controls = args.RowModel.Controls;
            PerformerUsageMode mode;
            if (controls.TryGet(KrConstants.Ui.KrSinglePerformerEntryAcAlias, out var control)
                && control.ControlVisibility == Visibility.Visible)
            {
                mode = PerformerUsageMode.Single;
            }
            else if (controls.TryGet(KrConstants.Ui.KrMultiplePerformersTableAcAlias, out control)
                || control.ControlVisibility == Visibility.Visible)
            {
                mode = PerformerUsageMode.Multiple;
            }
            else
            {
                return;
            }

            var controlSettings = control.CardTypeControl.ControlSettings;
            if (!controlSettings.TryGetValue(KrConstants.Ui.RequiredForTypesSetting, out var obj) ||
                !(obj is List<object> list))
            {
                return;
            }

            var row = args.Row;
            foreach (var id in list)
            {
                if (id.Equals(row[KrConstants.KrStages.StageTypeID]))
                {
                    if ((mode == PerformerUsageMode.Single
                            && row[KrConstants.KrSinglePerformerVirtual.PerformerID] is null)
                        || (mode == PerformerUsageMode.Multiple
                            && args.CardModel.Card.Sections.TryGetValue(KrConstants.KrPerformersVirtual.Synthetic, out var perfSec)
                            && !perfSec.Rows.Any(p => p[KrConstants.StageRowID].Equals(row.RowID) && p.State != CardRowState.Deleted)))
                    {
                        args.ValidationResult.AddWarning(this, "$UI_Error_PerformerNotSpecifiedWarn");
                    }

                    break;
                }
            }
        }



        #endregion

        #region base overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            var cardModel = context.Model;

            await SetTabIndicationAsync(KrConstants.KrStageTemplates.Name, KrConstants.Ui.CSharpSourceTable, context.CancellationToken);
            await SetTabIndicationAsync(KrConstants.KrStageGroups.Name, KrConstants.Ui.CSharpSourceTableDesign, context.CancellationToken);
            await SetTabIndicationAsync(KrConstants.KrStageGroups.Name, KrConstants.Ui.CSharpSourceTableRuntime, context.CancellationToken);

            // Для шаблона этапов и вторички тоже выполняем расширение без проверки компонентов
            if (!KrProcessSharedHelper.DesignTimeCard(cardModel.CardType.ID))
            {
                var usedComponents = KrComponentsHelper.GetKrComponents(cardModel.Card, this.typesCache);
                // Выходим если нет согласования
                if (usedComponents.HasNot(KrComponents.Routes))
                {
                    return;
                }
            }

            if (!cardModel.Controls.TryGet(KrConstants.Ui.KrApprovalStagesControlAlias, out var control) ||
                !(control is GridViewModel approvalStagesTable))
            {
                return;
            }

            if (!KrProcessSharedHelper.DesignTimeCard(context.Model.CardType.ID))
            {
                var selectedApprovalStages = approvalStagesTable.SelectedRows;
                approvalStagesTable.LeftButtons.Add(
                    new UIButton("$CardTypes_Buttons_ActivateStage",
                    btn => ActivateStagesHandler(selectedApprovalStages),
                    () => HasEnableActivateStageButton(cardModel.Card, selectedApprovalStages)));

                approvalStagesTable.ContextMenuGenerators.Add(
                    ctx =>
                    {
                        ctx.MenuActions.Add(
                            new MenuAction(
                                "ActivateStage",
                                "$CardTypes_Buttons_ActivateStage",
                                Icon.Empty,
                                new DelegateCommand(p => ActivateStagesHandler(selectedApprovalStages)),
                                isEnabled: HasEnableActivateStageButton(cardModel.Card, selectedApprovalStages)));
                    });

                foreach (var approvalStageRow in approvalStagesTable.Rows.Where(i => i.Model.Fields.TryGet<bool>(KrConstants.KrStages.Hidden)))
                {
                    approvalStageRow.Background = KrProcessBrushes.Gainsboro;
                }
            }

            approvalStagesTable.RowAdding += this.AddStageDialogHandlerAsync;
            approvalStagesTable.RowInitializing += SetStageTypeTitle;
            approvalStagesTable.RowInitializing += ShowSettingsBlock;
            approvalStagesTable.RowInvoked += (s, e) => this.currentRow = e.Row;
            approvalStagesTable.RowInvoked += this.BindUIHandlers;
            approvalStagesTable.RowInvoked += this.BindTimeFieldsRadio;
            approvalStagesTable.RowEditorClosing += this.FormatRowHandler;
            approvalStagesTable.RowValidating += this.ValidateViaHandlers;
            approvalStagesTable.RowValidating += this.ValidateTimeLimit;
            approvalStagesTable.RowValidating += this.ValidatePerformers;
            approvalStagesTable.RowEditorClosed += this.UnbindUIHandlers;
            approvalStagesTable.RowEditorClosed += this.UnbindTimeFieldsRadio;
            approvalStagesTable.RowEditorClosed += (s, e) => this.currentRow = null;

            if (context.Card.Sections.TryGetValue(KrConstants.KrStages.Virtual, out var stagesSec))
            {
                foreach (var row in stagesSec.Rows)
                {
                    await this.FormatRowAsync(row, cardModel, withChanges: false, cancellationToken: context.CancellationToken);
                }
            }

            async Task SetTabIndicationAsync(string sectionName, string controlName, CancellationToken cancellationToken = default)
            {
                if (cardModel.Card.Sections.TryGetValue(sectionName, out var templatesSec))
                {
                    var sectionMeta = (await this.cardMetadata.GetSectionsAsync(cancellationToken))[sectionName];
                    var fieldIDs = sectionMeta.Columns.ToDictionary(k => k.ID, v => v.Name);

                    var tabControl = (TabControlViewModel) cardModel.Controls[controlName];
                    var indicator = new TabContentIndicator(tabControl, templatesSec.RawFields, fieldIDs, true);
                    indicator.Update();
                    templatesSec.FieldChanged += indicator.FieldChangedAction;
                }
            }
        }

        /// <summary>
        /// Возвращает значение, показывающее, можно ли активировать выбранные пропущенные этапы.
        /// </summary>
        /// <param name="card">Карточка, содержащая констрол с выбранными этапами.</param>
        /// <param name="selectedRows">Коллекция выбранных строк.</param>
        /// <returns>Значение, показывающее, можно ли активировать выбранные пропущенные этапы.</returns>
        private static bool HasEnableActivateStageButton(Card card, IList<CardRowViewModel> selectedRows)
        {
            return (selectedRows.Count > 0)
                && TryGetKrToken(card, out var krToken)
                && CanActivateStages(krToken)
                && selectedRows.All(i => i.Model.Fields.TryGet<bool>(KrConstants.KrStages.Skip));
        }

        /// <summary>
        /// Активирует указанные этапы.
        /// </summary>
        /// <param name="selectedRowStages">Коллекция строк - этапов, которые необходимо активировать.</param>
        private static void ActivateStagesHandler(IList<CardRowViewModel> selectedRowStages)
        {
            foreach (var selectedRowStage in selectedRowStages)
            {
                selectedRowStage.Model.Fields[KrConstants.KrStages.Skip] = BooleanBoxes.False;
                selectedRowStage.Background = KrProcessBrushes.Transparent;
            }
        }

        /// <summary>
        /// Возвращает значение, показывающее, разрешено ли активировать этапы.
        /// </summary>
        /// <param name="krToken">Токен, для которого выполяется проверка.</param>
        /// <returns>Значение, показывающее, разрешено ли активировать этапы.</returns>
        public static bool CanActivateStages(KrToken krToken)
        {
            if (krToken.HasPermission(KrPermissionFlagDescriptors.EditRoute))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает токен безопасности связанный с указанной карточкой.
        /// </summary>
        /// <param name="card">Карточка из которой требуется получить токен.</param>
        /// <param name="krToken">Возвращаемое значение. Токен безопасности.</param>
        /// <returns>Значение true, если токен успешно получен, иначе - false.</returns>
        private static bool TryGetKrToken(Card card, out KrToken krToken)
        {
            krToken = KrToken.TryGet(card.Info);
            return krToken != null;
        }

        #endregion
    }
}