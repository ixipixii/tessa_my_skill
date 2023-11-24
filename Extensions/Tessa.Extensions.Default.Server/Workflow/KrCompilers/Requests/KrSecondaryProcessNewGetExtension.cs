using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrSecondaryProcessNewGetExtension : KrTemplateNewGetExtension
    {
        #region constructor

        /// <inheritdoc />
        public KrSecondaryProcessNewGetExtension(
            IKrStageSerializer serializer)
            : base(serializer)
        {
        }

        #endregion

        #region public

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            await base.AfterRequest(context);
            await AfterRequestInternalAsync(context, context.Response.Card);
        }

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            await base.AfterRequest(context);
            await AfterRequestInternalAsync(context, context.Response.Card);
        }

        #endregion

        #region private

        private static async Task AfterRequestInternalAsync(
            ICardExtensionContext context,
            Card card)
        {
            if (!context.ValidationResult.IsSuccessful()
                || card is null)
            {
                return;
            }

            var sec = card.Sections[KrConstants.KrSecondaryProcessGroupsVirtual.Name];
            await using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                var query = context.DbScope.BuilderFactory
                    .Select().C(null, KrConstants.ID, KrConstants.Name)
                    .From(KrConstants.KrStageGroups.Name).NoLock()
                    .Where().C(KrConstants.KrStageGroups.KrSecondaryProcessID).Equals().P("processID")
                    .Build();
                db.SetCommand(query, db.Parameter("processID", card.ID));
                await using var reader = await db.ExecuteReaderAsync(context.CancellationToken);
                while (await reader.ReadAsync(context.CancellationToken))
                {
                    var row = sec.Rows.Add();
                    row.RowID = reader.GetGuid(0);
                    row[KrConstants.StageGroupID] = reader.GetGuid(0);
                    row[KrConstants.StageGroupName] = reader.GetString(1);
                }
            }
        }

        #endregion

    }
}