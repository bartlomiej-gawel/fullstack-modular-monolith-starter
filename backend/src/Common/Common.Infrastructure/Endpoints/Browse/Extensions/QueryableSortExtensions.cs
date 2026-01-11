using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Infrastructure.Endpoints.Browse.Extensions;

internal static class QueryableSortExtensions
{
    private static readonly ConcurrentDictionary<string, PropertyInfo?> PropertyCache = new();

    public static IQueryable<TEntity> WithDynamicSort<TEntity>(
        this IQueryable<TEntity> query,
        string? sortBy,
        bool sortDesc) where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var cacheKey = $"{typeof(TEntity).FullName}.{sortBy}";

        var propertyInfo = PropertyCache.GetOrAdd(cacheKey, _ => GetProperty<TEntity>(sortBy));
        if (propertyInfo == null)
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDesc
            ? "OrderByDescending"
            : "OrderBy";

        var orderByMethod = typeof(Queryable).GetMethods()
            .First(m =>
                m.Name == methodName &&
                m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEntity), propertyInfo.PropertyType);

        return (IQueryable<TEntity>)orderByMethod.Invoke(null, [query, lambda])!;
    }

    private static PropertyInfo? GetProperty<TEntity>(string propertyName)
    {
        return typeof(TEntity).GetProperty(
            propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    }
}
