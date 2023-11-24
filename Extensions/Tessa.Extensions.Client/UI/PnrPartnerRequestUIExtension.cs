using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Files;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrPartnerRequestUIExtension : CardUIExtension
    {
        private readonly ICardRepository cardRepository;
        public PnrPartnerRequestUIExtension(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        // установка видимости контролов, зависимых от Тип контрагента
        private void SetPTDependenceVisibility(int? typeID, ICardModel cardModel)
        {
            // тип заявки
            int? requestTypeID = (int?)cardModel.Card.Sections["PnrPartnerRequests"].Fields["RequestTypeID"];

            // ИНН
            if (cardModel.Controls.TryGet("INN", out IControlViewModel inn))
            {
                inn.ControlVisibility = (typeID == 1 || typeID == 3) ? Visibility.Visible : Visibility.Collapsed;
                inn.Block.Rearrange();
            }

            // КПП
            if (cardModel.Controls.TryGet("KPP", out IControlViewModel kpp))
            {
                kpp.ControlVisibility = (typeID == 1) ? Visibility.Visible : Visibility.Collapsed;
                kpp.Block.Rearrange();
            }

            // ОГРН
            if (cardModel.Controls.TryGet("OGRN", out IControlViewModel ogrn))
            {
                ogrn.ControlVisibility = (typeID == 3) ? Visibility.Visible : Visibility.Collapsed;
                ogrn.Block.Rearrange();
            }

            // День рождения
            if (cardModel.Controls.TryGet("Birthday", out IControlViewModel birthday))
            {
                birthday.ControlVisibility = (typeID == 2) ? Visibility.Visible : Visibility.Collapsed;
                birthday.Block.Rearrange();
            }
            
            // Страна регистрации (всегда скрыта в режиме Согласования КА)
            if (cardModel.Controls.TryGet("CountryRegistration", out IControlViewModel countryRegistration))
            {
                countryRegistration.ControlVisibility = ((typeID == 1 || typeID == 2) && requestTypeID != 1) ? Visibility.Visible : Visibility.Collapsed;
                countryRegistration.Block.Rearrange();
            }

            // блоки-подсказки перечня категорий файлов по типам КА
            // для Юридического лица
            var fileHelperLegalEntity = cardModel.Blocks["FileHelperLegalEntity"];
            fileHelperLegalEntity.BlockVisibility = (typeID == 1 && requestTypeID == (int?)PnrPartnerRequestsTypes.ApproveID) ? Visibility.Visible : Visibility.Collapsed;
            fileHelperLegalEntity.Rearrange();
            // для Индивидуального предпринимателя
            var fileHelperSoleTrader = cardModel.Blocks["FileHelperSoleTrader"];
            fileHelperSoleTrader.BlockVisibility = (typeID == 3 && requestTypeID == (int?)PnrPartnerRequestsTypes.ApproveID) ? Visibility.Visible : Visibility.Collapsed;
            fileHelperSoleTrader.Rearrange();
            // для Физического лица
            var fileHelperIndividual = cardModel.Blocks["FileHelperIndividual"];
            fileHelperIndividual.BlockVisibility = (typeID == 2 && requestTypeID == (int?)PnrPartnerRequestsTypes.ApproveID) ? Visibility.Visible : Visibility.Collapsed;
            fileHelperIndividual.Rearrange();
            
            foreach (var block in cardModel.BlockBag)
            {
                block.Rearrange();
            }    
        }

        // видимость полей и блоков, зависимых от Тип заявки
        private void SetPartnerVisibility(int typeID, ICardModel cardModel)
        {
            // Видимость Контрагент
            if (cardModel.Controls.TryGet("Partner", out IControlViewModel partner))
            {
                partner.ControlVisibility = (typeID == 1) ? Visibility.Visible : Visibility.Collapsed;
                partner.Block.Rearrange();
            }

            // Страна регистрации скрыта в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("CountryRegistration", out IControlViewModel countryRegistration))
            {
                countryRegistration.ControlVisibility = (typeID == 1) ? Visibility.Collapsed : Visibility.Visible;
            }

            // Disable всех полей блока КА в режиме "Согласование КА"
            IBlockViewModel block = cardModel.Blocks["NewPartnerBlock"];
            foreach(IControlViewModel control in block.Controls)
            {
                control.IsReadOnly = (typeID == 1);
            }
            block.Rearrange();
            // Особый признак контрагента - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("SpecialSign", out IControlViewModel specialSign))
            {
                if (typeID == 1)
                    specialSign.IsReadOnly = false;
            }
            // ИНН - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("INN", out IControlViewModel INN))
            {
                if (typeID == 1)
                    INN.IsReadOnly = false;
            }
            // КПП - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("KPP", out IControlViewModel KPP))
            {
                if (typeID == 1)
                    KPP.IsReadOnly = false;
            }
            // Комментарий - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("Comment", out IControlViewModel comment))
            {
                if (typeID == 1)
                    comment.IsReadOnly = false;
            }
            // Тип контрагента - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("Type", out IControlViewModel type))
            {
                if (typeID == 1)
                    type.IsReadOnly = false;
            }
            // Полное наименование контрагента - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("FullName", out IControlViewModel fullName))
            {
                if (typeID == 1)
                    fullName.IsReadOnly = false;
            }
            // Краткое наименование контагента - должно быть доступно к выбору в режиме "Согласование КА"
            if (cardModel.Controls.TryGet("ShortName", out IControlViewModel shortName))
            {
                if (typeID == 1)
                    shortName.IsReadOnly = false;
            }

            // Видимость Направление
            //if (cardModel.Controls.TryGet("ControlDirection", out IControlViewModel controlDirection))
            //{
            //    controlDirection.ControlVisibility = (typeID == 1) ? Visibility.Collapsed : Visibility.Visible;
            //    controlDirection.Block.Rearrange();
            //}

            // Требует согласования КА - скрыто в режиме Согласование КА
            if (cardModel.Controls.TryGet("RequiresApprovalCA", out IControlViewModel requiresApprovalCA))
            {
                requiresApprovalCA.ControlVisibility = (typeID == 1) ? Visibility.Collapsed : Visibility.Visible;
                requiresApprovalCA.Block.Rearrange();
            }
            // Файлы - скрыто при Тип заявки - Создание нового контрагента
            if (cardModel.Controls.TryGet("Files", out IControlViewModel files))
            {
                files.ControlVisibility = (typeID == PnrPartnerRequestsTypes.CreateID) ? Visibility.Collapsed : Visibility.Visible;
                files.Block.Rearrange();
            }
        }

        // Видимость блока с паспортными данными физического лица
        private void SetIndividualVisibility(Card card, ICardModel cardModel)
        {
            var partnerTypeID = (int?)card.Sections["PnrPartnerRequests"].Fields["TypeID"];
            var requestTypeID = (int?)card.Sections["PnrPartnerRequests"].Fields["RequestTypeID"];

            var isCreateNewIndividual = partnerTypeID == 2 && requestTypeID == 0;

            var blockIndividual = cardModel.Blocks["BlockIndividual"];

            blockIndividual.BlockVisibility = isCreateNewIndividual ? Visibility.Visible : Visibility.Collapsed;

            blockIndividual.Rearrange();
        }

        private async void GetPartnerInfo(Guid partnerID, ICardModel cardModel)
        {
            CardRequest request = new CardRequest
            {
                RequestType = Shared.PnrRequestTypes.PartnerInfo,
                Info =
                {
                    { "partnerID", partnerID }
                }
            };

            CardResponse response = await cardRepository.RequestAsync(request);

            Tessa.Platform.Validation.ValidationResult result = response.ValidationResult.Build();
            TessaDialog.ShowNotEmpty(result);
            if (result.IsSuccessful)
            {
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["TypeID"] = response.Info.Get<int?>("TypeID");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["TypeName"] = response.Info.Get<string>("TypeName");

                cardModel.Card.Sections["PnrPartnerRequests"].Fields["ShortName"] = response.Info.Get<string>("Name");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["FullName"] = response.Info.Get<string>("FullName");

                // если в КА не установлен атрибут "Особый признак КА": он должен оставаться по умолчанию - "Нет"
                int? specialSignID = response.Info.Get<int?>("SpecialSignID");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["SpecialSignID"] = specialSignID != null ? specialSignID : PnrSpecialSigns.NoID;
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["SpecialSignName"] = specialSignID != null ? response.Info.Get<string>("SpecialSignName") : PnrSpecialSigns.NoName;
                
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["NonResident"] = response.Info.Get<bool?>("NonResident");

                cardModel.Card.Sections["PnrPartnerRequests"].Fields["INN"] = response.Info.Get<string>("INN");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["KPP"] = response.Info.Get<string>("KPP");

                cardModel.Card.Sections["PnrPartnerRequests"].Fields["CountryRegistrationID"] = response.Info.Get<Guid?>("CountryRegistrationID");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["CountryRegistrationName"] = response.Info.Get<string>("CountryRegistrationName");

                cardModel.Card.Sections["PnrPartnerRequests"].Fields["Comment"] = response.Info.Get<string>("Comment");

                // справочники разные, значение не устанавливаем
                //cardModel.Card.Sections["PnrPartnerRequests"].Fields["DirectionID"] = response.Info.Get<int?>("DirectionID");
                //cardModel.Card.Sections["PnrPartnerRequests"].Fields["DirectionName"] = response.Info.Get<string>("DirectionName");

                cardModel.Card.Sections["PnrPartnerRequests"].Fields["IdentityDocument"] = response.Info.Get<string>("IdentityDocument");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["IdentityDocumentKind"] = response.Info.Get<string>("IdentityDocumentKind");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["IdentityDocumentIssueDate"] = response.Info.Get<DateTime?>("IdentityDocumentIssueDate");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["IdentityDocumentSeries"] = response.Info.Get<string>("IdentityDocumentSeries");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["IdentityDocumentNumber"] = response.Info.Get<string>("IdentityDocumentNumber");
                cardModel.Card.Sections["PnrPartnerRequests"].Fields["IdentityDocumentIssuedBy"] = response.Info.Get<string>("IdentityDocumentIssuedBy");

                cardModel.Card.Sections["PnrPartnerRequests"].Fields["OGRN"] = response.Info.Get<string>("OGRN");
            }
        }

        private void SetNeedFieldValidation(Card card, ICardModel cardModel)
        {
            cardModel.Controls.TryGet("KPP", out IControlViewModel kpp);
            cardModel.Controls.TryGet("INN", out IControlViewModel inn);

            if(kpp != null && inn != null)
            {
                // Тип контрагента
                int? typeID = (int?)card.Sections["PnrPartnerRequests"].Fields["TypeID"];
                // Особый признак контрагента
                int? specialSignID = (int?)card.Sections["PnrPartnerRequests"].Fields["SpecialSignID"];
                // Нерезидент
                int? nonResidentID = (int?)card.Sections["PnrPartnerRequests"].Fields["NonResidentID"];

                // Особый признак контрагента != Гос.органы && Нерезидент != Да
                bool isNeedValidation = (specialSignID != 2) && (nonResidentID != 1);

                // Валидация КПП при: тип ЮЛ && Особый признак контрагента != Гос.органы && Нерезидент != Да
                kpp.HasActiveValidation = kpp.IsRequired = (typeID == 1) && isNeedValidation;
                kpp.NotifyUpdateValidation();

                // Валидация ИНН при: (тип ЮЛ || ИП) && Особый признак контрагента != Гос.органы && Нерезидент != Да
                inn.HasActiveValidation = inn.IsRequired = (typeID == 1 || typeID == 3) && isNeedValidation;
                inn.NotifyUpdateValidation();
            }
        }

        public override Task Initialized(ICardUIExtensionContext context)
        {
            Card card = context.Card;
            ICardModel cardModel = context.Model;

            // валидатор для КПП
            cardModel.Controls.TryGet("KPP", out IControlViewModel kpp);
            kpp.ValidationFunc = c => (((ControlViewModelBase)c).HasEmptyValue()) ? "Поле не заполнено." : null;
            
            // валидатор для ИНН
            cardModel.Controls.TryGet("INN", out IControlViewModel inn);
            inn.ValidationFunc = c => (((ControlViewModelBase)c).HasEmptyValue()) ? "Поле не заполнено." : null;

            // Есть Тип заявки - устанавливаем видимость Контрагент, зависимого от него
            if (card.Sections["PnrPartnerRequests"].Fields["RequestTypeID"] != null)
            {
                int typeID = (int)card.Sections["PnrPartnerRequests"].Fields["RequestTypeID"];
                SetPartnerVisibility(typeID, cardModel);
            }

            // Есть Тип контрагента - устанавливаем видимость контролов, зависимости от него
            //if (card.Sections["PnrPartnerRequests"].Fields["TypeID"] != null)
            //{
            //    int typeID = (int)card.Sections["PnrPartnerRequests"].Fields["TypeID"];
            //    SetPTDependenceVisibility(typeID, cardModel);
            //}
            SetPTDependenceVisibility((int?)card.Sections["PnrPartnerRequests"].Fields["TypeID"], cardModel);

            context.Card.Sections["PnrPartnerRequests"].FieldChanged += (s, e) =>
            {
                // Тип контрагента: Visibility контролов
                //if (e.FieldName == "TypeID" && e.FieldValue != null)
                if (e.FieldName == "TypeID")
                {
                    SetPTDependenceVisibility((int?)e.FieldValue, cardModel);
                    SetIndividualVisibility(card, cardModel);
                    SetNeedFieldValidation(card, cardModel);
                }

                // Тип заявки: установка видимости и доступности полей
                if (e.FieldName == "RequestTypeID" && e.FieldValue != null)
                {
                    SetPTDependenceVisibility((int?)card.Sections["PnrPartnerRequests"].Fields["TypeID"], cardModel);
                    SetPartnerVisibility((int)e.FieldValue, cardModel);
                    SetIndividualVisibility(card, cardModel);
                }

                // Контрагент: подчитываем данные по контрагенту
                if (e.FieldName == "PartnerID" && e.FieldValue != null)
                {
                    GetPartnerInfo((Guid)e.FieldValue, cardModel);
                }

                // Особый признак контрагента: установка необходимости валидации
                if (e.FieldName == "SpecialSignID" && e.FieldValue != null)
                {
                    SetNeedFieldValidation(card, cardModel);
                }

                // Нерезидент: установка необходимости валидации
                if (e.FieldName == "NonResidentID" && e.FieldValue != null)
                {
                    SetNeedFieldValidation(card, cardModel);
                }
            };

            if (context.Model.Controls.TryGet("Files",
                out IControlViewModel FileListModel))
            {
                FileListViewModel FileList = (FileListViewModel) FileListModel;
                foreach (var file in FileList.FileControl.Files)
                {
                    ChangeHelperControlVisibility(cardModel, new FileControlEventArgs(FileList.FileControl.Container, FileList.FileControl, file), Visibility.Collapsed);
                }

                FileList.FileControl.ContainerFileAdded += (sender, e) =>
                {
                    ChangeHelperControlVisibility(cardModel, e, Visibility.Collapsed);
                        
                };
                FileList.FileControl.ContainerFileRemoved += (sender, e) =>
                {
                    ChangeHelperControlVisibility(cardModel, e, Visibility.Visible);
                };
            }

            return base.Initialized(context);
        }

        private void ChangeHelperControlVisibility(ICardModel cardModel, FileControlEventArgs e, Visibility visibility)
        {
            IBlockViewModel currentHelpBlock = GetCurrentHelperBlock(cardModel);
            if(currentHelpBlock == null) return;

            foreach (var control in currentHelpBlock.Controls)
            {
                if (control.CardTypeControl.Type.Name == "Label")
                {
                    if (e.File.Category != null && ((LabelViewModel)control).Text.Contains(e.File.Category.Caption))
                    {
                        control.ControlVisibility = visibility;
                    }
                }
            }
        }

        private IBlockViewModel GetCurrentHelperBlock(ICardModel cardModel)
        {
            if (cardModel.Blocks["FileHelperLegalEntity"].BlockVisibility == Visibility.Visible)
            {
                return cardModel.Blocks["FileHelperLegalEntity"];
            }
            else if (cardModel.Blocks["FileHelperSoleTrader"].BlockVisibility == Visibility.Visible)
            {
                return cardModel.Blocks["FileHelperSoleTrader"];
            }
            else if (cardModel.Blocks["FileHelperIndividual"].BlockVisibility == Visibility.Visible)
            {
                return cardModel.Blocks["FileHelperIndividual"];
            }
            else
            {
                return null;
            }
        }
    }
}
