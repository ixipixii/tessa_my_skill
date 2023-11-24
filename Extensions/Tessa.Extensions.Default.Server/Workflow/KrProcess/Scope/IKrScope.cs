using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope
{
    public interface IKrScope
    {
        /// <summary>
        /// Признак того, что в текущий момент Scope существует.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Количество уровней в текущем scope.
        /// </summary>
        int Depth { get; }
        
        /// <summary>
        /// Результат валидации операций, производимых KrScope.
        /// Извне писать в этот ValidationResult не рекомендуется.
        /// </summary>
        IValidationResultBuilder ValidationResult { get; }

        /// <summary>
        /// Хранилище произвольных данных с областью видимости на текущий и вложеннйе запросы.
        /// </summary>
        Dictionary<string, object> Info { get; }

        /// <summary>
        /// Текущий уровень
        /// </summary>
        KrScopeLevel CurrentLevel { get; }

        /// <summary>
        /// Создать новый уровень вложенности KrScope
        /// </summary>
        /// <param name="levelValidationResult"></param>
        /// <param name="withReaderLocks">
        ///    При загрузке инстанса карточки (для инкремента версии, если карточка не загружена ранее)
        ///    использовать блокировку на чтение.
        /// </param>
        /// <returns></returns>
        KrScopeLevel EnterNewLevel(
            IValidationResultBuilder levelValidationResult,
            bool withReaderLocks = true);

        /// <summary>
        /// Получить полную карточку для текущего запроса.
        /// </summary>
        /// <param name="mainCardID">Идентификатор основной карточки.</param>
        /// <param name="validationResult">Объект с сообщениями после загрузки карточки или <c>null</c>, если объект не требуется.</param>
        /// <param name="withoutTransaction">Признак того, что карточка будет загружена без транзакции и без взятия блокировки на чтение карточки.</param>
        /// <returns>Загруженная карточка.</returns>
        Card GetMainCard(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null,
            bool withoutTransaction = false);

        /// <summary>
        /// Получить контейнер файлов для основной карточки.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        ICardFileContainer GetMainCardFileContainer(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null);
        
        /// <summary>
        /// Явно увеличить версию основной карточки.
        /// </summary>
        /// <param name="mainCardID"></param>
        void ForceIncrementMainCardVersion(
            Guid mainCardID);
        
        /// <summary>
        /// Удостоверится в том, что в основной карточке, загруженной в KrScope
        /// загружена история заданий. По умолчанию история заданий не загружается.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="validationResult"></param>
        void EnsureMainCardHasTaskHistory(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null);

        /// <summary>
        /// Получить сателлит для текущего запроса.
        /// При наличии изменений сателлит будет сохранен в BeforeCommitTransaction
        /// 
        /// Если контекста KrScopeContext не существует, то сателлит будет загружен явно,
        /// дальнейшее отслеживание производится не будет.
        /// </summary>
        /// <param name="mainCardID">
        ///     ID основной карточки
        /// </param>
        /// <param name="validationResult">
        ///     Опционально результат загрузки можно записать в явно передаваемый ValidationResult
        ///     вместо контекста текущего запроса.
        /// 
        ///     При вызове метода вне KrScopeContext настоятельно рекомендуется указывать параметр,
        ///     иначе информация о загрузке будет потеряна.
        /// </param>
        /// <returns></returns>
        Card GetKrSatellite(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null);

        /// <summary>
        /// Явное сохранение сателлита. В общем случае является избыточным и не рекомендуется.
        /// </summary>
        /// <param name="mainCardID">
        /// ID основной карточки
        /// </param>
        /// <param name="validationResult">
        /// Опционально результат сохранения можно записать в явно передаваемый ValidationResult
        /// вместо контекста текущего запроса.
        /// 
        /// При вызове метода вне KrScopeContext настоятельно рекомендуется указывать параметр,
        /// иначе информация о загрузке будет потеряна.
        /// </param>
        void StoreSatelliteExplicitly(
            Guid mainCardID,
            ValidationResultBuilder validationResult = null);

        /// <summary>
        /// Получить текущую группу истории заданий для указанной карточки,
        /// чей контекстуальный сателлит находится в текущем KrScope.
        /// </summary>
        /// <param name="mainCardID">
        /// Идентификатор основной карточки
        /// </param>
        /// <param name="validationResult">
        /// Опционально результат загрузки можно записать в явно передаваемый ValidationResult
        /// вместо контекста текущего запроса.
        /// </param>
        /// <returns>
        /// Идентификатор текущей группы истории заданий
        /// </returns>
        Guid? GetCurrentHistoryGroup(
            Guid mainCardID,
            IValidationResultBuilder validationResult = null);

        /// <summary>
        /// Установить новую группу истории заданий для указанной карточки,
        /// чей контекстуальный сателлит находится в текущем KrScope.
        /// </summary>
        /// <param name="mainCardID">
        /// Идентификатор основной карточки
        /// </param>
        /// <param name="newGroupHistoryID">
        /// Новая группа истории заданий. Если <c>null</c>, то записи будут заносится в пустую группу.
        /// </param>
        /// <param name="validationResult">
        /// Опционально результат загрузки можно записать в явно передаваемый ValidationResult
        /// вместо контекста текущего запроса.
        /// </param>
        void SetCurrentHistoryGroup(
            Guid mainCardID,
            Guid? newGroupHistoryID,
            IValidationResultBuilder validationResult = null);

        /// <summary>
        /// Создать и сохранить дополнительный сателлит для работы доп. процесса.
        /// </summary>
        /// <param name="mainCardID">ID основной карточки.</param>
        /// <param name="processID">ID доп. процесса.</param>
        /// <returns>Сателлит доп. процесса.</returns>
        /// <exception cref="InvalidOperationException">Вызов вне KrScopeContext не имеет смысла.</exception>
        Card CreateSecondaryKrSatellite(
            Guid mainCardID,
            Guid processID);

        /// <summary>
        /// Получить существующий дополнительный сателлит для работы с доп. процессом.
        /// </summary>
        /// <param name="processID">ID доп. процесса.</param>
        /// <returns>Сателлит доп. процесса.</returns>
        /// <exception cref="InvalidOperationException">Вызов вне KrScopeContext не имеет смысла.</exception>
        Card GetSecondaryKrSatellite(
            Guid processID);

        /// <summary>
        /// Заблокировать карточку для сохранения.
        /// Если карточка заблокирована, то при выходе с уровня сохранение произведено не будет
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns>
        /// Ключ для разблокировки карточки.
        /// <c>null</c>, если карточка уже была заблокирована ранее.
        /// </returns>
        Guid? LockCard(
            Guid cardID);

        /// <summary>
        /// Признак того, что карточка заблокирована.
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        bool IsCardLocked(
            Guid cardID);

        /// <summary>
        /// Снять блокировку с карточки на сохранение.
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="key">Ключ для снятия блокировки, полуженный от LockCard</param>
        /// <returns>
        /// <c>true</c>, если карточка успешно разблокирована.
        /// <c>false</c>, если карточка не заблокирована или ключ не подошел.
        /// </returns>
        bool ReleaseCard(
            Guid cardID,
            Guid? key);

        /// <summary>
        /// Добавить холдер процесса в текущий KrScope.
        /// </summary>
        /// <param name="processHolder"></param>
        void AddProcessHolder(
            ProcessHolder processHolder);

        /// <summary>
        /// Получить холдер процесса из текущего KrScope или null, если отсутствует.
        /// </summary>
        /// <param name="processHolderID"></param>
        /// <returns></returns>
        ProcessHolder GetProcessHolder(
            Guid processHolderID);

        /// <summary>
        /// Удалить холдер процесса из текущего KrScope.
        /// </summary>
        /// <param name="processHolderID"></param>
        void RemoveProcessHolder(
            Guid processHolderID);
    }
}