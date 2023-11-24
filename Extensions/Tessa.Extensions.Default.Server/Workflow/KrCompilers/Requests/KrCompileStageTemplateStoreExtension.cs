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
    /// <summary>
    /// Расширение на сохранение карточки KrStageTemplates
    /// Выполняет компиляцию при наличии соответствующих флагов в Info
    /// При изменении исходных кодов сбрасывается кэш компиляции и кэш этапов
    /// При изменении данных, не относящихся к компиляции, сбрасывается кэш этапов
    /// </summary>
    public sealed class KrCompileStageTemplateStoreExtension : KrCompileSourceStoreExtension
    {
        #region fields

        private readonly IKrCompiler compiler;
        private readonly IExtraSourceSerializer extraSourceSerializer;
        private readonly IKrStageSerializer stageSerializer;

        #endregion

        #region constructor

        public KrCompileStageTemplateStoreExtension(
            IDbScope dbScope,
            IKrCompiler compiler,
            IKrProcessCache stageCache,
            IKrCompilationCache compileCache,
            IKrCompilationResultStorage compilationResultStorage,
            IExtraSourceSerializer extraSourceSerializer,
            IKrStageSerializer stageSerializer) 
            :base(dbScope, stageCache, compileCache, compilationResultStorage)
        {
            this.compiler = compiler;
            this.extraSourceSerializer = extraSourceSerializer;
            this.stageSerializer = stageSerializer;
        }

        #endregion
        
        #region protected

        protected override IKrCompilationResult Build(ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;
            
            var krCompileContext = new KrCompilationContext();
            krCompileContext.StageTemplates.Add(KrCompilersSqlHelper.SelectStageTemplates(this.DbScope, card.ID).FirstOrDefault());
            krCompileContext.Stages.AddRange(
                KrCompilersSqlHelper.SelectRuntimeStages(this.DbScope, this.stageSerializer, this.extraSourceSerializer, card.ID));
            krCompileContext.CommonMethods.AddRange(this.StageCache.GetAllCommonMethods());

            return this.compiler.Compile(krCompileContext);
        }

        protected override bool SourceChanged(Card card)
        {
            return StageScriptsChanged(card) 
                || RuntimeScriptsChanged(card)
                || card.Info.TryGet<bool?>(KrConstants.Keys.ExtraSourcesChanged) == true;
        }

        protected override bool CardChanged(Card card)
        {
            return card.TryGetSections()?.Count > 0;
        }

        #endregion

        #region private

        private static bool StageScriptsChanged(Card card)
        {
            return card.TryGetKrStageTemplatesSection(out var krStageTemplateSec)
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

        #endregion
    }
}
