using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards.Caching;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Shared.Settings
{
    public sealed class PnrCommonSettings
    {
        readonly ICardCache cardCache;

        public PnrCommonSettings(ICardCache cardCache)
        {
            this.cardCache = cardCache;
        }

        /// <summary>
        /// Адрес сервиса для передачи договора и заявки на КА в MDM (НСИ)
        /// </summary>
        public async Task<string> GetMdmServiceUrl()
        {
            var settings = await cardCache.Cards.GetAsync("PnrCommonSettings");
            if (settings != null &&
                settings.Sections.TryGetValue("PnrCommonSettings", out var settingsSection))
            {
                return settingsSection.Fields.TryGet<string>("MdmServiceUrl");
            }
            return null;
        }
    }
}
