using System;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Cards.Controls.AutoComplete;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrIncomingUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        private readonly Tessa.Platform.Runtime.ISession session;

        public PnrIncomingUIExtension(ICardRepository cardRepository, Tessa.Platform.Runtime.ISession session)
        {
            this.cardRepository = cardRepository;
            this.session = session;
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

            return response.Info.Get<bool>("isUserInRole");
        }

        // установка видимости контролов, зависимых от Вида входящего документа
        private void SetDKDependenceVisibility(string documentKindIdx, ICardModel cardModel)
        {
            if (cardModel.Controls.TryGet("Correspondent", out IControlViewModel correspondent))
            {
                correspondent.ControlVisibility = documentKindIdx == "1-01" ? Visibility.Visible : Visibility.Collapsed;
                correspondent.Block.Rearrange();
            }

            IBlockViewModel block = cardModel.Blocks["ComplaintsBlock"];
            block.BlockVisibility = documentKindIdx == "1-02" ? Visibility.Visible : Visibility.Collapsed;
            block.Rearrange();

            IBlockViewModel IncomingLetterBlock = cardModel.Blocks["IncomingLetterBlock"];
            IncomingLetterBlock.BlockVisibility = documentKindIdx == PnrIncomingTypes.IncomingLetterIdx ? Visibility.Visible : Visibility.Collapsed;
            IncomingLetterBlock.Rearrange();
        }

        // установка значения скрытого поля Индекс ЮЛ
        async void SetLegalEntityIndex(AutoCompleteTableViewModel organizationControl, ICardModel cardModel)
        {
            if (organizationControl.Items.Count == 1)
            {
                RowAutoCompleteItem rowAutoCompleteItem = (RowAutoCompleteItem)organizationControl.Items[0];
                CardRow row = rowAutoCompleteItem.Row;
                Guid organizationID = (Guid)row["OrganizationID"];
                CardRequest request = new CardRequest
                {
                    RequestType = Shared.PnrRequestTypes.GetIndexLegalEntityRequestTypeID,
                    Info =
                    {
                        { "organizationID", organizationID }
                    }
                };

                CardResponse response = await cardRepository.RequestAsync(request);

                Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
                TessaDialog.ShowNotEmpty(result);
                if (result.IsSuccessful)
                {
                    //cardModel.Card.Sections["PnrIncoming"].Fields["LegalEntityIndexID"] = response.Info.Get<Guid?>("LegalEntityID");
                    cardModel.Card.Sections["PnrIncoming"].Fields["LegalEntityIndexIdx"] = response.Info.Get<string>("LegalEntityIdx");
                }
            }
            else
            {
                //cardModel.Card.Sections["PnrIncoming"].Fields["LegalEntityIndexID"] = PnrLegalEntityIndex.WholeOrganizationID;
                cardModel.Card.Sections["PnrIncoming"].Fields["LegalEntityIndexIdx"] = PnrLegalEntityIndex.WholeOrganizationIdx;
            }
        }

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            var currentUserID = session.User.ID;
            var isCurrentUserClerk = await GetIsUserInRole(currentUserID, PnrRoles.Clerk);
            var isCurrentUserOfficeManager = await GetIsUserInRole(currentUserID, PnrRoles.OfficeManager);
            var isCurrentUserAdmin = session.User.AccessLevel == Tessa.Platform.Runtime.UserAccessLevel.Administrator;

            if (cardModel.Controls.TryGet("DocumentKind", out IControlViewModel documentKind))
            {
                if (isCurrentUserAdmin || (isCurrentUserClerk && isCurrentUserOfficeManager))
                {
                    documentKind.IsReadOnly = false;
                }
                else
                {
                    documentKind.IsReadOnly = true;
                }
            }

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой(можно просто продублировать значение из Дата создания)
                if (cardModel.Controls.TryGet("RegistrationDate", out IControlViewModel regDateControl))
                {
                    ((DateTimeViewModel)regDateControl).SelectedDate = DateTime.Now;
                }

                // поле Вид входящего документа автозополнено - устанавливаем видимость контролов в зависимости от Вида
                SetDKDependenceVisibility((string)card.Sections["PnrIncoming"].Fields["DocumentKindIdx"], cardModel);

            }
            else if (card.Sections["PnrIncoming"].Fields["DocumentKindID"] != null)
            // Открытие карточки. Есть Вид входящего документа - устанавливаем видимость контролов в зависимости от Вида
            {
                SetDKDependenceVisibility((string)card.Sections["PnrIncoming"].Fields["DocumentKindIdx"], cardModel);
            }

            // Организация ГК Пионер
            if (context.Model.Controls.TryGet("Organizations", out IControlViewModel organizations))
            {
                var autoComplete = (AutoCompleteTableViewModel)organizations;
                autoComplete.ValueSelected +=
                    async (sender, args) =>
                    {
                        SetLegalEntityIndex((AutoCompleteTableViewModel)sender, context.Model);
                    };
                autoComplete.ValueDeleted +=
                    async (sender, args) =>
                    {
                        SetLegalEntityIndex((AutoCompleteTableViewModel)sender, context.Model);
                    };
            }

            if ((Guid)card.Sections["PnrIncoming"].Fields["DocumentKindID"] == PnrIncomingTypes.IncomingComplaintsID &&
                card.Sections["PnrIncoming"].Fields["DepartmentID"] == null)
            {
                SetAutorDepartment(PnrIncomingTypes.IncomingComplaintsID, card);
            }

            context.Card.Sections["PnrIncoming"].FieldChanged += (s, e) =>
            {
                // Вид входящего документа: Visibility контролов
                if (e.FieldName == "DocumentKindIdx" && e.FieldValue != null)
                {
                    SetDKDependenceVisibility((string)e.FieldValue, cardModel);
                }

                if (e.FieldName == "DocumentKindID" && e.FieldValue != null)
                {
                    SetAutorDepartment((Guid)e.FieldValue, card);
                }
            };

            // return base.Initialized(context);
        }

        private async void SetAutorDepartment(Guid fieldValue, Card card)
        {
            if (fieldValue.Equals(PnrIncomingTypes.IncomingComplaintsID))
            {
                CardRequest request = new CardRequest
                {
                    RequestType = Shared.PnrRequestTypes.GetUserDepartmentInfoRequestTypeID,
                    Info =
                    {
                        { "authorID", card.CreatedByID }
                    }
                };

                CardResponse response = await cardRepository.RequestAsync(request);

                var result = response.ValidationResult.Build();
                if (result.IsSuccessful)
                {
                    card.Sections["PnrIncoming"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                    card.Sections["PnrIncoming"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                    card.Sections["PnrIncoming"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
                }
            }
            else
            {
                card.Sections["PnrIncoming"].Fields["DepartmentID"] = null;
                card.Sections["PnrIncoming"].Fields["DepartmentName"] = null;
                card.Sections["PnrIncoming"].Fields["DepartmentIdx"] = null;
            }
        }
    }
}
