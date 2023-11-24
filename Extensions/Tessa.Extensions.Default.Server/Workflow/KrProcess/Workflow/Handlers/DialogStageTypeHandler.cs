using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Properties.Resharper;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class DialogStageTypeHandler : StageTypeHandlerBase
    {
        #region Nested Types

        public sealed class ScriptContext
        {
            private readonly IMainCardAccessStrategy dialogCard;

            public ScriptContext(
                IMainCardAccessStrategy dialogCard,
                string buttonName)
            {
                this.dialogCard = dialogCard;
                this.ButtonName = buttonName;
            }

            /// <summary>
            /// Прерывает обработку диалога.
            /// </summary>
            [UsedImplicitly]
            public bool Cancel { get; set; }

            /// <summary>
            /// Завершить этап диалога.
            /// </summary>
            [UsedImplicitly]
            public bool CompleteDialog { get; set; }

            /// <summary>
            /// Карточка диалога.
            /// </summary>
            [UsedImplicitly]
            public Card DialogCard => this.dialogCard.GetCard();

            /// <summary>
            /// Название нажатой кнопки.
            /// </summary>
            [UsedImplicitly]
            public string ButtonName { get; }

            /// <summary>
            /// Получение контента файла для карточек диалогов, работающих в режиме Info и Settings.
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public byte[] GetFileContent(
                CardFile file) =>
                CardTaskDialogHelper.GetFileContentFromBase64Async(CardTaskDialogHelper.GetFileContentFromInfo(file)).GetAwaiter().GetResult();

            /// <summary>
            /// Установить контент файла для карточек диалогов, работающих в режиме Info и Settings
            /// </summary>
            /// <param name="file"></param>
            /// <param name="content"></param>
            public void SetFileContent(CardFile file, byte[] content)
            {
                var fileVersion = CardTaskDialogHelper.GetFileVersionFromInfo(file);
                fileVersion.Size = content.Length;

                var base64 = Convert.ToBase64String(content);
                CardTaskDialogHelper.SetFileContentToInfo(file, base64, fileVersion);
            }

            /// <summary>
            /// Получение файлового контейнера для карточки диалога в режиме Card.
            /// </summary>
            /// <returns></returns>
            public ICardFileContainer GetFileContainer() => this.dialogCard.GetFileContainer();
        }


        public sealed class SavingScriptContext
        {
            private readonly IMainCardAccessStrategy dialogCard;

            public SavingScriptContext(
                IMainCardAccessStrategy dialogCard,
                string buttonName)
            {
                this.dialogCard = dialogCard;
                this.ButtonName = buttonName;
            }

            /// <summary>
            /// Карточка диалога.
            /// </summary>
            [UsedImplicitly]
            public Card DialogCard => this.dialogCard.GetCard();

            /// <summary>
            /// Название нажатой кнопки.
            /// </summary>
            [UsedImplicitly]
            public string ButtonName { get; }
        }


        #endregion

        #region Fields

        public const string ChangedCardKey = CardHelper.SystemKeyPrefix + "ChangedCard";

        public const string DialogsProcessInfoKey = CardHelper.SystemKeyPrefix + "Dialogs";

        public static readonly string ScriptContextParameterType =
            $"global::{typeof(DialogStageTypeHandler).FullName}.{typeof(ScriptContext).Name}";

        public const string MethodName = "DialogActionScript";

        public const string MethodParameterName = "Dialog";

        public static readonly string SavingScriptContextParameterType =
            $"global::{typeof(DialogStageTypeHandler).FullName}.{typeof(SavingScriptContext).Name}";

        public const string SavingMethodName = "SavingDialogScript";

        public const string SavingMethodParameterName = "Dialog";

        protected readonly ICardRepository CardRepositoryDwt;

        protected readonly IDbScope DbScope;

        protected readonly IKrProcessCache ProcessCache;

        protected readonly ISignatureProvider SignatureProvider;

        protected readonly IStageTasksRevoker TasksRevoker;

        protected readonly IKrTypesCache typesCache;

        #endregion

        #region Constructor

        public DialogStageTypeHandler(
            IKrScope krScope,
            IKrCompilationCache compilationCache,
            IUnityContainer unityContainer,
            [Dependency(CardRepositoryNames.DefaultWithoutTransaction)] ICardRepository cardRepositoryDwt,
            IDbScope dbScope,
            IKrProcessCache processCache,
            ISignatureProvider signatureProvider,
            IStageTasksRevoker tasksRevoker,
            IKrTypesCache typesCache)
        {
            this.KrScope = krScope;
            this.CompilationCache = compilationCache;
            this.UnityContainer = unityContainer;
            this.CardRepositoryDwt = cardRepositoryDwt;
            this.DbScope = dbScope;
            this.ProcessCache = processCache;
            this.SignatureProvider = signatureProvider;
            this.TasksRevoker = tasksRevoker;
            this.typesCache = typesCache;
        }

        #endregion

        #region Properties

        protected IKrScope KrScope { get; }

        protected IKrCompilationCache CompilationCache { get; }

        protected IUnityContainer UnityContainer { get; }

        #endregion

        #region Base overrides

        /// <inheritdoc />
        public override void BeforeInitialization(
            IStageTypeHandlerContext context)
        {
            if (!this.ProcessCache.GetAllRuntimeStages().TryGetValue(context.Stage.ID, out var runtimeStage)
                || string.IsNullOrWhiteSpace(runtimeStage.RuntimeSourceBefore))
            {
                // Если нет скрипта, то загружать карточку нет смысла.
                return;
            }

            // Тут возможны варианты:
            // Диалог неперсистентный и квазиперсистентный: создаем новую, без вариантов.
            // Диалог персистентный: либо берем готовую карточку по алиасу, либо создаем новую.

            var stage = context.Stage;
            var settingsStorage = stage.SettingsStorage;
            var storeMode = (CardTaskDialogStoreMode) settingsStorage.TryGet<int>(KrDialogStageTypeSettingsVirtual.CardStoreModeID);
            var alias = settingsStorage.TryGet<string>(KrDialogStageTypeSettingsVirtual.DialogAlias);

            IMainCardAccessStrategy cardAccessStrategy;
            Guid persistentCardID;
            if (storeMode == CardTaskDialogStoreMode.Card
                && (persistentCardID = GetAliasedDialogID(context, alias)) != Guid.Empty
                && KrProcessHelper.CardExistsAsync(persistentCardID, this.DbScope).GetAwaiter().GetResult()) // TODO async
            {
                cardAccessStrategy = new KrScopeMainCardAccessStrategy(persistentCardID, this.KrScope, context.ValidationResult);
            }
            else
            {
                var typeID = settingsStorage.TryGet<Guid?>(KrDialogStageTypeSettingsVirtual.DialogTypeID);
                if (!typeID.HasValue)
                {
                    context.ValidationResult.AddError(this, "$KrStages_Dialog_DialogTypeIDNotSpecified");
                    return;
                }

                var info = new Dictionary<string, object>(StringComparer.Ordinal);
                typeID = CreateInfoForNewCardIfDocType(typeID.Value, info);

                var newResponse = this.CardRepositoryDwt.NewAsync(new CardNewRequest
                {
                    CardTypeID = typeID,
                    Info = info
                }).GetAwaiter().GetResult(); // TODO async

                if (!newResponse.ValidationResult.IsSuccessful())
                {
                    context.ValidationResult.Add(newResponse.ValidationResult);
                    return;
                }

                cardAccessStrategy = new ObviousMainCardAccessStrategy(newResponse.Card);
            }

            context.Stage.InfoStorage[Keys.NewCard] = cardAccessStrategy;
        }

        /// <inheritdoc />
        public override StageHandlerResult HandleStageStart(
            IStageTypeHandlerContext context)
        {
            switch (context.RunnerMode)
            {
                case KrProcessRunnerMode.Sync:
                    return this.StartSyncDialog(context);
                case KrProcessRunnerMode.Async:
                    return this.StartAsyncDialog(context);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override StageHandlerResult HandleResurrection(
            IStageTypeHandlerContext context)
        {
            CardTaskDialogActionResult actionInfo;
            switch (context.CardExtensionContext)
            {
                case ICardStoreExtensionContext storeContext:
                    var card = storeContext.Request.Card;
                    actionInfo = CardTaskDialogHelper.GetCardTaskDialogAcionResult(card.Info);
                    break;
                case ICardRequestExtensionContext requestContext:
                    actionInfo = CardTaskDialogHelper.GetCardTaskDialogAcionResult(requestContext.Request);
                    break;
                default:
                    return StageHandlerResult.CompleteResult;
            }

            var scriptContext = new ScriptContext(
                new ObviousMainCardAccessStrategy(actionInfo.DialogCard),
                actionInfo.PressedButtonName)
            {
                Cancel = false,
                CompleteDialog = true,
            };
            var inst = HandlerHelper.CreateScriptInstance(
                this.CompilationCache,
                context.Stage.ID,
                context.ValidationResult);
            HandlerHelper.InitScriptContext(this.UnityContainer, inst, context);
            inst.InvokeExtra(MethodName, scriptContext);

            if (scriptContext.Cancel)
            {
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .Error(DefaultValidationKeys.CancelDialog)
                    .End();
                return StageHandlerResult.CancelProcessResult;
            }

            return StageHandlerResult.CompleteResult;

        }

        /// <inheritdoc />
        public override StageHandlerResult HandleTaskCompletion(
            IStageTypeHandlerContext context)
        {
            var task = context.TaskInfo.Task;
            if (task.OptionID == DefaultCompletionOptions.Complete)
            {
                return StageHandlerResult.CompleteResult;
            }

            if (task.OptionID != DefaultCompletionOptions.ShowDialog)
            {
                throw new InvalidOperationException();
            }

            var actionInfo = CardTaskDialogHelper.GetCardTaskDialogAcionResult(task);
            var coInfo = CardTaskDialogHelper.GetCompletionOptionSettings(task, DefaultCompletionOptions.ShowDialog);

            if (!string.IsNullOrEmpty(coInfo.DialogAlias)
                && coInfo.StoreMode == CardTaskDialogStoreMode.Card
                && coInfo.PersistentDialogCardID != default)
            {
                AddAliasedDialog(context, coInfo.DialogAlias, coInfo.PersistentDialogCardID);
            }

            var card = this.GetCard(coInfo, actionInfo, context);
            Card updatedSettingsCard = null;
            if (actionInfo.StoreMode == CardTaskDialogStoreMode.Settings)
            {
                var savingScriptContext = new SavingScriptContext(
                    card,
                    actionInfo.PressedButtonName);
                var saving = HandlerHelper.CreateScriptInstance(
                    this.CompilationCache,
                    context.Stage.ID,
                    context.ValidationResult);
                HandlerHelper.InitScriptContext(this.UnityContainer, saving, context);
                saving.InvokeExtra(SavingMethodName, savingScriptContext);
                if (!context.ValidationResult.IsSuccessful())
                {
                    return StageHandlerResult.EmptyResult;
                }

                if (card.WasUsed)
                {
                    updatedSettingsCard = card.GetCard();
                }
            }

            var scriptContext = new ScriptContext(
                card,
                actionInfo.PressedButtonName)
            {
                Cancel = false,
                CompleteDialog = true,
            };
            var inst = HandlerHelper.CreateScriptInstance(
                this.CompilationCache,
                context.Stage.ID,
                context.ValidationResult);
            HandlerHelper.InitScriptContext(this.UnityContainer, inst, context);
            inst.InvokeExtra(MethodName, scriptContext);

            if (scriptContext.Cancel)
            {
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .Error(DefaultValidationKeys.CancelDialog)
                    .End();
                return StageHandlerResult.InProgressResult;
            }

            if (this.KrScope.Exists
                && context.MainCardID != null)
            {
                if (scriptContext.CompleteDialog)
                {
                    var taskCopy = new CardTask(StorageHelper.Clone(task.GetStorage()));
                    taskCopy.RemoveChanges();
                    taskCopy.Action = CardTaskAction.Complete;
                    taskCopy.State = CardRowState.Deleted;
                    taskCopy.OptionID = DefaultCompletionOptions.Complete;
                    if (updatedSettingsCard != null)
                    {
                        var co = CardTaskDialogHelper.GetCompletionOptionSettings(taskCopy, DefaultCompletionOptions.ShowDialog);
                        updatedSettingsCard.RemoveChanges();
                        co.DialogCard = updatedSettingsCard;
                    }

                    this.KrScope.GetMainCard(context.MainCardID.Value).Tasks.Add(taskCopy);
                }
                else if (updatedSettingsCard != null)
                {
                    var taskCopy = new CardTask(StorageHelper.Clone(task.GetStorage()));
                    taskCopy.RemoveChanges();
                    taskCopy.Action = CardTaskAction.None;
                    taskCopy.OptionID = null;
                    taskCopy.State = CardRowState.Modified;
                    taskCopy.Flags |= CardTaskFlags.HistoryItemCreated;
                    var co = CardTaskDialogHelper.GetCompletionOptionSettings(taskCopy, DefaultCompletionOptions.ShowDialog);
                    updatedSettingsCard.RemoveChanges();
                    co.DialogCard = updatedSettingsCard;
                    this.KrScope.GetMainCard(context.MainCardID.Value).Tasks.Add(taskCopy);
                }
            }

            return StageHandlerResult.InProgressResult;
        }

        public override StageHandlerResult HandleSignal(
            IStageTypeHandlerContext context)
        {
            var signal = context.SignalInfo;
            var actionInfo = CardTaskDialogHelper.GetCardTaskDialogAcionResult(signal.Signal.Parameters);

            var cardAccessStrategy = new ObviousMainCardAccessStrategy(actionInfo.DialogCard);

            var scriptContext = new SavingScriptContext(
                cardAccessStrategy,
                actionInfo.PressedButtonName);
            var inst = HandlerHelper.CreateScriptInstance(
                this.CompilationCache,
                context.Stage.ID,
                context.ValidationResult);
            HandlerHelper.InitScriptContext(this.UnityContainer, inst, context);
            inst.InvokeExtra(SavingMethodName, scriptContext);

            if (cardAccessStrategy.WasUsed)
            {
                context.CardExtensionContext.Info[ChangedCardKey] = cardAccessStrategy.GetCard().GetStorage();
            }

            return StageHandlerResult.InProgressResult;
        }


        public override bool HandleStageInterrupt(IStageTypeHandlerContext context) =>
            this.TasksRevoker.RevokeAllStageTasks(new StageTaskRevokerContext(context));

        #endregion

        #region protected

        protected StageHandlerResult StartSyncDialog(
            IStageTypeHandlerContext context)
        {
            var storeMode = (CardTaskDialogStoreMode) context
                .Stage
                .SettingsStorage
                .TryGet<int>(KrDialogStageTypeSettingsVirtual.CardStoreModeID);
            if (storeMode != CardTaskDialogStoreMode.Info)
            {
                context.ValidationResult.AddError(this, "$KrStages_Dialog_StartingSyncDialogWithNotInfoStoreMode");
                return StageHandlerResult.CompleteResult;
            }

            var cardID = context.MainCardID ?? Guid.Empty;
            var processID = context.SecondaryProcess.ID;

            var serializedProcess = KrProcessHelper.SerializeWorkflowProcess(context.WorkflowProcess);
            var signature = KrProcessHelper.SignWorkflowProcess(serializedProcess, cardID, processID, this.SignatureProvider);

            var processInstance = new KrProcessInstance(
                context.SecondaryProcess.ID,
                context.MainCardID,
                serializedProcess,
                signature);

            var coSettings = this.CreateCompletionOptionSettings(context);
            if (coSettings is null)
            {
                return StageHandlerResult.CompleteResult;
            }

            this.PrepareNewCardInSettinsFromStageInfo(context.Stage, coSettings);

            this.KrScope.TryAddClientCommand(
                new KrProcessClientCommand(
                    DefaultCommandTypes.ShowAdvancedDialog,
                    new Dictionary<string, object>
                    {
                        [Keys.ProcessInstance] = processInstance.GetStorage(),
                        [Keys.CompletionOptionSettings] = coSettings.GetStorage(),
                    }));

            return StageHandlerResult.CancelProcessResult;
        }

        protected StageHandlerResult StartAsyncDialog(
            IStageTypeHandlerContext context)
        {
            var performer = context.Stage.Performer;
            var api = context.WorkflowAPI;
            var coSettings = this.CreateCompletionOptionSettings(context);
            var (kindID, kindCaption) = HandlerHelper.GetTaskKind(context);

            if (coSettings is null)
            {
                return StageHandlerResult.CompleteResult;
            }

            this.PrepareNewCardInSettinsFromStageInfo(context.Stage, coSettings);

            api.SendTask(
                DefaultTaskTypes.KrShowDialogTypeID,
                null,
                performer.PerformerID,
                performer.PerformerName,
                modifyTaskAction: t =>
                {
                    t.GroupRowID = HandlerHelper.GetTaskHistoryGroup(context, this.KrScope);
                    t.Planned = context.Stage.Planned;
                    t.PlannedQuants = context.Stage.PlannedQuants;
                    t.Digest = context.Stage.SettingsStorage.TryGet<string>(KrDialogStageTypeSettingsVirtual.TaskDigest);
                    HandlerHelper.SetTaskKind(t, kindID, kindCaption, context);
                    CardTaskDialogHelper.SetCompletionOptionSettings(t, coSettings);
                });

            return StageHandlerResult.InProgressResult;
        }

        protected IMainCardAccessStrategy GetCard(
            CardTaskCompletionOptionSettings coInfo,
            CardTaskDialogActionResult actionInfo,
            IStageTypeHandlerContext context)
        {
            switch (coInfo.StoreMode)
            {
                case CardTaskDialogStoreMode.Info:
                    return  new ObviousMainCardAccessStrategy(actionInfo.DialogCard);
                case CardTaskDialogStoreMode.Settings:
                    return  new ObviousMainCardAccessStrategy(coInfo.DialogCard);
                case CardTaskDialogStoreMode.Card:
                    return new KrScopeMainCardAccessStrategy(coInfo.PersistentDialogCardID, this.KrScope, context.ValidationResult);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected CardTaskCompletionOptionSettings CreateCompletionOptionSettings(IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            var settingsStorage = stage.SettingsStorage;

            var buttonsSettings = settingsStorage.TryGet<IList<object>>(KrDialogButtonSettingsVirtual.Synthetic);
            var buttonsList = new ListStorage<CardTaskDialogButtonInfo>(new List<object>(), CardTaskDialogButtonInfo.ItemFactory);
            if (buttonsSettings != null)
            {
                foreach (var buttonStorage in buttonsSettings.Cast<Dictionary<string, object>>())
                {
                    var button = new CardTaskDialogButtonInfo
                    {
                        Name = buttonStorage.TryGet<string>(KrDialogButtonSettingsVirtual.Name),
                        CardButtonType = (CardButtonType) buttonStorage.TryGet<int>(KrDialogButtonSettingsVirtual.TypeID),
                        Caption = buttonStorage.TryGet<string>(KrDialogButtonSettingsVirtual.Caption),
                        Icon = buttonStorage.TryGet<string>(KrDialogButtonSettingsVirtual.Icon),
                        Cancel = buttonStorage.TryGet<bool>(KrDialogButtonSettingsVirtual.Cancel),
                        Order = buttonStorage.TryGet<int>(Order),
                    };
                    buttonsList.Add(button);
                }
            }

            var storeModeInt = settingsStorage.TryGet<int?>(KrDialogStageTypeSettingsVirtual.CardStoreModeID);
            if (storeModeInt is null)
            {
                context.ValidationResult.AddWarning("$KrStages_Dialog_CardStoreModeNotSpecified");
                return null;
            }

            var openModeInt = settingsStorage.TryGet<int?>(KrDialogStageTypeSettingsVirtual.OpenModeID);
            if (openModeInt is null)
            {
                context.ValidationResult.AddWarning("$KrStages_Dialog_CardOpenModeNotSpecified");
                return null;
            }

            var dialogTypeID = settingsStorage.TryGet<Guid?>(KrDialogStageTypeSettingsVirtual.DialogTypeID);
            if (dialogTypeID is null)
            {
                context.ValidationResult.AddWarning("$KrStages_Dialog_DialogTypeIDNotSpecified");
                return null;
            }

            var info = new Dictionary<string, object>(StringComparer.Ordinal);
            dialogTypeID = CreateInfoForNewCardIfDocType(dialogTypeID.Value, info);

            var coSettings = new CardTaskCompletionOptionSettings
            {
                CompletionOptionID = DefaultCompletionOptions.ShowDialog,
                DialogTypeID = dialogTypeID.Value,
                TaskButtonCaption = settingsStorage.TryGet<string>(KrDialogStageTypeSettingsVirtual.ButtonName),
                DialogName = settingsStorage.TryGet<string>(KrDialogStageTypeSettingsVirtual.DialogName),
                DialogAlias = settingsStorage.TryGet<string>(KrDialogStageTypeSettingsVirtual.DialogAlias),
                StoreMode = (CardTaskDialogStoreMode) storeModeInt,
                OpenMode = (CardTaskDialogOpenMode) openModeInt,
                Buttons = buttonsList,
            };

            coSettings.GetStorage()["Info"] = info;

            if (!string.IsNullOrEmpty(coSettings.DialogAlias)
                && coSettings.StoreMode == CardTaskDialogStoreMode.Card)
            {
                var persistentCardID = GetAliasedDialogID(context, coSettings.DialogAlias);
                coSettings.PersistentDialogCardID = persistentCardID;
            }

            return coSettings;
        }

        protected static void AddAliasedDialog(
            IStageTypeHandlerContext context,
            string alias,
            Guid dialogCardID)
        {
            var processInfo = context.WorkflowProcess.InfoStorage;

            if (!processInfo.TryGetValue(DialogsProcessInfoKey, out var dialogsStorageObj)
                || !(dialogsStorageObj is IDictionary<string, object> dialogsStorage))
            {
                dialogsStorage = new Dictionary<string, object>();
                processInfo[DialogsProcessInfoKey] = dialogsStorage;
            }

            dialogsStorage[alias] = dialogCardID.ToString("N");
        }

        protected static Guid GetAliasedDialogID(
            IStageTypeHandlerContext context,
            string alias)
        {
            var processInfo = context.WorkflowProcess.InfoStorage;

            if (processInfo.TryGetValue(DialogsProcessInfoKey, out var dialogsStorageObj)
                && dialogsStorageObj is IDictionary<string, object> dialogsStorage
                && dialogsStorage.TryGetValue(alias, out var cardIDObj)
                && cardIDObj is string cardIDStr
                && Guid.TryParse(cardIDStr, out var cardID))
            {
                return cardID;
            }

            return default;
        }

        #endregion

        #region Private

        private Guid CreateInfoForNewCardIfDocType(Guid typeID, IDictionary<string, object> info)
        {
            var docType = this.typesCache.GetDocTypesAsync().GetAwaiter().GetResult().FirstOrDefault(x => x.ID == typeID); // TODO async
            if (docType != default)
            {
                info[Keys.DocTypeID] = typeID;
                info[Keys.DocTypeTitle] = docType.Caption;
                typeID = docType.CardTypeID;
            }

            return typeID;
        }

        /// <summary>
        /// Инициализирует параметры диалога информацией о подготовленной карточке хранящейся в <see cref="Stage.InfoStorage"/> этапа по ключу <see cref="Keys.NewCard"/>.
        /// </summary>
        /// <param name="stage">Этап из которого загружается информация по подготовленной карточке.</param>
        /// <param name="coSettings">Параметры диалога.</param>
        private void PrepareNewCardInSettinsFromStageInfo(Stage stage, CardTaskCompletionOptionSettings coSettings)
        {
            // TODO: взять карточку из инфо и если она загружалась и менялась, то что-нибудь сделать.
            if (stage.InfoStorage.TryGetValue(Keys.NewCard, out var newCardObj))
            {
                stage.InfoStorage.Remove(Keys.NewCard);
                if (newCardObj is IMainCardAccessStrategy cardAccessStrategy
                    && cardAccessStrategy.WasUsed)
                {
                    var card = cardAccessStrategy.GetCard();
                    var cardBytes = card.ToSerializable().Serialize();
                    var cardSignature = this.SignatureProvider.Sign(cardBytes);
                    coSettings.PreparedNewCard = cardBytes;
                    coSettings.PreparedNewCardSignature = cardSignature;
                }
            }
        }

        #endregion

    }
}