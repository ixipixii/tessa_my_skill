using System;
using Tessa.Cards;
using Tessa.Files;
using Tessa.Workflow.Actions.Descriptors;

namespace Tessa.Extensions.Default.Shared.Workflow.WorkflowEngine
{
    public static class KrDescriptors
    {

        public static readonly WorkflowActionDescriptor KrChangeStateDescriptor =
            new WorkflowActionDescriptor(DefaultCardTypes.KrChangeStateActionTypeID)
            {
                Group = "$KrActions_StandardSolutionGroup",
                Icon = "Thin248",
                Order = 99,
            };

        public static readonly WorkflowActionDescriptor CreateCardDescriptor =
            new WorkflowActionDescriptor(DefaultCardTypes.WorkflowCreateCardActionTypeID)
            {
                Icon = "Thin1",
                Methods = new[]
                {
                    new WorkflowActionMethodDescriptor()
                    {
                        ErrorDescription = "$WorkflowEngine_Actions_CreateCard_CompilationError",
                        MethodName = "InitCard",
                        Parameters = new[]
                        {
                            new Tuple<string, string>("dynamic", "newCard"),
                            new Tuple<string, string>("dynamic", "newCardTables"),
                            new Tuple<string, string>(nameof(Card), "newCardObject"),
                            new Tuple<string, string>(nameof(IFileContainer), "newCardFileContainer"),
                        },
                        StorePath = new[] { "KrCreateCardAction", "Script" },
                    }
                },
            };

        public static readonly WorkflowActionDescriptor AcquaintanceDescriptor =
            new WorkflowActionDescriptor(DefaultCardTypes.KrAcquaintanceActionTypeID)
            {
                Group = "$KrActions_StandardSolutionGroup",
                Icon = "Thin83",
            };

        public static readonly WorkflowActionDescriptor RegistrationDescriptor =
            new WorkflowActionDescriptor(DefaultCardTypes.KrRegistrationActionTypeID)
            {
                Group = "$KrActions_StandardSolutionGroup",
                Icon = "Thin325",
            };

        public static readonly WorkflowActionDescriptor DeregistrationDescriptor =
            new WorkflowActionDescriptor(DefaultCardTypes.KrDeregistrationActionTypeID)
            {
                Group = "$KrActions_StandardSolutionGroup",
                Icon = "Thin325",
            };
    }
}
