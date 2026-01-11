using System.Linq.Expressions;

namespace Common.Infrastructure.Database;

public static class QueryableExtensions
{
    extension<TEntity>(IQueryable<TEntity> query)
    {
        public IQueryable<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> predicate)
            => condition
                ? query.Where(predicate)
                : query;

        public async Task<IQueryable<TEntity>> WhereIf(Task<bool> condition, Expression<Func<TEntity, bool>> predicate)
            => await condition
                ? query.Where(predicate)
                : query;
    }
}
