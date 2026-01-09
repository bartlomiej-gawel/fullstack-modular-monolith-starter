using Common.Infrastructure.Database.Configurations;
using Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Database;

public abstract class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected abstract string SchemaName { get; }
    protected internal abstract bool EnableOutbox { get; }
    protected internal abstract bool EnableInbox { get; }

    internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        if (EnableOutbox)
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public abstract Task InitializeDataAsync(CancellationToken cancellationToken = default);
}
