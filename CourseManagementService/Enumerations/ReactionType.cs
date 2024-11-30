using System.Text.Json.Serialization;

namespace CourseManagementService.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReactionType
    {
        Like = 1,
        Love = 2,
        Crush = 3,
        Haha = 4,
        Wow = 5,
        Sad = 6,
        Angry = 7
    }
}