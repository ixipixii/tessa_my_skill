using System;
using Microsoft.Exchange.WebServices.Data;
using NLog;
using Tessa.Extensions.Default.Chronos.Notices;
using Tessa.Extensions.Default.Server.Notices;
using Tessa.Platform.Licensing;
using Tessa.Platform.Plugins;
using Unity;
using Unity.Lifetime;
using Task = System.Threading.Tasks.Task;

namespace Tessa.Extensions.Default.Chronos.Workflow
{
    public sealed class MobileApprovalPlugin : PluginExtension
    {
        #region Fields

        private MailingMode mode;
        private ExchangeServiceSettings exchangeServiceSettings;
        private Pop3ServiceSettings pop3ServiceSettings;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region MailingMode Enum

        private enum MailingMode
        {
            Exchange,
            Pop3,
            Disabled,
            Unknown,
        }

        private static MailingMode ParseMailingMode(string mode)
        {
            if (string.IsNullOrEmpty(mode))
            {
                return MailingMode.Disabled;
            }

            switch (mode.ToLowerInvariant())
            {
                case "exchange":
                    return MailingMode.Exchange;

                case "pop3":
                    return MailingMode.Pop3;

                case "disabled":
                    return MailingMode.Disabled;

                default:
                    return MailingMode.Unknown;
            }
        }

        #endregion

        #region Private methods

        private bool ParseSettings()
        {
            this.mode = ParseMailingMode(MobileApprovalConfig.Mode);
            bool settingsAreInvalid = false;

            if (this.mode == MailingMode.Disabled)
            {
                return false;
            }

            if (this.mode == MailingMode.Unknown)
            {
                logger.Error(
                    "Invalid mode setting specified in setting {0}.",
                    MobileApprovalConfig.MobileApproval_Mode_PropertyName);

                return false;
            }

            switch (this.mode)
            {
                case MailingMode.Exchange:
                    string mobileApprovalUser = MobileApprovalConfig.ExchangeUser;
                    ExchangeVersion? exchangeVersionTemp;

                    string exchangeUser;
                    string exchangePassword;
                    string exchangeServer;
                    string exchangeProxyAddress;
                    string exchangeProxyUser;
                    string exchangeProxyPassword;

                    if (string.IsNullOrEmpty(mobileApprovalUser))
                    {
                        exchangeUser = NoticeMailerConfig.ExchangeUser;
                        exchangePassword = NoticeMailerConfig.ExchangePassword;
                        exchangeServer = NoticeMailerConfig.ExchangeServer;
                        exchangeProxyAddress = NoticeMailerConfig.ExchangeProxyAddress;
                        exchangeProxyUser = NoticeMailerConfig.ExchangeProxyUser;
                        exchangeProxyPassword = NoticeMailerConfig.ExchangeProxyPassword;
                        exchangeVersionTemp = NoticeMailerConfig.ExchangeVersion;

                        if (string.IsNullOrEmpty(exchangeUser))
                        {
                            settingsAreInvalid = true;
                            logger.Error(
                                "Invalid settings specified for connection to Exchange server: {0}.",
                                NoticeMailerConfig.NoticeMailer_ExchangeUser_PropertyName);
                        }

                        if (!exchangeVersionTemp.HasValue)
                        {
                            settingsAreInvalid = true;
                            logger.Error(
                                "Invalid settings specified for connection to Exchange server: {0}.",
                                NoticeMailerConfig.NoticeMailer_ExchangeVersion_PropertyName);
                        }
                    }
                    else
                    {
                        exchangeUser = mobileApprovalUser;
                        exchangePassword = MobileApprovalConfig.ExchangePassword;
                        exchangeServer = MobileApprovalConfig.ExchangeServer;
                        exchangeProxyAddress = MobileApprovalConfig.ExchangeProxyAddress;
                        exchangeProxyUser = MobileApprovalConfig.ExchangeProxyUser;
                        exchangeProxyPassword = MobileApprovalConfig.ExchangeProxyPassword;
                        exchangeVersionTemp = MobileApprovalConfig.ExchangeVersion;

                        if (string.IsNullOrEmpty(exchangeUser))
                        {
                            settingsAreInvalid = true;
                            logger.Error(
                                "Invalid settings specified for connection to Exchange server: {0}.",
                                MobileApprovalConfig.MobileApproval_ExchangeUser_PropertyName);
                        }

                        if (!exchangeVersionTemp.HasValue)
                        {
                            settingsAreInvalid = true;
                            logger.Error(
                                "Invalid settings specified for connection to Exchange server: {0}.",
                                MobileApprovalConfig.MobileApproval_ExchangeVersion_PropertyName);
                        }
                    }

                    if (settingsAreInvalid)
                    {
                        return false;
                    }

                    var exchangeVersion = exchangeVersionTemp.Value;

                    this.exchangeServiceSettings =
                        new ExchangeServiceSettings
                        {
                            User = exchangeUser,
                            Password = exchangePassword,
                            Server = exchangeServer,
                            ProxyAddress =
                                string.IsNullOrWhiteSpace(exchangeProxyAddress)
                                    ? null
                                    : new Uri(exchangeProxyAddress),
                            ProxyUser = exchangeProxyUser,
                            ProxyPassword = exchangeProxyPassword,
                            Version = exchangeVersion
                        };
                    break;

                case MailingMode.Pop3:
                    var pop3Hostname = MobileApprovalConfig.Pop3Hostname;
                    var pop3Port = MobileApprovalConfig.Pop3Port;
                    var pop3User = MobileApprovalConfig.Pop3User;
                    var pop3Password = MobileApprovalConfig.Pop3Password;
                    var pop3UseSsl = MobileApprovalConfig.Pop3UseSsl;

                    if (string.IsNullOrEmpty(pop3Hostname))
                    {
                        settingsAreInvalid = true;
                        logger.Error(
                            "Invalid POP3 settings: {0}.", MobileApprovalConfig.MobileApproval_Pop3Hostname_PropertyName);
                    }

                    if (!pop3Port.HasValue)
                    {
                        settingsAreInvalid = true;
                        logger.Error(
                            "Invalid POP3 settings: {0}.", MobileApprovalConfig.MobileApproval_Pop3Port_PropertyName);
                    }

                    if (string.IsNullOrEmpty(pop3User))
                    {
                        settingsAreInvalid = true;
                        logger.Error(
                            "Invalid POP3 settings: {0}.", MobileApprovalConfig.MobileApproval_Pop3User_PropertyName);
                    }

                    if (string.IsNullOrEmpty(pop3Password))
                    {
                        settingsAreInvalid = true;
                        logger.Error(
                            "Invalid POP3 settings: {0}.", MobileApprovalConfig.MobileApproval_Pop3Password_PropertyName);
                    }

                    if (!pop3UseSsl.HasValue)
                    {
                        settingsAreInvalid = true;
                        logger.Error(
                            "Invalid POP3 settings: {0}.", MobileApprovalConfig.MobileApproval_Pop3UseSsl_PropertyName);
                    }

                    if (settingsAreInvalid)
                    {
                        return false;
                    }

                    this.pop3ServiceSettings =
                        new Pop3ServiceSettings
                        {
                            Pop3Hostname = pop3Hostname,
                            Pop3Port = pop3Port,
                            Pop3User = pop3User,
                            Pop3Password = pop3Password,
                            Pop3UseSsl = pop3UseSsl
                        };
                    break;

                case MailingMode.Unknown:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException("mode");
            }

            return true;
        }

