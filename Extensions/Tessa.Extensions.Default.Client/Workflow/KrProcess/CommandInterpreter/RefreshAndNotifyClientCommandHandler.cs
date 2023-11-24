using Tessa.Extensions.Default.Client.Notifications;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter;

namespace Tessa.Extensions.Default.Client.Workflow.KrProcess.CommandInterpreter
{
    public sealed class RefreshAndNotifyClientCommandHandler : ClientCommandHandlerBase
    {
        private readonly IKrNotificationManager notificationManager;

        public RefreshAndNotifyClientCommandHandler(IKrNotificationManager notificationManager)
        {
            // В клиентских командах можно получать любые IoC-зависимости
            this.notificationManager = notificationManager;
        }

        public override async void Handle(
            IClientCommandHandlerContext context)
        {
            if (await this.notificationManager.CanCheckTasksAsync())
            {
                var _ = this.notificationManager.CheckTasksAsync();
            }
        }
    }
}