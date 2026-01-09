using System.Reflection;
using Common.Abstractions.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.IntegrationEvents;

public static class IntegrationEventExtensions
{
    extension(IServiceCollection services)
    {
        public void AddIntegrationEvents()
        {
            services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
        }

        public void AddIntegrationEventHandlers(IEnumerable<Assembly> assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}
