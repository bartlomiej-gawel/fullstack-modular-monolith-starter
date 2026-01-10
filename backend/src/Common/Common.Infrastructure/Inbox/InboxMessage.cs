namespace Common.Infrastructure.Inbox;

public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    private InboxMessage(
        Guid eventId,
        string eventTypeName,
        string eventHandlerTypeName,
        DateTime processedAt,
        Guid? correlationId = null)
    {
        EventId = eventId;
        EventTypeName = eventTypeName;
        EventHandlerTypeName = eventHandlerTypeName;
        ProcessedAt = processedAt;
        CorrelationId = correlationId;
    }

    public Guid EventId { get; }
    public string EventTypeName { get; } = null!;
    public string EventHandlerTypeName { get; } = null!;
    public DateTime ProcessedAt { get; }
    public Guid? CorrelationId { get; }

    public static InboxMessage Create(
        Guid eventId,
        string eventTypeName,
        string eventHandlerTypeName,
        Guid? correlationId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventTypeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventHandlerTypeName);

        return new InboxMessage(
            eventId,
            eventTypeName,
            eventHandlerTypeName,
            DateTime.UtcNow,
            correlationId);
    }
}
