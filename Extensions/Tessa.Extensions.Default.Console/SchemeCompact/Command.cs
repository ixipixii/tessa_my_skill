using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.SchemeCompact
{
    public static class Command
    {
        [Verb("SchemeCompact")]
        [LocalizableDescription("Scheme_CLI_Compact")]
        public static async Task SchemeCompact(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument, LocalizableDescription("Scheme_CLI_X_Source")] string source,
            [Argument("out"), LocalizableDescription("Scheme_CLI_Compact_Out")] string target = null,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(SchemeCompact)), stdOut, stdErr, quiet);

            int result = await Operation.ExecuteAsync(logger, stdOut, source, target);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}