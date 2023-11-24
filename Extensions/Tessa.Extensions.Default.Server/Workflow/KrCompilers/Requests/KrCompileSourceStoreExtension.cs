using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Localization;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public abstract class KrCompileSourceStoreExtension : CardStoreExtension
    {
        #region fields

        protected readonly IDbScope DbScope;
        protected readonly IKrProcessCache StageCache;
        protected readonly IKrCompilationCache CompileCache;
        protected readonly IKrCompilationResultStorage CompilationResultStorage;

        #endregion

        #region constructor

        protected KrCompileSourceStoreExtension(
            IDbScope dbScope,
            IKrProcessCache stageCache,
            IKrCompilationCache compileCache,
            IKrCompilationResultStorage compilationResultStorage)
        {
            this.DbScope = dbScope;
            this.StageCache = stageCache;
            this.CompileCache = compileCache;
            this.CompilationResultStorage = compilationResultStorage;
        }

        #endregion

        #region private

        private void SetLastBuildOutput(ICardStoreExtensionContext context, IKrCompilationResult result)
        {
            this.CompilationResultStorage.Upsert(
                 context.Request.Card.ID,
                 result);
        }

        private void FillValidationResult(ICardStoreExtensionContext context, IKrCompilationResult result)
        {
            context.ValidationResult.AddInfo(
                this,
                result.Result.Assembly != null ?
                    LocalizationManager.GetString("KrMessages_KrStageSourceSuccessfulBuild"):
                    LocalizationManager.GetString("KrMessages_KrStageSourceFailedBuild"));
            context.ValidationResult.Add(result.ValidationResult);
        }

        private IKrCompilationResult RebuildAll(ICardStoreExtensionContext context)
        {
            this.CompileCache.Invalidate();
            this.StageCache.Invalidate();
            return this.CompileCache.Rebuild();
        }

        protected virtual bool CanBuild(
            ICardStoreExtensionContext context) => true;

        protected abstract IKrCompilationResult Build(ICardStoreExtensionContext context);

        protected abstract bool SourceChanged(Card card);

        protected abstract bool CardChanged(Card card);

        #endregion

        #region base overrides

        public override async Task AfterRequest(ICardStoreExtensionContext context)
        {
            if (!context.ValidationResult.IsSuccessful())
            {
                return;
            }

            if (context.Request.Info.ContainsKey(KrConstants.Keys.Compile)
                && this.CanBuild(context))
            {
                var result = this.Build(context);
                this.SetLastBuildOutput(context, result);
            }
            else if (context.Request.Info.ContainsKey(KrConstants.Keys.CompileWithValidationResult)
                && this.CanBuild(context))
            {
                var result = this.Build(context);
                this.SetLastBuildOutput(context, result);
                this.FillValidationResult(context, result);
            }
            else if (context.Request.Info.ContainsKey(KrConstants.Keys.CompileAll)
                && this.CanBuild(context))
            {
                this.RebuildAll(context);
            }
            else if (context.Request.Info.ContainsKey(KrConstants.Keys.CompileAllWithValidationResult)
                && this.CanBuild(context))
            {
                var result = this.RebuildAll(context);
                this.FillValidationResult(context, result);
            }
            else if (context.Request.Card.StoreMode == CardStoreMode.Insert
                    || this.SourceChanged(context.Request.Card))
            {
                this.StageCache.Invalidate();
                this.CompileCache.Invalidate();
            }
            else if (context.Request.Card.StoreMode == CardStoreMode.Insert
                || this.CardChanged(context.Request.Card))
            {
                this.StageCache.Invalidate();
            }
        }

        #endregion
    }
}
