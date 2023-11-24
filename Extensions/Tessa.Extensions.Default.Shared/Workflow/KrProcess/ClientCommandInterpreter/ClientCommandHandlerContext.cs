using System.Collections.Generic;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter
{
    public sealed class ClientCommandHandlerContext : IClientCommandHandlerContext
    {
        /// <inheritdoc />
        public KrProcessClientCommand Command { get; set; }

        /// <inheritdoc />
        public object OuterContext { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> Info { get; set; }
    }
}