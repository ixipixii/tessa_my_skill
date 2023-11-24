using System;
using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrPureProcess: KrSecondaryProcess, IKrPureProcess
    {
        /// <inheritdoc />
        public KrPureProcess(
            Guid id,
            string name,
            bool isGlobal,
            bool async,
            string executionAccessDeniedMessage,
            bool runOnce,
            IEnumerable<Guid> contextRolesIDs,
            string executionSqlCondition,
            string executionSourceCondition,
            bool allowClientSideLaunch,
            bool checkRecalcRestrictions)
            : base(id, name, isGlobal,
                async, executionAccessDeniedMessage, runOnce,
                contextRolesIDs,
                executionSqlCondition, executionSourceCondition)
        {
            this.AllowClientSideLaunch = allowClientSideLaunch;
            this.CheckRecalcRestrictions = checkRecalcRestrictions;
        }

        /// <inheritdoc />
        public bool AllowClientSideLaunch { get; }

        /// <inheritdoc />
        public bool CheckRecalcRestrictions { get; }
    }
}