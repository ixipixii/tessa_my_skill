using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Runtime;
using Tessa.Views;
using Tessa.Views.Parser;
using Tessa.Views.Parser.ExpressionEval;
using Tessa.Views.Parser.Serialization;
using Tessa.Views.Parser.SyntaxTree;
using Tessa.Views.Parser.SyntaxTree.Workplace;
using Tessa.Views.SearchQueries;
using Tessa.Views.Workplaces;

namespace Tessa.Extensions.Default.Console.ExportWorkplaces
{
    public sealed class Operation :
        ConsoleOperation<OperationContext>
    {
        #region Constructors

        public Operation(
            IConsoleLogger logger,
            ConsoleSessionManager sessionManager,
            ITessaWorkplaceService workplaceService,
            ITessaViewService viewService,
            WorkplaceFilePersistent workplaceFilePersistent,
            ISession session,
            ISyntaxNodeConverter<IWorkplaceSyntaxNode> syntaxNodeConverter,
            LexemeParser parser)
            : base(logger, sessionManager, extendedInitialization: true)
        {
            // расширенная инициализация нужна для корректной локализации имён выгружаемых рабочих мест
            this.workplaceService = workplaceService ?? throw new ArgumentNullException(nameof(workplaceService));
            this.viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            this.workplaceFilePersistent = workplaceFilePersistent ?? throw new ArgumentNullException(nameof(workplaceFilePersistent));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.syntaxNodeConverter = syntaxNodeConverter ?? throw new ArgumentNullException(nameof(syntaxNodeConverter));
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        #endregion

        #region Fields

        private readonly ITessaWorkplaceService workplaceService;

        private readonly ITessaViewService viewService;

        private readonly WorkplaceFilePersistent workplaceFilePersistent;

        private readonly ISession session;

        private readonly ISyntaxNodeConverter<IWorkplaceSyntaxNode> syntaxNodeConverter;

        private readonly LexemeParser parser;

        private Dictionary<string, TessaViewModel> viewsByName;

        private Dictionary<Guid, ISearchQueryMetadata> searchQueriesByID;

        #endregion

        #region Private Methods

        private async Task ExportWorkplaceCoreAsync(
            WorkplaceModel workplace,
            string exportPath,
            bool includeViews,
            bool includeSearchQueries,
            CancellationToken cancellationToken = default)
        {
            (IList<string> viewAliases, IList<Guid> searchQueryIdentifiers) =
                includeViews || includeSearchQueries
                    ? await this.GetDependenciesAsync(workplace, cancellationToken)
                    : default;

            List<TessaViewModel> views =
                includeViews
                    ? await this.GetViewsAsync(viewAliases, cancellationToken)
                    : null;

            List<ISearchQueryMetadata> searchQueries =
                includeSearchQueries
                    ? await this.GetSearchQueriesAsync(searchQueryIdentifiers, cancellationToken)
                    : null;

            await this.Logger.InfoAsync("Saving workplace \"{0}\"", GetWorkplaceDisplayName(workplace));

            this.workplaceFilePersistent.Write(
                workplace,
                views ?? Enumerable.Empty<TessaViewModel>(),
                searchQueries ?? Enumerable.Empty<ISearchQueryMetadata>(),
                exportPath,
                this.session.User.Name);
        }


        private async ValueTask<List<TessaViewModel>> GetViewsAsync(
            IList<string> viewAliases,
            CancellationToken cancellationToken = default)
        {
            Dictionary<string, TessaViewModel> viewsByName = await this.GetViewsByNameAsync(cancellationToken);

            var result = new List<TessaViewModel>(viewAliases.Count);
            foreach (string viewAlias in viewAliases)
            {
                if (viewsByName.TryGetValue(viewAlias, out TessaViewModel view))
                {
                    result.Add(view);
                }
            }

            return result;
        }


        private async ValueTask<List<ISearchQueryMetadata>> GetSearchQueriesAsync(
            IList<Guid> searchQueryIdentifiers,
            CancellationToken cancellationToken = default)
        {
            Dictionary<Guid, ISearchQueryMetadata> searchQueriesByID = await this.GetSearchQueriesByIDAsync(cancellationToken);

            var result = new List<ISearchQueryMetadata>(searchQueryIdentifiers.Count);
            foreach (Guid searchQueryID in searchQueryIdentifiers)
            {
                if (searchQueriesByID.TryGetValue(searchQueryID, out ISearchQueryMetadata searchQuery))
                {
                    result.Add(searchQuery);
                }
            }

            return result;
        }


        private async ValueTask<Dictionary<string, TessaViewModel>> GetViewsByNameAsync(
            CancellationToken cancellationToken = default)
        {
            if (this.viewsByName != null)
            {
                return this.viewsByName;
            }

            await this.Logger.InfoAsync("Loading views from service...");
            TessaViewModel[] views = await this.viewService.GetModelsAsync(new GetModelRequest { WithRoles = true }, cancellationToken);

            var viewsByName = new Dictionary<string, TessaViewModel>(views.Length, StringComparer.OrdinalIgnoreCase);
            foreach (TessaViewModel view in views)
            {
                viewsByName[view.Alias] = view;
            }

            return this.viewsByName = viewsByName;
        }


        private async ValueTask<Dictionary<Guid, ISearchQueryMetadata>> GetSearchQueriesByIDAsync(
            CancellationToken cancellationToken = default)
        {
            if (this.searchQueriesByID != null)
            {
                return this.searchQueriesByID;
            }

            await this.Logger.InfoAsync("Loading public search queries from service...");
            IEnumerable<ISearchQueryMetadata> searchQueries = await this.viewService.GetPublicSearchQueriesAsync(cancellationToken);

            var searchQueriesByID = new Dictionary<Guid, ISearchQueryMetadata>();
            foreach (ISearchQueryMetadata searchQuery in searchQueries)
            {
                searchQueriesByID[searchQuery.Id] = searchQuery;
            }

            return this.searchQueriesByID = searchQueriesByID;
        }


        private async Task<(IList<string> viewAliases, IList<Guid> searchQueryIdentifiers)> GetDependenciesAsync(
            WorkplaceModel workplace,
            CancellationToken cancellationToken = default)
        {
            CodeBlockCollection lexemes = this.parser.Parse(workplace.Metadata);
            IEnumerable<IWorkplaceSyntaxNode> nodes;

            try
            {
                nodes = this.syntaxNodeConverter.Convert(lexemes, SyntaxConverterOptions.WorkplaceConvertingOptions);
            }
            catch (Exception ex)
            {
                await this.Logger.LogExceptionAsync($"Error parsing workplace \"{workplace.Name}\", ID={workplace.ID:B}", ex);
                return (EmptyHolder<string>.Array, EmptyHolder<Guid>.Array);
            }

            return await this.GetWorkplaceDependenciesAsync(
                nodes.FlatNodes<IWorkplaceSyntaxNode, IWorkplaceCompositeSyntaxNode>().ToArray(),
                cancellationToken);
        }


        private async Task<(IList<string> viewAliases, IList<Guid> searchQueryIdentifiers)> GetWorkplaceDependenciesAsync(
            IWorkplaceSyntaxNode[] nodes,
            CancellationToken cancellationToken = default)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            var searchQueries = new List<Guid>();
            var views = new List<string>();

            IWorkplaceKeywordSyntaxNode[] knownNodes =
                nodes
                    .OfType<IWorkplaceKeywordSyntaxNode>()
                    .Where(
                        n => ParserNames.IsAny(
                            new[] { KeywordNames.DataView, KeywordNames.SearchQuery, KeywordNames.View },
                            n.NodeType))
                    .ToArray();

            foreach (IWorkplaceKeywordSyntaxNode node in knownNodes)
            {
                var parametersNode = (IParametrizedKeywordSyntaxNode) node;
                ParametersDictionary parameters = SyntaxNodeHelper.SplitParams(parametersNode.Parameters);

                switch (node.NodeType)
                {
                    case KeywordNames.DataView:
                    case KeywordNames.View:
                        if (!parameters.ContainsKey("ALIAS"))
                        {
                            break;
                        }

                        var alias = parameters["ALIAS"].ToUpperInvariant();

                        var sourceType = MetadataDataSourceTypes.ViewSource;
                        if (parameters.ContainsKey("MetadataType"))
                        {
                            Enum.TryParse(parameters["MetadataType"], true, out sourceType);
                        }

                        if (sourceType == MetadataDataSourceTypes.SearchQuerySource)
                        {
                            Guid id = Guid.Parse(alias);
                            if (!searchQueries.Contains(id))
                            {
                                searchQueries.Add(id);
                            }

                            ISearchQueryMetadata searchQuery = await this.viewService.GetSearchQueryAsync(id, cancellationToken);
                            if (searchQuery != null)
                            {
                                alias = searchQuery.ViewAlias;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(alias) && !views.Contains(alias))
                        {
                            views.Add(alias);
                        }

                        break;

                    case KeywordNames.SearchQuery:
                        if (parameters.TryGetValue("ID", out string value))
                        {
                            Guid id = Guid.Parse(value);
                            if (!searchQueries.Contains(id))
                            {
                                searchQueries.Add(id);
                            }
                        }

                        if (!parameters.ContainsKey("VIEWALIAS"))
                        {
                            break;
                        }

                        alias = parameters["VIEWALIAS"].ToUpperInvariant();
                        if (!string.IsNullOrWhiteSpace(alias) && !views.Contains(alias))
                        {
                            views.Add(alias);
                        }

                        break;
                }
            }

            return (views, searchQueries);
        }


        private static string GetWorkplaceDisplayName(WorkplaceModel workplace) =>
            LocalizationManager.LocalizeOrGetName(
                workplace.Name,
                CultureInfo.GetCultureInfo(LocalizationManager.EnglishLanguageCode));

        #endregion

        #region Base Overrides

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(OperationContext context, CancellationToken cancellationToken = default)
        {
            if (!this.SessionManager.IsOpened)
            {
                return -1;
            }

            int exportedCount = 0;
            int notFoundCount = 0;

            try
            {
                string exportPath = DefaultConsoleHelper.NormalizeFolderAndCreateIfNotExists(context.OutputFolder);
                if (string.IsNullOrEmpty(exportPath))
                {
                    exportPath = Directory.GetCurrentDirectory();
                }

                if (context.ClearOutputFolder)
                {
                    await this.Logger.InfoAsync("Removing existent workplaces from output folder \"{0}\"", exportPath);

                    foreach (string filePath in Directory.EnumerateFiles(exportPath, "*.workplace", SearchOption.TopDirectoryOnly))
                    {
                        File.Delete(filePath);
                    }
                }

                await this.Logger.InfoAsync("Loading workplaces from service...");
                WorkplaceModel[] workplaces = (await this.workplaceService
                    .GetModelsAsync(new GetModelRequest { WithRoles = true }, cancellationToken)).ToArray();

                string optionsSuffix = null;
                if (context.IncludeViews)
                {
                    optionsSuffix += ", include views";
                }

                if (context.IncludeSearchQueries)
                {
                    optionsSuffix += ", include search queries";
                }

                if (context.WorkplaceNamesOrIdentifiers is null || context.WorkplaceNamesOrIdentifiers.Count == 0)
                {
                    await this.Logger.InfoAsync("Exporting all workplaces to folder \"{0}\"{1}", exportPath, optionsSuffix);

                    foreach (WorkplaceModel workplace in workplaces.OrderBy(x => x.ID))
                    {
                        await this.ExportWorkplaceCoreAsync(workplace, exportPath, context.IncludeViews, context.IncludeSearchQueries, cancellationToken);
                        exportedCount++;
                    }
                }
                else
                {
                    await this.Logger.InfoAsync(
                        "Exporting workplaces to folder \"{0}\"{1}: {2}",
                        exportPath,
                        optionsSuffix,
                        string.Join(", ", context.WorkplaceNamesOrIdentifiers.Select(name => "\"" + name + "\"")));

                    var workplacesByID = new Dictionary<Guid, WorkplaceModel>(workplaces.Length);
                    var workplacesByName = new Dictionary<string, WorkplaceModel>(workplaces.Length * 2, StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < workplaces.Length; i++)
                    {
                        string displayName = GetWorkplaceDisplayName(workplaces[i]);

                        // также запишем идентификатор для поиска по нему
                        workplacesByID[workplaces[i].ID] = workplaces[i];

                        // запишем как нелокализованное, так и локализованное имя, которые могут даже совпасть
                        workplacesByName[workplaces[i].Name] = workplaces[i];
                        workplacesByName[displayName] = workplaces[i];
                    }

                    foreach (string nameOrIdentifier in context.WorkplaceNamesOrIdentifiers)
                    {
                        if (workplacesByName.TryGetValue(nameOrIdentifier, out WorkplaceModel workplace)
                            || Guid.TryParse(nameOrIdentifier, out Guid workplaceID)
                            && workplacesByID.TryGetValue(workplaceID, out workplace))
                        {
                            await this.ExportWorkplaceCoreAsync(workplace, exportPath, context.IncludeViews, context.IncludeSearchQueries, cancellationToken);
                            exportedCount++;
                        }
                        else
                        {
                            await this.Logger.ErrorAsync("Workplace \"{0}\" isn't found", nameOrIdentifier);
                            notFoundCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error exporting workplaces", e);
                return -1;
            }

            // количество экспортированных выводим как при наличии, так и при отсутствии ошибок
            if (exportedCount > 0)
            {
                await this.Logger.InfoAsync("Workplaces ({0}) are exported successfully", exportedCount);
            }

            if (notFoundCount != 0)
            {
                await this.Logger.ErrorAsync("Workplaces ({0}) aren't found by provided names or identifiers", notFoundCount);
            }
            else if (exportedCount == 0)
            {
                await this.Logger.InfoAsync("No workplaces to export");
            }

            return 0;
        }

        #endregion
    }
}