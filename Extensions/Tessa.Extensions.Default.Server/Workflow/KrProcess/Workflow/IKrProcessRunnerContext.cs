using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IKrProcessRunnerContext
    {
        /// <summary>
        /// Мост до WorkflowAPI
        /// </summary>
        IWorkflowAPIBridge WorkflowAPI { get; }

        /// <summary>
        /// Объект для работы с группами истории заданий.
        /// Может принимать <c>null</c>, если отсутствует возможность управления группами истории заданий.
        /// </summary>
        IKrTaskHistoryResolver TaskHistoryResolver { get; }

        /// <summary>
        /// Стратегия загрузки основной карточки.
        /// </summary>
        IMainCardAccessStrategy MainCardAccessStrategy { get; }

        /// <summary>
        /// ID карточки.
        /// </summary>
        Guid? CardID { get; }

        /// <summary>
        /// ID типа карточки.
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
        /// ID типа документа.
        /// </summary>
        Guid? DocTypeID { get; }

        /// <summary>
        /// Включенные компоненты типового решения для текущей карточки.
        /// </summary>
        KrComponents? KrComponents { get; }

        /// <summary>
        /// Контекст выполнения процесса.
        /// </summary>
        WorkflowProcess WorkflowProcess { get; }

        /// <summary>
        /// Холдер текущего процесса.
        /// </summary>
        ProcessHolder ProcessHolder { get; }
        
        /// <summary>
        /// Текущий контекстуальный сателлит.
        /// </summary>
        Card ContextualSatellite { get; }

        /// <summary>
        /// Текущий сателлит процесса.
        /// </summary>
        Card ProcessHolderSatellite { get; }

        /// <summary>
        /// Причина запуска процесса. 
        /// Складывается на основе информации в контексте.
        /// </summary>
        KrProcessRunnerInitiationCause InitiationCause { get; }

        /// <summary>
        /// Информация по запущенному процессу.
        /// Если InitiationCause == InMemory, то null.
        /// </summary>
        IWorkflowProcessInfo ProcessInfo { get; }

        /// <summary>
        /// Информация по заданию в процессе.
        /// Если InitiationCause != CompleteTask, то null. 
        /// </summary>
        IWorkflowTaskInfo TaskInfo { get; }

        /// <summary>
        /// Информация по заданию в процессе.
        /// Если InitiationCause != Signal, то null. 
        /// </summary>
        IWorkflowSignalInfo SignalInfo { get; }

        /// <summary>
        /// Результат валидации.
        /// </summary>
        IValidationResultBuilder ValidationResult { get; }

        /// <summary>
        /// Контекст расширения на карточке.
        /// </summary>
        ICardExtensionContext CardContext { get; }

        /// <summary>
        /// Конфигурация вторичного процесса.
        /// </summary>
        IKrSecondaryProcess SecondaryProcess { get; }

        /// <summary>
        /// Тип родительского процесса, если есть.
        /// </summary>
        string ParentProcessTypeName { get; }
        
        /// <summary>
        /// Идентификатор родительского процесса, если есть.
        /// </summary>
        Guid? ParentProcessID { get; }
        
        /// <summary>
        /// Игнорировать скрипты групп и частичный пересчет.
        /// </summary>
        bool IgnoreGroupScripts { get; }

        /// <summary>
        /// Кэш единиц исполнения в рамках одного выполнения Runner-а.
        /// </summary>
        Dictionary<Guid, IKrExecutionUnit> ExecutionUnitCache { get; }

        /// <summary>
        /// Список этапов, которые в процессе текущего выполнения были пропущены по условию скрипта.
        /// </summary>
        List<Guid> SkippedStagesByCondition { get; }

        /// <summary>
        /// Список групп, которые в процессе текущего выполнения были пропущены по условию скрипта.
        /// </summary>
        List<Guid> SkippedGroupsByCondition { get; }

        /// <summary>
        /// Фабрика для получения стандартных стратегий подготовки группы для пересчета.
        /// </summary>
        Func<IPreparingGroupRecalcStrategy> DefaultPreparingGroupStrategyFunc { get; }

        /// <summary>
        /// Стратегия для формирования данных, необходимых для пересчета.
        /// </summary>
        IPreparingGroupRecalcStrategy PreparingGroupStrategy { get; set; }
    }
}