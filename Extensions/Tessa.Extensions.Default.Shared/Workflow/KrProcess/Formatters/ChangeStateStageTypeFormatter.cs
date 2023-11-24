using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.Formatters
{
    public sealed class ChangeStateStageTypeFormatter : StageTypeFormatterBase
    {
        private const string KrChangeStateSettingsVirtual = nameof(KrChangeStateSettingsVirtual);
        private static readonly string SettingsStateName = StageTypeSettingsNaming.PlainColumnName(KrChangeStateSettingsVirtual, "StateName");

        public override void FormatClient(IStageTypeFormatterContext context)
        {
            // На клиенте доступны виртуальные секции.
            var state = context.StageRow.Fields.TryGet<string>(SettingsStateName);
            SetState(state, context);
        }

        public override void FormatServer(
            IStageTypeFormatterContext context)
        {
            // На сервере доступен словарь с настройками.
            var state = context.Settings.TryGet<string>(SettingsStateName);
            SetState(state, context);
        }

        private static void SetState(
            string state,
            IStageTypeFormatterContext context)
        {
            context.DisplaySettings = string.IsNullOrEmpty(state)
                ? string.Empty : state[0] == '$'
                ? string.Concat("{$UI_KrChangeState_State}: ", "{", state, "}")
                : string.Concat("{$UI_KrChangeState_State}: ", state);
        }

    }
}
