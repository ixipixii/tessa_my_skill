using System.Collections.Generic;
using Tessa.Properties.Resharper;

// Это поддержка для генерации T4-шаблонов. В действительности соответствующие классы не используются.
// ReSharper disable once CheckNamespace
namespace System.CodeDom.Compiler
{
    [UsedImplicitly]
    internal class CompilerErrorCollection : List<CompilerError>
    {
    }

    internal class CompilerError
    {
        public string ErrorText { get; set; }

        public bool IsWarning { get; set; }
    }
}