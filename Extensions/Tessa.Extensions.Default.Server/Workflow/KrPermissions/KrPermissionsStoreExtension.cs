﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrPermissionsStoreExtension : CardStoreExtension
    {
        #region Fields

        private readonly IKrTypesCache cache;
        private readonly IKrTokenProvider krTokenProvider;
        private readonly IKrPermissionsManager permissionsManager;
        private readonly IKrScope krScope;

        #endregion

        #region Constructors

        public KrPermissionsStoreExtension(
            IKrTypesCache cache,
            IKrTokenProvider krTokenProvider,
            IKrPermissionsManager permissionsManager,
            IKrScope krScope)
        {
            this.cache = cache;
            this.krTokenProvider = krTokenProvider;
            this.permissionsManager = permissionsManager;
            this.krScope = krScope;
        }

        #endregion

        #region Private Methods

        private static bool FileHasChangesExceptSignatures(CardFile file)
        {
            // подписи могут изменяться без прав на редактирование файлов
            CardFileState state = file.State;
            if (state == CardFileState.Replaced || state == CardFileState.ModifiedAndReplaced)
            {
                return true;
            }

            if (state != CardFileState.Modified)
            {
                return false;
            }

            // изменялись системные свойства
            if (file.Flags != CardFileFlags.None)
            {
                return true;
            }

            Card fileCard = file.TryGetCard();
            StringDictionaryStorage<CardSection> fileSections;
            if (fileCard != null
                && (fileSections = fileCard.TryGetSections()) != null)
            {
                // нет изменённых секций
                int count = fileSections.Count;
                if (count == 0)
                {
                    return false;
                }

                // изменялись другие секции, кроме как секции с подписями
                return count > 1 || fileSections.First().Key != CardSignatureHelper.SectionName;
            }

            // нет изменённых секций
            return false;
        }


        private KrPermissionFlagDescriptor[] GetRequiredPermissions(Guid userID, Card card, IValidationResultBuilder validationResults)
        {
            //Сначала посчитаем какие проверки нужны
            var required = new List<KrPermissionFlagDescriptor>();

            if (card.Sections.Any(x => x.Key != KrConstants.KrStages.Virtual
                                       && x.Key != KrConstants.KrActiveTasks.Virtual
                                       && x.Key != KrConstants.KrPerformersVirtual.Synthetic
                                       && (x.Key != KrConstants.KrApprovalCommonInfo.Virtual || x.Value.GetAllChanges().Any(p => p != "NeedRebuild"))
                                       && (x.Value.TryGetRawFields()?.Count > 0 || x.Value.TryGetRows()?.Count > 0))
                || card.Tasks.Any(x =>
                        x.State != CardRowState.None
                        && x.TryGetCard() is Card taskCard
                        && taskCard.Sections.Any(y =>
                            y.Value.TryGetRawFields()?.Count > 0 || y.Value.TryGetRows()?.Count > 0)))
            {
                required.Add(KrPermissionFlagDescriptors.EditCard);
            }

            // изменение собственных файлов - право на это изменение определяется отдельно согласно
            // заданиям карточки, поэтому пользователь может его получить, не обладая правом на 
            // редактирование файлов в целом
            if (card.Files.Any(x => x.Card.CreatedByID == userID && FileHasChangesExceptSignatures(x)))
            {
                required.Add(KrPermissionFlagDescriptors.EditOwnFiles);
            }

            // изменение файлов
            if (card.Files.Any(x => x.Card.CreatedByID != userID && FileHasChangesExceptSignatures(x)))
            {
                required.Add(KrPermissionFlagDescriptors.EditFiles);
            }

            // удаление собственных файлов
            if (card.Files.Any(x => x.State == CardFileState.Deleted && x.Card.CreatedByID == userID))
            {
                required.Add(KrPermissionFlagDescriptors.DeleteOwnFiles);
            }

            // удаление файлов
            if (card.Files.Any(x => x.State == CardFileState.Deleted && x.Card.CreatedByID != userID))
            {
                required.Add(KrPermissionFlagDescriptors.DeleteFiles);
            }

            // добавление файлов
            if (card.Files.Any(x => x.State == CardFileState.Inserted))
            {
                required.Add(KrPermissionFlagDescriptors.AddFiles);
            }

            if (card.StoreMode == CardStoreMode.Update)
            {
                var containsKrStagesVirtual = card.TryGetStagesSection(out var krStagesVirtual);
                bool hasSkipStages = default;
                bool hasModifiedStages = default;
                if (containsKrStagesVirtual)
                {
                    Card satellite = default;
                    foreach (var stage in krStagesVirtual.Rows.Where(i => i.State == CardRowState.Deleted))
                    {
                        if (satellite is null)
                        {
                            satellite = this.krScope.GetKrSatellite(card.ID, validationResults);

                            if (satellite is null)
                            {
                                break;
                            }
                        }

                        CardRow satelliteKrStage;
                        if (satellite.TryGetStagesSection(out var satelliteKrStages)
                            && ((satelliteKrStage = satelliteKrStages.Rows.SingleOrDefault(j => j.RowID == stage.RowID)) != null)
                            && KrStageSerializer.CanBeSkipped(satelliteKrStage))
                        {
                            hasSkipStages = true;
                            break;
                        }
                    }

                    hasModifiedStages = krStagesVirtual.Rows.Any(i => i.State == CardRowState.Modified || i.State == CardRowState.Inserted);
                }

                // Пропуск этапа.
                if (hasSkipStages)
                {
                    required.Add(KrPermissionFlagDescriptors.CanSkipStages);
                }

                // Изменение маршрута.
                if (hasModifiedStages
                    || card.Sections.ContainsKey(KrConstants.KrPerformersVirtual.Synthetic))
                {
                    required.Add(KrPermissionFlagDescriptors.EditRoute);
                }
            }

            // Подписание файлов
            if (CardSignatureHelper.AnySignatureRow(card,
                (file, signatureRow) => signatureRow.State != CardRowState.Deleted))
            {
                required.Add(KrPermissionFlagDescriptors.SignFiles);
            }

            return required.ToArray();
        }

        #endregion

        #region Base Overrides

        public override async Task AfterBeginTransaction(ICardStoreExtensionContext context)
        {
            // права на измененную карточку считаем в AfterBeginTransaction, чтобы считать доступ к карточке по данным,
            // по которым эти права были рассчитаны изначально

            Card card;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null
                || card.StoreMode != CardStoreMode.Update
                || KrComponentsHelper.GetKrComponents(card, this.cache)
                    .HasNot(KrComponents.Base))
            {
                return;
            }

            await CheckPermissionsOnUpdatingAsync(context, card);
        }

        public override async Task BeforeCommitTransaction(ICardStoreExtensionContext context)
        {
            // считаем права на возможность изменить только что созданную карточку внутри блокировки на запись карточки;
            // BeforeCommit нужен, чтобы карточка уже была в базе для выполнения контекстных ролей

            Card card;
            if (context.CardType == null
                || (card = context.Request.TryGetCard()) == null
                || card.StoreMode != CardStoreMode.Insert
                || KrComponentsHelper.GetKrComponents(card, this.cache)
                    .HasNot(KrComponents.Base))
            {
                return;
            }

            await CheckPermissionsOnCreatingAsync(context, card);
        }

        public override Task AfterRequest(ICardStoreExtensionContext context)
        {
            Card card;
            if (!context.RequestIsSuccessful)
            {
                PrepareMandatoryFailedData(context);
                return Task.CompletedTask;
            }
            else if ((card = context.Request.TryGetCard()) == null
                    || KrComponentsHelper.GetKrComponents(card, this.cache)
                        .HasNot(KrComponents.Base))
            {
                return Task.CompletedTask;
            }


            //Текущую версию создал текущий пользователь - кинем в респонс одноразовый токен с правами на чтение карточки
            KrToken token = this.krTokenProvider
                .CreateToken(
                    context.Response.CardID,
                    //context.Response.CardVersion, // TODO permissions - прокидывать корректню версию карточки после доработки отслеживания версий
                    permissions: new KrPermissionFlagDescriptor[]{ KrPermissionFlagDescriptors.ReadCard },
                    modifyTokenAction: (t) => t.ExpiryDate = DateTime.UtcNow.AddMinutes(1));

            token.Set(context.Response.Info);
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async Task CheckPermissionsOnCreatingAsync(ICardStoreExtensionContext context, Card storeCard)
        {
            KrToken krToken = KrToken.TryGet(storeCard.Info);
            KrToken serverToken = context.Info.TryGetServerToken();

            var permContext = await permissionsManager.TryCreateContextAsync(
                new KrPermissionsCreateContextParams
                {
                    Card = storeCard,
                    IsStore = true,
                    WithExtendedPermissions = true,
                    ValidationResult = context.ValidationResult,
                    AdditionalInfo = context.Info,
                    PrevToken = krToken,
                    ServerToken = serverToken,
                    ExtensionContext = context,
                },
                cancellationToken: context.CancellationToken);

            if (permContext != null)
            {
                context.Info[nameof(KrPermissionsStoreExtension)] = await permissionsManager.CheckRequiredPermissionsAsync(
                    permContext,
                    this.GetRequiredPermissions(context.Session.User.ID, storeCard, context.ValidationResult)).ConfigureAwait(false);
            }
        }

        private async Task CheckPermissionsOnUpdatingAsync(ICardStoreExtensionContext context, Card storeCard)
        {
            KrToken krToken = KrToken.TryGet(storeCard.Info);
            KrToken serverToken = context.Info.TryGetServerToken();
            if (krToken == null)
            {
                //Если запрос отправлен не из плагина Chronos или из неизвестного плагина (как правило это обычный запрос из клиента)
                if (!context.Request.GetIgnorePermissionsWarning()
                    && !context.Request.TryGetPluginType().HasValue)
                {
                    //Предупредим пользователя что что-то пошло не так и токен не был найден
                    context.ValidationResult.AddWarning(this, "$KrMessages_CardHasNoTokenWhenSaving");
                }
            }

            var permContext = await permissionsManager.TryCreateContextAsync(
                new KrPermissionsCreateContextParams
                {
                    Card = storeCard,
                    IsStore = true,
                    WithExtendedPermissions = true,
                    ValidationResult = context.ValidationResult,
                    AdditionalInfo = context.Info,
                    PrevToken = krToken,
                    ServerToken = serverToken,
                    ExtensionContext = context,
                },
                cancellationToken: context.CancellationToken);

            if (permContext != null)
            {
                context.Info[nameof(KrPermissionsStoreExtension)] = await permissionsManager.CheckRequiredPermissionsAsync(
                    permContext,
                    this.GetRequiredPermissions(context.Session.User.ID, storeCard, context.ValidationResult)).ConfigureAwait(false);
            }
        }

        private void PrepareMandatoryFailedData(ICardStoreExtensionContext context)
        {
            if (context.Info.TryGetValue(nameof(KrPermissionsStoreExtension), out var resultObject)
                && resultObject is KrPermissionsManagerCheckResult result
                && result.Info.TryGetValue(KrPermissionsHelper.FailedMandatoryRulesKey, out var rulesObj)
                && rulesObj is List<object> rules)
            {
                List<KrPermissionMandatoryRuleStorage> rulesForSend = new List<KrPermissionMandatoryRuleStorage>();
                foreach(KrPermissionMandatoryRule rule in rules)
                {
                    rulesForSend.Add(
                        new KrPermissionMandatoryRuleStorage(
                            rule.SectionID,
                            rule.HasColumns ? rule.ColumnIDs : null));
                }

                context.Response.Info[KrPermissionsHelper.FailedMandatoryRulesKey]
                    = rulesForSend.Select(x => (object)x.GetStorage());
            }
        }

        #endregion
    }
}
