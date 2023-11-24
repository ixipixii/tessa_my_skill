using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Metadata;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public abstract class KrCardMetadataExtensionBase :
        CardTypeMetadataExtension
    {
        #region nested types

        private sealed class KrFillBlockForDocStatusVisitor : CardTypeVisitor
        {
            private readonly CardTypeControl docStateControl;

            private readonly CardTypeControl docStateChangedControl;

            /// <summary>
            /// Создаёт экземпляр класса с указанием
            /// </summary>
            /// <param name="validationResult">Результат валидации, полученный посредством посещения объектов типа карточки.</param>
            /// <param name="docStateControl"></param>
            /// <param name="docStateChangedControl"></param>
            public KrFillBlockForDocStatusVisitor(
                IValidationResultBuilder validationResult,
                CardTypeControl docStateControl,
                CardTypeControl docStateChangedControl)
                : base(validationResult)
            {
                this.docStateControl = docStateControl;
                this.docStateChangedControl = docStateChangedControl;
            }

            public override ValueTask VisitBlockAsync(
                CardTypeBlock block,
                CardTypeForm form,
                CardType type,
                CancellationToken cancellationToken = default)
            {
                if (block.Name == KrConstants.Ui.KrBlockForDocStatusAlias)
                {
                    block.Controls.Add(this.docStateControl);
                    block.Controls.Add(this.docStateChangedControl);
                }

                return new ValueTask();
            }
        }

        #endregion

        #region Constructors

        protected KrCardMetadataExtensionBase(ICardMetadata clientCardMetadata)
            : base(clientCardMetadata)
        {
        }

        protected KrCardMetadataExtensionBase()
            : base()
        {
        }

        #endregion

        #region Fields

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region abstract methods

        protected abstract Task ExtendKrTypesAsync(
            IList<CardType> krTypes,
            ICardMetadataExtensionContext context);

        protected abstract ValueTask<CardMetadataSectionCollection> GetAllSectionsAsync(
            ICardMetadataExtensionContext context);

        protected abstract Task<List<Guid>> GetCardTypeIDsAsync(CancellationToken cancellationToken = default);

        #endregion

        #region Private Methods

        private static bool CheckCardType(CardType type, List<Guid> allowedCardTypeIDs) =>
            type.InstanceType == CardInstanceType.Card && allowedCardTypeIDs.Contains(type.ID);

        private static ValueTask EnhanceAsync(
            CardType krCardType,
            CardType targetCardType,
            CardMetadataSectionCollection enhanceableSections,
            CardMetadataSectionCollection allSections,
            CancellationToken cancellationToken = default)
        {
            // быстрее скопировать весь тип карточки, чем отдельные его части
            CardType krCardTypeClone = krCardType.DeepClone();

            targetCardType.SchemeItems.AddRange(krCardTypeClone.SchemeItems);
            targetCardType.Validators.AddRange(krCardTypeClone.Validators);
            targetCardType.Extensions.AddRange(krCardTypeClone.Extensions);

            targetCardType.Forms.AddRange(krCardTypeClone.Forms);

            for (int i = 0; i < targetCardType.Forms.Count; i++)
            {
                targetCardType.Forms[i].TabOrder = i;
            }

            CardMetadataSectionCollection sectionsToClone = null;
            foreach (CardTypeSchemeItem schemeItem in krCardTypeClone.SchemeItems)
            {
                Guid sectionID = schemeItem.SectionID;
                if (enhanceableSections.TryGetValue(sectionID, out CardMetadataSection metadataSection))
                {
                    foreach (CardMetadataColumn column in metadataSection.Columns)
                    {
                        column.CardTypeIDList.Add(targetCardType.ID);
                    }
                }
                else
                {
                    // мы на клиенте, там не все секции в текущей мете
                    CardMetadataSection section = allSections[sectionID];
                    if (sectionsToClone is null)
                    {
                        sectionsToClone = new CardMetadataSectionCollection();
                    }

                    sectionsToClone.Add(section);
                }
            }

            if (sectionsToClone != null)
            {
                // клонируем все секции за раз для оптимизации, используется на клиенте в предпросмотре типов
                foreach (CardMetadataSection section in sectionsToClone.DeepClone())
                {
                    foreach (CardMetadataColumn column in section.Columns)
                    {
                        column.CardTypeIDList.Add(targetCardType.ID);
                    }

                    enhanceableSections.Add(section);
                }
            }

            //Далее достанем из krCard контрол состояния документа и контрол последнего времени
            //его изменения и, если в целевом типе карточки есть блок с специальным именем,
            //добавим туда эти контролы
            return SubstituteDocStateControlAsync(krCardTypeClone, targetCardType, cancellationToken);
        }

        private static async ValueTask SubstituteDocStateControlAsync(
            CardType krCardTypeClone,
            CardType targetCardType,
            CancellationToken cancellationToken = default)
        {
            CardTypeControl docStateControl = null;
            CardTypeControl docStateChangedControl = null;

            foreach (CardTypeNamedForm form in krCardTypeClone.Forms)
            {
                foreach (CardTypeBlock block in form.Blocks)
                {
                    foreach (CardTypeControl control in block.Controls)
                    {
                        if (control.Name == "DocStateControl")
                        {
                            docStateControl = control;
                            if (docStateChangedControl != null)
                            {
                                await VisitAsync(targetCardType, docStateControl, docStateChangedControl, cancellationToken).ConfigureAwait(false);
                                return;
                            }
                        }
                        else if (control.Name == "DocStateChangedControl")
                        {
                            docStateChangedControl = control;
                            if (docStateControl != null)
                            {
                                await VisitAsync(targetCardType, docStateControl, docStateChangedControl, cancellationToken).ConfigureAwait(false);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static ValueTask VisitAsync(
            CardType targetCardType,
            CardTypeControl docStateControl,
            CardTypeControl docStateChangedControl,
            CancellationToken cancellationToken = default)
        {
            var validationResult = new ValidationResultBuilder();
            var visitor = new KrFillBlockForDocStatusVisitor(
                validationResult,
                docStateControl,
                docStateChangedControl);
            return targetCardType.VisitAsync(visitor, cancellationToken);
        }

        #endregion

        #region Base Overrides

        public override async Task ModifyMetadata(ICardMetadataExtensionContext context)
        {
            try
            {
                CardType krCardType = await this.TryGetCardTypeAsync(context, DefaultCardTypes.KrCardTypeID).ConfigureAwait(false);
                CardType krStageTemplateCardType = await this.TryGetCardTypeAsync(context, DefaultCardTypes.KrStageTemplateTypeID).ConfigureAwait(false);
                CardType secondaryProcessCardType = await this.TryGetCardTypeAsync(context, DefaultCardTypes.KrSecondaryProcessTypeID).ConfigureAwait(false);
                if (krCardType is null)
                {
                    return;
                }

                await this.ExtendKrTypesAsync(new List<CardType> { krCardType, krStageTemplateCardType, secondaryProcessCardType, }, context).ConfigureAwait(false);

                var allowedCardTypeIDs = await this.GetCardTypeIDsAsync(context.CancellationToken).ConfigureAwait(false);
                if (allowedCardTypeIDs is null)
                {
                    return;
                }

                var targetCardTypes = (await context.CardMetadata.GetCardTypesAsync(context.CancellationToken).ConfigureAwait(false))
                    .Where(x => CheckCardType(x, allowedCardTypeIDs));

                var enhanceableSections = await context.CardMetadata.GetSectionsAsync(context.CancellationToken).ConfigureAwait(false);
                var allSections = await this.GetAllSectionsAsync(context).ConfigureAwait(false);

                foreach (CardType targetCardType in targetCardTypes)
                {
                    if (!targetCardType.IsSealed)
                    {
                        await EnhanceAsync(krCardType, targetCardType, enhanceableSections, allSections, context.CancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, LogLevel.Error);
            }
        }

        #endregion
    }
}