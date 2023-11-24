#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Tessa.Platform.Collections;
using Tessa.Properties.Resharper;
using Tessa.UI;
using Tessa.Extensions.Default.Shared.Workplaces;

#endregion

namespace Tessa.Extensions.Default.Client.Workplaces
{
    public class TreeItemFilteringSettingsViewModel : ViewModel<ITreeItemFilteringSettings>
    {
        #region Fields

        [NotNull]
        private readonly List<StringValueModel> parameters = new List<StringValueModel>();

        [NotNull]
        private readonly List<StringValueModel> refSections = new List<StringValueModel>();

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public TreeItemFilteringSettingsViewModel(ITreeItemFilteringSettings model)
            : base(model)
        {
            this.InitializeEditors(model);
        }

        /// <inheritdoc />
        public TreeItemFilteringSettingsViewModel(ITreeItemFilteringSettings model, ViewModelScope scope)
            : base(model, scope)
        {
            this.InitializeEditors(model);
        }

        #endregion

        #region Public properties

        /// <summary>
        ///     Список имен параметров
        /// </summary>
        [NotNull]
        public StringsEditorViewModel Parameters { get; private set; }


        /// <summary>
        ///     Список имен ссылочных секций
        /// </summary>
        [NotNull]
        public StringsEditorViewModel RefSections { get; private set; }

        #endregion

        #region Other methods

        private StringsEditorViewModel CreateEditor(IList<StringValueModel> strings)
        {
            return new StringsEditorViewModel(strings, ViewModelScope.Global);
        }

        private void InitializeEditors(ITreeItemFilteringSettings model)
        {
            this.InitializeRefSections(model);
            this.InitializeParameters(model);
        }

        private void InitializeParameters(ITreeItemFilteringSettings model)
        {
            this.parameters.AddRange(model.Parameters.Select(c => new StringValueModel(c)));
            this.Parameters = this.CreateEditor(this.parameters);
            CollectionChangedEventManager.AddHandler(this.Parameters.Items, this.ParametersChanged);
        }

        private void InitializeRefSections(ITreeItemFilteringSettings model)
        {
            this.refSections.AddRange(model.RefSections.Select(c => new StringValueModel(c)));
            this.RefSections = this.CreateEditor(this.refSections);
            CollectionChangedEventManager.AddHandler(this.RefSections.Items, this.RefSectionsChanged);
        }

        private void ParametersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateCollection(this.Model.Parameters, e);
        }

        private void RefSectionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateCollection(this.Model.RefSections, e);
        }

        private static void UpdateCollection([NotNull] IList<string> collection, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    List<string> newItems = e.NewItems.OfType<StringValueViewModel>().Select(c => c.Value).ToList();
                    collection.AddRange(newItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    List<string> removedItems = e.OldItems.OfType<StringValueViewModel>().Select(c => c.Value).ToList();
                    collection.RemoveRange(removedItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}