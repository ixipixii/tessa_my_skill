using System;
using System.Linq;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Notices;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class NotificationStageTypeHandler : StageTypeHandlerBase
    {
        #region Fields

        public static readonly string ScriptContextParameterType =
            $"global::{typeof(NotificationEmail).FullName}";

        public const string MethodName = "ModifyEmailAction";

        public const string MethodParameterName = "email";

        #endregion

        #region Constructors

        public NotificationStageTypeHandler(
            [Dependency(NotificationManagerNames.WithoutTransaction)]INotificationManager notificationManager,
            ISession session,
            IKrCompilationCache compilationCache,
            IUnityContainer unityContainer)
        {
            this.NotificationManager = notificationManager;
            this.Session = session;
            this.CompilationCache = compilationCache;
            this.UnityContainer = unityContainer;
        }

        #endregion

        #region Protected Properties

        protected INotificationManager NotificationManager { get; }

        protected ISession Session { get; }

        protected IKrCompilationCache CompilationCache { get; }

        protected IUnityContainer UnityContainer { get; }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var roles = context.Stage.Performers.Select(x => x.PerformerID).ToList();
            if (roles.Count == 0)
            {
                // Некому отправлять уведомления, считаем, что этап завершен
                return StageHandlerResult.CompleteResult;
            }
            var mainCardID = context.Stage.InfoStorage.TryGet<Guid?>("MainCardID") ?? context.MainCardID ?? this.Session.User.ID;
            var notificationID = context.Stage.SettingsStorage.TryGet<Guid>(KrNotificationSettingVirtual.NotificationID);
            var excludeDeputies = context.Stage.SettingsStorage.TryGet<bool?>(KrNotificationSettingVirtual.ExcludeDeputies) ?? false;
            var excludeSubscribers = context.Stage.SettingsStorage.TryGet<bool?>(KrNotificationSettingVirtual.ExcludeSubscribers) ?? false;

            var inst = HandlerHelper.CreateScriptInstance(
                this.CompilationCache,
                context.Stage.ID,
                context.ValidationResult);
            HandlerHelper.InitScriptContext(this.UnityContainer, inst, context);

            context.ValidationResult.Add(
                this.NotificationManager.SendAsync(
                    notificationID,
                    roles,
                    new NotificationSendContext
                    {
                        MainCardID = mainCardID,
                        ExcludeDeputies = excludeDeputies,
                        DisableSubscribers = excludeSubscribers,
                        ModifyEmailActionAsync = async (e, ct) => inst.InvokeExtra(MethodName, e, false),
                    }).GetAwaiter().GetResult()); // TODO async

            return StageHandlerResult.CompleteResult;
        }

        #endregion
    }
}