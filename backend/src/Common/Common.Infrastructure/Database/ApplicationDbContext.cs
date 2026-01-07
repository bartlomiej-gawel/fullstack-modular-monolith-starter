using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Database;

public abstract class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected abstract string Schema { get; }
    protected abstract bool EnableOutbox { get; }
    protected abstract bool EnableInbox { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public abstract Task InitializeDataAsync(CancellationToken cancellationToken = default);
}
