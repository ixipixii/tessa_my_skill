using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Platform.Data;
using Tessa.Properties.Resharper;
using Tessa.Scheme;
using Tessa.Views;
using Tessa.Views.Metadata;
using Tessa.Views.Metadata.Criteria;
using Tessa.Views.Metadata.Types;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrPermissionsViewInterceptor : IViewInterceptor
    {
        #region Fields

        private IDictionary<string, ITessaView> overlayedViews;

        [NotNull]
        private static readonly string[] views =
        {
            "KrPermissions",
        };

        #endregion

        #region Constructors

        public KrPermissionsViewInterceptor(
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

            // Удаляемый любую попытку из вне прокинуть этот параметр в представление
            RemoveParameter(request, "PermissionExpression");
            RemoveParameter(request, "ByPermissionExpression");

            await using (dbScope.Create())
            {
                var permissionsParameter = request.TryGetParameter("Permission");
                if (permissionsParameter != null)
                {
                    var builder = dbScope.BuilderFactory.Create().And().E(b =>
                    {
                        bool first = true;
                        foreach (var crit in permissionsParameter.CriteriaValues)
                        {
                            var flagID = (Guid)crit.Values[0].Value;
                            var flag = KrPermissionFlagDescriptors.Full.IncludedPermissions.FirstOrDefault(x => x.ID == flagID);

                            if (flag != null
                                && !flag.IsVirtual)
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    b.Or();
                                }

                                if (crit.CriteriaName == CriteriaOperatorConst.Equality)
                                {
                                    b.C("t", flag.SqlName).Equals().V(true);
                                }
                                else if (crit.CriteriaName == CriteriaOperatorConst.NonEquality)
                                {
                                    b.C("t", flag.SqlName).Equals().V(false);
                                }
                            }
                        }
                    });

                    request.Values.Add(
                        new RequestParameterBuilder()
                            .WithMetadata(new ViewParameterMetadata() { Alias = "PermissionExpression", SchemeType = SchemeType.String })
                            .AddCriteria(new EqualsCriteriaOperator(), string.Empty, builder.ToString())
                            .AsRequestParameter());

                    // Освобождаем билдер
                    builder.Build();
                }

                if (request.SubsetName == "ByPermission")
                {
                    var builder = dbScope.BuilderFactory.Create();
                    var first = true;

                    foreach(var flag in KrPermissionFlagDescriptors.Full.IncludedPermissions)
                    {
                        if (flag.IsVirtual)
                        {
                            continue;
                        }

                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            builder.UnionAll();
                        }

                        builder
                            .Select()
                                .V(flag.ID).As("PermissionID")
                                .V(flag.Description).As("PermissionName")
                                .C("ID")
                            .From("KrPermissions").NoLock()
                            .Where().C(flag.SqlName).Equals().V(true);
                    }

                    request.Values.Add(
                        new RequestParameterBuilder()
                            .WithMetadata(new ViewParameterMetadata() { Alias = "ByPermissionExpression", SchemeType = SchemeType.String })
                            .AddCriteria(new EqualsCriteriaOperator(), string.Empty, builder.ToString())
                            .AsRequestParameter());

                    // Освобождаем билдер
                    builder.Build();
                }

                return await view.GetDataAsync(
                    request,
                    cancellationToken);
            }
        }

        public void InitOverlay([NotNull] IDictionary<string, ITessaView> overlayViews) => this.overlayedViews = overlayViews;

        #endregion

        #region Private Methods

        private void RemoveParameter(ITessaViewRequest request, string paramName)
        {
            var expressionParameter = request.Values.TryGetParameter(paramName);
            if (expressionParameter != null)
            {
                request.Values.Remove(expressionParameter);
            }
        }

        #endregion
    }
}
