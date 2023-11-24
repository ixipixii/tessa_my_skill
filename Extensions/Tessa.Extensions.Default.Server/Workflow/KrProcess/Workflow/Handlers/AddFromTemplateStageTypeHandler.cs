using System;
using System.Collections.Generic;
using System.IO;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Files;
using Tessa.Platform.Data;
using Tessa.Platform.IO;
using Tessa.Platform.Placeholders;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.Handlers
{
    public class AddFromTemplateStageTypeHandler : StageTypeHandlerBase
    {
        protected readonly ICardStreamServerRepository CardStreamRepository;

        protected readonly IPlaceholderManager PlaceholderManager;

        protected readonly IDbScope DbScope;

        protected readonly IUnityContainer UnityContainer;

        protected readonly ISession Session;

        protected readonly IKrScope KrScope;

        public AddFromTemplateStageTypeHandler(
            ICardStreamServerRepository cardStreamRepository,
            IPlaceholderManager placeholderManager,
            IDbScope dbScope,
            ISession session,
            IUnityContainer unityContainer,
            ICardFileManager fileManager,
            IKrScope krScope)
        {
            this.CardStreamRepository = cardStreamRepository;
            this.PlaceholderManager = placeholderManager;
            this.DbScope = dbScope;
            this.Session = session;
            this.UnityContainer = unityContainer;
            this.KrScope = krScope;
        }

        /// <inheritdoc />
        public override StageHandlerResult HandleStageStart(
            IStageTypeHandlerContext context)
        {
            var templateID = context.Stage.SettingsStorage.TryGet<Guid?>(KrConstants.KrAddFromTemplateSettingsVirtual.FileTemplateID);
            var fileName = context.Stage.SettingsStorage.TryGet<string>(KrConstants.KrAddFromTemplateSettingsVirtual.Name);

            if (templateID.HasValue)
            {
                var result = this.CardStreamRepository.GenerateFileFromTemplateAsync(
                    templateID.Value,
                    context.MainCardID).GetAwaiter().GetResult(); // TODO async

                context.ValidationResult.Add(result.Response.ValidationResult);

                if (result.HasContent)
                {
                    var fileContainer = context.MainCardAccessStrategy.GetFileContainer();
                    using var s = result.GetContentOrThrowAsync().GetAwaiter().GetResult();
                    var data = s.ReadAllBytes();

                    fileContainer
                        .FileContainer
                        .BuildFile(this.GetFileName(context, result.Response.TryGetSuggestedFileName(), fileName))
                        .SetContent(data)
                        .AddWithNotificationAsync().GetAwaiter().GetResult(); // TODO async
                }
            }

            return StageHandlerResult.CompleteResult;
        }

        #region protected

        protected string GetFileName(
            IStageTypeHandlerContext context,
            string suggestedName,
            string fileNameTemplate)
        {
            if (string.IsNullOrWhiteSpace(fileNameTemplate))
            {
                return suggestedName;
            }

            var extension = Path.GetExtension(suggestedName);

            return this.ExtendFileName(context, fileNameTemplate) + extension;
        }

        protected string ExtendFileName(IStageTypeHandlerContext context, string fileNameTemplate) =>
            this.PlaceholderManager.ReplaceTextAsync(
                fileNameTemplate,
                this.Session,
                this.UnityContainer,
                this.DbScope,
                null,
                context.MainCardAccessStrategy.GetCard(),
                info: CreatePlaceholderInfo(context)).GetAwaiter().GetResult(); // TODO async

        protected static Dictionary<string, object> CreatePlaceholderInfo(IStageTypeHandlerContext context) =>
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [PlaceholderHelper.TaskKey] = context.TaskInfo?.Task,
                ["WorkflowProcess"] = context.WorkflowProcess,
                ["Stage"] = context.Stage,
            };

        #endregion

    }
}