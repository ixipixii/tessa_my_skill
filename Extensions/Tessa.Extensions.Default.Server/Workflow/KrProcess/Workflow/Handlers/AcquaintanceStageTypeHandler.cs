using System;
using System.Linq;
using Tessa.Extensions.Default.Shared.Acquaintance;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class AcquaintanceStageTypeHandler : StageTypeHandlerBase
    {
        #region Constructors

        public AcquaintanceStageTypeHandler(
            [Dependency(KrAcquaintanceManagerNames.WithoutTransaction)]IKrAcquaintanceManager acquaintanceManager,
            IRoleRepository roleRepository)
        {
            this.AcquaintanceManager = acquaintanceManager;
            this.RoleRepository = roleRepository;
        }

        #endregion

        #region Protected Properties

        protected IKrAcquaintanceManager AcquaintanceManager { get; set; }
        protected IRoleRepository RoleRepository { get; set; }

        #endregion

        #region Base Overrides

        public override StageHandlerResult HandleStageStart(IStageTypeHandlerContext context)
        {
            var roles = context.Stage.Performers.Select(x => x.PerformerID).ToList();
            var mainCardID = context.Stage.InfoStorage.TryGet<Guid?>("MainCardID") ?? context.MainCardID ?? Guid.Empty;
            if (roles.Count == 0
                || mainCardID == Guid.Empty)
            {
                // Некому отправлять ознакомление или нет карточки для ознакомления, считаем, что этап завершен
                return StageHandlerResult.CompleteResult;
            }
            var notificationID = context.Stage.SettingsStorage.TryGet<Guid?>(KrAcquaintanceSettingsVirtual.NotificationID);
            var excludeDeputies = context.Stage.SettingsStorage.TryGet<bool?>(KrAcquaintanceSettingsVirtual.ExcludeDeputies) ?? false;
            var comment = context.Stage.SettingsStorage.TryGet<string>(KrAcquaintanceSettingsVirtual.Comment);
            var placeholderAliases = context.Stage.SettingsStorage.TryGet<string>(KrAcquaintanceSettingsVirtual.AliasMetadata);
            var senderID = context.Stage.SettingsStorage.TryGet<Guid?>(KrAcquaintanceSettingsVirtual.SenderID);

            if (senderID.HasValue)
            {
                var role = this.RoleRepository.GetRoleAsync(senderID.Value).GetAwaiter().GetResult(); // TODO async
                if (role == null)
                {
                    context.ValidationResult.AddError(this, "Sender role isn't found.");
                    return StageHandlerResult.EmptyResult;
                }

                switch (role.RoleType)
                {
                    case RoleType.Personal:
                        // Do Nothing
                        break;

                    case RoleType.Context:
                        var contextRole = this.RoleRepository.GetContextRoleAsync(senderID.Value).GetAwaiter().GetResult(); // TODO async

                        var users = this.RoleRepository.GetCardContextUsersAsync(contextRole, mainCardID).GetAwaiter().GetResult(); // TODO async
                        if (users.Count > 0)
                        {
                            senderID = users[0].UserID;
                        }
                        break;

                    default:
                        context.ValidationResult.AddError(this, "$KrProcess_Acquaintance_SenderShoudBePersonalOrContext");
                        return StageHandlerResult.EmptyResult;
                }
            }

            var result = this.AcquaintanceManager.SendAsync(
                mainCardID,
                roles,
                excludeDeputies,
                comment,
                placeholderAliases,
                null,
                notificationID,
                senderID).GetAwaiter().GetResult(); // TODO async

            // при успешной отправке записывается текст вида "Ознакомление отправлено N сотрудникам",
            // его нет смысла отображать пользователю, который "продвинул" маршрут
            if (!result.IsSuccessful || result.HasWarnings)
            {
                context.ValidationResult.Add(result);
            }

            return StageHandlerResult.CompleteResult;
        }

        #endregion
    }
}