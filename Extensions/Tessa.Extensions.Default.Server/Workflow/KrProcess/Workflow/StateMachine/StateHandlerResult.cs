namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine
{
    public sealed class StateHandlerResult : IStateHandlerResult
    {
        /// <summary>
        /// Состояние не было обработано.
        /// </summary>
        public static readonly IStateHandlerResult EmptyResult =
            new StateHandlerResult(false, false, false);

        /// <summary>
        /// Результат обработки состояния с необходимостью сохранить изменения.
        /// </summary>
        public static readonly IStateHandlerResult WithoutContinuationProcessResult = 
            new StateHandlerResult(true, false, false);

        /// <summary>
        /// Результат обработки состояния без необходимости сохранения изменений.
        /// </summary>
        public static readonly IStateHandlerResult ContinueCurrentRunResult = 
            new StateHandlerResult(true, true, false);
        
        /// <summary>
        /// Результат обработки состояния без необходимости сохранения изменений.
        /// </summary>
        public static readonly IStateHandlerResult ContinueCurrentRunWithStartingNewGroupResult = 
            new StateHandlerResult(true, true, true);
        
        public StateHandlerResult(
            bool handled,
            bool continueCurrentRun,
            bool forceStartGroup)
        {
            this.Handled = handled;
            this.ContinueCurrentRun = continueCurrentRun;
            this.ForceStartGroup = forceStartGroup;
        }

        /// <inheritdoc />
        public bool Handled { get; }

        /// <inheritdoc />
        public bool ContinueCurrentRun { get; }

        /// <inheritdoc />
        public bool ForceStartGroup { get; }
    }
}