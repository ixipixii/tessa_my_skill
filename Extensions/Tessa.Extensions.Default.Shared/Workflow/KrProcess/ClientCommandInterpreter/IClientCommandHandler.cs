namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter
{
    public interface IClientCommandHandler
    {
        /// <summary>
        /// Выполнить обработку клиентской команды.
        /// </summary>
        /// <param name="("></param>
        void Handle(
            IClientCommandHandlerContext context);
    }
}