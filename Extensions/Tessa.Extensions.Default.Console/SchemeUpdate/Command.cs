using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.SchemeUpdate
{
    public static class Command
    {
        [Verb("SchemeUpdate")]
        [LocalizableDescription("Scheme_CLI_Update")]
        public static async Task SchemeUpdate(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument, LocalizableDescription("Scheme_CLI_X_Source")] string source,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(SchemeUpdate)), stdOut, stdErr, quiet);

            int result = await Operation.ExecuteAsync(logger, source);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}