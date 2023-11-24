using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Platform;
using Tessa.Platform.Initialization;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Initialization
{
    /// <summary>
    /// Расширение добавляет указание на отключение загрузки файлов на мобильных устройствах
    /// </summary>
    public sealed class KrWebDownloadServerInitializationExtension :
        ServerInitializationExtension
    {
        #region Fields

        private readonly ICardCache cardCache;

        #endregion

        #region Constructors

        public KrWebDownloadServerInitializationExtension(ICardCache cardCache)
        {
            this.cardCache = cardCache;
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(IServerInitializationExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || context.ConfigurationIsCached)
            {
                return;
            }

            DeviceType deviceType = context.Session.Token.DeviceType;
            bool denyFileDownload = false;

            if (deviceType == DeviceType.Tablet || deviceType == DeviceType.Phone)
            {
                try
                {
                    denyFileDownload =
                        (await this.cardCache.Cards.GetAsync(CardHelper.ServerInstanceTypeName, context.CancellationToken))
                        .Sections["ServerInstances"]
                        .RawFields
                        .Get<bool>("DenyMobileFileDownload");
                }
                catch (SingletonNotFoundInCacheException)
                {
                    denyFileDownload = false;
                }
            }

            context.Response.Info["DenyFileDownload"] = BooleanBoxes.Box(denyFileDownload);
        }

        #endregion
    }
}
