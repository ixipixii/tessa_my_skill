﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;
using Unity;

namespace Tessa.Extensions.Default.Console.ExportWorkplaces
{
    public static class Command
    {
        [Verb("ExportWorkplaces")]
        [LocalizableDescription("Common_CLI_ExportWorkplaces")]
        public static async Task ExportWorkplaces(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument] [LocalizableDescription("Common_CLI_WorkplaceNamesOrIdentifiers")] IEnumerable<string> nameOrIdentifier = null,
            [Argument("a")] [LocalizableDescription("Common_CLI_Address")] string address = null,
            [Argument("i")] [LocalizableDescription("Common_CLI_Instance")] string instanceName = null,
            [Argument("u")] [LocalizableDescription("Common_CLI_UserName")] string userName = null,
            [Argument("p")] [LocalizableDescription("Common_CLI_Password")] string password = null,
            [Argument("o")] [LocalizableDescription("Common_CLI_OutputFolder")] string outputFolder = null,
            [Argument("c")] [LocalizableDescription("Common_CLI_ClearOutputFolder")] bool clearOutputFolder = false,
            [Argument("v")] [LocalizableDescription("Common_CLI_IncludeViewsIntoWorkplace")] bool includeViews = false,
            [Argument("s")] [LocalizableDescription("Common_CLI_IncludeSearchQueriesIntoWorkplace")] bool includeSearchQueries = false,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IUnityContainer container = new UnityContainer().ConfigureConsoleForClient(stdOut, stdErr, quiet, instanceName, address);

            int result;
            await using (var operation = container.Resolve<Operation>())
            {
                var context = new OperationContext
                {
                    WorkplaceNamesOrIdentifiers = nameOrIdentifier?.ToList(),
                    OutputFolder = outputFolder,
                    ClearOutputFolder = clearOutputFolder,
                    IncludeViews = includeViews,
                    IncludeSearchQueries = includeSearchQueries,
                };

                if (!await operation.LoginAsync(userName, password))
                {
                    ConsoleAppHelper.EnvironmentExit(ConsoleAppHelper.FailedLoginExitCode);
                    return;
                }

                result = await operation.ExecuteAsync(context);
                await operation.CloseAsync();
            }

            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}