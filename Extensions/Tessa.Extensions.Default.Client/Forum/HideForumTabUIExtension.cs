﻿using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Forums;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Forms;

namespace Tessa.Extensions.Default.Client.Forum
{
    public class HideForumTabUIExtension : CardUIExtension
    {
        private readonly IKrTypesCache typesCache;
        public HideForumTabUIExtension(IKrTypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            ICardModel model = context.Model;
            if (!(model.MainForm is DefaultFormMainViewModel mainForm))
            {
                return;
            }

            KrComponents usedComponents = KrComponentsHelper.GetKrComponents(model.Card, this.typesCache);
            var krType = await KrProcessSharedHelper.TryGetKrTypeAsync(
                this.typesCache, model.Card, model.Card.TypeID, cancellationToken: context.CancellationToken);

            if (usedComponents.HasNot(KrComponents.UseForum) || krType != null && !krType.UseForum)
            {
                if (model.Forms.TryGet(ForumHelper.ForumTabName, out var forumTab))
                {
                    // если вкладка была выбрана при том, что мы её удаляем, то мы, наверное, переходим из карточки-сателлита в основную карточку
                    // с восстановлением состояния основной карточки; это означает, что нам надо перевыбрать правильную вкладку по индексу
                    int selectedIndexToRestore = ReferenceEquals(mainForm.SelectedTab, forumTab)
                        ? model.Forms.IndexOf(forumTab)
                        : -1;
                    //forumTab.Blocks.RemoveAll(b => b.Name == ForumHelper.TopicsBlockName);

                    model.Forms.Remove(forumTab);
                    if (selectedIndexToRestore >= 0 && selectedIndexToRestore < model.Forms.Count)
                    {
                        mainForm.SelectedTab = model.Forms[selectedIndexToRestore];
                    }
                }
            }
        }
    }
}
