using Common.Abstractions.Domain;

namespace Modules.Identity.Domain.Users;

public sealed class UserId : EntityId<Guid>
{
    public UserId(Guid value)
        : base(value)
    {
    }

    public static UserId Create() => new(Guid.CreateVersion7());
    public static UserId CreateFrom(Guid value) => new(value);
}