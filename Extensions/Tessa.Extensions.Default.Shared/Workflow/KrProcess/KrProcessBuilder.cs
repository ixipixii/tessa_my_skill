using System;
using System.Collections.Generic;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public sealed class KrProcessBuilder
    {
        #region fields

        private Guid processID;

        private Guid? cardID;
        
        private IDictionary<string, object> processInfo;
        
        private Guid? processHolderID;

        private string parentProcessTypeName;
        
        private Guid? parentProcessID;
        
        private Guid? parentStageRowID;
        
        private int? nestedOrder;

        #endregion

        #region constructor

        private KrProcessBuilder()
        {

        }

        private KrProcessBuilder(KrProcessInstance instance)
        {
            this.processID = instance.ProcessID;
            this.cardID = instance.CardID;
            this.processInfo = StorageHelper.Clone(instance.ProcessInfo);
            this.processHolderID = instance.ProcessHolderID;
        }

        #endregion

        #region public

        public KrProcessBuilder SetProcess(
            Guid pID)
        {
            this.processID = pID;
            return this;
        }

        public KrProcessBuilder SetCard(
            Guid id)
        {
            this.cardID = id;
            return this;
        }

        public KrProcessBuilder SetProcessInfo(
            IDictionary<string, object> info)
        {
            this.processInfo = info;
            return this;
        }

        public KrProcessBuilder SetNestedProcess(
            Guid processHolder,
            string parentProcessTypeName,
            Guid? parentProcessID,
            Guid parentStageRow,
            int nestedOrder)
        {
            this.processHolderID = processHolder;
            this.parentProcessTypeName = parentProcessTypeName;
            this.parentProcessID = parentProcessID;
            this.parentStageRowID = parentStageRow;
            this.nestedOrder = nestedOrder;
            return this;
        }

        public KrProcessInstance Build()
        {
            return new KrProcessInstance(
                this.processID, 
                this.cardID,
                this.processInfo,
                this.processHolderID,
                this.parentProcessTypeName,
                this.parentProcessID,
                this.parentStageRowID,
                this.nestedOrder);
        }

        #endregion

        #region static

        public static KrProcessBuilder CreateProcess() => new KrProcessBuilder();

        public static KrProcessBuilder ModifyProcess(KrProcessInstance instance) => new KrProcessBuilder(instance);

        #endregion

    }
}