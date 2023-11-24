using System;
using System.Threading;
using System.Threading.Tasks;
using Chronos.Contracts;
using NLog;
using Tessa.Platform;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Chronos.Plugins
{
    /// <summary>
    /// Плагин для изменения состояний КА по истечению срока действия.
    /// </summary>
    [Plugin(
        Name = "PnrRefreshStatePartnerFromValidityPlugin",
        Description = "Плагин для изменения состояний КА по истечению срока действия.",
        Version = 1,
        ConfigFile = ConfigFilePath)]
    public sealed class PnrRefreshStatePartnerFromValidityPlugin : Plugin
    {
        #region Constants

        /// <summary>
        /// Относительный путь к конфигурационному файлу плагина.
        /// </summary>
        private const string ConfigFilePath = "configuration/PnrRefreshStatePartnerFromValidityPlugin.xml";

        #endregion

        #region Fields

        private static readonly Logger Logger = LogManager.GetLogger("PnrRefreshStatePartnerFromValidityPlugin");

        #endregion


        #region IPlugin Members

        public override async Task EntryPointAsync(CancellationToken cancellationToken = default)
        {
            Logger.Trace("Starting plugin.");
            TessaPlatform.InitializeFromConfiguration();
            try
            {
                IUnityContainer container = await new UnityContainer().RegisterServerForPluginAsync();
                IDbScope dbScope = container.Resolve<IDbScope>();
                await UpdatePartnerStateAsync(dbScope);
                Logger.Trace("Shutdown plugin.");
            }
            catch (Exception e)
            {
                Logger.Error("Error:" + e.Message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Обновление состояний КА с вышедним сроком.
        /// </summary>
        private async Task UpdatePartnerStateAsync(IDbScope dbScope)
        {
            await using (dbScope.Create())
            {
                // работа в пределах одного SQL-соединения, транзакция при этом явно не создаётся

                if (this.StopRequested)
                {
                    // была запрошена асинхронная остановка, можно периодически проверять значение этого свойства,
                    // и консистентно завершать выполнение (закрыть транзакцию, если была открыта, и др.)
                    return;
                }

                DbManager db = dbScope.Db;
                // залогируем ID КА которых будем обновлять
                var partnerIds = await db
                .SetCommand(
                @"SELECT [ID] FROM [Partners]
                                WHERE [Validity] < GETUTCDATE()
                                AND [StatusID] = 0")
                .LogCommand()
                .ExecuteListAsync<Guid>();

                foreach (var partner in partnerIds)
                {
                    Logger.Trace($"Partner ID: {partner}");
                }

                // обновим статус КА
                await db
                        .SetCommand(
                        @"UPDATE [Partners] SET 
                        [StatusID] = 1,
                        [StatusName] = N'Не согласован'
                        WHERE [Validity] < GETUTCDATE()
                        AND [StatusID] = 0")
                        .LogCommand()
                        .ExecuteNonQueryAsync();
            }
        }
        #endregion
    }
}
