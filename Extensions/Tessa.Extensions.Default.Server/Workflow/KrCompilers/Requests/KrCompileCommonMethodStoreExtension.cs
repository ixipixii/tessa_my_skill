using System.Linq;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrCompileCommonMethodStoreExtension : KrCompileSourceStoreExtension
    {
        #region fields

        private readonly IKrCompiler compiler;

        #endregion

        #region constructor

        public KrCompileCommonMethodStoreExtension(
            IDbScope dbScope,
            IKrCompiler compiler,
            IKrProcessCache stageCache,
            IKrCompilationCache compileCache,
            IKrCompilationResultStorage compilationResultStorage) : base(dbScope, stageCache, compileCache, compilationResultStorage)
        {
            this.compiler = compiler;
        }

        #endregion
        
        #region protected

        protected override IKrCompilationResult Build(ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;

            var krCompileContext = new KrCompilationContext();
            var methods = this.StageCache.GetAllCommonMethods().Where(p => p.ID != card.ID);
            krCompileContext.CommonMethods.AddRange(methods);
            krCompileContext.CommonMethods.Add(KrCompilersSqlHelper.SelectCommonMethods(this.DbScope, card.ID).FirstOrDefault());

            return this.compiler.Compile(krCompileContext);
        }

        protected override bool SourceChanged(Card card)
        {
            if (card.TryGetKrStageCommonMethodsSection(out var sec))
            {
                return sec.Fields.ContainsKey(KrConstants.Name) || sec.Fields.ContainsKey(KrConstants.KrStageCommonMethods.Source);
            }
            return false;
        }

        protected override bool CardChanged(Card card)
        {
            return card.TryGetKrStageCommonMethodsSection(out _);
        }

        #endregion
    }
}
