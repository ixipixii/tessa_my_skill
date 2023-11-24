using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    /// <summary>
    /// Расширение для инициализации карточки при создании на основании другой карточки.
    /// </summary>
    public sealed class KrCreateBasedOnNewExtension :
        CardNewExtension
    {
        #region Constructors

        public KrCreateBasedOnNewExtension(
            ICardRepository extendedRepository,
            IUnityContainer unityContainer)
        {
            this.extendedRepository = extendedRepository;
            this.unityContainer = unityContainer;
        }

        #endregion

        #region Fields

        private readonly ICardRepository extendedRepository;

        private readonly IUnityContainer unityContainer;

        #endregion

        #region Base Overrides

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            Card newCard;
            Dictionary<string, object> info;
            Guid? baseCardID;

            if (context.CardType == null
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.RequestIsSuccessful
                || (info = context.Request.TryGetInfo()) == null
                || !(baseCardID = info.TryGet<Guid?>(KrCreateBasedOnHelper.CardIDKey)).HasValue
                || !context.ValidationResult.IsSuccessful()
                || (newCard = context.Response.TryGetCard()) == null)
            {
                return;
            }

            var getRequest = new CardGetRequest
            {
                CardID = baseCardID,
                GetMode = CardGetMode.ReadOnly,
                RestrictionFlags =
                    CardGetRestrictionFlags.RestrictTaskCalendar
                    | CardGetRestrictionFlags.RestrictTaskHistory,
            };

            var baseTokenStorage = info.TryGet<Dictionary<string, object>>(KrCreateBasedOnHelper.TokenKey);
            KrToken baseToken = baseTokenStorage != null ? KrToken.TryGet(baseTokenStorage) : null;

            if (baseToken != null)
            {
                baseToken.Set(getRequest.Info);
            }

            CardGetResponse getResponse = await this.extendedRepository.GetAsync(getRequest, context.CancellationToken);
            context.ValidationResult.Add(getResponse.ValidationResult);

            if (!getResponse.ValidationResult.IsSuccessful())
            {
                return;
            }

            Card baseCard = getResponse.Card;
            context.Info[KrCreateBasedOnHelper.CardKey] = baseCard;

            KrCreateBasedOnHelper.CopyMainInfo(baseCard, newCard);

            bool copyFiles = info.TryGet<bool>(KrCreateBasedOnHelper.CopyFilesKey);
            if (copyFiles)
            {
                // файлы копируются как псевдосоздание по шаблону, и у карточки newCard к этому моменту
                // должна быть корректная структура, в т.ч. должен быть задан непустой идентификатор карточки,
                // причём сам идентификатор при копировании не используется, но он влияет на валидацию структуры

                // поэтому если идентификатор пустой, то мы его создаём и по завершении копирования сбрасываем

                bool identifierChanged;
                if (newCard.ID == Guid.Empty)
                {
                    newCard.ID = Guid.NewGuid();
                    identifierChanged = true;
                }
                else
                {
                    identifierChanged = false;
                }

                ValidationResult result = await CardHelper.CopyFilesAsync(
                    baseCard, newCard, this.unityContainer, cancellationToken: context.CancellationToken);

                if (identifierChanged)
                {
                    newCard.ID = Guid.Empty;
                }

                context.ValidationResult.Add(result);
            }
        }

        #endregion
    }
}
