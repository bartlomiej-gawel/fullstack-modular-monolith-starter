using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Communications.Domain.Messages;
using Modules.Communications.Infrastructure.Database.Converters;

namespace Modules.Communications.Infrastructure.Database.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<MessageIdConverter>()
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.RecipientId)
            .HasConversion<RecipientIdConverter>()
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Channel)
            .HasConversion<MessageChannelConverter>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<MessageStatusConverter>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.SentAt).IsRequired(false);
        builder.Property(x => x.FailedAt).IsRequired(false);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.ReadAt).IsRequired(false);
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.DeletedAt).IsRequired(false);
    }
}
