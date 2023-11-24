using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    [Serializable]
    public sealed class StartingSecondaryProcessInfo : StorageObject
    {
        #region constructor

        public StartingSecondaryProcessInfo(
            Guid? secondaryProcess,
            IDictionary<string, object> processInfo,
            Guid? parentStageRowID,
            string parentProcessTypeName,
            Guid? parentProcessID,
            Guid? processHolderID,
            int? nestedOrder)
            : base(new Dictionary<string, object>())
        {
            this.Init(nameof(this.SecondaryProcessID), secondaryProcess);
            this.Init(nameof(this.ProcessInfo), processInfo.ToDictionaryStorage());
            this.Init(nameof(this.ParentStageRowID), parentStageRowID);
            this.Init(nameof(this.ParentProcessTypeName), parentProcessTypeName);
            this.Init(nameof(this.ParentProcessID), parentProcessID);
            this.Init(nameof(this.ProcessHolderID), processHolderID);
            this.Init(nameof(this.NestedOrder), nestedOrder);
        }

        /// <inheritdoc />
        public StartingSecondaryProcessInfo(
            Dictionary<string, object> storage)
            : base(storage)
        {

        }

        /// <inheritdoc />
        private StartingSecondaryProcessInfo(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

        public Guid? SecondaryProcessID => this.Get<Guid?>(nameof(this.SecondaryProcessID));

        public IDictionary<string, object> ProcessInfo => this.Get<IDictionary<string, object>>(nameof(this.ProcessInfo));
        
        public Guid? ProcessHolderID => this.TryGet<Guid?>(nameof(this.ProcessHolderID));

        public string ParentProcessTypeName => this.TryGet<string>(nameof(this.ParentProcessTypeName));
        
        public Guid? ParentProcessID => this.TryGet<Guid?>(nameof(this.ParentProcessID));

        public Guid? ParentStageRowID => this.TryGet<Guid?>(nameof(this.ParentStageRowID));

        public int? NestedOrder => this.TryGet<int?>(nameof(this.NestedOrder));

        #endregion
    }
}