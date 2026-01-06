using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Database;

public abstract class ApplicationDbContext : DbContext
{
    protected abstract string Schema { get; }

    protected ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public abstract Task InitializeDataAsync(CancellationToken cancellationToken = default);
}
