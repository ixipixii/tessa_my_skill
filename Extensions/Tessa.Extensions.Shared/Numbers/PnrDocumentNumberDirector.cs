using System;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards.Caching;
using Tessa.Cards.Numbers;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Numbers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Shared.PnrCards;

namespace Tessa.Extensions.Shared.Numbers
{
    /// <summary>
    ///     Объект, управляющий взаимодействием с номерами карточек,
    ///     реализующий присвоение различных последовательностей по условиям в рамках одного типа карточки
    /// </summary>
    public class PnrDocumentNumberDirector :
        DocumentNumberDirector
    {
        #region Constructors

        /// <summary>
        ///     Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="typesCache">Кэш типов карточек и документов, включённых в типовое решение.</param>
        /// <param name="dependencies">Объект, содержащий внешние зависимости API номеров.</param>
        /// <param name="cardCache">Кэш настроек карточкек</param>
        public PnrDocumentNumberDirector(
            IKrTypesCache typesCache,
            ICardCache cardCache,
            INumberDependencies dependencies)
            : base(typesCache, dependencies)
        {
            TypesCache = typesCache ?? throw new ArgumentNullException(nameof(typesCache));
            CardCache = cardCache ?? throw new ArgumentNullException(nameof(typesCache));
        }

        #endregion

        #region Event Base Overrides

        protected override async Task<bool> OnRegisteringCardAsync(INumberContext context,
            CancellationToken cancellationToken = default)
        {
            // если поля с Secondary-номерами отсутствуют в схеме карточки,
            // то номер не следует изменять или освобождать, при этом дерегистрация считается успешной,
            // т.к. регистрация в этом случае - только изменение состояния документа

            if (!DefaultSchemeHelper.CardTypeHasSecondaryNumber(context.CardType)) return true;

            var reservedNumber = await context.Builder
                .ReserveAndCommitAtServerAsync(context, NumberTypes.Primary, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return !reservedNumber.IsEmpty();
        }

        #endregion

        #region NumberBuilder Base Overrides

        protected override async ValueTask<bool> IsAvailableCoreAsync(INumberContext context, NumberEventType eventType,
            CancellationToken cancellationToken = default)
        {
            if (eventType.Equals(NumberEventTypes.RegisteringCard))
            {
                return true;
            }

            return await base.IsAvailableCoreAsync(context, eventType);
        }

        protected override async Task<string> TryGetSequenceNameCoreAsync(
            INumberContext context,
            NumberTypeDescriptor numberType,
            CancellationToken cancellationToken = default)
        {
            string sequenceName;
            IKrType type = await context.Builder.GetAsync<IKrType>(context, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (type != null)
            {
                if (context.EventType == NumberEventTypes.CreatingCard
                    || context.EventType == NumberEventTypes.SavingNewCard)
                {
                    sequenceName = type.DocNumberRegularSequence;
                }
                else if (context.EventType == NumberEventTypes.RegisteringCard)
                {
                    // Если это приказ
                    if (type.ID.Equals(PnrCardTypes.PnrOrderTypeID))
                    {
                        Guid? documentKindID = Guid.Parse(context.Card.Sections["PnrOrder"].RawFields["DocumentKindID"].ToString());
                        
                        switch (documentKindID)
                        {
                            case Guid standard when standard == PnrDocumentKinds.OrderAdministrativeActivity:
                                sequenceName = "Order_adm{yyyy}";
                                break;
                            case Guid standard when standard == PnrDocumentKinds.OrderMainActivity:
                                sequenceName = "Order_main{yyyy}";
                                break;
                            case Guid standard when standard == PnrDocumentKinds.OrderImplementation:
                                sequenceName = "Order_imp{yyyy}";
                                break;
                            case Guid standard when standard == PnrDocumentKinds.OrderMobileCommunications:
                                sequenceName = "Order_mob{yyyy}";
                                break;
                            case Guid standard when standard == PnrDocumentKinds.Disposal:
                                sequenceName = "Order_dis{yyyy}";
                                break;
                            default:
                                sequenceName = type.DocNumberRegistrationSequence;
                                break;
                        }
                        return await base.FormatSequenceNameAsync(context, numberType, sequenceName, cancellationToken)
                        .ConfigureAwait(false);
                    }
                    // Если это входящий
                    else if (type.ID.Equals(PnrCardTypes.PnrIncomingTypeID))
                    {
                        Guid? documentKindID = Guid.Parse(context.Card.Sections["PnrIncoming"].RawFields["DocumentKindID"].ToString());
                        
                        switch (documentKindID)
                        {
                            case Guid standard when standard == PnrIncomingTypes.IncomingComplaintsID:
                                sequenceName = "Incoming_com{yyyy}";
                                break;
                            case Guid standard when standard == PnrIncomingTypes.IncomingLetterID:
                                sequenceName = "Incoming_let{yyyy}";
                                break;
                            default:
                                sequenceName = type.DocNumberRegistrationSequence;
                                break;
                        }
                    }
                    // Если это исходящий
                    else if (type.ID.Equals(PnrCardTypes.PnrOutgoingTypeID))
                    {
                        Guid? documentKindID = Guid.Parse(context.Card.Sections["PnrOutgoing"].RawFields["DocumentKindID"].ToString());
                        
                        switch (documentKindID)
                        {
                            case Guid standard when standard == PnrOutgoingTypes.OutgoingComplaintsID:
                                sequenceName = "Outgoing_com{yyyy}";
                                break;
                            case Guid standard when standard == PnrOutgoingTypes.OutgoingLetterID:
                                sequenceName = "Outgoing_let{yyyy}";
                                break;
                            default:
                                sequenceName = type.DocNumberRegistrationSequence;
                                break;
                        }
                    }
                    else
                    {
                        sequenceName = type.DocNumberRegistrationSequence;
                    }
                }
                else
                {
                    KrState? state = await context.Builder.GetAsync<KrState?>(context, cancellationToken: cancellationToken).ConfigureAwait(false);
                    sequenceName = state == KrState.Registered
                        ? type.DocNumberRegistrationSequence
                        : type.DocNumberRegularSequence;
                }
            }
            else
            {
                sequenceName = await base.TryGetSequenceNameAsync(context, numberType, cancellationToken).ConfigureAwait(false);
            }

            return await this.FormatSequenceNameAsync(context, numberType, sequenceName, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Кэш типов карточек и документов, включённых в типовое решение.
        /// </summary>
        public IKrTypesCache TypesCache { get; }

        public ICardCache CardCache { get; }

        #endregion
    }
}