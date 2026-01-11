using Common.Infrastructure.Inbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.HasKey(x => new { x.EventId, x.EventHandlerTypeName });

        builder.Property(x => x.EventId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.EventTypeName)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.EventHandlerTypeName)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.ProcessedAt).IsRequired();
        builder.Property(x => x.CorrelationId).IsRequired(false);

        builder.HasIndex(x => x.ProcessedAt);
    }
}
