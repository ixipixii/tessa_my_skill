using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    /// <summary>
    /// Расширение выполняет заполнение виртуальных секций результата
    /// компиляции для карточек KrStageTemplates и KrCommonMethod
    /// </summary>
    public sealed class KrSourceGetExtension: CardGetExtension
    {
        #region fields

        private readonly IKrCompilationResultStorage compilationResultStorage;

        #endregion

        #region constructors

        public KrSourceGetExtension(
            IKrCompilationResultStorage compilationResultStorage)
        {
            this.compilationResultStorage = compilationResultStorage;
        }

        #endregion

        #region private

        private void FillCompilerOutput(Card card)
        {
            var output = this.compilationResultStorage.GetCompilationOutput(card.ID);
            card.Sections[KrConstants.KrStageBuildOutputVirtual.Name].RawFields[KrConstants.KrStageBuildOutputVirtual.LocalBuildOutput] = output.Local;
            card.Sections[KrConstants.KrStageBuildOutputVirtual.Name].RawFields[KrConstants.KrStageBuildOutputVirtual.GlobalBuildOutput] = output.Global;
        }

        #endregion

        #region base overrides

        public override Task AfterRequest(ICardGetExtensionContext context)
        {
            var card = context.Response.Card;
            this.FillCompilerOutput(card);

            return Task.CompletedTask;
        }

        #endregion
    }
}
