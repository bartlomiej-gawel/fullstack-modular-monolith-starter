using Common.Infrastructure;
using Common.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var assemblies = ModulesExtensions.LoadAssemblies();
var modules = ModulesExtensions.LoadModules(assemblies);

foreach (var module in modules)
    module.Register(builder.Services, builder.Configuration);

builder.Services.AddInfrastructureAssemblies(assemblies);

var app = builder.Build();

app.UseInfrastructure();

foreach (var module in modules)
    module.Use(app);

assemblies.Clear();
modules.Clear();

app.Run();