using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.Files;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.FdProcesses.Shared.Fd;
using Tessa.FdProcesses.Shared.Fd.ApprovalList;
using Tessa.Files;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.HistoryList
{
    public sealed class PnrAddHistoryListGetExtension : CardGetExtension
    {
        #region Constructors

        public PnrAddHistoryListGetExtension(ICardFileManager fileManager, ICardCache cardCache)
        {
            this.fileManager = fileManager;
            this.cardCache = cardCache;
        }

        #endregion

        #region Fields

        private readonly ICardFileManager fileManager;
        private readonly ICardCache cardCache;

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;

            ListStorage<CardTaskHistoryItem> taskHistory;
            if (!context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) == null
                || card.StoreMode == CardStoreMode.Insert
                || (taskHistory = card.TryGetTaskHistory()) == null
                || context.CardType == null)
            {
                return;
            }

            bool isEnableForCardType = (
                context.CardType.ID == PnrCardTypes.PnrTenderTypeID ||
                context.CardType.ID == PnrCardTypes.PnrOutgoingTypeID ||
                context.CardType.ID == PnrCardTypes.PnrContractTypeID ||
                context.CardType.ID == PnrCardTypes.PnrRegulationTypeID ||
                context.CardType.ID == PnrCardTypes.PnrPowerAttorneyTypeID ||
                context.CardType.ID == PnrCardTypes.PnrSupplementaryAgreementTypeID ||
                context.CardType.ID == PnrCardTypes.PnrActTypeID ||
                context.CardType.ID == PnrCardTypes.PnrOrderTypeID ||
                context.CardType.ID == PnrCardTypes.PnrServiceNoteTypeID ||
                context.CardType.ID == PnrCardTypes.PnrTemplateTypeID);

            if (!isEnableForCardType)
            {
                return;
            }

            using (ICardFileContainer container = await this.fileManager.CreateContainerAsync(card, cancellationToken: context.CancellationToken))
            {
                await container.FileContainer.AddVirtualAsync(
                        new VirtualFile(
                            FdFileTypes.FdApprovalList,
                            PnrFiles.VirtualFileHistory,
                            FdApprovalListHelper.DefaultFileName),
                        context.CancellationToken,
                        new VirtualFileVersion(
                            PnrFiles.VersionVirtualFileHistory,
                            FdApprovalListHelper.PrintableFileName),
                        new VirtualFileVersion(
                            PnrFiles.VersionVirtualFileHistory,
                            FdApprovalListHelper.DefaultFileName));

                await container.FileContainer.AddVirtualAsync(
                        new VirtualFile(
                            FdFileTypes.FdApprovalList,
                            PnrFiles.VirtualFileHistoryShort,
                            "Лист согласования.html"),
                        context.CancellationToken,
                        new VirtualFileVersion(
                            PnrFiles.VersionVirtualFileHistoryShort,
                            "Печатный лист истории согласования.html"),
                        new VirtualFileVersion(
                            PnrFiles.VersionVirtualFileHistoryShort,
                             "Лист согласования.html"));
            }
            
            #endregion
        }
    }
}
