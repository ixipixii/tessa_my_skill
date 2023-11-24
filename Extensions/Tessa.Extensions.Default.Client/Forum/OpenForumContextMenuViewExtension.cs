using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Forums;
using Tessa.Forums.Models;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Controls.Forums;
using Tessa.UI.Menu;
using Tessa.UI.Views;
using Tessa.UI.Views.Content;
using Tessa.Views.Metadata.Criteria;

namespace Tessa.Extensions.Default.Client.Forum
{
    public sealed class OpenForumContextMenuViewExtension : IWorkplaceViewComponentExtension
    {
        #region Private Fields

        private readonly IForumProvider forumProvider;

        private readonly IForumDialogManager forumDialogManager;

        private readonly ISession session;

        private readonly IForumPermissionsProvider permissionsProvider;
        
        #endregion
        
        #region Constructors

        public OpenForumContextMenuViewExtension(
            IForumDialogManager forumDialogManager, 
            ISession session, 
            IForumProvider forumProvider,
            IForumPermissionsProvider permissionsProvider)
        {
            this.forumDialogManager = forumDialogManager;
            this.session = session;
            this.forumProvider = forumProvider;
            this.permissionsProvider = permissionsProvider;
        }

        #endregion

        #region IWorkplaceViewComponentExtension Implements

        public void Clone(IWorkplaceViewComponent source, IWorkplaceViewComponent cloned, ICloneableContext context)
        {
        }

        public void Initialize(IWorkplaceViewComponent model)
        {
            if (this.session.ApplicationID == ApplicationIdentifiers.TessaAdmin || !string.IsNullOrEmpty(model.RefSection))
            {
                // в TessaAdmin не будем ничего менять, т.к. там предпросмотр представлений
                // в режиме выборки не нужно создавать новую карточку.
                return;
            }

            model.ContextMenuGenerators.AddRange(this.GetParticipantsMenuAction());
        }

