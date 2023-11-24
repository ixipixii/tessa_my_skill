using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    [Serializable]
    public sealed class NestedProcessCommonInfo: ProcessCommonInfo
    {
        #region constructors

        public NestedProcessCommonInfo(
            Guid? currentStageRowID,
            IDictionary<string, object> info,
            Guid? secondaryProcessID,
            Guid nestedProcessID,
            Guid parentStageRowID,
            int nestedOrder)
            : base(currentStageRowID, info, secondaryProcessID)
        {
            this.Init(nameof(this.NestedProcessID), nestedProcessID);
            this.Init(nameof(this.ParentStageRowID), parentStageRowID);
            this.Init(nameof(this.NestedOrder), nestedOrder);
        }
        
        /// <inheritdoc />
        public NestedProcessCommonInfo(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        private NestedProcessCommonInfo(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

        public Guid NestedProcessID
        {
            get => this.Get<Guid>(nameof(this.NestedProcessID));
            set => this.Set(nameof(this.NestedProcessID), value);
        }

        public Guid ParentStageRowID
        {
            get => this.Get<Guid>(nameof(this.ParentStageRowID));
            set => this.Set(nameof(this.ParentStageRowID), value);
        }

        public int NestedOrder
        {
            get => this.Get<int>(nameof(this.NestedOrder));
            set => this.Set(nameof(this.NestedOrder), value);
        }

        #endregion
    }
}