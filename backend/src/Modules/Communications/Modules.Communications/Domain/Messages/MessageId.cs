using Common.Abstractions.Domain;

namespace Modules.Communications.Domain.Messages;

public sealed class MessageId : EntityId<Guid>
{
    public MessageId(Guid value)
        : base(value)
    {
    }

    public static MessageId Create() => new(Guid.CreateVersion7());
    public static MessageId CreateFrom(Guid value) => new(value);
}