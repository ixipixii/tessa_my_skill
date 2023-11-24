using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    [Serializable]
    public sealed class KrProcessInstance : StorageObject
    {
        #region constructor

        public KrProcessInstance(
            Guid processID,
            Guid? cardID,
            string serializedWorkflowProcess,
            byte[] signature
            )
            : base(new Dictionary<string, object>())
        {
            this.Init(nameof(this.ProcessID), processID);
            this.Init(nameof(this.CardID), cardID);
            this.Init(nameof(this.ProcessInfo), null);
            this.Init(nameof(this.ParentStageRowID), null);
            this.Init(nameof(this.ParentProcessTypeName), null);
            this.Init(nameof(this.ParentProcessID), null);
            this.Init(nameof(this.ProcessHolderID), null);
            this.Init(nameof(this.NestedOrder), null);
            this.Init(nameof(this.SerializedProcess), serializedWorkflowProcess);
            this.Init(nameof(this.SerializedProcessSignature), signature);
        }
        
        public KrProcessInstance(
            Guid processID,
            Guid? cardID,
            IDictionary<string, object> processInfo,
            Guid? parentStageRowID,
            string parentProcessTypeName,
            Guid? parentProcessID,
            Guid? processHolderID,
            int? nestedOrder)
            : base(new Dictionary<string, object>())
        {
            this.Init(nameof(this.ProcessID), processID);
            this.Init(nameof(this.CardID), cardID);
            this.Init(nameof(this.ProcessInfo), processInfo);
            this.Init(nameof(this.ParentStageRowID), parentStageRowID);
            this.Init(nameof(this.ParentProcessTypeName), parentProcessTypeName);
            this.Init(nameof(this.ParentProcessID), parentProcessID);
            this.Init(nameof(this.ProcessHolderID), processHolderID);
            this.Init(nameof(this.NestedOrder), nestedOrder);
            this.Init(nameof(this.SerializedProcess), null);
            this.Init(nameof(this.SerializedProcessSignature), null);
        }

        /// <inheritdoc />
        public KrProcessInstance(
            Dictionary<string, object> storage)
            : base(storage)
        {

        }

        /// <inheritdoc />
        private KrProcessInstance(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

        public Guid ProcessID => this.Get<Guid>(nameof(this.ProcessID));

        public Guid? CardID => this.Get<Guid?>(nameof(this.CardID));
        
        public IDictionary<string, object> ProcessInfo =>
            this.GetDictionary(nameof(this.ProcessInfo), o => new Dictionary<string, object>());
        
        public Guid? ProcessHolderID => this.TryGet<Guid?>(nameof(this.ProcessHolderID));

        public string ParentProcessTypeName => this.TryGet<string>(nameof(this.ParentProcessTypeName));
        
        public Guid? ParentProcessID => this.TryGet<Guid?>(nameof(this.ParentProcessID));

        public Guid? ParentStageRowID => this.TryGet<Guid?>(nameof(this.ParentStageRowID));

        public int? NestedOrder => this.TryGet<int?>(nameof(this.NestedOrder));

        public string SerializedProcess => this.TryGet<string>(nameof(this.SerializedProcess));
        
        public byte[] SerializedProcessSignature => this.TryGet<byte[]>(nameof(this.SerializedProcessSignature));
        
        #endregion

    }
}