using CourseManagementService.Services.Gemini.Settings;

namespace CourseManagementService.Services.Gemini.HttpClientModels
{
    public class GeminiRequestFactory
    {
        public static GeminiRequest CreateRequest(string prompt, string newUserInput)
        {
            var contents = new List<GeminiContent>
            {
                new()
                {
                    Role = "user",
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = prompt
                        }
                    ]
                },
                new()
                {
                    Role = "user",
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = newUserInput
                        }
                    ]
                }
            };

            return new GeminiRequest
            {
                Contents = contents,
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 1,
                    TopK = 40,
                    TopP = 0.95,
                    MaxOutputTokens = 2048,
                    StopSequences = []
                },
                SafetySettings =
                [
                    new() {
                        Category = "HARM_CATEGORY_HARASSMENT",
                        Threshold = "BLOCK_ONLY_HIGH"
                    },
                    new() {
                        Category = "HARM_CATEGORY_HATE_SPEECH",
                        Threshold = "BLOCK_ONLY_HIGH"
                    },
                    new() {
                        Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                        Threshold = "BLOCK_ONLY_HIGH"
                    },
                    new() {
                        Category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                        Threshold = "BLOCK_ONLY_HIGH"
                    }
                ]
            };
        }
    }
}