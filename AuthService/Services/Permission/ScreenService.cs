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

                var screen = await _context.Screens.FirstOrDefaultAsync(x => x.Id == id);
                if (screen == null)
                {
                    response.Message = "Screen not found";
                    return response;
                }

                _context.Screens.Remove(screen);
                await _context.SaveChangesAsync();

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