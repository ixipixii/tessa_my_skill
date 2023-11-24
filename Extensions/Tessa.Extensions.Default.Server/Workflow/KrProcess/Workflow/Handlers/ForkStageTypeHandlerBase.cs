using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public abstract class ForkStageTypeHandlerBase: StageTypeHandlerBase
    {
        
        protected static readonly IStorageValueFactory<int, BranchAdditionInfo> BranchAdditionInfoFactory =
            new DictionaryStorageValueFactory<int, BranchAdditionInfo>(
                (key, storage) => new BranchAdditionInfo(storage));
        
        protected static readonly IStorageValueFactory<int, BranchRemovalInfo> BranchRemovalInfoFactory =
            new DictionaryStorageValueFactory<int, BranchRemovalInfo>(
                (key, storage) => new BranchRemovalInfo(storage));
        
        /// <inheritdoc />
        public override void BeforeInitialization(
            IStageTypeHandlerContext context)
        {
            var infos = new Dictionary<string, object>();
            
            foreach (var secondaryProcessRow in EnumerateSecondaryProcessRows(context))
            {
                var rowID = secondaryProcessRow.TryGet<Guid>(KrConstants.RowID);
                SetProcessInfo(infos, rowID, new Dictionary<string, object>());
            }

            context.Stage.InfoStorage[KrConstants.Keys.ForkNestedProcessInfo] = infos;
        }
        
        public override void AfterPostprocessing(
            IStageTypeHandlerContext context)
        {
            context.Stage.InfoStorage.Remove(KrConstants.Keys.ForkNestedProcessInfo);
        }
        
        protected static IDictionary<string, object> GetProcessInfos(Stage stage)
        {
            if (stage.InfoStorage.TryGetValue(KrConstants.Keys.ForkNestedProcessInfo, out var processInfosObj)
                && processInfosObj is IDictionary<string, object> processInfos)
            {
                return processInfos;
            }
            return new Dictionary<string, object>();
        }
        
        protected static IDictionary<string, object> GetProcessInfo(
            IDictionary<string, object> processInfos,
            Guid rowID)
        {
            if (processInfos.TryGetValue(rowID.ToString("D"), out var processInfoObj)
                && processInfoObj is IDictionary<string, object> processInfo)
            {
                return processInfo;
            }
            return new Dictionary<string, object>();
        }

        protected static void SetProcessInfo(
            IDictionary<string, object> processInfos,
            Guid rowID,
            IDictionary<string, object> processInfo)
        {
            processInfos[rowID.ToString("D")] = processInfo;
        }

        
        protected static IEnumerable<IDictionary<string, object>> EnumerateSecondaryProcessRows(
            IStageTypeHandlerContext context)
        {
            var secondaryProcesses = 
                context
                    .Stage
                    .SettingsStorage
                    .TryGet<List<object>>(KrConstants.KrForkSecondaryProcessesSettingsVirtual.Synthetic);
            
            if (secondaryProcesses is null)
            {
                yield break;
            }

            foreach (var secProcRow in secondaryProcesses)
            {
                if (secProcRow is IDictionary<string, object> dict)
                {
                    yield return dict;
                }
            }
        }
    }
}