namespace Common.Abstractions.Domain;

public abstract class EntityId<T> : ValueObject
    where T : notnull
{
    public T Value { get; }

    protected EntityId(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
