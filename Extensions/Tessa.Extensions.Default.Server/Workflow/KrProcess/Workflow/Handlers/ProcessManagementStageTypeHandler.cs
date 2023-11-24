using System;
using System.Collections.Generic;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    /// <summary>
    /// Представляет обработчик этапа "Управление процессом".
    /// </summary>
    public class ProcessManagementStageTypeHandler : StageTypeHandlerBase
    {
        #region Nested Types

        /// <summary>
        /// Режим управления процессом.
        /// </summary>
        public enum ProcessManagementMode
        {
            /// <summary>
            /// Переход на этап.
            /// </summary>
            StageMode = 0,

            /// <summary>
            /// Переход на группу.
            /// </summary>
            GroupMode = 1,

            /// <summary>
            /// Переход на следующую группу.
            /// </summary>
            NextGroupMode = 2,

            /// <summary>
            /// Переход на предыдущую группу.
            /// </summary>
            PrevGroupMode = 3,

            /// <summary>
            /// Переход на начало текущей группы.
            /// </summary>
            CurrentGroupMode = 4,

            /// <summary>
            /// Отправить процесс.
            /// </summary>
            SendSignalMode = 5,

            /// <summary>
            /// Отменить процесс.
            /// </summary>
            CancelProcessMode = 6,

            /// <summary>
            /// Пропустить процесс.
            /// </summary>
            SkipProcessMode = 7,
        }

        #endregion

        #region Constructors

        public ProcessManagementStageTypeHandler(IKrScope krScope)
        {
            this.KrScope = krScope;
        }

        #endregion

        #region Protected Properties

        protected IKrScope KrScope { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Выполняет отправку сигнала.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult SendSignal(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess,
            string customSignal = null)
        {
            var signal = customSignal
                ?? context.Stage.SettingsStorage.TryGet<string>(KrConstants.KrProcessManagementStageSettingsVirtual.Signal);
            if (!managePrimaryProcess || !context.MainCardID.HasValue || string.IsNullOrWhiteSpace(signal))
            {
                return StageHandlerResult.SkipResult;
            }
            this.KrScope
                .GetMainCard(context.MainCardID.Value)
                .GetWorkflowQueue()
                .AddSignal(KrConstants.KrProcessName, signal);
            return StageHandlerResult.CompleteResult;
        }

        /// <summary>
        /// Выполняет переход на начало текущей группы.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult CurGroupTransition(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            if (!managePrimaryProcess || !context.MainCardID.HasValue)
            {
                return StageHandlerResult.CurrentGroupTransition();
            }
            this.KrScope
                .GetMainCard(context.MainCardID.Value)
                .GetWorkflowQueue()
                .AddSignal(
                    KrConstants.KrProcessName,
                    KrConstants.KrTransitionGlobalSignal,
                    parameters: new Dictionary<string, object>
                    {
                        [KrConstants.KrTransitionCurrentGroup] = BooleanBoxes.True,
                    });
            return StageHandlerResult.CompleteResult;
        }

        /// <summary>
        /// Выполняет переход на предыдущую группу.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult PrevGroupTransition(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            if (!managePrimaryProcess || !context.MainCardID.HasValue)
            {
                return StageHandlerResult.PreviousGroupTransition();
            }
            this.KrScope
                .GetMainCard(context.MainCardID.Value)
                .GetWorkflowQueue()
                .AddSignal(
                    KrConstants.KrProcessName,
                    KrConstants.KrTransitionGlobalSignal,
                    parameters: new Dictionary<string, object>
                    {
                        [KrConstants.KrTransitionPrevGroup] = BooleanBoxes.True,
                    });
            return StageHandlerResult.CompleteResult;
        }

        /// <summary>
        /// Выполняет переход на следующую группу.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult NextGroupTransition(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            if (!managePrimaryProcess || !context.MainCardID.HasValue)
            {
                return StageHandlerResult.NextGroupTransition();
            }
            this.KrScope
                .GetMainCard(context.MainCardID.Value)
                .GetWorkflowQueue()
                .AddSignal(
                    KrConstants.KrProcessName,
                    KrConstants.KrTransitionGlobalSignal,
                    parameters: new Dictionary<string, object>
                    {
                        [KrConstants.KrTransitionNextGroup] = BooleanBoxes.True,
                    });
            return StageHandlerResult.CompleteResult;
        }

        /// <summary>
        /// Выполняет переход на этап.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult StageTransition(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            var transitToStage = context.Stage.SettingsStorage
                .TryGet<Guid?>(KrConstants.KrProcessManagementStageSettingsVirtual.StageRowID);

            if (!transitToStage.HasValue)
            {
                return StageHandlerResult.SkipResult;
            }

            if (!managePrimaryProcess || !context.MainCardID.HasValue)
            {
                return StageHandlerResult.Transition(transitToStage.Value);
            }
            this.KrScope
                .GetMainCard(context.MainCardID.Value)
                .GetWorkflowQueue()
                .AddSignal(
                    KrConstants.KrProcessName,
                    KrConstants.KrTransitionGlobalSignal,
                    parameters: new Dictionary<string, object>
                    {
                        [KrConstants.StageRowID] = transitToStage.Value,
                    });
            return StageHandlerResult.CompleteResult;
        }

        /// <summary>
        /// Выполняет переход на группу.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult GroupTransition(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            var transitToGroup = context.Stage.SettingsStorage
                .TryGet<Guid?>(KrConstants.KrProcessManagementStageSettingsVirtual.StageGroupID);

            if (!transitToGroup.HasValue)
            {
                return StageHandlerResult.SkipResult;
            }

            if (!managePrimaryProcess || !context.MainCardID.HasValue)
            {
                return StageHandlerResult.GroupTransition(transitToGroup.Value);
            }

            this.KrScope
                .GetMainCard(context.MainCardID.Value)
                .GetWorkflowQueue()
                .AddSignal(
                    KrConstants.KrProcessName,
                    KrConstants.KrTransitionGlobalSignal,
                    parameters: new Dictionary<string, object>
                    {
                        [KrConstants.StageGroupID] = transitToGroup.Value,
                    });
            return StageHandlerResult.CompleteResult;
        }

        /// <summary>
        /// Выполняет отмену процесса.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult CancelProcess(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            if (!managePrimaryProcess)
            {
                return StageHandlerResult.CancelProcessResult;
            }

            return this.SendSignal(context, true, KrConstants.KrCancelProcessGlobalSignal);
        }

        /// <summary>
        /// Выполняет пропуск процесса.
        /// </summary>
        /// <param name="context">Контекст обработчика этапа.</param>
        /// <param name="managePrimaryProcess">Признак управления основным процессом.</param>
        /// <returns>Результат выполнения.</returns>
        protected virtual StageHandlerResult SkipProcess(
            IStageTypeHandlerContext context,
            bool managePrimaryProcess)
        {
            if (!managePrimaryProcess)
            {
                return StageHandlerResult.SkipProcessResult;
            }

            return this.SendSignal(context, true, KrConstants.KrSkipProcessGlobalSignal);
        }

        #endregion

        #region Base Overrides

        /// <inheritdoc />
        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var managePrimaryProcessFlag = context.Stage.SettingsStorage
                .TryGet<bool?>(KrConstants.KrProcessManagementStageSettingsVirtual.ManagePrimaryProcess) ?? false;
            var managePrimaryProcess = managePrimaryProcessFlag
                && context.ProcessInfo?.ProcessTypeName != KrConstants.KrProcessName;

            var modeInt = context.Stage.SettingsStorage
                .TryGet<int?>(KrConstants.KrProcessManagementStageSettingsVirtual.ModeID);

            if (!modeInt.HasValue
                || !(0 <= modeInt && modeInt <= (int)ProcessManagementMode.SkipProcessMode))
            {
                context.ValidationResult.AddError(this, "$KrStages_ProcessManagement_ModeNotSpecified");
                return StageHandlerResult.SkipResult;
            }
            
            // Этап синхронный и в любом случае будет завершен.
            // В дальнейшем переходы могут либо пропустить следующие этапы, либо отменять предыдущие
            // В случае пропуска следующих, текущий должен быть Completed, а не Skipped.
            // Это обрабатывается в KrProcessHelper.SetSkipStateToSubsequentStages
            context.Stage.State = KrStageState.Completed;

            var mode = (ProcessManagementMode)modeInt;
            switch (mode)
            {
                case ProcessManagementMode.StageMode:
                    return this.StageTransition(context, managePrimaryProcess);
                case ProcessManagementMode.GroupMode:
                    return this.GroupTransition(context, managePrimaryProcess);
                case ProcessManagementMode.NextGroupMode:
                    return this.NextGroupTransition(context, managePrimaryProcess);
                case ProcessManagementMode.PrevGroupMode:
                    return this.PrevGroupTransition(context, managePrimaryProcess);
                case ProcessManagementMode.CurrentGroupMode:
                    return this.CurGroupTransition(context, managePrimaryProcess);
                case ProcessManagementMode.SendSignalMode:
                    return this.SendSignal(context, managePrimaryProcess);
                case ProcessManagementMode.CancelProcessMode:
                    return this.CancelProcess(context, managePrimaryProcess);
                case ProcessManagementMode.SkipProcessMode:
                    return this.SkipProcess(context, managePrimaryProcess);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}