using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrRuntimeStage: IRuntimeSources, IExtraSources
    {
        /// <summary>
        /// ID шаблона этапов.
        /// </summary>
        Guid TemplateID { get; }

        /// <summary>
        /// Имя шаблона этапов, в котором находится этап.
        /// </summary>
        string TemplateName { get; }

        /// <summary>
        /// Идентификатор группы, в которой находится шаблон, в котором находится этап.
        /// </summary>
        Guid GroupID { get; }
        
        /// <summary>
        /// Имя группы этапов, в которой находится шаблон, в котором находится этап.
        /// </summary>
        string GroupName { get; }
        
        /// <summary>
        /// Порядок группы, в которой находится шаблон, в котором находится этап.
        /// </summary>
        int GroupOrder { get; }
        
        /// <summary>
        /// ID этапа.
        /// </summary>
        Guid StageID { get; }

        /// <summary>
        /// Назавние этапа.
        /// </summary>
        string StageName { get; }

        /// <summary>
        /// Порядок этапа в шаблоне.
        /// </summary>
        int? Order { get; }
        
        /// <summary>
        /// Ограничение по времени на этап.
        /// </summary>
        double? TimeLimit { get; }
        
        /// <summary>
        /// Плановая дата завершения этапа.
        /// </summary>
        DateTime? Planned { get; }
        
        /// <summary>
        /// Этап является скрытым
        /// </summary>
        bool Hidden { get; }
        
        /// <summary>
        /// Иденификатор типа этапа.
        /// </summary>
        Guid StageTypeID { get; }
        
        /// <summary>
        /// Название типа этапа.
        /// </summary>
        string StageTypeCaption { get; }
        
        /// <summary>
        /// SQL-запрос для вычисления роли.
        /// </summary>
        string SqlRoles { get; }

        /// <summary>
        /// Получить настройки этапа. Возвращает доступную для изменения копию настроек этапа.
        /// </summary>
        IDictionary<string, object> GetSettings();

        /// <summary>
        /// Возвращает значение флага разрешающего пропускать этап.
        /// </summary>
        bool Skip { get; }
        
        /// <summary>
        /// Возвращает значение признака, показывающего, разрешено ли пропускать этап.
        /// </summary>
        bool CanBeSkipped { get; }
    }
}