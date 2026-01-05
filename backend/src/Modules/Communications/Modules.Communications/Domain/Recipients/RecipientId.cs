using Common.Abstractions.Domain;

namespace Modules.Communications.Domain.Recipients;

public sealed class RecipientId : EntityId<Guid>
{
    public RecipientId(Guid value)
        : base(value)
    {
    }

    public static RecipientId Create() => new(Guid.CreateVersion7());
    public static RecipientId CreateFrom(Guid value) => new(value);
}