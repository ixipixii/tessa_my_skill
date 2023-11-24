using System;
using Tessa.Cards;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    /// <summary>
    /// Предоставляет права доступа на сервере в соответствии с типовым решением.
    /// </summary>
    public class KrCardServerPermissionsProvider :
        CardServerPermissionsProvider
    {
        #region Constructors

        public KrCardServerPermissionsProvider(IKrTokenProvider tokenProvider) =>
            this.tokenProvider = tokenProvider;

        #endregion

        #region Fields

        private readonly IKrTokenProvider tokenProvider;

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="ICardServerPermissionsProvider" and @item="SetFullPermissions:CardNewRequest"]'/>
        public override void SetFullPermissions(CardNewRequest request) =>
            this.tokenProvider.CreateToken(Guid.Empty).Set(request.Info);

        /// <doc path='info[@type="ICardServerPermissionsProvider" and @item="SetFullPermissions:CardGetRequest"]'/>
        public override void SetFullPermissions(CardGetRequest request) =>
            this.tokenProvider.CreateToken(request.CardID ?? Guid.Empty).Set(request.Info);

        /// <doc path='info[@type="ICardServerPermissionsProvider" and @item="SetFullPermissions:CardStoreRequest"]'/>
        public override void SetFullPermissions(CardStoreRequest request)
        {
            Card card = request.Card;
            this.tokenProvider.CreateToken(card).Set(card.Info);
        }

        /// <doc path='info[@type="ICardServerPermissionsProvider" and @item="SetFullPermissions:CardDeleteRequest"]'/>
        public override void SetFullPermissions(CardDeleteRequest request) =>
            this.tokenProvider.CreateToken(request.CardID ?? Guid.Empty).Set(request.Info);

        /// <doc path='info[@type="ICardServerPermissionsProvider" and @item="SetFullPermissions:CardGetFileContentRequest"]'/>
        public override void SetFullPermissions(CardGetFileContentRequest request) =>
            this.tokenProvider.CreateToken(request.CardID ?? Guid.Empty).Set(request.Info);

        /// <doc path='info[@type="ICardServerPermissionsProvider" and @item="SetFullPermissions:CardGetFileVersionsRequest"]'/>
        public override void SetFullPermissions(CardGetFileVersionsRequest request) =>
            this.tokenProvider.CreateToken(request.CardID ?? Guid.Empty).Set(request.Info);

        #endregion
    }
}
