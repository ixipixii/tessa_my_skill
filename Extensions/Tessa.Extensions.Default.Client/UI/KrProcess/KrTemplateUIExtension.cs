using System.Threading.Tasks;
using System.Windows;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.UI.Cards;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    /// <summary>
    /// При редактировании карточки в шаблоне скрываем все блоки, кроме <see cref="Ui.KrDisclaimerBlockAlias"/>,
    /// который наоборот отображаем.
    /// </summary>
    public sealed class KrTemplateUIExtension :
        CardUIExtension
    {
        #region Constructors

        public KrTemplateUIExtension(IKrTypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache typesCache;

        #endregion

        #region Private Methods

        private bool CardIsAvailableForExtension(ICardModel model)
        {
            if (KrProcessSharedHelper.DesignTimeCard(model.CardType.ID))
            {
                return true;
            }

            KrComponents usedComponents = KrComponentsHelper.GetKrComponents(model.Card, this.typesCache);

            return usedComponents.Has(KrComponents.Routes);
        }

        #endregion

        #region Base Overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            if (!context.Model.Flags.Has(CardModelFlags.EditTemplate)
                || !this.CardIsAvailableForExtension(context.Model)
                || !context.Model.Forms.TryGet(Ui.KrApprovalProcessFormAlias, out IFormViewModel routesForm))
            {
                return;
            }

            foreach (IBlockViewModel block in routesForm.Blocks)
            {
                block.BlockVisibility = block.Name == Ui.KrDisclaimerBlockAlias
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            routesForm.RearrangeSelf();
        }

        #endregion
    }
}
