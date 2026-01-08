using Common.Abstractions.Modules;
using Common.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Communications.Infrastructure.Database;

namespace Modules.Communications;

public sealed class CommunicationsModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<CommunicationsModuleDbContext>(CommunicationsModuleSchema.Name);
    }

    public void Use(WebApplication app)
    {
    }
}