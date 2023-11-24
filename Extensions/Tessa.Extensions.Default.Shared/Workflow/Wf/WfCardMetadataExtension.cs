using System;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.Wf
{
    public sealed class WfCardMetadataExtension :
        CardTypeMetadataExtension
    {
        #region Constructors

        public WfCardMetadataExtension(ICardMetadata clientCardMetadata)
            : base(clientCardMetadata)
        {
        }

        public WfCardMetadataExtension()
            : base()
        {
        }

        #endregion

        #region Private Methods

        private static void CopyMainFormToOtherForms(CardType sourceType)
        {
            foreach (CardTypeNamedForm namedForm in sourceType.Forms)
            {
                sourceType.Blocks.CopyToTheBeginningOf(namedForm.Blocks);
                StorageHelper.Merge(sourceType.FormSettings, namedForm.FormSettings);
            }
        }


        private static void CopyResolutionTaskType(CardType sourceType, CardType targetType)
        {
            sourceType.Blocks.CopyToTheBeginningOf(targetType.Blocks);
            sourceType.SchemeItems.CopyToTheBeginningOf(targetType.SchemeItems);
            sourceType.Forms.CopyToTheBeginningOf(targetType.Forms);
            sourceType.CompletionOptions.CopyToTheBeginningOf(targetType.CompletionOptions);
            sourceType.Validators.CopyToTheBeginningOf(targetType.Validators);
            sourceType.Extensions.CopyToTheBeginningOf(targetType.Extensions);

            StorageHelper.Merge(sourceType.FormSettings, targetType.FormSettings);
        }

        #endregion

        #region Base Overrides

        public override async Task ModifyTypes(ICardMetadataExtensionContext context)
        {
            CardType resolutionType = await this.TryGetCardTypeAsync(context, DefaultTaskTypes.WfResolutionTypeID).ConfigureAwait(false);
            if (resolutionType == null)
            {
                return;
            }

            if (!resolutionType.IsSealed)
            {
                // тип получен с сервера, скорее всего для предпросмотра в редакторе типов карточек
                CopyMainFormToOtherForms(resolutionType);
            }

            foreach (Guid taskTypeID in WfHelper.MetadataResolutionTaskTypeIDList)
            {
                CardType taskType = await this.TryGetCardTypeAsync(context, taskTypeID, useServerMetadataOnClient: false).ConfigureAwait(false);
                if (taskType != null)
                {
                    CopyResolutionTaskType(resolutionType, taskType);

                    // для проекта резолюций не должно быть отзыва
                    SealableObjectList<CardTypeCompletionOption> options = taskType.CompletionOptions;
                    if (taskTypeID == DefaultTaskTypes.WfResolutionProjectTypeID)
                    {
                        int revokeOptionIndex = options.IndexOf(x => x.TypeID == DefaultCompletionOptions.Revoke);
                        if (revokeOptionIndex >= 0)
                        {
                            options.RemoveAt(revokeOptionIndex);
                        }
                    }

                    // для проекта резолюции вариант "Завершить" должен быть спрятан в "ещё" и располагаться должен над вариантом "Отмена"
                    if (taskTypeID == DefaultTaskTypes.WfResolutionProjectTypeID)
                    {
                        int completeOptionIndex = options.IndexOf(x => x.TypeID == DefaultCompletionOptions.Complete);
                        if (completeOptionIndex >= 0)
                        {
                            CardTypeCompletionOption completeOption = options[completeOptionIndex];
                            completeOption.Flags = completeOption.Flags.SetFlag(CardTypeCompletionOptionFlags.Additional, true);

                            if (options.Count > 1)
                            {
                                options.RemoveAt(completeOptionIndex);

                                int cancelOptionIndex = options.IndexOf(x => x.TypeID == DefaultCompletionOptions.Cancel);
                                if (cancelOptionIndex >= 0)
                                {
                                    options.Insert(cancelOptionIndex, completeOption);
                                }
                                else
                                {
                                    options.Add(completeOption);
                                }
                            }
                        }
                    }

                    // для варианта завершения "Отмена" надо установить форму, которая не выбирается через редактор,
                    // т.к. ещё не существует в нужном типе карточки
                    CardTypeCompletionOption cancelOption = options.FirstOrDefault(x => x.TypeID == DefaultCompletionOptions.Cancel);
                    if (cancelOption != null)
                    {
                        cancelOption.FormName = WfHelper.RevokeOrCancelFormName;
                    }
                }
            }
        }

        #endregion
    }
}
