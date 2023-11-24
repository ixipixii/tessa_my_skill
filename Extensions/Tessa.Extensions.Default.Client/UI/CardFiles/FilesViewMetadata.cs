using System.Windows.Forms.VisualStyles;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.Properties.Resharper;
using Tessa.Scheme;
using Tessa.Views.Metadata;
using Tessa.Views.Metadata.Criteria;

namespace Tessa.Extensions.Default.Client.UI.CardFiles
{
    public static class FilesViewMetadata
    {
        /// <summary>
        ///     Создает метаданные для поставщика данных предоставляющего данные о файлах
        /// </summary>
        /// <returns>Метаданные</returns>
        [NotNull]
        [PublicAPI]
        public static IViewMetadata Create()
        {
            var viewMetadata = new ViewMetadata
            {
                Alias = CardFileCommonConsts.SpecialFilesViewAlias,
                EnableAutoWidth = true
            };
            
            AddDefaultColumns(viewMetadata);
            AddDefaultParameters(viewMetadata);
            return viewMetadata;
        }


        /// <summary>
        ///     Проверяет является ли <paramref name="viewAlias" /> именем специального представления
        /// </summary>
        /// <param name="viewAlias">Псевдоним представления</param>
        /// <returns>Результат проверки</returns>
        [PublicAPI]
        public static bool IsFilesViewMetadata([CanBeNull] string viewAlias)
        {
            return viewAlias == CardFileCommonConsts.SpecialFilesViewAlias;
        }

        /// <summary>
        ///     Проверяет является ли <paramref name="viewMetadata" /> специальным представлением
        /// </summary>
        /// <param name="viewMetadata">Метаданные представления</param>
        /// <returns>Результат проверки</returns>
        [PublicAPI]
        public static bool IsFilesViewMetadata([CanBeNull] IViewMetadata viewMetadata)
        {
            return IsFilesViewMetadata(viewMetadata?.Alias);
        }

        private static void AddDefaultParameters([NotNull] IViewMetadata viewMetadata)
        {
            var nameParameter = new ViewParameterMetadata
            {
                Alias = CardFileCommonConsts.NameParameterAlias,
                Caption = LocalizationManager.Localize(CardFileCommonConsts.NameParameterText),
                SchemeType = SchemeType.String,
                Multiple = true,
                AllowedOperands = new[]
                    { CriteriaOperatorConst.Contains, CriteriaOperatorConst.StartWith, CriteriaOperatorConst.EndWith, CriteriaOperatorConst.Equality }
            };

            viewMetadata.Parameters.Add(nameParameter);
        }

        private static void AddDefaultColumns([NotNull] IViewMetadata viewMetadata)
        {
            var idColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.KeyText),
                Alias = CardFileCommonConsts.Key,
                SchemeType = SchemeType.Guid,
                Hidden = true
            };

            var captionColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.CaptionText),
                Alias = CardFileCommonConsts.Caption,
                SchemeType = SchemeType.String,
                SortBy = CardFileCommonConsts.Caption
            };

            var nameColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.NameText),
                Alias = CardFileCommonConsts.Name,
                SchemeType = SchemeType.String,
                Hidden = true
            };

            var sizeColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.SizeText),
                Alias = CardFileCommonConsts.Size,
                SchemeType = SchemeType.UInt64,
                SortBy = CardFileCommonConsts.Size
            };

            var groupCaptionColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.GroupCaptionText),
                Alias = CardFileCommonConsts.GroupCaption,
                SchemeType = SchemeType.String,
                Hidden = true
            };
            var groupIdColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.GroupIdText),
                Alias = CardFileCommonConsts.GroupId,
                SchemeType = SchemeType.Guid,
                Hidden = true
            };

            var groupSortingColumn = new ViewColumnMetadata
            {
                Caption = LocalizationManager.Localize(CardFileCommonConsts.GroupSortingText),
                Alias = CardFileCommonConsts.GroupSorting,
                SchemeType = SchemeType.String,
                Hidden = true
            };

            viewMetadata.Columns.AddRange(idColumn,
                captionColumn,
                nameColumn,
                sizeColumn,
                groupCaptionColumn,
                groupIdColumn,
                groupSortingColumn);
        }
    }
}