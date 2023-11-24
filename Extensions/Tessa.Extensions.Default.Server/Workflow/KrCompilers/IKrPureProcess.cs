namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrPureProcess: IKrSecondaryProcess
    {
        /// <summary>
        /// Запуск процесса разрешен с клиента.
        /// </summary>
        bool AllowClientSideLaunch { get; }

        /// <summary>
        /// Проверять ограничения при запуске процесса.
        /// </summary>
        bool CheckRecalcRestrictions { get; }
    }
}