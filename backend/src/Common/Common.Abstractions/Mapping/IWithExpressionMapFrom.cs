using System.Linq.Expressions;

namespace Common.Abstractions.Mapping;

public interface IWithExpressionMapFrom<TSource, TDestination>
    where TSource : class
    where TDestination : class
{
    static abstract Expression<Func<TSource, TDestination>> MapExpression { get; }
}