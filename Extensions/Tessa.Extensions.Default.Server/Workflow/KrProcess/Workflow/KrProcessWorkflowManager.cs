using System;
using System.Threading;
using Tessa.Cards.Workflow;
using Tessa.Extensions.Default.Shared.Settings;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public sealed class KrProcessWorkflowManager : WorkflowManager
    {
        #region fields

        private readonly KrSettingsLazy settingsLazy;

        private readonly Lazy<Guid> satelliteIDLazy;

        private Guid? secondarySatelliteID;

        #endregion

        #region constructor

        public KrProcessWorkflowManager(
            KrProcessWorkflowContext context,
            IWorkflowQueueProcessor queueProcessor)
            : base(context, queueProcessor)
        {
            this.WorkflowContext = context;

            this.settingsLazy = context.SettingsLazy;
            this.satelliteIDLazy = new Lazy<Guid>(
                () => context.KrScope.GetKrSatellite(context.CardID).ID,
                LazyThreadSafetyMode.PublicationOnly);
        }

        #endregion

        #region override

        protected override Guid WorkflowCardID => this.secondarySatelliteID ?? this.satelliteIDLazy.Value;

        #endregion

        #region properties

        public KrProcessWorkflowContext WorkflowContext { get; }

        public KrSettings Settings => this.settingsLazy.GetValueAsync().GetAwaiter().GetResult(); // TODO async

        #endregion

        #region methods

        /// <summary>
        /// Задаёт ИД карточки сателлита вторичного процесса.
        /// </summary>
        /// <param name="secSatID">ИД карточки сателлита вторичного процесса.</param>
        public void SpecifySatelliteID(
            Guid secSatID)
        {
            this.SpecifySatelliteID(secSatID, true);
        }

        /// <summary>
        /// Задаёт ИД карточки сателлита вторичного процесса.
        /// </summary>
        /// <param name="secSatID">ИД карточки сателлита вторичного процесса.</param>
        /// <param name="isCheckSetSecondarySatelliteID">Проверять установлено ли значение ИД карточки сателлита вторичного процесса.</param>
        internal void SpecifySatelliteID(
            Guid secSatID,
            bool isCheckSetSecondarySatelliteID)
        {
            if (this.satelliteIDLazy.IsValueCreated)
            {
                throw new InvalidOperationException("Main satellite has already been used.");
            }

            if (isCheckSetSecondarySatelliteID && this.secondarySatelliteID.HasValue)
            {
                throw new InvalidOperationException("Secondary satellite ID already speciifed.");
            }

            this.secondarySatelliteID = secSatID;
        }

        #endregion
    }
}