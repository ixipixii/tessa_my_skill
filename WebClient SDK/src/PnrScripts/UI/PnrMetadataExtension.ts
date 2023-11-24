import { ApplicationExtension, IApplicationExtensionMetadataContext } from 'tessa';
import { tryGetFromInfo } from 'tessa/ui';

export class PnrMetadataContainer {

  //#region ctor

  private constructor() {}

  //#endregion

  //#region instance

  private static _instance: PnrMetadataContainer;

  public static get instance(): PnrMetadataContainer {
    if (!PnrMetadataContainer._instance) {
      PnrMetadataContainer._instance = new PnrMetadataContainer();
    }
    return PnrMetadataContainer._instance;
  }

  //#endregion

  //#region fields

  private _isUserInRoleStorage: boolean;

  //#endregion

  //#region props

  public get isUserInRole(): boolean {
    return this._isUserInRoleStorage;
  }

  //#endregion

  //#region methods

  public init(isUserInRoleStorage: boolean) {
    this._isUserInRoleStorage = isUserInRoleStorage;
  }

  //#endregion

}

export class PnrMetadataContainerClerk {

  private constructor() {}

  private static _instance: PnrMetadataContainerClerk;

  public static get instance(): PnrMetadataContainerClerk {
    if (!PnrMetadataContainerClerk._instance) {      
      PnrMetadataContainerClerk._instance = new PnrMetadataContainerClerk();
    }
    return PnrMetadataContainerClerk._instance;
  }

  private _isUserInRoleStorage: boolean;

  public get isUserInRole(): boolean {
    return this._isUserInRoleStorage;
  }

  public init(isUserInRoleStorage: boolean) {
    this._isUserInRoleStorage = isUserInRoleStorage;
  }
}

export class PnrMetadataContainerOfficeManager {

  private constructor() {}

  private static _instance: PnrMetadataContainerOfficeManager;

  public static get instance(): PnrMetadataContainerOfficeManager {
    if (!PnrMetadataContainerOfficeManager._instance) {      
      PnrMetadataContainerOfficeManager._instance = new PnrMetadataContainerOfficeManager();
    }
    return PnrMetadataContainerOfficeManager._instance;
  }

  private _isUserInRoleStorage: boolean;

  public get isUserInRole(): boolean {
    return this._isUserInRoleStorage;
  }

  public init(isUserInRoleStorage: boolean) {
    this._isUserInRoleStorage = isUserInRoleStorage;
  }
}

export class PnrMetadataInitalizationExtension extends ApplicationExtension {

  public afterMetadataReceived(context: IApplicationExtensionMetadataContext) {
    if (context.mainPartResponse) {
      const isUserInRole = tryGetFromInfo<boolean>(context.mainPartResponse.info, 'isUserInRole', false);
      PnrMetadataContainer.instance.init(isUserInRole);
      const isUserInRoleClerk = tryGetFromInfo<boolean>(context.mainPartResponse.info, 'isCurrentUserClerk', false);
      PnrMetadataContainerClerk.instance.init(isUserInRoleClerk);
      const isUserInRoleOfficeManager = tryGetFromInfo<boolean>(context.mainPartResponse.info, 'isCurrentUserOfficeManager', false);
      PnrMetadataContainerOfficeManager.instance.init(isUserInRoleOfficeManager);      
    }
  }
}