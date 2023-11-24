using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Files;
using Tessa.Extensions.Default.Server.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform.Data;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Проверка токена - должна выполняться для основной карточки.
    /// </summary>
    public class WfTaskSatelliteGetFileVersionsExtension :
        TaskSatelliteGetFileVersionsExtension
    {
        #region Constructors

        public WfTaskSatelliteGetFileVersionsExtension(
            IKrPermissionsManager permissionsManager)
        {
            this.permissionsManager = permissionsManager;
        }

        #endregion

        #region Fields

        private readonly IKrPermissionsManager permissionsManager;

        #endregion

        #region Base Overrides

        protected override Task<Guid?> TryGetMainCardIDFromSatelliteIDAsync(
            IDbScope dbScope,
            Guid satelliteID,
            CancellationToken cancellationToken = default) =>
            WfHelper.TryGetMainCardIDFromSatelliteIDAsync(dbScope, satelliteID, cancellationToken);


        protected override async Task CheckAccessAsync(ICardGetFileVersionsExtensionContext context, Guid mainCardID)
        {
            try
            {
                // проверяем доступ по основной карточке
                await KrFileAccessHelper.CheckAccessAsync(
                    context.Request,
                    context,
                    mainCardID,
                    permissionsManager,
                    context.CancellationToken);
            }
            finally
            {
                // чтобы типовое расширение на проверку прав не проверяло токен не от своей карточки
                KrToken.Remove(context.Request.Info);
            }
        }

        #endregion
    }
}
