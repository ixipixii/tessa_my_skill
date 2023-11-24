using System;
using System.IO;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;
using Unity;

namespace Tessa.Extensions.Default.Console.ImportCards
{
    public static class Command
    {
        [Verb("ImportCards")]
        [LocalizableDescription("Common_CLI_ImportCards")]
        public static async Task ImportCards(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument] [LocalizableDescription("Common_CLI_SourceCardLibrary")] string source,
            [Argument("a")] [LocalizableDescription("Common_CLI_Address")] string address = null,
            [Argument("i")] [LocalizableDescription("Common_CLI_Instance")] string instanceName = null,
            [Argument("u")] [LocalizableDescription("Common_CLI_UserName")] string userName = null,
            [Argument("p")] [LocalizableDescription("Common_CLI_Password")] string password = null,
            [Argument("c")] [LocalizableDescription("Common_CLI_CanDeleteExistentCards")] bool canDeleteExistentCards = false,
            [Argument("e")] [LocalizableDescription("Common_CLI_IgnoreExistentCards")] bool ignoreExistentCards = false,
            [Argument("r")] [LocalizableDescription("Common_CLI_IgnoreRepairMessages")] bool ignoreRepairMessages = false,
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
                    CanDeleteExistentCards = canDeleteExistentCards,
                    IgnoreExistentCards = ignoreExistentCards,
                    IgnoreRepairMessages = ignoreRepairMessages,
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