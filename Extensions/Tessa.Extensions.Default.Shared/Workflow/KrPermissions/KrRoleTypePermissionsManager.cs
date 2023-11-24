using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Shared.Workflow.KrPermissions
{
    public sealed class KrRoleTypePermissionsManager : IRoleTypePermissionsManager
    {
        #region Fields

        private readonly IKrTypesCache krTypesCache;
        private readonly IDbScope dbScope;

        #endregion

        #region Constructors

        public KrRoleTypePermissionsManager(
            IKrTypesCache krTypesCache,
            [OptionalDependency]IDbScope dbScope)
        {
            this.krTypesCache = krTypesCache;
            this.dbScope = dbScope;
        }

        #endregion

        #region IRoleTypePermissionsManager Implementation

        public ValueTask<bool> RoleTypeUseCustomPermissionsAsync(Guid roleTypeID, CancellationToken cancellationToken = default)
        {
            return new ValueTask<bool>(KrComponentsHelper.HasBase(roleTypeID, krTypesCache));
        }

        public async ValueTask<bool> RoleTypeUseCustomPermissionsOnMetadataAsync(Guid roleTypeID, CancellationToken cancellationToken = default)
        {
            if (dbScope != null)
            {
                await using (dbScope.Create())
                {
                    var db = dbScope.Db;

                    return
                        await db.SetCommand(
                            dbScope.BuilderFactory
                                .Select().Top(1).V(true)
                                .From("KrSettingsCardTypes").NoLock()
                                .Where().C("CardTypeID").Equals().P("RoleTypeID")
                                .Limit(1)
                                .Build(),
                            db.Parameter("RoleTypeID", roleTypeID))
                            .LogCommand()
                            .ExecuteAsync<bool>(cancellationToken).ConfigureAwait(false);
                }
            }

            return await RoleTypeUseCustomPermissionsAsync(roleTypeID, cancellationToken);
        }

        #endregion
    }
}
