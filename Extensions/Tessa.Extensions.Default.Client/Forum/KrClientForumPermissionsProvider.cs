using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Forums;
using Tessa.Platform.Validation;
using static Tessa.Forums.ForumPermissionsRequestExtensions;

namespace Tessa.Extensions.Default.Client.Forum
{
    public class KrClientForumPermissionsProvider : ForumPermissionsProvider
    {
        #region Private Fields

        private readonly ICardRepository cardRepository;

        #endregion

        #region Constructors

        public KrClientForumPermissionsProvider(ICardRepository cardRepository)
        {
            this.cardRepository = cardRepository;
        }

        #endregion
        
        public override async Task<(bool result, ValidationResult validationResult)> CheckHasPermissionAddTopicAsync(ICardExtensionContext context, Guid cardID, CancellationToken cancellationToken = default)
        {
            (var responseObject, ValidationResult validationResult) = await this.cardRepository.ForumAddTopicPermissionRequestAsync(
                new ForumPermissionsRequestObject()
                {
                    CardID = cardID,
                });
            
            return (responseObject.IsExistRequiredPermission, validationResult);
        }

        public override async Task<(bool result, ValidationResult validationResult)> CheckHasPermissionIsSuperModeratorAsync(ICardExtensionContext context, Guid cardID, CancellationToken cancellationToken = default)
        {
            (var responseObject, ValidationResult validationResult) = await this.cardRepository.ForumAddTopicPermissionRequestAsync(
                new ForumPermissionsRequestObject()
                {
                    CardID = cardID,
                });
            
            
            return (responseObject.IsExistRequiredPermission, validationResult);
        }

        public override bool IsEnableAddTopic(Card card)
        {
            if (this.TryGetKrToken(card, out var krToken))
            {
                if (krToken.HasPermission(KrPermissionFlagDescriptors.AddTopics))
                {
                    // Включаем кнопку добавить!
                    return true;
                }
            }
            return false;
        }

        public override bool IsEnableSuperModeratorMode(Card card)
        {
            if (this.TryGetKrToken(card, out var krToken))
            {
                if (krToken.HasPermission(KrPermissionFlagDescriptors.SuperModeratorMode))
                {
                    return true; 
                }
            }
            return false;
        }

        private bool TryGetKrToken(Card card, out KrToken krToken)
        {
            krToken = KrToken.TryGet(card.Info);
            return krToken != null;
        }
    }
}
