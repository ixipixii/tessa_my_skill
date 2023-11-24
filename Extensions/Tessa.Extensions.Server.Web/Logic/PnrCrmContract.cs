using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Models;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Web.Models
{
    /// <summary>
    /// Договор (интеграция с CRM)
    /// </summary>
    public partial class PnrCrmContractRequest : PnrBaseRequest
    {
        private void SetCardFdState(Card card, PnrFdCardState state)
        {
            if (state == null)
            {
                return;
            }

            // будем проставлять статус в вирт. секцию
            // в расширении движка FdSaveMainCardStoreExtension.FillSatellite он сохранится в сателлите
            var fscivSection = card.Sections.GetOrAdd("FdSatelliteCommonInfoVirtual");

            fscivSection.Fields["StateID"] = state.ID;
            fscivSection.Fields["StateName"] = state.Name;
        }

        private void SetCardFdStateFromCrm(Card card, string crmContractStatus)
        {
            if (string.IsNullOrEmpty(crmContractStatus))
            {
                return;
            }

            switch (crmContractStatus)
            {
                // Действует
                case "1":
                    SetCardFdState(card, PnrFdCardStates.PnrContractActing);
                    break;
                // На подписании
                case "2":
                    SetCardFdState(card, PnrFdCardStates.PnrContractOnSigning);
                    break;
            }
        }

        public override Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return Task.FromResult(GetGuidFromString(this.ProjectUId));
        }

        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope)
        {
            try
            {
                if (int.Parse(this.PioneerContractType) == 1)
                {
                    return PnrCardTypes.PnrSupplementaryAgreementTypeID;
                }
                return PnrCardTypes.PnrContractTypeID;
            }
            catch (Exception)
            {
                var cardID = await GetCardIDAsync(dbScope);
                return cardID != null ? await PnrCardHelper.GetCardTypeIDByCardID(dbScope, cardID.Value) : null;
            }
        }

        public override string GetDescription()
        {
            return GetTrimmedString(this.Number);
        }

        public override async Task FillCardDataAsync(IDbScope dbScope, Card card, IValidationResultBuilder validationResult)
        {
            if (card.Sections.TryGetValue("DocumentCommonInfo", out var dciSection)
                &&
                // основная секция будет разной в зависимости от типа карточки
                card.TypeID == PnrCardTypes.PnrContractTypeID
                ? card.Sections.TryGetValue("PnrContracts", out var mainSection)
                : card.Sections.TryGetValue("PnrSupplementaryAgreements", out mainSection))
            {
                // все поля, пришедшие в запросе

                // Внешний номер
                var externalNumber = GetTrimmedString(this.Number);
                if (!string.IsNullOrEmpty(externalNumber))
                {
                    mainSection.Fields["ExternalNumber"] = externalNumber;
                }

                // Тема / Предмет договора
                var subject = GetTrimmedString(this.Title);
                if (!string.IsNullOrEmpty(subject))
                {
                    dciSection.Fields["Subject"] = subject;
                    mainSection.Fields["Subject"] = subject;
                }

                // комментарий
                var comment = GetTrimmedString(this.Annotation);
                if (!string.IsNullOrEmpty(comment))
                {
                    mainSection.Fields["Comment"] = comment;
                }

                // Дата проекта
                var projectDate = GetDateFromString(this.ProjectDate);
                if (projectDate != null)
                {
                    mainSection.Fields["ProjectDate"] = projectDate;
                }

                // Дата начала
                var startDate = GetDateFromString(this.ProjectStartDate);
                if (startDate != null)
                {
                    mainSection.Fields["StartDate"] = startDate;
                }

                // Ссылка на карточку договора в CRM
                var linkCardCRM = GetTrimmedString(this.CRMUrl);
                if (!string.IsNullOrEmpty(linkCardCRM))
                {
                    mainSection.Fields["LinkCardCRM"] = linkCardCRM;
                }

                // Организация ГК Пионер
                if (!string.IsNullOrEmpty(this.PionerLegalEntity))
                {
                    var organization = await PnrOrganizationHelper.GetOrganizationByMDM(dbScope, this.PionerLegalEntity);
                    if (organization != null)
                    {
                        mainSection.Fields["OrganizationID"] = organization.ID;
                        mainSection.Fields["OrganizationName"] = organization.Name;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти организацию. MDM-Key={this.PionerLegalEntity}.");
                    }
                }

                // Проект
                if (!string.IsNullOrEmpty(this.RubricLink))
                {
                    var project = await PnrProjectHelper.GetProjectByMDM(dbScope, this.RubricLink);
                    if (project != null)
                    {
                        mainSection.Fields["ProjectID"] = project.ID;
                        mainSection.Fields["ProjectName"] = project.Name;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти проект. MDM-Key={this.RubricLink}.");
                    }
                }

                // MDM-key
                if (!string.IsNullOrEmpty(this.MDMKey))
                {
                    mainSection.Fields["MDMKey"] = this.MDMKey;
                }

                // Контрагент
                if (!string.IsNullOrEmpty(this.CorrespondentMDMKey))
                {
                    var partner = await PnrPartnerHelper.GetPartnerByMDM(dbScope, this.CorrespondentMDMKey);
                    if (partner != null)
                    {
                        mainSection.Fields["PartnerID"] = partner.ID;
                        mainSection.Fields["PartnerName"] = partner.Name;
                    }
                    else
                    {
                        validationResult.AddWarning($"Не удалось найти контрагента. MDM-Key={this.CorrespondentMDMKey}.");
                    }
                }

                // Статус
                SetCardFdStateFromCrm(card, this.CRMContractStatus);

                // Гиперссылка на карточку
                var hyperlinkCard = GetTrimmedString(this.MigrateFilesFromUrl);
                if (!string.IsNullOrEmpty(hyperlinkCard))
                {
                    mainSection.Fields["HyperlinkCard"] = hyperlinkCard;
                }

                // Код объекта недвижимости
                var flat = GetTrimmedString(this.Flat);
                if (!string.IsNullOrEmpty(flat))
                {
                    mainSection.Fields["Flat"] = flat;
                }

                // Флаг Согласование бухгалтерией
                var crmContractApprove = GetTrimmedString(this.CRMContractApprove);
                if (!string.IsNullOrEmpty(crmContractApprove))
                {
                    mainSection.Fields["CRMContractApprove"] = GetBoolFromString(crmContractApprove);
                }

                // далее оставшиеся обязательные поля (только при создании карточки)
                if (card.StoreMode == CardStoreMode.Insert)
                {
                    // Вид договора
                    mainSection.Fields["KindID"] = new Guid("7ede7958-e642-490c-b458-32c034ccb9d6");
                    mainSection.Fields["KindName"] = "С покупателями";
                    // ДС
                    if (card.TypeID == PnrCardTypes.PnrSupplementaryAgreementTypeID)
                    {
                        // Основной договор
                        Guid? parentContractID = GetGuidFromString(this.ParentProjectUId);
                        if (parentContractID != null)
                        {
                            var parentContract = await PnrContractHelper.GetContractByID(dbScope, parentContractID.Value);
                            if (parentContract != null)
                            {
                                mainSection.Fields["MainContractID"] = parentContract.ID;
                                mainSection.Fields["MainContractSubject"] = parentContract.Subject;
                                if (card.Sections.TryGetValue("OutgoingRefDocs", out var outgoingRefDocs))
                                {
                                    CardRow row = outgoingRefDocs.Rows.Add();
                                    row.State = CardRowState.Inserted;
                                    row.RowID = Guid.NewGuid();
                                    row.Fields["DocID"] = parentContract.ID;
                                    row.Fields["DocDescription"] = parentContract.Subject;
                                    row.Fields["Order"] = 0;
                                }
                            }
                            else
                            {
                                validationResult.AddError($"Не удалось найти договор. ID={parentContractID.Value}.");
                            }
                        }
                    }
                }
            }
        }

        public override PnrBaseResponse GetSuccessResult(Logger logger, string message, Card card, ISession session)
        {
            if (!string.IsNullOrEmpty(message))
            {
                logger.Info(message);
            }

            var cardLink = CardHelper.GetLink(session, card.ID);

            return new PnrCrmContractResponse(
                    PnrBaseResponseStatusCode.Success,
                    message,
                    new PnrCrmContractResponseValue()
                    {
                        ProjectId = card.ID.ToString(),
                        ProjectUId = card.ID.ToString(),
                        ProjectUrl = cardLink,
                        ProjectDisplayFormUrl = cardLink,
                        ProjectFilesFolderUId = card.ID.ToString()
                    });
        }

        public override PnrBaseResponse GetErrorResult(Logger logger, string message, string prefix = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                logger.Error(prefix + message);
            }
            return new PnrCrmContractResponse(PnrBaseResponseStatusCode.Error, message, new PnrCrmContractResponseValue());
        }
    }
}
