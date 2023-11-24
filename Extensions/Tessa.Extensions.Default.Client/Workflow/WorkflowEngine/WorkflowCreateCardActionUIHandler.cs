using Tessa.Extensions.Default.Shared.Workflow.WorkflowEngine;
using Tessa.UI.Cards;
using Tessa.UI.WorkflowViewer.Actions;
using Tessa.Workflow;
using Tessa.Workflow.Helpful;
using Tessa.Workflow.Storage;

namespace Tessa.Extensions.Default.Client.Workflow.WorkflowEngine
{
    public sealed class WorkflowCreateCardActionUIHandler : WorkflowActionUIHandlerBase
    {
        #region Fields

        private const string MainSection = "KrCreateCardAction";

        #endregion

        #region Constructors

        public WorkflowCreateCardActionUIHandler()
            :base(KrDescriptors.CreateCardDescriptor)
        {
        }

        #endregion

        #region Base Overrides

        protected override void AttachToCardCore(WorkflowEngineBindingContext bindingContext)
        {
            if (bindingContext.ActionTemplate != null)
            {
                bindingContext.Section = bindingContext.Card.Sections.GetOrAdd(MainSection);

                AttachFieldToTemplate(
                    bindingContext,
                    "Script",
                    typeof(string),
                    MainSection,
                    "Script");
            }

            base.AttachToCardCore(bindingContext);
        }

        protected override void UpdateFormCore(
            WorkflowStorageBase action, 
            WorkflowStorageBase node, 
            WorkflowStorageBase process, 
            ICardModel cardModel, 
            WorkflowActionStorage actionTemplate = null)
        {
            var mainSection = cardModel.Card.Sections[MainSection];

            mainSection.FieldChanged += (s, e) =>
            {
                switch(e.FieldName)
                {
                    case "TemplateID":
                        if (e.FieldValue != null)
                        {
                            mainSection.Fields[WorkflowEngineHelper.BindingPrefix + "TypeID"] = null;
                            mainSection.Fields[WorkflowEngineHelper.BindingPrefix + "TypeCaption"] = null;
                            mainSection.Fields["TypeID"] = null;
                            mainSection.Fields["TypeCaption"] = null;
                        }
                        break;

                    case "TypeID":
                        if (e.FieldValue != null)
                        {
                            mainSection.Fields[WorkflowEngineHelper.BindingPrefix + "TemplateID"] = null;
                            mainSection.Fields[WorkflowEngineHelper.BindingPrefix + "TemplateDigest"] = null;
                            mainSection.Fields["TemplateID"] = null;
                            mainSection.Fields["TemplateDigest"] = null;
                        }
                        break;
                }
            };
        }

        #endregion

    }
}
