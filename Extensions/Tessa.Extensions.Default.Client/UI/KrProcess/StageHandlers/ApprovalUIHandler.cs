using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;
using Tessa.Views;
using Tessa.Views.Metadata.Criteria;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class ApprovalUIHandler : StageTypeUIHandlerBase
    {
        private readonly IViewService viewService;

        private CardRow settings;

        private IControlViewModel returnIfNotApprovedFlagControl;
        private IControlViewModel returnAfterApprovalFlagControl;


        public ApprovalUIHandler(IViewService viewService)
        {
            this.viewService = viewService;
        }

        public override Task Initialize(IKrStageTypeUIHandlerContext context)
        {
            IControlViewModel flagsTabs;
            IFormViewModel tabViewModel;
            IBlockViewModel innerFlagsBlock;

            if (context.RowModel.Blocks.TryGet("ApprovalStageFlags", out var flagsBlock)
                && (flagsTabs = flagsBlock.Controls.FirstOrDefault(p => p.Name == "FlagsTabs")) != null
                && flagsTabs is TabControlViewModel flagsTabsViewModel)
            {
                if ((tabViewModel = flagsTabsViewModel.Tabs.FirstOrDefault(p => p.Name == "CommonSettings")) != null
                    && (innerFlagsBlock = tabViewModel.Blocks.FirstOrDefault(p => p.Name == "StageFlags")) != null)
                {
                    this.returnIfNotApprovedFlagControl = innerFlagsBlock.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.ReturnIfNotApproved);
                }

                if ((tabViewModel = flagsTabsViewModel.Tabs.FirstOrDefault(p => p.Name == "AdditionalSettings")) != null
                    && (innerFlagsBlock = tabViewModel.Blocks.FirstOrDefault(p => p.Name == "StageFlags")) != null)
                {
                    this.returnAfterApprovalFlagControl = innerFlagsBlock.Controls.FirstOrDefault(p => p.Name == KrConstants.Ui.ReturnAfterApproval);

                }
            }

            this.settings = context.Row;
            this.settings.FieldChanged += this.OnSettingsFieldChanged;

            AdvisoryConfigureFields(this.settings.TryGet<bool>(KrConstants.KrApprovalSettingsVirtual.Advisory));
            NotReturnEditConfigureFields(this.settings.TryGet<bool>(KrConstants.KrApprovalSettingsVirtual.NotReturnEdit));

            return Task.CompletedTask;
        }

        public override Task Finalize(IKrStageTypeUIHandlerContext context)
        {
            this.settings.FieldChanged -= this.OnSettingsFieldChanged;
            this.settings = null;

            return Task.CompletedTask;
        }

        private void OnSettingsFieldChanged(object sender, CardFieldChangedEventArgs e)
        {
            if (e.FieldName == KrConstants.KrApprovalSettingsVirtual.Advisory
                && e.FieldValue is bool advisory)
            {
                this.AdvisoryConfigureFields(advisory);

                if (advisory)
                {
                    if (this.settings.Fields.TryGet<Guid?>(KrConstants.KrTaskKindSettingsVirtual.KindID) == null)
                    {
                        var (kindID, kindCaption) = GetKindAsync(KrConstants.AdvisoryTaskKindID).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (kindID != default)
                        {
                            this.settings.Fields[KrConstants.KrTaskKindSettingsVirtual.KindID] = kindID;
                            this.settings.Fields[KrConstants.KrTaskKindSettingsVirtual.KindCaption] = kindCaption;
                        }
                    }
                }
                else
                {
                    if (this.settings.Fields.TryGet<Guid?>(KrConstants.KrTaskKindSettingsVirtual.KindID) ==
                        KrConstants.AdvisoryTaskKindID)
                    {
                        this.settings.Fields[KrConstants.KrTaskKindSettingsVirtual.KindID] = null;
                        this.settings.Fields[KrConstants.KrTaskKindSettingsVirtual.KindCaption] = null;
                    }

                    if (this.returnIfNotApprovedFlagControl != null)
                    {
                        this.returnIfNotApprovedFlagControl.IsReadOnly = false;
                    }
                }

                return;
            }

            if (e.FieldName == KrConstants.KrApprovalSettingsVirtual.NotReturnEdit
                && e.FieldValue is bool notReturnEdit)
            {
                this.NotReturnEditConfigureFields(notReturnEdit);
            }
        }

        private async Task<(Guid, string)> GetKindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ITessaView taskKindsView = await this.viewService.GetByNameAsync(KrConstants.Views.TaskKinds, cancellationToken);

            var request = new TessaViewRequest(taskKindsView.Metadata);

            var idParam = new RequestParameterBuilder()
                .WithMetadata(taskKindsView.Metadata.Parameters.FindByName("ID"))
                .AddCriteria(new EqualsCriteriaOperator(), string.Empty, id)
                .AsRequestParameter();

            request.Values.Add(idParam);

            var result = await taskKindsView.GetDataAsync(request, cancellationToken).ConfigureAwait(false);

            if (result.Rows == null
                || result.Rows.Count == 0)
            {
                return default;
            }

            var row = (IList<object>)result.Rows[0];
            return ((Guid) row[0], (string) row[1]);
        }

        /// <summary>
        /// Задаёт настройки полям в соответствии со значением флага "Рекомендательное согласование".
        /// </summary>
        /// <param name="isAdvisory">Значение флага "Рекомендательное согласование".</param>
        private void AdvisoryConfigureFields(bool isAdvisory)
        {
            if (isAdvisory)
            {
                if (this.returnIfNotApprovedFlagControl != null)
                {
                    this.returnIfNotApprovedFlagControl.IsReadOnly = true;
                    this.settings.Fields[KrConstants.KrApprovalSettingsVirtual.ReturnWhenDisapproved] = false;
                }
            }
        }

        /// <summary>
        /// Настраивает поля взависимости от значения флага.
        /// </summary>
        /// <param name="isNotReturnEdit">Значение флага <see cref=" KrConstants.KrApprovalSettingsVirtual.NotReturnEdit"/>.</param>
        private void NotReturnEditConfigureFields(bool isNotReturnEdit)
        {
            if (isNotReturnEdit)
            {
                this.returnIfNotApprovedFlagControl.ControlVisibility = System.Windows.Visibility.Collapsed;
                this.returnAfterApprovalFlagControl.ControlVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.returnIfNotApprovedFlagControl.ControlVisibility = System.Windows.Visibility.Visible;
                this.returnAfterApprovalFlagControl.ControlVisibility = System.Windows.Visibility.Visible;
            }
        }
    }
}