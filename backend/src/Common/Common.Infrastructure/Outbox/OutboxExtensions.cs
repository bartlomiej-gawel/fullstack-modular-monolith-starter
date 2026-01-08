using Common.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Outbox;

public static class OutboxExtensions
{
    extension(IServiceCollection services)
    {
        internal void AddOutbox(IConfiguration configuration)
        {
            services.AddOptions<OutboxOptions>()
                .Bind(configuration.GetSection(OutboxOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        public void AddOutbox<TDbContext>()
            where TDbContext : ApplicationDbContext
        {
            services.AddHostedService<OutboxProcessor<TDbContext>>();
            services.AddHostedService<OutboxCleanupProcessor<TDbContext>>();
        }
    }
}
