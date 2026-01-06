using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Database;

public sealed class DatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        IServiceProvider serviceProvider,
        ILogger<DatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var contextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                typeof(ApplicationDbContext).IsAssignableFrom(x) &&
                x is { IsInterface: false, IsAbstract: false } &&
                x != typeof(ApplicationDbContext))
            .ToList();

        if (contextTypes.Count == 0)
        {
            _logger.LogInformation("No database contexts found to migrate.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        foreach (var contextType in contextTypes)
        {
            if (scope.ServiceProvider.GetService(contextType) is not ApplicationDbContext context)
            {
                _logger.LogWarning("Could not resolve database context '{ContextType}'", contextType.Name);
                continue;
            }

            try
            {
                await ApplyMigrationsAsync(context, contextType.Name, cancellationToken);
                await InitializeDataAsync(context, contextType.Name, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process database context '{ContextType}'", contextType.Name);
                throw;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ApplyMigrationsAsync(ApplicationDbContext context, string contextName, CancellationToken cancellationToken)
    {
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying migrations for '{ContextName}'", contextName);

                await context.Database.MigrateAsync(cancellationToken);

                _logger.LogInformation("Successfully applied migrations for '{ContextName}'", contextName);
            }
            else
            {
                _logger.LogInformation("No pending migrations for '{ContextName}'", contextName);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to apply migrations for '{ContextName}'", contextName);
            throw;
        }
    }

    private async Task InitializeDataAsync(ApplicationDbContext context, string contextName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Initializing data for '{ContextName}'", contextName);

            await context.InitializeDataAsync(cancellationToken);

            _logger.LogInformation("Successfully initialized data for '{ContextName}'", contextName);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to initialize data for '{ContextName}'", contextName);
            throw;
        }
    }
}
