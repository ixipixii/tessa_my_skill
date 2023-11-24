using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter
{
    public interface IClientCommandInterpreter
    {
        /// <summary>
        /// Добавить обработчик команды с указанным типом.
        /// </summary>
        /// <param name="commandType">Тип команды</param>
        /// <param name="handlerType">Тип обработчика</param>
        /// <returns>this</returns>
        IClientCommandInterpreter RegisterHandler(
            string commandType,
            Type handlerType);

        /// <summary>
        /// Добавить обработчик команды с указанным типом.
        /// </summary>
        /// <typeparam name="T">Тип обработчика</typeparam>
        /// <param name="commandType">Тип команды</param>
        /// <returns>this</returns>
        IClientCommandInterpreter RegisterHandler<T>(
            string commandType)
            where T : IClientCommandHandler;

        /// <summary>
        /// Интерпретировать набор команд.
        /// </summary>
        /// <param name="commands">Набор команд</param>
        /// <param name="outerContext">Некоторый внешний контекст, доступный для обработчкиов команд</param>
        void Interpret(
            IEnumerable<KrProcessClientCommand> commands,
            object outerContext);
    }
}