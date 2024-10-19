using Microsoft.EntityFrameworkCore;
using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int currentPage, int pageSize)
        {
            var items = await source
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await source.CountAsync();

            return new PaginatedList<T>()
            {
                Items = items,
                Paging = new Paging(currentPage, pageSize, totalItems)
            };
        }
    }
}