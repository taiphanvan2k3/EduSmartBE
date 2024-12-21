using CourseManagementService.Services.Gemini.Settings;

namespace CourseManagementService.Services.Gemini.HttpClientModels;

public sealed class GeminiRequest
{
    public List<GeminiContent> Contents { get; set; }
    public GenerationConfig GenerationConfig { get; set; }
    public SafetySetting[] SafetySettings { get; set; }
}

public sealed class GeminiContent
{
    public string Role { get; set; }
    public List<GeminiPart> Parts { get; set; }
}

public sealed class GeminiPart
{
    // This one interests us the most
    public string Text { get; set; }
}