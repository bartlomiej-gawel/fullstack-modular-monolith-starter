namespace Common.Infrastructure.Endpoints.Browse.Extensions;

internal static class QueryablePaginationExtensions
{
    public static IQueryable<TEntity> WithOffsetPagination<TEntity>(
        this IQueryable<TEntity> queryable,
        int page = 1,
        int pageSize = 10)
    {
        var actualPage = Math.Max(1, page);
        var actualPageSize = Math.Clamp(pageSize, 1, 100);
        var skip = (actualPage - 1) * actualPageSize;

        return queryable
            .Skip(skip)
            .Take(actualPageSize);
    }
}
