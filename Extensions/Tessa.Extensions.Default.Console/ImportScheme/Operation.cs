using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.ImportScheme
{
    public sealed class Operation : ConsoleOperation<OperationContext>
    {
        private readonly ISchemeService schemeService;

        public Operation(
            ConsoleSessionManager sessionManager,
            IConsoleLogger logger,
            ISchemeService schemeService)
            : base(logger, sessionManager)
        {
            this.schemeService = schemeService;
        }

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(OperationContext context, CancellationToken cancellationToken = default)
        {
            if (!this.SessionManager.IsOpened)
            {
                return -1;
            }

            try
            {
                foreach (string source in DefaultConsoleHelper.GetSourceFiles(context.Source, "*.tsd"))
                {
                    await this.Logger.InfoAsync("Reading scheme from: \"{0}\"", source);

                    string[] partitions = FileSchemeService.GetPartitionPaths(source);
                    var fileSchemeService = new FileSchemeService(source, partitions);

                    if (!fileSchemeService.IsStorageUpToDate)
                    {
                        await this.Logger.InfoAsync("Scheme isn't up-to-date in the file folder, upgrading it...");
                        fileSchemeService.UpdateStorage();
                    }

                    SchemeDatabase tessaDatabase = new SchemeDatabase(DatabaseNames.Original);
                    tessaDatabase.Refresh(fileSchemeService);

                    await this.Logger.InfoAsync("Importing the scheme using web service");
                    tessaDatabase.SubmitChanges(this.schemeService);
                }
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error importing scheme", e);
                return -1;
            }

            await this.Logger.InfoAsync("Scheme has been imported successfully");
            return 0;
        }
    }
}
