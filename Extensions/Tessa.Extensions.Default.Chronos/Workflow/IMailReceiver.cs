using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Chronos.Workflow
{
    public interface IMailReceiver
    {
        Func<bool> StopRequestedFunc { get; set; }

        /// <summary>
        /// Обработка сообщений
        /// </summary>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        Task ReceiveMessagesAsync(CancellationToken cancellationToken = default);
    }
}