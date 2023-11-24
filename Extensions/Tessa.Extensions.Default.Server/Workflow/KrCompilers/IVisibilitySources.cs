namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IVisibilitySources
    {
        /// <summary>
        /// Текст SQL запроса с условием. Выполняется для определения видимости.
        /// </summary>
        string VisibilitySqlCondition { get; }

        /// <summary>
        /// C# код, определяющий видимость.
        /// </summary>
        string VisibilitySourceCondition { get; }
    }
}