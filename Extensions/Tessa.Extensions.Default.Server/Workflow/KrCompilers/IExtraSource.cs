namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IExtraSource
    {
        /// <summary>
        /// Отображаемое название.
        /// </summary>
        string DisplayName { get; }
        
        /// <summary>
        /// Название.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Тип возвращаемого значения.
        /// </summary>
        string ReturnType { get; }
        
        /// <summary>
        /// Тип параметра.
        /// </summary>
        string ParameterType { get; }
        
        /// <summary>
        /// Название параметра.
        /// </summary>
        string ParameterName { get; }
        
        /// <summary>
        /// Исходный код.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Преобразовать объект к модифицируемому типу.
        /// </summary>
        /// <returns></returns>
        IExtraSource ToMutable();

        /// <summary>
        /// Преобразовать объект к неизменяемому типу.
        /// </summary>
        /// <returns></returns>
        IExtraSource ToReadonly();
    }
}