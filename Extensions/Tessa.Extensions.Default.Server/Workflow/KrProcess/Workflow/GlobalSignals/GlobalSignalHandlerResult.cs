namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals
{
    /// <summary>
    /// Представляет результат обработки глобавльного сигнала.
    /// </summary>
    public sealed class GlobalSignalHandlerResult: IGlobalSignalHandlerResult
    {
        public static readonly IGlobalSignalHandlerResult EmptyHandlerResult = 
            new GlobalSignalHandlerResult(false, true, false);
        
        public static readonly IGlobalSignalHandlerResult WithoutContinuationProcessResult = 
            new GlobalSignalHandlerResult(true, false, false);
        
        public static readonly IGlobalSignalHandlerResult ContinueCurrentRunResult = 
            new GlobalSignalHandlerResult(true, true, false);
        
        public static readonly IGlobalSignalHandlerResult ContinueCurrentRunWithStartingNewGroupResult = 
            new GlobalSignalHandlerResult(true, true, true);

        public GlobalSignalHandlerResult(
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