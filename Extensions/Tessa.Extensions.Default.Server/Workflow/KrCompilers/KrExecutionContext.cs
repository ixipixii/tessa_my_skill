using System;
using System.Collections.Generic;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Контекст выполнения методов Before, After, Condition
    /// </summary>
    public sealed class KrExecutionContext : IKrExecutionContext
    {
        #region constructor

        private KrExecutionContext()
        {
        }

        public KrExecutionContext(
            ICardExtensionContext cardContext,
            IMainCardAccessStrategy mainCardAccessStrategy,
            Guid? cardID,
            Guid? cardTypeID,
            string cardTypeName,
            string cardTypeCaption,
            Guid? docTypeID,
            KrComponents? krComponents,
            WorkflowProcess workflowProcess,
            IKrCompilationResult compilationResult,
            IKrSecondaryProcess secondaryProcess = null,
            IEnumerable<Guid> executionUnits = null,
            Guid? groupID = null)
        {
            this.CardContext = cardContext;
            this.MainCardAccessStrategy = mainCardAccessStrategy;
            this.CardID = cardID;
            this.CardTypeID = cardTypeID;
            this.DocTypeID = docTypeID;
            this.KrComponents = krComponents;
            this.TypeID = docTypeID ?? cardTypeID;
            this.WorkflowProcess = workflowProcess;
            this.CompilationResult = compilationResult;
            this.GroupID = groupID;
            this.SecondaryProcess = secondaryProcess;

            this.ExecutionUnitIDs = executionUnits is null
                ? null
                : new HashSet<Guid>(executionUnits);
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public bool ExecuteAll => this.ExecutionUnitIDs == null;

        /// <inheritdoc />
        public HashSet<Guid> ExecutionUnitIDs { get; private set; }

        /// <inheritdoc />
        public IMainCardAccessStrategy MainCardAccessStrategy { get; private set; }

        /// <inheritdoc />
        public Guid? CardID { get; private set; }

        /// <inheritdoc />
        public Guid? CardTypeID { get; private set; }

        /// <inheritdoc />
        public string CardTypeName { get; private set; }

        /// <inheritdoc />
        public string CardTypeCaption { get; private set; }

        /// <inheritdoc />
        public Guid? DocTypeID { get; private set; }

        /// <inheritdoc />
        public Guid? TypeID { get; private set; }

        /// <inheritdoc />
        public KrComponents? KrComponents { get; private set; }

        /// <inheritdoc />
        public WorkflowProcess WorkflowProcess { get; private set; }

        /// <inheritdoc />
        public ICardExtensionContext CardContext { get; private set; }

        /// <inheritdoc />
        public IKrCompilationResult CompilationResult { get; private set; }

        /// <inheritdoc />
        public IKrSecondaryProcess SecondaryProcess { get; private set; }

        /// <inheritdoc />
        public Guid? GroupID { get; private set; }

        /// <inheritdoc />
        public IKrExecutionContext Copy(
            IEnumerable<Guid> executionUnits = null)
        {
            var newContext = this.CopyInternal();
            newContext.ExecutionUnitIDs = executionUnits is null
                ? null
                : new HashSet<Guid>(executionUnits);
            return newContext;
        }

        /// <inheritdoc />
        public IKrExecutionContext Copy(
            Guid? groupID,
            IEnumerable<Guid> executionUnits = null)
        {
            var newContext = this.CopyInternal();
            newContext.GroupID = groupID;
            newContext.ExecutionUnitIDs = executionUnits is null
                ? null
                : new HashSet<Guid>(executionUnits);
            return newContext;
        }

        /// <inheritdoc />
        public IKrExecutionContext Copy(
            IKrCompilationResult compilationCache,
            IEnumerable<Guid> executionUnits = null)
        {
            var newContext = this.CopyInternal();
            newContext.CompilationResult = compilationCache;
            newContext.ExecutionUnitIDs = executionUnits is null
                ? null
                : new HashSet<Guid>(executionUnits);
            return newContext;
        }

        #endregion

        #region private

        private KrExecutionContext CopyInternal()
        {
            var newContext = new KrExecutionContext
            {
                CardContext = this.CardContext,
                MainCardAccessStrategy = this.MainCardAccessStrategy,
                CardID = this.CardID,
                CardTypeID = this.CardTypeID,
                CardTypeName = this.CardTypeName,
                CardTypeCaption = this.CardTypeCaption,
                DocTypeID = this.DocTypeID,
                TypeID = this.DocTypeID ?? this.CardTypeID,
                KrComponents = this.KrComponents,
                SecondaryProcess = this.SecondaryProcess,
                WorkflowProcess = this.WorkflowProcess,
                CompilationResult = this.CompilationResult,
                GroupID = this.GroupID,
            };

            return newContext;
        }

        #endregion
    }
}
