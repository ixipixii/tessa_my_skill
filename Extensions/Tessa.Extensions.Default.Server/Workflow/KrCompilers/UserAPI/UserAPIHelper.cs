using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tessa.BusinessCalendar;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public static class UserAPIHelper
    {

        public static Guid? GetCurrentTaskHistoryGroup(
            IKrScript krScipt)
        {
            if (krScipt.Stage != null
                && HandlerHelper.TryGetOverridenTaskHistoryGroup(krScipt.Stage, out var overridenTaskHistoryGroupID))
            {
                return overridenTaskHistoryGroupID;
            }

            if (krScipt.KrScope.Exists)
            {
                return krScipt.KrScope.GetCurrentHistoryGroup(krScipt.CardID);
            }

            throw new InvalidOperationException();
        }



        public static ListStorage<CardRow> CardRows(
            IKrScript script,
            string sectionName) => script.CardObject.Sections[sectionName].Rows;

        public static bool IsMainProcess(IKrScript script) => script.ProcessTypeName == KrConstants.KrProcessName;

        public static bool IsMainProcessStarted(IKrScript script)
        {
            if (IsMainProcess(script))
            {
                return true;
            }

            if (script.ContextualSatellite is null)
            {
                return false;
            }

            return script.Db.SetCommand(
                    script.DbScope.BuilderFactory
                        .Select().Top(1)
                        .V(1)
                        .From("WorkflowProcesses").NoLock()
                        .Where().C("ID").Equals().P("ID")
                        .And().C("TypeName").Equals().V(KrConstants.KrProcessName)
                        .Limit(1)
                        .Build(),
                    script.Db.Parameter("ID", script.ContextualSatellite.ID))
                .LogCommand()
                .Execute<bool>();
        }

        public static bool IsMainProcessInactive(
            IKrScript krScript,
            Card contextualSatellite)
        {
            if (krScript.IsMainProcess())
            {
                return false;
            }

            if (contextualSatellite != null)
            {
                return contextualSatellite
                    .GetStagesSection()
                    .Rows
                    .All(p => (p.TryGet<int?>(KrConstants.StateID) ?? KrStageState.Inactive.ID) == KrStageState.Inactive);
            }

            var hasAtLeastNonInactiveStage = krScript
                .Db
                .SetCommand(krScript.DbScope.BuilderFactory
                    .Select().Top(1)
                    .V(true)
                    .From(KrConstants.KrStages.Name, "s").NoLock()
                    .InnerJoin(KrConstants.KrApprovalCommonInfo.Name, "aci").NoLock().On().C("aci", KrConstants.ID).Equals().C("s", KrConstants.ID)
                    .Where().C("aci", KrConstants.KrProcessCommonInfo.MainCardID).Equals().P("ID")
                        .And().C("s", KrConstants.KrStages.StageStateID).NotEquals().V(KrStageState.Inactive.ID)
                    .Limit(1)
                    .Build(),
                    krScript.Db.Parameter("ID", krScript.CardID))
                .LogCommand()
                .Execute<bool>();
            return !hasAtLeastNonInactiveStage;
        }

        public static T Resolve<T>(IUnityContainer unityContainer, string name = null)
        {
            return unityContainer.Resolve<T>(name);
        }

        public static void ForEachStage(
           IKrScript script,
           Action<CardRow> rowAction,
           bool withNesteds)
        {
            IEnumerable<CardRow> rows = script.ProcessHolderSatellite.GetStagesSection().Rows;
            if (!withNesteds)
            {
                rows = rows.Where(p => p.Fields.TryGet<Guid?>(KrConstants.KrStages.NestedProcessID) == null);
            }

            foreach (var row in rows)
            {
                rowAction(row);
            }
        }

        public static void ForEachStageInMainProcess(
            IKrScript script,
            Action<CardRow> rowAction,
            bool withNesteds)
        {
            IEnumerable<CardRow> rows = script.ContextualSatellite.GetStagesSection().Rows;
            if (!withNesteds)
            {
                rows = rows.Where(p => p.Fields.TryGet<Guid?>(KrConstants.KrStages.NestedProcessID) == null);
            }

            foreach (var row in rows)
            {
                rowAction(row);
            }
        }

        public static void SetStageState(
            IKrScript script,
            CardRow stage,
            KrStageState stageState)
        {
            stage.Fields["StateID"] = (int)stageState;
            stage.Fields["StateName"] = script.CardMetadata.GetStageStateName(stageState);
        }

        public static Stage GetOrAddStage(IKrScript script, string name, StageTypeDescriptor descriptor, int pos = int.MaxValue, bool ignoreManualChanges = false) =>
            AddStageInternal(script, name, descriptor, pos, ignoreManualChanges, true);

        public static Stage AddStage(IKrScript script, string name, StageTypeDescriptor descriptor, int pos = int.MaxValue, bool ignoreManualChanges = false) =>
            AddStageInternal(script, name, descriptor, pos, ignoreManualChanges, false);

        public static bool RemoveStage(IKrScript script, string name, bool ignoreManualChanges = false)
        {
            if (script.StagesContainer is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RemoveStage)} can be invoked only in route calculating context.");
            }

            var cnt = script.StagesContainer.Stages
                .RemoveAll(stage =>
                    stage.TemplateID == script.TemplateID
                    && !stage.BasedOnTemplateStage
                    && stage.BasedOnTemplate
                    && stage.Name == name
                    && (!stage.RowChanged || ignoreManualChanges));
            return cnt != 0;
        }

        public static void SetSinglePerformer(
            Guid id,
            string name,
            Stage intoStage,
            bool ignoreManualChanges = false)
        {
            id = id != default
                ? id
                : throw new NullReferenceException(LocalizationManager.Format("$KrMessages_StageTemplateNullReferenceException", "Performer ID"));
            name = name
                ?? throw new NullReferenceException(LocalizationManager.Format("KrMessages_StageTemplateNullReferenceException", "Performer Name"));
            intoStage = intoStage
                ?? throw new NullReferenceException(LocalizationManager.Format("KrMessages_StageTemplateNullReferenceException", "Into Stage"));

            if (ignoreManualChanges || !intoStage.RowChanged)
            {
                intoStage.Performer = new Performer(id, name);
            }
        }

        public static void ResetSinglePerformer(
            Stage stage,
            bool ignoreManualChanges = false)
        {
            stage = stage
                ?? throw new NullReferenceException(LocalizationManager.Format("KrMessages_StageTemplateNullReferenceException", "stage"));

            if (ignoreManualChanges || !stage.RowChanged)
            {
                stage.Performer = null;
            }
        }

        public static Performer AddPerformer(
            IKrScript script,
            Guid id,
            string name,
            Stage intoStage,
            int pos = int.MaxValue,
            bool ignoreManualChanges = false)
        {
            id = id != default
                ? id
                : throw new NullReferenceException(LocalizationManager.Format("$KrMessages_StageTemplateNullReferenceException", "Performer ID"));
            name = name
                ?? throw new NullReferenceException(LocalizationManager.Format("KrMessages_StageTemplateNullReferenceException", "Performer Name"));
            intoStage = intoStage
                ?? throw new NullReferenceException(LocalizationManager.Format("KrMessages_StageTemplateNullReferenceException", "Into Stage"));

            pos = NormalizePos(pos, intoStage.Performers);

            // Если этап не изменен, то у него есть предок, в котором можно найти исполнителя
            // Если этап изменен, то он не заменяется. Можно просто взять сам этап и посмотреть там.
            var ancestor = !intoStage.RowChanged
                ? intoStage.Ancestor
                : intoStage;
            if (!ignoreManualChanges
                && ancestor?.RowChanged == true)
            {
                return null;
            }
            // В старом этапе может быть такая же роль.
            var oldPerformer = ancestor?.Performers
                ?.FirstOrDefault(p =>
                    p.PerformerID == id
                    && p.PerformerName == name
                    && !p.IsSql);

            var newPerformer = new MultiPerformer(oldPerformer?.RowID ?? Guid.NewGuid(), id, name, intoStage.RowID);
            var offset = Convert.ToInt32(oldPerformer != null && intoStage.Performers.Remove(oldPerformer));
            intoStage.Performers.Insert(pos - offset, newPerformer);
            return newPerformer;
        }

        public static void RemovePerformer(
            Guid[] ids,
            Stage stage,
            bool ignoreManualChanges = false)
        {
            if (ignoreManualChanges || !stage.RowChanged)
            {
                stage.Performers.RemoveAll(p => ids.Contains(p.RowID));
            }
        }

        public static void AddTaskHistoryRecord(
            IKrScript script,
            Guid? taskHistoryGroupID,
            Guid typeID,
            string typeName,
            string typeCaption,
            Guid optionID,
            string result,
            Guid? performerID,
            string performerName,
            int? cycle,
            int? timeZoneID,
            TimeSpan? timeZoneUtcOffset,
            Action<CardTaskHistoryItem> modifyAction)
        {
            var cardMetadata = script.CardMetadata;
            var card = script.CardObject;
            var contextualSatellite = script.ContextualSatellite;
            var cycleInternal = cycle ?? script.Cycle;
            var perfIDInternal = performerID ?? script.Session.User.ID;
            var perfNameInternal = performerName ?? script.Session.User.Name;
            var option = cardMetadata.GetEnumerationsAsync().GetAwaiter().GetResult().CompletionOptions[optionID]; // TODO async
            var utcNow = DateTime.UtcNow;

            if (!timeZoneID.HasValue || !timeZoneUtcOffset.HasValue)
            {
                var timeZonesCard = script.CardCache.Cards.GetAsync("TimeZones").GetAwaiter().GetResult(); // TODO async
                var defaultTimeZoneSection = timeZonesCard.Sections[TimeZonesHelper.DefaultTimeZoneSection];

                timeZoneID = TimeZonesHelper.DefaultZoneID;
                timeZoneUtcOffset = TimeSpan.FromMinutes(defaultTimeZoneSection.Fields.Get<int>("UtcOffsetMinutes"));
            }

            var item = new CardTaskHistoryItem
            {
                GroupRowID = taskHistoryGroupID,
                State = CardTaskHistoryState.Inserted,
                RowID = Guid.NewGuid(),
                TypeID = typeID,
                TypeName = typeName,
                TypeCaption = typeCaption,
                Created = utcNow,
                Planned = utcNow,
                InProgress = utcNow,
                Completed = utcNow,
                CompletedByID = perfIDInternal,
                CompletedByName = perfNameInternal,
                AuthorID = perfIDInternal,
                AuthorName = perfNameInternal,
                UserID = perfIDInternal,
                UserName = perfNameInternal,
                RoleID = perfIDInternal,
                RoleName = perfNameInternal,
                RoleTypeID = RoleHelper.PersonalRoleTypeID,
                OptionID = option.ID,
                OptionName = option.Name,
                OptionCaption = option.Caption,
                Result = result ?? string.Empty,
                TimeZoneID = timeZoneID,
                TimeZoneUtcOffsetMinutes = (int?)timeZoneUtcOffset.Value.TotalMinutes
            };

            modifyAction?.Invoke(item);
            card.TaskHistory.Add(item);
            contextualSatellite.AddToHistory(item.RowID, cycleInternal > 0 ? cycleInternal : 1);
        }

        public static CardTaskHistoryGroup ResolveTaskHistoryGroup(
            IKrScript script,
            Guid groupTypeID,
            Guid? parentGroupTypeID = null,
            bool newIteration = false) => script.ResolveTaskHistoryGroup(groupTypeID, parentGroupTypeID, newIteration);

        public static int GetCycle(IKrScript script)
        {
            if (script.ProcessTypeName == KrConstants.KrProcessName)
            {
                // Для основного процесса цикл лежит в его инфо.
                return script.WorkflowProcess.InfoStorage.TryGet<int?>(KrConstants.Keys.Cycle) ?? 0;
            }

            var serializer = script.StageSerializer;
            return ProcessInfoCacheHelper.Get(serializer, script.ContextualSatellite)?.TryGet<int?>(KrConstants.Keys.Cycle) ?? 0;
        }

        public static bool SetCycle(
            IKrScript script,
            int newValue)
        {
            if (newValue < 1)
            {
                return false;
            }

            if (script.ProcessTypeName == KrConstants.KrProcessName)
            {
                // Для основного процесса цикл лежит в его инфо.
                script.WorkflowProcess.InfoStorage[KrConstants.Keys.Cycle] = newValue;
                return true;
            }

            var serializer = script.StageSerializer;
            var mainProcessInfo = ProcessInfoCacheHelper.Get(serializer, script.ContextualSatellite);
            mainProcessInfo[KrConstants.Keys.Cycle] = newValue;
            return true;
        }

        public static bool HasKrComponents(
            IKrScript script,
            KrComponents[] components)
        {
            var allComponents = KrComponents.None;
            for (var i = 0; i < components.Length; i++)
            {
                allComponents |= components[i];
            }

            return HasKrComponents(script, allComponents);
        }

        public static bool HasKrComponents(
            IKrScript script,
            KrComponents components)
        {
            return (script.KrComponents & components) == components;
        }

        public static ISerializableObject GetPrimaryProcessInfo(
            IKrScript script,
            Guid? cardID)
        {
            var satellite = cardID.HasValue
                ? script.KrScope.GetKrSatellite(cardID.Value, script.ValidationResult)
                : script.ContextualSatellite;
            return ProcessInfoCacheHelper.Get(script.StageSerializer, satellite);
        }

        public static ISerializableObject GetSecondaryProcessInfo(
            IKrScript script,
            Guid secondaryProcessID,
            Guid? mainCardID)
        {
            Card satellite;
            if (secondaryProcessID == script.ProcessID)
            {
                satellite = script.ProcessHolderSatellite;
            }
            else
            {
                satellite = script.KrScope.GetSecondaryKrSatellite(secondaryProcessID);
                var satelliteCardID = satellite
                    .GetApprovalInfoSection()
                    .RawFields
                    .TryGet<Guid?>(KrConstants.KrProcessCommonInfo.MainCardID);
                if (satelliteCardID != (mainCardID ?? script.CardID))
                {
                    throw new InvalidOperationException("Secondary satellite has different main card id");
                }
            }

            return ProcessInfoCacheHelper.Get(script.StageSerializer, satellite);
        }

        public static Card GetNewCard(IKrScript script)
        {
            return ((IMainCardAccessStrategy)script.Stage.InfoStorage[KrConstants.Keys.NewCard]).GetCard(withoutTransaction: true);
        }

        public static IDictionary<string, object> GetProcessInfoForBranch(
            IKrScript script,
            Guid rowID)
        {
            var infos = script.StageInfoStorage.TryGet<IDictionary<string, object>>(KrConstants.Keys.ForkNestedProcessInfo);
            if (infos is null)
            {
                return null;
            }

            var key = rowID.ToString("D");
            var info = infos.TryGet<IDictionary<string, object>>(key);
            if (info != null)
            {
                return info;
            }

            info = new Dictionary<string, object>();
            infos[key] = info;
            return info;
        }

        private static Stage AddStageInternal(
            IKrScript script,
            string name,
            StageTypeDescriptor descriptor,
            int pos,
            bool ignoreManualChanges,
            bool returnOldStage)
        {
            if (script.StagesContainer is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(AddStage)} can be invoked only in route calculating context.");
            }

            name = name
                ?? throw new NullReferenceException(LocalizationManager.Format("KrMessages_StageTemplateNullReferenceException", "Stage Name"));
            var currentStages = script.CurrentStages;
            pos = NormalizePos(pos, currentStages);

            var oldStage = script.StagesContainer.InitialStages
                .FirstOrDefault(initialStage =>
                    initialStage.TemplateID == script.TemplateID
                    && !initialStage.BasedOnTemplateStage
                    && initialStage.BasedOnTemplate
                    && initialStage.Name == name);

            if (!ignoreManualChanges
                && (oldStage?.RowChanged == true
                    || oldStage?.OrderChanged == true))
            {
                if (returnOldStage)
                {
                    var copiedStage = new Stage(oldStage);
                    // Чтобы oldStage стал предком для copiedStage
                    copiedStage.Inherit(oldStage);
                    copiedStage.TemplateStageOrder = pos;
                    var index = script.Stages.IndexOf(oldStage);
                    if (index != -1)
                    {
                        script.StagesContainer.ReplaceStage(index, copiedStage);
                    }
                    else
                    {
                        script.StagesContainer.InsertStage(copiedStage);
                    }
                    return copiedStage;
                }
                return null;
            }
            for (var index = pos; index < currentStages.Count; index++)
            {
                currentStages[index].TemplateStageOrder++;
            }
            var newStage = new Stage(
                oldStage?.ID ?? Guid.NewGuid(),
                name,
                descriptor.ID,
                descriptor.Caption,
                script.StageGroupID,
                script.StageGroupOrder,
                script.TemplateID,
                script.TemplateName,
                script.Order,
                script.CanChangeOrder,
                script.Position,
                oldStage,
                script.IsStagesReadonly);
            newStage.TemplateStageOrder = pos;
            script.StagesContainer.InsertStage(newStage);
            return newStage;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NormalizePos(
            int pos,
            ICollection collection)
        {
            if (pos < 0)
            {
                pos = 0;
            }
            else if (collection.Count < pos)
            {
                pos = collection.Count;
            }

            return pos;
        }

    }
}