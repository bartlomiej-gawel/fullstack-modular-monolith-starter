using Common.Infrastructure.Database.Configurations;
using Common.Infrastructure.Inbox;
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
    internal DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        if (EnableOutbox)
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        if (EnableInbox)
            modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public abstract Task InitializeDataAsync(CancellationToken cancellationToken = default);
}
