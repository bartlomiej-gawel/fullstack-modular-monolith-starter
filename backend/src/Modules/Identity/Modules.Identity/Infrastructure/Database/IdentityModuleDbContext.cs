using Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Modules.Identity.Infrastructure.Database;

public sealed class IdentityModuleDbContext : ApplicationDbContext
{
    public IdentityModuleDbContext(DbContextOptions<IdentityModuleDbContext> options)
        : base(options)
    {
    }

    protected override string Schema => IdentityModuleSchema.Name;

    public override Task InitializeDataAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}