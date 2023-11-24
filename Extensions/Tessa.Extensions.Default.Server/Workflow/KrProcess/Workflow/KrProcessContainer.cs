using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.GlobalSignals;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessContainer: IKrProcessContainer
    {
        #region nested types

        private struct StageHandlerItem
        {
            public StageTypeDescriptor Descriptor;
            public Type Type;
        }

        #endregion

        #region fields

        private readonly IUnityContainer unityContainer;
        private readonly IDbScope dbScope;

        private readonly object taskLock = new object();
        private readonly object stageLock = new object();
        private readonly object signalLock = new object();

        /// <summary>
        /// Хендлеры этапов.
        /// </summary>
        private readonly Dictionary<Guid, StageHandlerItem> stageHandlers =
            new Dictionary<Guid, StageHandlerItem>();

        /// <summary>
        /// Хендлеры глобальных сигналов.
        /// </summary>
        private readonly Dictionary<string, List<Type>> globalSignalHandlers =
            new Dictionary<string, List<Type>>();

        /// <summary>
        /// Зарегистрированные типы заданий.
        /// </summary>
        private readonly ConcurrentContainer<Guid, object> registeredTaskTypes =
            new ConcurrentContainer<Guid, object>();

        /// <summary>
        /// Расширенные типы заданий из настроек типового.
        /// </summary>
        private Lazy<ReadOnlyDictionary<Guid, object>> extraTaskTypesLazy;

        /// <summary>
        /// Все хендлеры этапов.
        /// </summary>
        private Lazy<ReadOnlyDictionary<Guid, StageHandlerItem>> allStageHandlersLazy;

        /// <summary>
        /// Отфильтрованные хендлеры этапов.
        /// </summary>
        private Lazy<ReadOnlyDictionary<Guid, StageHandlerItem>> filteredStageHandlersLazy;

        /// <summary>
        /// Все хендлеры сигналов.
        /// </summary>
        private Lazy<ReadOnlyDictionary<string, List<Type>>> allSignalHandlersLazy;

        /// <summary>
        /// Отфильтрованные хендлеры сигналов.
        /// </summary>
        private Lazy<ReadOnlyDictionary<string, List<Type>>> filteredSignalHandlersLazy;


        private readonly HashSet<Guid> stageHandlersToExclude = new HashSet<Guid>();
        private readonly HashSet<string> signalsToExclude = new HashSet<string>();
        private readonly HashSet<SignalFilterItem> signalHandlersToExclude = new HashSet<SignalFilterItem>();

        #endregion

        #region constructor

        public KrProcessContainer(IUnityContainer unityContainer,
            IDbScope dbScope)
        {
            this.unityContainer = unityContainer;
            this.dbScope = dbScope;
            this.ResetStages();
            this.ResetExtraTasksTypesInternal();
            this.ResetSignals();
        }

        #endregion

        #region implementation

        public IKrProcessContainer RegisterHandler<T>(
            StageTypeDescriptor descriptor)
            where T : IStageTypeHandler
        {
            if (!this.unityContainer.IsRegistered<T>())
            {
                throw new ArgumentException(
                    $"Type {typeof(T).FullName} is not registered in UnityContainer.{Environment.NewLine}" +
                    $"Add container.RegisterType<{typeof(IStageTypeHandler).Name}, {typeof(T).Name}>() in your Registrator class.");
            }
            this.RegisterHandlerInternal(descriptor, typeof(T));
            return this;
        }

        public IKrProcessContainer RegisterHandler(StageTypeDescriptor descriptor, Type handlerType)
        {
            if (!this.unityContainer.IsRegistered(handlerType))
            {
                throw new ArgumentException(
                    $"Type {handlerType.FullName} is not registered in UnityContainer.{Environment.NewLine}" +
                    $"Add container.RegisterType<{typeof(IStageTypeHandler).Name}, {handlerType.Name}>() in your Registrator class.");
            }
            this.RegisterHandlerInternal(descriptor, handlerType);
            return this;
        }

        /// <inheritdoc />
        public IKrProcessContainer RegisterTaskType(
            Guid taskTypeID)
        {
            this.registeredTaskTypes[taskTypeID] = null;
            return this;
        }

        /// <inheritdoc />
        public IKrProcessContainer RegisterTaskType(
            IEnumerable<Guid> taskTypeID)
        {
            foreach (var guid in taskTypeID)
            {
                this.registeredTaskTypes[guid] = null;
            }

            return this;
        }

        /// <inheritdoc />
        public IKrProcessContainer ResetExtraTaskTypes()
        {
            this.ResetExtraTasksTypesInternal();
            return this;
        }

        /// <inheritdoc />
        public IKrProcessContainer RegisterGlobalSignal(
            string signalType,
            Type handlerType)
        {
            if (!this.unityContainer.IsRegistered(handlerType))
            {
                throw new ArgumentException(
                    $"Type {handlerType.FullName} is not registered in UnityContainer.{Environment.NewLine}" +
                    $"Add container.RegisterType<{typeof(IGlobalSignalHandler).Name}, {handlerType.Name}>() in your Registrator class.");
            }
            this.RegisterGlobalSignalInternal(signalType, handlerType);
            return this;
        }

        /// <inheritdoc />
        public IKrProcessContainer RegisterGlobalSignal<T>(
            string signalType)
            where T : IGlobalSignalHandler
        {
            if (!this.unityContainer.IsRegistered<T>())
            {
                throw new ArgumentException(
                    $"Type {typeof(T).FullName} is not registered in UnityContainer.{Environment.NewLine}" +
                    $"Add container.RegisterType<{typeof(IGlobalSignalHandler).Name}, {typeof(T).Name}>() in your Registrator class.");
            }
            this.RegisterGlobalSignalInternal(signalType, typeof(T));
            return this;
        }

        public IKrProcessContainer AddFilter<T>(IKrProcessFilter<T> filter)
        {
            if (filter.Excluded?.Count >= 0 != true)
            {
                return this;
            }

            if (filter is StageTypeFilter)
            {
                var stageTypeFilter = (IKrProcessFilter<Guid>) filter;
                lock (this.stageLock)
                {
                    foreach (var id in stageTypeFilter.Excluded)
                    {
                        this.stageHandlersToExclude.Add(id);
                    }
                    this.ResetStages();
                }
            }
            else if (filter is SignalFilter)
            {
                var stageTypeFilter = (IKrProcessFilter<string>)filter;
                lock (this.signalLock)
                {
                    foreach (var type in stageTypeFilter.Excluded)
                    {
                        this.signalsToExclude.Add(type);
                    }
                    this.ResetSignals();
                }
            }
            else if (filter is SignalHandlerFilter)
            {
                var stageTypeFilter = (IKrProcessFilter<SignalFilterItem>)filter;
                lock (this.signalLock)
                {
                    foreach (var typeHandler in stageTypeFilter.Excluded)
                    {
                        this.signalHandlersToExclude.Add(typeHandler);
                    }
                    this.ResetSignals();
                }
            }

            return this;
        }

        public ICollection<StageTypeDescriptor> GetHandlerDescriptors(
            bool withFilters)
        {
            var items = withFilters
                ? this.filteredStageHandlersLazy.Value
                : this.allStageHandlersLazy.Value;
            return items
                .Select(p => p.Value.Descriptor)
                .ToArray();
        }


        public StageTypeDescriptor GetHandlerDescriptor(
            Guid descriptorID,
            bool withFilters)
        {
            var items = withFilters
                ? this.filteredStageHandlersLazy.Value
                : this.allStageHandlersLazy.Value;

            return items.TryGetValue(descriptorID, out var item)
                ? item.Descriptor
                : null;
        }

        public IStageTypeHandler ResolveHandler(Guid descriptorID, bool withFilters)
        {
            var currentItems = withFilters
                ? this.filteredStageHandlersLazy.Value
                : this.allStageHandlersLazy.Value;

            if (currentItems.TryGetValue(descriptorID, out var item))
            {
                return (IStageTypeHandler)this.unityContainer.Resolve(item.Type);
            }
            return null;
        }

        /// <inheritdoc />
        public List<IGlobalSignalHandler> ResolveSignal(
            string signal,
            bool withFilters)
        {
            var currentItems = withFilters
                ? this.filteredSignalHandlersLazy.Value
                : this.allSignalHandlersLazy.Value;

            if (currentItems.TryGetValue(signal, out var types))
            {
                var handlers = new List<IGlobalSignalHandler>(signal.Length);
                foreach (var type in types)
                {
                    handlers.Add((IGlobalSignalHandler)this.unityContainer.Resolve(type));
                }
                return handlers;
            }
            return null;
        }

        /// <inheritdoc />
        public bool IsTaskTypeRegistered(
            Guid taskTypeID)
        {
            return this.registeredTaskTypes.ContainsKey(taskTypeID)
                || this.extraTaskTypesLazy.Value.ContainsKey(taskTypeID);
        }

        #endregion

        #region private

        private void RegisterHandlerInternal(StageTypeDescriptor descriptor, Type t)
        {
            lock (this.stageLock)
            {
                this.stageHandlers[descriptor.ID] =
                    new StageHandlerItem
                    {
                        Descriptor = descriptor,
                        Type = t,
                    };
                this.ResetStages();
            }
        }

        private void RegisterGlobalSignalInternal(
            string signalType,
            Type t)
        {
            lock (this.signalLock)
            {
                if (!this.globalSignalHandlers.TryGetValue(signalType, out var list))
                {
                    list = new List<Type>();
                    this.globalSignalHandlers[signalType] = list;
                }

                list.Add(t);
                this.ResetSignals();
            }
        }

        private void ResetExtraTasksTypesInternal()
        {
            this.extraTaskTypesLazy = new Lazy<ReadOnlyDictionary<Guid, object>>(
                this.GetExtraTasks,
                LazyThreadSafetyMode.PublicationOnly);
        }

        private void ResetStages()
        {
            this.allStageHandlersLazy = new Lazy<ReadOnlyDictionary<Guid, StageHandlerItem>>(
                this.GetAllStagesHashSet,
                LazyThreadSafetyMode.PublicationOnly);

            this.filteredStageHandlersLazy = new Lazy<ReadOnlyDictionary<Guid, StageHandlerItem>>(
                this.GetFilteredStagesHashSet,
                LazyThreadSafetyMode.PublicationOnly);
        }

        private ReadOnlyDictionary<Guid, object> GetExtraTasks()
        {
            lock (this.taskLock)
            {
                var tmpDict = new Dictionary<Guid, object>();

                using (this.dbScope.Create())
                {
                    var db = this.dbScope.Db;
                    db.SetCommand(
                        this.dbScope.BuilderFactory
                            .Select()
                            .C(KrConstants.KrSettingsRouteExtraTaskTypes.TaskTypeID)
                            .From(KrConstants.KrSettingsRouteExtraTaskTypes.Name).NoLock()
                            .Build())
                        .LogCommand();

                    using var reader = db.ExecuteReader();
                    while (reader.Read())
                    {
                        tmpDict[reader.GetGuid(0)] = null;
                    }
                }

                return new ReadOnlyDictionary<Guid, object>(tmpDict);
            }
        }

        private ReadOnlyDictionary<Guid, StageHandlerItem> GetAllStagesHashSet()
        {
            lock (this.stageLock)
            {
                return new ReadOnlyDictionary<Guid, StageHandlerItem>(this.stageHandlers
                    .ToDictionary(k => k.Key, v => v.Value));
            }
        }

        private ReadOnlyDictionary<Guid, StageHandlerItem> GetFilteredStagesHashSet()
        {
            lock (this.stageLock)
            {
                return new ReadOnlyDictionary<Guid, StageHandlerItem>(this.stageHandlers
                    .Where(p => !this.stageHandlersToExclude.Contains(p.Key))
                    .ToDictionary(k => k.Key, v => v.Value));
            }
        }

        private void ResetSignals()
        {
            this.allSignalHandlersLazy = new Lazy<ReadOnlyDictionary<string, List<Type>>>(
                this.GetAllSignalsHashSet,
                LazyThreadSafetyMode.PublicationOnly);

            this.filteredSignalHandlersLazy = new Lazy<ReadOnlyDictionary<string, List<Type>>>(
                this.GetFilteredSignalsHashSet,
                LazyThreadSafetyMode.PublicationOnly);
        }

        private ReadOnlyDictionary<string, List<Type>> GetAllSignalsHashSet()
        {
            lock (this.signalLock)
            {
                return new ReadOnlyDictionary<string, List<Type>>(this.globalSignalHandlers
                    .ToDictionary(p => p.Key, q => q.Value));
            }
        }

        private ReadOnlyDictionary<string, List<Type>> GetFilteredSignalsHashSet()
        {
            lock (this.signalLock)
            {
                var filteredSignals = this.globalSignalHandlers
                    .Where(p => !this.signalsToExclude.Contains(p.Key));

                var result = new Dictionary<string, List<Type>>();
                foreach (var filteredSignal in filteredSignals)
                {
                    var filteredHandlers = filteredSignal.Value
                        .Where(p => this.signalHandlersToExclude.All(q => q.HandlerType != p))
                        .ToList();

                    result[filteredSignal.Key] = filteredHandlers;
                }

                return new ReadOnlyDictionary<string, List<Type>>(result);
            }
        }

        #endregion
    }
}