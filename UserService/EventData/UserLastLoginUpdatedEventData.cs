namespace UserService.EventData
{
    public class UserLastLoginUpdatedEventData
    {
        public int UserId { get; set; }

        public DateTimeOffset LastLogin { get; set; }
    }
}