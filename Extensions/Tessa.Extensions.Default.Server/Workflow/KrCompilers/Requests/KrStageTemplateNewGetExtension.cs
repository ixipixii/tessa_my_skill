using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.Requests
{
    public sealed class KrStageTemplateNewGetExtension : KrTemplateNewGetExtension
    {
        /// <inheritdoc />
        public KrStageTemplateNewGetExtension(
            IKrStageSerializer serializer)
            : base(serializer)
        {
        }
    }
}