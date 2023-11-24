using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrCompileStageGroupStoreExtension : KrCompileSourceStoreExtension
    {
        #region fields

        private readonly IKrCompiler compiler;

        #endregion

        #region constructor

        /// <inheritdoc />
        public KrCompileStageGroupStoreExtension(
            IDbScope dbScope,
            IKrProcessCache stageCache,
            IKrCompilationCache compileCache,
            IKrCompiler compiler, 
            IKrCompilationResultStorage compilationResultStorage)
            : base(dbScope, stageCache, compileCache, compilationResultStorage)
        {
            this.compiler = compiler;
        }

        #endregion

        #region base overrides

        /// <inheritdoc />
        protected override IKrCompilationResult Build(
            ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;

            var krCompileContext = new KrCompilationContext();
            krCompileContext.StageGroups.Add(KrCompilersSqlHelper.SelectStageGroups(this.DbScope, card.ID).FirstOrDefault());
            krCompileContext.CommonMethods.AddRange(this.StageCache.GetAllCommonMethods());

            return this.compiler.Compile(krCompileContext);
        }

        /// <inheritdoc />
        protected override bool SourceChanged(
            Card card)
        {
            return card.Sections.TryGetValue(KrConstants.KrStageGroups.Name, out var sec)
                && (sec.Fields.ContainsKey(KrConstants.SourceAfter)
                    || sec.Fields.ContainsKey(KrConstants.SourceBefore)
                    || sec.Fields.ContainsKey(KrConstants.SourceCondition)
                    || sec.Fields.ContainsKey(KrConstants.RuntimeSourceAfter)
                    || sec.Fields.ContainsKey(KrConstants.RuntimeSourceBefore)
                    || sec.Fields.ContainsKey(KrConstants.RuntimeSourceCondition));
        }

        /// <inheritdoc />
        protected override bool CardChanged(
            Card card) => card.TryGetSections()?.Count > 0;

        #endregion
    }
}