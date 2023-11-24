using System;
using System.Collections.Generic;
using System.Threading;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <inheritdoc />
    public class KrPermissionsManagerContext : IKrPermissionsManagerContext, IKrPermissionsRecalcContext
    {
        #region Constructors

        protected KrPermissionsManagerContext(
            IKrPermissionsManagerContext managerContext)
            :this(
                managerContext.DbScope,
                managerContext.Session,
                managerContext.Card,
                managerContext.CardID,
                managerContext.CardType,
                managerContext.DocTypeID,
                managerContext.DocState,
                managerContext.FileID,
                managerContext.FileVersionID,
                managerContext.WithRequiredPermissions,
                managerContext.WithExtendedPermissions,
                managerContext.IgnoreSections,
                managerContext.Mode,
                managerContext.ValidationResult,
                managerContext.Info,
                managerContext.PreviousToken,
                managerContext.ServerToken,
                managerContext.ExtensionContext,
                managerContext.CancellationToken)
        {
            this.Descriptor = managerContext.Descriptor;
        }

        public KrPermissionsManagerContext(
            IDbScope dbScope,
            ISession session,
            Card card,
            Guid? cardID,
            CardType cardType,
            Guid? docTypeID,
            KrState? docState,
            Guid? fileID,
            Guid? fileVersionID,
            bool withRequiredPermissions,
            bool withExtendedPermissions,
            ICollection<string> ignoreSections,
            KrPermissionsCheckMode mode,
            IValidationResultBuilder validationResult,
            IDictionary<string, object> additionalInfo,
            KrToken prevToken = null,
            KrToken serverToken = null,
            ICardExtensionContext extensionContext = null,
            CancellationToken cancellationToken = default)
        {
            this.DbScope = dbScope;
            this.Session = session;

            this.Card = card;
            this.CardID = cardID ?? card?.ID;
            this.CardType = cardType;
            this.DocTypeID = docTypeID;
            this.DocState = docState;
            this.FileID = fileID;
            this.FileVersionID = fileVersionID;

            this.WithRequiredPermissions = withRequiredPermissions;
            this.WithExtendedPermissions = withExtendedPermissions;
            this.IgnoreSections = ignoreSections ?? EmptyHolder<string>.Collection;
            this.Mode = mode;
            this.ValidationResult = validationResult ?? new ValidationResultBuilder();
            this.Info = additionalInfo ?? new Dictionary<string, object>(StringComparer.Ordinal);

            this.PreviousToken = prevToken;
            this.ServerToken = serverToken;
            this.ExtensionContext = extensionContext;
            this.CancellationToken = cancellationToken;
        }

        #endregion

        #region IExtensionContext Implementation

        /// <inheritdoc />
        public CancellationToken CancellationToken { get; set; }

        #endregion

        #region IKrPermissionsManagerContext Implementation

        /// <inheritdoc />
        public ICardExtensionContext ExtensionContext { get; }

        /// <inheritdoc />
        public KrToken PreviousToken { get; }

        /// <inheritdoc />
        public KrToken ServerToken { get; }

        /// <inheritdoc />
        public KrPermissionsDescriptor Descriptor { get; set; }

        /// <inheritdoc />
        public KrPermissionsCheckMode Mode { get; }

        /// <inheritdoc />
        public string Method { get; set; }

        /// <inheritdoc />
        public Card Card { get; }

        /// <inheritdoc />
        public Guid? CardID { get; }

        /// <inheritdoc />
        public CardType CardType { get; }

        /// <inheritdoc />
        public Guid? DocTypeID { get; }

        /// <inheritdoc />
        public KrState? DocState { get; }

        /// <inheritdoc />
        public Guid? FileID { get; }

        /// <inheritdoc />
        public Guid? FileVersionID { get; }

        /// <inheritdoc />
        public IDictionary<string, object> Info { get; }

        /// <inheritdoc />
        public IValidationResultBuilder ValidationResult { get; }

        /// <inheritdoc />
        public IDbScope DbScope { get; }

        /// <inheritdoc />
        public ISession Session { get; }

        /// <inheritdoc />
        public bool WithRequiredPermissions { get; }

        /// <inheritdoc />
        public bool WithExtendedPermissions { get; }

        /// <inheritdoc />
        public ICollection<string> IgnoreSections { get; }

        #endregion

        #region IKrPermissionsRecalcContext Implementation

        /// <inheritdoc />
        public bool IsRecalcRequired { get; set; }

        #endregion
    }
}
