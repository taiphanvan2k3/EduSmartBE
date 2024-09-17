namespace AuthService.Commons
{
    public class BackgroundJobData
    {
        public string JobType { get; set; }

        public Dictionary<string, dynamic> Data { get; set; }
    }
}