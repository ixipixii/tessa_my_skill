using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.UI.Cards;

namespace Tessa.Extensions.Default.Client.UI.KrProcess
{
    public sealed class KrSecondaryProcessUIExtension : CardUIExtension
    {
        #region fields

        private static readonly int pureProcess = KrConstants.KrSecondaryProcessModes.PureProcess.ID;
        private static readonly int button = KrConstants.KrSecondaryProcessModes.Button.ID;
        private static readonly int action = KrConstants.KrSecondaryProcessModes.Action.ID;

        private ICardModel model;

        #endregion

        #region base overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            this.model = context.Model;

            var currentMode = this.GetCurrentMode();
            this.UpdateVisibility(currentMode);
            if (currentMode == pureProcess)
            {
                this.UpdateCheckRestrictions();
            }
            context.Card
                .Sections[KrConstants.KrSecondaryProcesses.Name]
                .FieldChanged += this.FieldChanged;
        }

        public override async Task Finalized(ICardUIExtensionContext context)
        {
            context.Card
                .Sections[KrConstants.KrSecondaryProcesses.Name]
                .FieldChanged -= this.FieldChanged;
            this.model = null;
        }

        #endregion

        #region private

        private void FieldChanged(object sender, CardFieldChangedEventArgs e)
        {
            switch (e.FieldName)
            {
                case KrConstants.KrSecondaryProcesses.ModeID:
                    this.UpdateVisibility(this.GetCurrentMode());
                    break;
                case KrConstants.KrSecondaryProcesses.AllowClientSideLaunch:
                    this.UpdateVisibiltyForPureProcessMode();
                    break;
                case KrConstants.KrSecondaryProcesses.CheckRecalcRestrictions:
                    this.UpdateCheckRestrictions();
                    break;
            }
        }

        private void UpdateVisibility(int currentMode)
        {
            Visibility GetVisibility(int allowedMode, int allowedMode2 = -2) => 
                allowedMode == currentMode || allowedMode2 == currentMode
                ? Visibility.Visible
                : Visibility.Collapsed;

            var blocks = this.model.Blocks;
            blocks[KrConstants.Ui.PureProcessParametersBlock].BlockVisibility = GetVisibility(pureProcess);
            blocks[KrConstants.Ui.TileParametersBlock].BlockVisibility = GetVisibility(button);
            blocks[KrConstants.Ui.ActionParametersBlock].BlockVisibility = GetVisibility(action);
            blocks[KrConstants.Ui.VisibilityScriptsBlock].BlockVisibility = GetVisibility(button);

            blocks[KrConstants.Ui.RestictionsBlock].BlockVisibility = Visibility.Visible;
            blocks[KrConstants.Ui.ExecutionAccessDeniedBlock].BlockVisibility = Visibility.Visible;
            blocks[KrConstants.Ui.ExecutionScriptsBlock].BlockVisibility = Visibility.Visible;

            if (currentMode == pureProcess)
            {
                this.UpdateVisibiltyForPureProcessMode();
            }
        }

        private void UpdateVisibiltyForPureProcessMode()
        {
            var card = this.model.Card;
            var sec = card.Sections[KrConstants.KrSecondaryProcesses.Name];
            var allowClientSideLaunch =
                sec.Fields.TryGet<bool?>(KrConstants.KrSecondaryProcesses.AllowClientSideLaunch) ?? false;
            var checkRecalcControl = this.model
                .Blocks[KrConstants.Ui.PureProcessParametersBlock]
                .Controls
                .First(p => p.Name == KrConstants.Ui.CheckRecalcRestrictionsCheckbox);

            checkRecalcControl.IsReadOnly = allowClientSideLaunch;
            var checkRecalcRestrictions =
                sec.Fields.TryGet<bool?>(KrConstants.KrSecondaryProcesses.CheckRecalcRestrictions) ?? false;
            if (allowClientSideLaunch 
                && !checkRecalcRestrictions)
            {
                sec.Fields[KrConstants.KrSecondaryProcesses.CheckRecalcRestrictions] = BooleanBoxes.True;
            }
            else if (!allowClientSideLaunch
                && !checkRecalcRestrictions)
            {
                this.UpdateCheckRestrictions();
            }
        }

        private void UpdateCheckRestrictions()
        {
            var blocks = this.model.Blocks;
            var card = this.model.Card;
            var checkRecalcRestrictions = card
                .Sections[KrConstants.KrSecondaryProcesses.Name]
                .Fields
                .TryGet<bool?>(KrConstants.KrSecondaryProcesses.CheckRecalcRestrictions) ?? false;
            var visibilityForRestrictionFields = checkRecalcRestrictions
                ? Visibility.Visible
                : Visibility.Collapsed;
            blocks[KrConstants.Ui.RestictionsBlock].BlockVisibility = visibilityForRestrictionFields;
            blocks[KrConstants.Ui.ExecutionAccessDeniedBlock].BlockVisibility = visibilityForRestrictionFields;
            blocks[KrConstants.Ui.ExecutionScriptsBlock].BlockVisibility = visibilityForRestrictionFields;

            if (!checkRecalcRestrictions)
            {
                var sec = card.Sections[KrConstants.KrSecondaryProcesses.Name];
                sec.Fields[KrConstants.KrSecondaryProcesses.ExecutionAccessDeniedMessage] = null;
                sec.Fields[KrConstants.KrSecondaryProcesses.ExecutionSqlCondition] = null;
                sec.Fields[KrConstants.KrSecondaryProcesses.ExecutionSourceCondition] = null;

                Clear(KrConstants.KrStageDocStates.Name);
                Clear(KrConstants.KrStageTypes.Name);
                Clear(KrConstants.KrStageRoles.Name);
                Clear(KrConstants.KrSecondaryProcessContextRoles.Name);
                void Clear(string name)
                {
                    var rows = card.Sections[name].Rows;
                    rows.RemoveAll(p => p.State == CardRowState.Inserted);
                    foreach (var row in rows)
                    {
                        row.State = CardRowState.Deleted;
                    }
                }
            }
        }

        private int GetCurrentMode() => this.model.Card
            .Sections[KrConstants.KrSecondaryProcesses.Name]
            .Fields
            .TryGet<int?>(KrConstants.KrSecondaryProcesses.ModeID) ?? -1;

        #endregion
    }
}