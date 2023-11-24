using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    /// <summary>
    /// Команда, формируемая на сервере при работе процесса Kr
    /// И возвращаемая на клиент для дальнейшей интерпретаци..
    /// </summary>
    public sealed class KrProcessClientCommand : StorageObject
    {
        public KrProcessClientCommand(
            string commandType,
            Dictionary<string, object> parameters = null) 
            : base(new Dictionary<string, object>())
        {
            this.Set(nameof(this.CommandType), commandType);
            this.Set(nameof(this.Parameters), parameters ?? new Dictionary<string, object>());
        }

        /// <inheritdoc />
        public KrProcessClientCommand(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        public KrProcessClientCommand(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Тип команды.
        /// </summary>
        public string CommandType => this.Get<string>(nameof(this.CommandType));

        /// <summary>
        /// Парамеетрыы команды.
        /// </summary>
        public Dictionary<string, object> Parameters =>
            this.Get<Dictionary<string, object>>(nameof(this.Parameters), () => new Dictionary<string, object>(StringComparer.Ordinal)); 

    }
}