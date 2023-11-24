using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrSecondaryProcess: IExecutionSources
    {
        /// <summary>
        /// Идентификтор кнопки.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Название кнопки.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Кнопка является глобальной.
        /// </summary>
        bool IsGlobal { get; }

        /// <summary>
        /// Процесс, запускаемый по кнопке, допускает асинхронное выполнение.
        /// </summary>
        bool Async { get; }

        /// <summary>
        /// Сообщение о недоступности кнопки для нажатия.
        /// </summary>
        string ExecutionAccessDeniedMessage { get; }

        /// <summary>
        /// Запускать один раз в пределах KrScope
        /// </summary>
        bool RunOnce { get; }

        /// <summary>
        /// Список контекстных ролей, проверяемых при нажатии на кнопку.
        /// </summary>
        IReadOnlyList<Guid> ContextRolesIDs { get; }
    }
}