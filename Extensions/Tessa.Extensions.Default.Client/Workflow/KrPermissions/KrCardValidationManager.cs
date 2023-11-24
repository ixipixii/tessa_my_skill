﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Metadata;
using Tessa.Cards.Validation;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Client.Workflow.KrPermissions
{
    public sealed class KrCardValidationManager : ICardValidationManager
    {
        #region Fields

        private readonly ICardValidationManager platformValidationManager;

        #endregion

        #region Constructors

        public KrCardValidationManager(
            CardValidationManager platformValidationManager)
        {
            this.platformValidationManager = platformValidationManager;
        }

        #endregion

        #region ICardValidationManager Implementation

        public Task<ICardValidationResult> ValidateCardAsync(
            IEnumerable<CardTypeValidator> validators,
            Guid mainCardTypeID,
            Card mainCard,
            CardStoreMode storeMode,
            ISerializableObject externalContextInfo = null,
            Func<ICardValidationContext, Task> modifyContextActionAsync = null,
            CardValidationMode validationMode = CardValidationMode.Card,
            CancellationToken cancellationToken = default)
        {
            return platformValidationManager.ValidateCardAsync(
                validators,
                mainCardTypeID,
                mainCard,
                storeMode,
                externalContextInfo,
                GetModifyContextAction(modifyContextActionAsync),
                validationMode,
                cancellationToken);
        }

        public Task<ICardValidationResult> ValidateTaskAsync(
            IEnumerable<CardTypeValidator> validators,
            Guid mainCardTypeID,
            Card mainCard,
            CardStoreMode storeMode,
            Guid taskCardTypeID,
            Card taskCard,
            ISerializableObject externalContextInfo = null,
            Func<ICardValidationContext, Task> modifyContextActionAsync = null,
            CardValidationMode validationMode = CardValidationMode.Task,
            CancellationToken cancellationToken = default)
        {
            return platformValidationManager.ValidateTaskAsync(
                validators,
                mainCardTypeID,
                mainCard,
                storeMode,
                taskCardTypeID,
                taskCard,
                externalContextInfo,
                modifyContextActionAsync,
                validationMode,
                cancellationToken);
        }

        #endregion

        #region Private Methods

        private Func<ICardValidationContext, Task> GetModifyContextAction(
            Func<ICardValidationContext, Task> modifyContextActionAsync)
        {
            if (modifyContextActionAsync == null)
            {
                return ModifyContextAsync;
            }
            else
            {
                return async (c) =>
                {
                    await modifyContextActionAsync(c);
                    await ModifyContextAsync(c);
                };
            }
        }

        private async Task ModifyContextAsync(ICardValidationContext context)
        {
            var mainCard = context.MainCard;
            var token = KrToken.TryGet(mainCard.Info);

            if (token != null
                && token.ExtendedCardSettings != null)
            {
                var cardSettings = token.ExtendedCardSettings.GetCardSettings();
                var cardSections = await context.CardMetadata.GetSectionsAsync(context.CancellationToken);

                foreach (var sectionSettings in cardSettings)
                {
                    if (cardSections.TryGetValue(sectionSettings.ID, out var sectionMeta))
                    {
                        if (sectionSettings.IsMasked)
                        {
                            context.Limitations.ExcludeSections(sectionMeta.Name);
                        }
                        else if (sectionSettings.MaskedFields.Count > 0)
                        {
                            foreach(var field in sectionSettings.MaskedFields)
                            {
                                if (sectionMeta.Columns.TryGetValue(field, out var columnMeta))
                                {
                                    if (columnMeta.ColumnType == CardMetadataColumnType.Complex)
                                    {
                                        context.Limitations.ExcludeColumns(
                                            sectionMeta.Name, 
                                            sectionMeta
                                                .Columns
                                                .Where(x => x.ColumnType == CardMetadataColumnType.Physical && x.ComplexColumnIndex == columnMeta.ComplexColumnIndex)
                                                .Select(x => x.Name)
                                                .ToArray());
                                    }
                                    else
                                    {
                                        context.Limitations.ExcludeColumns(sectionMeta.Name, columnMeta.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
