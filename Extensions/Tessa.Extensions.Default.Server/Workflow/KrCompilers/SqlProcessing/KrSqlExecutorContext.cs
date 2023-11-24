using System;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing
{
    public sealed class KrSqlExecutorContext: IKrSqlExecutorContext
    {
        /// <summary>
        /// Конструктор контекста, явно принимающий все параметры.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="validationResult"></param>
        /// <param name="getErrorTextFunc"></param>
        /// <param name="secondaryProcess"></param>
        /// <param name="stageGroupID"></param>
        /// <param name="stageTypeID"></param>
        /// <param name="stageTemplateID"></param>
        /// <param name="stageRowID"></param>
        /// <param name="stageTemplateName"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="cardID"></param>
        /// <param name="cardTypeID"></param>
        /// <param name="docTypeID"></param>
        /// <param name="state"></param>
        /// <param name="stageName"></param>
        /// <param name="templateName"></param>
        /// <param name="groupName"></param>
        public KrSqlExecutorContext(
            string query,
            IValidationResultBuilder validationResult,
            Func<IKrSqlExecutorContext, string, object[], string> getErrorTextFunc,
            IKrSecondaryProcess secondaryProcess,
            Guid stageGroupID,
            Guid stageTypeID,
            Guid stageTemplateID,
            Guid stageRowID,
            string stageTemplateName,
            Guid? userID,
            string userName,
            Guid? cardID,
            Guid? cardTypeID,
            Guid? docTypeID,
            KrState state,
            string stageName,
            string templateName,
            string groupName)
        {
            this.Query = query;
            this.ValidationResult = validationResult;
            this.GetErrorTextFunc = getErrorTextFunc;
            this.SecondaryProcess = secondaryProcess;
            this.StageGroupID = stageGroupID;
            this.StageTypeID = stageTypeID;
            this.StageTemplateID = stageTemplateID;
            this.StageRowID = stageRowID;
            this.UserID = userID;
            this.UserName = userName;
            this.CardID = cardID;
            this.CardTypeID = cardTypeID;
            this.DocTypeID = docTypeID;
            this.State = state;
            this.StageName = stageName;
            this.TemplateName = templateName;
            this.GroupName = groupName;
        }

        /// <summary>
        /// Формирование контекста для вычисления условия этапов, шаблонов, групп
        /// </summary>
        /// <param name="query"></param>
        /// <param name="validationResult"></param>
        /// <param name="getErrorTextFunc"></param>
        /// <param name="unit"></param>
        /// <param name="secondaryProcess"></param>
        /// <param name="cardID"></param>
        /// <param name="cardType"></param>
        /// <param name="docType"></param>
        /// <param name="state"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        public KrSqlExecutorContext(
            string query,
            IValidationResultBuilder validationResult,
            Func<IKrSqlExecutorContext, string, object[], string> getErrorTextFunc,
            IKrExecutionUnit unit,
            IKrSecondaryProcess secondaryProcess,
            Guid? cardID,
            Guid? cardType,
            Guid? docType,
            KrState state,
            Guid? userID = null,
            string userName = null)
        {
            this.Query = query;
            this.ValidationResult = validationResult;
            this.GetErrorTextFunc = getErrorTextFunc;
            this.SecondaryProcess = secondaryProcess;

            this.StageTemplateID = unit.StageTemplateInfo?.ID ?? Guid.Empty;
            this.StageGroupID = unit.StageGroupInfo?.ID ?? unit.StageTemplateInfo?.StageGroupID ?? Guid.Empty;

            this.StageRowID = Guid.Empty;
            this.StageTypeID = Guid.Empty;

            this.CardID = cardID;
            this.CardTypeID = cardType;
            this.DocTypeID = docType;
            this.State = state;
            this.StageName = unit.RuntimeStage?.StageName;
            this.TemplateName = unit.StageTemplateInfo?.Name;
            this.GroupName = unit.StageGroupInfo?.Name ?? unit.StageTemplateInfo?.StageGroupName;

            this.UserID = userID;
            this.UserName = userName;
        }

        /// <summary>
        /// Формирование контекста для пересчета исполнителей в этапе.
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="validationResult"></param>
        /// <param name="getErrorTextFunc"></param>
        /// <param name="krSecondaryProcess"></param>
        /// <param name="cardID"></param>
        /// <param name="cardType"></param>
        /// <param name="docType"></param>
        /// <param name="state"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        public KrSqlExecutorContext(
            Stage stage,
            IValidationResultBuilder validationResult,
            Func<IKrSqlExecutorContext, string, object[], string> getErrorTextFunc,
            IKrSecondaryProcess krSecondaryProcess,
            Guid? cardID,
            Guid? cardType,
            Guid? docType,
            KrState state,
            Guid? userID = null,
            string userName = null)
        {
            this.Query = stage.SqlPerformers;
            this.ValidationResult = validationResult;
            this.GetErrorTextFunc = getErrorTextFunc;
            this.SecondaryProcess = krSecondaryProcess;

            this.StageTemplateID = stage.TemplateID ?? Guid.Empty;
            this.StageGroupID = stage.StageGroupID;

            this.StageRowID = stage.RowID;
            this.StageTypeID = stage.StageTypeID ?? Guid.Empty;

            this.CardID = cardID;
            this.CardTypeID = cardType;
            this.DocTypeID = docType;
            this.State = state;
            this.StageName = stage.Name;
            this.TemplateName = stage.TemplateName;
            this.GroupName = stage.StageGroupName;

            this.UserID = userID;
            this.UserName = userName;
        }

        public KrSqlExecutorContext(
            string query,
            IValidationResultBuilder validationResult,
            Func<IKrSqlExecutorContext, string, object[], string> getErrorTextFunc,
            IKrSecondaryProcess krSecondaryProcess,
            Guid? cardID,
            Guid? cardType,
            Guid? docType,
            KrState? state,
            Guid? userID = null,
            string userName = null)
        {
            this.Query = query;
            this.ValidationResult = validationResult;
            this.GetErrorTextFunc = getErrorTextFunc;
            this.SecondaryProcess = krSecondaryProcess;
            this.CardID = cardID;
            this.CardTypeID = cardType;
            this.DocTypeID = docType;
            this.State = state;

            this.UserID = userID;
            this.UserName = userName;
        }

        /// <inheritdoc />
        public string Query { get; }

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult { get; }

        /// <inheritdoc />
        public Func<IKrSqlExecutorContext, string, object[], string> GetErrorTextFunc { get; }

        /// <inheritdoc />
        public IKrSecondaryProcess SecondaryProcess { get; }

        /// <inheritdoc />
        public Guid StageGroupID { get; }

        /// <inheritdoc />
        public Guid StageTypeID { get; }

        /// <inheritdoc />
        public Guid StageTemplateID { get; }

        /// <inheritdoc />
        public Guid StageRowID { get; }

        /// <inheritdoc />
        public Guid? UserID { get; }

        /// <inheritdoc />
        public string UserName { get; }

        /// <inheritdoc />
        public Guid? CardID { get; }

        /// <inheritdoc />
        public Guid? CardTypeID { get; }

        /// <inheritdoc />
        public Guid? DocTypeID { get; }

        /// <inheritdoc />
        public Guid? TypeID => this.DocTypeID ?? this.CardTypeID;

        /// <inheritdoc />
        public KrState? State { get; }

        /// <inheritdoc />
        public string StageName { get; }

        /// <inheritdoc />
        public string TemplateName { get; }

        /// <inheritdoc />
        public string GroupName { get; }
        
    }
}