using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Tasks;
using Tessa.Views;
using Tessa.Views.Metadata;
using Tessa.Views.Metadata.Criteria;

namespace Tessa.Extensions.Default.Client.Workflow.KrPermissions
{
    public sealed class KrTokenToTaskHistoryUIExtension :
        CardUIExtension
    {
        #region Constructors

        public KrTokenToTaskHistoryUIExtension(IKrTypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        #endregion

        #region Fields

        private readonly IKrTypesCache typesCache;

        #endregion

        #region Base Overrides

        public override async Task Initialized(ICardUIExtensionContext context)
        {
            TaskHistoryViewModel taskHistory;
            if (context.Model.Flags.InSpecialMode()
                || (taskHistory = context.Model.TryGetTaskHistory()) == null
                || KrComponentsHelper.GetKrComponents(context.Card, this.typesCache).HasNot(KrComponents.Base))
            {
                return;
            }

            Card card = context.Model.Card;
            taskHistory.ModifyOpenViewRequestAction += request =>
            {
                KrToken token = KrToken.TryGet(card.Info);
                if (token != null)
                {
                    string tokenString = token.GetStorage().ToSerializable().ToBase64String();

                    IViewParameterMetadata tokenMetadata = request.TaskHistoryViewMetadata.Parameters.FindByName("Token");
                    if (tokenMetadata != null)
                    {
                        RequestParameter tokenParameter = new RequestParameterBuilder()
                            .WithMetadata(tokenMetadata)
                            .AddCriteria(new EqualsCriteriaOperator(), tokenString, tokenString)
                            .AsRequestParameter();

                        request.Parameters.Add(tokenParameter);
                    }
                }
            };
        }

        #endregion
    }
}
