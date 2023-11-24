using System;
using System.Collections.Generic;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrObjectModel
{
    public sealed class ProcessHolder
    {
        public string MainProcessType { get; set; }
        
        public bool Persistent { get; set; }
        
        public Guid ProcessHolderID { get; set; }
        
        public WorkflowProcess MainWorkflowProcess { get; set; }

        public Dictionary<Guid, WorkflowProcess> NestedWorkflowProcesses { get; } =
            new Dictionary<Guid, WorkflowProcess>();

        public MainProcessCommonInfo PrimaryProcessCommonInfo { get; set; }
        
        public MainProcessCommonInfo MainProcessCommonInfo { get; set; }
        
        public HashSet<Guid, NestedProcessCommonInfo> NestedProcessCommonInfos { get; set; }

        public List<NestedProcessCommonInfo> NestedProcessCommonInfosList
        {
            set
            {
                if (value is null)
                {
                    this.NestedProcessCommonInfos = null;
                }
                else
                {
                    this.NestedProcessCommonInfos = 
                        new HashSet<Guid, NestedProcessCommonInfo>(x => x.NestedProcessID, value);
                }
            }
        }

    }
}