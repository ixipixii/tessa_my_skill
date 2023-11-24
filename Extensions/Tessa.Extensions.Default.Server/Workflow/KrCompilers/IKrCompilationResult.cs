using System;
using System.Collections.Generic;
using Tessa.Compilation;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Результат компиляции KrCompiler'a
    /// </summary>
    public interface IKrCompilationResult
    {
        /// <summary>
        /// Результат компиляции
        /// </summary>
        ICompilationResult Result { get; }

        /// <summary>
        /// Форматированные ошибки и предупреждения
        /// </summary>
        ValidationResult ValidationResult { get; }

        /// <summary>
        /// Создать объект.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="alias"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        IKrScript CreateInstance(
            string prefix,
            string alias,
            Guid typeID);
    }
}
