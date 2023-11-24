using System;
using System.Text.RegularExpressions;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public static class KrCompilersHelper
    {
        #region methods 

        private const string PrefixGroup = "pfx";
        private const string AliasGroup = "alias";
        private const string IDGroup = "id";
        private const string ClassNameGuidRegex = "^(?<" + PrefixGroup + ">[a-z,A-Z]+)_(?<" + AliasGroup + ">[a-z,A-Z,0-9]+)_(?<" + IDGroup + ">[a-f,0-9]{32})$";
        
        public static string FormatClassName(
            string prefix,
            string alias,
            Guid id) => $"{prefix}_{alias}_{id:N}";

        public static bool CorrectClassName(string str)
        {
            var match = Regex.Match(str, ClassNameGuidRegex, RegexOptions.Compiled | RegexOptions.Singleline);
            return match.Success;
        }
        
        /// <summary>
        /// Этап относится к указанной группе этапов.
        /// </summary>
        /// <param name="stageGroupID"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        public static bool ReferToGroup(
            Guid stageGroupID,
            Stage st) => st.StageGroupID == stageGroupID || stageGroupID == Guid.Empty;

        /// <summary>
        /// и карточки шаблонов этапов KrStageTemplates
        /// Очищает значения физической секции KrStages,
        /// чтобы не передавать дублированную информацию
        /// </summary>
        /// <param name="card"></param>
        public static void ClearPhysicalSections(Card card)
        {
            if (card.Sections.TryGetValue(KrConstants.KrStages.Name, out CardSection sec))
            {
                sec.Rows.Clear();
            }
        }

        #endregion
    }
}
