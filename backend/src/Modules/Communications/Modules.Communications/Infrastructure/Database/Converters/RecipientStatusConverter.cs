using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Communications.Domain.Recipients;

namespace Modules.Communications.Infrastructure.Database.Converters;

public sealed class RecipientStatusConverter : ValueConverter<RecipientStatus, string>
{
    public RecipientStatusConverter() : base(
        value => value.ToString(),
        value => Enum.Parse<RecipientStatus>(value))
    {
    }
}
