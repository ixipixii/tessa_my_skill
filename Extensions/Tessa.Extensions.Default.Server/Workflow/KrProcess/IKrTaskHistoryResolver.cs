﻿using System;
using Tessa.Cards;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public interface IKrTaskHistoryResolver
    {
        /// <summary>
        /// Текущий используемый менеджер истории заданий.
        /// </summary>
        ICardTaskHistoryManager TaskHistoryManager { get; }

        /// <summary>
        /// Возвращает группу в истории заданий, вычисленную для заданных параметров. При необходимости группа будет создана.
        /// </summary>
        /// <param name="groupTypeID">Идентификатор типа группы, которую требуется найти или добавить.</param>
        /// <param name="parentGroupTypeID">
        /// <para>
        /// Идентификатор типа родительской группы, в которую добавляется искомая группа при её отсутствии (когда добавляется группа первой итерации).
        /// </para>
        /// <para>
        /// Если родительская группа также присутствует, то будет выбрана родительская группа заданного типа с наибольшей итерацией.
        /// </para>
        /// <para>
        /// Если родительская группа отсутствует, то она будет создана.
        /// </para>
        /// </param>
        /// <param name="newIteration">
        /// <para>
        /// Признак того, что метод всегда добавляет итерацию для группы типа <paramref name="groupTypeID" />.
        /// </para>
        /// <para>
        /// Если указано <c>true</c>, то метод создаёт новый экземпляр группы как при её существовании (тогда увеличивается номер итерации),
        /// так и при её отсутствии (тогда указывается итерация номер <c>1</c>).
        /// </para>
        /// <para>
        /// Если указано <c>false</c>, то метод возвращает экземпляр группы без его создания, если группа заданного типа была найдена
        /// (возвращается группа с наибольшей итерацией); если же группа не найдена, то также создаётся экземпляр группы с итерацией номер <c>1</c>.
        /// </para>
        /// </param>
        /// <param name="overrideValidationResult">
        /// <para>
        /// Результат валидации, содержащий информацию по проблемам, возникшим при загрузке истории заданий (если она ещё не была загружена)
        /// и при вычислении названия группы <c>Caption</c> (при замене плейсхолдеров).
        /// Вычисление названия группы выполняется при добавлении группы, а также при добавлении родительской группы.
        /// </para>
        /// <para>
        /// Если указано значение <c>null</c>, то используется стандартный объект <c>Manager.ValidationResult</c>,
        /// который влияет на успешность процесса сохранения и на сообщения, которые будут возвращены в результате сохранения.
        /// </para>
        /// </param>
        /// <returns>
        /// Созданная или найденная строка с информацией по группе в истории заданий, которая соответствует переданным параметрами,
        /// или <c>null</c>, если не удалось создать группу (например, ошибки в плейсхолдерах в карточке типа группы).
        /// </returns>
        CardTaskHistoryGroup ResolveTaskHistoryGroup(
            Guid groupTypeID,
            Guid? parentGroupTypeID = null,
            bool newIteration = false,
            IValidationResultBuilder overrideValidationResult = null);
    }
}