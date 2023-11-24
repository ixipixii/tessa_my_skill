using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Platform.Conditions;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Cards
{
    public sealed class KrPermissionRuleNewGetExtension : CardNewGetExtension
    {
        #region Fields

        private readonly ICardMetadata cardMetadata;

        #endregion

        #region Constructors

        public KrPermissionRuleNewGetExtension(ICardMetadata cardMetadata)
        {
            this.cardMetadata = cardMetadata;
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || (context.Method == CardNewMethod.Template
                    && !context.Request.Info.TryGet<bool>(CardHelper.CopyingCardKey)))
            {
                return;
            }
            
            await DeserializeConditionsAsync(context.Response.Card, context.CancellationToken);
        }

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            await DeserializeConditionsAsync(context.Response.Card, context.CancellationToken);
        }

        #endregion

        #region Private Methods

        private async ValueTask DeserializeConditionsAsync(Card card, CancellationToken cancellationToken = default)
        {
            await ConditionHelper.DeserializeConditionsToEntrySectionAsync(
                card,
                cardMetadata,
                "KrPermissions",
                "Conditions", 
                cancellationToken);
        }

        #endregion

    }
}
