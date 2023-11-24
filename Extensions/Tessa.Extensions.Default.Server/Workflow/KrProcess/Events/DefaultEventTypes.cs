namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Events
{
    public class DefaultEventTypes
    {
        public const string RegistrationEvent = nameof(RegistrationEvent);
        public const string DeregistrationEvent = nameof(DeregistrationEvent);

        public const string NewCard = nameof(NewCard);
        public const string BeforeStoreCard = nameof(BeforeStoreCard);
        public const string StoreCard = nameof(StoreCard);
        public const string BeforeCompleteTask = nameof(BeforeCompleteTask);
        public const string CompleteTask = nameof(CompleteTask);
        public const string BeforeNewTask = nameof(BeforeNewTask);
        public const string NewTask = nameof(NewTask);
        public const string SyncProcessCompleted = nameof(SyncProcessCompleted);
        public const string AsyncProcessCompleted = nameof(AsyncProcessCompleted);
    }
}