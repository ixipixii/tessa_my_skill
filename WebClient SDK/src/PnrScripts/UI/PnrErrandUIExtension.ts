import { Guid } from 'tessa/platform';
import { CardUIExtension, ICardUIExtensionContext } from 'tessa/ui/cards';
import { PnrCardTypes } from '../Shared/PnrCardTypes';

export class PnrErrandUIExtension extends CardUIExtension {
  public initialized(context: ICardUIExtensionContext) {
    if (!Guid.equals(context.card.typeId, PnrCardTypes.PnrErrandTypeID)) {
      return;
    }
    // const errandsControllers = context.card.sections.tryGet("PnrErrandsControllers");
    // TODO добавить добавление автора карточки в коллекционную секцию PnrErrandsControllers
  }
}