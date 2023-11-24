using System;
using System.Collections.Generic;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public abstract class KrExecutorBase : IKrExecutor
    {
        protected readonly IKrSqlExecutor SqlExecutor;
        protected readonly IDbScope DbScope;
        protected readonly ICardCache CardCache;
        protected readonly IKrTypesCache KrTypesCache;
        protected readonly IKrStageSerializer StageSerializer;

        protected IList<IKrExecutionUnit> ExecutionUnits;
        protected List<Guid> ConfirmedIDs;

        protected KrExecutionStatus Status;
        protected Guid? InterruptedStageTemplateID;

        protected IValidationResultBuilder SharedValidationResult;

        protected KrExecutorBase(
            IKrSqlExecutor sqlExecutor,
            IDbScope dbScope,
            ICardCache cardCache,
            IKrTypesCache krTypesCache,
            IKrStageSerializer stageSerializer)
        {
            this.SqlExecutor = sqlExecutor;
            this.DbScope = dbScope;
            this.CardCache = cardCache;
            this.KrTypesCache = krTypesCache;
            this.StageSerializer = stageSerializer;
        }


        protected void CheckInterruption(IKrExecutionUnit unit)
        {
            if (unit.Instance.ValidationResult != null && !unit.Instance.ValidationResult.IsSuccessful())
            {
                this.InterruptWithStatus(unit.ID, KrExecutionStatus.InterruptByValidationResultError);
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        protected void InterruptWithStatus(Guid templateID, KrExecutionStatus status)
        {
            this.InterruptedStageTemplateID = templateID;
            this.Status = status;
        }

        #region before

        protected static void RunBefore(IKrExecutionUnit unit)
        {
            try
            {
                unit.Instance.RunBefore();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.DesignTimeError(unit, e.Message);
                throw new ScriptExecutionException(text, unit.DesignTimeSources.SourceBefore, e);
            }
        }

        #endregion

        #region conditions

        protected void RunConditions(IKrExecutionUnit unit, IKrExecutionContext context)
        {
            unit.Instance.Confirmed = ExecCondition(unit) && this.ExecSQL(unit, context);
            if (unit.Instance.Confirmed)
            {
                this.ConfirmedIDs.Add(unit.ID);
            }
        }

        protected static bool ExecCondition(IKrExecutionUnit unit)
        {
            try
            {
                return unit.Instance.RunCondition();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.DesignTimeError(unit, e.Message);
                throw new ScriptExecutionException(text, unit.DesignTimeSources.SourceCondition, e);
            }
        }

        protected bool ExecSQL(IKrExecutionUnit unit, IKrExecutionContext context)
        {
            var sqlExecutionContext = new KrSqlExecutorContext(
                unit.DesignTimeSources.SqlCondition,
                this.SharedValidationResult,
                (ctx, txt, args) => KrErrorHelper.SqlDesignTimeError(unit, txt, args),
                unit,
                context.SecondaryProcess,
                context.CardID,
                context.CardTypeID,
                context.DocTypeID,
                context.WorkflowProcess.State);
            return this.SqlExecutor.ExecuteCondition(sqlExecutionContext);
        }

        #endregion

        #region after

        protected static void RunAfter(IKrExecutionUnit unit)
        {
            try
            {
                unit.Instance.RunAfter();
            }
            catch (Exception e)
            {
                var text = KrErrorHelper.DesignTimeError(unit, e.Message);
                throw new ScriptExecutionException(text, unit.DesignTimeSources.SourceAfter, e);
            }
        }

        #endregion

        protected void RunForAll(Action<IKrExecutionUnit> action)
        {
            foreach (var unit in this.ExecutionUnits)
            {
                if (this.Status != KrExecutionStatus.InProgress)
                {
                    return;
                }

                try
                {
                    action(unit);
                    this.CheckInterruption(unit);
                }
                catch (ExecutionExceptionBase eeb)
                {
                    var validator = ValidationSequence
                        .Begin(this.SharedValidationResult)
                        .SetObjectName(this)
                        .ErrorDetails(eeb.ErrorMessageText, eeb.SourceText);
                    if (eeb.InnerException != null)
                    {
                        validator.ErrorException(eeb.InnerException);
                    }
                    validator.End();
                    this.InterruptWithStatus(unit.ID, KrExecutionStatus.InterruptByException);
                }
                catch (Exception e)
                {
                    var text = KrErrorHelper.UnexpectedError(unit);

                    ValidationSequence
                        .Begin(this.SharedValidationResult)
                        .SetObjectName(this)
                        .ErrorText(text)
                        .ErrorException(e)
                        .End();
                    this.InterruptWithStatus(unit.ID, KrExecutionStatus.InterruptByException);
                }
            }
        }

        /// <inheritdoc />
        public abstract IKrExecutionResult Execute(
            IKrExecutionContext context);
    }
}