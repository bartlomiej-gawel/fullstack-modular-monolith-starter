using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Communications.Domain.Messages;

namespace Modules.Communications.Infrastructure.Database.Converters;

public sealed class MessageChannelConverter : ValueConverter<MessageChannel, string>
{
    public MessageChannelConverter() : base(
        value => value.ToString(),
        value => Enum.Parse<MessageChannel>(value))
    {
    }
}
