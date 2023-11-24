
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrCompiler
    {
        /// <summary>
        /// Список стандартных пространств имен, подставляемых в каждую единицу компиляции.
        /// Доступно для изменения, но не потокобезопасно.
        /// </summary>
        IList<string> DefaultUsings { get; }
        
        /// <summary>
        /// Список стандартных зависимостей, используемых при компиляции.
        /// Доступно для изменения, но не потокобезопасно.
        /// </summary>
        IList<string> DefaultReferences { get; }
        
        /// <summary>
        /// Выполнить комплияцию на основе контекста
        /// В контексте должны быть указаны шаблоны этапов, базовые методы, 
        /// пространства имен и референсы
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IKrCompilationResult Compile(IKrCompilationContext context);
    }
}
