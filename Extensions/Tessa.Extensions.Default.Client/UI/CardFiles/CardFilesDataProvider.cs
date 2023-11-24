using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Files;
using Tessa.Platform.Collections;
using Tessa.Platform.Validation;
using Tessa.Properties.Resharper;
using Tessa.Scheme;
using Tessa.UI.Cards.Controls;
using Tessa.UI.Files;
using Tessa.Views.Metadata;
using Tessa.Views.Metadata.Criteria;

namespace Tessa.Extensions.Default.Client.UI.CardFiles
{
    public class CardFilesDataProvider : IDataProvider
    {
        [NotNull]
        private readonly IFileViewModelCollection collection;
        
        /// <inheritdoc />
        public CardFilesDataProvider([NotNull] IFileViewModelCollection collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <inheritdoc />
        public async Task<IGetDataResponse> GetDataAsync(IGetDataRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = new GetDataResponse(new ValidationResultBuilder());
            AddFieldDescriptions(result);
            this.PopulateDataRows(request, result);
            return result;
        }

        private void PopulateDataRows(IGetDataRequest request, [NotNull] IGetDataResponse result)
        {
            var filter = this.GetFilter(request);
            var rows = new SortedSet<IDictionary<string, object>>(new FilesSorter(request));
            foreach (var file in this.collection.Where(filter).ToArray())
            {
                var row = new Dictionary<string, object>
                {
                    [CardFileCommonConsts.Caption] = file.Caption,
                    [CardFileCommonConsts.Size] = file.Model.Size,
                    [CardFileCommonConsts.Name] = file.Model.Name,
                    [CardFileCommonConsts.GroupCaption] = file.GroupCaption,
                    [CardFileCommonConsts.GroupSorting] = file.GroupSorting,
                    [CardFileCommonConsts.GroupId] = file.GroupID,
                    [CardFileCommonConsts.Key] = file.Model.ID
                };

                rows.Add(row);
            }
            
            result.Rows.AddRange(rows);
        }

        private Func<IFileViewModel, object> GetOrder(IGetDataRequest request)
        {
            return null;
        }

        private Func<IFileViewModel, bool> GetFilter(IGetDataRequest request)
        {
            return FileFilter.Create(request).Filter;
        }
        
        private static void AddFieldDescriptions([NotNull] IGetDataResponse result)
        {
            result.Columns.Add(new KeyValuePair<string, SchemeType>(CardFileCommonConsts.Caption, SchemeType.String));
            result.Columns.Add(new KeyValuePair<string, SchemeType>(CardFileCommonConsts.Size, SchemeType.Int64));
            result.Columns.Add(new KeyValuePair<string, SchemeType>(CardFileCommonConsts.Name, SchemeType.String));
            result.Columns.Add(
                new KeyValuePair<string, SchemeType>(CardFileCommonConsts.GroupCaption, SchemeType.String));
            result.Columns.Add(
                new KeyValuePair<string, SchemeType>(CardFileCommonConsts.GroupSorting, SchemeType.String));
            result.Columns.Add(new KeyValuePair<string, SchemeType>(CardFileCommonConsts.GroupId, SchemeType.String));
            result.Columns.Add(new KeyValuePair<string, SchemeType>(CardFileCommonConsts.Key, SchemeType.String));
        }

        private sealed class FileFilter
        {
            private readonly List<List<Func<IFileViewModel, bool>>> filter;

            [NotNull]
            private readonly IGetDataRequest request;

            /// <inheritdoc />
            private FileFilter([NotNull] IGetDataRequest request)
            {
                this.request = request ?? throw new ArgumentNullException(nameof(request));
                this.filter = new List<List<Func<IFileViewModel, bool>>>();
            }

            public bool Filter([NotNull] IFileViewModel fileObject)
            {
                return this.filter
                    .Select(block => block.Aggregate(false, (current, func) => current | func(fileObject)))
                    .Aggregate(true, (filterResult, blockResult) => filterResult & blockResult);
            }


            private void BuildFilter()
            {
                var alwaysTrueBlock = new List<Func<IFileViewModel, bool>> { AlwaysTrue };
                this.filter.Clear();
                this.filter.Add(alwaysTrueBlock);
                var requestParameters = this.BuildParametersCollectionFromRequest();
                foreach (var parameter in requestParameters)
                {
                    var filterBlock = new List<Func<IFileViewModel, bool>>();
                    foreach (var criteria in parameter.CriteriaValues)
                    {
                        AppendCriteriaToFilterFunc(criteria, filterBlock);
                    }

                    this.filter.Add(filterBlock);
                }
            }

            private static void AppendCriteriaToFilterFunc([NotNull] RequestCriteria criteria, [NotNull] ICollection<Func<IFileViewModel, bool>> filterBlock)
            {
                switch (criteria.CriteriaName)
                {
                    case CriteriaOperatorConst.Contains:
                        filterBlock.Add(f => f.Caption.IndexOf((string) criteria.Values.Single().Value,
                            StringComparison.OrdinalIgnoreCase) != -1);
                        break;
                    case CriteriaOperatorConst.Equality:
                        filterBlock.Add(f => string.Equals(f.Caption, (string) criteria.Values.Single().Value,
                            StringComparison.OrdinalIgnoreCase));
                        break;

                    case CriteriaOperatorConst.StartWith:
                        filterBlock.Add(f => f.Caption.StartsWith((string) criteria.Values.Single().Value,
                            StringComparison.OrdinalIgnoreCase));
                        break;

                    case CriteriaOperatorConst.EndWith:
                        filterBlock.Add(f => f.Caption.EndsWith((string) criteria.Values.Single().Value,
                            StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported Criteria:'{criteria.CriteriaName}'");
                }
            }

            [NotNull]
            private IEnumerable<RequestParameter> BuildParametersCollectionFromRequest()
            {
                var parametersCollection = new List<RequestParameter>();
                foreach (var action in this.request.ParametersActions)
                {
                    action(parametersCollection);
                }

                return parametersCollection;
            }

            private static bool AlwaysTrue([CanBeNull] IFileViewModel fileObject)
            {
                return true;
            }

            public static FileFilter Create(IGetDataRequest request)
            {
                var filter = new FileFilter(request);
                filter.BuildFilter();
                return filter;
            }
        }
    }

    internal class FilesSorter : IComparer<IDictionary<string, object>>
    {
        [NotNull]
        private readonly IGetDataRequest request;

        public FilesSorter([NotNull] IGetDataRequest request)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <inheritdoc />
        public int Compare(IDictionary<string, object> x, IDictionary<string, object> y)
        {
            if (!this.request.SortingColumns.Any())
            {
                return -1;
            }

            return (from column in this.request.SortingColumns let comparision = Comparer.Default.Compare(x[column.Alias], y[column.Alias]) where comparision != 0 select column.SortDirection == ListSortDirection.Ascending ? comparision : -comparision).FirstOrDefault();
        }
    }
}