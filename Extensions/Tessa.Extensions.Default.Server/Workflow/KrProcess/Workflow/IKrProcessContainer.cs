using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IKrProcessContainer
    {
        /// <summary>
        /// Регистрация обработчика типа этапа по дескриптору.
        /// </summary>
        /// <typeparam name="T">Тип обработчика</typeparam>
        /// <param name="descriptor">Дескриптор типа этапа</param>
        /// <returns>this</returns>
        IKrProcessContainer RegisterHandler<T>(
            StageTypeDescriptor descriptor) where T : IStageTypeHandler;

        /// <summary>
        /// Регистрация обработчика типа этапа по дескриптору.
        /// </summary>
        /// <param name="descriptor">Дескриптор типа этапа</param>
        /// <param name="handlerType">Тип обработчика</param>
        /// <returns>this</returns>
        IKrProcessContainer RegisterHandler(
            StageTypeDescriptor descriptor,
            Type handlerType);

        /// <summary>
        /// Зарегистрировать тип задания.
        /// </summary>
        /// <param name="taskTypeID">ID типа задания.</param>
        /// <returns>this</returns>
        IKrProcessContainer RegisterTaskType(
            Guid taskTypeID);

        /// <summary>
        /// Зарегистрировать тип задания.
        /// </summary>
        /// <param name="taskTypeID">ID типа задания.</param>
        /// <returns>this</returns>
        IKrProcessContainer RegisterTaskType(
            IEnumerable<Guid> taskTypeID);

        /// <summary>
        /// Сбросить типы заданий, загруженные из настроек типового решения.
        /// </summary>
        /// <returns></returns>
        IKrProcessContainer ResetExtraTaskTypes();
        
        /// <summary>
        /// Зарегистрировать тип обработчика глобального сигнала.
        /// </summary>
        /// <param name="signalType"></param>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        IKrProcessContainer RegisterGlobalSignal(
            string signalType,
            Type handlerType);

        /// <summary>
        /// Зарегистрировать тип обработчика глобального сигнала.
        /// </summary>
        /// <param name="signalType"></param>
        /// <returns></returns>
        IKrProcessContainer RegisterGlobalSignal <T> (
            string signalType) where T: IGlobalSignalHandler ;

        /// <summary>
        /// Добавить фильтр дескрипторов.
        /// </summary>
        /// <param name="filter">Фильтр дескрипторов</param>
        /// <returns>this</returns>
        IKrProcessContainer AddFilter<T>(
            IKrProcessFilter<T> filter);

        /// <summary>
        /// Получить коллекцию зарегистрированных дескрипторов обработчиков.
        /// </summary>
        /// <param name="withFilters">с применением фильтров</param>
        /// <returns>Коллекция зарегистрированных дескрипторов</returns>
        ICollection<StageTypeDescriptor> GetHandlerDescriptors(
            bool withFilters = true);

        /// <summary>
        /// Получить зарегистрированный дексриптор
        /// </summary>
        /// <param name="descriptorID">ID дескриптора</param>
        /// <param name="withFilters">С применением фильтров.</param>
        /// <returns>Дескриптор</returns>
        StageTypeDescriptor GetHandlerDescriptor(
            Guid descriptorID,
            bool withFilters = true);

        /// <summary>
        /// Получить обработчик типа этапа по его дескриптору.
        /// </summary>
        /// <param name="descriptorID">ID дескриптора</param>
        /// <param name="withFilters">С применением фильтров</param>
        /// <returns>Обработчик типа этапа.</returns>
        IStageTypeHandler ResolveHandler(
            Guid descriptorID,
            bool withFilters = true);

        /// <summary>
        /// Получитьт обработчик сигнала по типу сигнала.
        /// </summary>
        /// <param name="signal">Тип сигнала</param>
        /// <param name="withFilters">С применением фильтров</param>
        /// <returns></returns>
        List<IGlobalSignalHandler> ResolveSignal(
            string signal,
            bool withFilters = true);

        /// <summary>
        /// Зарегистрирован ли тип задания.
        /// </summary>
        /// <param name="taskTypeID"></param>
        /// <returns></returns>
        bool IsTaskTypeRegistered(
            Guid taskTypeID);
    }
}