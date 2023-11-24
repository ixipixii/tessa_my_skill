using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Metadata;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.Platform.Conditions;
using Tessa.Platform.Data;
using Tessa.Platform.IO;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Roles;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <inheritdoc />
    public sealed class KrPermissionsManager : IKrPermissionsManager
    {
        #region Nested Types

        private delegate void CalcAction(IKrPermissionsManagerContext context, IKrPermissionRuleSettings rule);
        private delegate bool FilterFunc(IKrPermissionRuleSettings rule);

        private class InnerContext : IAsyncDisposable
        {
            #region Fields

            private Dictionary<string, object> resultInfo;

            #endregion

            #region Constructors

            public InnerContext(IKrPermissionsManagerContext context, bool checkPermissions)
            {
                this.Context = context;
                this.CheckPermissions = checkPermissions;
            }

            #endregion

            #region Properties

            public IKrPermissionsManagerContext Context { get; }

            /// <summary>
            /// Флаг определяет, что происходит проверка прав.
            /// Если флег имеет значение false, значеит идет расчет прав.
            /// </summary>
            public bool CheckPermissions { get; }

            /// <summary>
            /// Флаг определяет, что расширенные настройки уже были рассчитаны
            /// </summary>
            public bool ExtendedSettingsCalculated { get; set; }

            public HashSet<Guid> SubmittedRules { get; } = new HashSet<Guid>();
            public HashSet<Guid> RejectedRules { get; } = new HashSet<Guid>();
            public HashSet<Guid> SubmittedRoles { get; } = new HashSet<Guid>();
            public HashSet<Guid> RejectedRoles { get; } = new HashSet<Guid>();

            public IKrPermissionsCache PermissionsCache { get; set; }
            public IConditionContext ConditionContext { get; set; }
            public IConditionCompilationResult ConditionCompilationResult { get; set; }
            public IExtensionExecutor<IKrPermissionsRuleExtension> RuleExecutor { get; set; }

            public Dictionary<string, object> ResultInfo => this.resultInfo ??= new Dictionary<string, object>(StringComparer.Ordinal);

            public bool HasResultInfo => resultInfo != null;

            #endregion

            #region IAsyncDisposable Implementation

            public async ValueTask DisposeAsync()
            {
                if (RuleExecutor != null)
                {
                    await RuleExecutor.DisposeAsync();
                    RuleExecutor = null;
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly IExtensionContainer extensionContainer;
        private readonly ISession session;
        private readonly IDbScope dbScope;
        private readonly ICardRepository cardRepository;
        private readonly ICardServerPermissionsProvider permissionsProvider;
        private readonly IRoleRepository roleRepository;
        private readonly ICardContextRoleCache roleCache;
        private readonly ICardGetStrategy cardGetStrategy;
        private readonly ICardMetadata cardMetadata;
        private readonly IKrTokenProvider krTokenProvider;
        private readonly IKrTypesCache krTypesCache;
        private readonly IConditionExecutor conditionExecutor;
        private readonly IConditionCompilationCache conditionCompilationCache;
        private readonly IKrPermissionsCacheContainer permissionsCacheContainer;
        private readonly IUnityContainer unityContainer;

        private readonly CalcAction[] sectionsAndMandatoryActions = new CalcAction[]
        {
            CalcExtendedPermissionsFromRule,
            CalcExtendedMandatoryFromRule,
        };

        private readonly CalcAction[] mandatoryCalcAction = new CalcAction[]
        {
            CalcExtendedMandatoryFromRule,
        };

        private readonly FilterFunc[] mandatoryFilterFuncs = new FilterFunc[]
        {
            new FilterFunc(x => x.MandatoryRules.Count > 0),
        };

        private readonly CalcAction[] fileRulesCalcAction = new CalcAction[]
        {
            CalcExtendedFileRuleFromRule,
        };

        private readonly FilterFunc[] fileRulesFilterFunc = new FilterFunc[]
        {
            new FilterFunc(x => x.FileRules.Count > 0),
        };

        private readonly CalcAction[] allCalcActions = new CalcAction[]
        {
            CalcExtendedPermissionsFromRule,
            CalcExtendedMandatoryFromRule,
            CalcVisibilityFromRule,
            CalcExtendedFileRuleFromRule,
        };


        #endregion

        #region Constructors

        public KrPermissionsManager(
            IExtensionContainer extensionContainer,
            ISession session,
            IDbScope dbScope,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)]ICardRepository cardRepository,
            ICardServerPermissionsProvider permissionsProvider,
            IRoleRepository roleRepository,
            ICardContextRoleCache roleCache,
            ICardGetStrategy cardGetStrategy,
            ICardMetadata cardMetadata,
            IKrTokenProvider krTokenProvider,
            IKrTypesCache krTypesCache,
            IConditionExecutor conditionExecutor,
            IConditionCompilationCache conditionCompilationCache,
            IKrPermissionsCacheContainer permissionsCacheContainer,
            IUnityContainer unityContainer)
        {
            this.extensionContainer = extensionContainer;
            this.session = session;
            this.dbScope = dbScope;
            this.cardRepository = cardRepository;
            this.permissionsProvider = permissionsProvider;
            this.roleRepository = roleRepository;
            this.roleCache = roleCache;
            this.cardGetStrategy = cardGetStrategy;
            this.cardMetadata = cardMetadata;
            this.krTokenProvider = krTokenProvider;
            this.krTypesCache = krTypesCache;
            this.conditionExecutor = conditionExecutor;
            this.conditionCompilationCache = conditionCompilationCache;
            this.permissionsCacheContainer = permissionsCacheContainer;
            this.unityContainer = unityContainer;
        }

        #endregion

        #region IKrPermissionsManager Implementation

        /// <inheritdoc />
        public ICollection<string> IgnoreSections { get; } = new HashSet<string>
        {
            KrConstants.KrStages.Virtual,
            KrConstants.KrActiveTasks.Virtual,
            KrConstants.KrPerformersVirtual.Synthetic,
            KrConstants.KrApprovalCommonInfo.Virtual,
            "FmTopicParticipantsVirtual",
            "FmTopicParticipantsInfoVirtual",
            "FmTopicRoleParticipantsInfoVirtual",
            "FmAddTopicInfoVirtual",
        };

        /// <inheritdoc />
        public async Task<IKrPermissionsManagerContext> TryCreateContextAsync(
            KrPermissionsCreateContextParams param,
            CancellationToken cancellationToken = default)
        {
            KrPermissionsCheckMode mode = KrPermissionsCheckMode.WithoutCard;

            if (param.Card != null)
            {
                mode = param.IsStore ? KrPermissionsCheckMode.WithStoreCard : KrPermissionsCheckMode.WithCard;
                param.CardTypeID ??= param.Card.TypeID;

                if (param.Card.StoreMode == CardStoreMode.Update)
                {
                    param.KrState ??= await KrProcessSharedHelper.GetKrStateAsync(param.Card, this.dbScope, cancellationToken) ?? KrState.Draft;
                }
            }
            else if (param.CardID.HasValue)
            {
                mode = KrPermissionsCheckMode.WithCardID;
                param.CardTypeID ??= await this.cardRepository.GetTypeIDAsync(param.CardID.Value, CardInstanceType.Card, cancellationToken);

                param.KrState ??= await KrProcessSharedHelper.GetKrStateAsync(param.CardID.Value, this.dbScope, cancellationToken) ?? KrState.Draft;
            }

            if (param.CardTypeID.HasValue
                && (await cardMetadata.GetCardTypesAsync(cancellationToken)).TryGetValue(param.CardTypeID.Value, out var cardType))
            {
                var components = KrComponentsHelper.GetKrComponents(param.CardTypeID.Value, krTypesCache);

                if (components.HasNot(KrComponents.Base))
                {
                    // Для данного типа карточек проверка не предусмотрена
                    return null;
                }

                if (components.Has(KrComponents.DocTypes))
                {
                    if (!param.DocTypeID.HasValue)
                    {
                        if (param.Card != null)
                        {
                            param.DocTypeID ??= KrProcessSharedHelper.GetDocTypeID(param.Card, this.dbScope); // TODO async - метод может быть асинхронным
                        }
                        else if (param.CardID.HasValue)
                        {
                            param.DocTypeID ??= KrProcessSharedHelper.GetDocTypeID(param.CardID.Value, this.dbScope); // TODO async - метод может быть асинхронным
                        }
                    }
                    if (!param.DocTypeID.HasValue)
                    {
                        param.ValidationResult?.AddError(this, "$KrMessages_DocTypeNotSpecified");
                        return null;
                    }
                }
                else
                {
                    param.DocTypeID = null;
                }
            }
            else
            {
                return null; // Тип карточки не известен или не задан, тогда проверку прав доступа не выполняем
            }

            return new KrPermissionsManagerContext(
                dbScope,
                session,
                param.Card,
                param.CardID,
                cardType,
                param.DocTypeID,
                param.KrState,
                param.FileID,
                param.FileVersionID,
                param.WithRequiredPermissions,
                param.WithExtendedPermissions,
                param.IgnoreSections,
                mode,
                param.ValidationResult,
                param.AdditionalInfo,
                param.PrevToken,
                param.ServerToken,
                param.ExtensionContext,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IKrPermissionsManagerResult> GetEffectivePermissionsAsync(
            IKrPermissionsManagerContext context,
            params KrPermissionFlagDescriptor[] requiredPermissions)
        {
            if (requiredPermissions == null
                || requiredPermissions.Length == 0)
            {
                return KrPermissionsManagerResult.Empty;
            }

            context.Descriptor = new KrPermissionsDescriptor(requiredPermissions);
            context.Method = nameof(GetEffectivePermissionsAsync);

            // Посследовательность расчета прав:
            // 1) Если запрашиваем обязательыне права доступа, сперва считаем их. Если все права рассчитаны, завершаемся
            // 2) Выполняем расширения по заданиям. Они могут как выдать права, так и запросить новые права.
            // 3) Выполняем расширения по карточке. Они могут как выдать права, так и запросить новые права.
            // 4) Проверяем расширения по токену. Проверка идет после расширений, чтобы была возможность перенести из токена запрашиваемые в расширениях права.
            //    если все права рассчитаны, завершаемся.
            // 5) Выполняем расчет прав по правилам доступа.
            // 6) Завершаемся
            // Важно! Если запрашивается расширенный расчет прав доступа, то при построении результата также рассчитываем расширенный доступ по полям.
            await using var innerContext = new InnerContext(context, false);
            await this.InitInnerContextAsync(innerContext);
            if (!context.ValidationResult.IsSuccessful())
            {
                return KrPermissionsManagerResult.Empty;
            }

            if (context.WithRequiredPermissions)
            {
                await this.ExtendRequiredPermissionsWithRulesAsync(innerContext);

                if (context.Descriptor.AllChecked())
                {
                    return await this.CreateResultAsync(innerContext);
                }
            }

            await this.ExtendPermissionsWithTasksAsync(context);
            if (!context.ValidationResult.IsSuccessful())
            {
                return KrPermissionsManagerResult.Empty;
            }

            await this.CheckPermissionsWithCardAndTokenAsync(innerContext);
            if (!context.ValidationResult.IsSuccessful())
            {
                return KrPermissionsManagerResult.Empty;
            }

            if (context.Descriptor.AllChecked())
            {
                return await this.CreateResultAsync(innerContext);
            }

            if (await this.CheckRulesAsync(innerContext))
            {
                return await this.CreateResultAsync(innerContext);
            }

            if (!context.ValidationResult.IsSuccessful())
            {
                return KrPermissionsManagerResult.Empty;
            }

            return await this.CreateResultAsync(innerContext);
        }

        /// <inheritdoc />
        public async Task<KrPermissionsManagerCheckResult> CheckRequiredPermissionsAsync(
            IKrPermissionsManagerContext context,
            params KrPermissionFlagDescriptor[] requiredPermissions)
        {
            if (requiredPermissions == null
                || requiredPermissions.Length == 0)
            {
                return true;
            }

            context.Descriptor = new KrPermissionsDescriptor(requiredPermissions);
            context.Method = nameof(CheckRequiredPermissionsAsync);

            // Посследовательность проверки прав:
            // 1) Выполняем расширения по заданиям. Они могут как выдать права, так и запросить новые права.
            // 2) Выполняем расширения по карточке. Они могут как выдать права, так и запросить новые права.
            // 3) Проверяем расширения по токену. Проверка идет после расширений, чтобы была возможность перенести из токена запрашиваемые в расширениях права.
            //    если прав доступа достаточно, завершаемся.
            // 4) Если запрошена расширенная проверка прав доступа по полям, производим проверку прав доступа по полям.
            //    Если проверка прав по полям исчерпывающая (нет не проверенных полей), то проверка на редактирование карточки считается упсешной.
            //    Расширенная проверка прав может вернуть ошибку, если есть изменения полей/секций, которые запрещены для изменения.
            // 5) Выполняем проверку прав по правилам доступа.
            // 6) Если при проверке прав какие-то из прав не были успешно проверены, записываем сообщение об ошибки в результат валидации.
            await using (var innerContext = new InnerContext(context, true))
            {
                await InitInnerContextAsync(innerContext);
                if (!context.ValidationResult.IsSuccessful())
                {
                    return false;
                }

                await ExtendPermissionsWithTasksAsync(context);
                if (!context.ValidationResult.IsSuccessful())
                {
                    return false;
                }

                await CheckPermissionsWithCardAndTokenAsync(innerContext);
                if (!context.ValidationResult.IsSuccessful())
                {
                    return false;
                }

                if (context.WithExtendedPermissions)
                {
                    if (context.Mode == KrPermissionsCheckMode.WithStoreCard)
                    {
                        if (requiredPermissions.Contains(KrPermissionFlagDescriptors.EditCard))
                        {
                            await CheckExtendedPermissionsAsync(innerContext);
                            if (!context.ValidationResult.IsSuccessful())
                            {
                                return false;
                            }
                        }

                        await CheckExtendedMandatoryAsync(innerContext);
                        if (!context.ValidationResult.IsSuccessful())
                        {
                            if (innerContext.HasResultInfo)
                            {
                                return new KrPermissionsManagerCheckResult(false, innerContext.ResultInfo);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else if (context.FileID.HasValue
                        && requiredPermissions.Contains(KrPermissionFlagDescriptors.ReadCard))
                    {
                        await CheckExtendedFileRulesAsync(innerContext);
                        if (!context.ValidationResult.IsSuccessful())
                        {
                            return false;
                        }
                    }
                }

                if (context.Descriptor.AllChecked())
                {
                    if (innerContext.HasResultInfo)
                    {
                        return new KrPermissionsManagerCheckResult(true, innerContext.ResultInfo);
                    }
                    else
                    {
                        return true;
                    }
                }

                if (await CheckRulesAsync(innerContext))
                {
                    if (innerContext.HasResultInfo)
                    {
                        return new KrPermissionsManagerCheckResult(true, innerContext.ResultInfo);
                    }
                    else
                    {
                        return true;
                    }
                }

                if (!context.ValidationResult.IsSuccessful())
                {
                    return false;
                }

                if (context.Descriptor.AllChecked())
                {
                    if (innerContext.HasResultInfo)
                    {
                        return new KrPermissionsManagerCheckResult(true, innerContext.ResultInfo);
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            context.ValidationResult.AddError(
                this,
                KrPermissionsHelper.GetNotEnoughPermissionsErrorMessage(context.Descriptor.StillRequired.ToArray()));

            return false;
        }

        #endregion

        #region Private Methods

        #region Check Methods

        private async Task CheckPermissionsWithCardAndTokenAsync(InnerContext innerContext)
        {
            var context = innerContext.Context;
            
            try
            {
                await using var extensions =
                    await this.extensionContainer.ResolveExecutorAsync<ICardPermissionsExtension>(context.CancellationToken);
                
                await this.ExtendPermissionsWithCardAsync(context, extensions);
                await this.CheckPermissionsWithPreviousTokenAsync(innerContext, extensions);
                await CheckPermissionsWithServerTokenAsync(innerContext, extensions);
            }
            catch (Exception ex)
            {
                context.ValidationResult.AddException(this, ex);
            }
        }

        private static async Task CheckPermissionsWithServerTokenAsync(
            InnerContext innerContext,
            IExtensionExecutor<ICardPermissionsExtension> extensions)
        {
            var context = innerContext.Context;
            var serverToken = context.ServerToken;
            if (serverToken == null)
            {
                return;
            }
            var descriptor = context.Descriptor;

            foreach (var permission in serverToken.Permissions)
            {
                // НЕ Принудительно добавляем настройки доступа из предыдущего валидного токена
                // Если права были запрошены, они перенесутся. Если нет - проигнорируются.
                descriptor.Set(permission, true);
            }

            if (innerContext.CheckPermissions
                && context.WithExtendedPermissions
                && serverToken.ExtendedCardSettings != null)
            {
                HashSet<KrPermissionSectionSettings> settingsForClean = null;
                if (serverToken.ExtendedCardSettings.GetCardSettings() is ICollection<IKrPermissionSectionSettings> cardSettings)
                {
                    if (descriptor.ExtendedCardSettings.Count > 0)
                    {
                        settingsForClean = new HashSet<KrPermissionSectionSettings>();

                        // Если в дескрипторе уже есть настройки из предыдущего токена, то мержим их
                        foreach (var settings in cardSettings)
                        {
                            if (descriptor.ExtendedCardSettings.TryGetItem(settings.ID, out var existedSettings))
                            {
                                existedSettings.MergeWith(settings, true);
                                settingsForClean.Add(existedSettings);
                            }
                            else
                            {
                                descriptor.ExtendedCardSettings.Add(KrPermissionSectionSettings.ConvertFrom(settings));
                            }
                        }
                    }
                    else
                    {
                        // Переносим настройки из токена в дескриптор как есть
                        descriptor.ExtendedCardSettings.AddRange(
                            cardSettings.Select(x => KrPermissionSectionSettings.ConvertFrom(x)));
                    }
                }

                if (serverToken.ExtendedCardSettings.GetTasksSettings() is var tasksSettings
                    && tasksSettings != null
                    && tasksSettings.Count > 0)
                {
                    foreach (var taskSettings in tasksSettings)
                    {
                        var taskType = taskSettings.Key;
                        if (descriptor.ExtendedTasksSettings.TryGetValue(taskType, out var existedTaskSettings)
                            && existedTaskSettings != null
                            && existedTaskSettings.Count > 0)
                        {
                            if (settingsForClean == null)
                            {
                                settingsForClean = new HashSet<KrPermissionSectionSettings>();
                            }

                            // Если в дескрипторе уже есть настройки из предыдущего токена, то мержим их
                            foreach (var settings in taskSettings.Value)
                            {
                                if (existedTaskSettings.TryGetItem(settings.ID, out var existedSettings))
                                {
                                    existedSettings.MergeWith(settings, true);
                                    settingsForClean.Add(existedSettings);
                                }
                                else
                                {
                                    descriptor.ExtendedCardSettings.Add(KrPermissionSectionSettings.ConvertFrom(settings));
                                }
                            }
                        }
                        else
                        {
                            descriptor.ExtendedTasksSettings[taskType]
                                = new HashSet<Guid, KrPermissionSectionSettings>(
                                    x => x.ID,
                                    taskSettings.Value.Select(x => KrPermissionSectionSettings.ConvertFrom(x)));
                        }
                    }
                }

                // Вычищаем смерженные настройки, если такие были
                if (settingsForClean != null
                    && settingsForClean.Count > 0)
                {
                    foreach (var settings in settingsForClean)
                    {
                        settings.Clean();
                    }
                }
            }
        }

        private async Task CheckPermissionsWithPreviousTokenAsync(
            InnerContext innerContext,
            IExtensionExecutor<ICardPermissionsExtension> extensions)
        {
            var context = innerContext.Context;
            if (context.PreviousToken == null
                || context.Mode == KrPermissionsCheckMode.WithoutCard)
            {
                return;
            }

            var card = await GetCardForTokenAsync(context);

            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            switch (await this.krTokenProvider.ValidateTokenAsync(card, context.PreviousToken, context.ValidationResult))
            {
                case KrTokenValidationResult.Fail:
                    //Сообщение об ошибке было установлено в krTokenProvider.ValidateToken
                    return;

                case KrTokenValidationResult.Success:
                    // если в токене нет всех запрашиваемых прав, то выкидываем этот токен и перерасчитываем новый;
                    // или если есть расширение на правило доступа, которое говорит "пересчитай токен", то опять же перерасчитываем новый токен
                    if (!await CheckPermissionsRecalcRequiredWithExtensionsAsync(context, extensions))
                    {
                        foreach (var permission in context.PreviousToken.Permissions)
                        {
                            // НЕ Принудительно добавляем настройки доступа из предыдущего валидного токена
                            // Если права были запрошены, они перенесутся. Если нет - проигнорируются.
                            context.Descriptor.Set(permission, true);
                        }

                        if (innerContext.CheckPermissions
                            && context.WithExtendedPermissions)
                        {
                            // Считаем, что настройки рассчитаны, даже если в токене их не было
                            innerContext.ExtendedSettingsCalculated = true;
                            if (context.PreviousToken.ExtendedCardSettings != null)
                            {
                                // Переносим настройки из токена в дескриптор
                                context.Descriptor.ExtendedCardSettings.AddRange(
                                    context.PreviousToken.ExtendedCardSettings.GetCardSettings().Select(x => KrPermissionSectionSettings.ConvertFrom(x)));

                                if (context.PreviousToken.ExtendedCardSettings.GetTasksSettings() is var tasksSettings
                                    && tasksSettings != null
                                    && tasksSettings.Count > 0)
                                {
                                    foreach (var taskSettings in tasksSettings)
                                    {
                                        var taskType = taskSettings.Key;
                                        context.Descriptor.ExtendedTasksSettings[taskType]
                                            = new HashSet<Guid, KrPermissionSectionSettings>(
                                                x => x.ID,
                                                taskSettings.Value.Select(x => KrPermissionSectionSettings.ConvertFrom(x)));
                                    }
                                }

                                // Если идет проверка по файлу, то пытаемся сформировать правило доступа для этого файла
                                if (context.FileID.HasValue
                                    && context.PreviousToken.ExtendedCardSettings.GetFileSettings() is ICollection<KrPermissionFileSettings> fileSettings)
                                {
                                    foreach(var fileSetting in fileSettings)
                                    {
                                        if (fileSetting.FileID == context.FileID)
                                        {
                                            context.Descriptor.FileRules.Add(
                                                new KrPermissionFileRule(null, false)
                                                {
                                                    AccessSetting = fileSetting.AccessSetting,
                                                    CheckOwnFiles = true,
                                                });

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        return;
                    }
                    break;

                case KrTokenValidationResult.NeedRecreating:
                    return;
            }

            return;
        }

        /// <summary>
        /// Метод для проверки доступа по правилам
        /// </summary>
        /// <param name="context">Контекст проверки доступа</param>
        /// <returns>Возвращает true, есил проверка прав доступа полностью завершена, иначе false</returns>
        private async Task<bool> CheckRulesAsync(
            InnerContext innerContext)
        {
            if (!await FillInnerContextAsync(innerContext))
            {
                return false;
            }

            var context = innerContext.Context;
            //Проверенные правила - успешные и неуспешные
            var checkingRules = new HashSet<Guid, IKrPermissionRuleSettings>(
                x => x.ID,
                innerContext.PermissionsCache.GetRulesByTypeAndState(
                    context.DocTypeID ?? context.CardType.ID,
                    context.DocState)
                    .Where(x => x.Flags.Any(y => context.Descriptor.StillRequired.Contains(y))));

            if (await CheckRulesForStaticRolesAsync(
                    innerContext,
                    checkingRules))
            {
                return true;
            }

            if (!context.ValidationResult.IsSuccessful())
            {
                return false;
            }

            if (await CheckRulesForContextRolesAsync(
                    innerContext,
                    checkingRules))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод для проверки доступа по статическим ролям
        /// </summary>
        /// <param name="context">Контекст проверки доступа</param>
        /// <param name="submittedRules">Список правил доступа, которые уже прошли проверку</param>
        /// <param name="rejectedRules">Список правл доступа, которые не прошли проверку</param>
        /// <param name="executor">Объект, выполняющий расширения правил доступа</param>
        /// <param name="conditionContext">Контекст проверки условий для правил доступа. Задан, если предполагается проверка по условиям</param>
        /// <param name="conditionCompilationResult">Результат кокмпиляции условий. Задан, если предполагается проверка по условиям</param>
        /// <returns>Возвращает true, есил проверка прав доступа полностью завершена, иначе false</returns>
        private async Task<bool> CheckRulesForStaticRolesAsync(
            InnerContext innerContext,
            HashSet<Guid, IKrPermissionRuleSettings> checkingRules,
            bool force = false)
        {
            var context = innerContext.Context;

            // Нет смысла проверять список ролей, если правило уже было отклонено
            foreach (var rejectedRuleID in innerContext.RejectedRules)
            {
                checkingRules.RemoveByKey(rejectedRuleID);
            }

            foreach (var submittedRuleID in innerContext.SubmittedRules)
            {
                // Нет смысла проверять список ролей, если правило уже было проверено
                if (checkingRules.TryGetItem(submittedRuleID, out var rule))
                {
                    //Карточка была успешно проверена ранее - добавляем соотв. разрешение
                    foreach (var flag in rule.Flags)
                    {
                        context.Descriptor.Set(flag, true, force);
                    }
                    checkingRules.RemoveByKey(submittedRuleID);
                }
            }

            if (!force && context.Descriptor.AllChecked())
            {
                return true;
            }

            var rulesForStaticRoles =
                await GetRulesForStaticRolesAsync(
                    checkingRules);

            foreach (var ruleID in rulesForStaticRoles)
            {
                if (!force && context.Descriptor.AllChecked())
                {
                    return true;
                }

                var rule = checkingRules[ruleID];
                if (force)
                {
                    // Если нет флагов, которые можно было бы добавить, то не проверяем правило
                    if (!rule.Flags.Any(x => !context.Descriptor.Permissions.Contains(x)))
                    {
                        continue;
                    }
                }
                else
                {
                    // Если нет флагов, которые нужно проверить
                    if (!context.Descriptor.StillRequired.Any(x => rule.Flags.Contains(x)))
                    {
                        continue;
                    }
                }

                bool success = await CheckWithConditionsAsync(innerContext, rule)
                            && await CheckWithExtensionsAsync(innerContext, ruleID);

                if (!context.ValidationResult.IsSuccessful())
                {
                    return false;
                }

                if (success)
                {
                    innerContext.SubmittedRules.Add(ruleID);
                    //Карточка "пролезла" через правило - добавляем соотв. разрешение
                    foreach (var flag in rule.Flags)
                    {
                        context.Descriptor.Set(flag, true, force);
                    }
                }
                else
                {
                    innerContext.RejectedRules.Add(ruleID);
                }
            }

            return false;
        }

        /// <summary>
        /// Метод для проверки доступа по контекстным ролям
        /// </summary>
        /// <param name="context">Контекст проверки доступа</param>
        /// <param name="submittedRules">Список правил доступа, которые уже прошли проверку</param>
        /// <param name="rejectedRules">Список правл доступа, которые не прошли проверку</param>
        /// <param name="executor">Объект, выполняющий расширения правил доступа</param>
        /// <param name="conditionContext">Контекст проверки условий для правил доступа. Задан, если предполагается проверка по условиям</param>
        /// <param name="conditionCompilationResult">Результат кокмпиляции условий. Задан, если предполагается проверка по условиям</param>
        /// <returns>Возвращает true, есил проверка прав доступа полностью завершена, иначе false</returns>
        private async Task<bool> CheckRulesForContextRolesAsync(
            InnerContext innerContext,
            HashSet<Guid, IKrPermissionRuleSettings> checkingRules,
            bool force = false)
        {
            var context = innerContext.Context;
            if (context.Mode == KrPermissionsCheckMode.WithoutCard)
            {
                return false;
            }

            foreach (var rule in checkingRules)
            {
                if (context.Descriptor.AllChecked())
                {
                    return true;
                }

                // Если правило уже было проверено и не прошло, или в правиле нет контекстных ролей
                if (rule.ContextRoles.Count == 0
                    || innerContext.RejectedRules.Contains(rule.ID))
                {
                    continue;
                }

                if (force)
                {
                    // Если нет флагов, которые можно было бы добавить, то не проверяем правило
                    if (!rule.Flags.Any(x => !context.Descriptor.Permissions.Contains(x)))
                    {
                        continue;
                    }
                }
                else
                {
                    // Если нет флагов, которые нужно проверить
                    if (!context.Descriptor.StillRequired.Any(x => rule.Flags.Contains(x)))
                    {
                        continue;
                    }
                }

                // Если правило уже было проверено и прошло
                if (innerContext.SubmittedRules.Contains(rule.ID))
                {
                    //Карточка была успешно проверена ранее - добавляем соотв. разрешение
                    foreach (var flag in rule.Flags)
                    {
                        context.Descriptor.Set(flag, true, force);
                    }
                    continue;
                }

                bool success = await CheckWithConditionsAsync(innerContext, rule)
                    && await CheckWithExtensionsAsync(innerContext, rule.ID)
                    && await CheckUserInContextRolesAsync(
                        innerContext,
                        rule);

                if (!context.ValidationResult.IsSuccessful())
                {
                    return false;
                }

                if (success)
                {
                    innerContext.SubmittedRules.Add(rule.ID);

                    foreach (var flag in rule.Flags)
                    {
                        //Карточка "пролезла" через правило - добавляем соотв. разрешение
                        context.Descriptor.Set(flag, true, force);
                    }
                }
                else
                {
                    //Карточка не "пролезла" через правило
                    //Запоминаем что правило проверено не успешно
                    innerContext.RejectedRules.Add(rule.ID);
                }
            }

            return false;
        }

        private async Task<bool> CheckUserInContextRolesAsync(
            InnerContext innerContext,
            IKrPermissionRuleSettings rule)
        {
            // Сортировка по ролям обеспечивает уменьшение необходимости проверки всех контекстных ролей в правиле,
            // т.к. сперва всегда будут проверяться уже рассчитанные контекстные роли, а затем остальные
            foreach (Guid role in rule.ContextRoles)
            {
                if (innerContext.SubmittedRoles.Contains(role))
                {
                    return true;
                }
                else if (innerContext.RejectedRoles.Contains(role))
                {
                    continue;
                }

                Card contextRole = await roleCache.GetAsync(role);
                string sqlTextForUser = contextRole.DynamicEntries.ContextRoles.SqlTextForUser;
                string sqlTextForCard = contextRole.DynamicEntries.ContextRoles.SqlTextForCard;

                bool userInRole = await roleRepository.CheckUserInCardContextAsync(
                    role,
                    contextRole.Sections["Roles"].Fields.Get<string>("Name"),
                    sqlTextForUser,
                    sqlTextForCard,
                    innerContext.Context.CardID.Value,
                    session.User.ID,
                    useSafeTransaction: false);

                //юзер входит хотя бы в одну контекстную роль карточки
                if (userInRole)
                {
                    //Запоминаем как успешно проверенную роль
                    innerContext.SubmittedRoles.Add(role);
                    return true;
                }
                else
                {
                    //Запоминем как не успешно проверенную роль
                    innerContext.RejectedRoles.Add(role);
                }
            }
            return false;
        }

        private async Task<bool> CheckPermissionsRecalcRequiredWithExtensionsAsync(
            IKrPermissionsManagerContext context,
            IExtensionExecutor<ICardPermissionsExtension> extensions)
        {
            if (extensions != null)
            {
                await extensions.ExecuteAsync(x => x.IsPermissionsRecalcRequired, (IKrPermissionsRecalcContext)context);
                return ((IKrPermissionsRecalcContext)context).IsRecalcRequired;
            }

            return false;
        }

        private async Task<bool> CheckWithExtensionsAsync(
            InnerContext innerContext,
            Guid ruleID)
        {
            try
            {
                var ruleContext = new KrPermissionsRuleExtensionContext(innerContext.Context, ruleID);
                await innerContext.RuleExecutor.ExecuteAsync(x => x.CheckRuleAsync, ruleContext);

                if (ruleContext.Cancel)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                innerContext.Context.ValidationResult.AddException(this, ex);
                return false;
            }

            return true;
        }

        private async ValueTask<bool> CheckWithConditionsAsync(
            InnerContext innerContext,
            IKrPermissionRuleSettings rule)
        {
            if (innerContext.ConditionContext != null)
            {
                await using (dbScope.Create())
                {
                    if (rule != null)
                    {
                        return await
                            conditionExecutor.CheckConditionAsync(
                                rule.Conditions,
                                innerContext.ConditionContext,
                                innerContext.ConditionCompilationResult);
                    }
                }
            }

            return true;
        }

        private async Task CheckExtendedFileRulesAsync(
            InnerContext innerContext)
        {
            var context = innerContext.Context;
            var descriptor = context.Descriptor;
            int currentAccessSetting = int.MaxValue;

            await TryCalcExtendedFileRulesAsync(innerContext);

            if (descriptor.FileRules.Count > 0)
            {
                var (isOwnFile, categoryID, extension, lastVersion, versionAuthor) = await GetFileDataAsync(context);

                // Карточки нет в базе, значит она виртуальная
                if (lastVersion == Guid.Empty)
                {
                    return;
                }

                foreach(var fileRule in descriptor.FileRules)
                {
                    if (fileRule.AccessSetting < currentAccessSetting
                        && (fileRule.CheckOwnFiles || !isOwnFile)
                        && (fileRule.Extensions.Count == 0 || fileRule.Extensions.Contains(extension))
                        && (fileRule.Categories.Count == 0 || (categoryID is Guid id && fileRule.Categories.Contains(id))))
                    {
                        currentAccessSetting = fileRule.AccessSetting;
                        if (currentAccessSetting == KrPermissionsHelper.FileAccessSettings.FileNotAvailable)
                        {
                            break;
                        }
                    }
                }

                switch(currentAccessSetting)
                {
                    case KrPermissionsHelper.FileAccessSettings.FileNotAvailable:
                    case KrPermissionsHelper.FileAccessSettings.ContentNotAvailable:
                        context.ValidationResult.AddError(
                            this,
                            "$KrPermissions_FailToLoadFile");
                        break;

                    case KrPermissionsHelper.FileAccessSettings.OnlyLastVersion:
                    case KrPermissionsHelper.FileAccessSettings.OnlyLastAndOwnVersions:
                        if (context.FileVersionID.HasValue
                            && lastVersion != context.FileVersionID
                            && (currentAccessSetting != KrPermissionsHelper.FileAccessSettings.OnlyLastAndOwnVersions
                                || versionAuthor != session.User.ID))
                        {
                            context.ValidationResult.AddError(
                                this,
                                "$KrPermissions_FailToLoadVersions");
                        }
                        break;
                }

                innerContext.ResultInfo[KrPermissionsHelper.FileAccessSettings.InfoKey] = currentAccessSetting;
            }
        }

        private async Task CheckExtendedMandatoryAsync(
            InnerContext innerContext)
        {
            var context = innerContext.Context;
            var descriptor = context.Descriptor;

            await TryCalcExtendedMandatoryAsync(innerContext);

            if (descriptor.ExtendedMandatorySettings.Count == 0)
            {
                return;
            }

            var checkCard = await GetCardForMandatoryCheckAsync(context);
            var cardMeta = await cardMetadata.GetMetadataForTypeAsync(checkCard.TypeID, context.CancellationToken);
            var cardMetaSections = await cardMeta.GetSectionsAsync(context.CancellationToken);
            var checkTasks = checkCard.Tasks.Where(x => x.Action == CardTaskAction.Complete && x.OptionID.HasValue).ToArray();

            foreach (var mandatoryRule in descriptor.ExtendedMandatorySettings)
            {
                bool hasError = false;
                switch (mandatoryRule.ValidationType)
                {
                    case KrPermissionsHelper.MandatoryValidationType.Always:
                    case KrPermissionsHelper.MandatoryValidationType.WhenOneFieldFilled:
                        hasError = CheckExtendedMandatoryForCard(
                            context,
                            mandatoryRule,
                            checkCard,
                            cardMetaSections);
                        break;

                    case KrPermissionsHelper.MandatoryValidationType.OnTaskCompletion:
                        bool needCardCheck = true;
                        foreach (var task in checkTasks)
                        {
                            if (hasError)
                            {
                                break;
                            }

                            if (mandatoryRule.HasTaskTypes
                                && mandatoryRule.TaskTypes.Contains(task.TypeID)
                                && (!mandatoryRule.HasCompletionOptions
                                    || mandatoryRule.CompletionOptions.Contains(task.OptionID.Value)))
                            {
                                if (needCardCheck)
                                {
                                    needCardCheck = false;
                                    hasError = CheckExtendedMandatoryForCard(
                                        context,
                                        mandatoryRule,
                                        checkCard,
                                        cardMetaSections);
                                }

                                if (!hasError)
                                {
                                    var taskMetaSections =
                                           await (await cardMetadata.GetMetadataForTypeAsync(task.TypeID, context.CancellationToken))
                                               .GetSectionsAsync(context.CancellationToken);

                                    hasError = CheckExtendedMandatoryForCard(
                                        context,
                                        mandatoryRule,
                                        task.Card,
                                        taskMetaSections);
                                }
                            }
                        }
                        break;
                }

                if (hasError)
                {
                    context.ValidationResult.AddError(
                        this,
                        LocalizationManager.Format(mandatoryRule.Text));
                    
                    if (!innerContext.ResultInfo.TryGetValue(KrPermissionsHelper.FailedMandatoryRulesKey, out var failedRules)
                        || !(failedRules is List<object> failedRulesList))
                    {
                        failedRulesList = new List<object>();
                        innerContext.ResultInfo[KrPermissionsHelper.FailedMandatoryRulesKey] = failedRulesList;
                    }

                    failedRulesList.Add(mandatoryRule);
                }
            }
        }

        private bool CheckExtendedMandatoryForCard(
            IKrPermissionsManagerContext context,
            KrPermissionMandatoryRule mandatoryRule,
            Card checkCard,
            CardMetadataSectionCollection cardMetaSections)
        {
            if (cardMetaSections.TryGetValue(mandatoryRule.SectionID, out var sectionMeta)
                && checkCard.Sections.TryGetValue(sectionMeta.Name, out var section))
            {
                if (mandatoryRule.HasColumns)
                {
                    return CheckExtendedMandatoryForSection(
                        context,
                        mandatoryRule,
                        section,
                        sectionMeta);
                }
                else if (sectionMeta.SectionType == CardSectionType.Table)
                {
                    return section.Rows.Count == 0;
                }
            }

            return false;
        }

        private bool CheckExtendedMandatoryForSection(
            IKrPermissionsManagerContext context,
            KrPermissionMandatoryRule mandatoryRule,
            CardSection section,
            CardMetadataSection sectionMeta)
        {
            if (sectionMeta.SectionType == CardSectionType.Entry)
            {
                return CheckExtendedMandatoryForEntry(
                    mandatoryRule,
                    section.RawFields,
                    sectionMeta);
            }
            else
            {
                foreach (var row in section.Rows)
                {
                    if (CheckExtendedMandatoryForEntry(
                        mandatoryRule,
                        row,
                        sectionMeta))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckExtendedMandatoryForEntry(
            KrPermissionMandatoryRule mandatoryRule,
            IDictionary<string, object> entry,
            CardMetadataSection sectionMeta)
        {
            bool hasError = false,
                 needCheck = mandatoryRule.ValidationType != KrPermissionsHelper.MandatoryValidationType.WhenOneFieldFilled;
            foreach (var column in mandatoryRule.ColumnIDs)
            {
                if (sectionMeta.Columns.TryGetValue(column, out var columnMeta))
                {
                    if (columnMeta.ColumnType == CardMetadataColumnType.Complex)
                    {
                        columnMeta = sectionMeta.Columns.FirstOrDefault(x =>
                            x.ComplexColumnIndex == columnMeta.ComplexColumnIndex
                            && x.ColumnType == CardMetadataColumnType.Physical
                            && x.IsReference);

                        if (columnMeta == null)
                        {
                            continue;
                        }
                    }

                    var checkValue = entry.TryGet<object>(columnMeta.Name);
                    if (checkValue == null
                        || (checkValue is string stringValue
                            && string.IsNullOrWhiteSpace(stringValue)))
                    {
                        hasError = true;
                        if (needCheck)
                        {
                            break;
                        }
                    }
                    else
                    {
                        needCheck = true;
                    }
                }
            }

            return needCheck
                && hasError;
        }

        private async Task CheckExtendedPermissionsAsync(
            InnerContext innerContext)
        {
            var context = innerContext.Context;
            var descriptor = context.Descriptor;

            await TryCalcExtendedPermissionsAsync(innerContext);

            Card checkCard = GetCardForExtendedCheck(context);
            bool editCheckCompleted = await CheckCardExtendedPermissionsAsync(
                context,
                checkCard,
                descriptor.ExtendedCardSettings);

            foreach (var task in checkCard.Tasks)
            {
                if (task.State == CardRowState.Inserted
                    || task.State == CardRowState.None)
                {
                    continue;
                }

                if (descriptor.ExtendedTasksSettings.TryGetValue(task.TypeID, out var taskSettings))
                {
                    if (task.Action != CardTaskAction.Complete)
                    {
                        await CheckCardExtendedPermissionsAsync(
                            context,
                            task.Card,
                            taskSettings);
                    }
                    else
                    {
                        var cardClone = task.Card.Clone();
                        cardClone.RemoveAllButChanged();
                        await CheckCardExtendedPermissionsAsync(
                            context,
                            cardClone,
                            taskSettings);
                    }
                }
            }

            // Если проверка на Edit прошла по настройкам полей, то ставим его как проверенный
            if (editCheckCompleted)
            {
                context.Descriptor.Set(KrPermissionFlagDescriptors.EditCard, true);
            }
        }

        private async Task<bool> CheckCardExtendedPermissionsAsync(
            IKrPermissionsManagerContext context,
            Card card,
            HashSet<Guid, KrPermissionSectionSettings> settings)
        {
            bool result = true;
            ICardMetadata cardTypeMeta = await cardMetadata.GetMetadataForTypeAsync(card.TypeID, context.CancellationToken);
            var sections = await cardTypeMeta.GetSectionsAsync(context.CancellationToken);

            foreach (var section in card.Sections.Values)
            {
                if (IgnoreSections.Contains(section.Name)
                    || context.IgnoreSections.Contains(section.Name))
                {
                    continue;
                }

                if (sections.TryGetValue(section.Name, out var sectionMeta)
                    && settings.TryGetItem(sectionMeta.ID, out var sectionSettings))
                {
                    result =
                        CheckSectionExtendedPermissions(
                            context,
                            section,
                            sectionMeta,
                            sectionSettings) && result;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private bool CheckSectionExtendedPermissions(
            IKrPermissionsManagerContext context,
            CardSection section,
            CardMetadataSection sectionMeta,
            IKrPermissionSectionSettings sectionSettings)
        {
            // Проверка секций идет по следующему алгоритму:
            // Сперва идем по полям секции/строки.
            // Если доступ к полю запрещен - ошибка
            // Если доступ к полю или самой секции разрешен, то считаем, что расчет редактирования для них прошел успешно
            // Если расчет не прошел до конца (в полях или строках еще есть непроверенные значения) и стоит запрет изменения секции, то ошибка

            // Расчет доступа на добавление/удаление строк живет отдельно.

            // Определяет, успешно ли прошла проверка. Если нет, то либо была ошибка, либо есть поля, проверка которых не прошла
            bool result = true;
            if (section.Type == CardSectionType.Entry)
            {
                result =
                    CheckEntryExtendedPermissions(
                        context,
                        section,
                        section.RawFields,
                        sectionMeta,
                        sectionSettings) && result;
            }
            else
            {
                bool addError = false,
                     deleteError = false;
                foreach (var row in section.Rows)
                {
                    switch (row.State)
                    {
                        case CardRowState.Deleted:
                            if (sectionSettings.DisallowRowDeleting)
                            {
                                deleteError = true;
                            }
                            else
                            {
                                result &= sectionSettings.IsAllowed && !sectionSettings.IsDisallowed;
                            }
                            break;

                        case CardRowState.Inserted:
                            if (sectionSettings.DisallowRowAdding)
                            {
                                addError = true;
                            }
                            else
                            {
                                result &= sectionSettings.IsAllowed && !sectionSettings.IsDisallowed;
                            }
                            break;

                        case CardRowState.Modified:
                            result =
                                CheckEntryExtendedPermissions(
                                    context,
                                    section,
                                    row,
                                    sectionMeta,
                                    sectionSettings) && result;
                            break;
                    }
                }

                if (addError)
                {
                    context.ValidationResult.AddError(
                       this,
                       "$KrPermissions_SectionAddRowAccessErrorTemplate",
                       section.Name);

                    result = false;
                }
                if (deleteError)
                {
                    context.ValidationResult.AddError(
                        this,
                        "$KrPermissions_SectionDeleteRowAccessErrorTemplate",
                        section.Name);

                    result = false;
                }
            }

            if (!result
                && sectionSettings.IsDisallowed)
            {
                context.ValidationResult.AddError(
                    this,
                    "$KrPermissions_SectionEditAccessErrorTemplate",
                    section.Name);

                return false;
            }

            return result;
        }

        private bool CheckEntryExtendedPermissions(
            IKrPermissionsManagerContext context,
            CardSection section,
            IDictionary<string, object> rawFields,
            CardMetadataSection sectionMeta,
            IKrPermissionSectionSettings sectionSettings)
        {
            bool result = true;
            bool defaultSectionAccess = sectionSettings.IsAllowed && !sectionSettings.IsDisallowed;
            List<int> checkedComplexColumns = null;
            foreach (var field in rawFields)
            {
                if (sectionMeta.Columns.TryGetValue(field.Key, out var fieldMeta))
                {
                    // Если колонка часть комплексной, то проверяем комплексную колонку
                    if (fieldMeta.ComplexColumnIndex != -1)
                    {
                        if (checkedComplexColumns == null)
                        {
                            checkedComplexColumns = new List<int>();
                        }
                        else if (checkedComplexColumns.Contains(fieldMeta.ComplexColumnIndex))
                        {
                            // Не проверяем одну и ту же комплексную колонку дважды
                            continue;
                        }

                        checkedComplexColumns.Add(fieldMeta.ComplexColumnIndex);
                        fieldMeta = sectionMeta.Columns.First(x =>
                            x.ComplexColumnIndex == fieldMeta.ComplexColumnIndex
                            && x.ColumnType == CardMetadataColumnType.Complex);
                    }

                    if (sectionSettings.DisallowedFields.Contains(fieldMeta.ID))
                    {
                        context.ValidationResult.AddError(
                           this,
                           "$KrPermissions_FieldEditAccessErrorTemplate",
                           fieldMeta.Name,
                           section.Name);

                        result = false;
                    }

                    result &= defaultSectionAccess || sectionSettings.AllowedFields.Contains(fieldMeta.ID);
                }
            }

            return result;
        }

        #endregion

        #region Calc Methods

        private async Task TryCalcExtendedMandatoryAsync(InnerContext innerContext)
        {
            var descriptor = innerContext.Context.Descriptor;
            // Если в дескрипторе еще нет рассчитаных прав, которые могли быть рассчитаны ранее, то рассчитываем права
            if (descriptor.ExtendedMandatorySettings.Count == 0)
            {
                await CalcExtendedPermissionsAsync(
                    innerContext,
                    mandatoryCalcAction,
                    mandatoryFilterFuncs);
            }
        }

        private async Task TryCalcExtendedPermissionsAsync(InnerContext innerContext)
        {
            if (!innerContext.ExtendedSettingsCalculated)
            {
                innerContext.ExtendedSettingsCalculated = true;
                await CalcExtendedPermissionsAsync(innerContext, sectionsAndMandatoryActions);
            }
        }

        private async Task TryCalcExtendedFileRulesAsync(InnerContext innerContext)
        {
            if (!innerContext.ExtendedSettingsCalculated)
            {
                innerContext.ExtendedSettingsCalculated = true;
                await CalcExtendedPermissionsAsync(innerContext, fileRulesCalcAction, fileRulesFilterFunc);
            }
        }

        private async Task CalcExtendedPermissionsAsync(
            InnerContext innerContext,
            CalcAction[] calcActions,
            FilterFunc[] filterFuncs = null)
        {
            if (!await FillInnerContextAsync(innerContext))
            {
                return;
            }

            var context = innerContext.Context;
            var descriptor = context.Descriptor;
            var allExtendedRules =
                innerContext.PermissionsCache.GetExtendedRules(
                    context.DocTypeID ?? context.CardType.ID,
                    context.DocState);

            if (filterFuncs != null
                && filterFuncs.Length > 0)
            {
                for(int i = 0; i < filterFuncs.Length; i++)
                {
                    var filterFunc = filterFuncs[i];
                    allExtendedRules = allExtendedRules.Where(x => filterFunc.Invoke(x));
                }
            }
            var extendedRules = new HashSet<Guid, IKrPermissionRuleSettings>(x => x.ID, allExtendedRules);

            // Нет смысла проверять список ролей, если правило уже было отклонено
            foreach (var rejectedRuleID in innerContext.RejectedRules)
            {
                extendedRules.RemoveByKey(rejectedRuleID);
            }

            foreach (var submittedRuleID in innerContext.SubmittedRules)
            {
                // Нет смысла проверять список ролей, если правило уже было проверено
                if (extendedRules.TryGetItem(submittedRuleID, out var rule))
                {
                    foreach(var action in calcActions)
                    {
                        action(context, rule);
                    }

                    extendedRules.RemoveByKey(submittedRuleID);
                }
            }

            var rulesWithStaticRoles = await GetRulesForStaticRolesAsync(extendedRules);
            foreach (var ruleID in rulesWithStaticRoles)
            {
                var rule = extendedRules[ruleID];

                // Правило либо будет успешно проверено, либо нет, поэтому сразу удаляем из списка правил, чтобы не проверять его при проверке контекстных ролей
                extendedRules.RemoveByKey(ruleID);
                if (await CheckWithConditionsAsync(innerContext, rule)
                    && await CheckWithExtensionsAsync(innerContext, rule.ID))
                {
                    foreach (var action in calcActions)
                    {
                        action(context, rule);
                    }
                }
            }

            foreach (var rule in extendedRules)
            {
                if (rule.ContextRoles.Count == 0)
                {
                    continue;
                }

                if (await CheckWithConditionsAsync(innerContext, rule)
                    && await CheckWithExtensionsAsync(innerContext, rule.ID)
                    && await CheckUserInContextRolesAsync(innerContext, rule))
                {
                    foreach (var action in calcActions)
                    {
                        action(context, rule);
                    }
                }
            }
        }

        private static void CalcExtendedPermissionsFromRule(
            IKrPermissionsManagerContext context,
            IKrPermissionRuleSettings rule)
        {
            var descriptor = context.Descriptor;

            // Сперва идет расчет прав на карточку
            foreach (var sectionSettings in rule.CardSettings)
            {
                if (descriptor.ExtendedCardSettings.TryGetItem(sectionSettings.ID, out var existedSectionSettings))
                {
                    existedSectionSettings.MergeWith(sectionSettings);
                }
                else
                {
                    descriptor.ExtendedCardSettings.Add(sectionSettings.Clone());
                }
            }

            if (!context.Info.TryGetValue(nameof(CalcExtendedPermissionsFromRule), out var taskTypesObj))
            {
                List<Guid> taskTypesList = null;
                if ((context.Mode == KrPermissionsCheckMode.WithCard
                    || context.Mode == KrPermissionsCheckMode.WithStoreCard)
                    && context.Card.Tasks.Count > 0)
                {
                    taskTypesList = new List<Guid>();
                    foreach (var task in context.Card.Tasks)
                    {
                        if (!taskTypesList.Contains(task.TypeID))
                        {
                            taskTypesList.Add(task.TypeID);
                        }
                    }
                }

                taskTypesObj = taskTypesList;
                context.Info[nameof(CalcExtendedPermissionsFromRule)] = taskTypesObj;
            }

            // Затем расчет прав по заданиям
            if (taskTypesObj is IEnumerable<Guid> taskTypes)
            {
                foreach (var taskType in taskTypes)
                {
                    if (!descriptor.ExtendedTasksSettings.TryGetValue(taskType, out var extendedTaskSettingsResult))
                    {
                        extendedTaskSettingsResult = descriptor.ExtendedTasksSettings[taskType] = new HashSet<Guid, KrPermissionSectionSettings>(x => x.ID);
                    }

                    if (rule.TaskSettingsByTypes.TryGetValue(taskType, out var extendedTaskSettings))
                    {
                        foreach (var sectionSettings in extendedTaskSettings)
                        {
                            if (extendedTaskSettingsResult.TryGetItem(sectionSettings.ID, out var existedSectionSettings))
                            {
                                existedSectionSettings.MergeWith(sectionSettings);
                            }
                            else
                            {
                                extendedTaskSettingsResult.Add(sectionSettings.Clone());
                            }
                        }
                    }
                }
            }
        }

        private static void CalcVisibilityFromRule(
            IKrPermissionsManagerContext context,
            IKrPermissionRuleSettings rule)
        {
            context.Descriptor.VisibilitySettings.AddRange(rule.VisibilitySettings);
        }

        private static void CalcExtendedMandatoryFromRule(
            IKrPermissionsManagerContext context,
            IKrPermissionRuleSettings rule)
        {
            context.Descriptor.ExtendedMandatorySettings.AddRange(rule.MandatoryRules);
        }

        private static void CalcExtendedFileRuleFromRule(
            IKrPermissionsManagerContext context,
            IKrPermissionRuleSettings rule)
        {
            context.Descriptor.FileRules.AddRange(rule.FileRules);
        }

        #endregion

        #region Extend Methods

        /// <summary>
        ///  Метод для расширения рассчитываемых прав доступа по правилам доступа
        /// </summary>
        /// <param name="innerContext">Контекст проверки прав доступа</param>
        /// <returns>Возвращает асинхронную задачу.</returns>
        private async Task ExtendRequiredPermissionsWithRulesAsync(InnerContext innerContext)
        {
            if (!await FillInnerContextAsync(innerContext))
            {
                return;
            }

            var requiredRules = new HashSet<Guid, IKrPermissionRuleSettings>(
                x => x.ID,
                innerContext.PermissionsCache.GetRequiredRules(
                    innerContext.Context.DocTypeID ?? innerContext.Context.CardType.ID,
                    innerContext.Context.DocState));

            await CheckRulesForStaticRolesAsync(
                innerContext,
                requiredRules,
                true);

            if (!innerContext.Context.ValidationResult.IsSuccessful())
            {
                return;
            }

            await CheckRulesForContextRolesAsync(
                 innerContext,
                 requiredRules,
                 true);
        }

        private async Task ExtendPermissionsWithCardAsync(
            IKrPermissionsManagerContext context,
            IExtensionExecutor<ICardPermissionsExtension> extensions)
        {
            try
            {
                await extensions.ExecuteAsync(x => x.ExtendPermissionsAsync, context);
            }
            catch (Exception ex)
            {
                context.ValidationResult.AddException(this, ex);
                return;
            }
        }

        private async Task ExtendPermissionsWithTasksAsync(IKrPermissionsManagerContext context)
        {
            if (context.Mode == KrPermissionsCheckMode.WithoutCard)
            {
                return;
            }

            var tasks = await GetTasksAsync(context);

            if (!context.ValidationResult.IsSuccessful()
                || tasks == null
                || tasks.Count == 0)
            {
                return;
            }

            var taskContext = new TaskPermissionsExtensionContext(context);

            try
            {
                var cardTypes = await cardMetadata.GetCardTypesAsync(context.CancellationToken);
                foreach (CardTask task in tasks)
                {
                    await using var extensions = await this.extensionContainer.ResolveExecutorAsync<ITaskPermissionsExtension>(context.CancellationToken);
                    taskContext.Task = task;
                    taskContext.TaskType = cardTypes[task.TypeID];
                    await extensions.ExecuteAsync(x => x.ExtendPermissionsAsync, taskContext);
                }
            }
            catch (Exception ex)
            {
                context.ValidationResult.AddException(this, ex);
                return;
            }

            return;
        }
        #endregion

        #region Additional Methods

        private async Task InitInnerContextAsync(InnerContext innerContext)
        {
            var context = innerContext.Context;
            innerContext.PermissionsCache = await permissionsCacheContainer.TryGetCacheAsync(context.ValidationResult, context.CancellationToken);
        }

        private async Task<bool> FillInnerContextAsync(InnerContext innerContext)
        {
            var context = innerContext.Context;

            if (innerContext.RuleExecutor != null)
            {
                return true;
            }

            var extensionExecutor = await extensionContainer.ResolveExecutorAsync<IKrPermissionsRuleExtension>(context.CancellationToken);
            innerContext.RuleExecutor = extensionExecutor;
            if (context.Mode == KrPermissionsCheckMode.WithoutCard)
            {
                return true;
            }

            var conditionContext =
                new ConditionContext(
                    context.CardID.Value,
                    (ct) => GetFullCardAsync(context),
                    dbScope,
                    session,
                    context.ValidationResult,
                    unityContainer)
                {
                    CancellationToken = context.CancellationToken,
                };

            var conditionCompilationResult = await conditionCompilationCache.GetAsync(context.CancellationToken);

            if (conditionCompilationResult != null
                && !conditionCompilationResult.ValidationResult.IsSuccessful())
            {
                context.ValidationResult.Add(conditionCompilationResult.ValidationResult);
                return false;
            }

            innerContext.ConditionContext = conditionContext;
            innerContext.ConditionCompilationResult = conditionCompilationResult;

            return true;
        }

        private async Task<Card> GetCardForMandatoryCheckAsync(IKrPermissionsManagerContext context)
        {
            var fullCard = (await GetFullCardAsync(context));

            if (context.Mode == KrPermissionsCheckMode.WithStoreCard)
            {
                fullCard = fullCard.Clone();
                var storeCard = context.Card;
                foreach (var section in storeCard.Sections)
                {
                    if (fullCard.Sections.TryGetValue(section.Key, out var fullSection))
                    {
                        if (section.Value.Type == CardSectionType.Entry)
                        {
                            StorageHelper.Merge(section.Value.GetStorage(), fullSection.GetStorage());
                        }
                        else
                        {
                            foreach (var row in section.Value.Rows)
                            {
                                if (row.State == CardRowState.Inserted)
                                {
                                    fullSection.Rows.AddValue(row);
                                }
                                else if (row.State == CardRowState.Deleted
                                    && fullSection.Rows.TryFirst(x => x.RowID == row.RowID, out var fullRow))
                                {
                                    fullSection.Rows.Remove(fullRow);
                                }
                                else if (row.State == CardRowState.Modified
                                    && fullSection.Rows.TryFirst(x => x.RowID == row.RowID, out fullRow))
                                {
                                    StorageHelper.Merge(row.GetStorage(), fullRow.GetStorage());
                                }
                            }
                        }
                    }
                }

                fullCard.Tasks.Clear();
                fullCard.Tasks.AddItems(storeCard.Tasks);
            }

            return fullCard;
        }

        private Card GetCardForExtendedCheck(IKrPermissionsManagerContext context)
        {
            var checkCard = context.Card;
            if (checkCard.StoreMode == CardStoreMode.Insert
                && context.PreviousToken != null
                && context.PreviousToken.Info.TryGet<object>(KrPermissionsHelper.NewCardSourceKey) is Dictionary<string, object> cardSource)
            {
                checkCard = checkCard.Clone();
                foreach (var section in checkCard.Sections.Values)
                {
                    if (section.Type == CardSectionType.Entry)
                    {
                        var entrySource = cardSource.TryGet<Dictionary<string, object>>(section.Name);

                        foreach (var field in section.RawFields.ToArray())
                        {
                            var fieldSource = entrySource?.TryGet<object>(field.Key);
                            if ((fieldSource is null && field.Value is null)
                                || (fieldSource?.Equals(field.Value) ?? false))
                            {
                                section.RawFields.Remove(field.Key);
                            }
                        }

                        if (section.RawFields.Count == 0)
                        {
                            checkCard.Sections.Remove(section.Name);
                        }
                    }
                    else if (section.Type == CardSectionType.Table)
                    {
                        var rowSources = cardSource
                            .TryGet<List<object>>(section.Name)
                            ?.ToDictionary(x => ((Dictionary<string, object>)x).TryGet<Guid>("RowID"), x => x);

                        foreach (var row in section.Rows.ToArray())
                        {
                            object rowSourceObj = null;
                            rowSources?.TryGetValue(row.RowID, out rowSourceObj);
                            var rowSource = rowSourceObj as Dictionary<string, object>;
                            if (row != null)
                            {
                                foreach (var field in row.ToArray())
                                {
                                    var fieldSource = rowSource?.TryGet<object>(field.Key);
                                    if ((fieldSource is null && field.Value is null)
                                        || (fieldSource?.Equals(field.Value) ?? false))
                                    {
                                        row.Remove(field.Key);
                                    }
                                }
                                if (row.Count == 0)
                                {
                                    section.Rows.Remove(row);
                                }
                            }
                        }
                        if (section.Rows.Count == 0)
                        {
                            checkCard.Sections.Remove(section.Name);
                        }
                    }
                }

                if (cardSource.TryGetValue("Files", out var filesSourceObj)
                    && filesSourceObj is List<object> filesSource)
                {
                    bool filesChecked = true;
                    Dictionary<Guid, Dictionary<string, object>> filesSourceDict
                        = filesSource.ToDictionary(
                            x => ((Dictionary<string, object>)x).Get<Guid>("RowID"),
                            x => ((Dictionary<string, object>)x).Get<Dictionary<string, object>>("ExternalSource"));

                    foreach (var file in checkCard.Files)
                    {
                        if (filesSourceDict.TryGetValue(file.RowID, out var sourceFileExternalSource))
                        {
                            // Проверка, что внешний источник данных файла соответствует информации, используемой при создании карточки
                            var fileExternalSource = file.ExternalSource?.GetStorage();
                            if (fileExternalSource == null
                                && sourceFileExternalSource != null)
                            {
                                filesChecked = false;
                            }
                            else if (fileExternalSource != null
                                && (sourceFileExternalSource?.Any(x =>
                                {
                                    var value = fileExternalSource.TryGet<object>(x.Key);
                                    return !((value is null && x.Value is null) || (value?.Equals(x.Value) ?? false));
                                }) ?? true))
                            {
                                // Ошибка, которая возникает только при подделке внешнего источника файлов
                                context.ValidationResult.AddError(
                                    this,
                                    "Unable to create file with unchecked external source");
                            }
                        }
                        else
                        {
                            filesChecked = false;
                        }
                    }

                    if (filesChecked)
                    {
                        context.Descriptor.Set(KrPermissionFlagDescriptors.AddFiles, true);
                    }
                }
            }

            return checkCard;
        }

        private async Task<Card> GetCardForTokenAsync(IKrPermissionsManagerContext context)
        {
            switch (context.Mode)
            {
                case KrPermissionsCheckMode.WithStoreCard:
                case KrPermissionsCheckMode.WithCard:
                    return context.Card;

                case KrPermissionsCheckMode.WithCardID:
                    await using (dbScope.Create())
                    {
                        var getContext = await this.cardGetStrategy.TryLoadCardInstanceAsync(
                            context.CardID.Value,
                            dbScope.Db,
                            cardMetadata,
                            context.ValidationResult,
                            cancellationToken: context.CancellationToken);

                        return getContext.Card;
                    }
                case KrPermissionsCheckMode.WithoutCard:
                    return new Card() { ID = context.CardType.ID, TypeID = context.CardType.ID };

                default:
                    return new Card();
            }
        }

        private async ValueTask<Card> GetFullCardAsync(IKrPermissionsManagerContext context)
        {
            if (context.Mode == KrPermissionsCheckMode.WithCard)
            {
                return context.Card;
            }
            else if (context.Info.TryGetValue("FullCard", out var cardObj)
                && cardObj is Card card)
            {
                return card;
            }
            else
            {
                var cardRequest = new CardGetRequest() { CardID = context.CardID };
                permissionsProvider.SetFullPermissions(cardRequest);

                var response = await cardRepository.GetAsync(cardRequest, context.CancellationToken);
                context.ValidationResult.Add(response.ValidationResult);

                context.Info["FullCard"] = response.Card;
                return response.Card;
            }
        }

        private async Task<ICollection<CardTask>> GetTasksAsync(IKrPermissionsManagerContext context)
        {
            if (context.Card != null)
            {
                return context.Card.Tasks.ToArray();
            }

            await using (dbScope.Create())
            {
                var card = new Card();
                var taskContexts = await this.cardGetStrategy.TryLoadTaskInstancesAsync(
                    context.CardID.Value,
                    card,
                    dbScope.Db,
                    cardMetadata,
                    context.ValidationResult,
                    session.User.ID,
                    CardNewMode.Default,
                    CardGetTaskMode.Default,
                    cancellationToken: context.CancellationToken);

                if (taskContexts != null
                    && taskContexts.Count > 0)
                {
                    foreach (var taskContext in taskContexts)
                    {
                        await cardGetStrategy.LoadSectionsAsync(taskContext, context.CancellationToken);
                    }

                    return card.Tasks.ToArray();
                }
            }

            return EmptyHolder<CardTask>.Array;
        }

        private async Task<(bool isOwnFile, Guid? categoryID, string extension, Guid lastVersion, Guid? versionAuthor)> GetFileDataAsync(IKrPermissionsManagerContext context)
        {
            if (!context.FileID.HasValue)
            {
                return default;
            }

            await using (dbScope.Create())
            {

                var db = dbScope.Db;
                var builder =
                    dbScope.BuilderFactory
                        .Select().Top(1)
                            .C("f", "Name", "CreatedByID", "CategoryID", "VersionRowID");

                if (context.FileVersionID.HasValue)
                {
                    builder
                        .C("fv", "CreatedByID");
                }

                builder.From("Files", "f").NoLock();

                if (context.FileVersionID.HasValue)
                {
                    builder.CrossJoin("FileVersions", "fv").NoLock();
                }

                builder.Where().C("f", "RowID").Equals().P("FileID");

                if (context.FileVersionID.HasValue)
                {
                    builder.And().C("fv", "RowID").Equals().P("FileVersionID");
                }

                db.SetCommand(
                    builder.Limit(1).Build(),
                    db.Parameter("FileID", context.FileID),
                    db.Parameter("FileVersionID", context.FileVersionID))
                    .LogCommand();

                await using var reader = await db.ExecuteReaderAsync(context.CancellationToken);
                if (await reader.ReadAsync(context.CancellationToken))
                {
                    string extension = FileHelper.GetExtension(reader.GetValue<string>(0)).TrimStart('.');
                    bool isOwnFile = reader.GetGuid(1) == this.session.User.ID;
                    Guid? categoryID = reader.GetValue<Guid?>(2);
                    Guid lastVersion = reader.GetGuid(3);
                    Guid? versionAuthor = null;

                    if (context.FileVersionID.HasValue)
                    {
                        versionAuthor = reader.GetGuid(4);
                    }


                    return (isOwnFile, categoryID, extension, lastVersion, versionAuthor);
                }
            }

            return default;
        }


        private async Task<IEnumerable<Guid>> GetRulesForStaticRolesAsync(
            IEnumerable<IKrPermissionRuleSettings> rules)
        {
            var count = rules.Count();
            if (count == 0)
            {
                return EmptyHolder<Guid>.Collection;
            }

            await using (dbScope.Create())
            {
                const int step = 1000;
                if (count <= step)
                {
                    var db = dbScope.Db;
                    var builderFactory = dbScope.BuilderFactory;
                    var builder = builderFactory
                        .SelectDistinct().C("pr", "ID")
                        .From("KrPermissionRoles", "pr").NoLock()
                        .InnerJoin("RoleUsers", "ru").NoLock()
                            .On().C("ru", "ID").Equals().C("pr", "RoleID")
                        .Where().C("ru", "UserID").Equals().P("CurrentUserID")
                            .And().C("pr", "ID").In(rules.Select(x => x.ID));

                    var userID = session.User.ID;

                    return await db
                        .SetCommand(
                            builder.Build(),
                            db.Parameter("CurrentUserID", userID))
                        .LogCommand()
                        .ExecuteListAsync<Guid>();
                }
                else
                {
                    int stepNum = 0;
                    List<Guid> result = new List<Guid>();

                    while (stepNum * step < count)
                    {
                        var db = dbScope.Db;
                        var builderFactory = dbScope.BuilderFactory;
                        var builder = builderFactory
                            .SelectDistinct().C("pr", "ID")
                            .From("KrPermissionRoles", "pr").NoLock()
                            .InnerJoin("RoleUsers", "ru").NoLock()
                                .On().C("ru", "ID").Equals().C("pr", "RoleID")
                            .Where().C("ru", "UserID").Equals().P("CurrentUserID")
                                .And().C("pr", "ID").In(rules.Skip(stepNum * step).Take(step).Select(x => x.ID));

                        var userID = session.User.ID;

                        result.AddRange(await db
                            .SetCommand(
                                builder.Build(),
                                db.Parameter("CurrentUserID", userID))
                            .LogCommand()
                            .ExecuteListAsync<Guid>());

                        stepNum++;
                    }

                    return result;
                }
            }
        }

        private async Task<IKrPermissionsManagerResult> CreateResultAsync(InnerContext innerContext)
        {
            var descriptor = innerContext.Context.Descriptor;
            if (innerContext.Context.WithExtendedPermissions)
            {
                await CalcExtendedPermissionsAsync(innerContext, allCalcActions);

                var cardType = innerContext.Context.CardType;
                foreach (var settings in descriptor.ExtendedCardSettings.ToArray())
                {
                    if (!settings.CheckAndClean(cardType))
                    {
                        descriptor.ExtendedCardSettings.Remove(settings);
                    }
                }
            }

            return new KrPermissionsManagerResult(descriptor, innerContext.Context.WithExtendedPermissions, innerContext.PermissionsCache.Version);
        }

        #endregion

        #endregion
    }
}