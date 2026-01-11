using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Infrastructure.Database.Converters;

public sealed class UserRoleConverter : ValueConverter<UserRole, string>
{
    public UserRoleConverter() : base(
        value => value.ToString(),
        value => Enum.Parse<UserRole>(value))
    {
    }
}
