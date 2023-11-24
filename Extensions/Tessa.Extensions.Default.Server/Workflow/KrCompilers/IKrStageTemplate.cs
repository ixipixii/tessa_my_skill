using System;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Интерфейс, предоставляющий информацию об шаблоне этапов,
    /// необходимую для его компиляции и выполнения
    /// </summary>
    public interface IKrStageTemplate: IDesignTimeSources
    {
        /// <summary>
        /// ID карточки KrStageTemplates
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Имя шаблона
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ID группы этапов, к которой. относится шаблон.
        /// </summary>
        Guid StageGroupID { get; }

        /// <summary>
        /// Имя группы этапов, к которой относится шаблон.
        /// </summary>
        string StageGroupName { get; }

        /// <summary>
        /// Порядок шаблона
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Положение относительно этапов, добавленных вручную (в начале/в конце)
        /// </summary>
        GroupPosition Position { get; }

        /// <summary>
        /// Можно ли менять порядок этапа
        /// </summary>
        bool CanChangeOrder { get; }

        /// <summary>
        /// Можно ли менять содержимое этапа
        /// </summary>
        bool IsStagesReadonly { get; }
    }
}
