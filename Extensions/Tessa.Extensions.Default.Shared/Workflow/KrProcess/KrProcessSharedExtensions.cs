using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Platform;
using Tessa.Platform.Initialization;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    /// <summary>
    /// Методы-расширения для пространства имён <c>Tessa.Extensions.Default.Shared.Workflow.KrProcess</c>.
    /// </summary>
    public static class KrProcessSharedExtensions
    {
        #region Flags Extensions

        /// <doc path='info[@type="flags" and @item="Has"]'/>
        public static bool Has(this KrComponents flags, KrComponents flag)
        {
            return (flags & flag) == flag;
        }

        /// <doc path='info[@type="flags" and @item="HasAny"]'/>
        public static bool HasAny(this KrComponents flags, KrComponents flag)
        {
            return (flags & flag) != 0;
        }

        /// <doc path='info[@type="flags" and @item="HasNot"]'/>
        public static bool HasNot(this KrComponents flags, KrComponents flag)
        {
            return (flags & flag) == 0;
        }

        /// <doc path='info[@type="flags" and @item="Has"]'/>
        public static bool Has(this InfoAboutChanges flags, InfoAboutChanges flag)
        {
            return (flags & flag) == flag;
        }

        /// <doc path='info[@type="flags" and @item="HasAny"]'/>
        public static bool HasAny(this InfoAboutChanges flags, InfoAboutChanges flag)
        {
            return (flags & flag) != 0;
        }

        /// <doc path='info[@type="flags" and @item="HasNot"]'/>
        public static bool HasNot(this InfoAboutChanges flags, InfoAboutChanges flag)
        {
            return (flags & flag) == 0;
        }

        #endregion

        #region Card Extensions

        public static CardSection GetApprovalInfoSection(this Card card)
        {
            return card.Sections[GetApprovalInfoAlias(card)];
        }

        public static bool TryGetKrApprovalCommonInfoSection(this Card card, out CardSection section)
        {
            return card.Sections.TryGetValue(GetApprovalInfoAlias(card), out section);
        }

        public static CardSection GetStagesSection(this Card card, bool preferVirtual = false)
        {
            return card.Sections[GetStagesAlias(card, preferVirtual)];
        }

        public static bool TryGetStagesSection(this Card card, out CardSection section, bool preferVirtual = false)
        {
            return card.Sections.TryGetValue(GetStagesAlias(card, preferVirtual), out section);
        }

        public static CardSection GetPerformersSection(this Card card)
        {
            return card.TypeID == DefaultCardTypes.KrApprovalStageTypeSettingsTypeID
                ? card.Sections[KrPerformersVirtual.Name]
                : card.Sections[KrPerformersVirtual.Synthetic];
        }

        public static bool TryGetPerformersSection(this Card card, out CardSection section)
        {
            var secAlias = card.TypeID == DefaultCardTypes.KrApprovalStageTypeSettingsTypeID
                ? KrPerformersVirtual.Name
                : KrPerformersVirtual.Synthetic;

            return card.Sections.TryGetValue(secAlias, out section);
        }

        public static CardSection GetActiveTasksSection(this Card card)
        {
            return card.TypeID == DefaultCardTypes.KrSatelliteTypeID
                ? card.Sections[KrActiveTasks.Name]
                : card.Sections[KrActiveTasks.Virtual];
        }

        public static bool TryGetActiveTasksSection(this Card card, out CardSection section)
        {
            var secAlias = card.TypeID == DefaultCardTypes.KrSatelliteTypeID
                ? KrActiveTasks.Name
                : KrActiveTasks.Virtual;

            return card.Sections.TryGetValue(secAlias, out section);
        }

        public static CardSection GetKrApprovalHistorySection(this Card card)
        {
            return card.TypeID == DefaultCardTypes.KrSatelliteTypeID
                ? card.Sections[KrApprovalHistory.Name]
                : card.Sections[KrApprovalHistory.Virtual];
        }

        public static bool TryGetKrApprovalHistorySection(this Card card, out CardSection section)
        {
            var secAlias = card.TypeID == DefaultCardTypes.KrSatelliteTypeID
                ? KrApprovalHistory.Name
                : KrApprovalHistory.Virtual;

            return card.Sections.TryGetValue(secAlias, out section);
        }

        private static string GetApprovalInfoAlias(Card card)
        {
            string secAlias;
            if (card.TypeID == DefaultCardTypes.KrSatelliteTypeID)
            {
                secAlias = KrApprovalCommonInfo.Name;
            }
            else if (card.TypeID == DefaultCardTypes.KrSecondarySatelliteTypeID)
            {
                secAlias = KrSecondaryProcessCommonInfo.Name;
            }
            else
            {
                secAlias = KrApprovalCommonInfo.Virtual;
            }

            return secAlias;
        }

        private static string GetStagesAlias(
            Card card,
            bool preferVirtual)
        {
            string secAlias;
            if (card.TypeID == DefaultCardTypes.KrSatelliteTypeID
                || card.TypeID == DefaultCardTypes.KrSecondarySatelliteTypeID
                || (card.TypeID == DefaultCardTypes.KrStageTemplateTypeID && !preferVirtual)
                || (card.TypeID == DefaultCardTypes.KrSecondaryProcessTypeID && !preferVirtual))
            {
                secAlias = KrStages.Name;
            }
            else
            {
                secAlias = KrStages.Virtual;
            }

            return secAlias;
        }

        #endregion

        #region ICardFieldContainer

        /// <summary>
        /// Подставить новое значение в поле, если изменено
        /// </summary>
        /// <param name="aci"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIfDiffer<T>(this ICardFieldContainer aci, string key, T value)
        {
            if (aci.Fields.TryGetValue(key, out var oldValue)
                && Equals(value, oldValue))
            {
                return false;
            }

            aci.Fields[key] = value;
            return true;
        }

        #endregion

        #region CardInfoStorageObject

        internal const string SecondaryProcessInfoMark = nameof(SecondaryProcessInfoMark);

        public static void SetStartingSecondaryProcess(
            this CardInfoStorageObject request,
            StartingSecondaryProcessInfo info)
        {
            request.Info[SecondaryProcessInfoMark] = info.GetStorage();
        }

        public static StartingSecondaryProcessInfo GetStartingSecondaryProcess(
            this CardInfoStorageObject request)
        {
            if (request.Info.TryGetValue(SecondaryProcessInfoMark, out var infoObj)
                && infoObj is Dictionary<string, object> info)
            {
                return new StartingSecondaryProcessInfo(info);
            }

            return null;
        }

        public static void RemoveSecondaryProcess(this CardInfoStorageObject request)
        {
            request.Info.Remove(SecondaryProcessInfoMark);
        }

        public const string KrProcessClientCommandInfoMark = nameof(KrProcessClientCommandInfoMark);

        public static void AddKrProcessClientCommands(
            this CardInfoStorageObject storage,
            List<KrProcessClientCommand> commands)
        {
            if (!storage.Info.TryGetValue(KrProcessClientCommandInfoMark, out var clientCommandsObj)
                || !(clientCommandsObj is List<object> clientCommands))
            {
                clientCommands = new List<object>();
            }
            clientCommands.AddRange(commands.Select(p => p.GetStorage()));
            storage.Info[KrProcessClientCommandInfoMark] = clientCommands;
        }

        public static List<KrProcessClientCommand> GetKrProcessClientCommands(
            this CardInfoStorageObject storage)
        {
            if (storage.Info.TryGetValue(KrProcessClientCommandInfoMark, out var commandObj))
            {
                switch (commandObj)
                {
                    case List<object> groupsObjList:
                        return groupsObjList
                            .Cast<Dictionary<string, object>>()
                            .Select(p => new KrProcessClientCommand(p))
                            .ToList();

                    case List<Dictionary<string, object>> groupsStorage:
                        return groupsStorage
                            .Select(p => new KrProcessClientCommand(p))
                            .ToList();
                }
            }

            return null;
        }

        public static bool TryGetKrProcessClientCommands(
            this CardInfoStorageObject storage,
            out List<KrProcessClientCommand> clientCommands)
        {
            var value = GetKrProcessClientCommands(storage);
            clientCommands = value;
            return value != null;
        }

        internal const string IgnoreButtonsInfoMark = nameof(IgnoreButtonsInfoMark);

        public static void IgnoreButtons(
            this CardInfoStorageObject request,
            bool value = true)
        {
            request.Info[IgnoreButtonsInfoMark] = BooleanBoxes.Box(value);
        }

        public static bool AreButtonsIgnored(
            this CardInfoStorageObject request)
        {
            return request.Info.TryGet(IgnoreButtonsInfoMark, false);
        }

        internal const string DontHideStagesInfoMark = nameof(DontHideStagesInfoMark);

        public static void DontHideStages(
            this Dictionary<string, object> storage,
            bool value = true)
        {
            storage[DontHideStagesInfoMark] = BooleanBoxes.Box(value);
        }

        public static void DontHideStages(
            this CardInfoStorageObject request,
            bool value = true)
        {
            request.Info.DontHideStages(value);
        }

        public static bool ConsiderHiddenStages(
            this CardInfoStorageObject request)
        {
            return !request.Info.TryGet(DontHideStagesInfoMark, false);
        }


        internal const string IgnoreKrSatelliteInfoMark = nameof(IgnoreKrSatelliteInfoMark);

        public static void IgnoreKrSatellite(
            this CardInfoStorageObject request,
            bool value = true)
        {
            request.Info[IgnoreKrSatelliteInfoMark] = BooleanBoxes.Box(value);
        }

        public static bool IsKrSatelliteIgnored(
            this CardInfoStorageObject request)
        {
            return request.Info.TryGet(IgnoreKrSatelliteInfoMark, false);
        }


        internal const string GlobalTilesInfoMark = nameof(GlobalTilesInfoMark);

        public static void SetGlobalTiles(
            this InitializationResponse response,
            List<KrTileInfo> tileInfos)
        {
            response.Info[GlobalTilesInfoMark] = tileInfos
                .Select(p => p.GetStorage())
                .ToList();
        }

        public static List<KrTileInfo> GetGlobalTiles(
            this InitializationResponse response)
        {
            if (response.Info.TryGetValue(GlobalTilesInfoMark, out var groupsObj))
            {
                switch (groupsObj)
                {
                    case List<object> groupsObjList:
                        return groupsObjList
                            .Cast<Dictionary<string, object>>()
                            .Select(p => new KrTileInfo(p))
                            .ToList();
                    case List<Dictionary<string, object>> groupsStorage:
                        return groupsStorage
                            .Select(p => new KrTileInfo(p))
                            .ToList();
                }
            }

            return null;
        }

        public static bool TryGetGlobalTiles(
            this InitializationResponse response,
            out List<KrTileInfo> globalTiles)
        {
            var value = GetGlobalTiles(response);
            globalTiles = value;
            return value != null;
        }

        internal const string LocalTilesInfoMark = nameof(LocalTilesInfoMark);

        public static void SetLocalTiles(
            this CardInfoStorageObject request,
            List<KrTileInfo> tileInfos)
        {
            request.Info[LocalTilesInfoMark] = tileInfos
                .Select(p => p.GetStorage())
                .ToList();
        }

        public static List<KrTileInfo> GetLocalTiles(
            this CardInfoStorageObject storage)
        {
            if (storage.Info.TryGetValue(LocalTilesInfoMark, out var groupsObj))
            {
                switch (groupsObj)
                {
                    case List<object> groupsObjList:
                        return groupsObjList
                            .Cast<Dictionary<string, object>>()
                            .Select(p => new KrTileInfo(p))
                            .ToList();
                    case List<Dictionary<string, object>> groupsStorage:
                        return groupsStorage
                            .Select(p => new KrTileInfo(p))
                            .ToList();
                }
            }

            return null;
        }

        public static bool TryGetLocalTiles(
            this CardInfoStorageObject storage,
            out List<KrTileInfo> globalTiles)
        {
            var value = GetLocalTiles(storage);
            globalTiles = value;
            return value != null;
        }

        public static void RemoveLocalTiles(
            this CardInfoStorageObject storage)
        {
            storage?.Info?.Remove(LocalTilesInfoMark);
        }


        internal const string StartKrProcessInstance = nameof(StartKrProcessInstance);

        public static void SetKrProcessInstance(
            this CardInfoStorageObject storage,
            KrProcessInstance krProcess)
        {
            storage.Info.SetKrProcessInstance(krProcess);
        }

        public static void SetKrProcessInstance(
            this Dictionary<string, object> storage,
            KrProcessInstance krProcess)
        {
            storage[StartKrProcessInstance] = krProcess.GetStorage();
        }

        public static KrProcessInstance GetKrProcessInstance(
            this CardInfoStorageObject storage)
        {
            if (storage.Info.TryGetValue(StartKrProcessInstance, out var processObj)
                && processObj is Dictionary<string, object> processDict)
            {
                return new KrProcessInstance(processDict);
            }

            return null;
        }

        public static bool TryGetKrProcessInstance(
            this CardInfoStorageObject storage,
            out KrProcessInstance pID)
        {
            var value = GetKrProcessInstance(storage);
            pID = value;
            return value != null;
        }

        internal const string KrProcessLaunchResult = nameof(KrProcessLaunchResult);

        public static void SetKrProcessLaunchResult(
            this CardInfoStorageObject storage,
            KrProcessLaunchResult launchResult)
        {
            storage.Info[KrProcessLaunchResult] = launchResult.GetStorage();
        }

        public static KrProcessLaunchResult GetKrProcessLaunchResult(
            this CardInfoStorageObject storage)
        {
            if (storage.Info.TryGetValue(KrProcessLaunchResult, out var processObj)
                && processObj is Dictionary<string, object> processDict)
            {
                return new KrProcessLaunchResult(processDict);
            }

            return null;
        }

        public static bool TryGetKrProcessLaunchResult(
            this CardInfoStorageObject storage,
            out KrProcessLaunchResult result)
        {
            var value = GetKrProcessLaunchResult(storage);
            result = value;
            return value != null;
        }

        internal const string StagePositionsInfoMark = "StagePositions";

        public static void SetStagePositions(
            this Card card,
            List<object> stagePositions)
        {
            card.Info[StagePositionsInfoMark] = stagePositions;
        }

        public static void SetStagePositions(
            this Card card,
            List<KrStagePositionInfo> stagePositions)
        {
            card.SetStagePositions(stagePositions.Cast<object>().ToList());
        }

        public static List<KrStagePositionInfo> GetStagePositions(
            this Card card)
        {
            if (card.Info.TryGetValue(StagePositionsInfoMark, out var stagePositions)
                && stagePositions is List<object> stagePositionsList)
            {
                return stagePositionsList
                    .Cast<Dictionary<string, object>>()
                    .Select(p => new KrStagePositionInfo(p))
                    .ToList();
            }

            return null;
        }

        public static bool TryGetStagePositions(
            this Card card,
            out List<KrStagePositionInfo> stagePositions)
        {
            var value = GetStagePositions(card);
            stagePositions = value;
            return value != null;
        }

        public static bool HasStagePositions(
            this Card card,
            bool atLeastOne)
        {
            return card.Info.TryGetValue(StagePositionsInfoMark, out var stagePositions)
                && stagePositions is List<object> stagePositionsList
                && (!atLeastOne
                    || stagePositionsList.Count > 0);
        }

        public static bool HasHiddenStages(
            this Card card)
        {
            return card.Info.TryGetValue(StagePositionsInfoMark, out var stagePositions)
                && stagePositions is List<object> stagePositionsList
                && stagePositionsList.Any(p => (p as Dictionary<string, object>)?.TryGet<bool?>(nameof(KrStagePositionInfo.Hidden)) == true);
        }

        public static void RemoveStagePositions(this Card card)
        {
            card.Info.Remove(StagePositionsInfoMark);
        }

        internal const string AsyncProcessCompletedSimultaniosly = nameof(AsyncProcessCompletedSimultaniosly);

        public static void SetAsyncProcessCompletedSimultaniosly(
            this IDictionary<string, object> info,
            bool flag = true)
        {
            info[AsyncProcessCompletedSimultaniosly] = BooleanBoxes.Box(flag);
        }

        public static bool GetAsyncProcessCompletedSimultaniosly(
            this IDictionary<string, object> info)
        {
            return info.TryGet<bool?>(AsyncProcessCompletedSimultaniosly) == true;
        }

        internal const string ProcessInfoAtEnd = nameof(ProcessInfoAtEnd);

        public static void SetProcessInfoAtEnd(
            this IDictionary<string, object> info,
            IDictionary<string, object> processInfo)
        {
            info[ProcessInfoAtEnd] = processInfo.ToDictionaryStorage();
        }

        public static IDictionary<string, object> GetProcessInfoAtEnd(
            this IDictionary<string, object> info)
        {
            return info.TryGet<IDictionary<string, object>>(ProcessInfoAtEnd);
        }

        #endregion

        #region Recalc

        private const string RecalcInfoMark = CardHelper.SystemKeyPrefix + "Recalc";

        private const string InfoAboutChangesInfoMark = CardHelper.SystemKeyPrefix + "InfoAboutChanges";

        private const string HasChangesInRoute = CardHelper.SystemKeyPrefix + "HasChangesInRoute";

        private const string RouteChanges = CardHelper.SystemKeyPrefix + "RouteChanges";

        public static bool GetRecalcFlag(
            this IDictionary<string, object> info) =>
            info.TryGetValue(RecalcInfoMark, out var flagObj)
            && flagObj is bool flag
            && flag;

        public static void SetRecalcFlag(
            this IDictionary<string, object> info) => info[RecalcInfoMark] = BooleanBoxes.True;

        public static bool GetRecalcFlag(
            this CardInfoStorageObject request) => request.Info.GetRecalcFlag();

        public static void SetRecalcFlag(
            this CardInfoStorageObject request) => request.Info.SetRecalcFlag();


        public static InfoAboutChanges? GetInfoAboutChanges(
            this IDictionary<string, object> info) =>
            info.TryGetValue(InfoAboutChangesInfoMark, out var iacObj) && iacObj is int iac
                ? (InfoAboutChanges?)iac
                : null;

        public static void SetInfoAboutChanges(
            this IDictionary<string, object> info,
            InfoAboutChanges infoAboutChanges) => info[InfoAboutChangesInfoMark] = (int)infoAboutChanges;

        public static InfoAboutChanges? GetInfoAboutChanges(
            this CardInfoStorageObject request) => request.Info.GetInfoAboutChanges();

        public static void SetInfoAboutChanges(
            this CardInfoStorageObject request,
            InfoAboutChanges infoAboutChanges) => request.Info.SetInfoAboutChanges(infoAboutChanges);


        public static bool? GetHasRecalcChanges(
            this CardInfoStorageObject obj) => obj.Info.TryGet<bool?>(HasChangesInRoute);

        public static void SetHasRecalcChanges(
            this CardInfoStorageObject response,
            bool hasChanges) => response.Info[HasChangesInRoute] = hasChanges;

        public static List<RouteDiff> GetRecalcChanges(
            this CardInfoStorageObject obj)
        {
            var info = obj.Info;
            if (info.TryGetValue(RouteChanges, out var diffsObj)
                && diffsObj is IEnumerable diffsEnum)
            {
                var diffs = new List<RouteDiff>();
                foreach (var diffObj in diffsEnum)
                {
                    if (diffObj is Dictionary<string, object> diffStorage)
                    {
                        diffs.Add(new RouteDiff(diffStorage));
                    }
                }

                return diffs;
            }

            return null;
        }

        public static void SetRecalcChanges(
            this CardInfoStorageObject obj,
            IEnumerable<RouteDiff> diffs) => obj.Info[RouteChanges] = diffs.Select(p => p.GetStorage()).ToList();

        /// <summary>
        /// Возвращает значение, показывающее, наличие скрытых пропущенных этапов.
        /// </summary>
        /// <param name="card">Карточка в которой выполняется проверка.</param>
        /// <returns>Значение true, если скрытые этапы содержатся, иначе - false.</returns>
        public static bool HasSkipStages(
            this Card card)
        {
            card.Sections.TryGetValue(KrConstants.KrStages.Virtual, out var krStagesVirtual);
            var krStagesVirtualRows = krStagesVirtual?.Rows;

            return card.Info.TryGetValue(StagePositionsInfoMark, out var stagePositions)
                && stagePositions is List<object> stagePositionsList
                && stagePositionsList.Any(p =>
                {
                    if (!(p is Dictionary<string, object> pTyped))
                    {
                        return false;
                    }

                    return pTyped.TryGet<bool?>(nameof(KrStagePositionInfo.Skip)) == true
                        && krStagesVirtualRows?.Any(i => i.RowID == pTyped.TryGet<Guid?>(nameof(KrStagePositionInfo.RowID))) == false;
                });
        }

        /// <summary>
        /// Ключ элемента в <see cref="Card"/>.<see cref="CardInfoStorageObject.Info"/> содержащий значение флага, показывающего, треюуется ли показать пропущенные этапы. Тип значения: <see cref="bool"/>.
        /// </summary>
        private const string DontHideSkippedStagesInfoMark = nameof(DontHideSkippedStagesInfoMark);

        public static void DontHideSkippedStages(
            this Dictionary<string, object> storage,
            bool value = true)
        {
            storage[DontHideSkippedStagesInfoMark] = BooleanBoxes.Box(value);
        }

        public static bool ConsiderSkippedStages(
            this CardInfoStorageObject request)
        {
            return request.Info.TryGet(DontHideSkippedStagesInfoMark, false);
        }

        #endregion

        #region ICardMetadata

        public static string GetDocumentStateName(this ICardMetadata metadata, KrState state)
        {
            if (state.IsDefault())
            {
                return state.TryGetDefaultName();
            }

            var intState = (int)state;

            var record = metadata
                .GetEnumerationsAsync().GetAwaiter().GetResult()[DefaultSchemeHelper.KrDocStateSectionID] // TODO async
                .Records
                .FirstOrDefault(p => p[ID].Equals(intState));
            if (!(record?[Name] is string name))
            {
                throw new InvalidOperationException(
                    $"State with ID = {state.ID} doesn't exist in KrDocState enumeration table.");
            }

            return name;
        }

        public static string GetStageStateName(this ICardMetadata metadata, KrStageState state)
        {
            if (state.IsDefault())
            {
                return state.TryGetDefaultName();
            }
            var intState = (int)state;
            var record = metadata
                .GetEnumerationsAsync().GetAwaiter().GetResult()[DefaultSchemeHelper.KrStageStateSectionID] // TODO async
                .Records
                .FirstOrDefault(p => p[ID].Equals(intState));

            if (!(record?[Name] is string name))
            {
                throw new InvalidOperationException(
                    $"State with ID = {state.ID} doesn't exist in KrStageState enumeration table.");
            }

            return name;
        }

        #endregion
    }
}