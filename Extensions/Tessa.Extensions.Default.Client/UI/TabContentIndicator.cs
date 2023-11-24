using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards;
using Tessa.Localization;
using Tessa.Platform.Storage;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Controls;

namespace Tessa.Extensions.Default.Client.UI
{
    public sealed class TabContentIndicator
    {
        private sealed class Visitor : BreadthFirstControlVisitor
        {
            private readonly Dictionary<string, int> fieldSectionMapping;

            private readonly IDictionary<Guid, string> fieldIDs;

            public Visitor(
                Dictionary<string, int> fieldSectionMapping,
                IDictionary<Guid, string> fieldIDs)
            {
                this.fieldIDs = fieldIDs;
                this.fieldSectionMapping = fieldSectionMapping;
            }
            
            public int Index { get; set; }

            /// <inheritdoc />
            protected override void VisitControl(
                IControlViewModel controlViewModel)
            {
                if (controlViewModel.CardTypeControl is CardTypeEntryControl textBoxControl
                    && textBoxControl.Type == CardControlTypes.String)
                {
                    foreach (var colID in textBoxControl.PhysicalColumnIDList)
                    {
                        this.fieldSectionMapping[this.fieldIDs[colID]] = this.Index;
                    }
                }
            }

            /// <inheritdoc />
            protected override void VisitBlock(
                IBlockViewModel blockViewModel)
            {
            }
        }

        private readonly Dictionary<string, int> fieldTabMapping = new Dictionary<string, int>();
        
        private readonly List<List<string>> tabFieldsMapping;
        
        private readonly List<IFormViewModel> tabs = new List<IFormViewModel>();

        private readonly IBlockViewModel blockViewModel;

        private readonly List<string> originalTabNames = new List<string>();

        private readonly string originalBlockName;

        private readonly IDictionary<string, object> fieldsStorage;

        private readonly List<bool> hasContent;
        
        public TabContentIndicator(
            TabControlViewModel tabControl,
            IDictionary<string, object> fieldsStorage,
            IDictionary<Guid, string> fieldIDs,
            bool updateBlockHeader = false)
        {
            this.fieldsStorage = fieldsStorage;
            if (updateBlockHeader)
            {
                this.blockViewModel = tabControl.Block;
                this.originalBlockName = this.blockViewModel.Caption;
            }

            var visitor = new Visitor(this.fieldTabMapping, fieldIDs);
            for (var i = 0; i < tabControl.Tabs.Count; i++)
            {
                var tab = tabControl.Tabs[i];
                this.tabs.Add(tab);
                this.originalTabNames.Add(tab.TabCaption);
                visitor.Index = i;
                visitor.Visit(tab);
            }

            this.tabFieldsMapping =
                this.fieldTabMapping
                    .GroupBy(p => p.Value)
                    .Select(p => new { tabOrder = p.Key, fields = p.Select(q => q.Key).ToList()})
                    .OrderBy(p => p.tabOrder)
                    .Select(p => p.fields)
                    .ToList();

            hasContent = this.originalTabNames.Select(_ => false).ToList();
        }


        public void Update()
        {
            for (var i = 0; i < this.tabs.Count; i++)
            {
                hasContent[i] = this.UpdateTabName(i);
            }

            if (this.blockViewModel != null)
            {
                this.blockViewModel.Caption = this.hasContent.Any(p => p) 
                    ? LocalizationManager.Format("$KrProcess_TabContainsText", this.originalBlockName) 
                    : this.originalBlockName;
            }
        }

        public void FieldChangedAction(object s, CardFieldChangedEventArgs e)
        {
            if (!this.fieldTabMapping.TryGetValue(e.FieldName, out var index))
            {
                return;
            }

            hasContent[index] = this.UpdateTabName(index);
            
            if (this.blockViewModel != null)
            {
                this.blockViewModel.Caption = this.hasContent.Any(p => p) 
                    ? LocalizationManager.Format("$KrProcess_TabContainsText", this.originalBlockName) 
                    : this.originalBlockName;
            }
        }
        
        private bool UpdateTabName(int index)
        {
            var tab = this.tabs[index];
            var fields = this.tabFieldsMapping[index];
            foreach (var field in fields)
            {
                if (!string.IsNullOrWhiteSpace(this.fieldsStorage.Get<string>(field)))
                {
                    tab.TabCaption = LocalizationManager.Format("$KrProcess_TabContainsText", this.originalTabNames[index]);
                    return true;
                }
            }

            tab.TabCaption = this.originalTabNames[index];
            return false;
        }
        
    }
}