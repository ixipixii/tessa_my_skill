﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;
using Unity;

namespace Tessa.Extensions.Default.Console.ExportCards
{
    public static class Command
    {
        [Verb("ExportCards")]
        [LocalizableDescription("Common_CLI_ExportCards")]
        public static async Task ExportCards(
            [Output] TextWriter stdOut,
            [Error] TextWriter stdErr,
            [Argument] [LocalizableDescription("Common_CLI_CardIdentifiers")] IEnumerable<string> identifiers = null,
            [Argument("a")] [LocalizableDescription("Common_CLI_Address")] string address = null,
            [Argument("i")] [LocalizableDescription("Common_CLI_Instance")] string instanceName = null,
            [Argument("u")] [LocalizableDescription("Common_CLI_UserName")] string userName = null,
            [Argument("p")] [LocalizableDescription("Common_CLI_Password")] string password = null,
            [Argument("s")] [LocalizableDescription("Common_CLI_ColumnSeparator")] string separatorChar = null,
            [Argument("o")] [LocalizableDescription("Common_CLI_CardsOutputFolder")] string outputFolder = null,
            [Argument("l")] [LocalizableDescription("Common_CLI_CardLibraryPath")] string libraryFilePath = null,
            [Argument("localize")] [LocalizableDescription("Common_CLI_CardLocalizationCulture")] string culture = null,
            [Argument("b"), LocalizableDescription("Common_CLI_CardBinaryFormat")] bool binaryFormat = false,
            [Argument("q"), LocalizableDescription("Common_CLI_Quiet")] bool quiet = false,
            [Argument("nologo")] [LocalizableDescription("CLI_NoLogo")] bool nologo = false)
        {
            if (!nologo && !quiet)
            {
                ConsoleAppHelper.WriteLogo(stdOut);
            }

            CultureInfo localizationCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture);

            IUnityContainer container = new UnityContainer().ConfigureConsoleForClient(stdOut, stdErr, quiet, instanceName, address);

            var logger = container.Resolve<IConsoleLogger>();
            List<CardInfo> cardInfoList = await DefaultConsoleHelper.TryParseCardInfoListAsync(identifiers, logger, separatorChar);
            if (cardInfoList == null)
            {
                // ошибка парсинга
                ConsoleAppHelper.EnvironmentExit(-1);
                return;
            }

            int result;
            await using (var operation = container.Resolve<Operation>())
            {
                var context = new OperationContext
                {
                    CardInfoList = cardInfoList,
                    CardLibraryPath = libraryFilePath,
                    OutputFolder = outputFolder,
                    BinaryFormat = binaryFormat,
                    LocalizationCulture = localizationCulture,
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