using System.Collections;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter
{
    public interface IClientCommandHandlerContext
    {
        /// <summary>
        /// Текущая клиентская команда
        /// </summary>
        KrProcessClientCommand Command { get; set; }
        
        /// <summary>
        /// Внешний контекст, в котором запущено выполнение. Может быть <c>null</c>
        /// </summary>
        object OuterContext { get; set; }
        
        /// <summary>
        /// Info
        /// </summary>
        IDictionary<string, object> Info { get; set; }
        
    }
}