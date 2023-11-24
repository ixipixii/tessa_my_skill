using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Default.Client.UI
{
    public sealed class KrExtendedPermissionsUIExtension : CardUIExtension
    {
        #region Nested Types

        private class VisibilitySettings
        {
            #region Fields

            public HashSet<string> BlocksForHide;
            public HashSet<string> BlocksForShow;
            public HashSet<string> TabsForHide;
            public HashSet<string> TabsForShow;
            public HashSet<string> ControlsForHide;
            public HashSet<string> ControlsForShow;

            public List<string> BlocksPatternsForHide;
            public List<string> BlocksPatternsForShow;
            public List<string> TabsPatternsForHide;
            public List<string> TabsPatternsForShow;
            public List<string> ControlsPatternsForHide;
            public List<string> ControlsPatternsForShow;

            #endregion

            #region Public Methods

            public void Fill(ICollection<KrPermissionVisibilitySettings> visibilitySettings)
            {
                foreach (var visibilitySetting in visibilitySettings)
                {
                    List<string> patternList;
                    HashSet<string> nameList;
                    switch (visibilitySetting.ControlType)
                    {
                        case KrPermissionsHelper.ControlType.Tab:
                            if (visibilitySetting.IsHidden)
                            {
                                patternList = this.TabsPatternsForHide ??= new List<string>();
                                nameList = this.TabsForHide ??= new HashSet<string>();
                            }
                            else
                            {
                                patternList = this.TabsPatternsForShow ??= new List<string>();
                                nameList = this.TabsForShow ??= new HashSet<string>();
                            }
                            break;

                        case KrPermissionsHelper.ControlType.Block:
                            if (visibilitySetting.IsHidden)
                            {
                                patternList = this.BlocksPatternsForHide ??= new List<string>();
                                nameList = this.BlocksForHide ??= new HashSet<string>();
                            }
                            else
                            {
                                patternList = this.BlocksPatternsForShow ??= new List<string>();
                                nameList = this.BlocksForShow ??= new HashSet<string>();
                            }
                            break;

                        case KrPermissionsHelper.ControlType.Control:
                            if (visibilitySetting.IsHidden)
                            {
                                patternList = this.ControlsPatternsForHide ??= new List<string>();
                                nameList = this.ControlsForHide ??= new HashSet<string>();
                            }
                            else
                            {
                                patternList = this.ControlsPatternsForShow ??= new List<string>();
                                nameList = this.ControlsForShow ??= new HashSet<string>();
                            }
                            break;
                        default: continue;
                    }

                    FillName(patternList, nameList, visibilitySetting.Alias);
                }
            }

            #endregion

            #region Private Methods

            private void FillName(List<string> patternList, HashSet<string> nameList, string alias)
            {
                int length = alias.Length;
                bool wildStart = alias[0] == '*',
                     wildEnd = alias[length - 1] == '*';

                if (!wildStart && !wildEnd)
                {
                    nameList.Add(alias);
                }
                else
                {
                    var escapedAlias =
                        Regex.Escape(alias.Substring(wildStart ? 1 : 0, Math.Max(0, length - (wildStart ? 1 : 0) - (wildEnd ? 1 : 0))));
                    patternList.Add((wildStart ? string.Empty : "^") + escapedAlias + (wildEnd ? string.Empty : "$"));
                }
            }

            #endregion
        }

        private class HideControlsVisitor
        {
            #region Fields

            private HashSet<IFormViewModel> formsForRearange = new HashSet<IFormViewModel>();
            private VisibilitySettings visibilitySettings;

            #endregion

            #region Properties

            public HashSet<Guid> HideSections { get; } = new HashSet<Guid>();
            public HashSet<Guid> HideFields { get; } = new HashSet<Guid>();

            public HashSet<Guid> MandatorySections { get; } = new HashSet<Guid>();
            public HashSet<Guid> MandatoryFields { get; } = new HashSet<Guid>();

            #endregion

            #region Public Methods

            public async Task VisitAsync(ICardModel cardModel)
            {
                this.formsForRearange.Clear();

                foreach(var control in cardModel.ControlBag)
                {
                    this.VisitControl(control);
                }

                foreach(var block in cardModel.BlockBag)
                {
                    this.VisitBlock(block);
                }

                foreach (var form in cardModel.FormBag)
                {
                    await this.VisitFormAsync(cardModel, form);
                }

                foreach (var form in formsForRearange)
                {
                    form.Rearrange();
                }
            }

            public void Fill(
                ICollection<IKrPermissionSectionSettings> sectionSettings,
                VisibilitySettings visibilitySettings)
            {
                if (sectionSettings != null)
                {
                    foreach (var sectionSetting in sectionSettings)
                    {
                        if (sectionSetting.IsHidden)
                        {
                            this.HideSections.Add(sectionSetting.ID);
                        }
                        else
                        {
                            this.HideFields.AddRange(sectionSetting.HiddenFields);
                        }

                        if (sectionSetting.IsMandatory)
                        {
                            this.MandatorySections.Add(sectionSetting.ID);
                        }
                        else
                        {
                            this.MandatoryFields.AddRange(sectionSetting.MandatoryFields);
                        }
                    }
                }

                this.visibilitySettings = visibilitySettings;
            }

            #endregion

            #region Private Methods

            private async Task VisitFormAsync(ICardModel model, IFormViewModel form)
            {
                if (!string.IsNullOrWhiteSpace(form.Name))
                {
                    if (CheckInList(visibilitySettings.TabsForHide, visibilitySettings.TabsPatternsForHide, form.Name))
                    {
                        await HideFormAsync(model, form);
                    }
                    // Нет возможности добавлять скрытую форму, т.к. она не была сгенерирована
                }
            }

            private void VisitBlock(IBlockViewModel block)
            {
                if (!string.IsNullOrWhiteSpace(block.Name))
                {
                    if (CheckInList(visibilitySettings.BlocksForHide, visibilitySettings.BlocksPatternsForHide, block.Name))
                    {
                        HideBlock(block);
                    }
                    else if (block.BlockVisibility == System.Windows.Visibility.Collapsed
                        && CheckInList(visibilitySettings.BlocksForShow, visibilitySettings.BlocksPatternsForShow, block.Name))
                    {
                        ShowBlock(block);
                    }
                }
            }

            private void VisitControl(IControlViewModel control)
            {
                var sourceInfo = control.CardTypeControl.GetSourceInfo();
                if (HideSections.Contains(sourceInfo.SectionID)
                    || sourceInfo.ColumnIDs.Any(x => HideFields.Contains(x)))
                {
                    HideControl(control);
                }

                if (!string.IsNullOrWhiteSpace(control.Name))
                {
                    if (CheckInList(visibilitySettings.ControlsForHide, visibilitySettings.ControlsPatternsForHide, control.Name))
                    {
                        HideControl(control);
                    }
                    else if (control.ControlVisibility == System.Windows.Visibility.Collapsed
                        && CheckInList(visibilitySettings.ControlsForShow, visibilitySettings.ControlsPatternsForShow, control.Name))
                    {
                        ShowControl(control);
                    }
                }

                if (MandatorySections.Contains(sourceInfo.SectionID)
                    || sourceInfo.ColumnIDs.Any(x => MandatoryFields.Contains(x)))
                {
                    MakeControlMandatory(control);
                }
            }

            private void HideControl(IControlViewModel controlViewModel)
            {
                controlViewModel.ControlVisibility = System.Windows.Visibility.Collapsed;
                formsForRearange.Add(controlViewModel.Block.Form);
            }

            private void ShowControl(IControlViewModel controlViewModel)
            {
                controlViewModel.ControlVisibility = System.Windows.Visibility.Visible;
                formsForRearange.Add(controlViewModel.Block.Form);
            }

            private void HideBlock(IBlockViewModel blockViewModel)
            {
                blockViewModel.BlockVisibility = System.Windows.Visibility.Collapsed;
                formsForRearange.Add(blockViewModel.Form);
            }

            private void ShowBlock(IBlockViewModel blockViewModel)
            {
                blockViewModel.BlockVisibility = System.Windows.Visibility.Visible;
                formsForRearange.Add(blockViewModel.Form);
            }

            private async Task HideFormAsync(ICardModel model, IFormViewModel formViewModel)
            {
                if (formViewModel.CardTypeForm.IsTopLevelForm())
                {
                    model.Forms.Remove(formViewModel);
                }
                else
                {
                    await formViewModel.CloseAsync();
                }
            }

            private void MakeControlMandatory(IControlViewModel controlViewModel)
            {
                controlViewModel.IsRequired = true;
                controlViewModel.RequiredText = LocalizationManager.Format("$KrPermissions_MandatoryControlTemplate", controlViewModel.Caption);
            }

            private bool CheckInList(
                HashSet<string> names,
                List<string> patterns,
                string checkName)
            {
                if (names != null
                    && names.Contains(checkName))
                {
                    return true;
                }

                if (patterns != null)
                {
                    foreach (var pattern in patterns)
                    {
                        if (Regex.IsMatch(checkName, pattern))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            #endregion
        }

        #endregion

        #region Base Overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            var token = KrToken.TryGet(context.Card.Info);

            // Если не используется типовое решение
            if (token == null
                || token.ExtendedCardSettings == null)
            {
                return;
            }
            var hideControlsVisitor = new HideControlsVisitor();

            // Набор визиторов по гридам
            Dictionary<GridViewModel, HideControlsVisitor> visitors = new Dictionary<GridViewModel, HideControlsVisitor>();

            // Инициализация видимости контролов по визитору
            async Task InitModelAsync(ICardModel initModel, HideControlsVisitor visitor)
            {
                await visitor.VisitAsync(initModel);

                foreach (var control in initModel.ControlBag)
                {
                    if (control is GridViewModel grid)
                    {
                        visitors[grid] = visitor;
                        grid.RowInitializing += RowInitializaing;
                        grid.RowEditorClosed += RowClosed;
                    }
                }
            }

            // Инициализация вдимости контролов по визитору при открытии строки таблицы
            async void RowInitializaing(object sender, GridRowEventArgs e)
            {
                try
                {
                    if (e.RowModel != null)
                    {
                        await InitModelAsync(e.RowModel, visitors[(GridViewModel)sender]);
                    }
                }
                catch(Exception ex)
                {
                    TessaDialog.ShowException(ex);
                }
            }

            // Отписка от созданных подписок при закрытии строки грида
            void RowClosed(object sender, GridRowEventArgs e)
            {
                if (e.RowModel != null)
                {
                    foreach (var control in e.RowModel.ControlBag)
                    {
                        if (control is GridViewModel grid)
                        {
                            visitors.Remove(grid);
                            grid.RowInitializing -= RowInitializaing;
                            grid.RowEditorClosed -= RowClosed;
                        }
                    }
                }
            }

            var extendedSettings = token.ExtendedCardSettings;
            var sectionSettings = extendedSettings.GetCardSettings();
            var tasksSettings = extendedSettings.GetTasksSettings();
            var visibilitySettings = extendedSettings.GetVisibilitySettings();

            var model = context.Model;

            if ((sectionSettings != null
                && sectionSettings.Count > 0)
                || (visibilitySettings != null
                    && visibilitySettings.Count > 0))
            {
                var uiVisibilitySettings = new VisibilitySettings();
                uiVisibilitySettings.Fill(visibilitySettings);
                hideControlsVisitor.Fill(sectionSettings, uiVisibilitySettings);

                await InitModelAsync(model, hideControlsVisitor);
                if (tasksSettings != null
                    && tasksSettings.Count > 0)
                {
                    await model.ModifyTasksAsync(async (tvm, m) =>
                    {
                        if (!tasksSettings.TryGetValue(tvm.TaskModel.CardTask.TypeID, out var taskSettings))
                        {
                            return;
                        }

                        var taskVisitor = new HideControlsVisitor();
                        taskVisitor.Fill(taskSettings, uiVisibilitySettings);

                        await tvm.ModifyWorkspaceAsync(async (t, b) =>
                        {
                            await InitModelAsync(t.TaskModel, taskVisitor);
                        });
                    });
                }
            }
        }

        #endregion
    }
}
