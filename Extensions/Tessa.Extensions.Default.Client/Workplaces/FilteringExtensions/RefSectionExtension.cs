#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Json;
using Tessa.Json.Bson;
using Tessa.Properties.Resharper;
using Tessa.UI;
using Tessa.UI.Views;
using Tessa.UI.Views.Extensions;
using Tessa.UI.Views.Workplaces.Tree;
using Tessa.Views.Metadata;
using Tessa.Views.Parser;
using Tessa.Views.Workplaces;
using Tessa.Extensions.Default.Shared.Workplaces;

#endregion

namespace Tessa.Extensions.Default.Client.Workplaces
{
    /// <summary>
    /// Расширение на узел рабочего места, которое задаёт список значений RefSection и параметров представлений,
    /// которые используются для узла при его скрытии или отображении в режиме отбора (по троеточию из ссылочных контролов),
    /// независимо от представлений в узле.
    /// </summary>
    public sealed class RefSectionExtension :
        ViewModel<EmptyModel>, ITreeItemExtension, IWorkplaceExtensionSettingsRestore,
        IWorkplaceFilteringRule
    {
        #region Fields

        [NotNull]
        private ITreeItemFilteringSettings settings;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public RefSectionExtension()
        {
            this.settings = new TreeItemFilteringSettings();
        }

        #endregion

        #region Public methods and operators

        /// <inheritdoc />
        public void Clone(ITreeItem source, ITreeItem cloned, ICloneableContext context)
        {
        }

        /// <inheritdoc />
        public ValueTask<CheckingResult> EvaluateAsync(IWorkplaceComponentMetadata metadata, IWorkplaceFilteringContext context) =>
            new ValueTask<CheckingResult>(
                ConditionEquals(context, this.settings) ? CheckingResult.Positive : CheckingResult.Negative);

        /// <inheritdoc />
        public void Initialize(ITreeItem model)
        {
        }

        /// <inheritdoc />
        public void Initialized(ITreeItem model)
        {
        }

        /// <inheritdoc />
        public void Restore(byte[] metadata)
        {
            using var stream = new MemoryStream(metadata);
            using var bsonReader = new BsonReader(stream);
            
            this.settings = new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                .Deserialize<ITreeItemFilteringSettings>(bsonReader);
        }

        #endregion

        #region Other methods

        private static bool ParametersEquals([CanBeNull] IEnumerable<RequestParameter> contextParameters,
            [NotNull] IEnumerable<string> settingsParameters)
        {
            if (contextParameters == null || !contextParameters.Any())
            {
                return true;
            }

            return contextParameters.Select(p => p.Metadata.Alias)
                .All(x => ParserNames.IsAny(settingsParameters.ToArray(), x));
        }

        private static bool ConditionEquals([NotNull] IWorkplaceFilteringContext context,
            [NotNull] ITreeItemFilteringSettings settings)
        {
            return settings != null &&
                ParserNames.IsAny(settings.RefSections, context.RefSection)
                && ParametersEquals(context.Parameters, settings.Parameters);
        }

        #endregion
    }
}