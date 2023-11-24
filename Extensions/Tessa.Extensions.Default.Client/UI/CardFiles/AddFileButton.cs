using System;
using Tessa.Cards;
using Tessa.Platform.Runtime;
using Tessa.Properties.Resharper;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Files;

namespace Tessa.Extensions.Default.Client.UI.CardFiles
{
    [UsedImplicitly]
    public sealed class AddFileButtonViewModel : CommandViewModel<object>
    {
        [NotNull]
        private readonly ICardMetadata cardMetadata;

        [NotNull]
        private readonly ISession session;

        private IFileControl fileControl;

        [CanBeNull]
        private CardViewControlViewModel viewModel;

        /// <inheritdoc />
        public AddFileButtonViewModel([NotNull] ISession session, [NotNull] ICardMetadata cardMetadata)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.cardMetadata = cardMetadata ?? throw new ArgumentNullException(nameof(cardMetadata));
        }

        [CanBeNull]
        [PublicAPI]
        public IFileControl FileControl
        {
            get => this.fileControl;
            set
            {
                if (Equals(value, this.fileControl))
                {
                    return;
                }

                this.fileControl = value;
                this.OnPropertyChanged(nameof(this.FileControl));
            }
        }


        [CanBeNull]
        [PublicAPI]
        public CardViewControlViewModel ViewModel
        {
            get => this.viewModel;
            set
            {
                if (Equals(value, this.viewModel))
                {
                    return;
                }

                this.viewModel = value;
                this.OnPropertyChanged(nameof(this.ViewModel));
            }
        }

        /// <inheritdoc />
        protected override bool CanExecuteOverride(object commandParameter)
        {
            return this.fileControl != null && this.viewModel != null;
        }

        /// <inheritdoc />
        protected override async void ExecuteOverride(object commandParameter)
        {
            if (this.ViewModel == null)
            {
                throw new InvalidOperationException("ViewModel is null");
            }

            if (this.FileControl == null)
            {
                throw new InvalidOperationException("fileControl is null");
            }

            await CardFilesActions.AddFiles(this.fileControl, this.session.User, this.cardMetadata,
                this.fileControl.Container, this.ViewModel.RefreshAsync);
        }
    }
}