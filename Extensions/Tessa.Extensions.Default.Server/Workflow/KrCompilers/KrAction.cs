using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrAction: KrSecondaryProcess, IKrAction
    {
        /// <inheritdoc />
        public KrAction(
            Guid id,
            string name,
            bool isGlobal,
            bool async,
            string executionAccessDeniedMessage,
            bool runOnce,
            IEnumerable<Guid> contextRolesIDs,
            string executionSqlCondition,
            string executionSourceCondition,
            string eventType)
            : base(
                id, name, isGlobal, 
                async, executionAccessDeniedMessage, runOnce,
                contextRolesIDs, 
                executionSqlCondition, executionSourceCondition)
        {
            this.EventType = eventType;
        }

        /// <inheritdoc />
        public string EventType { get; }
    }
}