using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.SchemeUpdate
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(IConsoleLogger logger, string source)
        {
            string filePath = DefaultConsoleHelper.GetSourceFiles(source, "*.tsd")[0];
            await logger.InfoAsync($"Updating the scheme from file: \"{filePath}\"");

            var partitions = FileSchemeService.GetPartitionPaths(source);
            var service = new FileSchemeService(source, partitions);

            if (service.IsStorageUpToDate)
            {
                await logger.InfoAsync("The storage is up to date");
            }
            else
            {
                service.UpdateStorage();
                await logger.InfoAsync("Updating the storage is successful");
            }

            return 0;
        }
    }
}
