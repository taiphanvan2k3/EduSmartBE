namespace UserService.Services.Users.Schemas
{
    public class UserProfileUpdateRequest
    {
        public UserUpdateDto UserInfo { get; set; }
        public IFormFile File { get; set; }
    }
}