using System;
using System.Collections.Generic;
using Tessa.Compilation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders
{
    public interface IKrSourceBuilder<in T>
    {
        /// <summary>
        /// Установить id для генерируемого класса.
        /// Итоговое имя класса будет Type_Alias_ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IKrSourceBuilder<T> SetClassID(
            Guid id);
        
        /// <summary>
        /// Установить alias для генерируемого класса
        /// Итоговое имя класса будет Type_Alias_ID
        /// </summary>
        /// <param name="classAlias"></param>
        /// <returns></returns>
        IKrSourceBuilder<T> SetClassAlias(
            string classAlias);
        
        /// <summary>
        /// Установить расположение элемента от текущего к корневому.
        /// </summary>
        /// <param name="trace"></param>
        /// <returns></returns>
        IKrSourceBuilder<T> SetLocation(
            params string[] trace);

        /// <summary>
        /// Установить источник исходных кодов.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IKrSourceBuilder<T> SetSources(
            T source);

        /// <summary>
        /// Установить дополнительные исходные коды.
        /// </summary>
        /// <param name="extraSources"></param>
        /// <returns></returns>
        IKrSourceBuilder<T> SetExtraSources(
            IExtraSources extraSources);
        
        /// <summary>
        /// Установить связь между генерируемыми исходниками и названиями основных членов в исходнике.
        /// </summary>
        /// <param name="anchorsMap"></param>
        /// <returns></returns>
        IKrSourceBuilder<T> FillAnchorsMap(
            Dictionary<Guid, string> anchorsMap);

        /// <summary>
        /// Сборка методов в partial-классы
        /// </summary>
        /// <returns></returns>
        IList<ICompilationSource> BuildSources();
    }
}