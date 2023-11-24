using System.Threading;
using System.Threading.Tasks;
using Tessa.Forums;
using Tessa.Platform.Initialization;
using Tessa.Platform.Licensing;
using Tessa.Views.Workplaces;

namespace Tessa.Extensions.Default.Server.Forum
{
    /// <summary>
    /// Расширение скрывает узлы типовые дерева "Мои обсуждения" и "Последние обсуждения" в рабочем месте "Пользователь",
    /// если не включён соответствующий модуль лицензии. Без модуля невозможно перейти к обсуждениям из представления,
    /// поэтому узлы рекомендуется скрыть.
    /// </summary>
    public sealed class ForumWorkplaceInitialization : WorkplaceInitializationRule
    {
        #region Private Fields

        private readonly ILicenseManager licenseManager;

        #endregion

        #region Constructors

        public ForumWorkplaceInitialization(ILicenseManager licenseManager) =>
            this.licenseManager = licenseManager;

        #endregion

        #region Base Override

        /// <doc path='info[@type="WorkplaceInitializationRule" and @item="IsSatisfiedByAsync"]'/>
        public override async ValueTask<bool> IsSatisfiedByAsync(
            IWorkplaceComponentMetadata node,
            IWorkplaceInitializationContext context,
            CancellationToken cancellationToken = default) =>
            node.CompositionId != ForumHelper.MyTopicsViewID && node.CompositionId != ForumHelper.LastTopicsViewID
            || LicensingHelper.CheckForumLicense(this.licenseManager.License, out _);

        #endregion
    }
}