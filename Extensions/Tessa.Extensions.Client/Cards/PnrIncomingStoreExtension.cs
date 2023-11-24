using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.UI;

namespace Tessa.Extensions.Client.Cards
{
    class PnrIncomingStoreExtension : CardStoreExtension
    {
        #region Constructors and Fields

        public PnrIncomingStoreExtension(IUIHost uiHost)
        {
            this.uiHost = uiHost;
        }

        private readonly IUIHost uiHost;
        
        #endregion

        #region Base overrides

        public override async Task AfterRequest(ICardStoreExtensionContext context)
        {
            var card = context.Request.TryGetCard();
            if (card == null || card.StoreMode != CardStoreMode.Insert)
            {
                return;
            }

            var getInfo = context.Response.Info.TryGetValue("similarIncominGuid", out object similarObj);
            if (getInfo && similarObj != null)
            {
                Guid similarCardID = (Guid) similarObj;
                bool? result = TessaDialog.ConfirmWithCancel(
                    "В системе уже имеется карточка с похожими данными. Желаете ее открыть?", "Найден аналогичный документ");
                if (!result.HasValue)
                {
                    return;
                }
                else
                {
                    if (result.Value)
                    {
                        await this.uiHost.OpenCardAsync(similarCardID);
                    }
                }
            }
        }
        #endregion
    }
}
