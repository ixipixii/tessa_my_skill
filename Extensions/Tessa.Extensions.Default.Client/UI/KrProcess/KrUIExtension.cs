using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Metadata;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Cards.Forms;
using Tessa.UI.Cards.Tasks;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    /// <summary>
    /// Скрывает результаты запроса комментария из задания согласования если комментарий не запрашивался.
    /// Скрывает поле "Комментарий" для варианта завершения "Согласовать", если установлена соответствующая настройка.
    /// </summary>
    public sealed class KrUIExtension : CardUIExtension
    {
        #region Constructors

        public KrUIExtension(ICardCache cardCache)
        {
            this.cardCache = cardCache;
        }

        #endregion

        #region Fields

        private readonly ICardCache cardCache;

        private bool? commentIsHiddenForApproval;

        #endregion

        #region Private Methods

        private async ValueTask<bool> CommentIsHiddenForApprovalAsync(CancellationToken cancellationToken = default)
        {
            if (this.commentIsHiddenForApproval.HasValue)
            {
                return this.commentIsHiddenForApproval.Value;
            }

            bool value;

            try
            {
                value = (await this.cardCache.Cards.GetAsync("KrSettings", cancellationToken))
                    .Entries
                    .Get<bool>("KrSettings", "HideCommentForApprove");
            }
            catch (SingletonNotFoundInCacheException)
            {
                value = false;
            }

            this.commentIsHiddenForApproval = value;
            return value;
        }

        private static Task ModifyDialogTaskAsync(TaskViewModel taskViewModel)
        {
            ICardModel taskModel = taskViewModel.TaskModel;
            if (taskModel.CardType.ID != DefaultTaskTypes.KrShowDialogTypeID
                || !taskModel.CardTask.IsPerformer)
            {
                return Task.CompletedTask;
            }

            return taskViewModel.ModifyWorkspaceAsync((t, b) =>
            {
                if (t.Workspace.Form?.Name != null)
                {
                    return Task.CompletedTask;
                }

                var co = CardTaskDialogHelper.GetCompletionOptionSettings(t.TaskModel.CardTask, DefaultCompletionOptions.ShowDialog);
                if (co is null)
                {
                    return Task.CompletedTask;
                }

                var action = t.Workspace.Actions.First(p => p.CompletionOption.ID == DefaultCompletionOptions.ShowDialog);
                if (action != null && !string.IsNullOrWhiteSpace(co.TaskButtonCaption))
                {
                    action.Caption = co.TaskButtonCaption;
                }

                return Task.CompletedTask;
            });

        }

        private Task ModifyUniversalTaskAsync(TaskViewModel taskViewModel)
        {
            ICardModel taskModel = taskViewModel.TaskModel;
            if (taskModel.CardType.ID != DefaultTaskTypes.KrUniversalTaskTypeID
                || !taskModel.CardTask.IsPerformer
                || taskModel.CardTask.StoredState != CardTaskState.InProgress)
            {
                return Task.CompletedTask;
            }

            return taskViewModel.ModifyWorkspaceAsync((t, b) =>
            {
                if (t.Workspace.Form?.Name != null)
                {
                    return Task.CompletedTask;
                }

                int actionsInitialCount = t.Workspace.Actions.Count;
                int additionalActionsInitialCount = t.Workspace.AdditionalActions.Count;

                foreach (var row in
                    taskModel
                        .Card
                        .Sections
                        .GetOrAddTable(KrConstants.KrUniversalTaskOptions.Name)
                        .Rows
                        .OrderBy(x => x.TryGet<int?>(KrConstants.KrUniversalTaskOptions.Order)))
                {
                    var optionID = row.Get<Guid>(KrConstants.KrUniversalTaskOptions.OptionID);
                    var caption = row.Get<string>(KrConstants.KrUniversalTaskOptions.Caption);
                    var showComment = row.Get<bool>(KrConstants.KrUniversalTaskOptions.ShowComment);
                    var message = row.Get<string>(KrConstants.KrUniversalTaskOptions.Message);
                    var additional = row.Get<bool>(KrConstants.KrUniversalTaskOptions.Additional);

                    if (additional)
                    {
                        int index = t.Workspace.AdditionalActions.Count - additionalActionsInitialCount;

                        if (index == 0)
                        {
                            t.Workspace.AdditionalActions.Insert(0, new TaskSeparatorActionViewModel());
                            additionalActionsInitialCount++;
                        }

                        t.Workspace.AdditionalActions.Insert(
                            index,
                            this.GenerateTaskAction(
                                t.Navigator,
                                optionID,
                                caption,
                                message,
                                showComment));
                    }
                    else
                    {
                        t.Workspace.Actions.Insert(
                            t.Workspace.Actions.Count - actionsInitialCount,
                            this.GenerateTaskAction(
                                t.Navigator,
                                optionID,
                                caption,
                                message,
                                showComment));
                    }
                }

                return Task.CompletedTask;
            });
        }

        private ITaskAction GenerateTaskAction(
            TaskNavigator navigator,
            Guid optionID,
            string caption,
            string message,
            bool showComment)
        {
            var formName = showComment || message != null
                ? KrConstants.Ui.ExtendedTaskForm
                : null;
            var taskModel = navigator.TaskModel;
            if (formName == null)
            {
                return new TaskActionViewModel(
                    caption,
                    () => TaskNavigationHelper.SaveCardAsync(
                        taskModel,
                        (task, ct) =>
                        {
                            task.Action = CardTaskAction.Complete;
                            task.State = CardRowState.Deleted;
                            task.OptionID = optionID;
                            return Task.CompletedTask;
                        }),
                    TaskActionType.Complete,
                    TaskGroupingType.Default,
                    model: taskModel);
            }

            return new TaskActionViewModel(
                TaskNavigationHelper.GetCaptionWithRightArrow(caption),
                async () =>
                {
                    await navigator.NavigateToFormAsync(
                        TaskWorkspaceState.OptionForm,
                        formName,
                        new ObservableCollection<ITaskAction>
                        {
                            this.GenerateTaskAction(navigator, optionID, caption, null, false),
                            TaskNavigationHelper.CreateNavigateBackAction(navigator),
                        }).ConfigureAwait(false);

                    var newTaskModel = navigator.TaskModel;
                    if (newTaskModel.Blocks.TryGet(KrConstants.Ui.ExtendedTaskForm, out var blockViewModel))
                    {
                        await DispatcherHelper.InvokeInUIAsync(() =>
                        {
                            IControlViewModel controlViewModel;
                            if ((controlViewModel = blockViewModel.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.MessageLabel)) != null
                                && controlViewModel is LabelViewModel label)
                            {
                                if (!string.IsNullOrEmpty(message))
                                {
                                    label.ControlVisibility = Visibility.Visible;
                                    label.Text = message;
                                }
                                else
                                {
                                    label.ControlVisibility = Visibility.Collapsed;
                                }
                            }

                            if ((controlViewModel = blockViewModel.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.Comment)) != null)
                            {
                                controlViewModel.ControlVisibility = showComment
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            }
                        });
                    }
                },
                TaskActionType.NavigateToForm,
                TaskGroupingType.Default,
                model: taskModel);
        }

        private void ModifyTaskAndAttachHandlers(TaskViewModel taskViewModel)
        {
            ICardModel taskModel = taskViewModel.TaskModel;
            if (taskModel.CardType.ID != DefaultTaskTypes.KrApproveTypeID &&
                taskModel.CardType.ID != DefaultTaskTypes.KrAdditionalApprovalTypeID &&
                taskModel.CardType.ID != DefaultTaskTypes.KrSigningTypeID
                || taskModel.CardTask.IsLockedEffective)
            {
                return;
            }

            // скрываем блок с комментариями в текущем представлении
            if (taskModel.Card.Sections[KrConstants.KrCommentsInfo.Virtual].Rows.Count == 0
                && taskModel.Blocks.TryGet("CommentsBlockShort", out IBlockViewModel commentBlock))
            {
                //Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                commentBlock.BlockVisibility = Visibility.Collapsed;
                taskModel.MainForm.RearrangeSelf();
            }

            if (taskModel.CardType.ID == DefaultTaskTypes.KrApproveTypeID
                || taskModel.CardType.ID == DefaultTaskTypes.KrAdditionalApprovalTypeID)
            {
                // скрываем блок с заданиями доп согласования в текущем представлении
                if (taskModel.Card.Sections[KrConstants.KrAdditionalApprovalInfo.Virtual].Rows.Count == 0
                    && taskModel.Blocks.TryGet("AdditionalApprovalBlockShort", out IBlockViewModel additionalApprovalBlock))
                {
                    //Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                    additionalApprovalBlock.BlockVisibility = Visibility.Collapsed;
                    taskModel.MainForm.RearrangeSelf();
                }
            }

            // в начальной форме задания гарантированно нет поля "Комментарий",
            // которое может понадобиться скрыть для варианта "Согласовать"

            // скрываем блок с комментариями в других представлениях
            taskViewModel.WorkspaceChanged += async (s, e) =>
            {
                // получить блок по taskModel.Blocks.TryGet нельзя, т.к. для формы откладывания заданий будет свой экземпляр блока,
                // при этом TryGet вернёт блок для предыдущей формы карточки

                IFormViewModel form = ((TaskViewModel) s).Workspace.Form;
                if (form == null)
                {
                    return;
                }

                ReadOnlyObservableCollection<IBlockViewModel> blocks = form.Blocks;

                bool rearrangeForm = false;

                IBlockViewModel innerCommentBlock;
                if (taskModel.Card.Sections["KrCommentsInfoVirtual"].Rows.Count == 0
                    && (innerCommentBlock = blocks.FirstOrDefault(x => x.Name == "CommentsBlockShort")) != null)
                {
                    //Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                    innerCommentBlock.BlockVisibility = Visibility.Collapsed;
                    rearrangeForm = true;
                }

                if (taskModel.CardType.ID == DefaultTaskTypes.KrApproveTypeID
                    || taskModel.CardType.ID == DefaultTaskTypes.KrAdditionalApprovalTypeID)
                {
                    IBlockViewModel innerAdditionalApprovalBlock;
                    if (taskModel.Card.Sections["KrAdditionalApprovalInfoVirtual"].Rows.Count == 0
                        && (innerAdditionalApprovalBlock = blocks.FirstOrDefault(x => x.Name == "AdditionalApprovalBlockShort")) != null)
                    {
                        //Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                        innerAdditionalApprovalBlock.BlockVisibility = Visibility.Collapsed;
                        rearrangeForm = true;
                    }

                    using (e.Defer())
                    {
                        // скрываем поле "Комментарий" для варианта завершения "Согласовать"
                        IBlockViewModel approvalCommentBlock;
                        if (form.Name == "Approve"
                            && await this.CommentIsHiddenForApprovalAsync()
                            && (approvalCommentBlock = blocks.FirstOrDefault(x => x.Name == "CommentBlock")) != null)
                        {
                            approvalCommentBlock.BlockVisibility = Visibility.Collapsed;
                            rearrangeForm = true;
                        }

                        if (rearrangeForm)
                        {
                            form.RearrangeSelf();
                        }
                    }
                }
            };

            // подписываемся на построение метаинформации и виртуальной карточки для формы откладывания задания
            taskViewModel.PostponeMetadataInitializing += PostponeMetadataInitializing;
            taskViewModel.PostponeContentInitializing += PostponeContentInitializing;
        }


        private static async void PostponeMetadataInitializing(object sender, TaskViewModelEventArgs e)
        {
            using (e.Defer())
            {
                ICardMetadata targetMetadata = e.Task.PostponeMetadata;
                CardType targetType = (await targetMetadata.GetCardTypesAsync())[0];
                CardTypeForm targetForm = targetType.Forms[0];
                SealableObjectList<CardTypeBlock> targetBlocks = targetForm.Blocks;

                // удаляем блок с информацией по заданию, т.к. он будет скопирован ниже из основной формы
                if (targetBlocks.Count > 0)
                {
                    targetBlocks.RemoveAt(0);
                }

                // копируем все блоки из основной формы задания в начало формы откладывания, потом сортируем по Order
                CardType sourceType = e.Task.TaskModel.CardType.DeepClone();
                CardTypeForm sourceForm = sourceType;
                sourceForm.Blocks.CopyToTheBeginningOf(targetBlocks);

                // копируем настройки формы из основной формы задания в форму откладывания
                targetForm.FormSettings.Clear();
                StorageHelper.Merge(sourceForm.FormSettings, targetForm.FormSettings);

                // копируем метаинформацию по виртуальной таблице для формы откладывания
                CardMetadataSectionCollection taskSections = await e.Task.TaskModel.CardMetadata.GetSectionsAsync();
                CardMetadataSectionCollection targetSections = null;
                if (taskSections.TryGetValue("KrCommentsInfoVirtual", out CardMetadataSection metadataSection))
                {
                    (targetSections = await targetMetadata.GetSectionsAsync()).Add(metadataSection.DeepClone());

                    CardTypeSchemeItem sourceItem = sourceType.SchemeItems
                        .FirstOrDefault(x => x.SectionID == metadataSection.ID);
                    if (sourceItem != null)
                    {
                        targetType.SchemeItems.Add(sourceItem);
                    }
                }

                if (taskSections.TryGetValue("KrAdditionalApprovalInfoVirtual", out metadataSection))
                {
                    (targetSections ?? await targetMetadata.GetSectionsAsync()).Add(metadataSection.DeepClone());

                    CardTypeSchemeItem sourceItem = sourceType.SchemeItems
                        .FirstOrDefault(x => x.SectionID == metadataSection.ID);
                    if (sourceItem != null)
                    {
                        targetType.SchemeItems.Add(sourceItem);
                    }
                }
            }
        }


        private static void PostponeContentInitializing(object sender, TaskFormContentViewModelEventArgs e)
        {
            if (e.Task.TaskModel.Card.Sections.TryGetValue("KrCommentsInfoVirtual", out CardSection sourceSection)
                && e.Card.Sections.TryGetValue("KrCommentsInfoVirtual", out CardSection targetSection))
            {
                targetSection.Set(sourceSection);
            }

            if (e.Task.TaskModel.Card.Sections.TryGetValue("KrAdditionalApprovalInfoVirtual", out sourceSection)
                && e.Card.Sections.TryGetValue("KrAdditionalApprovalInfoVirtual", out targetSection))
            {
                targetSection.Set(sourceSection);
            }
        }

        #endregion

        #region Base Overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            ICardModel model = context.Model;
            if (!(model.MainForm is DefaultFormTabWithTasksViewModel formWithTasks)
                || model.CardType.Flags.HasNot(CardTypeFlags.AllowTasks))
            {
                return;
            }

            foreach (TaskViewModel taskViewModel in formWithTasks.Tasks.OfType<TaskViewModel>())
            {
                this.ModifyTaskAndAttachHandlers(taskViewModel);
                await this.ModifyUniversalTaskAsync(taskViewModel);
                await ModifyDialogTaskAsync(taskViewModel);
            }

            formWithTasks.HiddenTaskCreated += async (s, e) =>
            {
                this.ModifyTaskAndAttachHandlers(e.Task);

                using (e.Defer())
                {
                    await this.ModifyUniversalTaskAsync(e.Task);
                    await ModifyDialogTaskAsync(e.Task);
                }
            };
        }

        #endregion
    }
}