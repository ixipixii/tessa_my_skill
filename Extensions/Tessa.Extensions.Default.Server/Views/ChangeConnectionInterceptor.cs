using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Properties.Resharper;
using Tessa.Views;

namespace Tessa.Extensions.Default.Server.Views
{
    /// <summary>
    /// Перехватчик представлений осуществляющий подмену соединения на котором
    /// требуется исполнить представление
    /// </summary>
    public sealed class ChangeConnectionInterceptor : IViewInterceptor
    {
        [NotNull]
        private static readonly string[] views = { "TestView" };

        private IDictionary<string, ITessaView> overlayedViews;

        /// <inheritdoc />
        public string[] InterceptedViews => views;

        /// <inheritdoc />
        public Task<ITessaViewResult> GetDataAsync(ITessaViewRequest request, CancellationToken cancellationToken = default)
        {
            if (this.overlayedViews == null)
            {
                throw new InvalidOperationException("Overlayed views is not initialized");
            }

            if (!this.overlayedViews.TryGetValue(request.ViewAlias, out ITessaView view))
            {
                throw new InvalidOperationException($"Can't find view with alias:'{request.ViewAlias}'");
            }

            request.ConnectionAlias = "mssql";
            return view.GetDataAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public void InitOverlay(IDictionary<string, ITessaView> overlayViews)
        {
            this.overlayedViews = overlayViews;
        }
    }
}
