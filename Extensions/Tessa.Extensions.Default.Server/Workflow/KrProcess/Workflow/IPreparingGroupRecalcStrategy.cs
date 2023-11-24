using System;
using System.Collections;
using System.Collections.Generic;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public interface IPreparingGroupRecalcStrategy
    {
        bool Used { get; }

        IList<Guid> ExecutionUnits { get; }
        
        Stage GetSuitableStage(
            IList<Stage> stages);

        void Apply(
            IKrProcessRunnerContext context,
            Stage stage,
            Stage prevStage);
    }
}