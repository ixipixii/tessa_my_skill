namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public interface IKrProcessVisibilityScript
    {
        /// <summary>
        /// Запуск определения условия видимости.
        /// </summary>
        bool RunVisibility();

        /// <summary>
        /// Определения условия видимости.
        /// </summary>
        bool Visibility();
    }
}