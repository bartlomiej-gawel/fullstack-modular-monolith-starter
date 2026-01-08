using System.Reflection;
using Common.Abstractions.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Domain;

internal static class DomainExtensions
{
    extension(IServiceCollection services)
    {
        public void AddDomainEvents()
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        }

        public void AddDomainEventHandlers(IEnumerable<Assembly> assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies.ToArray())
                .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}
