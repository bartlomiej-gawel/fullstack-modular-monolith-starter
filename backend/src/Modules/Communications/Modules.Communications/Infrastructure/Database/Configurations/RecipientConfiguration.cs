using Common.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Communications.Domain.Recipients;
using Modules.Communications.Infrastructure.Database.Converters;

namespace Modules.Communications.Infrastructure.Database.Configurations;

public sealed class RecipientConfiguration : IEntityTypeConfiguration<Recipient>
{
    public void Configure(EntityTypeBuilder<Recipient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<RecipientIdConverter>()
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion<EmailConverter>()
            .HasMaxLength(254)
            .IsRequired();

        builder.ComplexProperty(x => x.Phone, b =>
        {
            b.ToJson();
            b.Property(p => p.Prefix)
                .HasMaxLength(5)
                .IsRequired();

            b.Property(p => p.Number)
                .HasMaxLength(15)
                .IsRequired();
        });

        builder.ComplexProperty(x => x.Preferences, b =>
        {
            b.ToJson();
            b.Property(p => p.EmailEnabled).IsRequired();
            b.Property(p => p.SmsEnabled).IsRequired();
            b.Property(p => p.InAppEnabled).IsRequired();
        });

        builder.Property(x => x.Status)
            .HasConversion<RecipientStatusConverter>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<RecipientTypeConverter>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired(false);
    }
}
