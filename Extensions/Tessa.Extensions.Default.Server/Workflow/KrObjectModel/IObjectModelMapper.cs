using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public interface IObjectModelMapper
    {
        /// <summary>
        /// Загрузить из сателлита-холдера информацию по основному процессу.
        /// </summary>
        /// <param name="processHolderSatellite"></param>
        /// <param name="withInfo"></param>
        /// <returns></returns>
        MainProcessCommonInfo GetMainProcessCommonInfo(
            Card processHolderSatellite, 
            bool withInfo = true);

        /// <summary>
        /// Установить в сателлит-холдер информацию по основному процессу.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="processHolderSatellite"></param>
        /// <param name="processCommonInfo"></param>
        /// <returns></returns>
        void SetMainProcessCommonInfo(
            Guid mainCardID,
            Card processHolderSatellite,
            MainProcessCommonInfo processCommonInfo);
        
        /// <summary>
        /// Загрузить из сателлита-холдера основную информацию по вложенным процессам.
        /// </summary>
        /// <param name="processHolderSatellite"></param>
        /// <returns></returns>
        List<NestedProcessCommonInfo> GetNestedProcessCommonInfos(
            Card processHolderSatellite);

        /// <summary>
        /// Установить в сателлит-холдер основную информацию по вложенным процессам.
        /// </summary>
        /// <param name="processHolderSatellite"></param>
        /// <param name="nestedProcessCommonInfos"></param>
        void SetNestedProcessCommonInfos(
            Card processHolderSatellite,
            IReadOnlyCollection<NestedProcessCommonInfo> nestedProcessCommonInfos);

        /// <summary>
        /// Заполнение информации в объектной модели из pci.
        /// </summary>
        /// <param name="workflowProcess"></param>
        /// <param name="commonInfo"></param>
        /// <param name="primaryProcessCommonInfo"></param>
        void FillWorkflowProcessFromPci(
            WorkflowProcess workflowProcess,
            ProcessCommonInfo commonInfo,
            MainProcessCommonInfo primaryProcessCommonInfo);
        
        /// <summary>
        /// Преобразовать секционную модель процесса маршрутов в объектную модель.
        /// Данный метод удобно использовать для преобразования карточек шаблона этапов.
        /// </summary>
        /// <param name="primaryPci"></param>
        /// <param name="stageTemplate"></param>
        /// <param name="runtimeStages"></param>
        /// <param name="initialStage"></param>
        /// <param name="saveInitialStages"></param>
        /// <returns></returns>
        WorkflowProcess CardRowsToObjectModel(
            IKrStageTemplate stageTemplate,
            IReadOnlyCollection<IKrRuntimeStage> runtimeStages,
            MainProcessCommonInfo primaryPci,
            bool initialStage = true,
            bool saveInitialStages = false);

        /// <summary>
        /// Перенести информацию о процессе из объектной модели в pci.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="pci"></param>
        /// <param name="mainPci"></param>
        /// <param name="primaryPci"></param>
        void ObjectModelToPci(
            WorkflowProcess process,
            ProcessCommonInfo pci,
            MainProcessCommonInfo mainPci,
            MainProcessCommonInfo primaryPci);
        
        /// <summary>
        /// Преобразовать секционную модель процесса маршрутов в объектную модель.
        /// Данный метод удобно использовать для преобразования карточек документов.
        /// </summary>
        /// <param name="processHolder"></param>
        /// <param name="pci"></param>
        /// <param name="mainPci"></param>
        /// <param name="templates"></param>
        /// <param name="runtimeStages"></param>
        /// <param name="initialStage"></param>
        /// <param name="nestedProcessID"></param>
        /// <returns></returns>
        WorkflowProcess CardRowsToObjectModel(
            Card processHolder,
            ProcessCommonInfo pci,
            MainProcessCommonInfo mainPci,
            IReadOnlyDictionary<Guid, IKrStageTemplate> templates,
            IReadOnlyDictionary<Guid, IReadOnlyCollection<IKrRuntimeStage>> runtimeStages,
            bool initialStage = true,
            Guid? nestedProcessID = null);

        /// <summary>
        /// Преобразовать объектную модель процесса маршрутов в секционную модель с отслеживанием изменений.
        /// </summary>
        /// <param name="process">
        /// Переносимый процесс.
        /// </param>
        /// <param name="baseCard">
        /// Карточка, в которую необходимо перенести процесс.
        /// </param>
        /// <param name="pci">
        /// Основная информация о текущем процессе
        /// </param>
        /// <returns></returns>
        List<RouteDiff> ObjectModelToCardRows(
            WorkflowProcess process,
            Card baseCard,
            ProcessCommonInfo pci);
    }
}