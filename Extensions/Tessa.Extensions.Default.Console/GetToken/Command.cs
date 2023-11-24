using System.IO;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.GetToken
{
    public static class Command
    {
        [Verb("GetToken")]
        [LocalizableDescription("TokenEditor_CLI_Get")]
        public static async Task GetToken(
            [Output] TextWriter output)
        {
            int result = await Operation.ExecuteAsync(output);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}