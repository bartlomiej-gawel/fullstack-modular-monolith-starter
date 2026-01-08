namespace Common.Abstractions.Correlation;

public static class EventCorrelationContext
{
    private static readonly AsyncLocal<Guid?> CorrelationIdValue = new();

    public static Guid? CorrelationId
    {
        get => CorrelationIdValue.Value;
        set => CorrelationIdValue.Value = value;
    }
}
