using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Tiles;
using Tessa.UI.Tiles.Extensions;

namespace Tessa.Extensions.Client.Tiles
{
    public class PnrHierarchyContractMenuTilesExtension : TileExtension
    {
        private readonly IUIHost uiHost;
        private readonly Tessa.Platform.Runtime.ISession session;
        private readonly ICardRepository cardRepository;

        public PnrHierarchyContractMenuTilesExtension(IUIHost uiHost, Tessa.Platform.Runtime.ISession session, ICardRepository cardRepository)
        {
            this.uiHost = uiHost;
            this.session = session;
            this.cardRepository = cardRepository;
        }

        private async void PnrContractTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrContractTypeID,
                options: new CreateCardOptions
                {
                    Info = new Dictionary<string, object>
                    {
                        { "KindID", ((ITile)parameter).Info["KindID"] },
                        { "KindName", ((ITile)parameter).Info["KindName"] },
                        { "KindDUPID", ((ITile)parameter).Info["KindDUPID"] },
                        { "KindDUPName", ((ITile)parameter).Info["KindDUPName"] }
                    }
                }
            );
        }

        private async void PnrSupplementaryAgreementTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrSupplementaryAgreementTypeID,
                options: new CreateCardOptions
                {
                    Info = new Dictionary<string, object>
                    {
                        { "KindID", ((ITile)parameter).Info["KindID"] },
                        { "KindName", ((ITile)parameter).Info["KindName"] },
                        { "KindDUPID", ((ITile)parameter).Info["KindDUPID"] },
                        { "KindDUPName", ((ITile)parameter).Info["KindDUPName"] }
                    }
                }
            );
        }

        private async void PnrDefaultContractTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrContractTypeID
            );
        }

        private async void PnrDefaultSupplementaryAgreementTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrSupplementaryAgreementTypeID
            );
        }

        private async void PnrIncomingUKTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrIncomingUKTypeID
            );
        }

        private async void PnrOutgoingUKTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrOutgoingUKTypeID
            );
        }

        private async void PnrPartnerRequestTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrPartnerRequestTypeID
            );
        }

        private async void PnrContractUKTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrContractUKTypeID
            );
        }

        private async void PnrSupplementaryAgreementUKTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrSupplementaryAgreementUKTypeID
            );
        }

        private async void PnrOrderUKTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrOrderUKTypeID
            );
        }

        private async void PnrIncomingTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrIncomingTypeID
            );
        }

        private async void PnrOutgoingTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrOutgoingTypeID
            );
        }

        private async void PnrActTileAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrActTypeID
            );
        }

        private async void PnrOrderAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrOrderTypeID
            );
        }

        private async void PnrRegulationAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrRegulationTypeID
            );
        }

        private async void PnrServiceNoteAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrServiceNoteTypeID
            );
        }

        private async void PnrPowerAttorneyAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrPowerAttorneyTypeID
            );
        }

        private async void PnrTemplateAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrTemplateTypeID
            );
        }

        private async void PnrErrandAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrErrandTypeID
            );
        }

        private async void PnrTenderAction(object parameter)
        {
            await uiHost.CreateCardAsync(
                cardTypeID: PnrCardTypes.PnrTenderTypeID
            );
        }

        private async Task<bool> GetIsUserInRole(Guid userID, Guid roleID)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetIsUserInRoleExtension,
                Info =
                    {
                        { "userID", userID },
                        { "roleID", roleID }
                    }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            // Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();

            return response.Info.Get<bool>("isUserInRole");
        }

        private async Task ShowUKDocuments(ITile createCard, ITileContextSource contextSource)
        {
            createCard.Tiles.Add(
               new Tile(
               "DocumentsUK",
               "Документы УК ПС",
               Icon.Empty,
               contextSource,
               DelegateCommand.Empty,
               size: TileSize.Half));

            ITile documents = createCard.Tiles["DocumentsUK"];

            documents.Tiles.AddRange(
                new Tile(
                    "PnrIncomingUK",
                    "Входящие",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrIncomingUKTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrOutgoingUK",
                    "Исходящие",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrOutgoingUKTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrPartnerRequest",
                    "Заявки на контрагента",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrPartnerRequestTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrContractUK",
                    "Договоры",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrContractUKTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrSupplementaryAgreementUK",
                    "Дополнительные соглашения",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrSupplementaryAgreementUKTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrOrderUK",
                    "Приказы",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrOrderUKTileAction),
                    size: TileSize.Half
                    )
                );
        }

        private async Task ShowPioneerDocuments(ITile createCard, ITileContextSource contextSource)
        {
            createCard.Tiles.Add(
               new Tile(
               "DocumentsPioneer",
               "Документы",
               Icon.Empty,
               contextSource,
               DelegateCommand.Empty,
               size: TileSize.Half));

            ITile documents = createCard.Tiles["DocumentsPioneer"];

            documents.Tiles.AddRange(
                new Tile(
                    "PnrIncoming",
                    "Входящие",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrIncomingTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrOutgoing",
                    "Исходящие",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrOutgoingTileAction),
                    size: TileSize.Half
                    ),

                new Tile(
                    "PnrOutgoing",
                    "Заявки на КА",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrPartnerRequestTileAction),
                    size: TileSize.Half
                    )
                );

            documents.Tiles.Add(
                new Tile(
                "PnrContract",
                "Договор",
                Icon.Empty,
                contextSource,
                DelegateCommand.Empty,
                size: TileSize.Half,
                tiles: new TileCollection
                {
                    new Tile(
                        "PnrContractUK",
                        "Договор УК ПС",
                        Icon.Empty,
                        contextSource,
                        new DelegateCommand(this.PnrContractUKTileAction),
                        size: TileSize.Half
                        ),

                    new Tile(
                        "PnrContractWithBuyers",
                        "С покупателями",
                        Icon.Empty,
                        contextSource,
                        new DelegateCommand(this.PnrContractTileAction),
                        size: TileSize.Half,
                        info: new SerializableObject
                        {
                            ["KindID"] = PnrContractKinds.PnrContractWithBuyersID,
                            ["KindName"] = PnrContractKinds.PnrContractWithBuyersName,
                            ["KindDUPID"] = null,
                            ["KindDUPName"] = null
                        }
                        ),

                    new Tile(
                        "PnrContractIntercompany",
                        "Внутрихолдинговый",
                        Icon.Empty,
                        contextSource,
                        new DelegateCommand(this.PnrContractTileAction),
                        size: TileSize.Half,
                        info: new SerializableObject
                        {
                            ["KindID"] = PnrContractKinds.PnrContractIntercompanyID,
                            ["KindName"] = PnrContractKinds.PnrContractIntercompanyName,
                            ["KindDUPID"] = null,
                            ["KindDUPName"] = null
                        }
                        ),

                    new Tile(
                        "PnrContractCFO",
                        "ЦФО",
                        Icon.Empty,
                        contextSource,
                        new DelegateCommand(this.PnrContractTileAction),
                        size: TileSize.Half,
                        info: new SerializableObject
                        {
                            ["KindID"] = PnrContractKinds.PnrContractCFOID,
                            ["KindName"] = PnrContractKinds.PnrContractCFOName,
                            ["KindDUPID"] = null,
                            ["KindDUPName"] = null
                        }
                        ),

                    new Tile(
                        "PnrContractDUP",
                        "ДУП",
                        Icon.Empty,
                        contextSource,
                        DelegateCommand.Empty,
                        size: TileSize.Half,
                        tiles: new TileCollection
                        {
                            new Tile(
                                "PnrContractDUPIntragroup",
                                "Внутригрупповой",
                                Icon.Empty,
                                contextSource,
                                new DelegateCommand(this.PnrContractTileAction),
                                size: TileSize.Half,
                                info: new SerializableObject
                                {
                                    ["KindID"] = PnrContractKinds.PnrContractDUPID,
                                    ["KindName"] = PnrContractKinds.PnrContractDUPName,
                                    ["KindDUPID"] = PnrContractKinds.PnrContractDUPIntragroupID,
                                    ["KindDUPName"] = PnrContractKinds.PnrContractDUPIntragroupName
                                }
                                ),

                            new Tile(
                                "PnrContractDUPNotBuilding",
                                "Нестроительный",
                                Icon.Empty,
                                contextSource,
                                new DelegateCommand(this.PnrContractTileAction),
                                size: TileSize.Half,
                                info: new SerializableObject
                                {
                                    ["KindID"] = PnrContractKinds.PnrContractDUPID,
                                    ["KindName"] = PnrContractKinds.PnrContractDUPName,
                                    ["KindDUPID"] = PnrContractKinds.PnrContractDUPNotBuildingID,
                                    ["KindDUPName"] = PnrContractKinds.PnrContractDUPNotBuildingName
                                }
                                ),

                            new Tile(
                                "PnrContractDUPBuilding",
                                "Строительный",
                                Icon.Empty,
                                contextSource,
                                new DelegateCommand(this.PnrContractTileAction),
                                size: TileSize.Half,
                                info: new SerializableObject
                                {
                                    ["KindID"] = PnrContractKinds.PnrContractDUPID,
                                    ["KindName"] = PnrContractKinds.PnrContractDUPName,
                                    ["KindDUPID"] = PnrContractKinds.PnrContractDUPBuildingID,
                                    ["KindDUPName"] = PnrContractKinds.PnrContractDUPBuildingName
                                }
                                )
                        }
                        )
                }
                )
                );

            documents.Tiles.Add(
                new Tile(
                    "PnrSupplementaryAgreement",
                    "ДС",
                    Icon.Empty,
                    contextSource,
                    DelegateCommand.Empty,
                    size: TileSize.Half,
                    tiles: new TileCollection
                    {
                        new Tile(
                            "PnrSupplementaryAgreementUK",
                            "ДС УК ПС",
                            Icon.Empty,
                            contextSource,
                            new DelegateCommand(this.PnrSupplementaryAgreementUKTileAction),
                            size: TileSize.Half
                            ),

                        new Tile(
                            "PnrSupplementaryAgreementWithBuyers",
                            "С покупателями",
                            Icon.Empty,
                            contextSource,
                            new DelegateCommand(this.PnrSupplementaryAgreementTileAction),
                            size: TileSize.Half,
                            info: new SerializableObject
                            {
                                ["KindID"] = PnrContractKinds.PnrContractWithBuyersID,
                                ["KindName"] = PnrContractKinds.PnrContractWithBuyersName,
                                ["KindDUPID"] = null,
                                ["KindDUPName"] = null
                            }
                            ),

                        new Tile(
                            "PnrSupplementaryAgreementIntercompany",
                            "Внутрихолдинговый",
                            Icon.Empty,
                            contextSource,
                            new DelegateCommand(this.PnrSupplementaryAgreementTileAction),
                            size: TileSize.Half,
                            info: new SerializableObject
                            {
                                ["KindID"] = PnrContractKinds.PnrContractIntercompanyID,
                                ["KindName"] = PnrContractKinds.PnrContractIntercompanyName,
                                ["KindDUPID"] = null,
                                ["KindDUPName"] = null
                            }
                            ),

                        new Tile(
                            "PnrSupplementaryAgreementCFO",
                            "ЦФО",
                            Icon.Empty,
                            contextSource,
                            new DelegateCommand(this.PnrSupplementaryAgreementTileAction),
                            size: TileSize.Half,
                            info: new SerializableObject
                            {
                                ["KindID"] = PnrContractKinds.PnrContractCFOID,
                                ["KindName"] = PnrContractKinds.PnrContractCFOName,
                                ["KindDUPID"] = null,
                                ["KindDUPName"] = null
                            }
                            ),

                        new Tile(
                            "PnrSupplementaryAgreementDUP",
                            "ДУП",
                            Icon.Empty,
                            contextSource,
                            DelegateCommand.Empty,
                            size: TileSize.Half,
                            tiles: new TileCollection
                            {
                                new Tile(
                                    "PnrSupplementaryAgreementDUPIntragroup",
                                    "Внутригрупповой",
                                    Icon.Empty,
                                    contextSource,
                                    new DelegateCommand(this.PnrSupplementaryAgreementTileAction),
                                    size: TileSize.Half,
                                    info: new SerializableObject
                                    {
                                        ["KindID"] = PnrContractKinds.PnrContractDUPID,
                                        ["KindName"] = PnrContractKinds.PnrContractDUPName,
                                        ["KindDUPID"] = PnrContractKinds.PnrContractDUPIntragroupID,
                                        ["KindDUPName"] = PnrContractKinds.PnrContractDUPIntragroupName
                                    }
                                    ),

                                new Tile(
                                    "PnrSupplementaryAgreementDUPNotBuilding",
                                    "Нестроительный",
                                    Icon.Empty,
                                    contextSource,
                                    new DelegateCommand(this.PnrSupplementaryAgreementTileAction),
                                    size: TileSize.Half,
                                    info: new SerializableObject
                                    {
                                        ["KindID"] = PnrContractKinds.PnrContractDUPID,
                                        ["KindName"] = PnrContractKinds.PnrContractDUPName,
                                        ["KindDUPID"] = PnrContractKinds.PnrContractDUPNotBuildingID,
                                        ["KindDUPName"] = PnrContractKinds.PnrContractDUPNotBuildingName
                                    }
                                    ),

                                new Tile(
                                    "PnrSupplementaryAgreementDUPBuilding",
                                    "Строительный",
                                    Icon.Empty,
                                    contextSource,
                                    new DelegateCommand(this.PnrSupplementaryAgreementTileAction),
                                    size: TileSize.Half,
                                    info: new SerializableObject
                                    {
                                        ["KindID"] = PnrContractKinds.PnrContractDUPID,
                                        ["KindName"] = PnrContractKinds.PnrContractDUPName,
                                        ["KindDUPID"] = PnrContractKinds.PnrContractDUPBuildingID,
                                        ["KindDUPName"] = PnrContractKinds.PnrContractDUPBuildingName
                                    }
                                    )
                            }
                            )
                    }
                    )
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrAct",
                    "Акты",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrActTileAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrOrder",
                    "Приказы и распоряжения",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrOrderAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrRegulation",
                    "ВНД",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrRegulationAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrServiceNote",
                    "Служебная записка",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrServiceNoteAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrPowerAttorney",
                    "Доверенность",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrPowerAttorneyAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrTemplate",
                    "Шаблоны",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrTemplateAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrErrand",
                    "Поручения",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrErrandAction),
                    size: TileSize.Half)
                );

            documents.Tiles.AddRange(
                new Tile(
                    "PnrTender",
                    "Тендеры",
                    Icon.Empty,
                    contextSource,
                    new DelegateCommand(this.PnrTenderAction),
                    size: TileSize.Half)
                );
        }

        public override async Task InitializingGlobal(ITileGlobalExtensionContext context)
        {
            ITilePanel panel = context.Workspace.RightPanel;
            ITile createCard = panel.Tiles.TryGet(TileNames.CreateCard);

            ITileContextSource contextSource = context.Workspace.RightPanel;

            var IsCurrentUserAdmin = this.session.User.AccessLevel == Tessa.Platform.Runtime.UserAccessLevel.Administrator;

            var IsCurrentUserUK = await GetIsUserInRole(this.session.User.ID, PnrRoles.EmployeeUkID);
            var IsCurrentUserGK = await GetIsUserInRole(this.session.User.ID, PnrRoles.EmployeeGkID);

            if (IsCurrentUserAdmin)
            {
                await ShowUKDocuments(createCard, contextSource);
                await ShowPioneerDocuments(createCard, contextSource);
            }
            else
            {
                if (IsCurrentUserUK)
                {
                    await ShowUKDocuments(createCard, contextSource);
                }

                if (IsCurrentUserGK)
                {
                    await ShowPioneerDocuments(createCard, contextSource);
                }
            }
        }
    }
}
