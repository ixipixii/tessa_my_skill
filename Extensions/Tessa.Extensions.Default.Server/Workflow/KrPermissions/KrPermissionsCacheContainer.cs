﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <inheritdoc />
    public sealed class KrPermissionsCacheContainer : IKrPermissionsCacheContainer
    {
        #region Nested Types

        private class KrPermissionsByType : Dictionary<Guid, KrPermissionsByState>
        { }

        private class KrPermissionsByState : Dictionary<int, KrPermissionsLists>
        { }

        private class KrPermissionsLists
        {
            public List<IKrPermissionRuleSettings> ExtendedRules { get; } = new List<IKrPermissionRuleSettings>();
            public List<IKrPermissionRuleSettings> RequiredRules { get; } = new List<IKrPermissionRuleSettings>();
            public List<IKrPermissionRuleSettings> AllRules { get; } = new List<IKrPermissionRuleSettings>();
        }

        /// <inheritdoc />
        private class KrPermissionsCacheObject : IKrPermissionsCache
        {
            #region Constructors

            public KrPermissionsCacheObject(long version)
            {
                this.Version = version;
            }

            #endregion

            #region Properties

            public KrPermissionsByType PermissionsByType { get; set; }
            public IDictionary<Guid, IKrPermissionRuleSettings> PermissionsByID { get; set; }

            #endregion

            #region IKrPermissionsCache Implementation

            /// <inheritdoc />
            public long Version { get; }

            /// <inheritdoc />
            public IDictionary<Guid, IKrPermissionRuleSettings> GetAll()
            {
                return this.PermissionsByID;
            }

            /// <inheritdoc />
            public IKrPermissionRuleSettings GetRuleByID(Guid ruleID)
            {
                return this.PermissionsByID.TryGetValue(ruleID, out var result)
                    ? result
                    : null;
            }

            /// <inheritdoc />
            public IEnumerable<IKrPermissionRuleSettings> GetExtendedRules(Guid typeID, KrState? state)
            {
                if (this.PermissionsByType.TryGetValue(typeID, out var byState)
                    && byState.TryGetValue(state?.ID ?? CreateFakeStateID, out var lists))
                {
                    return lists.ExtendedRules;
                }

                return EmptyHolder<IKrPermissionRuleSettings>.Array;
            }

            /// <inheritdoc />
            public IEnumerable<IKrPermissionRuleSettings> GetRequiredRules(Guid typeID, KrState? state)
            {
                if (this.PermissionsByType.TryGetValue(typeID, out var byState)
                    && byState.TryGetValue(state?.ID ?? CreateFakeStateID, out var lists))
                {
                    return lists.RequiredRules;
                }

                return EmptyHolder<IKrPermissionRuleSettings>.Array;
            }

            /// <inheritdoc />
            public IEnumerable<IKrPermissionRuleSettings> GetRulesByTypeAndState(Guid typeID, KrState? state)
            {
                if (this.PermissionsByType.TryGetValue(typeID, out var byState)
                    && byState.TryGetValue(state?.ID ?? CreateFakeStateID, out var lists))
                {
                    return lists.AllRules;
                }

                return EmptyHolder<IKrPermissionRuleSettings>.Array;
            }

            #endregion
        }

        #endregion

        #region Fields

        private const int CreateFakeStateID = int.MinValue;

        private readonly IDbScope dbScope;
        private readonly ICardCache cache;
        private readonly ICardMetadata cardMetadata;
        private readonly ISchemeService schemeService;
        private readonly IKrPermissionsLockStrategy lockStrategy;
        private readonly AsyncLock asyncLock = new AsyncLock();

        private const string CacheKey = nameof(IKrPermissionsCacheContainer);
        private const string VersionKey = CacheKey + "Version";

        #endregion

        #region Constructors

        public KrPermissionsCacheContainer(
            IDbScope dbScope,
            ICardCache cache,
            ICardMetadata cardMetadata,
            ISchemeService schemeService,
            IKrPermissionsLockStrategy lockStrategy)
        {
            this.dbScope = dbScope;
            this.cache = cache;
            this.cardMetadata = cardMetadata;
            this.schemeService = schemeService;
            this.lockStrategy = lockStrategy;
        }

        #endregion

        #region IKrPermissionsCacheContainer Implementation

        /// <inheritdoc />
        public async ValueTask<long> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            var result = await cache.Settings.TryGetAsync<long?>(VersionKey, cancellationToken);
            if (result.HasValue)
            {
                return result.Value;
            }

            using (await asyncLock.EnterAsync(cancellationToken))
            {
                result = await cache.Settings.TryGetAsync<long?>(VersionKey, cancellationToken);
                if (result.HasValue)
                {
                    return result.Value;
                }

                var dbResult = await GetVersionFromDatabaseAsync(cancellationToken);
                return await cache.Settings.GetAsync(VersionKey, (x) => dbResult, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task UpdateVersionAsync(CancellationToken cancellationToken = default)
        {
            using (await asyncLock.EnterAsync(cancellationToken))
            {
                await cache.Settings.InvalidateAsync(VersionKey, cancellationToken);
                await cache.Settings.InvalidateAsync(CacheKey, cancellationToken);
                await IncreaseVersionAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async ValueTask<IKrPermissionsCache> TryGetCacheAsync(IValidationResultBuilder validationResult, CancellationToken cancellationToken = default)
        {
            var result = await cache.Settings.TryGetAsync<KrPermissionsCacheObject>(CacheKey, cancellationToken);
            if (result != null)
            {
                return result;
            }

            using (await asyncLock.EnterAsync(cancellationToken))
            {
                result = await cache.Settings.TryGetAsync<KrPermissionsCacheObject>(CacheKey, cancellationToken);
                if (result != null)
                {
                    return result;
                }

                result = await GetPermissionsFromDatabaseAsync(validationResult, cancellationToken);
                if (validationResult.IsSuccessful())
                {
                    await cache.Settings.GetAsync(VersionKey, (x) => result.Version, cancellationToken);
                    return await cache.Settings.GetAsync(CacheKey, (x) => result, cancellationToken);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task IncreaseVersionAsync(CancellationToken cancellationToken)
        {
            await using (dbScope.CreateNew())
            {
                var executor = dbScope.Executor;
                await executor.ExecuteNonQueryAsync(
                    dbScope.BuilderFactory
                        .Update(KrPermissionsHelper.SystemTable)
                        .C("Version").Assign().C("Version").Add(1)
                        .Build(),
                        cancellationToken);
            }
        }

        private async Task<long> GetVersionFromDatabaseAsync(CancellationToken cancellationToken)
        {
            await using (dbScope.CreateNew())
            {
                var db = dbScope.Db;

                return await db.SetCommand(dbScope.BuilderFactory
                    .Select().Top(1).C("Version")
                    .From(KrPermissionsHelper.SystemTable).NoLock()
                    .Limit(1)
                    .Build())
                    .LogCommand()
                    .ExecuteAsync<long>(cancellationToken);
            }
        }

        private async Task<KrPermissionsCacheObject> GetPermissionsFromDatabaseAsync(IValidationResultBuilder validationResult, CancellationToken cancellationToken)
        {
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID = new Dictionary<Guid, IKrPermissionRuleSettings>();
            var lockObject = await lockStrategy.TryObtainReaderLockAsync(cancellationToken);
            if (lockObject == null)
            {
                validationResult.AddError(
                    this,
                    "$KrPermissions_UnableToLoadPermissionsMessage");
                return null;
            }
            await using (lockObject)
            {
                var result = new KrPermissionsCacheObject(await GetVersionFromDatabaseAsync(cancellationToken));

                await using (dbScope.CreateNew())
                {
                    var db = dbScope.Db;

                    await FillPermissionMainAsync(db, permissionsByID, cancellationToken);
                    await FillPermissionContextRolesAsync(db, permissionsByID, cancellationToken);
                    await FillPermissionHashAsync<Guid>(db, "KrPermissionTypes", "TypeID", permissionsByID, (p, v) => p.Types.Add(v), cancellationToken);
                    await FillPermissionHashAsync<short>(db, "KrPermissionStates", "StateID", permissionsByID, (p, v) => p.States.Add(v), cancellationToken);
                    await FillPermissionCardExtendedSettingsAsync(db, permissionsByID, cancellationToken);
                    await FillPermissionTaskExtendedSettingsAsync(db, permissionsByID, cancellationToken);
                    await FillPermissionMandatoryExtendedSettingsAsync(db, permissionsByID, cancellationToken);
                    await FillPermissionVisibilityExtendedSettingsAsync(db, permissionsByID, cancellationToken);
                    await FillPermissionFileExtendedSettingsAsync(db, permissionsByID, cancellationToken);
                }

                result.PermissionsByID = permissionsByID;
                var byTypes = result.PermissionsByType = new KrPermissionsByType();

                foreach (var permission in permissionsByID.Values)
                {
                    foreach (var typeID in permission.Types)
                    {
                        if (!byTypes.TryGetValue(typeID, out var byState))
                        {
                            byState = new KrPermissionsByState();
                            byTypes[typeID] = byState;
                        }

                        foreach (var stateID in permission.States)
                        {
                            if (!byState.TryGetValue(stateID, out var lists))
                            {
                                lists = new KrPermissionsLists();
                                byState[stateID] = lists;
                            }

                            lists.AllRules.Add(permission);
                            if (permission.IsExtended)
                            {
                                lists.ExtendedRules.Add(permission);
                            }
                            if (permission.IsRequired)
                            {
                                lists.RequiredRules.Add(permission);
                            }
                        }

                        // Создаем список со всеми правилами, где есть создание карточки данного типа
                        if (permission.Flags.Contains(KrPermissionFlagDescriptors.CreateCard))
                        {
                            if (!byState.TryGetValue(CreateFakeStateID, out var lists))
                            {
                                lists = new KrPermissionsLists();
                                byState[CreateFakeStateID] = lists;
                            }

                            lists.AllRules.Add(permission);
                            if (permission.IsExtended)
                            {
                                lists.ExtendedRules.Add(permission);
                            }
                            if (permission.IsRequired)
                            {
                                lists.RequiredRules.Add(permission);
                            }
                        }
                    }
                }

                return result;
            }
        }

        private async Task FillPermissionFileExtendedSettingsAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken)
        {
            db.SetCommand(
                   dbScope.BuilderFactory
                       .Select()
                           .C("r", "ID", "RowID", "Extensions", "CheckOwnFiles", "AccessSettingID") // 0 - 4
                           .C("c", "CategoryID") // 5
                       .From("KrPermissionExtendedFileRules", "r").NoLock()
                       .LeftJoin("KrPermissionExtendedFileRuleCategories", "c").NoLock()
                           .On().C("c", "RuleRowID").Equals().C("r", "RowID")
                       .OrderBy("r", "ID").By("r", "RowID")
                       .Build())
                       .LogCommand();

            await using var reader = await db.ExecuteReaderAsync(cancellationToken);
            var prevRuleID = Guid.Empty;
            IKrPermissionRuleSettings prevRule = null;

            var prevFileRuleID = Guid.Empty;
            KrPermissionFileRule prevFileRule = null;

            while (await reader.ReadAsync(cancellationToken))
            {
                var ruleID = reader.GetValue<Guid>(0);
                if (ruleID != prevRuleID)
                {
                    // При смене правила сбрасываем секцию
                    prevRuleID = ruleID;
                    prevFileRuleID = Guid.Empty;
                    if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                    {
                        continue;
                    }
                }

                // Правило, для которого актуальны данные настройки, отключено
                if (prevRule == null)
                {
                    continue;
                }

                var fileRuleID = reader.GetValue<Guid>(1);
                var categoryID = reader.GetValue<Guid?>(5);

                if (fileRuleID != prevFileRuleID)
                {
                    var extensions = reader.GetValue<string>(2);
                    var checkOwnFiles = reader.GetValue<bool>(3);
                    var accessSettings = reader.GetValue<int>(4);

                    prevFileRuleID = fileRuleID;
                    prevFileRule = new KrPermissionFileRule(extensions, categoryID.HasValue)
                    {
                        CheckOwnFiles = checkOwnFiles,
                        AccessSetting = accessSettings,
                    };

                    prevRule.FileRules.Add(prevFileRule);
                }

                if (categoryID.HasValue)
                {
                    prevFileRule.Categories.Add(fileRuleID);
                }
            }
        }

        private async Task FillPermissionVisibilityExtendedSettingsAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken)
        {
            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("s", "ID", "Alias", "ControlTypeID", "IsHidden") // 0 - 3
                    .From("KrPermissionExtendedVisibilityRules", "s").NoLock()
                    .OrderBy("s", "ID")
                    .Build())
                    .LogCommand();

            await using var reader = await db.ExecuteReaderAsync(cancellationToken);
            var prevRuleID = Guid.Empty;
            IKrPermissionRuleSettings prevRule = null;
            while (await reader.ReadAsync(cancellationToken))
            {
                var ruleID = reader.GetValue<Guid>(0);
                if (ruleID != prevRuleID)
                {
                    // При смене правила сбрасываем секцию
                    prevRuleID = ruleID;
                    if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                    {
                        continue;
                    }
                }

                // Правило, для которого актуальны данные настройки, отключено
                if (prevRule == null)
                {
                    continue;
                }

                prevRule.VisibilitySettings.Add(
                    new KrPermissionVisibilitySettings(
                        reader.GetValue<string>(1),
                        reader.GetValue<int>(2),
                        reader.GetValue<bool>(3)));
            }
        }

        private async Task FillPermissionMandatoryExtendedSettingsAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken)
        {
            var allMandatoryRules = new Dictionary<Guid, KrPermissionMandatoryRule>();
            var allSections = await this.cardMetadata.GetSectionsAsync(cancellationToken);

            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("s", "ID", "RowID", "SectionID", "ValidationTypeID", "Text") // 0 - 4
                        .C("f", "FieldID") // 5
                    .From("KrPermissionExtendedMandatoryRules", "s").NoLock()
                    .LeftJoin("KrPermissionExtendedMandatoryRuleFields", "f").NoLock()
                        .On().C("f", "RuleRowID").Equals().C("s", "RowID")
                    .OrderBy("s", "ID").By("s", "SectionID")
                    .Build())
                    .LogCommand();

            // Сперва грузим все настройки по строкам. При этом настройки для одной и той же секции в разных строках пишутся в несколько объектов
            await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
            {
                var prevRuleID = Guid.Empty;
                IKrPermissionRuleSettings prevRule = null;

                var prevRowID = Guid.Empty;
                KrPermissionMandatoryRule prevMandatory = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    var ruleID = reader.GetValue<Guid>(0);
                    if (ruleID != prevRuleID)
                    {
                        // При смене правила сбрасываем секцию
                        prevRuleID = ruleID;
                        prevRowID = Guid.Empty;
                        if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                        {
                            continue;
                        }
                    }

                    // Правило, для которого актуальны данные настройки, отключено
                    if (prevRule == null)
                    {
                        continue;
                    }

                    var rowID = reader.GetValue<Guid>(1);
                    if (rowID != prevRowID)
                    {
                        var sectionID = reader.GetValue<Guid>(2);
                        var validationTypeID = reader.GetValue<int>(3);
                        var text = reader.GetValue<string>(4);

                        prevRowID = rowID;
                        prevMandatory = new KrPermissionMandatoryRule(
                            sectionID,
                            text,
                            validationTypeID);
                        allMandatoryRules[rowID] = prevMandatory;
                        prevRule.MandatoryRules.Add(prevMandatory);
                    }

                    var fieldID = reader.GetValue<Guid?>(5);

                    if (fieldID.HasValue)
                    {
                        prevMandatory.ColumnIDs.Add(fieldID.Value);
                    }
                }
            }

            // Затем грузим типы заданий по ID строкам и формируем результат по типам
            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("t", "ID", "RuleRowID", "TaskTypeID") // 0 - 2
                    .From("KrPermissionExtendedMandatoryRuleTypes", "t").NoLock()
                    .OrderBy("t", "ID").By("t", "RuleRowID")
                    .Build())
                    .LogCommand();

            await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
            {
                var prevRuleID = Guid.Empty;
                IKrPermissionRuleSettings prevRule = null;

                var prevMandatoryID = Guid.Empty;
                KrPermissionMandatoryRule prevMandatory = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    var ruleID = reader.GetValue<Guid>(0);
                    if (ruleID != prevRuleID)
                    {
                        prevRuleID = ruleID;
                        prevMandatoryID = Guid.Empty;
                        prevMandatory = null;
                        if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                        {
                            continue;
                        }
                    }

                    // Правило, для которого актуальны данные настройки, отключено
                    if (prevRule == null)
                    {
                        continue;
                    }

                    var mandatoryID = reader.GetValue<Guid>(1);
                    var typeID = reader.GetValue<Guid>(2);
                    if (mandatoryID != prevMandatoryID)
                    {
                        prevMandatoryID = mandatoryID;
                        prevMandatory = allMandatoryRules[mandatoryID];
                    }

                    prevMandatory.TaskTypes.Add(typeID);
                }
            }

            // Затем грузим варианты завершения по ID строкам и формируем результат по типам
            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("t", "ID", "RuleRowID", "OptionID") // 0 - 2
                    .From("KrPermissionExtendedMandatoryRuleOptions", "t").NoLock()
                    .OrderBy("t", "ID").By("t", "RuleRowID")
                    .Build())
                    .LogCommand();

            await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
            {
                var prevRuleID = Guid.Empty;
                IKrPermissionRuleSettings prevRule = null;

                var prevMandatoryID = Guid.Empty;
                KrPermissionMandatoryRule prevMandatory = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    var ruleID = reader.GetValue<Guid>(0);
                    if (ruleID != prevRuleID)
                    {
                        prevRuleID = ruleID;
                        prevMandatoryID = Guid.Empty;
                        prevMandatory = null;
                        if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                        {
                            continue;
                        }
                    }

                    // Правило, для которого актуальны данные настройки, отключено
                    if (prevRule == null)
                    {
                        continue;
                    }

                    var mandatoryID = reader.GetValue<Guid>(1);
                    var optionID = reader.GetValue<Guid>(2);
                    if (mandatoryID != prevMandatoryID)
                    {
                        prevMandatoryID = mandatoryID;
                        prevMandatory = allMandatoryRules[mandatoryID];
                    }

                    prevMandatory.CompletionOptions.Add(optionID);
                }
            }

            var sections = schemeService.GetTables().ToDictionary(x => x.ID, x => x);
            var sectionsMeta = await cardMetadata.GetSectionsAsync(cancellationToken);
            // Заполняем обязательность полей в настройках правил доступа
            foreach (var permission in permissionsByID.Values)
            {
                foreach (var mandatoryRule in permission.MandatoryRules)
                {
                    // Такой род обязательности не отмечаем на карточке
                    if (mandatoryRule.ValidationType == KrPermissionsHelper.MandatoryValidationType.WhenOneFieldFilled)
                    {
                        continue;
                    }

                    var section = sections[mandatoryRule.SectionID];
                    if (section.InstanceType == SchemeTableInstanceType.Cards
                        && mandatoryRule.ValidationType == KrPermissionsHelper.MandatoryValidationType.Always)
                    {
                        if (!permission.CardSettings.TryGetItem(mandatoryRule.SectionID, out var sectionSettings))
                        {
                            sectionSettings = new KrPermissionSectionSettings
                            {
                                ID = mandatoryRule.SectionID,
                            };

                            permission.CardSettings.Add(sectionSettings);
                        }

                        if (mandatoryRule.HasColumns)
                        {
                            sectionSettings.MandatoryFields.AddRange(mandatoryRule.ColumnIDs);
                        }
                        else
                        {
                            sectionSettings.IsMandatory = true;
                        }
                    }
                    else if (section.InstanceType == SchemeTableInstanceType.Tasks
                        && mandatoryRule.ValidationType == KrPermissionsHelper.MandatoryValidationType.OnTaskCompletion
                        && mandatoryRule.HasTaskTypes)
                    {
                        foreach (var taskType in mandatoryRule.TaskTypes)
                        {
                            var taskMeta = await cardMetadata.GetMetadataForTypeAsync(taskType, cancellationToken);
                            if (!(await taskMeta.GetSectionsAsync(cancellationToken)).Contains(mandatoryRule.SectionID))
                            {
                                // Эта секция отсутствует в данном типе задания
                                continue;
                            }

                            if (!permission.TaskSettingsByTypes.TryGetValue(taskType, out var taskSettigns))
                            {
                                taskSettigns = new HashSet<Guid, KrPermissionSectionSettings>(x => x.ID);
                                permission.TaskSettingsByTypes.Add(taskType, taskSettigns);
                            }

                            if (!taskSettigns.TryGetItem(mandatoryRule.SectionID, out var sectionSettings))
                            {
                                sectionSettings = new KrPermissionSectionSettings
                                {
                                    ID = mandatoryRule.SectionID,
                                };

                                taskSettigns.Add(sectionSettings);
                            }

                            if (mandatoryRule.HasColumns)
                            {
                                sectionSettings.MandatoryFields.AddRange(mandatoryRule.ColumnIDs);
                            }
                            else
                            {
                                sectionSettings.IsMandatory = true;
                            }

                        }
                    }
                }
            }
        }

        private async Task FillPermissionTaskExtendedSettingsAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken)
        {
            var allTaskSettings = new Dictionary<Guid, KrPermissionSectionSettings>();

            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("s", "ID", "RowID", "SectionID", "AccessSettingID", "IsHidden") // 0 - 4
                        .C("f", "FieldID") // 5
                    .From("KrPermissionExtendedTaskRules", "s").NoLock()
                    .LeftJoin("KrPermissionExtendedTaskRuleFields", "f").NoLock()
                        .On().C("f", "RuleRowID").Equals().C("s", "RowID")
                    .OrderBy("s", "ID").By("s", "SectionID")
                    .Build())
                    .LogCommand();

            // Сперва грузим все настройки по строкам. При этом настройки для одной и той же секции в разных строках пишутся в несколько объектов
            await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
            {
                var prevRuleID = Guid.Empty;
                IKrPermissionRuleSettings prevRule = null;

                var prevRowID = Guid.Empty;
                KrPermissionSectionSettings prevSection = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    var ruleID = reader.GetValue<Guid>(0);
                    if (ruleID != prevRuleID)
                    {
                        // При смене правила сбрасываем секцию
                        prevRuleID = ruleID;
                        prevRowID = Guid.Empty;
                        if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                        {
                            continue;
                        }
                    }

                    // Правило, для которого актуальны данные настройки, отключено
                    if (prevRule == null)
                    {
                        continue;
                    }

                    var rowID = reader.GetValue<Guid>(1);
                    var sectionID = reader.GetValue<Guid>(2);
                    if (rowID != prevRowID)
                    {
                        prevRowID = rowID;
                        prevSection = new KrPermissionSectionSettings
                        {
                            ID = sectionID,
                        };
                        allTaskSettings[rowID] = prevSection;
                    }

                    var accessSettings = reader.GetValue<int?>(3);
                    var isHidden = reader.GetValue<bool>(4);
                    var fieldID = reader.GetValue<Guid?>(5);

                    if (fieldID.HasValue)
                    {
                        if (isHidden)
                        {
                            prevSection.HiddenFields.Add(fieldID.Value);
                        }

                        if (accessSettings == KrPermissionsHelper.AccessSettings.AllowEdit)
                        {
                            prevSection.AllowedFields.Add(fieldID.Value);
                        }
                        else if (accessSettings == KrPermissionsHelper.AccessSettings.DisallowEdit)
                        {
                            prevSection.DisallowedFields.Add(fieldID.Value);
                        }
                    }
                    else
                    {
                        if (isHidden)
                        {
                            prevSection.IsHidden = true;
                        }

                        switch (accessSettings)
                        {
                            case KrPermissionsHelper.AccessSettings.AllowEdit:
                                prevSection.IsAllowed = true;
                                break;
                            case KrPermissionsHelper.AccessSettings.DisallowEdit:
                                prevSection.IsDisallowed = true;
                                break;
                            case KrPermissionsHelper.AccessSettings.DisallowRowAdding:
                                prevSection.DisallowRowAdding = true;
                                break;
                            case KrPermissionsHelper.AccessSettings.DisallowRowDeleting:
                                prevSection.DisallowRowDeleting = true;
                                break;
                        }
                    }
                }
            }

            // Затем грузим типы карточек заданий по ID строкам и формируем результат по типам
            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("t", "ID", "RuleRowID", "TaskTypeID") // 0 - 2
                    .From("KrPermissionExtendedTaskRuleTypes", "t").NoLock()
                    .OrderBy("t", "ID").By("t", "TaskTypeID")
                    .Build())
                    .LogCommand();

            await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
            {
                var prevRuleID = Guid.Empty;
                IKrPermissionRuleSettings prevRule = null;

                var prevTypeID = Guid.Empty;
                HashSet<Guid, KrPermissionSectionSettings> prevTypeSettings = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    var ruleID = reader.GetValue<Guid>(0);
                    if (ruleID != prevRuleID)
                    {
                        prevRuleID = ruleID;
                        prevTypeID = Guid.Empty;
                        if (prevTypeSettings != null)
                        {
                            prevTypeSettings.ForEach(x => x.Clean());
                            prevTypeSettings = null;
                        }
                        if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                        {
                            continue;
                        }
                    }

                    // Правило, для которого актуальны данные настройки, отключено
                    if (prevRule == null)
                    {
                        continue;
                    }

                    var typeID = reader.GetValue<Guid>(2);
                    if (typeID != prevTypeID)
                    {
                        prevTypeID = typeID;
                        if (prevTypeSettings != null)
                        {
                            prevTypeSettings.ForEach(x => x.Clean());
                        }

                        prevTypeSettings = new HashSet<Guid, KrPermissionSectionSettings>(x => x.ID);
                        prevRule.TaskSettingsByTypes[typeID] = prevTypeSettings;
                    }

                    var ruleRowID = reader.GetValue<Guid>(1);
                    var settings = allTaskSettings[ruleRowID];
                    if (prevTypeSettings.TryGetItem(settings.ID, out var existedSettings))
                    {
                        existedSettings.MergeWith(settings);
                    }
                    else
                    {
                        prevTypeSettings.Add(settings.Clone());
                    }
                }

                if (prevTypeSettings != null)
                {
                    prevTypeSettings.ForEach(x => x.Clean());
                    prevTypeSettings = null;
                }
            }
        }

        private async Task FillPermissionCardExtendedSettingsAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken)
        {
            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C("s", "ID", "SectionID", "IsHidden", "AccessSettingID", "Mask") // 0 - 4
                        .C("f", "FieldID") // 5
                    .From("KrPermissionExtendedCardRules", "s").NoLock()
                    .LeftJoin("KrPermissionExtendedCardRuleFields", "f").NoLock()
                        .On().C("f", "RuleRowID").Equals().C("s", "RowID")
                    .OrderBy("s", "ID").By("s", "SectionID")
                    .Build())
                    .LogCommand();

            await using var reader = await db.ExecuteReaderAsync(cancellationToken);
            var prevRuleID = Guid.Empty;
            IKrPermissionRuleSettings prevRule = null;

            var prevSectionID = Guid.Empty;
            KrPermissionSectionSettings prevSection = null;

            while (await reader.ReadAsync(cancellationToken))
            {
                var ruleID = reader.GetValue<Guid>(0);
                if (ruleID != prevRuleID)
                {
                    // При смене правила сбрасываем секцию
                    prevRuleID = ruleID;
                    prevSectionID = Guid.Empty;
                    if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                    {
                        continue;
                    }
                }

                // Правило, для которого актуальны данные настройки, отключено
                if (prevRule == null)
                {
                    continue;
                }

                var sectionID = reader.GetValue<Guid>(1);
                if (sectionID != prevSectionID)
                {
                    if (prevSection != null)
                    {
                        prevSection.Clean();
                    }

                    prevSectionID = sectionID;
                    prevSection = new KrPermissionSectionSettings
                    {
                        ID = sectionID,
                    };
                    prevRule.CardSettings.Add(prevSection);
                }

                var isHidden = reader.GetValue<bool>(2);
                var accessSettings = reader.GetValue<int?>(3);
                var mask = reader.GetValue<string>(4);
                var fieldID = reader.GetValue<Guid?>(5);

                if (fieldID.HasValue)
                {
                    if (isHidden)
                    {
                        prevSection.HiddenFields.Add(fieldID.Value);
                    }

                    if (accessSettings == KrPermissionsHelper.AccessSettings.AllowEdit)
                    {
                        prevSection.AllowedFields.Add(fieldID.Value);
                    }
                    else if (accessSettings == KrPermissionsHelper.AccessSettings.DisallowEdit)
                    {
                        prevSection.DisallowedFields.Add(fieldID.Value);
                    }
                    else if (accessSettings == KrPermissionsHelper.AccessSettings.MaskData)
                    {
                        prevSection.MaskedFields.Add(fieldID.Value);
                        prevSection.MaskedFieldsData[fieldID.Value] = mask;
                    }
                }
                else
                {
                    if (isHidden)
                    {
                        prevSection.IsHidden = true;
                    }

                    switch (accessSettings)
                    {
                        case KrPermissionsHelper.AccessSettings.AllowEdit:
                            prevSection.IsAllowed = true;
                            break;
                        case KrPermissionsHelper.AccessSettings.DisallowEdit:
                            prevSection.IsDisallowed = true;
                            break;
                        case KrPermissionsHelper.AccessSettings.DisallowRowAdding:
                            prevSection.DisallowRowAdding = true;
                            break;
                        case KrPermissionsHelper.AccessSettings.DisallowRowDeleting:
                            prevSection.DisallowRowDeleting = true;
                            break;
                        case KrPermissionsHelper.AccessSettings.MaskData:
                            prevSection.IsMasked = true;
                            prevSection.Mask = mask;
                            break;
                    }
                }
            }

            if (prevSection != null)
            {
                prevSection.Clean();
            }
        }

        private async Task FillPermissionContextRolesAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken)
        {
            var builderFactory = dbScope.BuilderFactory;

            db.SetCommand(
                builderFactory
                    .Select().C(null, "ID", "RoleID")
                    .From("KrPermissionRoles").NoLock()
                    .Where().C("IsContext").Equals().V(true)
                    .OrderBy("ID").By("RoleID") // сортировка не стоит ничего из-за индекса ndx_KrPermissionRoles_IDRoleIDIsContext
                    .Build())
                .LogCommand();

            await using var reader = await db.ExecuteReaderAsync(cancellationToken);
            var prevRuleID = Guid.Empty;
            IKrPermissionRuleSettings prevRule = null;

            while (await reader.ReadAsync(cancellationToken))
            {
                var ruleID = reader.GetValue<Guid>(0);
                if (ruleID != prevRuleID)
                {
                    prevRuleID = ruleID;
                    if (!permissionsByID.TryGetValue(ruleID, out prevRule))
                    {
                        continue;
                    }
                }

                prevRule?.ContextRoles.Add(reader.GetValue<Guid>(1));
            }
        }

        private async Task FillPermissionMainAsync(
            DbManager db,
            Dictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            CancellationToken cancellationToken = default)
        {
            var builder = dbScope.BuilderFactory
                           .Select()
                               .C("p", "ID", "Conditions", "IsExtended", "IsRequired"); // 0 - 3

            foreach (var flag in KrPermissionFlagDescriptors.Full.IncludedPermissions)
            {
                if (flag.IsVirtual)
                {
                    continue;
                }

                builder.C(flag.SqlName); // 4 - ...
            }

            builder
                .From("KrPermissions", "p").NoLock()
                .Where().C("p", "IsDisabled").Equals().V(false);

            db.SetCommand(builder.Build())
                    .LogCommand();

            await using var reader = await db.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var id = reader.GetValue<Guid>(0);
                var permission = new KrPermissionRuleSettings(
                    id,
                    reader.GetValue<string>(1),
                    reader.GetValue<bool>(2),
                    reader.GetValue<bool>(3));

                int i = 4; // Список колонок флагов начинается с 4
                foreach (var flag in KrPermissionFlagDescriptors.Full.IncludedPermissions)
                {
                    if (flag.IsVirtual)
                    {
                        continue;
                    }

                    if (reader.GetValue<bool>(i++))
                    {
                        permission.Flags.Add(flag);
                    }
                }

                permissionsByID[id] = permission;
            }
        }

        private async Task FillPermissionHashAsync<T>(
            DbManager db,
            string section,
            string field,
            IDictionary<Guid, IKrPermissionRuleSettings> permissionsByID,
            Action<IKrPermissionRuleSettings, T> setAction,
            CancellationToken cancellationToken = default)
        {
            db.SetCommand(
                dbScope.BuilderFactory
                    .Select()
                        .C(null, "ID", field)
                    .From(section).NoLock()
                    .OrderBy("ID")
                    .Build())
                    .LogCommand();

            await using var reader = await db.ExecuteReaderAsync(cancellationToken);
            var prevPermissionID = Guid.Empty;
            IKrPermissionRuleSettings permission = null;
            while (await reader.ReadAsync(cancellationToken))
            {
                var permissionID = reader.GetValue<Guid>(0);
                if (permissionID != prevPermissionID)
                {
                    prevPermissionID = permissionID;
                    if (!permissionsByID.TryGetValue(permissionID, out permission))
                    {
                        continue;
                    }
                }

                if (permission != null)
                {
                    var value = reader.GetValue<T>(1);
                    setAction(permission, value);
                }
            }
        }

        #endregion
    }
}
