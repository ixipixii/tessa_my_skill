using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.SetToken
{
    public static class Command
    {
        [Verb("SetToken")]
        [LocalizableDescription("TokenEditor_CLI_Set")]
        public static async Task SetToken(
            [Input] TextReader input,
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument, LocalizableDescription("TokenEditor_CLI_Folder")] string serviceFolder,
            [Argument("signature"), LocalizableDescription("TokenEditor_CLI_Signature")] string signature = null,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (string.IsNullOrWhiteSpace(serviceFolder))
            {
                throw new ArgumentException("Please, specify path to the folder.");
            }

            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            IConsoleLogger logger = new ConsoleLogger(LogManager.GetLogger(nameof(SetToken)), stdOut, stdErr, quiet);
            string tokenSignature = signature ?? input.ReadLine();

            int result = await Operation.ExecuteAsync(logger, serviceFolder, tokenSignature);
            ConsoleAppHelper.EnvironmentExit(result);
        }
    }
}