using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Server.PnrCards
{
    public sealed class PartnerPermissionsGetExtension :
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
            if (card.TypeID != Guid.Parse("b9a1f125-ab1d-4cff-929f-5ad8351bda4f")    // Partner
                && card.CreatedByID == Guid.Parse("11111111-1111-1111-1111-111111111111")
                && card.Created < new DateTime(2020, 09, 19)
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
