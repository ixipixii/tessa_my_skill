using System;
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
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrSecondaryProcessExecutionEvaluator: IKrSecondaryProcessExecutionEvaluator
    {
        #region fields

        private readonly IKrCompilationCache compilationCache;

        private readonly IKrSqlExecutor sqlExecutor;

        private readonly ISession session;

        private readonly IDbScope dbScope;

        private readonly ICardMetadata cardMetadata;

        private readonly IUnityContainer unityContainer;

        private readonly IKrScope scope;

        private readonly ICardCache cardCache;

        private readonly IKrTypesCache krTypesCache;

        private readonly IRoleRepository roleRepository;

        private readonly ICardContextRoleCache contextRoleCache;

        private readonly IKrStageSerializer stageSerializer;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region constructor

        public KrSecondaryProcessExecutionEvaluator(
            IKrCompilationCache compilationCache,
            IKrSqlExecutor sqlExecutor,
            ISession session,
            IDbScope dbScope,
            ICardMetadata cardMetadata,
            IUnityContainer unityContainer,
            IKrScope scope,
            ICardCache cardCache,
            IKrTypesCache krTypesCache,
            IRoleRepository roleRepository,
            ICardContextRoleCache contextRoleCache,
            IKrStageSerializer stageSerializer)
        {
            this.compilationCache = compilationCache;
            this.sqlExecutor = sqlExecutor;
            this.session = session;
            this.dbScope = dbScope;
            this.cardMetadata = cardMetadata;
            this.unityContainer = unityContainer;
            this.scope = scope;
            this.cardCache = cardCache;
            this.krTypesCache = krTypesCache;
            this.roleRepository = roleRepository;
            this.contextRoleCache = contextRoleCache;
            this.stageSerializer = stageSerializer;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public bool Evaluate(
            IKrSecondaryProcessEvaluatorContext context)
        {
            var process = context.SecondaryProcess;

            if (!RunOnce(process, this.scope))
            {
                return false;
            }

            if (process is KrPureProcess pureProcess
                && !pureProcess.CheckRecalcRestrictions)
            {
                return true;
            }

            using (this.dbScope.Create())
            {
                var restrictionsResult = context.CardID.HasValue
                    ? this.CheckPermissionLocalContext(context)
                    : this.CheckPermissionGlobalContext(context.SecondaryProcess.ID);
                if (!restrictionsResult)
                {
                    return false;
                }

                var source = context.SecondaryProcess.ExecutionSourceCondition;
                var sqlText = context.SecondaryProcess.ExecutionSqlCondition;
                if (string.IsNullOrWhiteSpace(source)
                    && string.IsNullOrWhiteSpace(sqlText))
                {
                    // Если в карточке не задан исходный текст,
                    // то по дефолту там true и генерить контексты для выполнения излишне
                    return true;
                }

                return this.RunScript(source, context)
                    && this.RunSql(sqlText, context);
            }
        }

        #endregion

        #region private

        private bool RunScript(
            string script,
            IKrSecondaryProcessEvaluatorContext context)
        {
            var compilationResult = this.compilationCache.Get();
            if (compilationResult.Result.Assembly == null)
            {
                logger.LogResult(compilationResult.ValidationResult);
                context.ValidationResult.Add(compilationResult.ToMissingAssemblyResult());
                return false;
            }

            var instance = this.CreateInstance(compilationResult, context);
            try
            {
                return instance.RunExecution();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.SecondaryProcessExecutionError(context.SecondaryProcess, e.Message);
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(text, script)
                    .ErrorException(e)
                    .End();
                return false;
            }
        }

        private bool RunSql(
            string sql,
            IKrSecondaryProcessEvaluatorContext context)
        {
            try
            {
                var ctx = new KrSqlExecutorContext(
                    sql,
                    context.ValidationResult,
                    (c, txt, args) => KrErrorHelper.SecondaryProcessSqlExecutionError(context.SecondaryProcess, txt, args),
                    context.SecondaryProcess,
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
                var text = KrErrorHelper.SecondaryProcessSqlExecutionError(context.SecondaryProcess, e.Message);
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .ErrorDetails(text, sql)
                    .ErrorException(e)
                    .End();
            }
            return false;
        }

        private bool CheckPermissionGlobalContext(Guid id)
        {
            var userID = this.session.User.ID;

            var db = this.dbScope.Db;
            var builder = this.dbScope.BuilderFactory
                .Select().Top(1).V(null)
                .From(nameof(KrConstants.KrSecondaryProcesses), "t").NoLock()
                .Where()
                .C(KrConstants.ID).Equals().P("ID")
                .And()
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
                        .And().C("ru", "UserID").Equals().P("UserID")))
                .Limit(1);

            return db
                .SetCommand(
                    builder.Build(),
                    db.Parameter("UserID", userID),
                    db.Parameter("ID", id))
                .LogCommand()
                .ExecuteNonQuery() != 0;
        }

        private bool CheckPermissionLocalContext(
            IKrSecondaryProcessEvaluatorContext context)
        {
            var userID = this.session.User.ID;

            var db = this.dbScope.Db;
            var builder = this.dbScope.BuilderFactory
                .Select().Top(1).V(true)
                .From(KrConstants.KrSecondaryProcesses.Name, "t").NoLock()
                .LeftJoin(KrConstants.KrStageTypes.Name, "tt").NoLock()
                .On().C("tt", KrConstants.ID).Equals().C("t", KrConstants.ID)
                .LeftJoin(KrConstants.KrStageDocStates.Name, "tds").NoLock()
                .On().C("tds", KrConstants.ID).Equals().C("t", KrConstants.ID)
                .Where()
                .C("t", KrConstants.ID).Equals().P("ID").N()
                .And()
                .C(KrConstants.KrSecondaryProcesses.IsGlobal).Equals().V(false).N()
                .And()
                .E(w => w
                    .C("tt", KrConstants.TypeID).IsNull()
                    .Or().N()
                    .C("tt", KrConstants.TypeID).Equals().P("TypeID")).N()
                .And()
                .E(w => w
                    .C("tds", KrConstants.StateID).IsNull().N()
                    .Or()
                    .C("tds", KrConstants.StateID).Equals().P("StateID")).N()
                .And()
                .E(w => w
                    .NotExists(e => e
                        .Select().V(null)
                        .From(KrConstants.KrStageRoles.Name, "r").NoLock()
                        .Where().C("r", KrConstants.ID).Equals().C("t", KrConstants.ID)).N()
                    .Or()
                    .Exists(e => e
                        .Select().V(null)
                        .From(KrConstants.KrStageRoles.Name, "r").NoLock()
                        .InnerJoin("RoleUsers", "ru").NoLock()
                        .On().C("ru", KrConstants.ID).Equals().C("r", KrConstants.RoleID)
                        .Where().C("r", KrConstants.ID).Equals().C("t", KrConstants.ID)
                        .And().C("ru", "UserID").Equals().P("UserID")))
                .Limit(1);

            var result = db
                .SetCommand(
                    builder.Build(),
                    db.Parameter("UserID", userID),
                    db.Parameter("ID", context.SecondaryProcess.ID),
                    db.Parameter("TypeID", context.DocTypeID ?? context.CardTypeID ?? Guid.Empty),
                    db.Parameter("StateID", context.State?.ID ?? KrState.Draft.ID))
                .LogCommand()
                .Execute<bool>();
            if (!result)
            {
                return false;
            }

            var roleIDs = context.SecondaryProcess.ContextRolesIDs;
            if (roleIDs.Count == 0)
            {
                return true;
            }

            foreach (var roleID in roleIDs)
            {
                var contextRole = this.contextRoleCache.GetAsync(roleID).GetAwaiter().GetResult(); // TODO async

                var sqlTextForUser = contextRole.Entries["ContextRoles", "SqlTextForUser"] as string ?? string.Empty;
                var sqlTextForCard = contextRole.Entries["ContextRoles", "SqlTextForCard"] as string ?? string.Empty;

                // TODO async
                var userInRole = this.roleRepository.CheckUserInCardContextAsync(
                    roleID,
                    contextRole.Sections["Roles"].Fields.Get<string>("Name"),
                    sqlTextForUser,
                    sqlTextForCard,
                    context.CardID ?? Guid.Empty,
                    userID,
                    useSafeTransaction: false).GetAwaiter().GetResult();

                if (userInRole)
                {
                    return true;
                }
            }

            return false;
        }


        private IKrScript CreateInstance(
            IKrCompilationResult compilationResult,
            IKrSecondaryProcessEvaluatorContext context)
        {
            var instance = compilationResult.CreateInstance(
                SourceIdentifiers.KrExecutionClass, SourceIdentifiers.SecondaryProcessAlias, context.SecondaryProcess.ID);

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

            instance.ContextualSatellite = context.ContextualSatellite;
            instance.SecondaryProcess = context.SecondaryProcess;
            instance.CardContext = context.CardContext;
            instance.ValidationResult = context.ValidationResult;
            instance.Session = this.session;
            instance.DbScope = this.dbScope;
            instance.UnityContainer = this.unityContainer;
            instance.CardMetadata = this.cardMetadata;
            instance.KrScope = this.scope;
            instance.CardCache = this.cardCache;
            instance.KrTypesCache = this.krTypesCache;
            instance.StageSerializer = this.stageSerializer;

            return instance;
        }

        /// <summary>
        /// Возвращает <c>true</c>, если одиночный запуск не требуется или скрипт запускается в первый раз.
        /// <c>false</c>, если скрипт запускается повторно, хотя требуется один раз.
        /// </summary>
        /// <param name="secondaryProcess"></param>
        /// <param name="krScope"></param>
        /// <returns></returns>
        public static bool RunOnce(IKrSecondaryProcess secondaryProcess, IKrScope krScope)
        {
            if (krScope is null
                || secondaryProcess is null
                || !secondaryProcess.RunOnce)
            {
                return true;
            }

            var info = krScope.Info;
            var key = FormatKey(secondaryProcess);
            if (info.ContainsKey(key))
            {
                return false;
            }

            info[key] = BooleanBoxes.True;
            return true;
        }

        private static string FormatKey(
            IKrSecondaryProcess secondaryProcess) =>
            $"RunExecutionScriptOnce_{secondaryProcess.ID:N}";

        #endregion
    }
}