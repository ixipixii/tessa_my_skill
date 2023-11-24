import { WorkplaceViewComponentExtension, TreeItemExtension } from 'tessa/ui/views/extensions';
import { IWorkplaceViewComponent, IWorkplaceViewModel } from 'tessa/ui/views';
import { ITreeItem } from 'tessa/ui/views/workplaces/tree';
/**
 * Обновление представлений при смене окна
 */
export class PnrRefresh extends WorkplaceViewComponentExtension {

  public getExtensionName(): string {
    return 'PnrRefresh';
  }
  public shouldExecute(model: IWorkplaceViewComponent) {
    return model.view != null && model.view.metadata != null;
  }
  public initialize(model: IWorkplaceViewComponent) {
    if (model) {
      let view = model.workplace as IWorkplaceViewModel;
      view.activated.add(() => {
        model.refreshView();
        model.refresh();
      });
    }
  }
}
export class PnrRefreshTreeItemExtension extends TreeItemExtension {
  public getExtensionName(): string {
    return 'PnrRefreshTreeItemExtension';
  }
  public shouldExecute(model: ITreeItem) {
    return model != null;
  }
  public initialize(model: ITreeItem) {
    if (model) {
      let view = model.workplace as IWorkplaceViewModel;
      view.activated.add(() => {
        model.refreshNode();
      });
    }
  }
}