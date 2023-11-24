using System;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Forums;

namespace Tessa.Extensions.Default.Server.Forum.Satellite
{
    public class ForumSatelliteGetFileContentExtension : CardGetFileContentExtension
    {
        private readonly IForumProvider forumProvider;

        public ForumSatelliteGetFileContentExtension(IForumProvider forumProvider)
        {
            this.forumProvider = forumProvider;
        }

        public override async Task BeforeRequest(ICardGetFileContentExtensionContext context)
        {
            Guid? fileID = context.Request.FileID;
            if (!fileID.HasValue)
            {
                return;
            }

            var response = await forumProvider.CheckPermissionByFileAsync(fileID.Value, context.CancellationToken);
            if (!response.ValidationResult.IsSuccessful())
            {
                context.ValidationResult.Add(response.ValidationResult);
            }
        }
    }
}
