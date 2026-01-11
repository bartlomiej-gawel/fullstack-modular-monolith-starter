using System.Reflection;
using Common.Infrastructure.Database;
using Common.Infrastructure.Domain;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.IntegrationEvents;
using Common.Infrastructure.Outbox;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.AddDomainEvents();
            services.AddIntegrationEvents();
            services.AddDatabase(configuration);
            services.AddOutbox(configuration);
            services.AddInbox(configuration);
        }

        public void AddInfrastructureAssemblies(IList<Assembly> assemblies)
        {
            services.AddDomainEventHandlers(assemblies);
            services.AddIntegrationEventHandlers(assemblies);
            services.AddValidatorsFromAssemblies(assemblies);
        }
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        return app;
    }
}
