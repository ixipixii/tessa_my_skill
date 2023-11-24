using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Console.SchemeRename
{
    public static class Command
    {
        [Verb("SchemeRename")]
        [LocalizableDescription("Scheme_CLI_Rename")]
        public static async Task SchemeRename(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument, LocalizableDescription("Scheme_CLI_X_Source")] string source,
            [Argument("t"), LocalizableDescription("Scheme_CLI_Rename_TableName")] string tableName,
            [Argument("c"), LocalizableDescription("Scheme_CLI_Rename_ColumnName")] string columnName,
            [Argument("out"), LocalizableDescription("Scheme_CLI_Rename_Out")] string target = null,
            [Argument("dbms"), LocalizableDescription("Scheme_CLI_Script_DBMS")] Dbms dbms = Dbms.SqlServer,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(SchemeRename)), stdOut, stdErr, quiet);

            int result = await Operation.ExecuteAsync(logger, stdOut, source, tableName, columnName, target, dbms);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}