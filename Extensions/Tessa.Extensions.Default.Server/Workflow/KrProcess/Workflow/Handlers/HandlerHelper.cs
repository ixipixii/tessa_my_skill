using System;
using System.Collections.Generic;
using NLog;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public static class HandlerHelper
    {
        private const string OverridenTaskHistoryGroup = CardHelper.SystemKeyPrefix + nameof(OverridenTaskHistoryGroup);

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static bool TryGetOverridenTaskHistoryGroup(Stage stage, out Guid? taskHistoryGroupID)
        {
            taskHistoryGroupID = null;
            if (stage != null
                && stage.InfoStorage.TryGetValue(OverridenTaskHistoryGroup, out var idObj))
            {
                if (idObj is Guid id)
                {
                    taskHistoryGroupID = id;
                }
                return true;
            }
            return false;
        }

        public static void RemoveTaskHistoryGroupOverride(
            Stage stage)
        {
            stage.InfoStorage.Remove(OverridenTaskHistoryGroup);
        }

        public static Guid? GetTaskHistoryGroup(
            IStageTypeHandlerContext context,
            IKrScope scope)
        {
            if (context.MainCardID == null)
            {
                throw new ArgumentNullException(nameof(context.MainCardID));
            }

            var stage = context.Stage;
            if (TryGetOverridenTaskHistoryGroup(stage, out var overridenID))
            {
                return overridenID;
            }

            var taskHistoryGroupID =
                stage.SettingsStorage.TryGet<Guid?>(
                    KrConstants.KrHistoryManagementStageSettingsVirtual.TaskHistoryGroupTypeID);
            var parentTaskHistoryGroupID =
                stage.SettingsStorage.TryGet<Guid?>(
                    KrConstants.KrHistoryManagementStageSettingsVirtual.ParentTaskHistoryGroupTypeID);
            var newIteration =
                stage.SettingsStorage.TryGet<bool?>(
                    KrConstants.KrHistoryManagementStageSettingsVirtual.NewIteration);
            if (taskHistoryGroupID.HasValue)
            {
                var newGroup = context.TaskHistoryResolver.ResolveTaskHistoryGroup(
                    taskHistoryGroupID.Value,
                    parentTaskHistoryGroupID,
                    newIteration == true);
                var newRowID = newGroup.RowID;
                stage.InfoStorage[OverridenTaskHistoryGroup] = newRowID;
                return newRowID;
            }

            return scope.GetCurrentHistoryGroup(context.MainCardID.Value, context.ValidationResult);
        }

        // TODO: Убрать этот костыльный метод. Данный метод в дальнейшем следует
        // заменить в местах использование на простое присовение значения
        // свойству Result для CardTask. Но на текущий момент сохранение задания
        // происходит до начала этапа и значение не попадает в базу данных.
        public static void SetTaskResult(IStageTypeHandlerContext context, CardTask task, string value)
        {
            var storeContext = (ICardStoreExtensionContext)context.CardExtensionContext;
            var scope = storeContext.DbScope;
            using (scope.Create())
            {
                scope.Db
                    .SetCommand(
                        scope.BuilderFactory
                            .Update("TaskHistory").C("Result").Assign().P("Result")
                            .Where().C("RowID").Equals().P("RowID")
                            .Build(),
                        scope.Db.Parameter("RowID", task.RowID),
                        scope.Db.Parameter("Result", value))
                    .ExecuteNonQuery();
            }

            task.Result = value;
        }


        public static Author GetStageAuthor(
            IStageTypeHandlerContext context,
            IRoleRepository roleRepository,
            ISession session)
        {
            var initiator = context.WorkflowProcess.Author;
            var overridenAuthor = context.Stage.Author;

            if (overridenAuthor != null)
            {
                var authorID = overridenAuthor.AuthorID;

                var role = roleRepository.GetRoleAsync(authorID).GetAwaiter().GetResult(); // TODO async
                if (role == null)
                {
                    context.ValidationResult.AddError("$KrProcess_ErrorMessage_AuthorRoleIsntFound");
                    return null;
                }

                switch (role.RoleType)
                {
                    case RoleType.Personal:
                        return overridenAuthor;

                    case RoleType.Context:
                        Guid? mainCardID = context.MainCardID;
                        if (!mainCardID.HasValue)
                        {
                            context.ValidationResult.AddError("$KrProcess_ErrorMessage_ContextRoleRequiresCard");
                            return null;
                        }

                        var contextRole = roleRepository.GetContextRoleAsync(authorID).GetAwaiter().GetResult(); // TODO async

                        var users = roleRepository.GetCardContextUsersAsync(contextRole, mainCardID.Value).GetAwaiter().GetResult(); // TODO async
                        if (users.Count > 0)
                        {
                            return new Author(users[0].UserID, users[0].UserName);
                        }
                        context.ValidationResult.AddError("$KrProcess_ErrorMessage_ContextRoleIsEmpty");
                        return null;

                    default:
                        context.ValidationResult.AddError("$KrProcess_ErrorMessage_OnlyPersonalAndContextRoles");
                        return null;
                }

            }
            if(initiator != null)
            {
                return initiator;
            }
            return new Author(session.User.ID, session.User.Name);
        }

        public static (Guid?, string) GetTaskKind(IStageTypeHandlerContext context)
        {
            var stage = context.Stage;
            var kindID = stage.SettingsStorage.TryGet<Guid?>(KrConstants.KrTaskKindSettingsVirtual.KindID);
            var kindCaption = stage.SettingsStorage.TryGet<string>(KrConstants.KrTaskKindSettingsVirtual.KindCaption);
            return (kindID, kindCaption);
        }

        public static void SetTaskKind(
            CardTask task,
            Guid? kindID,
            string kindCaption,
            IStageTypeHandlerContext context)
        {
            if (kindID != null
                && kindCaption != null)
            {
                task.Info[CardHelper.TaskKindIDKey] = kindID;
                task.Info[CardHelper.TaskKindCaptionKey] = kindCaption;

                if (task.Card.Sections.TryGetValue(KrConstants.TaskCommonInfo.Name, out var tci))
                {
                    tci.Fields[KrConstants.TaskCommonInfo.KindID] = kindID;
                    tci.Fields[KrConstants.TaskCommonInfo.KindCaption] = kindCaption;
                }
                else
                {
                    context.ValidationResult.AddError(
                        nameof(HandlerHelper),
                        "$KrProcess_ErrorMessage_MissingTaskCommonInfoKind",
                        context.Stage.Name);
                }
            }
        }

        public static IKrScript CreateScriptInstance(
            IKrCompilationCache compilationCache,
            Guid id,
            IValidationResultBuilder validationResult)
        {
            var compilationResult = compilationCache.Get();
            if (compilationResult.Result.Assembly == null)
            {
                logger.LogResult(compilationResult.ValidationResult);
                validationResult.Add(compilationResult.ToMissingAssemblyResult());
                throw new ProcessRunnerInterruptedException();
            }

            try
            {
                return compilationResult.CreateInstance(
                    SourceIdentifiers.KrRuntimeClass,
                    SourceIdentifiers.StageAlias,
                    id);
            }
            catch (KeyNotFoundException)
            {
                validationResult.AddError(
                    nameof(HandlerHelper),
                    string.Format(LocalizationManager.GetString("KrProcess_ClassMissed"), $"{SourceIdentifiers.KrRuntimeClass}_{id:N}"));
                throw new ProcessRunnerInterruptedException();
            }
        }

        public static void InitScriptContext(
            IUnityContainer unityContainer,
            IKrScript instance,
            IStageTypeHandlerContext context)
        {
            var currentStage = context.Stage;
            var processCache = unityContainer.Resolve<IKrProcessCache>();

            instance.MainCardAccessStrategy = context.MainCardAccessStrategy;
            instance.CardID = context.MainCardID ?? Guid.Empty;
            instance.CardTypeID = context.MainCardTypeID ?? Guid.Empty;
            instance.CardTypeName = context.MainCardTypeName;
            instance.CardTypeCaption = context.MainCardTypeCaption;
            instance.DocTypeID = context.MainCardDocTypeID ?? Guid.Empty;
            if (context.KrComponents.HasValue)
            {
                instance.KrComponents = context.KrComponents.Value;
            }

            instance.WorkflowProcessInfo = context.ProcessInfo;
            instance.ProcessID = context.ProcessInfo?.ProcessID;
            instance.ProcessTypeName = context.ProcessInfo?.ProcessTypeName;
            instance.InitiationCause = context.InitiationCause;
            instance.ContextualSatellite = context.ContextualSatellite;
            instance.ProcessHolderSatellite = context.ProcessHolderSatellite;
            instance.SecondaryProcess = context.SecondaryProcess;
            instance.CardContext = context.CardExtensionContext;
            instance.ValidationResult = context.ValidationResult;
            instance.TaskHistoryResolver = context.TaskHistoryResolver;
            instance.Session = unityContainer.Resolve<ISession>();
            instance.DbScope = unityContainer.Resolve<IDbScope>();
            instance.UnityContainer = unityContainer;
            instance.CardMetadata = unityContainer.Resolve<ICardMetadata>();
            instance.KrScope = unityContainer.Resolve<IKrScope>();
            instance.CardCache = unityContainer.Resolve<ICardCache>();
            instance.KrTypesCache = unityContainer.Resolve<IKrTypesCache>();
            instance.StageSerializer = unityContainer.Resolve<IKrStageSerializer>();
            if (currentStage.TemplateID == null
                || !processCache.GetAllStageTemplates().TryGetValue(currentStage.TemplateID.Value, out var stageTemplate))
            {
                return;
            }

            instance.StageGroupID = currentStage.StageGroupID;
            instance.StageGroupName = currentStage.StageGroupName;
            instance.StageGroupOrder = currentStage.StageGroupOrder;
            instance.TemplateID = currentStage.TemplateID ?? Guid.Empty;
            instance.TemplateName = currentStage.TemplateName;
            instance.Order = stageTemplate?.Order ?? -1;
            instance.Position = stageTemplate?.Position ?? GroupPosition.Unspecified;
            instance.CanChangeOrder = stageTemplate?.CanChangeOrder ?? true;
            instance.IsStagesReadonly = stageTemplate?.IsStagesReadonly ?? true;

            // На данном этапе нет контейнера, способного пересчитывать положения этапов.
            instance.StagesContainer = null;
            instance.WorkflowProcess = context.WorkflowProcess;
            instance.Stage = currentStage;

            // Необходимо сбросить информацию о переключении контекста
            instance.DifferentContextCardID = null;
            instance.DifferentContextWholeCurrentGroup = false;
            instance.DifferentContextProcessInfo = null;
            instance.DifferentContextSetupScriptType = null;
        }

        public static void ClearCompletedTasks(Stage stage)
        {
            stage.InfoStorage.Remove(KrConstants.Keys.Tasks);
        }

        public static void AppendToCompletedTasksWithPreparing(
            Stage stage,
            CardTask task)
        {
            var taskStorage = StorageHelper.Clone(task.GetStorage());
            var taskCopy = new CardTask(taskStorage);
            taskCopy.RemoveChanges();

            if (!stage.WriteTaskFullInformation)
            {
                taskStorage.Remove(nameof(CardTask.SectionRows));
                taskStorage.Remove(nameof(CardTask.Card));
                taskStorage.Remove(CardInfoStorageObject.InfoKey);
            }

            AppendToCompletedTasks(stage, taskCopy);
        }

        public static void AppendToCompletedTasks(
            Stage stage,
            CardTask task)
        {
            var stageInfoStorage = stage.InfoStorage;
            var list = stageInfoStorage.TryGet<List<object>>(KrConstants.Keys.Tasks);
            if (list == null)
            {
                list = new List<object>();
                stageInfoStorage[KrConstants.Keys.Tasks] = list;
            }

            list.Add(task.GetStorage());
        }

    }
}