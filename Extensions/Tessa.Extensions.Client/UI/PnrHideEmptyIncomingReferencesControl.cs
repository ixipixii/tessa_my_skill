using System.Linq;
using System.Threading.Tasks;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls.AutoComplete;
using Tessa.UI.Cards.Forms;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrHideEmptyIncomingReferencesControl : CardUIExtension
    {
        public override async Task Initialized(ICardUIExtensionContext context)
        {
            ICardModel model = context.Model;

            if (!(model.MainForm is DefaultFormMainViewModel view))
            {
                return;
            }

            IBlockViewModel block = view.Blocks.FirstOrDefault(x => x.Name == "LinksBlock");
            if (block == null)
            {
                return;
            }

            IControlViewModel control = block.Controls.FirstOrDefault(x => x.Name == "IncomingRefsControl");

            if (control is AutoCompleteTableViewModel autoComplete && autoComplete.Items.Count == 0)
            {
                control.ControlVisibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
