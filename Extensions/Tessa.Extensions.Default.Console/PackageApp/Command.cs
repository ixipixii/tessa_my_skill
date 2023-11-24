using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.PackageApp
{
    public static class Command
    {
        [Verb("PackageApp")]
        [LocalizableDescription("Common_CLI_PackageApp")]
        public static async Task PackageApp(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument] [LocalizableDescription("Common_CLI_AppSourceExecutable")] string executable,
            [Argument("out")] [LocalizableDescription("Common_CLI_AppOutputPackage")] string jcardFile = null,
            [Argument("ico")] [LocalizableDescription("Common_CLI_AppIconFile")] string icon = null,
            [Argument("a")] [LocalizableDescription("Common_CLI_AppAlias")] string alias = null,
            [Argument("n")] [LocalizableDescription("Common_CLI_AppName")] string name = null,
            [Argument("g")] [LocalizableDescription("Common_CLI_AppGroup")] string group = null,
            [Argument("admin"), LocalizableDescription("Common_CLI_AppAdmin")] bool admin = false,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(PackageApp)), stdOut, stdErr, quiet);

            int result = await Operation.ExecuteAsync(logger, executable, jcardFile, icon, alias, name, group, admin);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}