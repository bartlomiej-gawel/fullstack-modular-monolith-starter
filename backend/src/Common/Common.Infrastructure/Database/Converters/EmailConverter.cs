using Common.Abstractions.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Common.Infrastructure.Database.Converters;

public sealed class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter() : base(
        value => value.Value,
        value => Email.Create(value).Value)
    {
    }
}
