using Common.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Identity.Domain.Users;
using Modules.Identity.Infrastructure.Database.Converters;

namespace Modules.Identity.Infrastructure.Database.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<UserIdConverter>()
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

        builder.ComplexProperty(x => x.EmailVerificationToken, b =>
        {
            b.ToJson();

            b.Property(t => t.Value)
                .HasMaxLength(44)
                .IsRequired();

            b.Property(t => t.CreatedAt)
                .IsRequired();

            b.Property(t => t.ExpiresAt)
                .IsRequired();
        });

        builder.Property(x => x.Password)
            .HasConversion<PasswordConverter>()
            .HasMaxLength(128)
            .IsRequired(false);

        builder.ComplexProperty(x => x.PasswordResetToken, b =>
        {
            b.ToJson();

            b.Property(t => t.Value)
                .HasMaxLength(44)
                .IsRequired();

            b.Property(t => t.CreatedAt)
                .IsRequired();

            b.Property(t => t.ExpiresAt)
                .IsRequired();
        });

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

        builder.Property(x => x.Status)
            .HasConversion<UserStatusConverter>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasConversion<UserRoleConverter>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);
    }
}
