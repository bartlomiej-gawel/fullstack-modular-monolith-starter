using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Communications.Domain.Recipients;

namespace Modules.Communications.Infrastructure.Database.Converters;

public sealed class RecipientTypeConverter : ValueConverter<RecipientType, string>
{
    public RecipientTypeConverter() : base(
        value => value.ToString(),
        value => Enum.Parse<RecipientType>(value))
    {
    }
}
