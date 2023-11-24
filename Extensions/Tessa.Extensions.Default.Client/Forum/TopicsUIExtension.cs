using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Forums;
using Tessa.Forums.Models;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Controls.Forums;
using Tessa.UI.Windows;
using Tessa.Views.Parser.SyntaxTree.Workplace;

namespace Tessa.Extensions.Default.Client.Forum
{
    /// <summary>
    /// Класс для управления вкладкой обсуждения
    /// </summary>
    public sealed class TopicsUIExtension : CardUIExtension
    {
        #region Fields
        
        private readonly IForumDialogManager forumDialogManager;
        private ICardUIExtensionContext context;
        private readonly IDocumentTabManager documentTabManager;
        private readonly IWorkplaceInterpreter interpreter;

        private ForumControlViewModel forumControlViewModel;

        #endregion

        #region Constructors

        public TopicsUIExtension(
            IForumDialogManager forumDialogManager,
            IDocumentTabManager documentTabManager,
            IWorkplaceInterpreter interpreter)
        {
            this.documentTabManager = documentTabManager;
            this.interpreter = interpreter;
            this.forumDialogManager = forumDialogManager;
        }

        #endregion

        #region BaseOverrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            if (context.Model.Forms.TryGet(ForumHelper.ForumTabName, out var forumTab))
            {
                var topicsBlock = forumTab.Blocks.FirstOrDefault(b => b.Name == ForumHelper.TopicsBlockName);
                if (topicsBlock != null)
                {
                    this.forumControlViewModel = (topicsBlock.Controls[0] as ForumControlViewModel);
                    // обновляем всю модель при выборе вкладки
                    if (this.forumControlViewModel != null)
                    {
                        if (forumControlViewModel.Content is ForumLicenseNotExistViewModel)
                        {
                            return;
                        }

                        this.context = context;
                        
                        this.forumControlViewModel.OpenParticipants(this.OpenParticipantsView);
                        this.forumControlViewModel.CheckAddTopicPermission(this.CheckAddTopicPermissionsAsync);
                        this.forumControlViewModel.CheckSuperModeratorPermissionAsync(this.CheckSuperModeratorPermissionsAsync);
                        
                        // Если в инфо есть topicID значит мы пришли из OpenTopicOnDoubleClickExtension. и Это ID выбронного топика
                        if (context.Model.Card.Info.TryGetValue(ForumHelper.TopicIDKey, out var t) && t is Guid topicID)
                        {
                            var data = this.forumControlViewModel.ForumViewModel.ForumClientCachedDataManager.GetForumData();
                            await this.forumControlViewModel.SelectTopicAsync(
                                topicID, data.ReadTopicIDList.TryFirst(
                                    top => top.TopicID == topicID, out var userStat) 
                                    ? userStat.LastReadMessageTime 
                                    : null,
                                context.CancellationToken);
                
                            context.Model.Card.Info.Remove(ForumHelper.TopicIDKey);
                        }
                    }
                }
            }
        }

        #endregion

        #region PrivataMembers
        
        private async Task CheckAddTopicPermissionsAsync(CancellationToken cancellationToken = default)
        {
            await OpenMarkedCardAsync(
                this.context.UIContext, 
                KrPermissionsHelper.CalculateAddTopicPermissions, 
                null, //Не требуем подтверждения дейтсвия если не было изменений
                (cardIsNew) => cardIsNew 
                    ? (TessaDialog.Confirm("$KrTiles_EditModeConfirmation") ? (bool?)true : null) 
                    : TessaDialog.ConfirmWithCancel("$KrTiles_EditModeConfirmation"),
                () => this.AddTopicShowDialogAsync(this.context),
                cancellationToken: cancellationToken);
        }

        private async Task CheckSuperModeratorPermissionsAsync(CancellationToken cancellationToken = default)
        {
            await OpenMarkedCardAsync(
                this.context.UIContext, 
                KrPermissionsHelper.CalculateSuperModeratorPermissions, 
                null, //Не требуем подтверждения дейтсвия если не было изменений
                (cardIsNew) => cardIsNew 
                    ? (TessaDialog.Confirm("$KrTiles_EditModeConfirmation") ? (bool?)true : null) 
                    : TessaDialog.ConfirmWithCancel("$KrTiles_EditModeConfirmation"),
                async () => await this.SuperModeratorPermissionsMessageAsync(this.context),
                cancellationToken: cancellationToken);
        }

        private async Task<bool> AddTopicShowDialogAsync(ICardUIExtensionContext context)
        {
            if (this.forumControlViewModel.PermissionsProvider.IsEnableAddTopic(context.UIContext.CardEditor.CardModel.Card))
            {
                await this.forumDialogManager.AddTopicShowDialogAsync(context.Card.ID, this.forumControlViewModel.ForumViewModel.CancellationToken);
                return true;
            }
            else
            {
                TessaDialog.ShowError("$Forum_Permission_NoPermissionToAddTopic");
                return false;
            }
        }

