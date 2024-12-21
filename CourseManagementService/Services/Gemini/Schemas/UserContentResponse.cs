using Newtonsoft.Json;

namespace CourseManagementService.Services.Gemini.Schemas
{
    public class UserContentResponse
    {
        public ContentClassification Classification { get; set; }

        [JsonProperty("toxicity_level")]
        public ToxicityLevel ToxicLevel { get; set; }
    }

    public enum ContentClassification
    {
        Valid,
        Invalid,
        Spam
    }

    public enum ToxicityLevel
    {
        None,
        Low,
        Medium,
        High
    }
}