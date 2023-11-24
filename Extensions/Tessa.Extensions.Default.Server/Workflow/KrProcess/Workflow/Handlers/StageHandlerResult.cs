using System;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    /// <summary>
    /// Представляет сообщение для ProcessRunner содержащее результаты обработки этапа.
    /// </summary>
    public struct StageHandlerResult : IEquatable<StageHandlerResult>
    {
        #region static

        /// <summary>
        /// Сообщение для ProcessRunner о том, что обработчик не обработал этап.
        /// </summary>
        public static readonly StageHandlerResult EmptyResult = 
            new StageHandlerResult(StageHandlerAction.None, null, null);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что этап в активном состоянии.
        /// </summary>
        public static readonly StageHandlerResult InProgressResult =
            new StageHandlerResult(StageHandlerAction.InProgress, null, null);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что этап завершен.
        /// </summary>
        public static readonly StageHandlerResult CompleteResult =
            new StageHandlerResult(StageHandlerAction.Complete, null, null);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что этап следует пропустить.
        /// </summary>
        public static readonly StageHandlerResult SkipResult =
            new StageHandlerResult(StageHandlerAction.Skip, null, null);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что  следует совершить переход на другой этап в текущей группе.
        /// </summary>
        /// <param name="stageRowID">ID этапа, на который нужно совершить переход</param>
        /// <param name="keepStageStates">Признак того, нужно ли сохранять состояния этапов при переходе</param>
        public static StageHandlerResult Transition(Guid stageRowID, bool keepStageStates = false) =>
            new StageHandlerResult(StageHandlerAction.Transition, stageRowID, keepStageStates);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что следует совершить переход в начало другой группы этапов.
        /// </summary>
        /// <param name="stageGroupID">ID группы этапов.</param>
        /// <param name="keepStageStates">Признак того, нужно ли сохранять состояния этапов при переходе</param>
        public static StageHandlerResult GroupTransition(Guid stageGroupID, bool keepStageStates = false) =>
            new StageHandlerResult(StageHandlerAction.GroupTransition, stageGroupID, keepStageStates);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что следует совершить переход в начало текущей группы.
        /// </summary>
        /// <param name="keepStageStates">Признак того, нужно ли сохранять состояния этапов при переходе</param>
        public static StageHandlerResult CurrentGroupTransition(bool keepStageStates = false) =>
            new StageHandlerResult(StageHandlerAction.CurrentGroupTransition, null, keepStageStates);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что следует совершить переход на следующую группу с учетом пересчета набора групп.
        /// </summary>
        /// <param name="keepStageStates">Признак того, нужно ли сохранять состояния этапов при переходе</param>
        public static StageHandlerResult NextGroupTransition(bool keepStageStates = false) =>
            new StageHandlerResult(StageHandlerAction.NextGroupTransition, null, keepStageStates);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что следует совершить переход на следующую группу с учетом пересчета набора групп.
        /// </summary>
        /// <param name="keepStageStates">Признак того, нужно ли сохранять состояния этапов при переходе</param>
        public static StageHandlerResult PreviousGroupTransition(bool keepStageStates = false) =>
            new StageHandlerResult(StageHandlerAction.PreviousGroupTransition, null, keepStageStates);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что необходимо пропустить весь процесс.
        /// </summary>
        public static readonly StageHandlerResult SkipProcessResult =
            new StageHandlerResult(StageHandlerAction.SkipProcess, null, null);

        /// <summary>
        /// Сообщение для ProcessRunner о том, что необходимо отменить весь процесс.
        /// </summary>
        public static readonly StageHandlerResult CancelProcessResult =
            new StageHandlerResult(StageHandlerAction.CancelProcess, null, null);

        #endregion

        #region constructor

        public StageHandlerResult(
            StageHandlerAction action,
            Guid? transitionID,
            bool? keepStageStates)
        {
            this.Action = action;
            this.TransitionID = transitionID;
            this.KeepStageStates = keepStageStates;
        }

        #endregion

        #region public

        public StageHandlerAction Action { get; }

        public Guid? TransitionID { get; }

        public bool? KeepStageStates { get; }

        /// <inheritdoc />
        public bool Equals(
            StageHandlerResult other)
        {
            return this.Action == other.Action
                && this.TransitionID.Equals(other.TransitionID)
                && this.KeepStageStates.Equals(other.KeepStageStates);
        }

        /// <inheritdoc />
        public override bool Equals(
            object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is StageHandlerResult result 
                && this.Equals(result);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) this.Action * 397) ^ this.TransitionID.GetHashCode();
            }
        }

        public static bool operator ==(
            StageHandlerResult left,
            StageHandlerResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            StageHandlerResult left,
            StageHandlerResult right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}