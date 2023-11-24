using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared.Workflow.Wf;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Запрещаем удаление карточки-сателлита для задания.
    /// Надо зарегистрировать после расширений, расчитывающий Permission-ы по правилам доступа.
    /// </summary>
    public class WfTaskSatellitePermissionsGetExtension :
        TaskSatellitePermissionsGetExtension
    {
        #region Base Overrides

        /// <doc path='info[@type="TaskSatellitePermissionsGetExtension" and @item="FileIsExternalKey"]'/>
        protected override string FileIsExternalKey => WfHelper.FileIsExternalKey;

        /// <doc path='info[@type="TaskSatellitePermissionsGetExtension" and @item="CanModifyTaskCardAsync"]'/>
        protected override async ValueTask<bool> CanModifyTaskCardAsync(ICardGetExtensionContext context, Card satellite) =>
            WfHelper.CanModifyTaskCard(satellite);

        #endregion
    }
}
