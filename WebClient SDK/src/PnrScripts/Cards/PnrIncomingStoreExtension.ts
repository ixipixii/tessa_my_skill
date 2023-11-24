import { CardStoreExtension, ICardStoreExtensionContext } from "tessa/cards/extensions";
import { showConfirmWithCancel } from "tessa/ui";
import { PnrCardTypes } from "../Shared/PnrCardTypes";
import { openCard } from 'tessa/ui/uiHost';
import { Guid } from "tessa/platform";

export class PnrIncomingStoreExtension extends CardStoreExtension {
    public async afterRequest(context: ICardStoreExtensionContext) {
        if (!context) {
            return;
        }
        if (!context.cardType
            || !(Guid.equals(context.cardType.id, PnrCardTypes.PnrIncomingTypeID))
            || context.response == null) {
            return;
        }

        let similarCardID = context.response.info['similarIncominGuid'].$value;
        const dialogResult = await showConfirmWithCancel(
            'В системе уже имеется карточка с похожими данными. Желаете ее открыть?',
            'Найден аналогичный документ');
        if (dialogResult !== null || dialogResult !== false) {
            await openCard({cardId: similarCardID});
        }
    }
}
