namespace Common.Infrastructure.Correlation;

internal static class EventCorrelationContext
{
    private static readonly AsyncLocal<Guid?> CorrelationIdValue = new();

    public static Guid? CorrelationId
    {
        get => CorrelationIdValue.Value;
        set => CorrelationIdValue.Value = value;
    }
}