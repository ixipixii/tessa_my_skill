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
    public sealed class PnrOutgoingUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrOutgoingUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

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
                    var legalEntityIdx = response.Info.Get<string>("LegalEntityIdx");
                    if (legalEntityIdx != null)
                    {
                        cardModel.Card.Sections["PnrOutgoing"].Fields["LegalEntityIndexIdx"] = legalEntityIdx;
                    }
                    // Если в единственной указанной организации нет индекса то делаем поле LegalEntityIndexIdx пустым
                    // чтобы при проверке на сервере в валидацию была возвращена ошибка индекса организации
                    else
                    {
                        cardModel.Card.Sections["PnrOutgoing"].Fields["LegalEntityIndexIdx"] = null;
                    }
                }
            }
            else
            {
                //cardModel.Card.Sections["PnrIncoming"].Fields["LegalEntityIndexID"] = PnrLegalEntityIndex.WholeOrganizationID;
                cardModel.Card.Sections["PnrOutgoing"].Fields["LegalEntityIndexIdx"] = PnrLegalEntityIndex.WholeOrganizationIdx;
            }
        }

        private async void SetUserDepartmentInfo(Guid? authorID, ICardModel cardModel)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.GetUserDepartmentInfoRequestTypeID,
                Info =
                {
                    { "authorID", authorID }
                }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
            TessaDialog.ShowNotEmpty(result);
            if (result.IsSuccessful && cardModel.Card.Sections["PnrOutgoing"].Fields["DepartmentID"] == null)
            {
                cardModel.Card.Sections["PnrOutgoing"].Fields["DepartmentID"] = response.Info.Get<Guid?>("DepartmentID");
                cardModel.Card.Sections["PnrOutgoing"].Fields["DepartmentName"] = response.Info.Get<string>("Name");
                cardModel.Card.Sections["PnrOutgoing"].Fields["DepartmentIdx"] = response.Info.Get<string>("Index");
            }
        }

        // установка видимости контролов, зависимых от Вида исходящего документа
        private void SetDKDependenceVisibility(string documentKindIdx, ICardModel cardModel)
        {
            if (cardModel.Controls.TryGet("Destination", out IControlViewModel destination))
            {
                destination.ControlVisibility = documentKindIdx == PnrOutgoingTypes.OutgoingLetterIdx ? Visibility.Visible : Visibility.Collapsed;
                destination.Block.Rearrange();
            }
            if (cardModel.Controls.TryGet("DestinationFIO", out IControlViewModel destinationFIO))
            {
                destinationFIO.ControlVisibility = documentKindIdx == PnrOutgoingTypes.OutgoingLetterIdx ? Visibility.Visible : Visibility.Collapsed;
                destinationFIO.Block.Rearrange();
            }

            // Подписант - только в Исходящее
            if (cardModel.Controls.TryGet("Signatory", out IControlViewModel signatory))
            {
                signatory.ControlVisibility = documentKindIdx == PnrOutgoingTypes.OutgoingLetterIdx ? Visibility.Visible : Visibility.Collapsed;
                signatory.Block.Rearrange();
            }

            //if (cardModel.Controls.TryGet("FullName", out IControlViewModel fullName))
            //{
            //    fullName.ControlVisibility = documentKindIdx == PnrOutgoingTypes.OutgoingComplaintsIdx ? Visibility.Visible : Visibility.Collapsed;
            //    fullName.Block.Rearrange();
            //}

            IBlockViewModel block = cardModel.Blocks["RepliesComplaintsBlock"];
            block.BlockVisibility = documentKindIdx == PnrOutgoingTypes.OutgoingComplaintsIdx ? Visibility.Visible : Visibility.Collapsed;
            block.Rearrange();

            IBlockViewModel OutgoingLetterBlock = cardModel.Blocks["OutgoingLetterBlock"];
            OutgoingLetterBlock.BlockVisibility = documentKindIdx == PnrOutgoingTypes.OutgoingLetterIdx ? Visibility.Visible : Visibility.Collapsed;
            OutgoingLetterBlock.Rearrange();

            // валидация Доп.согласующие - только в Исходящее письмо
            //if (cardModel.Controls.TryGet("Performers", out IControlViewModel performers))
            //{
            //    //performers.HasActiveValidation = false;
            //    performers.IsRequired = (documentKindIdx == PnrOutgoingTypes.OutgoingLetterIdx);
            //    performers.NotifyUpdateValidation();
            //}
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            ICardModel model;
            Card card;
            CardSection PnrOutgoing, Dci;
            if (context == null ||
                (model = context.Model) == null ||
                (card = context.Card) == null || 
                !card.Sections.ContainsKey("PnrOutgoing") ||
                !card.Sections.ContainsKey("DocumentCommonInfo"))
            {
                return Task.CompletedTask;
            }
            else
            {
                Dci = card.Sections["DocumentCommonInfo"];
                PnrOutgoing = card.Sections["PnrOutgoing"];
            }

            // Создание карточки
            if (card.StoreMode == CardStoreMode.Insert)
            {
                // Заполнить поле Дата регистрации текущей датой
                if (model.Controls.TryGet("RegistrationDate", out IControlViewModel regDateControl))
                {
                    ((DateTimeViewModel)regDateControl).SelectedDate = DateTime.Now;
                }
                // начальная инициализация поля Подразделение по автору
                SetUserDepartmentInfo((Guid?)Dci.Fields["AuthorID"], model);

                // поле Вид исходящего документа автозополнено - устанавливаем видимость контролов в зависимости от Вида
                SetDKDependenceVisibility((string)PnrOutgoing.Fields["DocumentKindIdx"], model);
            }
            else if (PnrOutgoing.Fields["DocumentKindID"] != null)
            // Открытие карточки. Есть Вид исходящего документа - устанавливаем видимость контролов в зависимости от Вида
            {
                SetDKDependenceVisibility((string)PnrOutgoing.Fields["DocumentKindIdx"], model);
            }

            PnrOutgoing.FieldChanged += (s, e) =>
            {
                // Вид исходящего документа: Visibility контролов и переформирование номера
                if (e.FieldName == "DocumentKindIdx" && e.FieldValue != null)
                {
                    SetDKDependenceVisibility((string)e.FieldValue, model);
                }
            };

            // Организация ГК Пионер
            if (model.Controls.TryGet("Organizations", out IControlViewModel organizations))
            {
                var organizationsTable = (AutoCompleteTableViewModel)organizations;

                // Условие для того, чтобы не затрагивались импортиртированные карточки
                if (!card.CreatedByID.Equals(Guid.Parse("11111111-1111-1111-1111-111111111111")))
                {
                    // Если карточка скопирована то индекс переносится из предыдущей, однако пользователи могли уже изменить скцию организаций ГК
                    // чтобы не было чудес проверяем при инициализации карточки индекс и актуализируем его.
                    if (organizationsTable == null) return Task.CompletedTask;

                    if (organizationsTable.Items.Count > 1)
                    {
                        PnrOutgoing.Fields["LegalEntityIndexIdx"] = PnrLegalEntityIndex.WholeOrganizationIdx;
                    }
                    else if (organizationsTable.Items.Count == 1)
                    {
                        this.SetLegalEntityIndex(organizationsTable, model);
                    }
                    else
                    {
                        PnrOutgoing.Fields["LegalEntityIndexIdx"] = null;
                    }
                }

                organizationsTable.ValueSelected +=
                    async (sender, args) =>
                    {
                        SetLegalEntityIndex((AutoCompleteTableViewModel)sender, model);
                    };
                organizationsTable.ValueDeleted +=
                    async (sender, args) =>
                    {
                        SetLegalEntityIndex((AutoCompleteTableViewModel)sender, model);
                    };
            }

            return base.Initialized(context);
        }
    }
}
