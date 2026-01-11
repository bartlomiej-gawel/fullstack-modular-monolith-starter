namespace Common.Infrastructure.Endpoints.Browse.Base;

public sealed class Pagination
{
    private Pagination(
        int page,
        int pageSize,
        int total,
        int pageCount)
    {
        Page = page;
        PageSize = pageSize;
        Total = total;
        PageCount = pageCount;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int Total { get; }
    public int PageCount { get; }

    internal static Pagination Create(
        int page,
        int pageSize,
        int total)
    {
        var actualPage = Math.Max(1, page);
        var actualPageSize = Math.Clamp(pageSize, 1, 100);
        var pageCount = (int)Math.Ceiling((double)total / actualPageSize);

        return new Pagination(
            actualPage,
            actualPageSize,
            total,
            pageCount);
    }
}
