using System.Threading.Tasks;
using Tessa.Platform;
using Tessa.Platform.ConsoleApps;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.ImportSchemeSql
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            string source,
            string configurationString,
            string databaseName)
        {
            bool headerLogged = false;

            foreach (string filePath in DefaultConsoleHelper.GetSourceFiles(source, "*.tsd"))
            {
                if (!headerLogged)
                {
                    await logger.InfoAsync("Importing the scheme using sql connection");

                    await logger.InfoAsync(
                        string.IsNullOrEmpty(configurationString)
                            ? "Connection will be opened to default database"
                            : "Connection will be opened to database with connection \"{0}\"",
                        configurationString);

                    if (!string.IsNullOrEmpty(databaseName))
                    {
                        await logger.InfoAsync("Changes will be applied to database \"{0}\"", databaseName);
                    }

                    headerLogged = true;
                }

                await logger.InfoAsync("Reading scheme from: \"{0}\"", filePath);

                string[] partitions = FileSchemeService.GetPartitionPaths(filePath);
                var fileSchemeService = new FileSchemeService(filePath, partitions);

                if (!fileSchemeService.IsStorageUpToDate)
                {
                    await logger.InfoAsync("Scheme isn't up-to-date in the file folder, upgrading it...");
                    fileSchemeService.UpdateStorage();
                }

                SchemeDatabase tessaDatabase = new SchemeDatabase(DatabaseNames.Original);
                tessaDatabase.Refresh(fileSchemeService);

                await logger.InfoAsync("Importing the scheme");

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
                    await logger.InfoAsync("Scheme doesn't exists in the database, creating it...");
                    databaseSchemeService.CreateStorage();
                }

                if (!databaseSchemeService.IsStorageUpToDate)
                {
                    await logger.InfoAsync("Scheme isn't up-to-date in the database, upgrading it...");
                    databaseSchemeService.UpdateStorage();
                }

                tessaDatabase.SubmitChanges(databaseSchemeService);
            }

            await logger.InfoAsync("Scheme has been imported successfully");
            return 0;
        }
    }
}
