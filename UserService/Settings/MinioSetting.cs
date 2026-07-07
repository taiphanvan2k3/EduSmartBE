namespace UserService.Settings
{
    public class MinioSetting
    {
        public string Host { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string Bucket { get; set; }

        public bool SSL { get; set; } = true;
    }
}
