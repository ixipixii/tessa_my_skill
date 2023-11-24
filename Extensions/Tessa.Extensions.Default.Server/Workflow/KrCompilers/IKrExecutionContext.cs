using System;
using System.Collections.Generic;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Контекст выполнения методов шаблонов этапов Before, After, Condition
    /// </summary>
    public interface IKrExecutionContext
    {
        /// <summary>
        /// Выполнить все возможные единицы выполнения.
        /// </summary>
        bool ExecuteAll { get; }

        /// <summary>
        /// Список идентификаторов единиц выполнения, которые необходимо выполнить.
        /// </summary>
        HashSet<Guid> ExecutionUnitIDs { get; }

        /// <summary>
        /// Стратегия загрузки основной карточки.
        /// </summary>
        IMainCardAccessStrategy MainCardAccessStrategy { get; }

        /// <summary>
        /// Тип карточки.
        /// </summary>
        Guid? CardID { get; }

        /// <summary>
        /// Тип карточки.
        /// </summary>
        Guid? CardTypeID { get; }

        /// <summary>
        /// Название типа карточки.
        /// </summary>
        string CardTypeName { get; }

        /// <summary>
        /// Отображаемое название типа карточки.
        /// </summary>
        string CardTypeCaption { get; }

        /// <summary>
        /// Тип докумета.
        /// </summary>
        Guid? DocTypeID { get; }

        /// <summary>
        /// Тип карточки/докумета.
        /// </summary>
        Guid? TypeID { get; }
        
        /// <summary>
        /// Включенные компоненты типового решения для текущей карточки.
        /// </summary>
        KrComponents? KrComponents { get; }

        /// <summary>
        /// Объектная модель процесса.
        /// </summary>
        WorkflowProcess WorkflowProcess { get; }

        /// <summary>
        /// Контекст расширения карточки, попадающий в контекст выполнения
        /// </summary>
        ICardExtensionContext CardContext { get; }
        
        /// <summary>
        /// Кэш скомпилированных сборок.
        /// </summary>
        IKrCompilationResult CompilationResult { get; }

        /// <summary>
        /// Пересчет ведется только для указанной кнопки.
        /// </summary>
        IKrSecondaryProcess SecondaryProcess { get; }

        /// <summary>
        /// Идентификатор сущности, объединяющей сущности <see cref="ExecutionUnitIDs"/>.
        /// Используется для передачи идентифкатора группы при расчете шаблонов по одной группе.
        /// </summary>
        Guid? GroupID { get; }

        /// <summary>
        /// Создать новый контекст на основе существующего с учетом новых элементов исполнения.
        /// </summary>
        /// <param name="executionUnits">
        /// Список новых идентификаторов или <c>null</c>, если нужно выполнить все возможное.
        /// </param>
        /// <returns></returns>
        IKrExecutionContext Copy(
            IEnumerable<Guid> executionUnits = null);

        /// <summary>
        /// Создать новый контекст на основе существующего с учетом новых элементов исполнения и идентификатора группы.
        /// </summary>
        /// <param name="groupID">
        /// Идентификатор сущности, объединяющей единицы исполнения.
        /// </param>
        /// <param name="executionUnits">
        /// Список новых идентификаторов или <c>null</c>, если нужно выполнить все возможное.
        /// </param>
        /// <returns></returns>
        IKrExecutionContext Copy(
            Guid? groupID,
            IEnumerable<Guid> executionUnits = null);

        /// <summary>
        /// Создать новый контекст на основу существующего с учетом кэша скомпилированных сборок и новых элементов исполнения.
        /// </summary>
        /// <param name="compilationCache"></param>
        /// <param name="executionUnits"></param>
        /// <returns></returns>
        IKrExecutionContext Copy(
            IKrCompilationResult compilationCache,
            IEnumerable<Guid> executionUnits = null);
    }
}
