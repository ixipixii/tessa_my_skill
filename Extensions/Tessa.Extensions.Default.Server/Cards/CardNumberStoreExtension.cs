using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Переносит поля с Primary-номером в поля с Secondary-номером при изменении полей с Primary-номером
    /// (или при первом сохранении), если в типе карточки есть секция DocumentCommonInfo с Secondary-номерами.
    ///
    /// Это сделано расширением на карточку, а не в <c>DocumentNumberDirector</c>, т.к. есть ручное редактирование номера
    /// через контрол, которое тоже должно переносить данные из Primary-полей в Secondary-поля.
    /// </summary>
    public sealed class CardNumberStoreExtension :
        CardStoreExtension
    {
        #region Private Methods

        private static async Task<KrState?> TryGetStateAsync(ICardStoreExtensionContext context, Card card)
        {
            IDbScope dbScope = context.DbScope;

            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var builderFactory = dbScope.BuilderFactory;

                return (KrState?) await db
                    .SetCommand(
                        builderFactory
                            .Select().C("StateID")
                            .From("KrApprovalCommonInfo").NoLock()
                            .Where().C("MainCardID").Equals().P("CardID")
                            .Build(),
                        db.Parameter("CardID", card.ID))
                    .LogCommand()
                    .ExecuteAsync<int?>(context.CancellationToken);
            }
        }

        #endregion

        #region Base Overrides

        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            Card card;
            StringDictionaryStorage<CardSection> sections;
            if (context.CardType == null
                || !DefaultSchemeHelper.CardTypeHasSecondaryNumber(context.CardType)
                || (card = context.Request.TryGetCard()) == null
                || (sections = card.TryGetSections()) == null
                || !sections.TryGetValue("DocumentCommonInfo", out CardSection section))
            {
                return;
            }

            // мы на сервере, поэтому используем RawFields, а не Fields
            Dictionary<string, object> fields = section.RawFields;

            bool hasNumber = fields.TryGetValue("Number", out object number);

            bool hasFullNumber = fields.TryGetValue("FullNumber", out object fullNumber);

            bool hasSequence = fields.TryGetValue("Sequence", out object sequence);

            if (hasNumber || hasFullNumber || hasSequence)
            {
                // в процессе регистрации одновременно изменяется номер и состояние карточки,
                // причём состояние будет записано в той же транзакции в сателлите, но уже после того,
                // как закончится сохранение основной карточки

                KrState? state = null;
                Card satelliteCard;
                Dictionary<string, object> info = card.TryGetInfo();

                bool isTemplateOrCopy = info != null
                    && (info.ContainsKey(CardHelper.CardWasCreatedFromTemplateIDKey)
                        || info.ContainsKey(CardHelper.CardWasCopiedFromIDKey));

                if (!isTemplateOrCopy
                    && (satelliteCard = CardSatelliteHelper.TryGetSatelliteCard(card, KrConstants.KrSatelliteInfoKey)) != null
                    && satelliteCard.Sections.TryGetValue(KrConstants.KrApprovalCommonInfo.Name, out CardSection approvalInfo))
                {
                    state = (KrState?) approvalInfo.RawFields.TryGet<int?>(KrConstants.StateID);
                }

                if (!state.HasValue)
                {
                    state = await TryGetStateAsync(context, card);
                }

                if (state != KrState.Registered)
                {
                    var fieldsToAllow = new List<string>();
                    if (hasNumber)
                    {
                        fields[KrConstants.DocumentCommonInfo.SecondaryNumber] = number;
                        fieldsToAllow.Add(KrConstants.DocumentCommonInfo.SecondaryNumber);
                    }

                    if (hasFullNumber)
                    {
                        fields[KrConstants.DocumentCommonInfo.SecondaryFullNumber] = fullNumber;
                        fieldsToAllow.Add(KrConstants.DocumentCommonInfo.SecondaryFullNumber);
                    }

                    if (hasSequence)
                    {
                        fields[KrConstants.DocumentCommonInfo.SecondarySequence] = sequence;
                        fieldsToAllow.Add(KrConstants.DocumentCommonInfo.SecondarySequence);
                    }

                    if (fieldsToAllow.Count > 0)
                    {
                        await context.SetCardAccessAsync(
                            KrConstants.DocumentCommonInfo.Name,
                            fieldsToAllow);
                    }
                }
            }
        }

        #endregion
    }
}