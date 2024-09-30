namespace UserService.EventProcessing
{
    public class MessagePayload<T>
    {
        public string Type { get; set; }

        public T Data { get; set; }
    }
}