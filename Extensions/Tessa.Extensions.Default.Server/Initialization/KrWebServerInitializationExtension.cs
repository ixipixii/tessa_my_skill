using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Settings;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Initialization;
using Tessa.Platform.Settings;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Initialization
{
    public sealed class KrWebServerInitializationExtension :
        ServerInitializationExtension
    {
        #region Constructors

        public KrWebServerInitializationExtension(
            IKrTypesCache krTypesCache,
            ICardRepository extendedRepository,
            ISettingsProvider settingsProvider)
        {
            if (krTypesCache == null)
            {
                throw new ArgumentNullException("krTypesCache");
            }
            if (extendedRepository == null)
            {
                throw new ArgumentNullException("extendedRepository");
            }
            if (settingsProvider == null)
            {
                throw new ArgumentNullException("settingsProvider");
            }

            this.krTypesCache = krTypesCache;
            this.extendedRepository = extendedRepository;
            this.settingsProvider = settingsProvider;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        private readonly ICardRepository extendedRepository;

        private readonly ISettingsProvider settingsProvider;

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(IServerInitializationExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || context.ConfigurationIsCached)
            {
                return;
            }

            List<object> unavailableTypes =
                (await KrPermissionsHelper.GetUnavailableTypesAsync(
                    this.extendedRepository,
                    this.krTypesCache,
                    cancellationToken: context.CancellationToken))
                .Cast<object>()
                .ToList();

            ListStorage<KrDocType> docTypesInCache = await this.krTypesCache.GetDocTypesAsync(context.CancellationToken);
            IList<object> docTypes = docTypesInCache != null ? docTypesInCache.GetStorage() : new List<object>();

            // настройки типов карточек, у которых нет типов документов и которые добавлены в типовое решение
            ListStorage<KrCardType> cardTypesInCache = await this.krTypesCache.GetCardTypesAsync(context.CancellationToken);
            IList<object> cardTypes = cardTypesInCache != null
                ? cardTypesInCache
                    .Where(x => !x.UseDocTypes)
                    .Select(x => (object)x.GetStorage())
                    .ToList()
                : new List<object>();

            ISettings settings = await this.settingsProvider.GetSettingsAsync(context.CancellationToken);
            KrSettings krSettings = settings.TryGet<KrSettings>();
            var createBasedOnTypes = krSettings != null ? krSettings.CreateBasedOnTypes : new HashSet<Guid>();

            context.Response.Info["KrUnavailableTypes"] = unavailableTypes;
            context.Response.Info["KrDocTypes"] = docTypes;
            context.Response.Info["KrCardTypes"] = cardTypes;
            context.Response.Info["CreateBasedOnTypes"] = createBasedOnTypes;
        }

        #endregion
    }
}
