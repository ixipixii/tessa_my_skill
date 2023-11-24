using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public static class KrProcessExtensions
    {
        #region Card

        public static bool IsMainProcessStarted(
            this Card satellite)
        {
            KrErrorHelper.AssertKrSatellte(satellite);

            return satellite.Sections.TryGetValue("WorkflowProcesses", out var wpSec)
                && wpSec.Rows.Any(p => p["TypeName"].Equals(KrConstants.KrProcessName));
        }


        #endregion

        #region IKrScope

        private const string LaunchOnLevel = nameof(LaunchOnLevel);

        private const string SingleRunList = nameof(SingleRunList);

        private const string KrProcessTrace = nameof(KrProcessTrace);

        private const string KrProcessClientCommands = nameof(KrProcessClientCommands);

        private const string LaunchedRunners = nameof(LaunchedRunners);
        
        public static bool FirstLaunchPerRequest(
            this IKrScope scope,
            Guid processID) => !GetListFromInfo(scope, LaunchOnLevel, processID).Contains(scope.CurrentLevel.LevelID);

        public static void AddToLaunchedLevels(
            this IKrScope scope,
            Guid processID) => GetListFromInfo(scope, LaunchOnLevel, processID).Add(scope.CurrentLevel.LevelID);

        public static void DisableMultirunForRequest(
            this IKrScope scope,
            Guid processID) => GetListFromInfo(scope, SingleRunList, processID).Add(scope.CurrentLevel.LevelID);

        public static bool MultirunEnabled(
            this IKrScope scope,
            Guid processID) => !GetListFromInfo(scope, SingleRunList, processID).Contains(scope.CurrentLevel.LevelID);

        public static List<KrProcessTraceItem> GetKrProcessRunnerTrace(
            this KrScopeContext context) => context?.Info?.TryGet<List<KrProcessTraceItem>>(KrProcessTrace);

        public static List<KrProcessTraceItem> GetKrProcessRunnerTrace(
            this IKrScope scope)
        {
            if (!scope.Exists)
            {
                return null;
            }

            if (!scope.Info.TryGetValue(KrProcessTrace, out var traceObj)
                || !(traceObj is List<KrProcessTraceItem> trace))
            {
                trace = new List<KrProcessTraceItem>();
                scope.Info[KrProcessTrace] = trace;
            }

            return trace;
        }

        public static void TryAddToTrace(this IKrScope scope, KrProcessTraceItem traceItem) =>
            scope.GetKrProcessRunnerTrace()?.Add(traceItem);

        public static List<KrProcessClientCommand> GetKrProcessClientCommands(
            this KrScopeContext context) => context?.Info?.TryGet<List<KrProcessClientCommand>>(KrProcessClientCommands);

        public static List<KrProcessClientCommand> GetKrProcessClientCommands(
            this IKrScope scope)
        {
            if (!scope.Exists)
            {
                return null;
            }

            if (!scope.Info.TryGetValue(KrProcessClientCommands, out var commandsListObj)
                || !(commandsListObj is List<KrProcessClientCommand> commandsList))
            {
                commandsList = new List<KrProcessClientCommand>();
                scope.Info[KrProcessClientCommands] = commandsList;
            }

            return commandsList;
        }

        public static void TryAddClientCommand(this IKrScope scope, KrProcessClientCommand clientCommand) =>
            scope.GetKrProcessClientCommands()?.Add(clientCommand);

        public static void AddLaunchedRunner(
            this IKrScope scope,
            Guid processID)
        {
            if (!scope.Exists)
            {
                return;
            }

            if (!scope.Info.TryGetValue(LaunchedRunners, out var runnersObj)
                || !(runnersObj is List<Guid> runnersList))
            {
                runnersList = new List<Guid>();
                scope.Info[LaunchedRunners] = runnersList;
            }

            runnersList.Add(processID);
        }
        
        public static bool HasLaunchedRunner(
            this IKrScope scope,
            Guid processID)
        {
            if (scope.Exists
                && scope.Info.TryGetValue(LaunchedRunners, out var runnersObj)
                && runnersObj is List<Guid> runnersList)
            {
                return runnersList.Contains(processID);
            }

            return false;
        }
        public static void RemoveLaunchedRunner(
            this IKrScope scope,
            Guid processID)
        {
            if (scope.Exists
                && scope.Info.TryGetValue(LaunchedRunners, out var runnersObj)
                && runnersObj is List<Guid> runnersList)
            {
                runnersList.Remove(processID);
            }
        }
        
        #endregion

        #region KrProcessRunnerMode

        public static string GetCaption(
            this KrProcessRunnerMode mode)
        {
            switch (mode)
            {
                case KrProcessRunnerMode.Sync:
                    return "$KrProcess_SyncMode";
                case KrProcessRunnerMode.Async:
                    return "$KrProcess_AsyncMode";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        #endregion

        #region IKrProcessCache
        
        public static (IReadOnlyDictionary<Guid, IKrStageTemplate>, IReadOnlyDictionary<Guid, IReadOnlyCollection<IKrRuntimeStage>>) GetRelatedTemplates(
            this IKrProcessCache processCache,
            Card satellite,
            Guid? nestedProcessID = null)
        {
            // ID карточек KrStageTemplates, на которые ссылаются текущие этапы.
            var dependentTemplateIDs = satellite.GetStagesSection()
                .Rows
                .Where(p => p.Fields[KrConstants.KrStages.BasedOnStageTemplateID] != null
                    && Equals(p.Fields[KrConstants.KrStages.NestedProcessID], nestedProcessID))
                .Select(p => (Guid)p.Fields[KrConstants.KrStages.BasedOnStageTemplateID])
                .Distinct();

            var templatesCache = processCache.GetAllStageTemplates();
            
            var templateTable = new Dictionary<Guid, IKrStageTemplate>();
            var stagesTable = new Dictionary<Guid, IReadOnlyCollection<IKrRuntimeStage>>();
            foreach (var id in dependentTemplateIDs)
            {
                if (templatesCache.TryGetValue(id, out var template))
                {
                    templateTable[id] = template;
                    stagesTable[id] = processCache.GetRuntimeStagesForTemplate(id);
                }
            }
            return (
                new ReadOnlyDictionary<Guid, IKrStageTemplate>(templateTable),
                new ReadOnlyDictionary<Guid, IReadOnlyCollection<IKrRuntimeStage>>(stagesTable));
        }

        
        public static IReadOnlyList<IKrRuntimeStage> GetRuntimeStages(
            this IKrProcessCache processCache,
            ISet<Guid> ids) => GetValuesByIDs(ids, processCache.GetAllRuntimeStages());
        
        public static IReadOnlyList<IKrStageTemplate> GetStageTemplates(
            this IKrProcessCache processCache,
            ISet<Guid> ids) => GetValuesByIDs(ids, processCache.GetAllStageTemplates());
        
        public static IReadOnlyList<IKrStageGroup> GetStageGroups(
            this IKrProcessCache processCache,
            ISet<Guid> ids) => GetValuesByIDs(ids, processCache.GetAllStageGroups());
        
        public static IReadOnlyList<IKrPureProcess> GetPureProcesses(
            this IKrProcessCache processCache,
            ISet<Guid> ids) => GetValuesByIDs(ids, processCache.GetAllPureProcesses());
        
        public static IReadOnlyList<IKrProcessButton> GetButtons(
            this IKrProcessCache processCache,
            ISet<Guid> ids) => GetValuesByIDs(ids, processCache.GetAllButtons());
        
        public static IReadOnlyList<IKrAction> GetActions(
            this IKrProcessCache processCache,
            ISet<Guid> ids) => GetValuesByIDs(ids, processCache.GetAllActions());

        private static IReadOnlyList<T> GetValuesByIDs<T>(
            ISet<Guid> ids,
            IReadOnlyDictionary<Guid, T> values)
        {
            var list = new List<T>(ids.Count);
            foreach (var id in ids)
            {
                if (values.TryGetValue(id, out var val))
                {
                    list.Add(val);
                }
            }

            return list;
        }

        #endregion
        
        #region private

        private static List<Guid> GetListFromInfo(
            IKrScope scope,
            string listKey,
            Guid processID)
        {
            Dictionary<Guid, List<Guid>> singleRunDict;
            if ((singleRunDict = scope.Info.TryGet<Dictionary<Guid, List<Guid>>>(listKey)) is null)
            {
                singleRunDict = new Dictionary<Guid, List<Guid>>();
                scope.Info[listKey] = singleRunDict;
            }

            if (!singleRunDict.TryGetValue(processID, out var singleRunList))
            {
                singleRunList = new List<Guid>();
                singleRunDict[processID] = singleRunList;
            }

            return singleRunList;
        }

        #endregion
        
    }
}