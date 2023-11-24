using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessButtonVisibilityEvaluator: IKrProcessButtonVisibilityEvaluator
    {
        #region fields

        private readonly IKrProcessCache processCache;

        private readonly IDbScope dbScope;

        private readonly ISession session;

        private readonly IKrCompilationCache compilationCache;

        private readonly IUnityContainer unityContainer;

        private readonly ICardMetadata cardMetadata;

        private readonly IKrScope scope;

        private readonly IKrTypesCache typesCache;

        private readonly ICardCache cardCache;

        private readonly IKrSqlExecutor sqlExecutor;

        private readonly IKrStageSerializer stageSerializer;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region constructor

        public KrProcessButtonVisibilityEvaluator(
            IKrProcessCache processCache,
            IDbScope dbScope,
            ISession session,
            IKrCompilationCache compilationCache,
            IUnityContainer unityContainer,
            ICardMetadata cardMetadata,
            IKrScope scope,
            IKrTypesCache typesCache,
            ICardCache cardCache,
            IKrSqlExecutor sqlExecutor,
            IKrStageSerializer stageSerializer)
        {
            this.processCache = processCache;
            this.dbScope = dbScope;
            this.session = session;
            this.compilationCache = compilationCache;
            this.unityContainer = unityContainer;
            this.cardMetadata = cardMetadata;
            this.scope = scope;
            this.typesCache = typesCache;
            this.cardCache = cardCache;
            this.sqlExecutor = sqlExecutor;
            this.stageSerializer = stageSerializer;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public async Task<IList<IKrProcessButton>> EvaluateGlobalButtonsAsync(
            IKrProcessButtonVisibilityEvaluatorContext context,
            CancellationToken cancellationToken = default)
        {
            var buttonIDs = await this.GetGlobalButtonsIDsAsync(cancellationToken);
            return this.EvaluateButtonsInternal(buttonIDs, context);
        }

        /// <inheritdoc />
        public async Task<IList<IKrProcessButton>> EvaluateLocalButtonsAsync(
            IKrProcessButtonVisibilityEvaluatorContext context,
            CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var typeID = context.DocTypeID ?? context.CardTypeID ?? Guid.Empty;
                var buttonIDs = await this.GetLocalButtonIDsAsync(typeID, context.State?.ID ?? -1, cancellationToken);
                return this.EvaluateButtonsInternal(buttonIDs, context);
            }
        }

        #endregion

        #region private

        private IList<IKrProcessButton> EvaluateButtonsInternal(
            List<Guid> buttonIDs,
            IKrProcessButtonVisibilityEvaluatorContext context)
        {
            using (this.dbScope.Create())
            {
                var set = new HashSet<Guid>(buttonIDs);
                var buttonsToFilter = this.processCache.GetButtons(set);

                var filteredButtons = new List<IKrProcessButton>(buttonsToFilter.Count);
                foreach (var button in buttonsToFilter)
                {
                    if (this.EvaluateVisibility(button, context))
                    {
                        filteredButtons.Add(button);
                    }

                    if (!context.ValidationResult.IsSuccessful())
                    {
                        break;
                    }
                }

                return filteredButtons;
            }
        }

        /// <summary>
        /// Загрузка глобальных кнопок для пользователя.
        /// </summary>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns></returns>
        private async Task<List<Guid>> GetGlobalButtonsIDsAsync(CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builder = this.dbScope.BuilderFactory
                    .Select().C("t", KrConstants.ID)
                    .From(KrConstants.KrSecondaryProcesses.Name, "t").NoLock()
                    .Where()
                    .C(KrConstants.KrSecondaryProcesses.IsGlobal).Equals().V(true)
                    .And()
                    .E(w => w
                        .NotExists(e => e
                            .Select().V(null)
                            .From(KrConstants.KrStageRoles.Name, "r").NoLock()
                            .Where().C("r", KrConstants.ID).Equals().C("t", KrConstants.ID))
                        .Or()
                        .Exists(e => e
                            .Select().V(null)
                            .From(KrConstants.KrStageRoles.Name, "r").NoLock()
                            .InnerJoin("RoleUsers", "ru").NoLock()
                            .On().C("ru", KrConstants.ID).Equals().C("r", KrConstants.RoleID)
                            .Where().C("r", KrConstants.ID).Equals().C("t", KrConstants.ID)
                            .And().C("ru", "UserID").Equals().P("UserID")));

                return await db
                    .SetCommand(builder.Build(), db.Parameter("UserID", this.session.User.ID))
                    .LogCommand()
                    .ExecuteListAsync<Guid>(cancellationToken);
            }
        }

        private async Task<List<Guid>> GetLocalButtonIDsAsync(
            Guid typeID,
            int stateID,
            CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builder = this.dbScope.BuilderFactory
                    .Select().C("t", KrConstants.ID)
                    .From(KrConstants.KrSecondaryProcesses.Name, "t").NoLock()
                    .LeftJoin(KrConstants.KrStageTypes.Name, "tt").NoLock()
                        .On().C("tt", "ID").Equals().C("t", "ID")
                    .LeftJoin(KrConstants.KrStageDocStates.Name, "tds").NoLock()
                        .On().C("tds", "ID").Equals().C("t", "ID")
                    .Where()
                    .C(KrConstants.KrSecondaryProcesses.IsGlobal).Equals().V(false)
                    .And()
                    .E(w => w
                        .C("tt", "TypeID").IsNull()
                        .Or()
                        .C("tt", "TypeID").Equals().P("TypeID"))
                    .And()
                    .E(w => w
                        .C("tds", "StateID").IsNull()
                        .Or()
                        .C("tds", "StateID").Equals().P("StateID"))
                    .And()
                    .E(w => w
                        .NotExists(e => e
                            .Select().V(null)
                            .From(KrConstants.KrStageRoles.Name, "r").NoLock()
                            .Where().C("r", KrConstants.ID).Equals().C("t", KrConstants.ID))
                        .Or()
                        .Exists(e => e
                            .Select().V(null)
                            .From(KrConstants.KrStageRoles.Name, "r").NoLock()
                            .InnerJoin("RoleUsers", "ru").NoLock()
                            .On().C("ru", KrConstants.ID).Equals().C("r", KrConstants.RoleID)
                            .Where().C("r", KrConstants.ID).Equals().C("t", KrConstants.ID)
                            .And().C("ru", "UserID").Equals().P("UserID")));

                return await db
                    .SetCommand(
                        builder.Build(),
                        db.Parameter("UserID", this.session.User.ID),
                        db.Parameter("TypeID", typeID),
                        db.Parameter("StateID", stateID))
                    .LogCommand()
                    .ExecuteListAsync<Guid>(cancellationToken);
            }
        }

        private bool EvaluateVisibility(
            IKrProcessButton button,
            IKrProcessButtonVisibilityEvaluatorContext context)
        {
            var source = button.VisibilitySourceCondition;
            var sqlText = button.VisibilitySqlCondition;
            if (string.IsNullOrWhiteSpace(source)
                && string.IsNullOrWhiteSpace(sqlText))
            {
                // Если в карточке не задан исходный текст,
                // то по дефолту там true и генерить контексты для выполнения излишне
                return true;
            }

            var compilationResult = this.compilationCache.Get();
            if (compilationResult.Result.Assembly == null)
            {
                logger.LogResult(compilationResult.ValidationResult);
                context.ValidationResult.Add(compilationResult.ToMissingAssemblyResult());
                return false;
            }

            var instance = this.CreateInstance(button, compilationResult, context);
            bool scriptResult;
            try
            {
                scriptResult = instance.RunVisibility();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.ButtonVisibilityError(button, e.Message);
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(text, source)
                    .ErrorException(e)
                    .End();
                return false;
            }
            if (!scriptResult)
            {
                return false;
            }

            try
            {
                var ctx = new KrSqlExecutorContext(
                    sqlText,
                    context.ValidationResult,
                    (c, txt, args) => KrErrorHelper.ButtonSqlVisibilityError(button, txt, args),
                    button,
                    context.CardID,
                    context.CardTypeID,
                    context.DocTypeID,
                    context.State);

                return this.sqlExecutor.ExecuteCondition(ctx);
            }
            catch (QueryExecutionException qee)
            {
                var validator = ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(qee.ErrorMessageText, qee.SourceText);
                if (qee.InnerException != null)
                {
                    validator.ErrorException(qee.InnerException);
                }
                validator.End();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.ButtonSqlVisibilityError(button, e.Message);
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(text, sqlText)
                    .ErrorException(e)
                    .End();
            }
            return false;
        }

        private IKrScript CreateInstance(
            IKrProcessButton button,
            IKrCompilationResult compilationResult,
            IKrProcessButtonVisibilityEvaluatorContext context)
        {
            var instance = compilationResult.CreateInstance(
                SourceIdentifiers.KrVisibilityClass,
                SourceIdentifiers.SecondaryProcessAlias,
                button.ID);

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

            instance.SecondaryProcess = button;
            instance.CardContext = context.CardContext;
            instance.ValidationResult = context.ValidationResult;
            instance.Session = this.session;
            instance.DbScope = this.dbScope;
            instance.UnityContainer = this.unityContainer;
            instance.CardMetadata = this.cardMetadata;
            instance.KrScope = this.scope;
            instance.CardCache = this.cardCache;
            instance.KrTypesCache = this.typesCache;
            instance.StageSerializer = this.stageSerializer;

            return instance;
        }

        #endregion
    }
}