using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Forums;
using Tessa.Platform;
using Tessa.Platform.Licensing;

namespace Tessa.Extensions.Default.Server.Cards
{
    public sealed class KrSettingsForumLicenseGetExtension : CardGetExtension
    {
        #region Fields

        private readonly ILicenseManager licenseManager;

        #endregion

        #region Constructors

        public KrSettingsForumLicenseGetExtension(ILicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        #endregion

        #region Base Overrides

        public override Task AfterRequest(ICardGetExtensionContext context)
        {
            if (!context.RequestIsSuccessful ||
                this.licenseManager.License.Modules.HasEnterpriseOrContains(LicenseModules.ForumsID))
            {
                return Task.CompletedTask;
            }

            // расширение зарегистрировано только для этих двух типов карточек
            Card card = context.Response.Card;
            if (context.CardTypeIs(DefaultCardTypes.KrSettingsTypeID))
            {
                card.Permissions.Sections.GetOrAddTable("KrSettingsCardTypes").FieldPermissions["UseForum"] = CardPermissionFlags.ProhibitModify;
            }
            else if (context.CardTypeIs(DefaultCardTypes.KrDocTypeTypeID))
            {
                card.Permissions.Sections.GetOrAddEntry("KrDocType").FieldPermissions["UseForum"] = CardPermissionFlags.ProhibitModify;
            }

            card.Info[ForumHelper.LicenseWarningFlag] = BooleanBoxes.True;
            return Task.CompletedTask;
        }

        #endregion
    }
}