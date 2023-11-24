using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public interface IKrProcessLaunchResult
    {
        /// <summary>
        /// Статус процесса после запуска процесса
        /// </summary>
        KrProcessLaunchStatus LaunchStatus { get; }
        
        /// <summary>
        /// Идентификатор запущеного асинхронного процесса.
        /// </summary>
        Guid? ProcessID { get; }

        /// <summary>
        /// Результат валидации запуска процесса.
        /// </summary>
        ValidationStorageResultBuilder ValidationResult { get; }
        
        /// <summary>
        /// Инфо процесса после его завершения.
        /// </summary>
        IDictionary<string, object> ProcessInfo { get; }

        /// <summary>
        /// Ответ на запрос, при котором был запущен процесс.
        /// Может быть <c>null</c>
        /// </summary>
        CardStoreResponse StoreResponse { get; }
        
        /// <summary>
        /// Ответ на запрос, при котором был запущен процесс.
        /// Может быть <c>null</c>
        /// </summary>
        CardResponse CardResponse { get; }
    }
}