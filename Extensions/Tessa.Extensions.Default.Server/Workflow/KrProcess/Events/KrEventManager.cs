using System;
using System.Threading.Tasks;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    public sealed class KrEventManager : IKrEventManager
    {
        #region Fields

        private readonly IExtensionContainer extensionContainer;

        #endregion

        #region Constructors

        public KrEventManager(IExtensionContainer extensionContainer) =>
            this.extensionContainer = extensionContainer ?? throw new ArgumentNullException(nameof(extensionContainer));

        #endregion

        #region IKrEventManager Members

        /// <inheritdoc />
        public async Task RaiseAsync(IKrEventExtensionContext context)
        {
            await using var executor = await this.extensionContainer.ResolveExecutorAsync<IKrEventExtension>(context.CancellationToken);
            await executor.ExecuteAsync(x => x.HandleEvent, context);
        }

        #endregion
    }
}