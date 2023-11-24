using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.Extensions;
using Tessa.Cards.Metadata;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrPermissionsNewGetExtension : CardNewGetExtension
    {
        #region Constructors

        public KrPermissionsNewGetExtension(
            ICardCache cache,
            IKrTokenProvider krTokenProvider,
            IKrPermissionsManager permissionsManager,
            ICardMetadata cardMetadata,
            IKrTypesCache typesCache,
            IKrScope krScope)
        {
            this.cache = cache;
            this.krTokenProvider = krTokenProvider;
            this.permissionsManager = permissionsManager;
            this.cardMetadata = cardMetadata;
            this.typesCache = typesCache;
            this.krScope = krScope;
        }

        #endregion

        #region Fields

        private readonly ICardCache cache;

        private readonly IKrTokenProvider krTokenProvider;

        private readonly IKrPermissionsManager permissionsManager;

        private readonly ICardMetadata cardMetadata;

        private readonly IKrTypesCache typesCache;

        private readonly IKrScope krScope;

        #endregion

        #region Private Methods

        /// <summary>
        /// Устанавливает на секции и строки карточки разрешения в зависимости от полученных разрешений
        /// </summary>
        /// <param name="userID">ИД пользователя</param>
        /// <param name="card">Карточка</param>
        /// <param name="permissionsResult">Результат расчета прав доступа</param>
        private async Task SetCardPermissionsAsync(
            Guid userID,
            Card card,
            IKrPermissionsManagerResult permissionsResult,
            HashSet<Guid, KrPermissionFileSettings> fileSettings,
            CancellationToken cancelationToken = default)
        {
            CardPermissionInfo permissions = card.Permissions;
            var krPermissions = permissionsResult.Permissions;

            if (permissionsResult.WithEdit)
            {
                await SetCardExtendedPermissionsAsync(card, permissionsResult.ExtendedCardSettings, cancelationToken);
            }
            foreach (var task in card.Tasks)
            {
                if (permissionsResult.ExtendedTasksSettings.TryGetValue(task.TypeID, out var extendedTaskSettings))
                {
                    await SetCardExtendedPermissionsAsync(task.Card, extendedTaskSettings, cancelationToken);
                }
            }

            //Права на редактирование карточки
            permissions.SetCardPermissions(
                krPermissions.Contains(KrPermissionFlagDescriptors.EditCard)
                    ? CardPermissionFlags.AllowModify
                    | CardPermissionFlags.AllowDeleteRow
                    | CardPermissionFlags.AllowInsertRow
                    : CardPermissionFlags.ProhibitModify
                    | CardPermissionFlags.ProhibitDeleteRow
                    | CardPermissionFlags.ProhibitInsertRow);

            // Права на подписание файлов
            permissions.SetCardPermissions(
                krPermissions.Contains(KrPermissionFlagDescriptors.SignFiles)
                    ? CardPermissionFlags.AllowSignFile
                    : CardPermissionFlags.ProhibitSignFile);

            //Право на редактирование (и удаление) файлов
            for (int i = card.Files.Count - 1; i >= 0; i--)
            {
                var file = card.Files[i];
                bool isOwnFile = file.Card.CreatedByID == userID;
                if (fileSettings != null && fileSettings.TryGetItem(file.RowID, out var settings))
                {
                    switch (settings.AccessSetting)
                    {
                        case KrPermissionsHelper.FileAccessSettings.FileNotAvailable:
                            card.Files.RemoveAt(i);
                            permissions.FilePermissions.Remove(file.RowID);
                            continue;

                        case KrPermissionsHelper.FileAccessSettings.ContentNotAvailable:
                        case KrPermissionsHelper.FileAccessSettings.OnlyLastVersion:
                            file.VersionsLoaded = true;
                            break;
                    }
                }

                //Пользователь может редактировать файлы, которые он добавил, если они не виртуальные
                if (file.IsVirtual)
                {
                    permissions.SetFilePermissions(file.RowID, CardPermissionFlagValues.ProhibitAllFile, overwrite: true);
                }
                else
                {
                    permissions.SetFilePermissions(
                        file.RowID,
                        // есть разрешение на редактирование файлов
                        (krPermissions.Contains(KrPermissionFlagDescriptors.EditFiles)
                            // или есть разрешение на редактирование своих файлов и файл его
                            || krPermissions.Contains(KrPermissionFlagDescriptors.EditOwnFiles) && isOwnFile
                                ? CardPermissionFlags.AllowModify | CardPermissionFlags.AllowReplaceFile
                                : CardPermissionFlags.ProhibitModify | CardPermissionFlags.ProhibitReplaceFile)
                        |
                        // право на удаление файлов
                        (krPermissions.Contains(KrPermissionFlagDescriptors.DeleteFiles)
                            // или право на удаление собственных файлов и файл его
                            || krPermissions.Contains(KrPermissionFlagDescriptors.DeleteOwnFiles) && isOwnFile
                                ? CardPermissionFlags.AllowDeleteFile
                                : CardPermissionFlags.ProhibitDeleteFile),
                        overwrite: true);
                }
            }

            // Добавление файлов в карточку.
            permissions.SetCardPermissions(
                krPermissions.Contains(KrPermissionFlagDescriptors.AddFiles)
                    ? CardPermissionFlags.AllowInsertFile
                    : CardPermissionFlags.ProhibitInsertFile);

            // Управление номерами в карточке.
            permissions.SetCardPermissions(
                krPermissions.Contains(KrPermissionFlagDescriptors.EditNumber)
                    ? CardPermissionFlags.AllowEditNumber
                    : CardPermissionFlags.ProhibitEditNumber);

            // Редактирование маршрута согласования.

            //Редактирование этапов
            CardSectionPermissionInfo stagesSection =
                permissions.Sections.GetOrAdd(KrConstants.KrStages.Virtual);
            stagesSection.Type = CardSectionType.Table;

            //И согласующих
            CardSectionPermissionInfo approversSection =
                permissions.Sections.GetOrAdd(KrConstants.KrPerformersVirtual.Synthetic);
            approversSection.Type = CardSectionType.Table;

            if (krPermissions.Contains(KrPermissionFlagDescriptors.EditRoute))
            {
                //Если можно редактировать маршрут - позволяем изменять / добавлять / удалять этапы и согласантов
                stagesSection.SetSectionPermissions(
                    CardPermissionFlags.AllowModify
                    | CardPermissionFlags.AllowInsertRow
                    | CardPermissionFlags.AllowDeleteRow,
                    overwrite: true);

                approversSection.SetSectionPermissions(
                    CardPermissionFlags.AllowModify
                    | CardPermissionFlags.AllowInsertRow
                    | CardPermissionFlags.AllowDeleteRow,
                    overwrite: true);
            }
            else
            {
                stagesSection.SetSectionPermissions(
                    CardPermissionFlags.ProhibitModify
                    | CardPermissionFlags.ProhibitInsertRow
                    | CardPermissionFlags.ProhibitDeleteRow,
                    overwrite: true);

                approversSection.SetSectionPermissions(
                    CardPermissionFlags.ProhibitModify
                    | CardPermissionFlags.ProhibitInsertRow
                    | CardPermissionFlags.ProhibitDeleteRow,
                    overwrite: true);
            }

            GuidDictionaryStorage<CardRowPermissionInfo> stagesRows = stagesSection.Rows;
            GuidDictionaryStorage<CardRowPermissionInfo> approverRows = approversSection.Rows;
            if (card.Sections.TryGetValue(KrConstants.KrStages.Virtual, out var stagesDataSection))
            {
                foreach (CardRow stage in stagesDataSection.Rows)
                {
                    bool canEditStage = krPermissions.Contains(KrPermissionFlagDescriptors.EditRoute)
                        && stage.Get<int>("StateID") == (int)KrStageState.Inactive;
                    //Если нет прав или этап активен или завершен - редактирование запрещено
                    stagesRows
                        .GetOrAdd(stage.RowID)
                        .SetRowPermissions(canEditStage
                                ? CardPermissionFlagValues.AllowAllRow
                                : CardPermissionFlagValues.ProhibitAllRow,
                            overwrite: true);

                    foreach (CardRow approver in card.Sections[KrConstants.KrPerformersVirtual.Synthetic].Rows)
                    {
                        if (approver.Fields.Get<Guid>("StageRowID") != stage.RowID)
                        {
                            continue;
                        }

                        approverRows
                            .GetOrAdd(approver.RowID)
                            .SetRowPermissions(canEditStage
                                    ? CardPermissionFlagValues.AllowAllRow
                                    : CardPermissionFlagValues.ProhibitAllRow,
                                overwrite: true);
                    }

                    if (KrStageSerializer.CanBeSkipped(stage))
                    {
                        CardPermissionFlags flags;
                        if (krPermissions.Contains(KrPermissionFlagDescriptors.CanSkipStages))
                        {
                            flags = KrStageSerializer.IsSkip(stage)
                                ? CardPermissionFlags.ProhibitDeleteRow
                                : CardPermissionFlags.AllowDeleteRow;
                        }
                        else
                        {
                            flags = CardPermissionFlags.ProhibitDeleteRow;
                        }

                        stagesRows
                            .GetOrAdd(stage.RowID)
                            .SetRowPermissions(flags);
                    }
                }
            }

            //если сателлит еще не создан - состояние = драфт
            KrState state = card.Sections.ContainsKey(KrConstants.KrApprovalCommonInfo.Virtual)
                && card.Sections[KrConstants.KrApprovalCommonInfo.Virtual].Fields.ContainsKey("StateID")
                && card.Sections[KrConstants.KrApprovalCommonInfo.Virtual].Fields.Get<object>("StateID") != null
                    ? (KrState)card.Sections[KrConstants.KrApprovalCommonInfo.Virtual].Fields.Get<int>("StateID")
                    : KrState.Draft;

            approversSection.SetSectionPermissions(
                krPermissions.Contains(KrPermissionFlagDescriptors.EditRoute)
                && state != KrState.Approved
                    ? CardPermissionFlags.AllowInsertRow
                    : CardPermissionFlags.ProhibitInsertRow);

            // Запуск процесса согласования проверяется непосредственно при попытке запуска процесса
            // Отзыв, возврат, отмена процесса согласования проверяется непосредственно при попытке

            //Даем право на удаление чтобы не скрывался тайл
            permissions.SetCardPermissions(CardPermissionFlags.AllowDeleteCard);
        }

        private async Task SetCardExtendedPermissionsAsync(
            Card card,
            HashSet<Guid, KrPermissionSectionSettings> extendedCardSettings,
            CancellationToken cancellationToken = default)
        {
            var permissions = card.Permissions;
            var cardTypeMeta = await cardMetadata.GetMetadataForTypeAsync(card.TypeID, cancellationToken);
            var cardTypeMetaSections = await cardTypeMeta.GetSectionsAsync(cancellationToken);
            foreach (var sectionSettings in extendedCardSettings)
            {
                SetSectionPermission(card, permissions, cardTypeMetaSections, sectionSettings);
            }
        }

        /// <summary>
        /// Устанавливает права на секцию
        /// </summary>
        private void SetSectionPermission(
            Card card,
            CardPermissionInfo permissions,
            CardMetadataSectionCollection cardMetadataSections,
            KrPermissionSectionSettings sectionSettings)
        {
            if (cardMetadataSections.TryGetValue(sectionSettings.ID, out var sectionMeta)
                && card.Sections.ContainsKey(sectionMeta.Name))
            {
                var sectionPermissions = permissions.Sections.GetOrAdd(sectionMeta.Name);
                sectionPermissions.Type = sectionMeta.SectionType;

                // Запрет редактирования всей секции выше всего, остальное можно игнорировать сразу же
                if (sectionSettings.IsDisallowed)
                {
                    sectionPermissions.SetSectionPermissions(CardPermissionFlags.ProhibitModify);
                }
                else if (sectionSettings.IsAllowed)
                {
                    sectionPermissions.SetSectionPermissions(CardPermissionFlags.AllowModify);
                }

                if (sectionSettings.IsMasked)
                {
                    // Если секция замаскирована целиком, то настройки по полям игнорируются
                    return;
                }

                if (sectionMeta.SectionType == CardSectionType.Table)
                {
                    if (sectionSettings.DisallowRowAdding)
                    {
                        sectionPermissions.SetSectionPermissions(CardPermissionFlags.ProhibitInsertRow);
                    }
                    if (sectionSettings.DisallowRowDeleting)
                    {
                        sectionPermissions.SetSectionPermissions(CardPermissionFlags.ProhibitDeleteRow);
                    }
                }

                foreach (var field in sectionSettings.AllowedFields)
                {
                    SetFieldPermission(sectionMeta, sectionPermissions, field, CardPermissionFlags.AllowModify);
                }
                foreach (var field in sectionSettings.DisallowedFields)
                {
                    SetFieldPermission(sectionMeta, sectionPermissions, field, CardPermissionFlags.ProhibitModify);
                }
            }
        }

        /// <summary>
        /// Устанавливает права на поле секции с учетом комплексных полей
        /// </summary>
        private void SetFieldPermission(
            CardMetadataSection sectionMeta,
            CardSectionPermissionInfo sectionPermissions,
            Guid field,
            CardPermissionFlags permissionFlags)
        {
            if (sectionMeta.Columns.TryGetValue(field, out var columnMeta))
            {
                if (columnMeta.ColumnType == CardMetadataColumnType.Complex)
                {
                    foreach (var refColumnMeta in sectionMeta.Columns
                        .Where(x => x.ColumnType == CardMetadataColumnType.Physical
                            && x.ComplexColumnIndex == columnMeta.ComplexColumnIndex))
                    {
                        sectionPermissions.SetFieldPermissions(refColumnMeta.Name, permissionFlags);
                    }
                }
                else
                {
                    sectionPermissions.SetFieldPermissions(columnMeta.Name, permissionFlags);
                }
            }
        }

        private void StoreOriginalSource(KrToken token, Card card)
        {
            var cardSource = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var section in card.Sections.Values)
            {
                if (permissionsManager.IgnoreSections.Contains(section.Name))
                {
                    continue;
                }

                if (section.Type == CardSectionType.Entry)
                {
                    Dictionary<string, object> sectionSource = null;
                    foreach (var field in section.RawFields)
                    {
                        if (field.Value != null)
                        {
                            if (sectionSource == null)
                            {
                                cardSource[section.Name] = sectionSource = new Dictionary<string, object>(StringComparer.Ordinal);
                            }

                            sectionSource[field.Key] = field.Value;
                        }
                    }
                }
                else
                {
                    List<object> sectionSource = null;
                    foreach (var row in section.Rows)
                    {
                        Dictionary<string, object> rowSource = null;
                        foreach (var field in row)
                        {
                            if (field.Value != null)
                            {
                                if (rowSource == null)
                                {
                                    rowSource = new Dictionary<string, object>(StringComparer.Ordinal);
                                }

                                rowSource[field.Key] = field.Value;
                            }
                        }
                        if (rowSource != null)
                        {
                            if (sectionSource == null)
                            {
                                cardSource[section.Name] = sectionSource = new List<object>();
                            }
                            sectionSource.Add(rowSource);
                        }
                    }
                }
            }

            var fileSoucrces = new List<object>();
            cardSource["Files"] = fileSoucrces;
            foreach (var file in card.Files)
            {
                fileSoucrces.Add(new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["ExternalSource"] = file.ExternalSource?.GetStorage(),
                    ["RowID"] = file.RowID,
                });
            }

            token.Info[KrPermissionsHelper.NewCardSourceKey] = cardSource;
        }

        #endregion

        #region Base Overrides New

        public override async Task BeforeRequest(ICardNewExtensionContext context)
        {
            if (context.CardType == null
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.ValidationResult.IsSuccessful())
            {
                return;
            }

            var permContext = await permissionsManager.TryCreateContextAsync(
                new KrPermissionsCreateContextParams
                {
                    CardTypeID = context.CardType.ID,
                    DocTypeID = context.Method == CardNewMethod.Template
                        && context.Request.TryGetTemplateCard() is Card templateCard
                        ? KrProcessSharedHelper.GetDocTypeID(templateCard)
                        : context.Request.Info.TryGet<Guid?>("docTypeID"), //Если для типа карточки используются типы документов - тип документа д.б. указан
                    WithExtendedPermissions = true,
                    ValidationResult = context.ValidationResult,
                    AdditionalInfo = context.Info,
                    PrevToken = KrToken.TryGet(context.Request.Info),
                    ExtensionContext = context,
                },
                cancellationToken: context.CancellationToken);

            if (permContext != null)
            {
                var result = await permissionsManager.GetEffectivePermissionsAsync(
                    permContext,
                    KrPermissionFlagDescriptors.CreateCard,
                    KrPermissionFlagDescriptors.FullCardPermissionsGroup);

                // Проверяем возможность создания карточки
                // если возможность создания дана, то даем все права на карточку, т.е. ничего не закрываем
                // после сохранения при обновлении карточка уже будет получаться в соответствии с указанными
                // правами
                if (!result.Permissions.Contains(KrPermissionFlagDescriptors.CreateCard)
                    && context.ValidationResult.IsSuccessful())
                {
                    context.ValidationResult.AddError(this, "$KrMessages_HaveNoPermissionsToCreateCard");
                    return;
                }

                context.Info[nameof(KrPermissionsNewGetExtension)] = result;
            }
        }

        public override async Task AfterRequest(ICardNewExtensionContext context)
        {
            if (context.CardType == null
                || context.CardType.InstanceType != CardInstanceType.Card
                || context.CardType.Flags.Has(CardTypeFlags.Singleton)
                || !context.RequestIsSuccessful
                || !context.Response.ValidationResult.IsSuccessful())
            {
                return;
            }

            var card = context.Response.Card;
            if (context.Info.TryGetValue(nameof(KrPermissionsNewGetExtension), out var obj)
                && obj is IKrPermissionsManagerResult result)
            {
                CalcPermissionCanFullRecalcRoute(
                    card,
                    context.DbScope,
                    result);

                var extendedCardSettings = result.CreateExtendedCardSettings(context.Session.User.ID, card);
                var fileSettings = GetFileSettings(extendedCardSettings);

                await SetCardPermissionsAsync(
                    context.Session.User.ID,
                    card,
                    result,
                    fileSettings,
                    context.CancellationToken);

                // Создаем токен
                var token = this.krTokenProvider.CreateToken(
                    card,
                    result.Version,
                    result.Permissions,
                    extendedCardSettings,
                    (t) => StoreOriginalSource(t, card));

                //кладем токен в карточку
                token.Set(card.Info);
            }
        }

        private HashSet<Guid, KrPermissionFileSettings> GetFileSettings(IKrPermissionExtendedCardSettings extendedCardSettings)
        {
            var fileSettings = extendedCardSettings.GetFileSettings();
            if (fileSettings == null
                || fileSettings.Count == 0)
            {
                return null;
            }
            else
            {
                return new HashSet<Guid, KrPermissionFileSettings>(x => x.FileID, fileSettings);
            }
        }

        /// <summary>
        /// Рассчитывает, для указанной карточки, возможность полного пересчёта прочецца.
        /// </summary>
        /// <param name="card">Карточка для которой выполняется расчёт <see cref="KrPermissionFlagDescriptors.CanFullRecalcRoute"/>.</param>
        /// <param name="dbScope">Объект для взаимодействия с БД.</param>
        /// <param name="permissionsResult">Результат выполнения проверки прав доступа в <see cref="IKrPermissionsManager"/>.</param>
        private void CalcPermissionCanFullRecalcRoute(
            Card card,
            IDbScope dbScope,
            IKrPermissionsManagerResult permissionsResult)
        {
            if (permissionsResult.Has(KrPermissionFlagDescriptors.CanFullRecalcRoute))
            {
                if (!KrProcessHelper.CardSupportsRoutes(card, dbScope, this.typesCache)
                    || !this.krScope.GetKrSatellite(card.ID)
                            .GetStagesSection()
                            .Rows
                            .All(p => (p.TryGet<int?>(KrConstants.StateID) ?? KrStageState.Inactive.ID) == KrStageState.Inactive))
                {
                    permissionsResult.Permissions.Remove(KrPermissionFlagDescriptors.CanFullRecalcRoute);
                }
            }
        }

        #endregion

        #region Base Overrides Get

        public override async Task AfterRequest(ICardGetExtensionContext context)
        {
            Card card;

            if (!context.RequestIsSuccessful
                || !context.ValidationResult.IsSuccessful()
                || context.CardType == null
                || !KrComponentsHelper.HasBase(context.Response.Card.TypeID, this.cache)
                || (card = context.Response.TryGetCard()) == null)
            {
                return;
            }

            if (context.Method == CardGetMethod.Default)
            {
                bool needMessage = false;
                var requriedPermissions = new List<KrPermissionFlagDescriptor>();

                var calculateFullPermissions = context.Request.Info.TryGet<bool>(KrPermissionsHelper.CalculatePermissionsMark);

                if (calculateFullPermissions)
                {
                    //Нужно рассчитать как бы все права, но на самом деле многие права (удаление/отмена и т.д.)
                    //нужны непосредственно при попытке совершения действия - поэтому будем проверять не все права
                    requriedPermissions.Add(KrPermissionFlagDescriptors.FullCardPermissionsGroup);

                    if (!context.Request.Info.TryGet<bool>(KrPermissionsHelper.PermissionsCalculatedMark))
                    {
                        //Если пришел признак что права уже были были рассчитаны по плитке "редактировать"
                        //- не будем отображать сообщение
                        needMessage = true;
                    }
                }
                else
                {
                    requriedPermissions.Add(KrPermissionFlagDescriptors.ReadCard);
                    if (context.Request.Info.TryGet<bool>(KrPermissionsHelper.CalculateSuperModeratorPermissions))
                    {
                        requriedPermissions.Add(KrPermissionFlagDescriptors.SuperModeratorMode);
                    }

                    if (context.Request.Info.TryGet<bool>(KrPermissionsHelper.CalculateAddTopicPermissions))
                    {
                        requriedPermissions.Add(KrPermissionFlagDescriptors.AddTopics);
                    }

                    if (context.Request.Info.TryGet<bool>(KrPermissionsHelper.CalculateResolutionPermissionsMark))
                    {
                        //Нужно рассчитать как бы все права, но на самом деле многие права (удаление/отмена и т.д.)
                        //нужны непосредственно при попытке совершения действия - поэтому будем проверять не все права
                        requriedPermissions.Add(KrPermissionFlagDescriptors.CreateResolutions);
                    }
                }

                var permContext = await permissionsManager.TryCreateContextAsync(
                    new KrPermissionsCreateContextParams
                    {
                        Card = context.Response.Card,
                        // Рассчет обязательных прав и расширенных настроек на сервере не имеет смысла
                        WithRequiredPermissions = context.Request.ServiceType != CardServiceType.Default,
                        WithExtendedPermissions = context.Request.ServiceType != CardServiceType.Default,
                        ValidationResult = context.ValidationResult,
                        AdditionalInfo = context.Info,
                        PrevToken = KrToken.TryGet(context.Request.Info),
                        ExtensionContext = context,
                    },
                    cancellationToken: context.CancellationToken);

                IKrPermissionsManagerResult result;
                await using (context.DbScope.Create())
                {
                    result = await permissionsManager.GetEffectivePermissionsAsync(
                        permContext,
                        requriedPermissions.ToArray());

                    context.Info[nameof(KrPermissionsNewGetExtension)] = result;
                }

                //Если был запрос на полный рассчет прав - отметим это в инфо карточки, чтобы не отображать
                //плитку "редактировать"
                if (calculateFullPermissions
                    //Или если все права были получены
                    || result.Has(KrPermissionFlagDescriptors.FullCardPermissionsGroup))
                {
                    card.Info[KrPermissionsHelper.PermissionsCalculatedMark] = true;
                }

                if (context.Request.Info.TryGet<bool>(KrPermissionsHelper.CalculateSuperModeratorPermissions)
                    || result.Has(KrPermissionFlagDescriptors.SuperModeratorMode))
                {
                    card.Info.Add(KrPermissionsHelper.SuperModeratorPermissionsCalculated, true);
                }

                if (context.Request.Info.TryGet<bool>(KrPermissionsHelper.CalculateAddTopicPermissions)
                    || result.Has(KrPermissionFlagDescriptors.SuperModeratorMode)) // TODO тут по логике жолжно быть AddTopic
                {
                    card.Info.Add(KrPermissionsHelper.AddTopicPermissionsCalculated, true);
                }

                //Гарантируем что будет работать только наша логика
                //CardHelper.ProhibitAllPermissions(card, removeOtherPermissions: true, excludeTasks: true);

                //Право на создание проверяется в соотв. расширении

                //Права на чтение карточки
                // Не пишем ошибку об отсутствии доступа на чтение, если есть другие ошибки
                if (!result.Permissions.Contains(KrPermissionFlagDescriptors.ReadCard)
                    && context.ValidationResult.IsSuccessful())
                {
                    context.ValidationResult.AddError(this, "$KrMessages_HaveNoPermissionsToReadCard");
                    return;
                }

                CalcPermissionCanFullRecalcRoute(
                    card,
                    context.DbScope,
                    result);

                var extendedCardSettings = result.CreateExtendedCardSettings(context.Session.User.ID, card);
                var fileSettings = GetFileSettings(extendedCardSettings);

                await SetCardPermissionsAsync(
                    context.Session.User.ID,
                    card,
                    result,
                    fileSettings,
                    context.CancellationToken);

                //Если был запрос на полный рассчет прав (по плитке "Редактировать") и не было дано
                //право на редактирование карточки, то, чтобы пользователь понял что рассчет был -
                //отобразим информационное сообщение какие права были получены
                if (needMessage && !result.Permissions.Contains(KrPermissionFlagDescriptors.EditCard))
                {
                    string message = KrPermissionsHelper.GetGrantedPermissionsMessage(result.Permissions.ToArray());
                    context.ValidationResult.AddInfo(this, message);
                }

                // Создаем токен
                var token = this.krTokenProvider.CreateToken(card, result.Version, result.Permissions, extendedCardSettings);

                //кладем токен в карточку
                token.Set(card.Info);
            }
            // Проверка экспорта не затирание данных при нем не выполняется для админов
            else if (context.Method == CardGetMethod.Export
                && !context.Session.User.IsAdministrator())
            {
                var permContext = await permissionsManager.TryCreateContextAsync(
                    new KrPermissionsCreateContextParams
                    {
                        Card = context.Response.Card,
                        // Состояние грузится напрямую через базу, т.к. в карточке при экспорте его нет
                        KrState = await KrProcessSharedHelper.GetKrStateAsync(context.Response.Card.ID, context.DbScope, context.CancellationToken),
                        WithExtendedPermissions = true,
                        ValidationResult = context.ValidationResult,
                        AdditionalInfo = context.Info,
                        ExtensionContext = context,
                    },
                    cancellationToken: context.CancellationToken);

                IKrPermissionsManagerResult result;
                await using (context.DbScope.Create())
                {
                    result = await permissionsManager.GetEffectivePermissionsAsync(
                        permContext,
                        KrPermissionFlagDescriptors.ReadCard, KrPermissionFlagDescriptors.CreateTemplateAndCopy);
                    context.Info[nameof(KrPermissionsNewGetExtension)] = result;
                }

                // Если при расчете прав нет права на создание шаблона и копирование, то пишем ошибку
                if (result.Has(KrPermissionFlagDescriptors.CreateTemplateAndCopy))
                {
                    var extendedCardSettings = result.CreateExtendedCardSettings(context.Session.User.ID, card);
                    var fileSettings = GetFileSettings(extendedCardSettings);

                    if (fileSettings != null
                        && fileSettings.Count > 0)
                    {
                        var cardFiles = card.Files;
                        for (int i = cardFiles.Count - 1; i >= 0; i--)
                        {
                            var file = cardFiles[i];
                            if (fileSettings.TryGetItem(file.RowID, out var settings)
                                && settings.AccessSetting <= KrPermissionsHelper.FileAccessSettings.ContentNotAvailable)
                            {
                                card.Files.RemoveAt(i);
                            }
                        }
                    }
                }
                else if (context.ValidationResult.IsSuccessful()) // Не пишем ошибку об отсутсвии доступа, если есть другие ошибки
                {
                    context.ValidationResult.AddError(
                        this,
                        KrPermissionsHelper.GetNotEnoughPermissionsErrorMessage(
                            KrPermissionFlagDescriptors.CreateTemplateAndCopy));
                }
            }
        }

        #endregion
    }
}
