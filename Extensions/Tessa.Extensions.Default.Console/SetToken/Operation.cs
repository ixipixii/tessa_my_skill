using System.Linq;
using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Default.Console.SetToken
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            string serviceFolder,
            string tokenSignature)
        {
            if (string.IsNullOrEmpty(serviceFolder))
            {
                await logger.ErrorAsync("Can't set token: folder path is empty.");
                return -1;
            }

            if (string.IsNullOrEmpty(tokenSignature))
            {
                await logger.ErrorAsync("Can't set token: signature is empty.");
                return -1;
            }

            await logger.InfoAsync("Replacing token in: {0}", serviceFolder);
            await logger.InfoAsync("New token signature: {0}", tokenSignature);

            RuntimeHelper.ReplaceTokenSignatureInServices(
                serviceFolder,
                tokenSignature,
                out string[] successfulFiles,
                out string[] failedFiles);

            if (successfulFiles.Length > 0)
            {
                await logger.InfoAsync(
                    "Token has been replaced in files ({0}): {1}",
                    successfulFiles.Length,
                    string.Join("; ", successfulFiles.Select(x => "\"" + x + "\"")));
            }

            if (failedFiles.Length > 0)
            {
                // это не ошибка, просто некоторые файлы не зареплейсились
                await logger.InfoAsync(
                    "There are no token to replace in files ({0}): {1}",
                    failedFiles.Length,
                    string.Join("; ", failedFiles.Select(x => "\"" + x + "\"")));
            }

            return 0;
        }
    }
}
