using System;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    /// <summary>
    /// Элемент истории выполнения этапов.
    /// </summary>
    public sealed class KrProcessTraceItem
    {
        public KrProcessTraceItem(
            Stage stage,
            StageHandlerResult? result,
            Guid? cardID)
        {
            this.Stage = new Stage(stage);
            this.Result = result;
            this.CardID = cardID;
        }

        /// <summary>
        /// Копия выполненного этапа.
        /// </summary>
        public Stage Stage { get; }

        /// <summary>
        /// Результат выполнения этапа.
        /// </summary>
        public StageHandlerResult? Result { get; }

        /// <summary>
        /// Идентификатор карточки, в рамках которой выполнялся этап.
        /// </summary>
        public Guid? CardID { get; }

        /// <summary>
        /// Признак того, что этап был прерван.
        /// </summary>
        public bool Interrupted => !this.Result.HasValue;
    }
}