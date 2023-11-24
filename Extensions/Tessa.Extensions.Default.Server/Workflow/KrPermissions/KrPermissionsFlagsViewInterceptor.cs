using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Localization;
using Tessa.Platform.Data;
using Tessa.Properties.Resharper;
using Tessa.Views;
using Tessa.Views.Metadata.Types;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrPermissionsFlagsViewInterceptor : IViewInterceptor
    {
        #region Fields

        private IDictionary<string, ITessaView> overlayedViews;

        [NotNull]
        private static readonly string[] views =
        {
            "KrPermissionFlags",
        };

        #endregion

        #region Constructors

        public KrPermissionsFlagsViewInterceptor(
            ISchemeTypeConverter schemeTypeConverter,
            IDbScope dbScope)
        {
            this.schemeTypeConverter = schemeTypeConverter;
            this.dbScope = dbScope;
        }

        #endregion

        #region IViewInterceptor Implementation

        private readonly ISchemeTypeConverter schemeTypeConverter;
        private readonly IDbScope dbScope;

        public string[] InterceptedViews => views;

        public async Task<ITessaViewResult> GetDataAsync(ITessaViewRequest request, CancellationToken cancellationToken = default)
        {
            if (this.overlayedViews == null)
            {
                throw new InvalidOperationException("Overlayed views is not initialized");
            }

            if (!this.overlayedViews.TryGetValue(
                request.ViewAlias ?? throw new InvalidOperationException("View alias isn't specified."),
                out ITessaView view))
            {
                throw new InvalidOperationException($"Can't find view with alias:'{request.ViewAlias}'");
            }

            var dbms = dbScope.Dbms;

            var preparedData = KrPermissionFlagDescriptors.Full.IncludedPermissions
                .Select(x => (ID: x.ID, Name: x.Description, LocalizedName: LocalizationManager.Localize(x.Description)));

            var sortDirection = request.SortDirection("FlagCaption");
            switch (sortDirection)
            {
                case "asc":
                    preparedData = preparedData.OrderBy(x => x.LocalizedName);
                    break;
                case "desc":
                    preparedData = preparedData.OrderByDescending(x => x.LocalizedName);
                    break;
            }

            request.IterateParameterCriterias("CaptionParam",
                crit =>
                {
                    if (crit.Values.Count > 0)
                    {
                        preparedData = preparedData.Where(x => x.LocalizedName.Contains((string)crit.Values[0].Value));
                    }
                });

            return
                new TessaViewResult
                {
                    SchemeTypes = (from c in view.Metadata.Columns select c.SchemeType).ToList(),
                    Columns = (from c in view.Metadata.Columns select (object)c.Alias).ToList(),
                    DataTypes = (from c in view.Metadata.Columns select (object)schemeTypeConverter.TryGetSqlTypeName(c.SchemeType, dbms)).ToList(),
                    Rows = preparedData.Select(x => (object)new List<object> { x.ID, x.Name }).ToList(),
                };
        }

        public void InitOverlay([NotNull] IDictionary<string, ITessaView> overlayViews) => this.overlayedViews = overlayViews;

        #endregion
    }
}
