namespace CourseManagementService.Services.Gemini.Settings
{
    public class GenerationConfig
    {
        public int Temperature { get; set; }

        public double TopK { get; set; }

        public double TopP { get; set; }

        public int MaxOutputTokens { get; set; }

        public List<object> StopSequences { get; set; }
    }
}