using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.SourceBuilders;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public sealed class ExtraSourcesStageRowExtension: KrStageRowExtension
    {
        private readonly IExtraSourceSerializer extraSourceSerializer;

        public ExtraSourcesStageRowExtension(
            IExtraSourceSerializer extraSourceSerializer)
        {
            this.extraSourceSerializer = extraSourceSerializer;
        }

        /// <inheritdoc />
        public override Task BeforeSerialization(
            IKrStageRowExtensionContext context)
        {
            var rows = context.InnerCard.GetStagesSection().Rows;
            foreach (var row in rows)
            {
                var stageTypeID = row.TryGet<Guid?>(KrConstants.KrStages.StageTypeID);
                if (stageTypeID == StageTypeDescriptors.ForkDescriptor.ID)
                {
                    this.SetSingleSource(
                        context, row, KrConstants.KrForkSettingsVirtual.AfterEachNestedProcess,
                        "$KrStages_Fork_AfterNested", ForkStageTypeHandler.AfterNestedMethodName,
                        ForkStageTypeHandler.ScriptContextParameterType, ForkStageTypeHandler.MethodParameterName);
                }
                else if (stageTypeID == StageTypeDescriptors.TypedTaskDescriptor.ID)
                {
                    this.SetSingleSource(
                        context, row, KrConstants.KrTypedTaskSettingsVirtual.AfterTaskCompletion,
                        "$KrStages_TypedTask_AfterTask", TypedTaskStageTypeHandler.AfterTaskMethodName,
                        TypedTaskStageTypeHandler.ScriptContextParameterType, TypedTaskStageTypeHandler.MethodParameterName);
                }
                else if (stageTypeID == StageTypeDescriptors.NotificationDescriptor.ID)
                {
                    this.SetSingleSource(
                        context, row, KrConstants.KrNotificationSettingVirtual.EmailModificationScript,
                        "$KrStages_Notification_EmailModification", NotificationStageTypeHandler.MethodName,
                        NotificationStageTypeHandler.ScriptContextParameterType, NotificationStageTypeHandler.MethodParameterName);
                }
                else if (stageTypeID == StageTypeDescriptors.DialogDescriptor.ID)
                {
                    var extraSources = this.GetSourcesFromSettings(row);

                    AddSource(
                        extraSources, context, row, KrConstants.KrDialogStageTypeSettingsVirtual.DialogActionScript,
                        "$UI_KrDialog_Script", DialogStageTypeHandler.MethodName,
                        DialogStageTypeHandler.ScriptContextParameterType, DialogStageTypeHandler.MethodParameterName);

                    AddSource(
                        extraSources, context, row, KrConstants.KrDialogStageTypeSettingsVirtual.DialogCardSavingScript,
                        "$UI_KrDialog_SavingScript", DialogStageTypeHandler.SavingMethodName,
                        DialogStageTypeHandler.SavingScriptContextParameterType, DialogStageTypeHandler.SavingMethodParameterName);

                    this.SetSourcesToSettings(row, extraSources);
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task DeserializationBeforeRepair(
            IKrStageRowExtensionContext context)
        {
            var rows = context.CardToRepair.Sections[KrConstants.KrStages.Virtual].Rows;
            foreach (var row in rows)
            {
                var stageTypeID = row.TryGet<Guid?>(KrConstants.KrStages.StageTypeID);
                if (stageTypeID == StageTypeDescriptors.ForkDescriptor.ID)
                {
                    this.MoveSourceToSettings(row, KrConstants.KrForkSettingsVirtual.AfterEachNestedProcess);
                }
                else if (stageTypeID == StageTypeDescriptors.TypedTaskDescriptor.ID)
                {
                    this.MoveSourceToSettings(row, KrConstants.KrTypedTaskSettingsVirtual.AfterTaskCompletion);
                }
                else if (stageTypeID == StageTypeDescriptors.NotificationDescriptor.ID)
                {
                    this.MoveSourceToSettings(row, KrConstants.KrNotificationSettingVirtual.EmailModificationScript);
                }
                else if (stageTypeID == StageTypeDescriptors.DialogDescriptor.ID)
                {
                    this.MoveSourceToSettings(row, KrConstants.KrDialogStageTypeSettingsVirtual.DialogActionScript, DialogStageTypeHandler.MethodName);
                    this.MoveSourceToSettings(row, KrConstants.KrDialogStageTypeSettingsVirtual.DialogCardSavingScript, DialogStageTypeHandler.SavingMethodName);
                }
            }

            return Task.CompletedTask;
        }

        private void SetSingleSource(
            IKrStageRowExtensionContext context,
            CardRow row,
            string scriptField,
            string displayName,
            string name,
            string paremeterType,
            string parameterName,
            string returnType = SourceIdentifiers.Void)
        {
            if (context.StageStorages.TryGetValue(row.RowID, out var settings)
                && SourceChanged(context, row, scriptField))
            {
                var newSrc = settings.TryGet<string>(scriptField);
                var extraSources = new List<IExtraSource>
                {
                    new ExtraSource
                    {
                        DisplayName = displayName,
                        Name = name,
                        ReturnType = returnType,
                        ParameterType = paremeterType,
                        ParameterName = parameterName,
                        Source = newSrc,
                    }
                };

                settings[scriptField] = null;
                this.SetSourcesToSettings(row, extraSources);
            }
        }

        private void SetSourcesToSettings(
            CardRow row,
            IList<IExtraSource> extraSources)
        {
            row.Fields[KrConstants.KrStages.ExtraSources] = this.extraSourceSerializer.Serialize(extraSources);
        }

        private IList<IExtraSource> GetSourcesFromSettings(
            CardRow row)
        {
            return this.extraSourceSerializer.Deserialize((string) row.Fields[KrConstants.KrStages.ExtraSources]);
        }

        private static void AddSource(
            IList<IExtraSource> extraSources,
            IKrStageRowExtensionContext context,
            CardRow row,
            string scriptField,
            string displayName,
            string name,
            string paremeterType,
            string parameterName,
            string returnType = SourceIdentifiers.Void)
        {
            if (context.StageStorages.TryGetValue(row.RowID, out var settings)
                && SourceChanged(context, row, scriptField))
            {
                var newSrc = settings.TryGet<string>(scriptField);
                var source = new ExtraSource
                {
                    DisplayName = displayName,
                    Name = name,
                    ReturnType = returnType,
                    ParameterType = paremeterType,
                    ParameterName = parameterName,
                    Source = newSrc,
                };

                settings[scriptField] = null;
                var index = 0;
                foreach (var item in extraSources)
                {
                    if (item.Name == name)
                    {
                        break;
                    }
                    index++;
                }

                if (index != extraSources.Count)
                {
                    extraSources[index] = source;
                }
                else
                {
                    extraSources.Add(source);
                }
            }
        }

        private static bool SourceChanged(
            IKrStageRowExtensionContext context,
            CardRow row,
            string key)
        {
            return context.OuterCard.StoreMode == CardStoreMode.Insert
                || context.OuterCard.TryGetStagesSection(out var krStages, preferVirtual: true)
                && krStages.Rows.Any(p =>
                    p.RowID == row.RowID && p.ContainsKey(key));
        }

        private void MoveSourceToSettings(CardRow row, string key, string sourceName = null)
        {
            var extraSources = this.extraSourceSerializer.Deserialize(row.Fields[KrConstants.KrStages.ExtraSources] as string);

            IExtraSource source = null;
            if (sourceName == null
                && extraSources.Count > 0)
            {
                source = extraSources[0];
            }
            else if (sourceName != null)
            {
                foreach (var s in extraSources)
                {
                    if (s.Name == sourceName)
                    {
                        source = s;
                        break;
                    }
                }
            }

            if (source != null)
            {
                row[key] = source.Source;
            }
        }
    }
}