﻿using System;
using System.IO;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;
using Unity;

namespace Tessa.Extensions.Default.Console.ImportViews
{
    public static class Command
    {
        [Verb("ImportViews")]
        [LocalizableDescription("Views_CLI_ImportViews")]
        public static async Task ImportViews(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument] [LocalizableDescription("Common_CLI_Source")] string source,
            [Argument("a")] [LocalizableDescription("Common_CLI_Address")] string address = null,
            [Argument("i")] [LocalizableDescription("Common_CLI_Instance")] string instanceName = null,
            [Argument("u")] [LocalizableDescription("Common_CLI_UserName")] string userName = null,
            [Argument("p")] [LocalizableDescription("Common_CLI_Password")] string password = null,
            [Argument("c")] [LocalizableDescription("Views_Import_DeleteViewsOption")] bool clearViews = false,
            [Argument("r")] [LocalizableDescription("Views_Import_ReplacePermissions")] bool importRoles = false,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source is null");
            }

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
                    Source = source,
                    ClearViews = clearViews,
                    ImportRoles = importRoles,
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