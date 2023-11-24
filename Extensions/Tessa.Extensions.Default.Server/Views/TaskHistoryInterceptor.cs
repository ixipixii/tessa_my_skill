using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.ComponentModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Properties.Resharper;
using Tessa.Scheme;
using Tessa.Views;

namespace Tessa.Extensions.Default.Server.Views
{
    /// <summary>
    /// Перед возвратом данных представлением выполняем проверку или расчёт токена на карточку.
    /// </summary>
    public sealed class TaskHistoryInterceptor :
        IViewInterceptor
    {
        #region Constructors

        public TaskHistoryInterceptor(
            IKrTokenProvider krTokenProvider,
            ICardCache cardCache,
            ICardRepository cardRepository,
            ISession session,
            ICardMetadata cardMetadata,
            ICardGetStrategy getStrategy,
            IDbScope dbScope)
        {
            this.krTokenProvider = krTokenProvider;
            this.cardCache = cardCache;
            this.cardRepository = cardRepository;
            this.session = session;
            this.cardMetadata = cardMetadata;
            this.getStrategy = getStrategy;
            this.dbScope = dbScope;
        }

        #endregion

        #region Fields

        private IDictionary<string, ITessaView> overlayedViews;

        private readonly IKrTokenProvider krTokenProvider;

        private readonly ICardCache cardCache;

        private readonly ICardRepository cardRepository;

        private readonly ISession session;

        private readonly ICardMetadata cardMetadata;

        private readonly ICardGetStrategy getStrategy;

        private readonly IDbScope dbScope;

        [NotNull]
        private static readonly string[] views = { SystemViewAliases.TaskHistory };

        #endregion

        #region Private Methods

        private static TessaViewResult CreateEmptyResult()
        {
            return new TessaViewResult
            {
                Columns = new List<object>(),
                DataTypes = new List<object>(),
                SchemeTypes = new List<SchemeType>(),
                Result = new Dictionary<string, object>(),
                Rows = new List<object>(),
                RowCount = 0,
                HasTimeOut = false,
            };
        }

        #endregion

        #region IViewInterceptor Members

        /// <inheritdoc />
        public string[] InterceptedViews => views;

        /// <inheritdoc />
        public void InitOverlay(IDictionary<string, ITessaView> overlayViews) => this.overlayedViews = overlayViews;

        /// <inheritdoc />
        public async Task<ITessaViewResult> GetDataAsync(ITessaViewRequest request, CancellationToken cancellationToken = default)
        {
            if (this.overlayedViews is null)
            {
                throw new InvalidOperationException("Overlayed views is not initialized");
            }

            if (!this.overlayedViews.TryGetValue(
                request.ViewAlias ?? throw new InvalidOperationException("View alias isn't specified."),
                out ITessaView view))
            {
                throw new InvalidOperationException($"Can't find view with alias:'{request.ViewAlias}'");
            }

            await using (this.dbScope.Create())
            {
                if (request.TryGetParameter("CardID")?.CriteriaValues.FirstOrDefault()?.Values.FirstOrDefault()?.Value is Guid cardID)
                {
                    var validationResult = new ValidationResultBuilder();

                    CardGetContext getContext = await this.getStrategy.TryLoadCardInstanceAsync(
                        cardID, this.dbScope.Db, this.cardMetadata, validationResult, cancellationToken: cancellationToken);

                    if (getContext is null
                        || !validationResult.IsSuccessful()
                        || !(await this.cardMetadata.GetCardTypesAsync(cancellationToken)).TryGetValue(getContext.CardTypeID, out CardType cardType)
                        || !cardType.Flags.Has(CardTypeFlags.AllowTasks))
                    {
                        // или карточки с таким идентификатором нет в базе, или метаинформация по этому типу повреждена,
                        // или тип не поддерживает задания - дальнейшие проверки не имеют смысла
                        return CreateEmptyResult();
                    }

                    // административные карточки показываем только в случае, если текущий сотрудник - это администратор
                    if (cardType.Flags.Has(CardTypeFlags.Administrative))
                    {
                        return this.session.User.IsAdministrator()
                            ? await view.GetDataAsync(request, cancellationToken)
                            : CreateEmptyResult();
                    }

                    // карточки, входящие в типовое решение, должны подчиняться правилам доступа
                    if (KrComponentsHelper.GetKrComponents(cardType.ID, this.cardCache).Has(KrComponents.Base))
                    {
                        string tokenString = request.TryGetParameter("Token")?.CriteriaValues.FirstOrDefault()?.Values.FirstOrDefault()?.Value as string;
                        if (!string.IsNullOrEmpty(tokenString))
                        {
                            try
                            {
                                var tokenSerialized = new SerializableObject().FromBase64String(tokenString);
                                var token = new KrToken(tokenSerialized.GetStorage());

                                if (token.IsValid()
                                    && token.CardID == cardID
                                    && token.CardVersion > 0
                                    && token.HasPermission(KrPermissionFlagDescriptors.ReadCard)
                                    && token.ExpiryDate.ToUniversalTime() > DateTime.UtcNow)
                                {
                                    var tokenResult = new ValidationResultBuilder();
                                    int cardVersion = getContext.Card.Version;

                                    if (await this.krTokenProvider.ValidateTokenAsync(
                                            new Card { ID = cardID, Version = cardVersion },
                                            token, tokenResult) == KrTokenValidationResult.Success
                                        && tokenResult.IsSuccessful())
                                    {
                                        // токен валиден для актуальной версии карточки, в нём есть права на чтение карточки;
                                        // поэтому возвращаем данные представления
                                        return await view.GetDataAsync(request, cancellationToken);
                                    }
                                }
                            }
                            catch
                            {
                                // ignored
                            }
                        }

                        // либо токен не был задан, либо был, но не прошёл проверку, например,
                        // если версия карточки изменилась с момента отправки токена;
                        // рассчитываем токен заново, выполняя загрузку карточки с расширениями;
                        // если карточка успешно загрузится, то права присутствуют

                        bool canReadCard = false;

                        try
                        {
                            var getRequest = new CardGetRequest
                            {
                                CardID = cardID,
                                CardTypeID = cardType.ID,
                                GetMode = CardGetMode.ReadOnly,
                                RestrictionFlags = CardGetRestrictionFlags.RestrictFiles
                                    | CardGetRestrictionFlags.RestrictTaskCalendar
                                    | CardGetRestrictionFlags.RestrictTaskHistory
                            };

                            CardGetResponse getResponse = await this.cardRepository.GetAsync(getRequest, cancellationToken);
                            if (getResponse.ValidationResult.IsSuccessful())
                            {
                                canReadCard = true;
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        return canReadCard
                            ? await view.GetDataAsync(request, cancellationToken)
                            : CreateEmptyResult();
                    }

                    // карточка не связана с типовым решением и не является административной
                }

                // не указан идентификатор карточки
                return await view.GetDataAsync(request, cancellationToken);
            }
        }

        #endregion
    }
}
