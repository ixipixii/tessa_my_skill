using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public interface IKrStageRowExtension : IExtension
    {
        /// <summary>
        /// Выполняется перед началом сериализации настроек этапа.
        /// </summary>
        /// <param name="context"></param>
        Task BeforeSerialization(IKrStageRowExtensionContext context);

        /// <summary>
        /// Выполняется после десериализации, но перед восстановлением строки этапа.
        /// В карточке на восстановление доступны строки с полями, перенесенными из KrStages,
        /// даже если в KrStagesVirtual они отсутствуют
        /// </summary>
        /// <param name="context"></param>
        Task DeserializationBeforeRepair(IKrStageRowExtensionContext context);

        /// <summary>
        /// Выполняется после десериализации и после восстановления строки этапа.
        /// </summary>
        /// <param name="context"></param>
        Task DeserializationAfterRepair(IKrStageRowExtensionContext context);
    }
}