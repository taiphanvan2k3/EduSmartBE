using AuthService.Commons;
using AuthService.Services.Permission.Schemas.Screen;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TblScreen = AuthService.Databases.Schemas.Screen;

namespace AuthService.Services.Permission
{
    public interface IScreenService
    {
        /// <summary>
        /// Get screen by Id
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 09/11/2024</para>
        /// </summary>
        /// <param name="Id">Id of screen</param>s
        /// <returns></returns>
        public Task<ScreenDto> GetScreenById(string Id);

        /// <summary>
        /// Get next screen Id
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 09/1/2024</para>
        /// </summary>
        public Task<ResponseInfo> GetNextScreenInfo();

        /// <summary>
        /// Get list of screens
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 05/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<List<ScreenDto>> GetListScreen();

        /// <summary>
        /// Create a new screen
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 05/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CreateScreen(ScreenCreateDto screen);

        /// <summary>
        /// Update a screen
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateScreen(string id, ScreenUpdateDto screen);

        /// <summary>
        /// Delete a screen
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteScreen(string id);
    }

    public class ScreenService(IServiceProvider serviceProvider, IMapper mapper, ILogger<ScreenService> logger)
        : BaseService(serviceProvider, logger), IScreenService
    {
        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> GetNextScreenInfo()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var lastScreen = await _context.Screens
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new
                    {
                        x.Id,
                        x.Order
                    })
                    .FirstOrDefaultAsync();

                var nextId = "SCR_0001";
                var nextOrder = 1;

                if (lastScreen != null)
                {
                    var lastId = lastScreen.Id.Split("_")[1];
                    nextId = $"SCR_{int.Parse(lastId) + 1:D4}";
                    nextOrder = lastScreen.Order + 1;
                }

                responseInfo.Data.Add("nextScreen", new
                {
                    Id = nextId,
                    Order = nextOrder
                });

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ScreenDto> GetScreenById(string Id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                var screen = await _context.Screens
                    .Where(x => x.Id == Id)
                    .Select(x => new ScreenDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        Order = x.Order
                    })
                    .FirstOrDefaultAsync();

                LogInfo("End", methodName);
                return screen;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<List<ScreenDto>> GetListScreen()
        {
            try
            {
                _logger.LogInformation("[ScreenService][GetListScreen] Start");
                var screens = await _context.Screens
                    .AsNoTracking()
                    .OrderBy(x => x.Order)
                    .Select(x => new ScreenDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        Order = x.Order
                    })
                    .ToListAsync();

                _logger.LogInformation("[ScreenService][GetListScreen] End");
                return screens;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ScreenService][GetListScreen][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateScreen(ScreenCreateDto screen)
        {
            var response = new ResponseInfo();
            try
            {
                _logger.LogInformation("[ScreenService][CreateScreen] Start");
                var screenEntity = _mapper.Map<TblScreen>(screen);
                await _context.Screens.AddAsync(screenEntity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[ScreenService][CreateScreen] End");

                response.Data.Add("result", screenEntity);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ScreenService][CreateScreen][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateScreen(string id, ScreenUpdateDto screen)
        {
            try
            {
                var response = new ResponseInfo();
                _logger.LogInformation("[ScreenService][UpdateScreen] Start");

                var screenEntity = _mapper.Map<TblScreen>(screen);
                var existedScreen = await _context.Screens.FirstOrDefaultAsync(x => x.Id == id);
                if (existedScreen == null)
                {
                    response.Message = "Screen not found";
                    return response;
                }

                existedScreen.Name = screenEntity.Name;
                existedScreen.Code = screenEntity.Code;
                existedScreen.Order = screenEntity.Order;

                await _context.SaveChangesAsync();

                _logger.LogInformation("[ScreenService][UpdateScreen] End");

                response.Data.Add("result", existedScreen);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ScreenService][CreateScreen][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteScreen(string id)
        {
            try
            {
                var response = new ResponseInfo();
                _logger.LogInformation("[ScreenService][DeleteScreen] Start");

                var isExistFunctions = await _context.Functions
                    .AnyAsync(x => x.ScreenId == id);

                if (isExistFunctions)
                {
                    response.Error = "BadRequest";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Screen is being used";
                    return response;
                }

                int totalAffectedRecords = await _context.Screens.Where(x => x.Id == id).ExecuteDeleteAsync();
                if (totalAffectedRecords == 0)
                {
                    response.Error = "NotFound";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Screen not found";
                    return response;
                }

                response.Data.Add("id", id);
                _logger.LogInformation("[ScreenService][DeleteScreen] End");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ScreenService][CreateScreen][{Error}]", e.Message);
                throw;
            }
        }
    }
}