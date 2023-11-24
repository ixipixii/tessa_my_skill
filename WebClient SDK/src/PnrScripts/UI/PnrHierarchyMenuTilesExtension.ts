import { TileExtension, ITileGlobalExtensionContext, Tile, TileGroups, ITile } from 'tessa/ui/tiles';
import { LoadingOverlay } from 'tessa/ui';
import { createTypedField, DotNetType } from 'tessa/platform';
import { createCard } from 'tessa/ui/uiHost';
import { PnrCardTypes } from '../Shared/PnrCardTypes';
import { PnrContractKinds } from '../Shared/PnrContractKinds';
import { PnrMetadataContainer } from 'src/PnrScripts/UI/PnrMetadataExtension';
import { userSession }  from 'common/utility';

export class PnrHierarchyMenuTilesExtension extends TileExtension {
    public async initializingGlobal(context: ITileGlobalExtensionContext) {
        const panel = context.workspace.rightPanel;
        const createCard = panel.tryGetTile('CreateCard');
        const contextSource = panel.contextSource;
        if (!createCard) return;

        const defaultDocuments = createCard.tryGetTile('Documents');
        if (defaultDocuments) {
            defaultDocuments.isCollapsed = true;
            defaultDocuments.isEnabled = false;
            defaultDocuments.isHidden = true;
        }

        if (userSession.isAdmin) {
            await this.ShowUKDocuments(createCard, contextSource);
            await this.ShowPioneerDocuments(createCard, contextSource);
        }
        else {
            const isUkUser = PnrMetadataContainer.instance.isUserInRole;

            if (isUkUser) {
                await this.ShowUKDocuments(createCard, contextSource);
            }
            else {
                await this.ShowPioneerDocuments(createCard, contextSource);
            }
        }
    }

