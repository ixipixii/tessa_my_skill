﻿using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards.Caching;
using Tessa.Localization;
using Tessa.Notices;

namespace Tessa.Extensions.Default.Server.Notices
{
    /// <inheritdoc />
    public sealed class KrNotificationDefaultLanguagePicker : INotificationDefaultLanguagePicker
    {
        #region Fields

        private readonly ICardCache cardCache;

        #endregion

        #region Constructors

        public KrNotificationDefaultLanguagePicker(ICardCache cardCache)
        {
            this.cardCache = cardCache;
        }

        #endregion

        #region INotificationDefaultLanguagePicker Implementation

        /// <inheritdoc />
        public async ValueTask<string> GetDefaultLanguageAsync(CancellationToken cancellationToken = default)
        {
            string result;

            try
            {
                result = (await cardCache.Cards.GetAsync("KrSettings", cancellationToken))
                    .Entries
                    .Get<string>("KrSettings", "NotificationsDefaultLanguageCode");
            }
            catch (SingletonNotFoundInCacheException)
            {
                result = null;
            }

            return string.IsNullOrEmpty(result) ? LocalizationManager.EnglishLanguageCode : result;
        }

        #endregion
    }
}
