using Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Modules.Communications.Domain.Messages;
using Modules.Communications.Domain.Recipients;

namespace Modules.Communications.Infrastructure.Database;

public sealed class CommunicationsModuleDbContext : ApplicationDbContext
{
    public CommunicationsModuleDbContext(DbContextOptions<CommunicationsModuleDbContext> options)
        : base(options)
    {
    }

    protected override string SchemaName => CommunicationsModuleSchema.Name;
    protected override bool EnableOutbox => false;
    protected override bool EnableInbox => true;

    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<Message> Messages => Set<Message>();

    public override Task InitializeDataAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
