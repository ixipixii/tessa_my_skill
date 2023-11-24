using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Initialization;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Initialization
{
    public sealed class KrServerInitializationExtension :
        ServerInitializationExtension
    {
        #region Constructors

        public KrServerInitializationExtension(
            IKrTypesCache krTypesCache,
            ICardRepository extendedRepository)
        {
            if (krTypesCache == null)
            {
                throw new ArgumentNullException("krTypesCache");
            }
            if (extendedRepository == null)
            {
                throw new ArgumentNullException("extendedRepository");
            }

            this.krTypesCache = krTypesCache;
            this.extendedRepository = extendedRepository;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache krTypesCache;

        private readonly ICardRepository extendedRepository;

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(IServerInitializationExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || context.ConfigurationIsCached)
            {
                return;
            }

            context.AddHandler(
                DefaultInitializationHandlers.Types,
                async (ctx, stream) =>
                {
                    List<object> unavailableTypes =
                        (await KrPermissionsHelper.GetUnavailableTypesAsync(
                            this.extendedRepository,
                            this.krTypesCache,
                            cancellationToken: ctx.CancellationToken))
                        .Cast<object>()
                        .ToList();

                    ListStorage<KrDocType> docTypesInCache = await this.krTypesCache.GetDocTypesAsync(ctx.CancellationToken);
                    IList<object> docTypes = docTypesInCache != null ? docTypesInCache.GetStorage() : new List<object>();

                    var data = new Dictionary<string, object>(StringComparer.Ordinal)
                    {
                        { "unavailableTypes", unavailableTypes },
                        { "docTypes", docTypes },
                    };

                    byte[] bytes = data.ToSerializable().Serialize();
                    await stream.WriteAsync(bytes, 0, bytes.Length, ctx.CancellationToken);
                });
        }

        #endregion
    }
}
