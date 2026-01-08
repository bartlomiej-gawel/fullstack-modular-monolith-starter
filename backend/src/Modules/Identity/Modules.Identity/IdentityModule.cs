using Common.Abstractions.Modules;
using Common.Infrastructure.Database;
using Common.Infrastructure.Outbox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity;

public sealed class IdentityModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<IdentityModuleDbContext>(IdentityModuleSchema.Name);
        services.AddOutbox<IdentityModuleDbContext>();
    }

    public void Use(WebApplication app)
    {
    }
}