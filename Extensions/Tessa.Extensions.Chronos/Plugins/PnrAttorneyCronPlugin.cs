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
    /// Пример плагина, который может работать через серверное API.
    /// </summary>
    [Plugin(
        Name = "PnrAttorneyCronPlugin",
        Description = "Плагин проверяет даты окончания действия доверенностей и при выходе срока меняет статус доверенности на [Не действует]",
        Version = 1,
        ConfigFile = ConfigFilePath)]
    public sealed class PnrAttorneyCronPlugin :
        Plugin
    {
        #region Constants

        /// <summary>
        /// Относительный путь к конфигурационному файлу плагина.
        /// </summary>
        private const string ConfigFilePath = "configuration/PnrAttorneyCronPlugin.xml";

        #endregion

        #region Fields

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Base Overrides

        public override async Task EntryPointAsync(CancellationToken cancellationToken = default)
        {
            logger.Trace("Starting plugin");

            // конфигурируем контейнер Unity для использования стандартных серверных API (в т.ч. API карточек)
            // а также для получения прямого доступа к базе данных через IDbScope по строке подключения из app.config;
            // предполагаем, что все действия, совершаемые плагином, будут выполняться от имени пользователя System
            logger.Trace("Configuring container");

            TessaPlatform.InitializeFromConfiguration();

            IUnityContainer container = await new UnityContainer()
                // дополнительные регистрации в контейнере Unity могут быть здесь:
                // .RegisterType<MyClass>(new ContainerControlledLifetimeManager())

                .RegisterServerForPluginAsync()
                ;

            // container
            //     .RegisterType<MyService>();

            // любая полезная работа плагина может быть здесь
            logger.Trace("Doing useful stuff here");

            IDbScope dbScope = container.Resolve<IDbScope>();
            await using (dbScope.Create())
            {
                // работа в пределах одного SQL-соединения, транзакция при этом явно не создаётся

                if (this.StopRequested)
                {
                    // была запрошена асинхронная остановка, можно периодически проверять значение этого свойства,
                    // и консистентно завершать выполнение (закрыть транзакцию, если была открыта, и др.)
                    return;
                }

                var db = dbScope.Db;
                var result =await db
                    .SetCommand("UPDATE  [FdSatelliteCommonInfo] " +
                                "SET StateID = '24293b1c-8627-4443-8792-223f6a4e6cca', StateName = 'Не действует' " +
                                "FROM[FdSatelliteCommonInfo][s] " +
                                "INNER JOIN PnrPowerAttorney a " +
                                "ON a.ID = s.MainCardId " +
                                "WHERE a.EndDate IS NOT NULL AND a.EndDate < GETDATE()")
                    .LogCommand()
                    .ExecuteNonQueryAsync(cancellationToken);

                if (result == 1)
                {
                    logger.Trace("Update attorney successful");
                }
            }
            logger.Trace("Shutting down");
        }
        #endregion
    }
}
