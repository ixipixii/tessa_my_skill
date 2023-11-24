using System;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public class KrStageGroup : IKrStageGroup
    {
        public KrStageGroup(
            Guid id,
            string name,
            int order,
            bool isGroupReadonly,
            Guid? secondaryProcessID,
            string sqlCondition,
            string runtimeSqlCondition,
            string sourceCondition,
            string sourceBefore,
            string sourceAfter,
            string runtimeSourceCondition,
            string runtimeSourceBefore,
            string runtimeSourceAfter)
        {
            this.ID = id;
            this.Name = name;
            this.Order = order;
            this.IsGroupReadonly = isGroupReadonly;
            this.SecondaryProcessID = secondaryProcessID;
            this.SqlCondition = sqlCondition;
            this.RuntimeSqlCondition = runtimeSqlCondition;
            this.SourceCondition = sourceCondition;
            this.SourceBefore = sourceBefore;
            this.SourceAfter = sourceAfter;
            this.RuntimeSourceCondition = runtimeSourceCondition;
            this.RuntimeSourceBefore = runtimeSourceBefore;
            this.RuntimeSourceAfter = runtimeSourceAfter;
        }

        /// <inheritdoc />
        public Guid ID { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public int Order { get; }

        /// <inheritdoc />
        public bool IsGroupReadonly { get; }

        /// <inheritdoc />
        public Guid? SecondaryProcessID { get; }

        /// <inheritdoc />
        public string SqlCondition { get; }

        /// <inheritdoc />
        public string RuntimeSqlCondition { get; }

        /// <inheritdoc />
        public string SourceCondition { get; }

        /// <inheritdoc />
        public string SourceBefore { get; }

        /// <inheritdoc />
        public string SourceAfter { get; }

        /// <inheritdoc />
        public string RuntimeSourceCondition { get; }

        /// <inheritdoc />
        public string RuntimeSourceBefore { get; }

        /// <inheritdoc />
        public string RuntimeSourceAfter { get; }
    }
}