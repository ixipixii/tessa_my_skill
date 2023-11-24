using System;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Platform.Data;
using Tessa.Extensions.Server.DataHelpers;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PnrPartnerRequestNewExtension : CardNewExtension
    {
        private readonly ICardRepository extendedCardRepository;

        public PnrPartnerRequestNewExtension(ICardRepository extendedCardRepository)
        {
            this.extendedCardRepository = extendedCardRepository;
        }

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            Card card = context.Response.Card;

            // Дата регистрации - текущая дата
            card.Sections["PnrPartnerRequests"].Fields["RegistrationDate"] = DateTime.Now;

            // Страна регистрации - Россия
            card.Sections["PnrPartnerRequests"].Fields["CountryRegistrationID"] = PnrCountries.RussiaID;
            card.Sections["PnrPartnerRequests"].Fields["CountryRegistrationName"] = PnrCountries.RussiaName;

            // Особый признак - Нет
            card.Sections["PnrPartnerRequests"].Fields["SpecialSignID"] = PnrSpecialSigns.NoID;
            card.Sections["PnrPartnerRequests"].Fields["SpecialSignName"] = PnrSpecialSigns.NoName;

            // если в Info был указан ID КА, значит нужно заполнить поля значениями из указанной карточки
            var partnerID =  context.Request.Info.TryGet<Guid?>(PnrInfoKeys.PnrCreatePartnerRequestPartnerID);
            if (partnerID != null)
            {
                await FillCardFromPartner(context, partnerID.Value);
            }
        }

        private async Task FillCardFromPartner(ICardNewExtensionContext context, Guid partnerID)
        {
            // загрузим карточку КА
            var getResponse = await extendedCardRepository.GetAsync(new CardGetRequest()
            {
                CardID = partnerID,
                CardTypeID = DefaultCardTypes.PartnerTypeID
            });

            if (!getResponse.ValidationResult.IsSuccessful())
            {
                context.ValidationResult.AddError($"Не удалось загрузить карточку контрагента, ошибка:{Environment.NewLine}{getResponse.ValidationResult.Build()}");
                return;
            }

            var partnerCard = getResponse.Card;
            var resultCard = context.Response.Card;

            if (!resultCard.Sections.TryGetValue("PnrPartnerRequests", out var res)
                || !partnerCard.Sections.TryGetValue("Partners", out var src))
            {
                return;
            }

            // заявка всегда будет на согласование
            res.Fields["RequestTypeID"] = 1;
            res.Fields["RequestTypeName"] = "Согласование контрагента";

            // КА
            res.Fields["PartnerID"] = partnerCard.ID;
            res.Fields["PartnerName"] = src.Fields.TryGet<string>("Name");

            res.Fields["TypeID"] = src.Fields.TryGet<int?>("TypeID");
            res.Fields["TypeName"] = src.Fields.TryGet<string>("TypeName");

            res.Fields["ShortName"] = src.Fields.TryGet<string>("Name");
            res.Fields["FullName"] = src.Fields.TryGet<string>("FullName");

            // если в КА не установлен атрибут "Особый признак КА": он должен оставаться по умолчанию - "Нет"
            int? specialSignID = src.Fields.TryGet<int?>("SpecialSignID");
            res.Fields["SpecialSignID"] = specialSignID != null ? specialSignID : PnrSpecialSigns.NoID;
            res.Fields["SpecialSignName"] = specialSignID != null ? src.Fields.TryGet<string>("SpecialSignName") : PnrSpecialSigns.NoName;

            res.Fields["NonResident"] = src.Fields.TryGet<bool?>("NonResident");

            res.Fields["INN"] = src.Fields.TryGet<string>("INN");
            res.Fields["KPP"] = src.Fields.TryGet<string>("KPP");

            res.Fields["CountryRegistrationID"] = src.Fields.TryGet<Guid?>("CountryRegistrationID");
            res.Fields["CountryRegistrationName"] = src.Fields.TryGet<string>("CountryRegistrationName");

            res.Fields["Comment"] = src.Fields.TryGet<string>("Comment");


            res.Fields["IdentityDocument"] = src.Fields.TryGet<string>("IdentityDocument");
            res.Fields["IdentityDocumentKind"] = src.Fields.TryGet<string>("IdentityDocumentKind");
            res.Fields["IdentityDocumentIssueDate"] = src.Fields.TryGet<DateTime?>("IdentityDocumentIssueDate");
            res.Fields["IdentityDocumentSeries"] = src.Fields.TryGet<string>("IdentityDocumentSeries");
            res.Fields["IdentityDocumentNumber"] = src.Fields.TryGet<string>("IdentityDocumentNumber");
            res.Fields["IdentityDocumentIssuedBy"] = src.Fields.TryGet<string>("IdentityDocumentIssuedBy");

            res.Fields["OGRN"] = src.Fields.TryGet<string>("OGRN");

            // направление ищем в заявке на создание
            if (res.Fields.TryGet<short?>("DirectionID") == null)
            {
                using (context.DbScope.Create())
                {
                    var db = context.DbScope.Db;

                    db
                        .SetCommand(@"select top 1 DirectionID, DirectionName
                                    from PnrPartnerRequests with(nolock)
                                    where PartnerID = @partnerID
                                        and RequestTypeID = 0", // заявка на создание
                                    // связанный КА
                                    db.Parameter("@partnerID", partnerID));

                    await using(var reader = await db.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            res.Fields["DirectionID"] = reader.GetValue<short?>("DirectionID");
                            res.Fields["DirectionName"] = reader.GetValue<string>("DirectionName");
                        }
                    }
                }
            }
        }
    }
}
