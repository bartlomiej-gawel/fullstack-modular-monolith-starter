using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Communications.Domain.Recipients;

namespace Modules.Communications.Infrastructure.Database.Converters;

public sealed class RecipientIdConverter : ValueConverter<RecipientId, Guid>
{
    public RecipientIdConverter() : base(
        value => value.Value,
        value => RecipientId.CreateFrom(value))
    {
    }
}
