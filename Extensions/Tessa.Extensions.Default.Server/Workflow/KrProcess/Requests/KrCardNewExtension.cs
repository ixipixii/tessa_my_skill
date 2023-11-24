using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public class KrCardNewExtension : CardNewExtension
    {
        #region fields

        private readonly IKrTypesCache typesCache;

        private readonly IKrStageSerializer stageSerializer;

        private readonly ISignatureProvider signatureProvider;

        #endregion

        #region constructor

        public KrCardNewExtension(IKrTypesCache typesCache,
            IKrStageSerializer stageSerializer,
            ISignatureProvider signatureProvider)
        {
            this.typesCache = typesCache;
            this.stageSerializer = stageSerializer;
            this.signatureProvider = signatureProvider;
        }

        #endregion

        #region private

        private static void SetDocType(CardNewRequest request, Card card)
        {
            if (request.Info.TryGetValue(Keys.DocTypeID, out var docTypeIDObj)
                && docTypeIDObj is Guid docTypeID
                && card.Sections.TryGetValue(DocumentCommonInfo.Name, out var dci))
            {
                dci.RawFields[DocumentCommonInfo.DocTypeID] = docTypeID;
                dci.RawFields[DocumentCommonInfo.DocTypeTitle] = request.Info.TryGet(Keys.DocTypeTitle, string.Empty);
            }
        }

        private static void FillDocumentCommonInfo(
            Card card,
            CardType cardType,
            ISession currentSession)
        {
            if (!card.Sections.TryGetValue(DocumentCommonInfo.Name, out var dci))
            {
                return;
            }
            var fields = dci.RawFields;

            if (fields.ContainsKey(DocumentCommonInfo.CardTypeID))
            {
                fields[DocumentCommonInfo.CardTypeID] = cardType.ID;
            }
            if (fields.ContainsKey(DocumentCommonInfo.AuthorID))
            {
                fields[DocumentCommonInfo.AuthorID] = currentSession.User.ID;
            }
            if (fields.ContainsKey(DocumentCommonInfo.AuthorName))
            {
                fields[DocumentCommonInfo.AuthorName] = currentSession.User.Name;
            }
            if (fields.ContainsKey(DocumentCommonInfo.RegistratorID))
            {
                fields[DocumentCommonInfo.RegistratorID] = currentSession.User.ID;
            }
            if (fields.ContainsKey(DocumentCommonInfo.RegistratorName))
            {
                fields[DocumentCommonInfo.RegistratorName] = currentSession.User.Name;
            }

            DateTime utcNow = DateTime.UtcNow;
            if (fields.ContainsKey(DocumentCommonInfo.CreationDate))
            {
                fields[DocumentCommonInfo.CreationDate] = utcNow;
            }
            if (fields.ContainsKey(DocumentCommonInfo.DocDate))
            {
                fields[DocumentCommonInfo.DocDate] = (utcNow + currentSession.ClientUtcOffset).Date;
            }
        }

        private static void FillApprovalCommonInfoTemplate(Card card)
        {
            if (!card.TryGetKrApprovalCommonInfoSection(out var aci))
            {
                return;
            }

            aci.Fields[StateID] = (int)KrState.Draft;
            aci.Fields[StateName] = KrState.Draft.TryGetDefaultName();
            aci.Fields[KrApprovalCommonInfo.StateChangedDateTimeUTC] = null;

            aci.Fields[KrProcessCommonInfo.MainCardID] = null;
            aci.Fields[KrProcessCommonInfo.CurrentApprovalStageRowID] = null;
            aci.Fields[KrApprovalCommonInfo.ApprovedBy] = null;
            aci.Fields[KrApprovalCommonInfo.DisapprovedBy] = null;
            aci.Fields[KrApprovalCommonInfo.AuthorID] = null;
            aci.Fields[KrApprovalCommonInfo.AuthorName] = null;

            KrProcessHelper.SetInactiveStateToStages(card);
        }

        #endregion

        #region base overrides

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            Card card;
            if (context.CardType == null
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.ValidationResult.IsSuccessful()
                || (card = context.Response.TryGetCard()) == null
                || !KrComponentsHelper.HasBase(context.CardType.ID, this.typesCache))
            {
                return;
            }

            SetDocType(context.Request, card);
            FillDocumentCommonInfo(card, context.CardType, context.Session);

            if (context.Method == CardNewMethod.Template)
            {
                var satellite = CardSatelliteHelper.TryGetSatelliteCard(card, KrSatelliteInfoKey);
                if (satellite != null)
                {
                    card.Info.Remove(KrSatelliteInfoKey);
                    new KrProcessSectionMapper(satellite, context.Response.Card)
                        .Map(KrApprovalCommonInfo.Name, KrApprovalCommonInfo.Virtual, modifyAction: (_, p) => p.Remove(Info))
                        ;

                    await StageRowMigrationHelper.MigrateAsync(
                        satellite,
                        card,
                        KrProcessSerializerHiddenStageMode.ConsiderWithStoringCardRows,
                        this.stageSerializer,
                        this.signatureProvider,
                        context.CancellationToken);
                }
                FillApprovalCommonInfoTemplate(card);
            }

            var components = KrComponentsHelper.GetKrComponents(card, this.typesCache);
            if (components.Has(KrComponents.Routes))
            {
                KrProcessHelper.SetStageDefaultValues(context.Response);
            }
        }

        #endregion
    }
}