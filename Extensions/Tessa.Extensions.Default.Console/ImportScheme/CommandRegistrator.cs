﻿using System.IO;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.ImportScheme
{
    [ConsoleRegistrator]
    public sealed class CommandRegistrator :
        ConsoleRegistratorBase
    {
        public override void RegisterCommands()
        {
            this.CommandContext
                .AddCommand<TextWriter, TextWriter, string, string, string, string, string, bool, bool>(Command.ImportScheme)
                ;
        }
    }
}