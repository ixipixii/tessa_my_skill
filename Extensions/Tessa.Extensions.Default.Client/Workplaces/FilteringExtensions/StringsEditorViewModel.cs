#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Tessa.UI;
using Tessa.UI.Controls;
using Tessa.Views.Parser;

#endregion

namespace Tessa.Extensions.Default.Client.Workplaces
{
    public class StringsEditorViewModel : SelectorViewModel<StringValueModel, StringValueViewModel>
    {
        #region Fields

        private DelegateCommand addNewItemCommand;
        private DelegateCommand deleteItemCommand;
        private string text;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public StringsEditorViewModel(IList<StringValueModel> itemsSource, ViewModelScope scope)
            : base(itemsSource, scope)
        {
        }

        /// <inheritdoc />
        public StringsEditorViewModel(IList<StringValueModel> itemsSource, IList<StringValueModel> selectedItems,
            ViewModelScope scope)
            : base(itemsSource, selectedItems, scope)
        {
        }

        /// <inheritdoc />
        protected StringsEditorViewModel(ViewModelScope scope)
            : base(scope)
        {
        }

        #endregion

        #region Public properties

        public ICommand AddNewItemCommand => this.addNewItemCommand;

        public ICommand DeleteItemCommand => this.deleteItemCommand;

        public string Text
        {
            get => this.text;
            set
            {
                if (value == this.text)
                {
                    return;
                }

                this.text = value;
                this.OnPropertyChanged(nameof(this.Text));
                this.UpdateCommandsState();
            }
        }

        #endregion

        #region Other methods

        /// <inheritdoc />
        protected override StringValueViewModel GetItemViewModel(StringValueModel itemModel)
        {
            return new StringValueViewModel(itemModel);
        }

        /// <inheritdoc />
        protected override void Initialize(IList<StringValueModel> itemsSource)
        {
            base.Initialize(itemsSource);
            this.InitializeCommands();
        }

        /// <inheritdoc />
        protected override void OnItemSelected(StringValueViewModel item)
        {
            base.OnItemSelected(item);
            this.UpdateCommandsState();
        }

        /// <inheritdoc />
        protected override void OnItemUnselected(StringValueViewModel item)
        {
            base.OnItemUnselected(item);
            this.UpdateCommandsState();
        }

        private void AddNewItemExecute(object obj)
        {
            var model = new StringValueModel { Value = this.Text.Trim() };
            this.ItemsSource.Add(model);
            var viewModel = new StringValueViewModel(model);
            this.Items.Add(viewModel);
            this.SelectedItem = viewModel;
            this.Text = string.Empty;
            this.UpdateCommandsState();
        }

        private bool DeleteItemCanExecute(object arg)
        {
            return this.SelectedItem != null;
        }

        private void DeleteItemExecute(object obj)
        {
            var newSelectedIndex = Math.Max(this.Items.IndexOf(this.SelectedItem)-1, 0);
            this.Items.Remove(this.SelectedItem);
            if (this.Items.Any())
            {
                this.SelectedItem = this.Items[newSelectedIndex];
            }
            
            this.UpdateCommandsState();
        }


        private void InitializeCommands()
        {
            this.addNewItemCommand = new DelegateCommand(this.AddNewItemExecute, this.AddNewItemCanExecute);
            this.deleteItemCommand = new DelegateCommand(this.DeleteItemExecute, this.DeleteItemCanExecute);
        }

        private bool AddNewItemCanExecute(object arg)
        {
            return !string.IsNullOrWhiteSpace(this.Text) && 
                !ParserNames.IsAny(this.Items.Select(c => c.Value).ToArray(), this.Text.Trim());
        }

        private void UpdateCommandsState()
        {
            this.deleteItemCommand.RaiseCanExecuteChanged();
            this.addNewItemCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}