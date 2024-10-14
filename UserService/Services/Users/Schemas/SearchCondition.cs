using UserService.Commons.Schemas;

namespace UserService.Services.Users.Schemas
{
    public class SearchCondition : ParamsSearch
    {
        public string SearchInput { get; set; } = string.Empty;

        public bool? IsActive { get; set; }
    }
}