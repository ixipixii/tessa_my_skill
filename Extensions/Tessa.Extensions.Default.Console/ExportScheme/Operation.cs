﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.ExportScheme
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
                await this.Logger.InfoAsync("Reading scheme from web service");

                SchemeDatabase tessaDatabase = new SchemeDatabase(DatabaseNames.Original);
                tessaDatabase.Refresh(this.schemeService);

                string exportPath = DefaultConsoleHelper.NormalizeFolderAndCreateIfNotExists(context.OutputFolder);
                if (string.IsNullOrEmpty(exportPath))
                {
                    exportPath = Directory.GetCurrentDirectory();
                }

                string tsdFilePath = Directory.EnumerateFiles(exportPath, "*.tsd").OrderBy(x => x).FirstOrDefault()
                    ?? Path.Combine(exportPath, "Tessa.tsd");

                await this.Logger.InfoAsync("Reading scheme from file \"{0}\"", tsdFilePath);

                string[] partitions = FileSchemeService.GetPartitionPaths(tsdFilePath);
                var fileSchemeService = new FileSchemeService(tsdFilePath, partitions);

                if (!fileSchemeService.IsStorageExists)
                {
                    await this.Logger.InfoAsync("Scheme doesn't exist in the file folder, creating it...");
                    fileSchemeService.CreateStorage();
                }

                if (!fileSchemeService.IsStorageUpToDate)
                {
                    await this.Logger.InfoAsync("Scheme isn't up-to-date in the file folder, upgrading it...");
                    fileSchemeService.UpdateStorage();
                }

                await this.Logger.InfoAsync("Exporting the scheme using web service to folder \"{0}\"", exportPath);
                tessaDatabase.SubmitChanges(fileSchemeService);
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error exporting scheme", e);
                return -1;
            }

            await this.Logger.InfoAsync("Scheme has been exported successfully");
            return 0;
        }
    }
}