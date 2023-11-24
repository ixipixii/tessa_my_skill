namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrAction: IKrSecondaryProcess
    {
        /// <summary>
        /// Тип события, по которому запускается действие.
        /// </summary>
        string EventType { get; }
    }
}