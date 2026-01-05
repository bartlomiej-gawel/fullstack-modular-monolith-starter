using Common.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Communications;

public sealed class CommunicationsModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void Use(WebApplication app)
    {
    }
}