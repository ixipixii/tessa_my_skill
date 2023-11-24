using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Cards;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    [Serializable]
    public sealed class KrProcessLaunchResult: StorageObject, IKrProcessLaunchResult
    {
        public KrProcessLaunchResult(
            KrProcessLaunchStatus launchStatus,
            Guid? processID,
            ValidationResult validationResult,
            IDictionary<string, object> processInfo,
            CardStoreResponse storeResponse,
            CardResponse cardResponse)
            : this(new Dictionary<string, object>())
        {
            this.Init(nameof(this.LaunchStatus), (int)launchStatus);
            this.Init(nameof(this.ProcessID), processID);
            this.SetStorageValue(nameof(this.ValidationResult), new ValidationStorageResultBuilder { validationResult });
            this.Set(nameof(this.ProcessInfo), processInfo);
            this.SetStorageValue(nameof(this.StoreResponse), storeResponse);
            this.SetStorageValue(nameof(this.CardResponse), cardResponse);
        }

        /// <inheritdoc />
        public KrProcessLaunchResult(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        private KrProcessLaunchResult(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public KrProcessLaunchStatus LaunchStatus => (KrProcessLaunchStatus) this.Get<int>(nameof(this.LaunchStatus));

        /// <inheritdoc />
        public Guid? ProcessID => this.Get<Guid?>(nameof(this.ProcessID));

        /// <inheritdoc />
        public ValidationStorageResultBuilder ValidationResult =>
            this.GetDictionary(nameof(this.ValidationResult), x => new ValidationStorageResultBuilder(x));

        /// <inheritdoc />
        public IDictionary<string, object> ProcessInfo => 
            this.Get<IDictionary<string, object>>(nameof(this.ProcessInfo));

        /// <inheritdoc />
        public CardStoreResponse StoreResponse => 
            this.GetDictionary(nameof(this.StoreResponse), dict => new CardStoreResponse(dict));
        
        /// <inheritdoc />
        public CardResponse CardResponse => 
            this.GetDictionary(nameof(this.CardResponse), dict => new CardResponse(dict));
    }
}