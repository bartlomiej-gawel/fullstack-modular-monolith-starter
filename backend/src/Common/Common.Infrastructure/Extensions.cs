using System.Reflection;
using Common.Infrastructure.Database;
using Common.Infrastructure.Domain;
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
            services.AddDatabase(configuration);
        }

        public void AddInfrastructureAssemblies(IEnumerable<Assembly> assemblies)
        {
            services.AddDomainEventHandlers(assemblies);
        }
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        return app;
    }
}