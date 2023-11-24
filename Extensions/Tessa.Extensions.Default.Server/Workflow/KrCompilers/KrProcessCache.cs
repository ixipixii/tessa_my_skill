using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Кэш для данных из карточек шаблонов этапов
    /// </summary>
    public sealed class KrProcessCache : IKrProcessCache
    {
        #region CachedObject Private Class

        private sealed class CachedObject
        {
            #region Constructors

            public CachedObject(
                IEnumerable<IKrStageTemplate> stages,
                IEnumerable<IKrRuntimeStage> runtimeStages,
                IReadOnlyCollection<IKrStageGroup> stageGroups,
                IEnumerable<IKrPureProcess> pureProcesses,
                IEnumerable<IKrAction> actions,
                IEnumerable<IKrProcessButton> buttons,
                IEnumerable<IKrCommonMethod> methods)
            {
                this.Stages = new ReadOnlyDictionary<Guid, IKrStageTemplate>(stages.ToDictionary(k => k.ID, v => v));

                this.StageTemplatesByGroups = new ReadOnlyDictionary<Guid, ReadOnlyCollection<IKrStageTemplate>>(this.Stages
                    .GroupBy(p => p.Value.StageGroupID)
                    .ToDictionary(k => k.Key, v => v.Select(p => p.Value).ToList().AsReadOnly()));

                this.RuntimeStages = new ReadOnlyDictionary<Guid, IKrRuntimeStage>(runtimeStages.ToDictionary(k => k.StageID, v => v));

                this.RuntimeStagesByTemplates = new ReadOnlyDictionary<Guid, ReadOnlyCollection<IKrRuntimeStage>>(this.RuntimeStages
                    .GroupBy(p => p.Value.TemplateID)
                    .ToDictionary(k => k.Key, v => v.Select(p => p.Value).ToList().AsReadOnly()));

                this.StageGroups = new ReadOnlyDictionary<Guid, IKrStageGroup>(stageGroups.ToDictionary(k => k.ID, v => v));

                this.OrderedStageGroups = stageGroups.OrderBy(p => p.Order).ThenBy(p => p.ID).ToList().AsReadOnly();

                this.StageGroupsByProcesses = new ReadOnlyDictionary<Guid, ReadOnlyCollection<IKrStageGroup>>(this.StageGroups
                    .GroupBy(p => p.Value.SecondaryProcessID ?? Guid.Empty)
                    .ToDictionary(k => k.Key, v => v.Select(p => p.Value).ToList().AsReadOnly()));

                this.PureProcesses = new ReadOnlyDictionary<Guid, IKrPureProcess>(pureProcesses.ToDictionary(k => k.ID, v => v));

                this.Actions = new ReadOnlyDictionary<Guid, IKrAction>(actions.ToDictionary(k => k.ID, v => v));

                this.ActionsByTypes = new ReadOnlyDictionary<string, ReadOnlyCollection<IKrAction>>(this.Actions
                    .Where(p => p.Value.EventType != null)
                    .GroupBy(p => p.Value.EventType)
                    .ToDictionary(k => k.Key, v => v.Select(p => p.Value).ToList().AsReadOnly()));

                this.Buttons = new ReadOnlyDictionary<Guid, IKrProcessButton>(buttons.ToDictionary(k => k.ID, v => v));

                this.Methods = methods.ToList().AsReadOnly();
            }

            #endregion

            #region Properties

            public ReadOnlyDictionary<Guid, IKrStageTemplate> Stages { get; }

            public ReadOnlyDictionary<Guid, ReadOnlyCollection<IKrStageTemplate>> StageTemplatesByGroups { get; }

            public ReadOnlyDictionary<Guid, IKrRuntimeStage> RuntimeStages { get; }

            public ReadOnlyDictionary<Guid, ReadOnlyCollection<IKrRuntimeStage>> RuntimeStagesByTemplates { get; }

            public ReadOnlyDictionary<Guid, IKrStageGroup> StageGroups { get; }

            public ReadOnlyCollection<IKrStageGroup> OrderedStageGroups { get; }

            public ReadOnlyDictionary<Guid, ReadOnlyCollection<IKrStageGroup>> StageGroupsByProcesses { get; }

            public ReadOnlyDictionary<Guid, IKrPureProcess> PureProcesses { get; }

            public ReadOnlyDictionary<Guid, IKrAction> Actions { get; }

            public ReadOnlyDictionary<string, ReadOnlyCollection<IKrAction>> ActionsByTypes { get; }

            public ReadOnlyDictionary<Guid, IKrProcessButton> Buttons { get; }

            public ReadOnlyCollection<IKrCommonMethod> Methods { get; }

            #endregion
        }

        #endregion

        #region constants

        private const string CacheKey = "KrProcessCache";

        #endregion

        #region fields

        private readonly ICardCache cardCache;
        private readonly IDbScope dbScope;
        private readonly IExtraSourceSerializer extraSourceSerializer;
        private readonly IKrStageSerializer stageSerializer;

        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        #endregion

        #region constructor

        public KrProcessCache(
            ICardCache cardCache,
            IDbScope dbScope,
            IExtraSourceSerializer extraSourceSerializer,
            IKrStageSerializer stageSerializer)
        {
            Check.ArgumentNotNull(cardCache, nameof(cardCache));
            Check.ArgumentNotNull(dbScope, nameof(dbScope));

            this.cardCache = cardCache;
            this.dbScope = dbScope;
            this.extraSourceSerializer = extraSourceSerializer;
            this.stageSerializer = stageSerializer;
        }

        #endregion

        #region private

        private CachedObject UpdateCachedObject(string key)
        {
            using (new WriterLockSlimWrapper(this.cacheLock))
            {
                // соединение с БД для заполнения кэша не должно зависеть от текущего соединения и его транзакции
                using (this.dbScope.CreateNew())
                {
                    List<IKrStageTemplate> stages = KrCompilersSqlHelper.SelectStageTemplates(this.dbScope);
                    stages.AddRange(KrCompilersSqlHelper.SelectVirtualStageTemplates(this.dbScope));
                    List<IKrRuntimeStage> runtimeStages = KrCompilersSqlHelper.SelectRuntimeStages(this.dbScope, this.stageSerializer, this.extraSourceSerializer);
                    runtimeStages.AddRange(KrCompilersSqlHelper.SelectSecondaryProcessRuntimeStages(this.dbScope, this.stageSerializer, this.extraSourceSerializer));
                    List<IKrStageGroup> stageGroups = KrCompilersSqlHelper.SelectStageGroups(this.dbScope);
                    stageGroups.AddRange(KrCompilersSqlHelper.SelectVirtualStageGroups(this.dbScope));
                    List<IKrCommonMethod> methods = KrCompilersSqlHelper.SelectCommonMethods(this.dbScope);
                    KrCompilersSqlHelper.SelectKrSecondaryProcesses(
                        this.dbScope, null,
                        out var pureProcesses,
                        out var actions,
                        out var buttons);
                    return new CachedObject(stages, runtimeStages, stageGroups, pureProcesses, actions, buttons, methods);
                }
            }
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IKrStageGroup> GetAllStageGroups()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().StageGroups; // TODO async
        }

        /// <inheritdoc />
        public IReadOnlyList<IKrStageGroup> GetOrderedStageGroups()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().OrderedStageGroups; // TODO async
        }

        /// <inheritdoc />
        public IReadOnlyList<IKrStageGroup> GetStageGroupsForSecondaryProcess(
            Guid? process)
        {
            var groupsByProcesses = this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().StageGroupsByProcesses; // TODO async

            return groupsByProcesses.TryGetValue(process ?? Guid.Empty, out var groupList)
                ? groupList
                : EmptyHolder<IKrStageGroup>.Collection;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IKrStageTemplate> GetAllStageTemplates()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().Stages; // TODO async
        }

        /// <inheritdoc />
        public IReadOnlyList<IKrStageTemplate> GetStageTemplatesForGroup(
            Guid groupID)
        {
            var stagesByGroups = this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().StageTemplatesByGroups; // TODO async

            return stagesByGroups.TryGetValue(groupID, out var templateList)
                ? templateList
                : EmptyHolder<IKrStageTemplate>.Collection;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IKrRuntimeStage> GetAllRuntimeStages()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().RuntimeStages; // TODO async
        }

        /// <inheritdoc />
        public IReadOnlyList<IKrRuntimeStage> GetRuntimeStagesForTemplate(
            Guid templateID)
        {
            var runtimeStagesByTemplates = this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().RuntimeStagesByTemplates; // TODO async

            return runtimeStagesByTemplates.TryGetValue(templateID, out var stagesList)
                ? stagesList
                : EmptyHolder<IKrRuntimeStage>.Collection;
        }

        /// <inheritdoc />
        public IReadOnlyList<IKrCommonMethod> GetAllCommonMethods()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().Methods; // TODO async
        }

        /// <inheritdoc />
        public IKrSecondaryProcess GetSecondaryProcess(
            Guid pid)
        {
            var cachedObject = this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult(); // TODO async
            if (cachedObject.PureProcesses.TryGetValue(pid, out var pure))
            {
                return pure;
            }
            if (cachedObject.Buttons.TryGetValue(pid, out var button))
            {
                return button;
            }
            if (cachedObject.Actions.TryGetValue(pid, out var action))
            {
                return action;
            }
            throw new InvalidOperationException($"Process with ID = {pid} doesn't exist.");
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IKrPureProcess> GetAllPureProcesses()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().PureProcesses; // TODO async
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IKrAction> GetActionsByType(
            string actionType)
        {
            var actionsByTypes = this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().ActionsByTypes; // TODO async

            return actionsByTypes.TryGetValue(actionType, out var actionsList)
                ? actionsList
                : EmptyHolder<IKrAction>.Collection;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IKrAction> GetAllActions()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().Actions; // TODO async
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, IKrProcessButton> GetAllButtons()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.UpdateCachedObject).GetAwaiter().GetResult().Buttons; // TODO async
        }

        /// <inheritdoc />
        public void Invalidate()
        {
            using (new ReaderLockSlimWrapper(this.cacheLock))
            {
                this.cardCache.Settings.InvalidateAsync(CacheKey).GetAwaiter().GetResult(); // TODO async
            }
        }

        #endregion
    }
}