    private async ShowPioneerDocuments(createCard: ITile, contextSource) {
        createCard.tiles.push(new Tile({
            name: 'DocumentsPioneer',
            caption: 'Документы',
            icon: '',
            contextSource: contextSource,
            group: TileGroups.Cards,
            tiles: [
                new Tile({
                    name: 'PnrIncoming',
                    caption: 'Входящие',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrIncomingTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrOutgoing',
                    caption: 'Исходящие',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrOutgoingTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrPartnerRequest',
                    caption: 'Заявки на КА',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrPartnerRequestTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrContract',
                    caption: 'Договор',
                    icon: '',
                    contextSource: contextSource,
                    group: TileGroups.Cards,
                    tiles: [
                        new Tile({
                            name: 'PnrContractUK',
                            caption: 'Договор УК ПС',
                            icon: '',
                            contextSource,
                            command: async () => {this.CreateTileAction(PnrCardTypes.PnrContractUKTypeID); },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrContractWithBuyers',
                            caption: 'С покупателями',
                            icon: '',
                            contextSource,
                            command: async () => {
                                this.CreateTileWithKindAction(
                                    PnrCardTypes.PnrContractTypeID,
                                    PnrContractKinds.PnrContractWithBuyersID,
                                    PnrContractKinds.PnrContractWithBuyersName
                                    );
                                },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrContractIntercompany',
                            caption: 'Внутрихолдинговый',
                            icon: '',
                            contextSource,
                            command: async () => {
                                this.CreateTileWithKindAction(
                                    PnrCardTypes.PnrContractTypeID,
                                    PnrContractKinds.PnrContractIntercompanyID,
                                    PnrContractKinds.PnrContractIntercompanyName
                                    );
                                },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrContractCFO',
                            caption: 'ЦФО',
                            icon: '',
                            contextSource,
                            command: async () => {
                                this.CreateTileWithKindAction(
                                    PnrCardTypes.PnrContractTypeID,
                                    PnrContractKinds.PnrContractCFOID,
                                    PnrContractKinds.PnrContractCFOName
                                    );
                                },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrContractDUP',
                            caption: 'ДУП',
                            icon: '',
                            contextSource,
                            group: TileGroups.Cards,
                            tiles: [
                                new Tile({
                                    name: 'PnrContractDUPIntragroup',
                                    caption: 'Внутригрупповой',
                                    icon: '',
                                    contextSource,
                                    command: async () => {
                                        this.CreateTileWithKindDUPAction(
                                            PnrCardTypes.PnrContractTypeID,
                                            PnrContractKinds.PnrContractDUPID,
                                            PnrContractKinds.PnrContractDUPName,
                                            PnrContractKinds.PnrContractDUPIntragroupID,
                                            PnrContractKinds.PnrContractDUPIntragroupName
                                            );
                                        },
                                    group: TileGroups.Cards
                                }),
                                new Tile({
                                    name: 'PnrContractDUPNotBuilding',
                                    caption: 'Нестроительный',
                                    icon: '',
                                    contextSource,
                                    command: async () => {
                                        this.CreateTileWithKindDUPAction(
                                            PnrCardTypes.PnrContractTypeID,
                                            PnrContractKinds.PnrContractDUPID,
                                            PnrContractKinds.PnrContractDUPName,
                                            PnrContractKinds.PnrContractDUPNotBuildingID,
                                            PnrContractKinds.PnrContractDUPNotBuildingName
                                            );
                                        },
                                    group: TileGroups.Cards
                                }),
                                new Tile({
                                    name: 'PnrContractDUPBuilding',
                                    caption: 'Строительный',
                                    icon: '',
                                    contextSource,
                                    command: async () => {
                                        this.CreateTileWithKindDUPAction(
                                            PnrCardTypes.PnrContractTypeID,
                                            PnrContractKinds.PnrContractDUPID,
                                            PnrContractKinds.PnrContractDUPName,
                                            PnrContractKinds.PnrContractDUPBuildingID,
                                            PnrContractKinds.PnrContractDUPBuildingName
                                            );
                                        },
                                    group: TileGroups.Cards
                                })
                            ]
                        })
                    ]
                }),
                new Tile({
                    name: 'PnrSupplementaryAgreement',
                    caption: 'ДС',
                    icon: '',
                    contextSource: contextSource,
                    group: TileGroups.Cards,
                    tiles: [
                        new Tile({
                            name: 'PnrSupplementaryAgreementUK',
                            caption: 'ДС УК ПС',
                            icon: '',
                            contextSource,
                            command: async () => {this.CreateTileAction(PnrCardTypes.PnrSupplementaryAgreementUKTypeID); },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrSupplementaryAgreementWithBuyers',
                            caption: 'С покупателями',
                            icon: '',
                            contextSource,
                            command: async () => {
                                this.CreateTileWithKindAction(
                                    PnrCardTypes.PnrSupplementaryAgreementTypeID,
                                    PnrContractKinds.PnrContractWithBuyersID,
                                    PnrContractKinds.PnrContractWithBuyersName
                                    );
                                },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrSupplementaryAgreementIntercompany',
                            caption: 'Внутрихолдинговый',
                            icon: '',
                            contextSource,
                            command: async () => {
                                this.CreateTileWithKindAction(
                                    PnrCardTypes.PnrSupplementaryAgreementTypeID,
                                    PnrContractKinds.PnrContractIntercompanyID,
                                    PnrContractKinds.PnrContractIntercompanyName
                                    );
                                },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrSupplementaryAgreementCFO',
                            caption: 'ЦФО',
                            icon: '',
                            contextSource,
                            command: async () => {
                                this.CreateTileWithKindAction(
                                    PnrCardTypes.PnrSupplementaryAgreementTypeID,
                                    PnrContractKinds.PnrContractCFOID,
                                    PnrContractKinds.PnrContractCFOName
                                    );
                                },
                            group: TileGroups.Cards
                        }),
                        new Tile({
                            name: 'PnrSupplementaryAgreementDUP',
                            caption: 'ДУП',
                            icon: '',
                            contextSource,
                            group: TileGroups.Cards,
                            tiles: [
                                new Tile({
                                    name: 'PnrSupplementaryAgreementDUPIntragroup',
                                    caption: 'Внутригрупповой',
                                    icon: '',
                                    contextSource,
                                    command: async () => {
                                        this.CreateTileWithKindDUPAction(
                                            PnrCardTypes.PnrSupplementaryAgreementTypeID,
                                            PnrContractKinds.PnrContractDUPID,
                                            PnrContractKinds.PnrContractDUPName,
                                            PnrContractKinds.PnrContractDUPIntragroupID,
                                            PnrContractKinds.PnrContractDUPIntragroupName
                                            );
                                        },
                                    group: TileGroups.Cards
                                }),
                                new Tile({
                                    name: 'PnrSupplementaryAgreementDUPNotBuilding',
                                    caption: 'Нестроительный',
                                    icon: '',
                                    contextSource,
                                    command: async () => {
                                        this.CreateTileWithKindDUPAction(
                                            PnrCardTypes.PnrSupplementaryAgreementTypeID,
                                            PnrContractKinds.PnrContractDUPID,
                                            PnrContractKinds.PnrContractDUPName,
                                            PnrContractKinds.PnrContractDUPNotBuildingID,
                                            PnrContractKinds.PnrContractDUPNotBuildingName
                                            );
                                        },
                                    group: TileGroups.Cards
                                }),
                                new Tile({
                                    name: 'PnrSupplementaryAgreementDUPBuilding',
                                    caption: 'Строительный',
                                    icon: '',
                                    contextSource,
                                    command: async () => {
                                        this.CreateTileWithKindDUPAction(
                                            PnrCardTypes.PnrSupplementaryAgreementTypeID,
                                            PnrContractKinds.PnrContractDUPID,
                                            PnrContractKinds.PnrContractDUPName,
                                            PnrContractKinds.PnrContractDUPBuildingID,
                                            PnrContractKinds.PnrContractDUPBuildingName
                                            );
                                        },
                                    group: TileGroups.Cards
                                })
                            ]
                        })
                    ]
                }),
                new Tile({
                    name: 'PnrAct',
                    caption: 'Акты',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrActTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrOrder',
                    caption: 'Приказы и распоряжения',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrOrderTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrRegulation',
                    caption: 'ВНД',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrRegulationTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrServiceNote',
                    caption: 'Служебная записка',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrServiceNoteTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrPowerAttorney',
                    caption: 'Доверенность',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrPowerAttorneyTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrTemplate',
                    caption: 'Шаблоны',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrTemplateTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrErrand',
                    caption: 'Поручения',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrErrandTypeID); },
                    group: TileGroups.Cards
                }),
                new Tile({
                    name: 'PnrTender',
                    caption: 'Тендеры',
                    icon: '',
                    contextSource,
                    command: async () => {this.CreateTileAction(PnrCardTypes.PnrTenderTypeID); },
                    group: TileGroups.Cards
                })
            ]
        }));
    }

    private async ShowUKDocuments(createCard: ITile, contextSource) {
    createCard.tiles.push(new Tile({
        name: 'DocumentsUK',
        caption: 'Документы УК ПС',
        icon: '',
        contextSource: contextSource,
        group: TileGroups.Cards,
        tiles: [
            new Tile({
                name: 'PnrIncomingUK',
                caption: 'Входящие',
                icon: '',
                contextSource,
                command: async () => {this.CreateTileAction(PnrCardTypes.PnrIncomingUKTypeID); },
                group: TileGroups.Cards,
                order: 1
            }),
            new Tile({
                name: 'PnrOutgoingUK',
                caption: 'Исходящие',
                icon: '',
                contextSource,
                command: async () => {this.CreateTileAction(PnrCardTypes.PnrOutgoingUKTypeID); },
                group: TileGroups.Cards,
                order: 2
            }),
            new Tile({
                name: 'PnrPartnerRequest',
                caption: 'Заявки на контрагента',
                icon: '',
                contextSource,
                command: async () => {this.CreateTileAction(PnrCardTypes.PnrPartnerRequestTypeID); },
                group: TileGroups.Cards,
                order: 3
            }),
            new Tile({
                name: 'PnrContractUK',
                caption: 'Договоры',
                icon: '',
                contextSource,
                command: async () => {this.CreateTileAction(PnrCardTypes.PnrContractUKTypeID); },
                group: TileGroups.Cards,
                order: 4
            }),
            new Tile({
                name: 'PnrSupplementaryAgreementUK',
                caption: 'Дополнительные соглашения',
                icon: '',
                contextSource,
                command: async () => {this.CreateTileAction(PnrCardTypes.PnrSupplementaryAgreementUKTypeID); },
                group: TileGroups.Cards,
                order: 5
            }),
            new Tile({
                name: 'PnrOrderUK',
                caption: 'Приказы',
                icon: '',
                contextSource,
                command: async () => {this.CreateTileAction(PnrCardTypes.PnrOrderUKTypeID); },
                group: TileGroups.Cards,
                order: 6
            })
        ]
    }));
    }

    private async CreateTileAction(pnrCardTypeId) {
        await LoadingOverlay.instance.show(async () => {
            await createCard({
                cardTypeId: pnrCardTypeId
            });
        });
    }

    private async CreateTileWithKindAction(pnrCardTypeId, kindID, kindName) {
        await LoadingOverlay.instance.show(async () => {
            await createCard({
                cardTypeId: pnrCardTypeId,
                info: {
                    KindID: createTypedField(kindID, DotNetType.Guid),
                    KindName: createTypedField(kindName, DotNetType.String)
                }
            });
        });
    }

    private async CreateTileWithKindDUPAction(pnrCardTypeId, kindID, kindName, kindDUPID, kindDUPName) {
        await LoadingOverlay.instance.show(async () => {
            await createCard({
                cardTypeId: pnrCardTypeId,
                info: {
                    KindID: createTypedField(kindID, DotNetType.Guid),
                    KindName: createTypedField(kindName, DotNetType.String),
                    KindDUPID: createTypedField(kindDUPID, DotNetType.Guid),
                    KindDUPName: createTypedField(kindDUPName, DotNetType.String)
                }
            });
        });
    }
}