        public void Initialized(IWorkplaceViewComponent model)
        {
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Возвращает действие создающее элемент меню
        /// </summary>
        /// <returns>
        /// Действие создающее элемент меню
        /// </returns>
        private Action<ViewContextMenuContext> GetParticipantsMenuAction()
        {
            return c =>
            {
                if (!string.IsNullOrEmpty(c.ViewContext.RefSection))
                {
                    return;
                }

                var viewContext = c.ViewContext;

                IEnumerable<IDictionary<string, object>> selectedRowsEnumerable;
                IDictionary<string, object>[] selectedRows;

                if ((selectedRowsEnumerable = viewContext.SelectedRows) == null
                    || (selectedRows = selectedRowsEnumerable.ToArray()).Length == 0)
                {
                    return;
                }

                var items = this.TryGetRows(selectedRows);
                if(items.Any())
                {
                    c.MenuActions.Add(
                        new MenuAction(
                            "ChangeParticipant",
                            "$Forum_MenuAction_ChangeParticipants",
                            Icon.Empty,
                            new DelegateCommand(
                                async o => await c.MenuContext.UIContextExecutorAsync(
                                    async (ctx, ct) => await this.ChangeParticipantsCommandAsync(viewContext, items, ct)))));

                    c.MenuActions.Add(
                        new MenuAction(
                            "RemoveParticipants",
                            "$Forum_MenuAction_RemoveParticipants",
                            Icon.Empty,
                            new DelegateCommand(
                                async o => await c.MenuContext.UIContextExecutorAsync(
                                    async (ctx, ct) => await this.RemoveParticipantsCommandAsync(viewContext, items, ct)))));
                }
            };
        }

        private bool HasModeratorPermissions(IViewContext context)
        {
            var participantType = (ParticipantTypes) context.Parameters[1].CriteriaValues[0].Values[0].Value;
            return participantType != ParticipantTypes.Participant && participantType != ParticipantTypes.ParticipantFromRole;
        }
        
        private bool HasSuperModeratorPermissions(IViewContext context)
        {
            return context.Parameters[2].CriteriaValues[0].CriteriaName == CriteriaOperatorConst.IsTrue;
        }

        private Guid GetCardID(IViewContext context)
        {
            return (Guid)context.Parameters[3].CriteriaValues[0].Values[0].Value;
        }

        private async Task RemoveParticipantsCommandAsync(IViewContext context, List<ForumOperationItem> items, CancellationToken cancellationToken = default)
        {
            Check.ArgumentNotNull(items, nameof(items));
            Check.ArgumentNotNull(context, nameof(context));

            if (!this.HasModeratorPermissions(context))
            {
                TessaDialog.ShowError(
                    string.Format(
                    LocalizationManager.Localize(
                        "$Forum_UI_Cards_RemoveParticipants_NoRequiredPermissions"),
                        ForumHelper.ConcatUsersName(items.Select(i=> i.RoleName).ToList(), ",")));
                return;
            }

            var currentUserID = this.session.User.ID;
            if (!items.TryFirst(i=>i.CardID == currentUserID, out  _))
            {
                try
                {
                    var participants = items.Select(i => i.RoleID).ToList();
                    
                    ForumResponse forumResponse = await this.RemoveParticipants(
                        items,
                        currentUserID,
                        participants, 
                        this.GetCardID(context),
                        this.HasSuperModeratorPermissions(context), cancellationToken);
                    
                    if (forumResponse.ValidationResult.IsSuccessful())
                    {
                        if (context.CanRefreshView())
                        {
                            await context.RefreshViewAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        TessaDialog.ShowNotEmpty(forumResponse.ValidationResult);
                    }
                }
                catch (Exception ex)
                {
                    TessaDialog.ShowException(ex);
                }
            }
            else
            {
                TessaDialog.ShowError("$Forum_UI_Cards_RemoveParticipants_CantRemoveCurrentParticipant");
            }
        }

        private async Task<ForumResponse> RemoveParticipants(List<ForumOperationItem> items, Guid currentUserID, List<Guid> participants, Guid cardID, bool isSuperModerator = false, CancellationToken cancellationToken = default)
        {
            ForumResponse response = new ForumResponse();

            // проверяем что строки из с одним RoleType либо это роли, либо участники.
            var groupedItems = items.GroupBy(i => i.RoleType == 1);
            if (groupedItems.Count() > 1)
            {
                response = new ForumResponse();
                response.ValidationResult.Add(
                    ForumValidationKey.PermissionError,
                    ValidationResultType.Error,
                    LocalizationManager.Localize("$Forum_UI_Cards_RemoveParticipants_CannotDeleteParticipantsAndRoles"));

                return response;
            }

            var item = items[0];

            if (item.RoleType != 1) // 1 - участник.Запрос к разным табличкам
            {
                response = await this.ProcessingRemoveRoleAsync(items, currentUserID, participants, cardID, isSuperModerator, cancellationToken);
            }
            else
            {
                var result = await TessaDialog.ConfirmAsync(
                    string.Format(
                    LocalizationManager.Localize("$Forum_UI_Cards_RemoveParticipant_ConfirmSingle"),ForumHelper.ConcatUsersName(items.Select(u => u.RoleName).ToList(), ",")));
                if (result)
                {
                    response = await this.forumProvider.RemoveParticipantsAsync(item.TopicID, currentUserID, participants, cancellationToken).ConfigureAwait(false);
                }
            }

            return response;
        }

        private async Task<ForumOperationItem> ProcessingChangeRoleAsync(ForumOperationItem item, Guid cardID, bool isSuperModerator = false, CancellationToken cancellationToken = default)
        {
            // для редактирования ролей нужны права супермодератора
            // смотрим есть ли у нас права из карточки
            if (isSuperModerator)
            {
                return await this.forumDialogManager.ChangeRoleParticipantsShowDialogAsync(item, cancellationToken);
            }
            
            // если нет, то спрашиваем у пользователя, и пробуем получить права с сервера
            var result = await TessaDialog.ConfirmAsync("$Forum_UI_Cards_ChangeParticipants_TryGetSupermoderatorPermission");
            if (result)
            {
                (bool isEnableSuperModeratorMode, ValidationResult vr) = await this.permissionsProvider.CheckHasPermissionIsSuperModeratorAsync(
                    null, 
                    cardID, 
                    cancellationToken);
                         
                if (!vr.IsSuccessful)
                {
                    await TessaDialog.ShowNotEmptyAsync(vr);
                    return null;
                }

                if (isEnableSuperModeratorMode)
                {
                    return await this.forumDialogManager.ChangeRoleParticipantsShowDialogAsync(item, cancellationToken);
                }
            }

            return null;
        }

        private async Task<ForumResponse> ProcessingRemoveRoleAsync(List<ForumOperationItem> items, Guid currentUserID, List<Guid> participants, Guid cardID, bool isSuperModerator = false, CancellationToken cancellationToken = default)
        {
            // для редактирования ролей нужны права супермодератора
            // смотрим есть ли у нас права из карточки
            if (isSuperModerator)
            {
                var confirmed = await TessaDialog.ConfirmAsync(
                    string.Format(
                        LocalizationManager.Localize("$Forum_UI_Cards_RemoveParticipant_ConfirmSingle"), ForumHelper.ConcatUsersName(items.Select(u => u.RoleName).ToList(), ",")));
                if (confirmed)
                {
                    return await this.forumProvider.RemoveRolesAsync(items[0].TopicID, currentUserID, participants, cancellationToken).ConfigureAwait(false);
                }

                return new ForumResponse();
            }
            
            var response = new ForumResponse();
            // если нет, то спрашиваем у пользователя, и пробуем получить права с сервера
            var result = await TessaDialog.ConfirmAsync("$Forum_UI_Cards_ChangeParticipants_TryGetSupermoderatorPermission");
            if (result)
            {
                (bool isEnableSuperModeratorMode, ValidationResult vr) = await this.permissionsProvider.CheckHasPermissionIsSuperModeratorAsync(
                    null, 
                    cardID, 
                    cancellationToken);
                         
                if (!vr.IsSuccessful)
                {
                    response = new ForumResponse();
                    response.ValidationResult.Add(vr);
                    return response;
                }

                if (isEnableSuperModeratorMode)
                {
                    result = await TessaDialog.ConfirmAsync(
                        string.Format(
                            LocalizationManager.Localize("$Forum_UI_Cards_RemoveParticipant_ConfirmSingle"), ForumHelper.ConcatUsersName(items.Select(u => u.RoleName).ToList(), ",")));
                            
                    if (result)
                    {
                        response = await this.forumProvider.RemoveRolesAsync(items[0].TopicID, currentUserID, participants, cancellationToken).ConfigureAwait(false);
                        return response;
                    }
                }
            }
            return response;
        }
        

        private async Task ChangeParticipantsCommandAsync(IViewContext context, List<ForumOperationItem> items, CancellationToken cancellationToken = default)
        {
            Check.ArgumentNotNull(items, nameof(items));
            Check.ArgumentNotNull(context, nameof(context));

            if (!this.HasModeratorPermissions(context))
            {
                TessaDialog.ShowError(
                    string.Format(
                        LocalizationManager.Localize(
                            "$Forum_UI_Cards_RemoveParticipants_NoRequiredPermissions"),
                        ForumHelper.ConcatUsersName(items.Select(i=> i.RoleName).ToList(), ",")));
                return;
            }

            var currentUserID = this.session.User.ID;
            if (items.TryFirst(i => i.CardID == currentUserID, out _))
            {
                await TessaDialog.ShowErrorAsync("$Forum_UI_Cards_ChangeParticipants_СannotEditYourself").ConfigureAwait(false);
                return;
            }

            //пока делаем так, что можно редактировать только по одной строке.
            if (items.Count > 1)
            {
                await TessaDialog.ShowErrorAsync("$Forum_UI_Cards_ChangeParticipants_YouCanEditOnlyOneEntry").ConfigureAwait(false);
                return;
            }

            var item = items[0];

            ForumOperationItem newItem;
            if (item.RoleType != 1) // 1 - участник.Запрос к разным табличкам
            {
                newItem = await this.ProcessingChangeRoleAsync(item, this.GetCardID(context), this.HasSuperModeratorPermissions(context), cancellationToken);
            }
            else
            {
                newItem = await this.forumDialogManager.ChangeParticipantsShowDialogAsync(item, cancellationToken);
            }
            
            //нажали отмена
            if (newItem == null)
            {
                return;
            }

            var participants = new List<Guid>() { newItem.RoleID };

            var forumResponse = newItem.RoleType != 1 ? // 1 - участник.Запрос к разным табличкам
                await this.forumProvider.UpdateRolesAsync(newItem.TopicID, participants, newItem.ReadOnly, newItem.Subscribed, cancellationToken).ConfigureAwait(false) :
                await this.forumProvider.UpdateParticipantsAsync(newItem.TopicID, participants, newItem.ReadOnly, newItem.ParticipantType, newItem.Subscribed, cancellationToken).ConfigureAwait(false);

            if (forumResponse.ValidationResult.IsSuccessful())
            {
                if (context.CanRefreshView())
                {
                    await context.RefreshViewAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                await TessaDialog.ShowNotEmptyAsync(forumResponse.ValidationResult);
            }
        }

        private List<ForumOperationItem> TryGetRows(IDictionary<string, object>[] rows)
        {
            var result = new List<ForumOperationItem>();
            foreach (var row in rows)
            {
                result.Add(this.TryGetSelectedItemsFromViewContext(row));
            }
            return result;
        }

        private ForumOperationItem TryGetSelectedItemsFromViewContext(IDictionary<string, object> selectedRows)
        {
            var forumItem = new ForumOperationItem(Guid.Parse(selectedRows["RoleID"].ToString()), (string) selectedRows["RoleName"]);
            forumItem.RoleType = (int) selectedRows["TypeID"];
            forumItem.TopicID = Guid.Parse(selectedRows["TopicID"].ToString());
            forumItem.ReadOnly = (bool) selectedRows["ReadOnly"];
            forumItem.ParticipantType = (ParticipantTypes) selectedRows["TypeParticipant"];
            forumItem.Subscribed = (bool) selectedRows["Subscribed"];

            return forumItem;
        }

        #endregion
    }
}