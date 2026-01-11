using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Infrastructure.Database.Converters;

public sealed class PasswordConverter : ValueConverter<Password, string>
{
    public PasswordConverter() : base(
        value => value.Value,
        value => Password.CreateFromHash(value))
    {
    }
}
