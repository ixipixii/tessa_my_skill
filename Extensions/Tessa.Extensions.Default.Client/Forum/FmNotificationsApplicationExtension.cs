using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Platform.Licensing;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.UI;
using Tessa.UI.Controls.Forums;

namespace Tessa.Extensions.Default.Client.Forum
{
    internal class FmNotificationsApplicationExtension : ApplicationExtension
    {
        #region Private Fields

        private Timer timer;

        private readonly INotificationButtonUIManager manager;

        private readonly IUserSettings userSettings;

        private readonly ICardCache cardCache;

        private readonly ILicenseManager licenseManager;

        private int forumRefreshInterval;

        #endregion

        #region Constructors

        public FmNotificationsApplicationExtension(
            INotificationButtonUIManager manager,
            IUserSettings userSettings,
            ICardCache cardCache,
            ILicenseManager licenseManager)
        {
            this.manager = manager;
            this.userSettings = userSettings;
            this.cardCache = cardCache;
            this.licenseManager = licenseManager;
        }

        #endregion

        #region Private Methods

        private async void CheckNotificationsAsync()
        {
            // Обрабатываем случай, у пользователя были включены уведомления и он их отключил.
            // Таймер крутится, но не обновляет ленту уведомлений.
            if (!this.userSettings.EnableMessageIndicator)
            {
                return;
            }

            if (await this.manager.IsExistNotificationsAsync())
            {
                await this.manager.ShowNotificationButtonAsync(false);
            }
        }

        #endregion

        #region BaseOverride

        public override async Task Initialize(IApplicationExtensionContext context)
        {
            // проверяем this.userSettings.EnableMessageIndicator, если отключен, то выходим.
            // Но при этом надо учесть момент, что для того чтобы включить индикатор, необходим перезапуск приложения.

            if (LicensingHelper.CheckForumLicense(licenseManager.License, out _))
            {
                if (!this.userSettings.DoNotShowMessageIndicatorOnStartup &&
                    this.userSettings.EnableMessageIndicator &&
                    await this.manager.IsExistNotificationsAsync(context.CancellationToken))
                {
                    await this.manager.ShowNotificationButtonAsync(false, context.CancellationToken);
                }

                this.timer?.Dispose();

                this.forumRefreshInterval =
                    (await cardCache.Cards.GetAsync(CardHelper.ServerInstanceTypeName))
                    .Sections["ServerInstances"]
                    .RawFields
                    .Get<int>("ForumRefreshInterval");

                // таймер запускаем с задержкой в period, т.к. выше уже выполнялась проверка IsExistNotificationsAsync
                long period = forumRefreshInterval * 1000L;

                this.timer = new Timer(
                    p => this.CheckNotificationsAsync(),
                    null,
                    period,
                    period);
            }
        }

        public override async Task Shutdown(IApplicationExtensionContext context)
        {
            this.timer?.Dispose();
            this.timer = null;
        }

        #endregion
    }
}