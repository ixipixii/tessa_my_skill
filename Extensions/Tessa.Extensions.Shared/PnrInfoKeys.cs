using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared
{
    public static class PnrInfoKeys
    {
        /// <summary>
        /// Нужно ли пропускать кастомную валидацию при сохранении карточки
        /// </summary>
        public const string PnrSkipCardFieldsCustomValidation = nameof(PnrSkipCardFieldsCustomValidation);

        /// <summary>
        /// ID карточки контрагента для которого создается заявка на КА
        /// </summary>
        public const string PnrCreatePartnerRequestPartnerID = nameof(PnrCreatePartnerRequestPartnerID);

        /// <summary>
        /// Нужно ли отправить карточку в НСИ
        /// </summary>
        public const string PnrIsNeedSendCardToMDM = nameof(PnrIsNeedSendCardToMDM);
    }
}
