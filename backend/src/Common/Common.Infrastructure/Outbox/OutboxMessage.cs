namespace Common.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    private OutboxMessage(
        Guid eventId,
        string eventTypeName,
        string eventTypeAssemblyName,
        string eventPayload,
        DateTime occurredAt)
    {
        EventId = eventId;
        EventTypeName = eventTypeName;
        EventTypeAssemblyName = eventTypeAssemblyName;
        EventPayload = eventPayload;
        OccurredAt = occurredAt;
        ProcessedAt = null;
        Error = null;
        RetryCount = 0;
        LastRetryAt = null;
    }

    public Guid EventId { get; }
    public string EventTypeName { get; } = null!;
    public string EventTypeAssemblyName { get; } = null!;
    public string EventPayload { get; } = null!;
    public DateTime OccurredAt { get; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? LastRetryAt { get; private set; }

    public static OutboxMessage Create(
        Guid eventId,
        string eventTypeName,
        string eventTypeAssemblyName,
        string eventPayload,
        DateTime occurredAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventTypeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventTypeAssemblyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventPayload);

        return new OutboxMessage(
            eventId,
            eventTypeName,
            eventTypeAssemblyName,
            eventPayload,
            occurredAt);
    }

    public void MarkAsProcessed()
    {
        if (ProcessedAt is not null)
            return;

        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
        LastRetryAt = DateTime.UtcNow;
    }
}
