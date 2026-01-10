using Common.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Inbox;

public static class InboxExtensions
{
    extension(IServiceCollection services)
    {
        internal void AddInbox(IConfiguration configuration)
        {
            services.AddOptions<InboxOptions>()
                .Bind(configuration.GetSection(InboxOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        public void AddInbox<TDbContext>()
            where TDbContext : ApplicationDbContext
        {
            services.AddHostedService<InboxCleanupProcessor<TDbContext>>();
        }
    }
}