using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.SchemeDiff
{
    public static class Command
    {
        [Verb("SchemeDiff")]
        [LocalizableDescription("Scheme_CLI_Difference")]
        public static async Task SchemeDiff(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument("a"), LocalizableDescription("Scheme_CLI_X_Source")] string sourceA,
            [Argument("b"), LocalizableDescription("Scheme_CLI_X_Source")] string sourceB,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(SchemeDiff)), stdOut, stdErr, quiet);

            int result = await Operation.ExecuteAsync(logger, stdOut, sourceA, sourceB);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}