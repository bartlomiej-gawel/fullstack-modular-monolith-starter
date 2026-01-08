using Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.EventId);

        builder.Property(x => x.EventId)
            .ValueGeneratedNever();

        builder.Property(x => x.EventTypeName)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.EventTypeAssemblyName)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.EventPayload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.Property(x => x.LastRetryAt)
            .IsRequired(false);

        builder.HasIndex(x => new { x.ProcessedAt, x.OccurredAt });
    }
}
