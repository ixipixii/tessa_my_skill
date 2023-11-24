using System;
using System.Collections.Generic;
using System.Linq;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public abstract class KrSecondaryProcess: IKrSecondaryProcess
    {
        protected KrSecondaryProcess(
            Guid id,
            string name,
            bool isGlobal,
            bool async,
            string executionAccessDeniedMessage,
            bool runOnce,
            IEnumerable<Guid> contextRolesIDs,
            string executionSqlCondition,
            string executionSourceCondition)
        {
            this.ID = id;
            this.Name = name;
            this.IsGlobal = isGlobal;
            this.Async = async;
            this.ExecutionAccessDeniedMessage = executionAccessDeniedMessage;
            this.RunOnce = runOnce;
            this.ExecutionSqlCondition = executionSqlCondition;
            this.ExecutionSourceCondition = executionSourceCondition;
            this.ContextRolesIDs = contextRolesIDs.ToList().AsReadOnly();
        }

        /// <inheritdoc />
        public Guid ID { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsGlobal { get; }

        /// <inheritdoc />
        public bool Async { get; }

        /// <inheritdoc />
        public string ExecutionAccessDeniedMessage { get; }

        /// <inheritdoc />
        public bool RunOnce { get; }

        /// <inheritdoc />
        public IReadOnlyList<Guid> ContextRolesIDs { get; }

        /// <inheritdoc />
        public string ExecutionSqlCondition { get; }

        /// <inheritdoc />
        public string ExecutionSourceCondition { get; }
    }
}