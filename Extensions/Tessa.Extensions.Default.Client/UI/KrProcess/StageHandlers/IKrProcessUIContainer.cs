using System;
using System.Collections.Generic;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public interface IKrProcessUIContainer
    {
        IKrProcessUIContainer RegisterUIHandler<T>() where T : IStageTypeUIHandler;
        
        IKrProcessUIContainer RegisterUIHandler<T>(
            StageTypeDescriptor descriptor) where T : IStageTypeUIHandler;

        IKrProcessUIContainer RegisterUIHandler(
            Type handlerType);
        
        IKrProcessUIContainer RegisterUIHandler(
            StageTypeDescriptor descriptor,
            Type handlerType);

        List<IStageTypeUIHandler> ResolveUIHandlers(Guid descriptorID);
    }
}