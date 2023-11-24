using System.Threading.Tasks;
using Tessa.Cards.Extensions;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrCompileSourceDeleteExtension: CardDeleteExtension
    {
        #region fields

        private readonly IKrCompilationCache compilationCache;

        private readonly IKrProcessCache stageCache;

        private readonly IKrCompilationResultStorage compilationResultStorage;

        #endregion

        #region constructor

        public KrCompileSourceDeleteExtension(IKrCompilationCache compilationCache, IKrProcessCache stageCache,
            IKrCompilationResultStorage compilationResultStorage)
        {
            this.compilationCache = compilationCache;
            this.stageCache = stageCache;
            this.compilationResultStorage = compilationResultStorage;
        }

        #endregion

        #region base overrides

        public override Task AfterRequest(ICardDeleteExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            this.compilationCache.Invalidate();
            this.stageCache.Invalidate();
            var cardID = context.Request.CardID;
            if (cardID.HasValue)
            {
                this.compilationResultStorage.Delete(cardID.Value);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}