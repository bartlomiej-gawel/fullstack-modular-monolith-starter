using Common.Abstractions.Mapping;
using Common.Infrastructure.Endpoints.Browse.Base;

namespace Common.Infrastructure.Endpoints.Browse;

public sealed record BrowseResult<TItem>
    where TItem : class
{
    private BrowseResult(
        Pagination pagination,
        IReadOnlyList<TItem> data)
    {
        Pagination = pagination;
        Data = data;
    }

    public Pagination Pagination { get; }
    public IReadOnlyList<TItem> Data { get; }

    internal static BrowseResult<TItem> Create(
        int page,
        int pageSize,
        int total,
        List<TItem> data)
    {
        var pagination = Pagination.Create(
            page,
            pageSize,
            total);

        return new BrowseResult<TItem>(
            pagination,
            data);
    }

    internal static BrowseResult<TItem> Empty(
        int page = 1,
        int pageSize = 10)
    {
        return new BrowseResult<TItem>(Pagination.Create(
                page,
                pageSize,
                total: 0),
            data: []);
    }

    internal BrowseResult<TDestination> MapTo<TDestination>()
        where TDestination : class, IWithExpressionMapFrom<TItem, TDestination>
    {
        var mapper = TDestination.MapExpression.Compile();
        var mappedData = Data
            .Select(mapper)
            .ToList();

        return new BrowseResult<TDestination>(
            Pagination,
            mappedData);
    }
}
