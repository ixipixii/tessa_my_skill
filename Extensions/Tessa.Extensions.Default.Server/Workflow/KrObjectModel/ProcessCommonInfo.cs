using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public abstract class ProcessCommonInfo : StorageObject
    {
        #region constructors

        protected ProcessCommonInfo(
            Guid? currentStageRowID,
            IDictionary<string, object> info,
            Guid? secondaryProcessID)
            : base(new Dictionary<string, object>())
        {
            this.Init(nameof(this.CurrentStageRowID), currentStageRowID);
            this.Init(nameof(this.Info), info);
            this.Init(nameof(this.SecondaryProcessID), secondaryProcessID);
        }
        
        /// <inheritdoc />
        protected ProcessCommonInfo(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        protected ProcessCommonInfo(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

        public Guid? CurrentStageRowID
        {
            get => this.Get<Guid?>(nameof(this.CurrentStageRowID));
            set => this.Set(nameof(this.CurrentStageRowID), value);
        }

        public IDictionary<string, object> Info
        {
            get => this.TryGet<IDictionary<string, object>>(nameof(this.Info));
            set => this.Set(nameof(this.Info), value);
        }

        public Guid? SecondaryProcessID
        {
            get => this.TryGet<Guid?>(nameof(this.SecondaryProcessID));
            set => this.Set(nameof(this.SecondaryProcessID), value);
        }

        #endregion
    }

}