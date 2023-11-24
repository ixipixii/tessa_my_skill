using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Forums;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Runtime;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Forms;
using Tessa.UI.Controls;
using Tessa.UI.Controls.Forums;
using Tessa.UI.Views;

namespace Tessa.Extensions.Default.Client.Forum
{
    public sealed class OpenTopicOnDoubleClickExtension :
        IWorkplaceViewComponentExtension
    {
        private readonly ISession session;
        private readonly IUIHost uiHost;

        public OpenTopicOnDoubleClickExtension(ISession session, IUIHost uiHost)
        {
            this.session = session;
            this.uiHost = uiHost;
        }


        public void Clone(IWorkplaceViewComponent source, IWorkplaceViewComponent cloned, ICloneableContext context)
        {
        }

        public void Initialize(IWorkplaceViewComponent model)
        {
            if (this.session.ApplicationID == ApplicationIdentifiers.TessaAdmin
                || !string.IsNullOrEmpty(model.RefSection))
            {
                // в TessaAdmin не будем ничего менять, т.к. там предпросмотр представлений
                // в режиме выборки не нужно создавать новую карточку.
                return;
            }
            model.DoubleClickAction = new DoubleClickAction(this.uiHost);
        }

        public void Initialized(IWorkplaceViewComponent model)
        {
        }
        
        private sealed class DoubleClickAction : OpenCardDoubleClickAction
        {
            private readonly IUIHost uiHost;
            public DoubleClickAction(IUIHost uiHost)
            {
                this.uiHost = uiHost;
            }
            protected override async Task OpenCardAsync(Guid cardID, string displayValue, IUIContext context, ViewDoubleClickInfo info)
            {
                var selectedRow = context.ViewContext.SelectedRow;
                if (selectedRow != null)
                {
                    if(selectedRow.TryGetValue("TopicID", out var topicID))
                    {
                        using ISplash splash = TessaSplash.Create(TessaSplashMessage.OpeningCard);
                        var objContext = await this.uiHost.OpenCardAsync(cardID, 
                            options: new OpenCardOptions
                            {
                                DisplayValue = displayValue,
                                UIContext = context,
                                Splash = splash,
                                CardModifierActionAsync = openingContext => this.CardModifierActionAsync(openingContext, topicID)
                            }, 
                            cancellationToken: CancellationToken.None);

                        if (objContext != null)
                        {
                            ICardModel cardModel = objContext.Context.CardEditor.CardModel;
                            if (cardModel != null)
                            {
                                var mainForm = (DefaultFormMainViewModel)cardModel.MainForm;
                                if (mainForm.Tabs.TryGet(ForumHelper.ForumTabName, out var tabTopics))
                                {
                                    mainForm.SelectedTab = tabTopics;

                                    if (tabTopics.Blocks[0].Controls[0] is ForumControlViewModel forumControlViewModel)
                                    {
                                        var data = forumControlViewModel.ForumViewModel.ForumClientCachedDataManager.GetForumData();
                                        await forumControlViewModel.SelectTopicAsync(
                                            (Guid)topicID, 
                                            data.ReadTopicIDList.TryFirst(top => top.TopicID == (Guid)topicID, out var userStat)
                                                ? userStat.LastReadMessageTime
                                                : null,
                                            CancellationToken.None);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private Task CardModifierActionAsync(ICardEditorOpeningContext cardEditorCreationContext, object topicID)
            {
                cardEditorCreationContext.Card.Info.Add(ForumHelper.TopicIDKey, topicID);
                return Task.CompletedTask;
            }
        }
    }
}
