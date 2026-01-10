using Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Outbox;

public sealed class OutboxCleanupProcessor<TDbContext> : BackgroundService
    where TDbContext : ApplicationDbContext
{
    private readonly string _dbContextName = typeof(TDbContext).Name;

    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<OutboxOptions> _options;
    private readonly ILogger<OutboxCleanupProcessor<TDbContext>> _logger;

    public OutboxCleanupProcessor(
        IServiceProvider serviceProvider,
        IOptions<OutboxOptions> options,
        ILogger<OutboxCleanupProcessor<TDbContext>> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                if (!dbContext.EnableOutbox)
                    return;

                var cleanupDate = DateTime.UtcNow.AddDays(-_options.Value.RetentionDays);

                var deletedCount = await dbContext.OutboxMessages
                    .Where(x =>
                        x.ProcessedAt != null &&
                        x.ProcessedAt < cleanupDate)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deletedCount > 0)
                    _logger.LogInformation("Cleaned up {Count} old outbox messages from {DbContext}", deletedCount, _dbContextName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured during processing outbox cleanup messages for {DbContext}", _dbContextName);
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.Value.CleanupIntervalSeconds), stoppingToken);
        }
    }
}
