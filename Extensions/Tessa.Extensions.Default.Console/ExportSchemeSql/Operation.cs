using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Platform;
using Tessa.Platform.ConsoleApps;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.ExportSchemeSql
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            string outputFolder,
            bool upgradeSchemeInDatabase,
            string configurationString,
            string databaseName)
        {
            await logger.InfoAsync("Exporting the scheme using sql connection");

            await logger.InfoAsync(
                string.IsNullOrEmpty(configurationString)
                    ? "Connection will be opened to default database"
                    : "Connection will be opened to database with connection \"{0}\"",
                configurationString);

            if (!string.IsNullOrEmpty(databaseName))
            {
                await logger.InfoAsync("Changes will be applied from database \"{0}\"", databaseName);
            }

            var configurationProvider = ConfigurationManager.Default.Configuration
                .GetConfigurationDataProvider(configurationString);
            var factory = ConfigurationManager
                .GetConfigurationDataProviderFromType(configurationProvider.Item2.DataProvider)
                .GetDbProviderFactory();
            var connectionString = configurationProvider.Item2.ConnectionString;

            if (!string.IsNullOrEmpty(databaseName))
            {
                var builder = factory.CreateConnectionStringBuilder();

                builder.ConnectionString = connectionString;
                builder["Database"] = databaseName;
                connectionString = builder.ToString();
            }

            var databaseSchemeService = new DatabaseSchemeService(() =>
            {
                var connection = ConfigurationManager
                    .GetDbProviderFactory(configurationString)
                    .CreateConnection();
                connection.ConnectionString = connectionString;
                return connection;
            });

            if (!databaseSchemeService.IsStorageExists)
            {
                await logger.ErrorAsync("Scheme doesn't exists in the database, can't continue");
                return -1;
            }

            if (!databaseSchemeService.IsStorageUpToDate)
            {
                if (!upgradeSchemeInDatabase)
                {
                    await logger.ErrorAsync("Scheme isn't up-to-date in the database, can't continue");
                    return -1;
                }

                await logger.InfoAsync("Scheme isn't up-to-date in the database, upgrading it...");
                databaseSchemeService.UpdateStorage();
            }

            SchemeDatabase tessaDatabase = new SchemeDatabase(DatabaseNames.Original);
            tessaDatabase.Refresh(databaseSchemeService);

            string exportPath = DefaultConsoleHelper.NormalizeFolderAndCreateIfNotExists(outputFolder);
            if (string.IsNullOrEmpty(exportPath))
            {
                exportPath = Directory.GetCurrentDirectory();
            }

            string tsdFilePath = Directory.EnumerateFiles(exportPath, "*.tsd").OrderBy(x => x).FirstOrDefault()
                ?? Path.Combine(exportPath, "Tessa.tsd");

            await logger.InfoAsync("Reading scheme from file \"{0}\"", tsdFilePath);

            string[] partitions = FileSchemeService.GetPartitionPaths(tsdFilePath);
            var fileSchemeService = new FileSchemeService(tsdFilePath, partitions);

            if (!fileSchemeService.IsStorageExists)
            {
                await logger.InfoAsync("Scheme doesn't exist in the file folder, creating it...");
                fileSchemeService.CreateStorage();
            }

            if (!fileSchemeService.IsStorageUpToDate)
            {
                await logger.InfoAsync("Scheme isn't up-to-date in the file folder, upgrading it...");
                fileSchemeService.UpdateStorage();
            }

            await logger.InfoAsync("Exporting the scheme using database to folder \"{0}\"", exportPath);
            tessaDatabase.SubmitChanges(fileSchemeService);

            await logger.InfoAsync("Scheme has been exported successfully");
            return 0;
        }
    }
}