using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class CardPermissionsGetExtension :
        CardGetExtension
    {
        #region Base Overrides

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;
            if (!context.RequestIsSuccessful
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }

            // Запрет редактирования всех мигрированных карточек кроме Сотрудников, Организаций
            if ((card.TypeID != Guid.Parse("929ad23c-8a22-09aa-9000-398bf13979b2")      // PersonalRole
                && card.TypeID != Guid.Parse("a668f7ea-efcd-47f0-a3c7-c4d1e7ed0bc8")    // PnrOrganization
                && card.TypeID != Guid.Parse("b9a1f125-ab1d-4cff-929f-5ad8351bda4f")    // Partner
                && card.TypeID != Guid.Parse("c17a5031-e7d8-4ea6-b03f-4c88b4bd6063")    // PnrProject
                && card.TypeID != Guid.Parse("38bbd7ed-ab6f-4a12-81c2-ea0069da316f")    // PnrCFO                
                && card.TypeID != Guid.Parse("1c7a5718-09ae-4f65-aa67-e66f23bb7aee")    // PnrContract
                && card.TypeID != PnrCardTypes.PnrIncomingTypeID                        // PnrIncoming
                && card.TypeID != PnrCardTypes.PnrOutgoingTypeID                        // PnrOutgoing
                )
                && card.Sections.TryGetValue("DocumentCommonInfo", out var documentCommonInfo)
                && documentCommonInfo.Fields.TryGet<bool?>("IsMigrated") == true
                && !context.Session.User.ID.Equals(Guid.Parse("11111111-1111-1111-1111-111111111111")))
            {
                card.Permissions.SetCardPermissions(
                    CardPermissionFlags.ProhibitModify |
                    CardPermissionFlags.ProhibitDeleteCard |
                    CardPermissionFlags.ProhibitDeleteFile |
                    CardPermissionFlags.ProhibitDeleteRow |
                    CardPermissionFlags.ProhibitEditNumber |
                    CardPermissionFlags.ProhibitInsertFile |
                    CardPermissionFlags.ProhibitReplaceFile |
                    CardPermissionFlags.ProhibitSignFile);
            }
        }

        #endregion
    }
}
