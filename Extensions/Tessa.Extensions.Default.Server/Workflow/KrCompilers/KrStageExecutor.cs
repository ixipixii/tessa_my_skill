using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrStageExecutor : KrExecutorBase
    {
        #region fields

        private readonly IObjectModelMapper objectModelMapper;
        private readonly IKrScope krScope;
        private readonly IKrStageSerializer krSerializer;
        private readonly ISession session;
        private readonly IUnityContainer unityContainer;
        private readonly IKrProcessCache processCache;
        private readonly ICardMetadata cardMetadata;

        private StagesContainer stagesContainer;
        private Guid stageGroupID;

        #endregion

        #region constructor

        public KrStageExecutor(
            IKrSqlExecutor sqlExecutor,
            IDbScope dbScope,
            ICardCache cardCache,
            IKrTypesCache typesCache,
            IKrStageSerializer stageSerializer,
            IObjectModelMapper objectModelMapper,
            IKrScope krScope,
            IKrStageSerializer krSerializer,
            ISession session,
            IUnityContainer unityContainer,
            IKrProcessCache processCache,
            ICardMetadata cardMetadata) : base(sqlExecutor, dbScope, cardCache, typesCache, stageSerializer)
        {
            this.objectModelMapper = objectModelMapper;
            this.krScope = krScope;
            this.krSerializer = krSerializer;
            this.session = session;
            this.unityContainer = unityContainer;
            this.processCache = processCache;
            this.cardMetadata = cardMetadata;

            this.stagesContainer = null;
            this.SharedValidationResult = null;
            this.ExecutionUnits = null;
            this.ConfirmedIDs = null;
            this.Status = KrExecutionStatus.None;
            this.InterruptedStageTemplateID = null;
        }

        #endregion

        #region public

        public override IKrExecutionResult Execute(IKrExecutionContext context)
        {
            var stageTemplates = this.processCache.GetStageTemplates(context.ExecutionUnitIDs);

            if (context.GroupID == null)
            {
                return new KrExecutionResult(
                    ValidationResult.Empty,
                    EmptyHolder<Guid>.Collection,
                    KrExecutionStatus.Complete);
            }

            this.stageGroupID = context.GroupID.Value;
            this.CheckStageGroupID(stageTemplates);

            this.SharedValidationResult = new ValidationResultBuilder();
            this.stagesContainer = new StagesContainer(this.objectModelMapper, context.WorkflowProcess, this.stageGroupID);
            this.ConfirmedIDs = new List<Guid>();
            this.Status = KrExecutionStatus.InProgress;
            this.InterruptedStageTemplateID = null;

            this.ExecutionUnits = new List<IKrExecutionUnit>(context.ExecutionUnitIDs.Count);
            if (!this.processCache.GetAllStageGroups().TryGetValue(this.stageGroupID, out var stageGroup))
            {
                return new KrExecutionResult(
                    ValidationResult.FromText(this, $"Stage group with ID = {this.stageGroupID} not found", ValidationResultType.Error), 
                    EmptyHolder<Guid>.Collection,
                    KrExecutionStatus.Complete);
            }
            
            foreach (var stageTemplate in stageTemplates)
            {
                var ct = context.CompilationResult.CreateInstance(
                    SourceIdentifiers.KrDesignTimeClass,
                    SourceIdentifiers.TemplateAlias,
                    stageTemplate.ID);
                this.ExecutionUnits.Add(this.CreateExecutionUnit(context, stageTemplate.ID, stageTemplate, stageGroup, ct));
            }

            this.RunForAll(this.PrepareStage);
            this.RunForAll(RunBefore);
            if (this.Status == KrExecutionStatus.InProgress)
            {
                using (this.DbScope.Create())
                {
                    // Формируется список подтвержденных шаблонов
                    this.RunForAll(p => this.RunConditions(p, context));
                    // Загружаются карточки подтвержденных шаблонов и сливаются с шаблонами из карточки (только если статус == в процессе)
                    this.UpdateStagesWithConfirmedTemplates();
                }
            }
            this.RunForAll(RunAfter);

            this.stagesContainer.RestoreFlags();

            if (this.Status == KrExecutionStatus.InProgress)
            {
                this.Status = KrExecutionStatus.Complete;
            }

            return new KrExecutionResult(
                this.SharedValidationResult.Build(),
                this.ConfirmedIDs,
                this.Status,
                this.InterruptedStageTemplateID);
        }

        #endregion

        #region private

        private void CheckStageGroupID(IReadOnlyList<IKrStageTemplate> templates)
        {
            foreach (var template in templates)
            {
                if (template.StageGroupID != this.stageGroupID)
                {
                    throw new InvalidOperationException(
                        $"{this.GetType().Name} doesn't support templates from different groups {Environment.NewLine}" +
                        $"{templates[0].Name} refers to {this.stageGroupID} but {template.Name} refers to {template.StageGroupID}");
                }
            }
        }

        private IKrExecutionUnit CreateExecutionUnit(
            IKrExecutionContext context,
            Guid id,
            IKrStageTemplate stageTemplate,
            IKrStageGroup stageGroup,
            IKrScript instance)
        {
            instance.StageGroupID = stageGroup.ID;
            instance.StageGroupName = stageGroup.Name;
            instance.StageGroupOrder = stageGroup.Order;

            instance.TemplateID = id;
            instance.TemplateName = stageTemplate.Name;
            instance.Order = stageTemplate.Order;
            instance.Position = stageTemplate.Position;
            instance.CanChangeOrder = stageTemplate.CanChangeOrder;
            instance.IsStagesReadonly = stageTemplate.IsStagesReadonly;

            instance.WorkflowProcess = context.WorkflowProcess;

            instance.MainCardAccessStrategy = context.MainCardAccessStrategy;
            instance.CardID = context.CardID ?? Guid.Empty;
            instance.CardTypeID = context.CardTypeID ?? Guid.Empty;
            instance.CardTypeName = context.CardTypeName;
            instance.CardTypeCaption = context.CardTypeCaption;
            instance.DocTypeID = context.DocTypeID ?? Guid.Empty;
            instance.CardContext = context.CardContext;
            if (context.KrComponents.HasValue)
            {
                instance.KrComponents = context.KrComponents.Value;
            }

            instance.Session = this.session;
            instance.DbScope = this.DbScope;
            instance.UnityContainer = this.unityContainer;
            instance.CardMetadata = this.cardMetadata;
            instance.KrScope = this.krScope;
            instance.ValidationResult = this.SharedValidationResult;
            instance.CardCache = this.CardCache;
            instance.KrTypesCache = this.KrTypesCache;
            instance.StageSerializer = this.StageSerializer;

            return new KrExecutionUnit(stageTemplate, instance);
        }
        
        private void PrepareStage(IKrExecutionUnit unit)
        {
            unit.Instance.StagesContainer = this.stagesContainer;
            unit.Instance.Seal();
        }

        private void UpdateStagesWithConfirmedTemplates()
        {
            if (this.Status != KrExecutionStatus.InProgress)
            {
                return;
            }

            var idSet = new HashSet<Guid>(this.ConfirmedIDs);
            var templates = this.processCache.GetStageTemplates(idSet);

            var stages = new Dictionary<Guid, IReadOnlyList<IKrRuntimeStage>>();
            foreach (var id in idSet)
            {
                stages[id] = this.processCache.GetRuntimeStagesForTemplate(id);
            }
            
            this.stagesContainer.MergeWith(templates, stages);
            this.stagesContainer.DeleteUnconfirmedStages();

            var currentGroupStages =
                this.stagesContainer.Stages.Where(p => KrCompilersHelper.ReferToGroup(this.stageGroupID, p));
            foreach (var stage in currentGroupStages)
            {
                var storage = stage.SettingsStorage;
                foreach (var referenceToStages in this.krSerializer.ReferencesToStages)
                {
                    if (storage.TryGetValue(referenceToStages.SectionName, out var rowsStorages)
                        && rowsStorages is IList<object> rows)
                    {
                        foreach (var row in rows)
                        {
                            if (row is IDictionary<string, object> rowStorage)
                            {
                                rowStorage[referenceToStages.RowIDFieldName] = stage.RowID;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
