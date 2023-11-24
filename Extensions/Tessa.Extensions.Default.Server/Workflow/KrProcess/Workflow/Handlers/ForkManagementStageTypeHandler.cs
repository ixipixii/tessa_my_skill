using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class ForkManagementStageTypeHandler: ForkStageTypeHandlerBase
    {
        #region nested types

        public enum ForkManagementMode
        {
            Add = 0,
            Remove = 1,
        }

        #endregion

        #region base overrides

        /// <inheritdoc />
        public override StageHandlerResult HandleStageStart(
            IStageTypeHandlerContext context)
        {
            var managePrimaryProcessFlag = context.Stage.SettingsStorage
                .TryGet<bool?>(KrConstants.KrForkManagementSettingsVirtual.ManagePrimaryProcess) ?? false;
            var managePrimaryProcess = managePrimaryProcessFlag
                && context.ProcessInfo?.ProcessTypeName != KrConstants.KrProcessName;
            Guid? processID;
            string processType;
            if (managePrimaryProcess)
            {
                processType = KrConstants.KrProcessName;
                processID = null;
            }
            else
            {
                processType = context.ParentProcessTypeName;
                processID = context.ParentProcessID;
            }

            var modeInt = context.Stage.SettingsStorage.TryGet<int?>(KrConstants.KrForkManagementSettingsVirtual.ModeID);

            if (!modeInt.HasValue
                || !(0 <= modeInt && modeInt <= (int)ForkManagementMode.Remove))
            {
                context.ValidationResult.AddError(this, "$KrStages_ForkManagement_ModeNotSpecified");
                return StageHandlerResult.SkipResult;
            }

            switch ((ForkManagementMode) modeInt)
            {
                case ForkManagementMode.Add:
                    this.Add(processType, processID, context);
                    break;
                case ForkManagementMode.Remove:
                    this.Remove(processType, processID, context);
                    break;
            }
            
            return StageHandlerResult.CompleteResult;
        }

        #endregion
        
        #region protected

        protected void Add(
            string processType,
            Guid? processID,
            IStageTypeHandlerContext context)
        {
            var branchAdditionInfos = new ListStorage<BranchAdditionInfo>(new List<object>(), BranchAdditionInfoFactory);
            var processInfos = GetProcessInfos(context.Stage);

            foreach (var storage in EnumerateSecondaryProcessRows(context))
            {
                var rowID = storage.TryGet<Guid>(KrConstants.RowID);
                var spID = storage.TryGet<Guid>(
                        KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessID);
                var spName = storage.TryGet<string>(
                    KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessName);
                var processInfo = GetProcessInfo(processInfos, rowID);

                branchAdditionInfos.Add(new BranchAdditionInfo(spID, spName, processInfo.ToDictionaryStorage()));
            }
            
            context.MainCardAccessStrategy
                .GetCard()
                .GetWorkflowQueue()
                .AddSignal(
                    processType,
                    KrConstants.ForkAddBranchSignal,
                    processID: processID,
                    parameters: new Dictionary<string, object> { [nameof(BranchAdditionInfo)] = branchAdditionInfos.GetStorage(), });
        }
        
        protected void Remove(
            string processType,
            Guid? processID,
            IStageTypeHandlerContext context)
        {
            var secondaryProcesses = EnumerateSecondaryProcessRows(context)
                .Select(p => p.TryGet<Guid>(KrConstants.KrForkSecondaryProcessesSettingsVirtual.SecondaryProcessID))
                .ToList();
            var nestedProcesses = context
                .Stage
                .SettingsStorage
                .TryGet<List<object>>(KrConstants.KrForkNestedProcessesSettingsVirtual.Synthetic)
                .Select(p => ((IDictionary<string, object>)p).TryGet<Guid>(KrConstants.KrForkNestedProcessesSettingsVirtual.NestedProcessID))
                .ToList();
            
            var directionAfterInterrupt = (DirectionAfterInterrupt)context.Stage.SettingsStorage
                .TryGet<int>(KrConstants.KrForkManagementSettingsVirtual.DirectionAfterInterrupt);
            var branchRemovalInfo = new BranchRemovalInfo(secondaryProcesses, nestedProcesses, directionAfterInterrupt);
            
            context.MainCardAccessStrategy
                .GetCard()
                .GetWorkflowQueue()
                .AddSignal(
                    processType,
                    KrConstants.ForkRemoveBranchSignal,
                    processID: processID,
                    parameters: new Dictionary<string, object> { [nameof(BranchRemovalInfo)] = branchRemovalInfo.GetStorage(), });
        }
        
        #endregion
        
    }
}