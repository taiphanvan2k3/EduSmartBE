using AuthService.Commons;
using AuthService.Services.Permission.Schemas.Function;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TblFunction = AuthService.Databases.Schemas.Function;

namespace AuthService.Services.Permission
{
    public interface IFunctionService
    {
        /// <summary>
        /// Get list of functions
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <param name="pagingInfo"></param>
        /// <returns></returns>
        public Task<ResponseInfo> GetListOfFunctions(PagingInfo pagingInfo);

        /// <summary>
        /// Create a new function
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <param name="functionCreateDto"></param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateFunction(FunctionCreateDto functionCreateDto);

        /// <summary>
        /// Update a function
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="functionUpdateDto"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateFunction(string id, FunctionUpdateDto functionUpdateDto);

        /// <summary>
        /// Delete a function
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 07/09/2024</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteFunction(string id);
    }

    public class FunctionService(IServiceProvider serviceProvider, IMapper mapper, ILogger<FunctionService> logger)
        : BaseService(serviceProvider, logger), IFunctionService
    {
        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> GetListOfFunctions(PagingInfo pagingInfo)
        {
            try
            {
                _logger.LogInformation("[FunctionService][GetListOfFunctions] Start");
                var responseInfo = new ResponseInfo();
                var query = _context.Functions.AsNoTracking()
                    .OrderBy(x => x.Screen.Order)
                    .ThenBy(x => x.Order)
                    .Select(x => new FunctionDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        ScreenId = x.ScreenId
                    });

                int totalRow = await query.CountAsync();
                var functions = await query
                    .Skip((pagingInfo.PageIndex - 1) * pagingInfo.PageSize)
                    .Take(pagingInfo.PageSize)
                    .ToListAsync();

                responseInfo.Data.Add("result", new
                {
                    totalRow,
                    functions,
                    pagingInfo.PageIndex,
                });

                _logger.LogInformation("[FunctionService][GetListOfFunctions] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[FunctionService][GetListOfFunctions][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateFunction(FunctionCreateDto functionCreateDto)
        {
            try
            {
                _logger.LogInformation("[FunctionService][CreateFunction] Start");
                var responseInfo = new ResponseInfo();

                if (await _context.Screens.AsNoTracking().AnyAsync(x => x.Id == functionCreateDto.ScreenId) == false)
                {
                    responseInfo.Message = "Screen not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                var newFunction = _mapper.Map<TblFunction>(functionCreateDto);

                await _context.Functions.AddAsync(newFunction);
                await _context.SaveChangesAsync();

                responseInfo.Data.Add("result", newFunction);
                _logger.LogInformation("[FunctionService][CreateFunction] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[FunctionService][CreateFunction][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateFunction(string id, FunctionUpdateDto functionUpdateDto)
        {
            try
            {
                var responseInfo = new ResponseInfo();
                _logger.LogInformation("[FunctionService][UpdateFunction] Start");

                var existedFunction = await _context.Functions.FirstOrDefaultAsync(x => x.Id == id);
                if (existedFunction == null)
                {
                    responseInfo.Message = "Function not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                if (await _context.Screens.AsNoTracking().AnyAsync(x => x.Id == functionUpdateDto.ScreenId) == false)
                {
                    responseInfo.Message = "Screen not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                existedFunction.Name = functionUpdateDto.Name;
                existedFunction.Code = functionUpdateDto.Code;
                existedFunction.ScreenId = functionUpdateDto.ScreenId;

                await _context.SaveChangesAsync();
                _logger.LogInformation("[FunctionService][UpdateFunction] End");

                responseInfo.Data.Add("result", existedFunction);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[FunctionService][UpdateFunction][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteFunction(string id)
        {
            try
            {
                var responseInfo = new ResponseInfo();
                _logger.LogInformation("[FunctionService][DeleteFunction] Start");

                var existedFunction = await _context.Functions.FirstOrDefaultAsync(x => x.Id == id);
                if (existedFunction == null)
                {
                    responseInfo.Message = "Function not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                _context.Functions.Remove(existedFunction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[FunctionService][DeleteFunction] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[FunctionService][DeleteFunction][{Error}]", e.Message);
                throw;
            }
        }
    }
}