        private async Task<bool> SuperModeratorPermissionsMessageAsync(ICardUIExtensionContext context)
        {
            if (this.forumControlViewModel.PermissionsProvider.IsEnableSuperModeratorMode(context.UIContext.CardEditor.CardModel.Card))
            {
                TessaDialog.ShowMessage("$Forum_Permission_SuperModeratorModeOn");
                return false;
            }
            else
            {
                TessaDialog.ShowError("$Forum_Permission_NoRequiredPermissions");
                return false;
            }
        }

        private static async Task OpenMarkedCardAsync(IUIContext context,
            string mark,
            Func<bool> proceedConfirmation,
            Func<bool, bool?> proceedAndSaveCardConfirmation,
            Func<Task<bool>> continuationOnSuccessFunc = null,
            Dictionary<string, object> getInfo = null,
            CancellationToken cancellationToken = default)
        {
            ICardEditorModel editor = context.CardEditor;
            ICardModel model;

            if (editor == null || editor.OperationInProgress || (model = editor.CardModel) == null)
            {
                return;
            }

            bool cardIsNew = model.Card.StoreMode == CardStoreMode.Insert;
            bool hasChanges = cardIsNew || await model.HasChangesAsync(cancellationToken: cancellationToken);
            bool? saveCardBeforeOpening;

            if (hasChanges && proceedAndSaveCardConfirmation != null)
            {
                saveCardBeforeOpening = proceedAndSaveCardConfirmation(cardIsNew);
            }
            //Если не указана функция подтверждения с вариантом отмены - сохраняем карточку
            //если есть подтверждение основного действия
            else if (proceedConfirmation != null && hasChanges)
            {
                saveCardBeforeOpening = proceedConfirmation() ? (bool?) true : null;
            }
            //Если в карточке не было изменений - не вызываем сохранения
            else if (proceedConfirmation != null)
            {
                saveCardBeforeOpening = proceedConfirmation() ? (bool?) false : null;
            }
            //Если не указана функция подтверждения и нет изменений - вызываем основное действие
            //без подтверждения и сохранения
            else
            {
                saveCardBeforeOpening = false;
            }

            if (getInfo == null)
            {
                getInfo = new Dictionary<string, object>(StringComparer.Ordinal);
            }

            if (!string.IsNullOrWhiteSpace(mark))
            {
                getInfo[mark] = BooleanBoxes.True;
            }

            if (!saveCardBeforeOpening.HasValue)
            {
                return;
            }

            if (saveCardBeforeOpening.Value)
            {
                KrToken token = KrToken.TryGet(editor.Info);
                KrToken.Remove(editor.Info);

                var res = await editor.SaveCardAsync(
                    context,
                    info:
                    new Dictionary<string, object>
                    {
                        { KrPermissionsHelper.SaveWithPermissionsCalcFlag, true }
                    },
                    request: new CardSavingRequest(CardSavingMode.KeepPreviousCard),
                    cancellationToken: cancellationToken);
                if (!res)
                {
                    return;
                }

                token?.Set(getInfo);
            }

            Guid cardID = model.Card.ID;
            CardType cardType = model.CardType;

            var sendTaskSucceeded = await editor.OpenCardAsync(
                cardID,
                cardType.ID,
                cardType.Name,
                context,
                getInfo,
                cancellationToken: cancellationToken);

            if (sendTaskSucceeded)
            {
                editor.IsUpdatedServer = true;
            }
            else if (cardIsNew || saveCardBeforeOpening.Value)
            {
                // если карточка новая или была сохранена, а также не удалось выполнить mark-действие при открытии,
                // то у нас будет "висеть" карточка с некорректной версией;
                // её надо обновить, на этот раз без mark'и

                await editor.OpenCardAsync(
                    cardID,
                    cardType.ID,
                    cardType.Name,
                    context,
                    cancellationToken: cancellationToken);
            }

            if ( /*!success || */continuationOnSuccessFunc == null)
            {
                return;
                /*return success;*/
            }

            await using (UIContext.Create(context))
            {
                await continuationOnSuccessFunc();
            }
        }

        private async void OpenParticipantsView(Guid topicID, ParticipantTypes participantTypes, bool isSupermModerator)
        {
            await TopicParticipantsWorkplaceTab.OpenParticipantsViewTabAsync(this.interpreter, this.documentTabManager, topicID, this.context.Card.ID, participantTypes, isSupermModerator );
        }

        #endregion
    }
}