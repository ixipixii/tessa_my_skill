using System;
using System.Collections.Generic;
using Tessa.Cards;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public interface IStageTasksRevoker
    {
        /// <summary>
        /// Завершить все задания этапа.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool RevokeAllStageTasks(IStageTasksRevokerContext context);

        /// <summary>
        /// Завершить задание <see cref="IStageTasksRevokerContext.TaskID"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool RevokeTask(IStageTasksRevokerContext context);

        /// <summary>
        /// Завершить все задания из <see cref="IStageTasksRevokerContext.TaskIDs"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        int RevokeTasks(IStageTasksRevokerContext context);
    }
}