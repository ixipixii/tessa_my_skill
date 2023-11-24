using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrGroupExecutor : KrExecutorBase
    {
        #region fields

        private readonly IKrProcessCache processCache;
        private readonly Func<IKrExecutor> getExecutor;
        private readonly ISession session;
        private readonly IUnityContainer unityContainer;
        private readonly IDbScope dbScope;
        private readonly ICardMetadata cardMetadata;
        private readonly IKrScope krScope;

        #endregion

        #region constructor

        public KrGroupExecutor(
            IKrSqlExecutor sqlExecutor,
            IDbScope dbScope,
            ICardCache cardCache,
            IKrTypesCache typesCache,
            IKrStageSerializer stageSerializer,
            IKrProcessCache processCache,
            [Dependency(KrExecutorNames.StageExecutor)] Func<IKrExecutor> getExecutor,
            ISession session,
            IUnityContainer unityContainer,
            IDbScope dbScope1,
            ICardMetadata cardMetadata,
            IKrScope krScope) : base(sqlExecutor, dbScope, cardCache, typesCache, stageSerializer)
        {
            this.processCache = processCache;
            this.getExecutor = getExecutor;
            this.session = session;
            this.unityContainer = unityContainer;
            this.dbScope = dbScope1;
            this.cardMetadata = cardMetadata;
            this.krScope = krScope;
        }

        #endregion

        #region public

        /// <inheritdoc />
        public override IKrExecutionResult Execute(
            IKrExecutionContext context)
        {
            this.SharedValidationResult = new ValidationResultBuilder();
            this.ConfirmedIDs = new List<Guid>();
            this.Status = KrExecutionStatus.InProgress;
            this.InterruptedStageTemplateID = null;
            this.ExecutionUnits = this.CreateExecutionUnitList(context);

            var results = new List<IKrExecutionResult>();
            this.RunForAll(PrepareUnit);
            this.RunForAll(RunBefore);
            if (this.Status == KrExecutionStatus.InProgress)
            {
                using (this.DbScope.Create())
                {
                    // Формируется список подтвержденных групп
                    this.RunForAll(p => this.RunConditions(p, context));
                    // Запуск пересчета внутри подтвержденных групп
                    results.AddRange(this.ExecuteStages(context));
                    // Удаление этапов неподтверженных групп
                    this.DeleteUnconfirmedGroups(context);
                }
            }
            this.RunForAll(RunAfter);

            if (this.Status == KrExecutionStatus.InProgress)
            {
                this.Status = KrExecutionStatus.Complete;
            }

            foreach (var res in results)
            {
                this.SharedValidationResult.Add(res.Result);
            }

            return new KrExecutionResult(
                this.SharedValidationResult.Build(),
                this.ConfirmedIDs,
                this.Status,
                this.InterruptedStageTemplateID);
        }

        #endregion

        #region private 

        /// <summary>
        /// Создать список единиц выполнения для шаблонов.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IList<IKrExecutionUnit> CreateExecutionUnitList(
            IKrExecutionContext context)
        {
            var stageGroups = this.processCache.GetStageGroups(context.ExecutionUnitIDs);
            var executionUnits = new List<IKrExecutionUnit>(stageGroups.Count);
            
            foreach (var stageGroup in stageGroups)
            {
                var ct = context.CompilationResult.CreateInstance(
                    SourceIdentifiers.KrDesignTimeClass, 
                    SourceIdentifiers.GroupAlias,
                    stageGroup.ID);
                executionUnits.Add(this.CreateExecutionUnit(context, stageGroup.ID, stageGroup, ct));
            }
            return executionUnits
                .OrderBy(p => p.StageGroupInfo.Order)
                .ThenBy(p => p.ID)
                .ToList();
        }

        /// <summary>
        /// Создание ExecutionUnit'a и наполнение его необходимыми объектами,
        /// которые будут доступны в пользовательском коде
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="stageGroup"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private IKrExecutionUnit CreateExecutionUnit(
            IKrExecutionContext context,
            Guid id,
            IKrStageGroup stageGroup,
            IKrScript instance)
        {
            instance.StageGroupID = stageGroup.ID;
            instance.StageGroupName = stageGroup.Name;
            instance.StageGroupOrder = stageGroup.Order;

            // Шаблона сейчас нет
            instance.TemplateID = Guid.Empty;
            instance.TemplateName = string.Empty;
            instance.Order = -1;
            instance.Position = GroupPosition.Unspecified;
            instance.CanChangeOrder = false;
            instance.IsStagesReadonly = false;

            // На данном этапе нет контейнера, способного пересчитывать положения этапов.
            instance.StagesContainer = null;
            instance.WorkflowProcess = context.WorkflowProcess;

            instance.MainCardAccessStrategy = context.MainCardAccessStrategy;
            instance.CardID = context.CardID ?? Guid.Empty;
            instance.CardTypeID = context.CardTypeID ?? Guid.Empty;
            instance.CardTypeName = context.CardTypeName;
            instance.CardTypeCaption = context.CardTypeCaption;
            instance.DocTypeID = context.DocTypeID ?? Guid.Empty;
            if (context.KrComponents.HasValue)
            {
                instance.KrComponents = context.KrComponents.Value;
            }

            instance.CardContext = context.CardContext;

            instance.Session = this.session;
            instance.DbScope = this.dbScope;
            instance.UnityContainer = this.unityContainer;
            instance.CardMetadata = this.cardMetadata;
            instance.ValidationResult = this.SharedValidationResult;
            instance.KrScope = this.krScope;
            instance.CardCache = this.CardCache;
            instance.KrTypesCache = this.KrTypesCache;
            instance.StageSerializer = this.StageSerializer;

            return new KrExecutionUnit(stageGroup, instance);
        }
        
        private static void PrepareUnit(IKrExecutionUnit unit)
        {
            unit.Instance.Seal();
        }

        private List<IKrExecutionResult> ExecuteStages(
            IKrExecutionContext context)
        {
            // Пусть каждой группе этапов будет хотя бы 5 шаблонов
            var results = new List<IKrExecutionResult>(5 * context.ExecutionUnitIDs.Count);
            foreach (var groupID in this.ConfirmedIDs)
            {
                var templateIDs = KrCompilersSqlHelper.GetFilteredStageTemplates(
                        this.DbScope, 
                        context.TypeID ?? Guid.Empty, 
                        this.session.User.ID,
                        groupID,
                        context.SecondaryProcess?.ID);

                var ctx = context.Copy(groupID, templateIDs);
                results.Add(this.getExecutor().Execute(ctx));
            }

            return results;
        }

        private void DeleteUnconfirmedGroups(
            IKrExecutionContext context)
        {
            // Этапы группы остаются если они подтверждены или вообще не относятся к пересчитываемым группам.
            context.WorkflowProcess.Stages = context.WorkflowProcess.Stages
                .Where(p => this.ConfirmedIDs.Contains(p.StageGroupID) || context.ExecutionUnitIDs.All(q => q != p.StageGroupID))
                .ToSealableObjectList();
        }

        #endregion
    }
}