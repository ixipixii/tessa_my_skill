using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    public sealed class KrRecalcStagesUIExtension : CardUIExtension
    {
        #region fields

        private readonly IKrTypesCache typesCache;

        #endregion

        #region constructor

        public KrRecalcStagesUIExtension(IKrTypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        #endregion

        #region base overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            // выходим, если не включены процессы маршрутов
            var cardModel = context.Model;
            var usedComponents = KrComponentsHelper.GetKrComponents(cardModel.Card, this.typesCache);
            if (usedComponents.HasNot(KrComponents.Routes))
            {
                return;
            }

            if (cardModel.Controls.TryGet(KrConstants.Ui.KrApprovalStagesControlAlias) is GridViewModel approvalStagesTable
                && context.Model.Flags.HasNot(CardModelFlags.Disabled)
                && KrToken.TryGet(context.Card.Info)?.HasPermission(KrPermissionFlagDescriptors.CanFullRecalcRoute) == true)
            {
                IUIContext uiContext = context.UIContext;

                bool hasStagePositions = cardModel.Card.HasStagePositions(false);
                bool newCard = cardModel.Card.StoreMode == CardStoreMode.Insert;

                approvalStagesTable.LeftButtons.Add(
                    new UIButton("$CardTypes_Buttons_RecalcApprovalStages",
                        btn =>
                        {
                            ICardEditorModel editor = uiContext.CardEditor;
                            if (editor?.OperationInProgress == false)
                            {
                                var info = new Dictionary<string, object>();
                                info.SetRecalcFlag();

                                var mode = hasStagePositions || newCard
                                    ? InfoAboutChanges.ChangesListToValidationResult
                                    : InfoAboutChanges.ChangesListToValidationResult | InfoAboutChanges.ChangesInHiddenStages;
                                info.SetInfoAboutChanges(mode);

                                // это асинхронный вызов, но после его завершения ничего не нужно, так что нет await
                                editor.SaveCardAsync(uiContext, info);
                            }
                        }));
            }
        }

        #endregion
    }
}