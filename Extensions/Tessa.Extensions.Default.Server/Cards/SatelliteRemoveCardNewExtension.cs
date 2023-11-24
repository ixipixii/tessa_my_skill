using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Forums;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Расширение удаляющее из карточки шаблона информацию о ненужных сателлитах.
    /// </summary>
    public class SatelliteRemoveCardNewExtension : CardNewExtension
    {
        #region Constants

        private static string templateCardKey = CardHelper.SystemKeyPrefix + "templateCard";

        #endregion

        #region Base Overrides

        public override Task BeforeRequest(ICardNewExtensionContext context)
        {
            object templateCardObj = null;

            var requestInfo = context.Request.TryGetInfo();

            if (requestInfo?.TryGetValue(templateCardKey, out templateCardObj) == false)
            {
                return Task.CompletedTask;
            }

            var templateCard = new Card((Dictionary<string, object>)templateCardObj);

            templateCard.Info.Remove(WfHelper.SatelliteKey);
            templateCard.Info.Remove(WfHelper.TaskSatelliteListKey);
            templateCard.Info.Remove(FmHelper.FmSatelliteInfoKey);
            templateCard.Info.Remove(KrConstants.KrDialogSatelliteListInfoKey);

            requestInfo[templateCardKey] = templateCard.GetStorage();

            return Task.CompletedTask;
        }

        #endregion
    }
}