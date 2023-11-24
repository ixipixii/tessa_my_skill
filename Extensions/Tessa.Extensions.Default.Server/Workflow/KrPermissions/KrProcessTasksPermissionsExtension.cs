using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    public sealed class KrProcessTasksPermissionsExtension : ITaskPermissionsExtension
    {
        #region Nested Types

        private sealed class PermissionsRegex
        {
            #region Constructors

            public PermissionsRegex(string canEditCardField, string canEditFilesField)
            {
                this.CanEditCardField = canEditCardField;
                this.CanEditCardRegex = new Regex($"\"{canEditCardField}\":\\s*true", RegexOptions.Compiled);

                this.CanEditFilesField = canEditFilesField;
                this.CanEditFilesRegex = new Regex($"\"{canEditFilesField}\":\\s*true", RegexOptions.Compiled);
            }

            #endregion

            #region Properties

            public string CanEditCardField { get; }

            public Regex CanEditCardRegex { get; }

            public string CanEditFilesField { get; }

            public Regex CanEditFilesRegex { get; }

            #endregion
        }

        #endregion

        #region Fields

        private readonly IKrProcessContainer processContainer;
        private readonly IKrScope krScope;
        private readonly IKrStageSerializer serializer;

        private static readonly Dictionary<Guid, PermissionsRegex> permissionsByTaskTypes =
            new Dictionary<Guid, PermissionsRegex>
            {
                {
                    DefaultTaskTypes.KrApproveTypeID,
                    new PermissionsRegex(KrApprovalSettingsVirtual.CanEditCard, KrApprovalSettingsVirtual.CanEditFiles)
                },
                {
                    DefaultTaskTypes.KrSigningTypeID,
                    new PermissionsRegex(KrSigningStageSettingsVirtual.CanEditCard, KrSigningStageSettingsVirtual.CanEditFiles)
                },
                {
                    DefaultTaskTypes.KrRegistrationTypeID,
                    new PermissionsRegex(KrRegistrationStageSettingsVirtual.CanEditCard, KrRegistrationStageSettingsVirtual.CanEditFiles)
                },
                {
                    DefaultTaskTypes.KrUniversalTaskTypeID,
                    new PermissionsRegex(KrUniversalTaskSettingsVirtual.CanEditCard, KrUniversalTaskSettingsVirtual.CanEditFiles)
                },
            };

        #endregion

        #region Constructors

        public KrProcessTasksPermissionsExtension(
            IKrProcessContainer processContainer,
            IKrScope krScope,
            IKrStageSerializer serializer)
        {
            this.processContainer = processContainer;
            this.krScope = krScope;
            this.serializer = serializer;
        }

        #endregion

        #region ITaskPermissionsExtension Members

        public async Task ExtendPermissionsAsync(ITaskPermissionsExtensionContext context)
        {
            var taskTypeID = context.TaskType.ID;

            bool supportedTaskType = this.processContainer.IsTaskTypeRegistered(taskTypeID);
            if (!supportedTaskType)
            {
                return;
            }

            var descriptor = context.Descriptor;
            var task = context.Task;

            descriptor.Set(KrPermissionFlagDescriptors.ReadCard, true);
            descriptor.Set(KrPermissionFlagDescriptors.SignFiles, true);

            //Задания процесса согласования дают права только если в работе/отложено и пользователь - исполнитель
            if (!task.IsPerformer
                || task.StoredState != CardTaskState.InProgress
                && task.StoredState != CardTaskState.Postponed
                && taskTypeID != DefaultTaskTypes.KrRequestCommentTypeID)
            {
                return;
            }

            //Задание редактирования, как исполнитель, задание в работе или отложено
            if (taskTypeID == DefaultTaskTypes.KrEditTypeID ||
                taskTypeID == DefaultTaskTypes.KrEditInterjectTypeID)
            {
                descriptor.Set(KrPermissionFlagDescriptors.FullCardPermissionsGroup, true, true);
                return;
            }

            // для всех типов задач в маршрутах, кроме перечисленных выше, даём права на новые файлы, когда они в работе
            descriptor
                .Set(KrPermissionFlagDescriptors.AddFiles, true)
                .Set(KrPermissionFlagDescriptors.EditOwnFiles, true);

            //Задание согласования, как исполнитель, задание в работе или отложено
            if (permissionsByTaskTypes.TryGetValue(taskTypeID, out PermissionsRegex permissions))
            {
                // Задание также может выдавать право редактирования всех файлов и карточки - нужно залезть в карточку и глянуть, что там в этапе указано.
                if (context.Mode == KrPermissionsCheckMode.WithCard)
                {
                    //Проверка на загрузке карточки - можно глянуть в саму карточку в контексте
                    var card = context.Card;
                    var stage = card.Sections[KrStages.Virtual].Rows.FirstOrDefault(x =>
                        x.RowID == card.Sections[KrApprovalCommonInfo.Virtual].Fields.Get<Guid?>(KrProcessCommonInfo.CurrentApprovalStageRowID));

                    if (stage is null)
                    {
                        // Если в основной карточке не найден этап, то он может быть запущен во вторичном процессе.
                        await using (context.DbScope.Create())
                        {
                            DbManager db = context.DbScope.Db;

                            Guid? processRowID = await db
                                .SetCommand(
                                    context.DbScope.BuilderFactory
                                        .Select().C("ProcessRowID")
                                        .From("WorkflowTasks").NoLock()
                                        .Where().C("RowID").Equals().P("TaskRowID")
                                        .Build(),
                                    db.Parameter("TaskRowID", task.RowID))
                                .LogCommand()
                                .ExecuteAsync<Guid?>(context.CancellationToken);

                            if (!processRowID.HasValue)
                            {
                                return;
                            }

                            await using var krScopeContext = KrScopeContext.Create();
                            var satellite = this.krScope.GetSecondaryKrSatellite(processRowID.Value);

                            context.ValidationResult.Add(krScopeContext.Value.ValidationResult);

                            if (!krScopeContext.Value.ValidationResult.IsSuccessful())
                            {
                                return;
                            }

                            if (!satellite.Sections.ContainsKey(KrStages.Virtual))
                            {
                                satellite.Sections.Add(KrStages.Virtual);
                            }

                            await this.serializer.DeserializeSectionsAsync(
                                satellite,
                                satellite,
                                cancellationToken: context.CancellationToken);

                            stage = satellite.Sections[KrStages.Virtual].Rows.FirstOrDefault(x =>
                                x.RowID == satellite.Sections[KrSecondaryProcessCommonInfo.Name].Fields.Get<Guid?>(KrProcessCommonInfo.CurrentApprovalStageRowID));

                            if (stage is null)
                            {
                                return;
                            }
                        }
                    }

                    if (stage.Get<bool>(permissions.CanEditCardField))
                    {
                        descriptor.Set(KrPermissionFlagDescriptors.EditCard, true);
                    }

                    if (stage.Get<bool>(permissions.CanEditFilesField))
                    {
                        descriptor.Set(KrPermissionFlagDescriptors.EditFiles, true);
                    }
                }
                else
                {
                    await using (context.DbScope.Create())
                    {
                        DbManager db = context.DbScope.Db;

                        string settings = await db
                            .SetCommand(
                                context.DbScope.BuilderFactory
                                    .Select().C("ks", "Settings")
                                    .From("KrStages", "ks").NoLock()
                                    .InnerJoin("KrApprovalCommonInfo", "kaci").NoLock()
                                    .On().C("kaci", "CurrentApprovalStageRowID").Equals().C("ks", "RowID")
                                    .Where().C("kaci", "MainCardID").Equals().P("MainCardID")
                                    .Build(),
                                db.Parameter("MainCardID", context.CardID))
                            .LogCommand()
                            .ExecuteAsync<string>();

                        if (!string.IsNullOrEmpty(settings))
                        {
                            descriptor
                                .Set(KrPermissionFlagDescriptors.EditCard, permissions.CanEditCardRegex.IsMatch(settings))
                                .Set(KrPermissionFlagDescriptors.EditFiles, permissions.CanEditFilesRegex.IsMatch(settings));
                        }
                    }
                }
            }
        }

        #endregion
    }
}