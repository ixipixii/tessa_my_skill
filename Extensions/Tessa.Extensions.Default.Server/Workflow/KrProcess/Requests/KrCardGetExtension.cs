using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrPermissions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Requests
{
    public sealed class KrCardGetExtension: CardGetExtension
    {
        #region fields

        private readonly IKrTypesCache typesCache;
        private readonly IKrStageSerializer stageSerializer;
        private readonly IKrScope krScope;
        private readonly IKrPermissionsManager permissionsManager;

        #endregion

        #region constructor

        public KrCardGetExtension(
            IKrTypesCache typesCache,
            IKrStageSerializer stageSerializer,
            IKrScope krScope,
            IKrPermissionsManager permissionsManager)
        {
            this.typesCache = typesCache;
            this.stageSerializer = stageSerializer;
            this.krScope = krScope;
            this.permissionsManager = permissionsManager;
        }

        #endregion

        #region private

        private Task FillSectionsAsync(
            Card main,
            Card satellite,
            KrProcessSerializerHiddenStageMode hiddenStageMode,
            ICardGetExtensionContext context)
        {
            new KrProcessSectionMapper(satellite, main)
                .Map(KrApprovalCommonInfo.Name, KrApprovalCommonInfo.Virtual, modifyAction: RemoveRedundantKeysAci)
                .Map(KrActiveTasks.Name, KrActiveTasks.Virtual)
                .Map(KrApprovalHistory.Name, KrApprovalHistory.Virtual)
                ;

            return this.stageSerializer.DeserializeSectionsAsync(
                satellite,
                main,
                hiddenStageMode: hiddenStageMode,
                cardContext: context,
                cancellationToken: context.CancellationToken);
        }

        private static void RemoveRedundantKeysAci(
            CardSection sec,
            IDictionary<string, object> storage)
        {
            storage.Remove(Info);
            storage.Remove(KrApprovalCommonInfo.CurrentHistoryGroup);
            storage.Remove(KrProcessCommonInfo.NestedWorkflowProcesses);
        }

        #endregion

        #region base overrides

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (context.Request.IsKrSatelliteIgnored()
                || context.CardType == null
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.ValidationResult.IsSuccessful()
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }

            var components = KrComponentsHelper.GetKrComponents(card, this.typesCache);
            if (components.Has(KrComponents.Routes))
            {
                KrProcessHelper.SetStageDefaultValues(context.Response);
            }

            if (!await KrProcessHelper.SatelliteExistsAsync(card.ID, context.DbScope, context.CancellationToken))
            {
                return;
            }

            var satellite = this.krScope.GetKrSatellite(card.ID, context.ValidationResult);
            if (satellite == null)
            {
                return;
            }

            KrProcessSerializerHiddenStageMode hiddenStageMode;

            if (context.Request.ConsiderSkippedStages())
            {
                var permContext = await permissionsManager.TryCreateContextAsync(
                    new KrPermissionsCreateContextParams
                    {
                        Card = context.Response.Card,
                        WithRequiredPermissions = false,
                        WithExtendedPermissions = false,
                        ValidationResult = context.ValidationResult,
                        AdditionalInfo = context.Info,
                        PrevToken = KrToken.TryGet(context.Request.Info),
                        ExtensionContext = context,
                    },
                    cancellationToken: context.CancellationToken).ConfigureAwait(false);

                hiddenStageMode =
                    await this.permissionsManager.CheckRequiredPermissionsAsync(
                        permContext,
                        KrPermissionFlagDescriptors.EditRoute)
                    ? KrProcessSerializerHiddenStageMode.ConsiderOnlySkippedStages
                    : KrProcessSerializerHiddenStageMode.Consider;
            }
            else
            {
                hiddenStageMode =
                    context.Request.ConsiderHiddenStages()
                    ? KrProcessSerializerHiddenStageMode.Consider
                    : KrProcessSerializerHiddenStageMode.Ignore;
            }

            await this.FillSectionsAsync(card, satellite, hiddenStageMode, context).ConfigureAwait(false);
        }

        #endregion
    }
}