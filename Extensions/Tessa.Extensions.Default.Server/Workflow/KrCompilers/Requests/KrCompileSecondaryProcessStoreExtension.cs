using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrCompileSecondaryProcessStoreExtension: KrCompileSourceStoreExtension
    {
        private readonly IKrCompiler compiler;

        private readonly IKrStageSerializer stageSerializer;

        private readonly IExtraSourceSerializer extraSourceSerializer;
        
        /// <inheritdoc />
        public KrCompileSecondaryProcessStoreExtension(
            IDbScope dbScope,
            IKrProcessCache stageCache,
            IKrCompilationCache compileCache,
            IKrCompiler compiler, 
            IKrCompilationResultStorage compilationResultStorage,
            IKrStageSerializer stageSerializer,
            IExtraSourceSerializer extraSourceSerializer)
            : base(dbScope, stageCache, compileCache, compilationResultStorage)
        {
            this.compiler = compiler;
            this.stageSerializer = stageSerializer;
            this.extraSourceSerializer = extraSourceSerializer;
        }

        /// <inheritdoc />
        protected override IKrCompilationResult Build(
            ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;

            var krCompileContext = new KrCompilationContext();
            KrCompilersSqlHelper.SelectKrSecondaryProcesses(
                this.DbScope, 
                card.ID, 
                out var pure,
                out var act, 
                out var buttons);
            krCompileContext.SecondaryProcesses.AddRange(pure);
            krCompileContext.SecondaryProcesses.AddRange(act);
            krCompileContext.SecondaryProcesses.AddRange(buttons);
            krCompileContext.Stages.AddRange(
                KrCompilersSqlHelper.SelectSecondaryProcessRuntimeStages(
                    this.DbScope,
                    this.stageSerializer,
                    this.extraSourceSerializer,
                    card.ID));
            krCompileContext.CommonMethods.AddRange(this.StageCache.GetAllCommonMethods());

            return this.compiler.Compile(krCompileContext);
        }

        protected override bool SourceChanged(Card card)
        {
            return StageScriptsChanged(card) 
                || RuntimeScriptsChanged(card)
                || card.Info.TryGet<bool?>(KrConstants.Keys.ExtraSourcesChanged) == true;
        }

        /// <inheritdoc />
        protected override bool CardChanged(
            Card card) => card.TryGetSections()?.Count > 0;
        
        
        private static bool StageScriptsChanged(Card card)
        {
            return card.Sections.TryGetValue(KrConstants.KrSecondaryProcesses.Name, out var sec)
                && (sec.Fields.ContainsKey(KrConstants.KrSecondaryProcesses.VisibilitySourceCondition)
                    || sec.Fields.ContainsKey(KrConstants.KrSecondaryProcesses.ExecutionSourceCondition))
                || card.TryGetKrStageTemplatesSection(out var krStageTemplateSec)
                && (krStageTemplateSec.Fields.ContainsKey(KrConstants.SourceCondition)
                    || krStageTemplateSec.Fields.ContainsKey(KrConstants.SourceBefore)
                    || krStageTemplateSec.Fields.ContainsKey(KrConstants.SourceAfter));
        }

        private static bool RuntimeScriptsChanged(Card card)
        {
            return card.Sections.TryGetValue(KrConstants.KrStages.Virtual, out var sec)
                && sec.Rows.Any(p =>
                    p.State == CardRowState.Deleted
                    || p.State == CardRowState.Inserted
                    || p.ContainsKey(KrConstants.RuntimeSourceAfter)
                    || p.ContainsKey(KrConstants.RuntimeSourceBefore)
                    || p.ContainsKey(KrConstants.RuntimeSourceCondition));
        }
    }
}