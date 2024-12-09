namespace UserService.EventProcessing
{
    public static class EventType
    {
        public const string UserCreated = "UserService.UserCreated";
        public const string ActiveStatusUpdated = "UserService.ActiveStatusUpdated";
        public const string UserLastLoginUpdated = "UserService.UserLastLoginUpdated";
    }
}