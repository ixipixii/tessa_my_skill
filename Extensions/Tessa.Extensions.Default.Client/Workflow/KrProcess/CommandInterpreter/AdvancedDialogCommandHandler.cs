using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.Extensions.Platform.Client.UI;
using Tessa.UI;
using Tessa.UI.Cards;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.CommandInterpreter
{
    public sealed class AdvancedDialogCommandHandler : ClientCommandHandlerBase
    {
        #region Fields

        private readonly IKrProcessLauncher launcher;

        private readonly IAdvancedCardDialogManager advancedCardDialogManager;

        #endregion

        #region Constructor

        public AdvancedDialogCommandHandler(
            IKrProcessLauncher launcher,
            IAdvancedCardDialogManager advancedCardDialogManager)
        {
            this.launcher = launcher;
            this.advancedCardDialogManager = advancedCardDialogManager;
        }

        #endregion

        #region Base Overrides

        /// <inheritdoc />
        public override void Handle(
            IClientCommandHandlerContext context)
        {
            var parameters = context.Command.Parameters;

            if (!parameters.TryGetValue(KrConstants.Keys.ProcessInstance, out var instanceStorageObj)
                || !(instanceStorageObj is Dictionary<string, object> instanceStorage))
            {
                return;
            }
            var processInstance = new KrProcessInstance(instanceStorage);
            
            if (!parameters.TryGetValue(KrConstants.Keys.CompletionOptionSettings, out var coSettingsObj)
                || !(coSettingsObj is Dictionary<string, object> coSettingsStorage))
            {
                return;
            }
            var coSettings = new CardTaskCompletionOptionSettings(coSettingsStorage);

            var editor = UIContext.Current.CardEditor;
            if (editor != null)
            {
                editor.Info[CardTaskDialogUIExtension.DialogNonTaskCompletionOptionSettingsKey] = coSettings;
                editor.Info[CardTaskDialogUIExtension.OnDialogButtonPressedKey] = 
                    (Func<ICardEditorModel, CardTaskCompletionOptionSettings, string, bool, Task>)(
                        async (
                            dialogCardEditor,
                            cos,
                            buttonName,
                            cancel) => await this.CompleteDialogAsync(dialogCardEditor, editor, cos, buttonName, processInstance));
            }
            else
            {
                var _ = ShowGlobalDialog(processInstance, coSettings);
            }
        }

        #endregion

        #region Private

        private static T Get<T>(CardTaskCompletionOptionSettings coSettings, string key, Func<object> defaultValueFunc)
        {
            var storage = coSettings.GetStorage();
            object value = storage[key];
            if (value != null)
            {
                return (T)value;
            }

            object defaultValue = defaultValueFunc();
            if (defaultValue != null)
            {
                storage[key] = defaultValue;
            }

            return (T)defaultValue;
        }

        private static Dictionary<string, object> GetInfo(CardTaskCompletionOptionSettings settings) =>
            Get<Dictionary<string, object>>(settings, "Info",
                () => new Dictionary<string, object>(StringComparer.Ordinal));

        private async Task ShowGlobalDialog(
            KrProcessInstance krProcessInstance, 
            CardTaskCompletionOptionSettings coSettings,
            CancellationToken cancellationToken = default)
        {
            var info = GetInfo(coSettings);
            if (coSettings.PreparedNewCard != null
                && coSettings.PreparedNewCardSignature != null)
            {
                info[CardHelper.NewCardBilletKey] = coSettings.PreparedNewCard;
                info[CardHelper.NewCardBilletSignatureKey] = coSettings.PreparedNewCardSignature;
            }
            
            await this.advancedCardDialogManager.CreateCardAsync(
                coSettings.DialogTypeID,
                options: new CreateCardOptions
                {
                    CardEditorModifierActionAsync = async (ctx) =>
                    {
                        var uiContext = UIContext.Current;
                        uiContext.SetDialogClosingAction((uiCtx, eventArgs) =>
                        {
                            if (uiCtx.CardEditor.OperationInProgress)
                            {
                                eventArgs.Cancel = true;
                            }
                            return Task.FromResult(false);
                        });
                        this.PrepareDialog(ctx.Editor, coSettings, krProcessInstance);
                    },
                    Info = info,
                }, 
                cancellationToken: cancellationToken);
        }
        
         private void PrepareDialog(
            ICardEditorModel editor,
            CardTaskCompletionOptionSettings coSettings,
            KrProcessInstance instance)
         {
            editor.StatusBarIsVisible = false;
            if (coSettings.DialogName != null)
            {
                editor.DialogName = coSettings.DialogName;
            }
            
            var actions = editor.Toolbar.Actions;
            var bottomActions = editor.BottomToolbar.Actions;
            var bottomButtons = editor.BottomDialogButtons;
            actions.Clear();
            bottomActions.Clear();
            bottomButtons.Clear();

            foreach (var actionInfo in coSettings.Buttons)
            {
                var name = actionInfo.Name;
                var cancel = actionInfo.Cancel;
                object icon = null;
                if (!string.IsNullOrEmpty(actionInfo.Icon))
                {
                    icon = editor.Toolbar.CreateIcon(actionInfo.Icon);
                }
                switch (actionInfo.CardButtonType)
                {
                    case CardButtonType.ToolbarButton:
                        actions.Add(new CardToolbarAction(
                            name,
                            actionInfo.Caption,
                            icon,
                            new DelegateCommand(GetButtonAction(name, cancel)),
                            order: actionInfo.Order
                        ));
                        break;
                    case CardButtonType.BottomToolbarButton:
                        
                        bottomActions.Add(new CardToolbarAction(
                            name,
                            actionInfo.Caption,
                            icon,
                            new DelegateCommand(GetButtonAction(name, cancel)),
                            order: actionInfo.Order
                        ));
                        break;
                    case CardButtonType.BottomDialogButton:
                        bottomButtons.Add(new UIButton(
                            actionInfo.Caption,
                            GetButtonAction(name, cancel)
                        ));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Action<object> GetButtonAction(string name, bool cancel)
            {
                return !cancel
                    ? (Action<object>) (async _ => await this.CompleteDialogAsync(editor, coSettings, name, instance))
                    : async _ => await CancelCommand(editor);
            }
        }

        private static async Task CancelCommand(ICardEditorModel editor)
        {
            await editor.CloseAsync();
        }
        
        private async Task CompleteDialogAsync(
            ICardEditorModel dialogCardEditor,
            CardTaskCompletionOptionSettings coSettings,
            string buttonName,
            KrProcessInstance instance)
        {
            var dialogCard = dialogCardEditor.CardModel.Card;
            var actionResult = new CardTaskDialogActionResult
            {
                MainCardID = default,
                PressedButtonName = buttonName,
                StoreMode = coSettings.StoreMode,
            };
            actionResult.SetDialogCard(dialogCard.Clone());
                        
            var info = new Dictionary<string, object>();
            CardTaskDialogHelper.SetCardTaskDialogAcionResult(info, actionResult);
                        
            var result = await this.launcher.LaunchAsync(
                instance, 
                specificParameters: new KrProcessClientLauncher.SpecificParameters
                {
                    RequestInfo = info,
                });
            
            if (result.ValidationResult.IsSuccessful()
                && !result.CardResponse.GetKeepTaskDialog())
            {
                await dialogCardEditor.CloseAsync();
            }
            
            await TessaDialog.ShowNotEmptyAsync(result.ValidationResult);
        }
        
        private async Task CompleteDialogAsync(
            ICardEditorModel dialogCardEditor,
            ICardEditorModel parentEditor,
            CardTaskCompletionOptionSettings coSettings,
            string buttonName,
            KrProcessInstance instance)
        {
            var card = parentEditor.CardModel.Card;
            var dialogCard = dialogCardEditor.CardModel.Card;
            var actionResult = new CardTaskDialogActionResult
            {
                MainCardID = card.ID,
                PressedButtonName = buttonName,
                StoreMode = coSettings.StoreMode,
            };
            actionResult.SetDialogCard(dialogCard.Clone());
                        
            CardTaskDialogHelper.SetCardTaskDialogAcionResult(card.Info, actionResult);
                        
            var result = await this.launcher.LaunchAsync(
                instance, 
                specificParameters: new KrProcessClientLauncher.SpecificParameters
                {
                    CardEditor = parentEditor,
                });
                        
            if (result.StoreResponse.ValidationResult.IsSuccessful()
                && !result.CardResponse.GetKeepTaskDialog())
            {
                await dialogCardEditor.CloseAsync();
            }
        }
        
        #endregion
        
    }
}