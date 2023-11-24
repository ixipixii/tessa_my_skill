using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Files.VirtualFiles;
using Tessa.Extensions.Default.Server.Files.VirtualFiles.Compilation;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Сбрасывание кешей при удалении виртуального файла
    /// </summary>
    public sealed class KrVirtualFileDeleteExtension : CardDeleteExtension
    {
        #region Fields

        private readonly IKrVirtualFileCache virtualFileCache;
        private readonly IKrVirtualFileCompilationCache compilationCache;

        #endregion

        #region Constructors

        public KrVirtualFileDeleteExtension(
            IKrVirtualFileCache virtualFileCache,
            IKrVirtualFileCompilationCache compilationCache)
        {
            this.virtualFileCache = virtualFileCache;
            this.compilationCache = compilationCache;
        }

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardDeleteExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return;
            }

            await compilationCache.InvalidateAsync(context.CancellationToken);
            await virtualFileCache.InvalidateAsync(context.CancellationToken);
        }

        #endregion
    }
}
