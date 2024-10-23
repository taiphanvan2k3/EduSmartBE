using System.Linq.Expressions;
using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> SingleSort<T>(this IQueryable<T> source, ParamsSearchWithSingleSort singleSort)
        {
            try
            {
                var expression = source.Expression;
                var parameter = Expression.Parameter(typeof(T), "x");

                // Example: x.Property1.Property2.Property3
                Expression selector = parameter;
                foreach (var propertyPath in singleSort.OrderBy.Split('.'))
                {
                    selector = Expression.PropertyOrField(selector, propertyPath);
                }

                var sortMethod = string.Equals(singleSort.OrderDirection, "ASC", StringComparison.OrdinalIgnoreCase)
                    ? "OrderBy"
                    : "OrderByDescending";

                // Create a property selector, e.g. x => x.Property1.Property2.Property3
                var lambda = Expression.Lambda(selector, parameter);
                expression = Expression.Call(
                    typeof(Queryable),
                    sortMethod,
                    [source.ElementType, selector.Type],
                    expression,
                    Expression.Quote(lambda)
                );

                return source.Provider.CreateQuery<T>(expression);
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
                return source;
            }
        }

        public static IQueryable<T> MultiSort<T>(this IQueryable<T> source, ParamsSearchWithMultiSort multiSort)
        {
            try
            {
                var sortParamIndex = 0;
                var expression = source.Expression;
                foreach (var sortBy in multiSort.OrderByList)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");

                    // Create a property selector, maybe use navigation property
                    // selector example: x => x.Property1.Property2.Property3
                    Expression selector = parameter;
                    foreach (var propertyPath in sortBy.Split('.'))
                    {
                        selector = Expression.PropertyOrField(selector, propertyPath);
                    }

                    var sortMethod = string.Equals(multiSort.SortDirectionsList[sortParamIndex], "ASC",
                        StringComparison.OrdinalIgnoreCase)
                            ? (sortParamIndex == 0 ? "OrderBy" : "ThenBy")
                            : (sortParamIndex == 0 ? "OrderByDescending" : "ThenByDescending");

                    var lambda = Expression.Lambda(selector, parameter);

                    // expression
                    expression = Expression.Call(
                        typeof(Queryable),
                        sortMethod,
                        [source.ElementType, selector.Type],
                        expression,
                        Expression.Quote(lambda)
                    );
                    sortParamIndex++;
                }

                return sortParamIndex == 0 ? source : source.Provider.CreateQuery<T>(expression);
            }
            catch
            {
                return source;
            }
        }
    }
}