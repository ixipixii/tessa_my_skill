using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LinqToDB.Tools;
using Tessa.Cards;
using Tessa.Extensions.Default.Client.UI.CardFiles;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Shared.Models;
using Tessa.Files;
using Tessa.Json;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Properties.Resharper;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Blocks;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Controls;
using Tessa.UI.Files;
using Tessa.UI.Files.Controls;
using Tessa.Views;

namespace Tessa.Extensions.Client.UI
{
    public sealed class PnrFileControlUIExtension : FileExtension 
    {
        #region Constructors

        public PnrFileControlUIExtension(
            [NotNull] ICardMetadata cardMetadata,
            [NotNull] IUIHost uiHost,
            [NotNull] ICardRepository cardRepository,
            [NotNull] IViewService viewService,
            [NotNull] ISession session)
        {
            this.cardMetadata = cardMetadata;
            this.uiHost = uiHost;
            this.cardRepository = cardRepository;
            this.viewService = viewService;
            this.session = session;
        }

        #endregion

        #region Fields

        [NotNull]
        private readonly ICardMetadata cardMetadata;
        [NotNull]
        private readonly ICardRepository cardRepository;
        [NotNull]
        private readonly IViewService viewService;
        [NotNull]
        private readonly IUIHost uiHost;
        [NotNull]
        private readonly ISession session;

        #endregion

        #region Base Overrides


        #endregion
    }
}