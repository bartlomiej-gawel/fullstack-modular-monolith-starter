using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Infrastructure.Database.Converters;

public sealed class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter() : base(
        value => value.Value,
        value => UserId.CreateFrom(value))
    {
    }
}
