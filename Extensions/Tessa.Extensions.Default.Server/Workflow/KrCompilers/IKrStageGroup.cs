using System;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrStageGroup : IRuntimeSources, IDesignTimeSources
    {
        /// <summary>
        /// ID карточки KrStageGroup.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Имя группы этапов KrStageGroup.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Порядок группы этапов.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Можно ли менять содержимое.
        /// </summary>
        bool IsGroupReadonly { get; }
        
        /// <summary>
        /// Идентификатор вторичного процесса, к которому привязан процесс.
        /// </summary>
        Guid? SecondaryProcessID { get; }
    }
}