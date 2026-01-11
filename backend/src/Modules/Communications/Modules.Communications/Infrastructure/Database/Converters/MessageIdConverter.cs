using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Communications.Domain.Messages;

namespace Modules.Communications.Infrastructure.Database.Converters;

public sealed class MessageIdConverter : ValueConverter<MessageId, Guid>
{
    public MessageIdConverter() : base(
        value => value.Value,
        value => MessageId.CreateFrom(value))
    {
    }
}
