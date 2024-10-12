using UserService.Commons.Schemas;

namespace UserService.Services.Users.Schemas
{
    public class SearchCondition : ParamsSearch
    {
        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}