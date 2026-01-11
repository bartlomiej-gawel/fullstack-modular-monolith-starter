using System.Linq.Expressions;
using System.Reflection;
using Common.Infrastructure.Endpoints.Browse.Base;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Endpoints.Browse.Extensions;

internal static class QueryableSearchExtensions
{
    private static readonly MethodInfo? PostgresILikeMethod;
    private static readonly MethodInfo LikeMethod;
    private static readonly MethodInfo ToLowerMethod;

    static QueryableSearchExtensions()
    {
        var npgsqlExtensions = Type.GetType(
            "Microsoft.EntityFrameworkCore.NpgsqlDbFunctionsExtensions, Npgsql.EntityFrameworkCore.PostgreSQL");

        PostgresILikeMethod = npgsqlExtensions?.GetMethod(
            "ILike",
            [typeof(DbFunctions), typeof(string), typeof(string)]);

        LikeMethod = typeof(DbFunctionsExtensions).GetMethod(
            nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

        ToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
    }

    public static IQueryable<TEntity> WithDynamicSearch<TEntity>(
        this IQueryable<TEntity> query,
        string? searchValue,
        SearchPattern searchPattern,
        bool caseSensitive = false,
        params Expression<Func<TEntity, string?>>[] propertySelectors) where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(searchValue) || propertySelectors.Length == 0)
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "e");

        Expression? combinedPredicate = null;

        foreach (var selector in propertySelectors)
        {
            var property = ReplaceParameter(selector.Body, selector.Parameters[0], parameter);
            var predicate = CreateSearchPredicate(property, searchValue, searchPattern, caseSensitive);

            combinedPredicate = combinedPredicate == null
                ? predicate
                : Expression.OrElse(combinedPredicate, predicate);
        }

        if (combinedPredicate == null)
            return query;

        var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedPredicate, parameter);
        return query.Where(lambda);
    }

    private static Expression ReplaceParameter(
        Expression expression,
        ParameterExpression oldParam,
        ParameterExpression newParam)
    {
        return new ParameterReplacer(oldParam, newParam).Visit(expression);
    }

    private static Expression CreateSearchPredicate(
        Expression property,
        string searchValue,
        SearchPattern searchPattern,
        bool caseSensitive)
    {
        var escapedValue = EscapeLikePattern(searchValue);
        var pattern = searchPattern switch
        {
            SearchPattern.StartsWith => $"{escapedValue}%",
            SearchPattern.EndsWith => $"%{escapedValue}",
            SearchPattern.Exact => escapedValue,
            _ => $"%{escapedValue}%"
        };

        if (property.Type != typeof(string))
            property = Expression.Convert(property, typeof(string));

        if (searchPattern == SearchPattern.Exact)
        {
            var nullCheck = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));
            var equalExpression = caseSensitive
                ? Expression.Equal(property, Expression.Constant(pattern))
                : Expression.Equal(
                    Expression.Call(property, ToLowerMethod),
                    Expression.Constant(pattern.ToLower()));

            return Expression.AndAlso(nullCheck, equalExpression);
        }

        switch (caseSensitive)
        {
            case false when PostgresILikeMethod != null:
                return Expression.Call(
                    null,
                    PostgresILikeMethod,
                    Expression.Constant(EF.Functions),
                    property,
                    Expression.Constant(pattern));

            case true:
                return Expression.Call(
                    null,
                    LikeMethod,
                    Expression.Constant(EF.Functions),
                    property,
                    Expression.Constant(pattern));
        }

        var emptyString = Expression.Constant(string.Empty, typeof(string));
        var coalesce = Expression.Coalesce(property, emptyString);
        var lowerProperty = Expression.Call(coalesce, ToLowerMethod);

        return Expression.Call(
            null,
            LikeMethod,
            Expression.Constant(EF.Functions),
            lowerProperty,
            Expression.Constant(pattern.ToLower()));
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", @"\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }

    private sealed class ParameterReplacer(
        ParameterExpression oldParam,
        ParameterExpression newParam) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParam
                ? newParam
                : base.VisitParameter(node);
        }
    }
}
