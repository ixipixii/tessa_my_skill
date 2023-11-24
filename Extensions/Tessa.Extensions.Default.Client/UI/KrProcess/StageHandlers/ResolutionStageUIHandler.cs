using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI.Cards;
using Tessa.UI.Controls;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class ResolutionStageUIHandler : StageTypeUIHandlerBase
    {
        private CardRow settings;
        private ListStorage<CardRow> performers;
        private ICardModel card;
        private IControlViewModel controller;
        private readonly HashSet<CardRow> subscribedTo;
        private readonly IDialogService dialogService;

        private const string RowID = KrConstants.RowID;
        private const string StageRowID = KrConstants.StageRowID;

        private static readonly string controllerID = KrConstants.KrResolutionSettingsVirtual.ControllerID;
        private static readonly string controllerName = KrConstants.KrResolutionSettingsVirtual.ControllerName;
        private static readonly string planned = KrConstants.KrResolutionSettingsVirtual.Planned;
        private static readonly string durationInDays = KrConstants.KrResolutionSettingsVirtual.DurationInDays;
        private static readonly string withControl = KrConstants.KrResolutionSettingsVirtual.WithControl;
        private static readonly string massCreation = KrConstants.KrResolutionSettingsVirtual.MassCreation;
        private static readonly string majorPerformer = KrConstants.KrResolutionSettingsVirtual.MajorPerformer;
        private static readonly string performersSection = KrConstants.KrPerformersVirtual.Synthetic;

        public ResolutionStageUIHandler(
            IDialogService dialogService)
        {
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.subscribedTo = new HashSet<CardRow>();
        }

        public override Task Initialize(
            IKrStageTypeUIHandlerContext context)
        {
            this.settings = context.Row;
            this.settings.FieldChanged += this.OnSettingsFieldChanged;
            this.performers = context.CardModel.Card.Sections[performersSection].Rows;
            this.performers.ItemChanged += this.OnPerformersChanged;

            foreach (var performer in this.performers)
            {
                if (this.AlivePerformer(performer))
                {
                    this.subscribedTo.Add(performer);
                    performer.StateChanged += this.OnPerformerStateChanged;
                }
            }

            this.card = context.RowModel;
            this.controller = this.card.Controls["Controller"];

            if ((bool) this.settings[withControl])
            {
                this.controller.CaptionVisibility = Visibility.Visible;
                this.controller.ControlVisibility = Visibility.Visible;
            }

            return Task.CompletedTask;
        }

        public override Task Finalize(
            IKrStageTypeUIHandlerContext context)
        {
            if (this.settings.Fields[durationInDays] == null && this.settings.Fields[planned] == null)
            {
                this.dialogService.ShowValidationResult(
                    new ValidationResultBuilder()
                        .AddWarning("$WfResolution_Error_ResolutionHasNoPlannedDate")
                        .Build());
            }

            this.settings.FieldChanged -= this.OnSettingsFieldChanged;
            this.settings = null;
            this.performers.ItemChanged -= this.OnPerformersChanged;
            this.performers = null;

            foreach (var performer in this.subscribedTo)
            {
                performer.StateChanged -= this.OnPerformerStateChanged;
            }

            this.subscribedTo.Clear();
            this.controller = null;

            return Task.CompletedTask;
        }

        private void OnSettingsFieldChanged(
            object sender,
            CardFieldChangedEventArgs e)
        {
            if (e.FieldName == planned)
            {
                if (e.FieldValue != null)
                {
                    this.settings.Fields[durationInDays] = null;
                }
            }
            else if (e.FieldName == durationInDays)
            {
                if (e.FieldValue != null)
                {
                    this.settings.Fields[planned] = null;
                }
            }
            else if (e.FieldName == withControl)
            {
                var visibility = Visibility.Collapsed;
                if ((bool) e.FieldValue)
                {
                    visibility = Visibility.Visible;
                }
                else
                {
                    this.settings.Fields[controllerID] = null;
                    this.settings.Fields[controllerName] = null;
                }

                this.controller.CaptionVisibility = visibility;
                this.controller.ControlVisibility = visibility;
                this.controller.Block.Form.Rearrange();
            }
            else if (e.FieldName == massCreation && !(bool) e.FieldValue)
            {
                this.settings.Fields[majorPerformer] = BooleanBoxes.False;
            }
        }

        private void OnPerformerStateChanged(
            object sender,
            CardRowStateEventArgs e)
        {
            if (e.NewState == CardRowState.Deleted)
            {
                this.OnPerformersChanged(ListStorageAction.Remove, (CardRow) sender);
            }

            if (e.OldState == CardRowState.Deleted)
            {
                this.OnPerformersChanged(ListStorageAction.Insert, (CardRow) sender);
            }
        }

        private void OnPerformersChanged(
            object sender,
            ListStorageItemEventArgs<CardRow> e)
            => this.OnPerformersChanged(e.Action, e.Item);

        private void OnPerformersChanged(
            ListStorageAction action,
            CardRow performer)
        {
            switch (action)
            {
                case ListStorageAction.Insert:
                    if (this.subscribedTo.Add(performer))
                    {
                        performer.StateChanged += this.OnPerformerStateChanged;
                    }

                    // Действия могут производиться только в текущем диалоге, а значит,
                    // всякий новодобавленный оказывается в текущем этапе. По этой причине
                    // требуется наличие лишь одного исполняющего в таблице. Второй уже
                    // добавлен, но его связь и прочие поля будут указаны позже.
                    if (this.performers.Count(this.AlivePerformer) >= 1)
                    {
                        this.EnableMassCreation(true);
                    }

                    break;

                case ListStorageAction.Remove:
                    if (this.subscribedTo.Remove(performer))
                    {
                        performer.StateChanged -= this.OnPerformerStateChanged;
                    }

                    if (this.performers.Count(this.AlivePerformer) < 2)
                    {
                        this.EnableMassCreation(false);
                    }

                    break;

                case ListStorageAction.Clear:
                    foreach (var subscribedToItem in this.subscribedTo)
                    {
                        subscribedToItem.StateChanged -= this.OnPerformerStateChanged;
                    }

                    this.subscribedTo.Clear();
                    this.EnableMassCreation(false);
                    break;
            }
        }

        private bool AlivePerformer(
            CardRow performer) =>
            performer.State != CardRowState.Deleted && performer.TryGetValue(StageRowID, out var value) &&
            Equals(value, this.settings[RowID]);

        private void EnableMassCreation(
            bool value) => this.settings.Fields[massCreation] = BooleanBoxes.Box(value);
    }
}