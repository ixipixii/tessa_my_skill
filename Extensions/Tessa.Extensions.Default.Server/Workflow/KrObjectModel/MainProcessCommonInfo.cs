using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    [Serializable]
    public sealed class MainProcessCommonInfo: ProcessCommonInfo
    {
        #region constructors

        public MainProcessCommonInfo(
            Guid? currentStageRowID,
            IDictionary<string, object> info,
            Guid? secondaryProcessID,
            Guid? authorID,
            string authorName,
            string authorComment,
            int state)
            : base(currentStageRowID, info, secondaryProcessID)
        {
            this.Init(nameof(this.AuthorID), authorID);
            this.Init(nameof(this.AuthorName), authorName);
            this.Init(nameof(this.AuthorTimestamp), 0L);
            this.Init(nameof(this.AuthorComment), authorComment);
            this.Init(nameof(this.AuthorCommentTimestamp), 0L);
            this.Init(nameof(this.State), state);
            this.Init(nameof(this.StateTimestamp), 0L);
            this.Init(nameof(this.AffectMainCardVersionWhenStateChanged), true);
            this.Init(nameof(this.AffectMainCardVersionWhenStateChangedTimestamp), 0L);
        }
        
        /// <inheritdoc />
        public MainProcessCommonInfo(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        private MainProcessCommonInfo(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region properties

        public Guid? AuthorID
        {
            get => this.Get<Guid?>(nameof(this.AuthorID));
            set => this.Set(nameof(this.AuthorID), value);
        }
        
        public string AuthorName
        {
            get => this.Get<string>(nameof(this.AuthorName));
            set => this.Set(nameof(this.AuthorName), value);
        }
        
        public long AuthorTimestamp
        {
            get => this.Get<long>(nameof(this.AuthorTimestamp));
            set => this.Set(nameof(this.AuthorTimestamp), value);
        }
        
        public string AuthorComment
        {
            get => this.Get<string>(nameof(this.AuthorComment));
            set => this.Set(nameof(this.AuthorComment), value);
        }

        public long AuthorCommentTimestamp
        {
            get => this.Get<long>(nameof(this.AuthorCommentTimestamp));
            set => this.Set(nameof(this.AuthorCommentTimestamp), value);
        }
        
        public int State
        {
            get => this.Get<int>(nameof(this.State));
            set => this.Set(nameof(this.State), value);
        }
        
        public long StateTimestamp
        {
            get => this.Get<long>(nameof(this.StateTimestamp));
            set => this.Set(nameof(this.StateTimestamp), value);
        }

        public bool AffectMainCardVersionWhenStateChanged
        {
            get => this.Get<bool>(nameof(this.AffectMainCardVersionWhenStateChanged));
            set => this.Set(nameof(this.AffectMainCardVersionWhenStateChanged), value);
        }
        
        public long AffectMainCardVersionWhenStateChangedTimestamp
        {
            get => this.Get<long>(nameof(this.AffectMainCardVersionWhenStateChangedTimestamp));
            set => this.Set(nameof(this.AffectMainCardVersionWhenStateChangedTimestamp), value);
        }

        #endregion
        
    }
}