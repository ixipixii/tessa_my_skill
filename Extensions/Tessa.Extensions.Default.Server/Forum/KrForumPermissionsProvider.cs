using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Forums;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Forum
{
    public class KrForumPermissionsProvider : ForumPermissionsProvider
    {
        private readonly IKrPermissionsManager permissionManager;

        public KrForumPermissionsProvider(
            IKrPermissionsManager permissionManager)
        {
            this.permissionManager = permissionManager;
        }

        public override async Task<(bool result, ValidationResult validationResult)> CheckHasPermissionAddTopicAsync(ICardExtensionContext context, Guid cardID, CancellationToken cancellationToken = default)
        {
            KrPermissionFlagDescriptor required = KrPermissionFlagDescriptors.AddTopics;

            return  await this.CheckHasPermissions(
                required, 
                context, 
                cardID,
                cancellationToken);
        }


        public override async Task<(bool result, ValidationResult validationResult)> CheckHasPermissionIsSuperModeratorAsync(ICardExtensionContext context, Guid cardID, CancellationToken cancellationToken = default)
        {
            KrPermissionFlagDescriptor required = KrPermissionFlagDescriptors.SuperModeratorMode;

            return await this.CheckHasPermissions(
                required,
                context,
                cardID,
                cancellationToken);
        }

        private async Task<(bool result, ValidationResult validationResult)> CheckHasPermissions(KrPermissionFlagDescriptor required, ICardExtensionContext context, Guid cardID, CancellationToken cancellationToken = default)
        {
            var permissionContext = await permissionManager.TryCreateContextAsync(
                new KrPermissionsCreateContextParams
                {
                    CardID = cardID,
                    ExtensionContext = context,
                    AdditionalInfo = context.Info,
                    ValidationResult = new ValidationResultBuilder(),
                },
                cancellationToken);

            if (permissionContext == null)
            {
                // карточка не входит в типовое решение, возвращаем тру
                return (result: true, validationResult: new ValidationResultBuilder().Build());
            }

            var result = await permissionManager.CheckRequiredPermissionsAsync(
                permissionContext,
                required);
            
            return (result.Result, validationResult: result ? new ValidationResultBuilder().Build() : permissionContext.ValidationResult.Build());
        }
    }
}
