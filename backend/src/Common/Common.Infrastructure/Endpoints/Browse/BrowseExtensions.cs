using System.Linq.Expressions;
using Common.Abstractions.Mapping;
using Common.Infrastructure.Endpoints.Browse.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Endpoints.Browse;

public static class BrowseExtensions
{
    extension<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        public async Task<BrowseResult<TEntity>> BrowseAsync(
            BrowseModel? browseModel,
            string defaultSortBy,
            Expression<Func<TEntity, string?>>[] searchProperties,
            CancellationToken cancellationToken)
        {
            var model = browseModel ?? new BrowseModel();

            if (!string.IsNullOrWhiteSpace(model.Search) && searchProperties.Length > 0)
                query = query.WithDynamicSearch(
                    model.Search,
                    model.SearchPattern,
                    model.SearchCaseSensitive,
                    searchProperties);

            var total = await query.CountAsync(cancellationToken);
            if (total == 0)
                return BrowseResult<TEntity>.Empty(model.Page, model.PageSize);

            var sortBy = string.IsNullOrWhiteSpace(model.SortBy)
                ? defaultSortBy
                : model.SortBy;

            query = query.WithDynamicSort(sortBy, model.SortDesc);

            var items = await query
                .WithOffsetPagination(model.Page, model.PageSize)
                .ToListAsync(cancellationToken);

            return BrowseResult<TEntity>.Create(
                model.Page,
                model.PageSize,
                total,
                items);
        }

        public Task<BrowseResult<TEntity>> BrowseAsync(
            BrowseModel? browseModel,
            string defaultSortBy,
            params Expression<Func<TEntity, string?>>[] searchProperties)
        {
            return query.BrowseAsync(
                browseModel,
                defaultSortBy,
                searchProperties,
                CancellationToken.None);
        }

        public Task<BrowseResult<TEntity>> BrowseAsync(
            BrowseModel? browseModel,
            string defaultSortBy,
            CancellationToken cancellationToken)
        {
            return query.BrowseAsync(
                browseModel,
                defaultSortBy,
                searchProperties: [],
                cancellationToken);
        }
    }

    public static async Task<BrowseResult<TEntityDto>> MapBrowseResult<TEntity, TEntityDto>(
        this Task<BrowseResult<TEntity>> browseTask)
        where TEntity : class
        where TEntityDto : class, IWithExpressionMapFrom<TEntity, TEntityDto>
    {
        var browseResult = await browseTask;
        return browseResult.MapTo<TEntityDto>();
    }
}
