using System;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public static class KrErrorHelper
    {
        #region public

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertKrSatellte(Card card)
        {
            if (card.TypeID != DefaultCardTypes.KrSatelliteTypeID)
            {
                throw new InvalidOperationException($"{nameof(Card)}.{nameof(card.TypeID)} != " +
                    $"{nameof(DefaultCardTypes)}.{nameof(DefaultCardTypes.KrSatelliteTypeID)}");
            }
        }

        public static void WarnStageTypeIsNull(
            IKrProcessRunnerContext context,
            Stage stage)
        {
            context.ValidationResult
                .BeginSequence()
                .SetObjectName(nameof(IKrProcessRunner))
                .WarningText("$KrProcessRunner_StageTypeIsNull", stage.Name, stage.RowID)
                .End();

        }

        public static void WarnStageHandlerIsNull(
            IKrProcessRunnerContext context,
            Stage stage)
        {
            context.ValidationResult
                .BeginSequence()
                .SetObjectName(nameof(IKrProcessRunner))
                .WarningText("$KrProcessRunner_StageHandlerIsNull", stage.Name, stage.RowID, stage.StageTypeID, stage.StageTypeCaption)
                .End();
        }

        public static void PerformerNotSpecified(Stage stage)
        {
            throw new ProcessRunnerInterruptedException(LocalizationManager.Format("$UI_Error_PerformerNotSpecified", stage.Name));
        }

        public static void TimeLimitNotSpecified(Stage stage)
        {
            throw new ProcessRunnerInterruptedException(LocalizationManager.Format("$UI_Error_TimeLimitNotSpecified", stage.Name));
        }
        
        public static void PlannedNotSpecified(Stage stage)
        {
            throw new ProcessRunnerInterruptedException(LocalizationManager.Format("$UI_Error_PlannedNotSpecified", stage.Name));
        }
        
        public static void TimeLimitOrPlannedNotSpecified(Stage stage)
        {
            throw new ProcessRunnerInterruptedException(LocalizationManager.Format("$UI_Error_TimeLimitOrPlannedNotSpecified", stage.Name));
        }

        public static string GetTraceTextFromExecutionUnit(IKrExecutionUnit unit, string scriptType = null)
        {
            var stageName = unit.Instance.Stage?.Name;
            var templateName = unit.Instance.TemplateName;
            var groupName = unit.Instance.StageGroupName;
            var buttonName = unit.Instance.Button?.Name;
            return FormatErrorMessageTrace(scriptType, stageName, templateName, groupName, buttonName);
        }

        public static string GetTraceTextFromStage(Stage stage, string scriptType = null)
        {
            var stageName = stage.Name;
            var templateName = stage.TemplateName;
            var groupName = stage.StageGroupName;
            return FormatErrorMessageTrace(scriptType, stageName, templateName, groupName, null);
        }

        public static string UnexpectedError(IKrExecutionUnit unit)
        {
            return LocalizationManager.Format(
                "$KrProcess_ErrorMessage_ErrorFormat",
                GetTraceTextFromExecutionUnit(unit),
                "$KrProcess_ErrorMessage_UnexpectedException",
                String.Empty);
        }

        public static string UnexpectedError(Stage stage)
        {
            return LocalizationManager.Format(
                "$KrProcess_ErrorMessage_ErrorFormat",
                GetTraceTextFromStage(stage),
                "$KrProcess_ErrorMessage_UnexpectedException",
                String.Empty);
        }

        public static string DesignTimeError(
            IKrExecutionUnit unit,
            string errorText,
            params object[] args)
        {
            return ScriptErrorInternal(unit, "Design", errorText, args);
        }

        public static string SqlDesignTimeError(
            IKrExecutionUnit unit,
            string errorText,
            params object[] args)
        {
            return QueryErrorInternal(unit, "Design", errorText, args);
        }

        public static string RuntimeError(
            IKrExecutionUnit unit,
            string errorText,
            params object[] args)
        {
            return ScriptErrorInternal(unit, "Runtime", errorText, args);
        }

        public static string SqlRuntimeError(
            IKrExecutionUnit unit,
            string errorText,
            params object[] args)
        {
            return QueryErrorInternal(unit, "Runtime", errorText, args);
        }

        public static string ButtonVisibilityError(
            IKrProcessButton button,
            string errorText,
            params object[] args)
        {
            return SecondaryProcessErrorInternal(
                button,
                "Visibility",
                "$KrProcess_ErrorMessage_FullScriptInDetails",
                errorText,
                args);
        }

        public static string ButtonSqlVisibilityError(
            IKrProcessButton button,
            string errorText,
            params object[] args)
        {
            return SecondaryProcessErrorInternal(
                button,
                "VisibilitySql",
                "$KrProcess_ErrorMessage_FullQueryInDetails",
                errorText,
                args);
        }

        public static string SecondaryProcessExecutionError(
            IKrSecondaryProcess secondaryProcess,
            string errorText,
            params object[] args)
        {
            return SecondaryProcessErrorInternal(
                secondaryProcess,
                "Execution",
                "$KrProcess_ErrorMessage_FullScriptInDetails",
                errorText,
                args);
        }

        public static string SecondaryProcessSqlExecutionError(
            IKrSecondaryProcess secondaryProcess,
            string errorText,
            params object[] args)
        {
            return SecondaryProcessErrorInternal(
                secondaryProcess,
                "ExecutionSql",
                "$KrProcess_ErrorMessage_FullQueryInDetails",
                errorText,
                args);
        }

        public static string SqlPerformersError(
            string stage,
            string template,
            string group,
            string button,
            string errorText,
            params object[] args)
        {
            var text = LocalizationManager.Format(errorText, args);
            return LocalizationManager.Format(
                "$KrProcess_ErrorMessage_ErrorFormat",
                FormatErrorMessageTrace("$KrProcess_ErrorMessage_SqlPerformersTrace", stage, template, @group, button),
                text,
                "$KrProcess_ErrorMessage_FullQueryInDetails");
        }

        public static string FormatErrorMessageTrace(
            string name,
            string stage,
            string template,
            string group,
            string button)
        {
            return FormatErrorMessageTrace(name, FormatErrorMessageTrace(stage, template, group, button));
        }

        public static string FormatErrorMessageTrace(
            string name,
            string location)
        {
            return LocalizationManager.Localize(name) + ": " + location;
        }

        public static string FormatErrorMessageTrace(
            string stage,
            string template,
            string group,
            string button)
        {
            var sb = StringBuilderHelper.Acquire(256);

            if (!string.IsNullOrWhiteSpace(stage))
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(LocalizationManager.Format("$KrProcess_ErrorMessage_StageTrace", stage));
            }

            if (!string.IsNullOrWhiteSpace(template))
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(LocalizationManager.Format("$KrProcess_ErrorMessage_TemplateTrace", template));
            }

            if (!string.IsNullOrWhiteSpace(group))
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(LocalizationManager.Format("$KrProcess_ErrorMessage_GroupTrace", group));
            }

            if (!string.IsNullOrWhiteSpace(button))
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(LocalizationManager.Format("$KrProcess_ErrorMessage_SecondaryProcessTrace", button));
            }

            return sb.ToStringAndRelease();
        }

        /// <summary>
        /// Форматирование сообщения о том, что в маршруте нет активных этапов с дополнительным выводом кнопки.
        /// </summary>
        /// <param name="secondaryProcess"></param>
        /// <returns></returns>
        public static string FormatEmptyRoute(
            IKrSecondaryProcess secondaryProcess)
        {
            var secondPart = secondaryProcess != null
                ? LocalizationManager.Format(
                    "$KrStages_RouteIsEmptySecondaryProcessDescription",
                    secondaryProcess.Name,
                    secondaryProcess.ID)
                : "$KrProcess_MainRouteHasNoActiveStages";
            return LocalizationManager.Format("$KrProcess_RouteHasNoActiveStages", secondPart);
        }

        public static string ProcessStartingForDifferentCardID() =>
            LocalizationManager.GetString("KrSecondaryProcess_ProcessStartingForDifferentCardID");

        #endregion

        #region private

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ScriptErrorInternal(
            IKrExecutionUnit unit,
            string executionType,
            string errorText,
            params object[] args)
        {
            return ErrorInternal(unit, executionType, "$KrProcess_ErrorMessage_FullScriptInDetails", errorText, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string QueryErrorInternal(
            IKrExecutionUnit unit,
            string executionType,
            string errorText,
            params object[] args)
        {
            return ErrorInternal(unit, executionType, "$KrProcess_ErrorMessage_FullQueryInDetails", errorText, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ErrorInternal(
            IKrExecutionUnit unit,
            string executionType,
            string whereIsCode,
            string errorText,
            params object[] args)
        {
            var et = LocalizationManager.Format(errorText, args);
            var scriptType = LocalizeScriptKrStringType(unit.Instance.KrScriptType, executionType);
            var trace = GetTraceTextFromExecutionUnit(unit, scriptType);

            return LocalizationManager.Format(
                "$KrProcess_ErrorMessage_ErrorFormat",
                trace,
                et,
                whereIsCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string SecondaryProcessErrorInternal(
            IKrSecondaryProcess secondaryProcess,
            string executionType,
            string whereIsCode,
            string errorText,
            params object[] args)
        {
            var et = LocalizationManager.Format(errorText, args);
            var scriptType = LocalizationManager.GetString($"KrProcess_ErrorMessage_{executionType}Trace");
            var trace = FormatErrorMessageTrace(scriptType, null, null, null, secondaryProcess.Name);

            return LocalizationManager.Format(
                "$KrProcess_ErrorMessage_ErrorFormat",
                trace,
                et,
                whereIsCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string LocalizeScriptKrStringType(KrScriptType type, string pref = null)
        {
            switch (type)
            {
                case KrScriptType.Before:
                    return LocalizationManager.GetString($"KrProcess_ErrorMessage_{pref}BeforeTrace");
                case KrScriptType.Condition:
                    return LocalizationManager.GetString($"KrProcess_ErrorMessage_{pref}ConditionTrace");
                case KrScriptType.After:
                    return LocalizationManager.GetString($"KrProcess_ErrorMessage_{pref}AfterTrace");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}