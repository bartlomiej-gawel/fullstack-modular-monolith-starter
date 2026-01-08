using Common.Infrastructure.Database;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.Outbox;

public sealed class OutboxProcessor<TDbContext> : BackgroundService
    where TDbContext : ApplicationDbContext
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
