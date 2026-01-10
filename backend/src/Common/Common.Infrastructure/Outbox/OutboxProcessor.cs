using Common.Infrastructure.Database;
using Common.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Outbox;

public sealed class OutboxProcessor<TDbContext> : BackgroundService
    where TDbContext : ApplicationDbContext
{
    private readonly string _dbContextName = typeof(TDbContext).Name;

    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<OutboxOptions> _options;
    private readonly ILogger<OutboxProcessor<TDbContext>> _logger;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        IOptions<OutboxOptions> options,
        ILogger<OutboxProcessor<TDbContext>> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processedCount = 0;

            try
            {
                using var scope = _serviceProvider.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                if (!dbContext.EnableOutbox)
                    return;

                var dispatcher = scope.ServiceProvider.GetRequiredService<IIntegrationEventDispatcher>();

                await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                    var schema = dbContext.Model.GetDefaultSchema() ?? "public";
                    var sql = $"""
                               SELECT * FROM "{schema}"."OutboxMessages"
                               WHERE "ProcessedAt" IS NULL AND 
                                     "RetryCount" < {_options.Value.MaxRetryCount}
                               ORDER BY "OccurredAt"
                               LIMIT {_options.Value.BatchSize}
                               FOR UPDATE SKIP LOCKED
                               """;

                    var outboxMessages = await dbContext.OutboxMessages
                        .FromSqlRaw(sql)
                        .ToListAsync(stoppingToken);

                    processedCount = outboxMessages.Count;
                    if (processedCount > 0)
                    {
                        foreach (var outboxMessage in outboxMessages)
                        {
                            try
                            {
                                await dispatcher.DispatchAsync(outboxMessage, stoppingToken);

                                outboxMessage.MarkAsProcessed();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing outbox message '{EventId}'", outboxMessage.EventId);

                                outboxMessage.MarkAsFailed(ex.Message);
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }

                    await transaction.CommitAsync(stoppingToken);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured during processing outbox messages for '{DbContext}'", _dbContextName);
            }

            if (processedCount >= _options.Value.BatchSize)
                continue;

            await Task.Delay(TimeSpan.FromSeconds(_options.Value.IntervalSeconds), stoppingToken);
        }
    }
}
