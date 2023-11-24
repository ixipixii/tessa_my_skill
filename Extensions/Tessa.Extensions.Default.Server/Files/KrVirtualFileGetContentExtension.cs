using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Files.VirtualFiles;

namespace Tessa.Extensions.Default.Server.Files
{
    public sealed class KrVirtualFileGetContentExtension :
        CardGetFileContentExtension
    {
        #region Fields

        private readonly IKrVirtualFileManager krVirtualFileManager;
        private readonly IKrVirtualFileCache krVirtualFileCache;
        private readonly ICardStreamServerRepository cardStreamRepository;

        #endregion

        #region Constructors

        public KrVirtualFileGetContentExtension(
            IKrVirtualFileManager krVirtualFileManager,
            IKrVirtualFileCache krVirtualFileCache,
            ICardStreamServerRepository cardStreamRepository)
        {
            this.krVirtualFileManager = krVirtualFileManager;
            this.krVirtualFileCache = krVirtualFileCache;
            this.cardStreamRepository = cardStreamRepository;
        }

        #endregion

        #region Base Overrides

        public override async Task BeforeRequest(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;
            var fileID = context.Request.FileID;
            var versionID = context.Request.VersionRowID;

            if (!cardID.HasValue
                || !fileID.HasValue
                || !versionID.HasValue)
            {
                return;
            }

            context.ValidationResult.Add(
                await krVirtualFileManager.CheckAccessForFileAsync(cardID.Value, fileID.Value, context.CancellationToken));

            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            var virtualFile = await krVirtualFileCache.GetAsync(fileID.Value, context.CancellationToken);
            var version = virtualFile.Versions.FirstOrDefault(x => x.ID == versionID.Value);

            if (version is null)
            {
                return;
            }

            var result = await cardStreamRepository.GenerateFileFromTemplateAsync(
                version.FileTemplateID,
                cardID);

            result.Response.ValidationResult.Add(context.ValidationResult);
            context.Response = result.Response;

            if (!string.IsNullOrEmpty(context.Request.FileName))
            {
                context.Response.SetSuggestedFileName(context.Request.FileName);
            }

            context.ContentFuncAsync = result.GetContentOrThrowAsync;
        }

        #endregion
    }
}