using CourseManagementService.Services.Gemini.HttpClientModels;

namespace CourseManagementService.Services.Gemini.Prompts
{
    public static class ContentValidationPrompt
    {
        public const string Prompt =
            "Our community values respect and positivity. Please ensure your comments adhere to our Community Guidelines and avoid containing any illegal, abusive, or offensive content.\n\n" +
            "When a user submits a comment:\n\n" +
            "If the comment violates our guidelines (e.g., contains offensive language, hate speech, or illegal content), classify it as Invalid.\n" +
            "If the comment complies with the guidelines, classify it as Valid.\n" +
            "Additionally, provide a toxicity level for all comments based on their language and tone:\n\n" +
            "Low: Mildly negative or dismissive remarks.\n" +
            "Medium: Moderately offensive or inappropriate content.\n" +
            "High: Explicitly harmful, hateful, or threatening language.\n\n" +
            "If the comment appears to be spam (e.g., contains random characters or has no meaningful content), classify it as Spam.\n\n" +
            "No explain. Response for me an object. If classification is valid, toxicity_level is empty. ### Rules for the response: 1. Only respond in JSON format like this:\n" +
            "```json\n{'classification': 'Valid', 'toxicity_level': 'None'}\n```";

        public static List<GeminiContent> HistorySessions
        {
            get
            {
                return
                [
                    new()
                    {
                        Role = "user",
                        Parts =
                        [
                            new()
                            {
                                Text = "Bài giảng không ổn như mong đợi"
                            }
                        ]
                    }
                ];
            }
        }
    }
}