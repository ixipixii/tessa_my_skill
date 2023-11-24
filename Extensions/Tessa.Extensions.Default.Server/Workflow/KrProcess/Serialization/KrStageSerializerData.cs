using System.Collections.Generic;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public sealed class KrStageSerializerData
    {
        /// <summary>
        /// Список секций-настроек.
        /// </summary>
        public List<string> SettingsSectionNames { get; } = new List<string>();

        /// <summary>
        /// Список полей-настроек.
        /// </summary>
        public List<string> SettingsFieldNames { get; } = new List<string>();

        /// <summary>
        /// Список прямых детей секции этапа.
        /// </summary>
        public List<ReferenceToStage> ReferencesToStages { get; } = new List<ReferenceToStage>();

        /// <summary>
        /// Список секция и столбцов, по которым в табличных контролах проводится сортировка
        /// </summary>
        public List<OrderColumn> OrderColumns { get; } = new List<OrderColumn>();

    }
}