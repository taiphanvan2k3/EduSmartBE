using AutoMapper;
using UserService.Commons;
using UserService.Services.UserInfos.Schemas;

namespace UserService.Services.UserInfos
{
    public interface IUserInfoService
    {
        /// <summary>
        /// Update profile user
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateProfileUser(int userId, UserInfoDto userInfo, IFormFile file);
    }

    public class UserInfoService(IServiceProvider serviceProvider,
        ILogger<UserInfoService> logger,
        IMapper mapper,
        IPhotoService photoService) : BaseService(serviceProvider, logger), IUserInfoService
    {
        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IPhotoService _photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));
        

        public async Task<ResponseInfo> UpdateProfileUser(int userId, UserInfoDto userInfo, IFormFile file)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[UserInfoService] [{Method}] Start", methodName);
                var response = new ResponseInfo();
                var userInfoDB = await _context.UserInfos.FindAsync(userId);
                if(userInfoDB == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "User not found";
                    _logger.LogInformation("[UserInfoService] [{Method}] End", methodName);
                    return response;
                }

                var result = await _photoService.AddPhotoAsync(file);
                userInfoDB.FirstName = userInfo.FirstName;
                userInfoDB.LastName = userInfo.LastName;
                userInfoDB.AvatarURL = result.Url.ToString();
                userInfoDB.Phone = userInfo.Phone;
                userInfoDB.Gender = userInfo.Gender;

                await _context.SaveChangesAsync();

                response.Message = "Update profile user successfully";
                response.Data.Add("userInfo", userInfoDB);
                _logger.LogInformation("[UserInfoService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[UserInfoService] [{Method}] Error", methodName);
                throw;
            }
        }
    }   
}