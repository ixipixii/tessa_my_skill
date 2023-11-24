using System;
using System.Runtime.CompilerServices;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public static class KrAdditionalApprovalMarker
    {
        public const string AdditionalApproverMark = "(+) ";

        /// <summary>
        /// Добавить метку о наличии доп согласующих, если она отсутствует.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Mark(
            string name)
        {
            if (name.Length == 0
                || HasMark(name))
            {
                return name;
            }

            return name[0] == '$'
                ? "(+) {" + name + '}'
                : "(+) " + name;
        }

        /// <summary>
        /// Удалить метку о доп согласующих, если она присутствует.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Unmark(
            string name)
        {
            if (HasMark(name))
            {
                var from = 4;
                var len = name.Length - from;
                // (+) {} - Если расширенная локализация, скобки надо выпиливать.
                if (name.Length >= 6
                    && name[4] == '{'
                    && name[name.Length - 1] == '}')
                {
                    from++;
                    len -= 2;
                }
                return name.Substring(from, len);
            }

            return name;
        }

        /// <summary>
        /// Присутствует ли метка о доп согласующих.
        /// </summary>
        /// <param name="approverName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasMark(
            string approverName) =>
            approverName.StartsWith(AdditionalApproverMark, StringComparison.Ordinal);

    }
}