using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Console.SchemeScript
{
    public static class Command
    {
        [Verb("SchemeScript")]
        [LocalizableDescription("Scheme_CLI_Script")]
        public static async Task SchemeScript(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument, LocalizableDescription("Scheme_CLI_X_Source")] string source,
            [Argument("out"), LocalizableDescription("Scheme_CLI_Script_Out")] string target = null,
            [Argument("dbms"), LocalizableDescription("Scheme_CLI_Script_DBMS")] Dbms dbms = Dbms.SqlServer,
            [Argument("dbmsv"), LocalizableDescription("Scheme_CLI_Script_DBMSV"), TypeConverter(typeof(VersionConverter))] Version dbmsv = null,
            [Argument("notran"), LocalizableDescription("Scheme_CLI_Script_NoTran")] bool withoutTransactions = false,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(SchemeScript)), stdOut, stdErr, quiet);

            int result = await Operation.ExecuteAsync(logger, stdOut, source, target, dbms, dbmsv, withoutTransactions);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}