        // здесь нельзя делать RestrictTaskSections, т.к. секции заданий ожидают расширения на загрузку карточки, такие как WfTasksServerGetExtension
        #endregion

        #region Base Overrides

        public override async Task EntryPoint(IPluginExtensionContext context)
        {
            // парсим настройки получения почты
            if (!this.ParseSettings())
            {
                logger.Trace("Plugin disabled");
                return;
            }

            if (context.StopRequested)
            {
                return;
            }

            if (!context.UnityContainer.IsRegistered<SetProcessorTokenAction>())
            {
                context.UnityContainer
                    .RegisterFactory<SetProcessorTokenAction>(
                        c => new SetProcessorTokenAction(token => context.SessionToken = token),
                        new ContainerControlledLifetimeManager())
                    ;
            }

            await using (context.DbScope.Create())
            {
                var licenseValidator = context.Resolve<ILicenseValidator>();

                ILicense license = context.Resolve<ILicenseManager>().License;

                if (!license.Modules.Contains(LicenseModules.MobileApprovalID))
                {
                    logger.Error("Mobile approval license is not found");
                    return;
                }

                int userCount = await licenseValidator.GetMobileLicenseCountAsync(context.CancellationToken);
                int count = license.GetMobileCount();

                if (userCount > count)
                {
                    logger.Error("Mobile approval license limit exceeded. Selected employees: {0}. " +
                        "The total number of licenses: {1}", userCount, count);
                    return;
                }

                if (context.StopRequested)
                {
                    return;
                }

                logger.Trace("Mail processing.");

                IMailReceiver receiver = null;

                switch (this.mode)
                {
                    case MailingMode.Exchange:
                        receiver = context.Resolve<IMailReceiver>(MailReceiverNames.ExchangeMailReceiver);
                        receiver.StopRequestedFunc = () => context.StopRequested;
                        ((ExchangeMailReceiver)receiver).Settings = this.exchangeServiceSettings;
                        break;

                    case MailingMode.Pop3:
                        receiver = context.Resolve<IMailReceiver>(MailReceiverNames.Pop3MailReceiver);
                        receiver.StopRequestedFunc = () => context.StopRequested;
                        ((Pop3MailReceiver)receiver).Settings = this.pop3ServiceSettings;
                        break;
                }

                if (receiver != null)
                {
                    await receiver.ReceiveMessagesAsync(context.CancellationToken);
                }
            }
        }

        #endregion
    }
}
