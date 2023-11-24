using System.Threading.Tasks;
using NLog;
using Tessa.Cards;
using Tessa.Extensions.Platform.Server.AdSync;
using Tessa.Ldap;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Roles;

namespace Tessa.Extensions.Default.Server.Cards
{
    public class DefaultAdExtension : AdExtension
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected static bool HasAccountLock(AdUserAccountControl accountControl) =>
            accountControl.Has(AdUserAccountControl.NORMAL_ACCOUNT | AdUserAccountControl.LOCKOUT)
            || accountControl.Has(AdUserAccountControl.NORMAL_ACCOUNT | AdUserAccountControl.ACCOUNTDISABLE);

        public override async Task SyncUser(IAdExtensionContext context)
        {
            AdEntry entry = context.Entry;
            Card card = context.Card;

            //account info
            string accountName = context.Connection.IsActiveDirectory ? entry.GetString(AdAttributes.SAmAccountName) : entry.GetString(AdAttributes.Uid);
            string login = context.Connection.IsActiveDirectory ? entry.DomainName + "\\" + accountName : entry.GetString(AdAttributes.Uid);
            string cn = entry.GetString(AdAttributes.Cn);
            string fullName = entry.GetString(AdAttributes.Name);
            string firstName = entry.GetString(AdAttributes.GivenName);
            string middleName = entry.GetString(AdAttributes.MiddleName);
            string displayName = entry.GetString(AdAttributes.DisplayName);
            string lastName = entry.GetString(AdAttributes.Sn);
            string position = entry.GetString(AdAttributes.Title);
            string name = GetFullName(cn, fullName, displayName);

            //contacts
            string fax = entry.GetString(AdAttributes.Fax);
            string email = entry.GetString(AdAttributes.Mail);

            string mobilePhone = entry.GetString(AdAttributes.Mobile);
            string ipPhone = entry.GetString(AdAttributes.IpPhone);
            string homePhone = entry.GetString(AdAttributes.HomePhone);
            string phone = entry.GetString(AdAttributes.TelephoneNumber);

            await using (context.DbScope.Create())
            {
                DbManager db = context.DbScope.Db;
                if (string.IsNullOrWhiteSpace(accountName))
                {
                    logger.Warn($"Login is empty for user ID {card.ID}. User is disabled.");
                    accountName = null;
                }
                else
                {
                    bool isNotUniqueLogin = await db
                        .SetCommand(context.DbScope.BuilderFactory
                            .Select().V(true).
                            From(RoleStrings.PersonalRoles).NoLock().
                            Where().C("ID").NotEquals().P("ID").
                            And().C("Login").Equals().P("Login").
                            Build(),
                        db.Parameter("ID", card.ID),
                        db.Parameter("Login", login))
                        .ExecuteAsync<bool>(context.CancellationToken);

                    if (isNotUniqueLogin)
                    {
                        logger.Warn($"Login {login} is not unique for user ID {card.ID}. User is disabled.");
                        accountName = null;
                    }
                }

                //Блокируем или разблокируем пользователя
                AdUserAccountControl adAccountControl = entry.GetUserAccountControl();
                bool isAccountDisabled = string.IsNullOrWhiteSpace(accountName) || HasAccountLock(adAccountControl);
                int loginTypeID = context.Connection.IsActiveDirectory
                    ? isAccountDisabled
                        ? (int) UserLoginType.None
                        : (int) UserLoginType.Windows
                    : (int) UserLoginType.Ldap;

                string loginType = await db.SetCommand(context.DbScope.BuilderFactory
                        .Select().C("Name").
                        From("LoginTypes").NoLock().
                        Where().C("ID").Equals().P("LoginTypeID").
                        Build(),
                    db.Parameter("LoginTypeID", loginTypeID))
                    .ExecuteAsync<string>(context.CancellationToken);

                string middleNamePr = GetMiddleName(middleName, name);
                string firstNameePr = GetFirstName(firstName, name);

                card.Sections[RoleStrings.PersonalRoles].Fields["Login"] = isAccountDisabled ? null : login;
                card.Sections[RoleStrings.PersonalRoles].Fields["LastName"] = lastName;
                card.Sections[RoleStrings.PersonalRoles].Fields["MiddleName"] = middleNamePr;
                card.Sections[RoleStrings.PersonalRoles].Fields["FirstName"] = firstNameePr;
                card.Sections[RoleStrings.PersonalRoles].Fields["FullName"] = name;
                card.Sections[RoleStrings.PersonalRoles].Fields["Name"] = GetShortName(firstNameePr, middleNamePr, lastName);
                card.Sections[RoleStrings.PersonalRoles].Fields["Email"] = email != null && email.Length > 128 ? email.Substring(0, 128) : email;
                card.Sections[RoleStrings.PersonalRoles].Fields["Fax"] = fax != null && fax.Length > 128 ? fax.Substring(0, 128) : fax;
                card.Sections[RoleStrings.PersonalRoles].Fields["Phone"] = phone != null && phone.Length > 64 ? phone.Substring(0, 64) : phone;
                card.Sections[RoleStrings.PersonalRoles].Fields["HomePhone"] = homePhone != null && homePhone.Length > 64 ? homePhone.Substring(0, 64) : homePhone;
                card.Sections[RoleStrings.PersonalRoles].Fields["MobilePhone"] = mobilePhone != null && mobilePhone.Length > 64 ? mobilePhone.Substring(0, 64) : mobilePhone;
                card.Sections[RoleStrings.PersonalRoles].Fields["IPPhone"] = ipPhone != null && ipPhone.Length > 64 ? ipPhone.Substring(0, 64) : ipPhone;
                card.Sections[RoleStrings.PersonalRoles].Fields["Position"] = position;
                card.Sections[RoleStrings.PersonalRoles].Fields["LoginTypeID"] = loginTypeID;
                card.Sections[RoleStrings.PersonalRoles].Fields["LoginTypeName"] = loginType;

                card.Sections[RoleStrings.Roles].Fields["Name"] = name;
                card.Sections[RoleStrings.Roles].Fields["Hidden"] = isAccountDisabled;
                card.Sections[RoleStrings.Roles].Fields["AdSyncWhenChanged"] = entry.GetWhenChanged();
                card.Sections[RoleStrings.Roles].Fields["AdSyncDistinguishedName"] = entry.DN;
            }
        }

