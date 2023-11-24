﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Files;
using Tessa.Platform.Runtime;
using Unity;

namespace Tessa.Extensions.Default.Client.ExternalFiles
{
    public class ExternalFileSource : FileSource
    {
        #region Constructors

        public ExternalFileSource(
            IFileCache cache,
            ISession session,
            [OptionalDependency] IFileManager manager = null)
            : base(cache, session, manager)
        {
        }

        #endregion

        #region IFileSource Items

        protected override async ValueTask<IFile> CreateFileCoreAsync(
            IFileCreationToken token,
            IFileContent content = null,
            CancellationToken cancellationToken = default)
        {
            // здесь возможно исключение, связанное с параметрами метода
            var typedToken = (ExternalFileCreationToken)token;

            string name = token.Name;
            IFileContent allocatedContent = null;
            IFileContent actualContent = content ?? (allocatedContent = this.Cache.Allocate(name));

            try
            {
                // все свойства токена проверяются в конструкторе
                return new ExternalFile(
                    token.ID ?? Guid.NewGuid(),
                    name,
                    token.Size,
                    token.Category,
                    token.Type,
                    actualContent,
                    this,
                    token.Permissions.Clone(),
                    token.IsLocal,
                    description: typedToken.Description);
            }
            catch (Exception)
            {
                allocatedContent?.Dispose();
                throw;
            }
        }

        protected override async ValueTask<IFileCreationToken> GetFileCreationTokenCoreAsync(
            CancellationToken cancellationToken = default) =>
            new ExternalFileCreationToken();

        protected override async Task<IFileContentResponse> GetContentCoreAsync(
            IFileContentRequest request,
            CancellationToken cancellationToken = default)
        {
            string content = ((ExternalFile) request.Version.File).Description ?? string.Empty;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                Func<Stream, CancellationToken, Task> processContentActionAsync =
                    request.ProcessContentActionAsync ?? ((s, ct2) => Task.CompletedTask);

                await processContentActionAsync(stream, cancellationToken).ConfigureAwait(false);
            }

            return new FileContentResponse(request.Version);
        }

        #endregion
    }
}
