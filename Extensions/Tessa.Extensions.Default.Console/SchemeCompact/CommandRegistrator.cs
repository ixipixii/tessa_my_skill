﻿using System.IO;
using Tessa.Platform.CommandLine;
using Tessa.Platform.ConsoleApps;

namespace Tessa.Extensions.Default.Console.SchemeCompact
{
    [ConsoleRegistrator]
    public sealed class CommandRegistrator :
        ConsoleRegistratorBase
    {
        public override void RegisterCommands()
        {
            this.CommandContext
                .AddCommand<TextWriter, TextWriter, string, string, bool, bool>(Command.SchemeCompact)
                ;
        }
    }
}