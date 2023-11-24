using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrRuntimeStage : IKrRuntimeStage
    {
        #region fields

        private readonly Lazy<IDictionary<string, object>> settingsLazy;

        private readonly Lazy<IReadOnlyList<IExtraSource>> extraSourcesLazy;

        #endregion

        #region constructor

        public KrRuntimeStage(
            Guid templateID,
            string templateName,
            Guid groupID,
            string groupName,
            int groupOrder,
            Guid stageID,
            string stageName,
            int? order,
            double? timeLimit,
            DateTime? planned,
            bool hidden,
            Guid stageTypeID,
            string stageTypeCaption,
            string sqlRoles = null,
            string settings = null,
            string extraSources = null,
            string runtimeSqlCondition = null,
            string sourceCondition = null,
            string sourceBefore = null,
            string sourceAfter = null,
            IExtraSourceSerializer extraSourceSerializer = null,
            IKrStageSerializer serializer = null,
            bool skip = default,
            bool canBeSkipped = default)
        {
            this.TemplateID = templateID;
            this.StageID = stageID;
            this.StageName = stageName;
            this.Order = order;
            this.GroupID = groupID;
            this.GroupOrder = groupOrder;
            this.TimeLimit = timeLimit;
            this.Planned = planned;
            this.Hidden = hidden;
            this.StageTypeID = stageTypeID;
            this.StageTypeCaption = stageTypeCaption;
            this.TemplateName = templateName;
            this.GroupName = groupName;
            this.SqlRoles = sqlRoles ?? string.Empty;
            this.RuntimeSqlCondition = runtimeSqlCondition ?? string.Empty;
            this.RuntimeSourceCondition = sourceCondition ?? string.Empty;
            this.RuntimeSourceBefore = sourceBefore ?? string.Empty;
            this.RuntimeSourceAfter = sourceAfter ?? string.Empty;
            this.Skip = skip;
            this.CanBeSkipped = canBeSkipped;

            this.extraSourcesLazy = new Lazy<IReadOnlyList<IExtraSource>>(
                () =>  extraSourceSerializer != null && !string.IsNullOrWhiteSpace(extraSources)
                    ? new ReadOnlyCollection<IExtraSource>(extraSourceSerializer.Deserialize(extraSources).Select(p => p.ToReadonly()).ToList())
                    : EmptyHolder<IExtraSource>.Collection,
                LazyThreadSafetyMode.PublicationOnly);
            this.settingsLazy = new Lazy<IDictionary<string, object>>(
                () =>
                    serializer != null && !string.IsNullOrWhiteSpace(settings)
                        ? serializer.DeserializeSettingsStorageAsync(settings, this.StageID).GetAwaiter().GetResult() // TODO async
                        : null,
                LazyThreadSafetyMode.PublicationOnly);
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public Guid TemplateID { get; }

        /// <inheritdoc />
        public string TemplateName { get; }

        /// <inheritdoc />
        public Guid GroupID { get; }

        /// <inheritdoc />
        public string GroupName { get; }

        /// <inheritdoc />
        public int GroupOrder { get; }

        /// <inheritdoc />
        public Guid StageID { get; }

        /// <inheritdoc />
        public string StageName { get; }

        /// <inheritdoc />
        public int? Order { get; }

        /// <inheritdoc />
        public double? TimeLimit { get; }

        /// <inheritdoc />
        public DateTime? Planned { get; }

        /// <inheritdoc />
        public bool Hidden { get; }

        /// <inheritdoc />
        public Guid StageTypeID { get; }

        /// <inheritdoc />
        public string StageTypeCaption { get; }

        /// <inheritdoc />
        public string SqlRoles { get; }

        /// <inheritdoc />
        public IDictionary<string, object> GetSettings() => StorageHelper.Clone(this.settingsLazy.Value);

        /// <inheritdoc />
        public string RuntimeSqlCondition { get; }

        /// <inheritdoc />
        public string RuntimeSourceCondition { get; }

        /// <inheritdoc />
        public string RuntimeSourceBefore { get; }

        /// <inheritdoc />
        public string RuntimeSourceAfter { get; }

        /// <inheritdoc />
        public IReadOnlyList<IExtraSource> ExtraSources => this.extraSourcesLazy.Value;

        /// <inheritdoc />
        public bool Skip { get; }

        /// <inheritdoc />
        public bool CanBeSkipped { get; }

        #endregion
    }
}