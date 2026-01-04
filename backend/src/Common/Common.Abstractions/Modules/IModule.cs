using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Abstractions.Modules;

public interface IModule
{
    void Register(IServiceCollection services, IConfiguration configuration);
    void Use(WebApplication app);
}