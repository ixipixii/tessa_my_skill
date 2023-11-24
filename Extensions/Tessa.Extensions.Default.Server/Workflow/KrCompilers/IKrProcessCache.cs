using System;
using System.Collections.Generic;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Кэш для данных из карточек шаблонов этапов
    /// </summary>
    public interface IKrProcessCache
    {
        /// <summary>
        /// Получение информации обо всех группах этапов.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IKrStageGroup> GetAllStageGroups();

        /// <summary>
        /// Получение отсортированного списка всех групп этапов.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKrStageGroup> GetOrderedStageGroups();

        /// <summary>
        /// Получение информацаии о группах этапов для процесса.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        IReadOnlyList<IKrStageGroup> GetStageGroupsForSecondaryProcess(
            Guid? process);
        
        /// <summary>
        /// Получение информации обо всех шаблонах этапов.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IKrStageTemplate> GetAllStageTemplates();
        
        /// <summary>
        /// Получение информацаии обо всех шаблонах для группы.
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        IReadOnlyList<IKrStageTemplate> GetStageTemplatesForGroup(
            Guid groupID);
        
        /// <summary>
        /// Получение информации обо всех рантайм скриптах.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IKrRuntimeStage> GetAllRuntimeStages();

        /// <summary>
        /// Получение информацаии обо всех этапах для указанного шаблона.
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        IReadOnlyList<IKrRuntimeStage> GetRuntimeStagesForTemplate(
            Guid templateID);

        /// <summary>
        /// Получение информации обо всех базовых методах.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKrCommonMethod> GetAllCommonMethods();

        /// <summary>
        /// Получить вторичный процесс по его идентификатору
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        IKrSecondaryProcess GetSecondaryProcess(
            Guid pid);

        /// <summary>
        /// Получение информации обо всех вторичных процессах.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IKrPureProcess> GetAllPureProcesses();

        /// <summary>
        /// Получение информации о действиях по типу действия
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        IReadOnlyCollection<IKrAction> GetActionsByType(
            string actionType);

        /// <summary>
        /// Получение информации обо всех действиях.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IKrAction> GetAllActions();

        /// <summary>
        /// Получение информации обо всех кнопках процесса.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IKrProcessButton> GetAllButtons();

        /// <summary>
        /// Сброс кэша.
        /// </summary>
        void Invalidate();
    }
}
