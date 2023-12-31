﻿using Chronos.Contracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tessa.Extensions.Default.Shared;
using Tessa.Notices;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Validation;
using Unity;
#pragma warning disable 1998

namespace Tessa.Extensions.Default.Chronos.Notices
{
    /// <summary>
    /// Плагин, выполняющий добавление уведомлений о необходимости обновить токен для подписи.
    /// </summary>
    [Plugin(
        Name = "Token notification plugin",
        Description = "Plugin send messages about token notifications.",
        Version = 1,
        ConfigFile = ConfigFilePath)]
    public sealed class TokenNotificationPlugin :
        Plugin
    {
        #region Constants

        /// <summary>
        /// Относительный путь к конфигурационному файлу плагина.
        /// </summary>
        public const string ConfigFilePath = "configuration/TokenNotifications.xml";

        #endregion

        #region Fields

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Base Overrides

        public override async Task EntryPointAsync(CancellationToken cancellationToken = default)
        {
            IPlugin plugin = this;
            XElement pluginConfig = plugin.TryLoadConfig();
            if (pluginConfig == null)
            {
                return;
            }

            logger.Trace("Starting token notification plugin.");

            TessaPlatform.InitializeFromConfiguration();

            IUnityContainer unityContainer = await new UnityContainer().RegisterServerForPluginAsync();
            var dbScope = unityContainer.Resolve<IDbScope>();
            var notificationManager = unityContainer.Resolve<INotificationManager>();

            logger.Trace("Collecting token notification receivers.");

            await using (dbScope.Create())
            {
                List<Guid> users = await SettingNotificationHelper.GetEmailSettingsAsync(dbScope, "NotificationTokenRoles", cancellationToken);
                if (users.Count > 0)
                {
                    var validationResult = new ValidationResultBuilder();
                    try
                    {
                        validationResult.Add(
                            await notificationManager
                                .SendAsync(
                                    DefaultNotifications.TokenNotification,
                                    users,
                                    new NotificationSendContext()
                                    {
                                        MainCardID = Session.SystemID,
                                        IgnoreUserSessions = true,
                                        ExcludeDeputies = true,
                                        DisableSubscribers = true,
                                    },
                                    cancellationToken));
                    }
                    finally
                    {
                        logger.LogResult(validationResult);
                    }
                }
            }

            logger.Trace("Token notification plugin completed.");
        }

        #endregion
    }
}
