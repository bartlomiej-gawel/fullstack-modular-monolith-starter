using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Infrastructure.Database.Converters;

public sealed class UserStatusConverter : ValueConverter<UserStatus, string>
{
    public UserStatusConverter() : base(
        value => value.ToString(),
        value => Enum.Parse<UserStatus>(value))
    {
    }
}