        protected static string GetFullName(string cn, string fullName, string displayName)
        {
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }
            return !string.IsNullOrWhiteSpace(displayName) ? displayName : cn;
        }

        protected static string GetFirstName(string firstName, string name)
        {
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                return firstName;
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                int index = name.IndexOf(' ');
                return index > 0 ? name.Substring(0, index - 1) : name;
            }

            return null;
        }

        protected static string GetShortName(string firstName, string middleName, string lastName)
        {
            // если не указано сокращенное имя - формируем его как:
            // - Фамилия И.О., если указаны все поля ФИО
            // - Фамилия И., если Отчество не указано
            // - Имя, если Фамилия не указана

            var sb = StringBuilderHelper.Acquire().Append(lastName);

            if (!string.IsNullOrEmpty(firstName))
            {
                sb.Append(' ').Append(firstName[0]).Append('.');

                if (!string.IsNullOrEmpty(middleName))
                {
                    sb.Append(middleName[0]).Append('.');
                }
            }

            return sb.ToStringAndRelease();
        }

        protected static string GetMiddleName(string middleName, string fullName)
        {
            if (!string.IsNullOrWhiteSpace(middleName))
            {
                return middleName;
            }

            var words = fullName.Split(' ');
            if (words.Length < 3) //Если у нас только Фамилия + Имя
            {
                return null;
            }

            int index = fullName.LastIndexOf(' ');
            return index > 0 && index + 1 < fullName.Length ? fullName.Substring(index + 1) : fullName;
        }

        public override async Task SyncDepartment(IAdExtensionContext context)
        {
            LdapEntry entry = context.Entry;
            Card card = context.Card;
            card.Sections[RoleStrings.Roles].Fields["Hidden"] = BooleanBoxes.False;
            card.Sections[RoleStrings.Roles].Fields["AdSyncWhenChanged"] = entry.GetWhenChanged();
            card.Sections[RoleStrings.Roles].Fields["AdSyncDistinguishedName"] = entry.DN;
        }

        public override async Task SyncStaticRole(IAdExtensionContext context)
        {
            LdapEntry entry = context.Entry;
            Card card = context.Card;
            if (!context.Settings.DisableStaticRoleRename)
            {
                card.Sections[RoleStrings.Roles].Fields["Name"] = entry.GetString(AdAttributes.Name) ?? entry.GetString(AdAttributes.Cn);
            }
            card.Sections[RoleStrings.Roles].Fields["Description"] = entry.GetString(AdAttributes.Description);
            card.Sections[RoleStrings.Roles].Fields["Hidden"] = BooleanBoxes.False;
            card.Sections[RoleStrings.Roles].Fields["AdSyncWhenChanged"] = entry.GetWhenChanged();
            card.Sections[RoleStrings.Roles].Fields["AdSyncDistinguishedName"] = entry.DN;
        }
    }
}