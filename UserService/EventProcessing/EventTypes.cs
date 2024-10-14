namespace UserService.EventProcessing
{
    public static class EventType
    {
        public const string UserCreated = "UserCreatedEvent";
        public const string ActiveStatusUpdated = "ActiveStatusUpdated";
        public const string UserLastLoginUpdated = "UserLastLoginUpdatedEvent";
    }
}