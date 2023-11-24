using System.Collections.Generic;

// Это поддержка для генерации T4-шаблонов. В действительности соответствующие классы не используются.
// ReSharper disable once CheckNamespace
namespace System.CodeDom.Compiler
{
    internal class CompilerErrorCollection : List<CompilerError>
    {
    }

    internal class CompilerError
    {
        public string ErrorText { get; set; }

        public bool IsWarning { get; set; }
    }
}