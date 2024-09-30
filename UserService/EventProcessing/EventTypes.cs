namespace UserService.EventProcessing
{
    public static class EventType
    {
        public const string UserCreated = "UserCreatedEvent";
        public const string UserActivated = "UserActivatedEvent";
        public const string UserLastLoginUpdated = "UserLastLoginUpdatedEvent";
    }
}