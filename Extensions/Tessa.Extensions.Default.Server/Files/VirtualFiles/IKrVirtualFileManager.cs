using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Files.VirtualFiles
{
    /// <summary>
    /// Объект для получения информации о виртуальных файлах по карточке и проверки доступа к файлам
    /// </summary>
    public interface IKrVirtualFileManager
    {
        /// <summary>
        /// Метод для установки виртуальных файлов в карточку с учетом проверок доступа.
        /// </summary>
        /// <param name="card">Карточка, в которую добавляются виртуальные файлы</param>
        /// <param name="validationResult">Билдер результата валидации, куда пишутся ошибки в случае добавления виртуальных файлов</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        Task FillCardWithFilesAsync(Card card, IValidationResultBuilder validationResult, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для проверки доступа на виртуальный файл для картчоки
        /// </summary>
        /// <param name="cardID">ID карточки</param>
        /// <param name="fileID">ID файла</param>
        /// <returns>Возвращает результат валидации проверки прав доступа. Возвращает <see cref="ValidationResult.Empty"/>, если проверка доступа прошла успешно</returns>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        Task<ValidationResult> CheckAccessForFileAsync(Guid cardID, Guid fileID, CancellationToken cancellationToken = default);
    }
}
