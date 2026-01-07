namespace Common.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid EventId { get; init; }
    public string EventType { get; init; } = null!;
    public string EventTypeAssembly { get; init; } = null!;
    public string Payload { get; init; } = null!;
    public DateTime OccurredAt { get; init; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? LastRetryAt { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
        LastRetryAt = DateTime.UtcNow;
    }
}