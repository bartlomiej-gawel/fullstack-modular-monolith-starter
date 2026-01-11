using Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Infrastructure.Database;

public sealed class IdentityModuleDbContext : ApplicationDbContext
{
    public IdentityModuleDbContext(DbContextOptions<IdentityModuleDbContext> options)
        : base(options)
    {
    }

    protected override string SchemaName => IdentityModuleSchema.Name;
    protected override bool EnableOutbox => true;
    protected override bool EnableInbox => true;

    public DbSet<User> Users => Set<User>();

    public override Task InitializeDataAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
