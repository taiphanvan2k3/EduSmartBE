namespace UserService.Services.UserInfos.Schemas
{
    public class UserProfileUpdateRequest
    {
         public UserInfoDto UserInfo { get; set; }
         public IFormFile File { get; set; }  
    }
}