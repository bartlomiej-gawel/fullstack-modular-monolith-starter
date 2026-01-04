using Common.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

var assemblies = ModulesExtensions.LoadAssemblies(builder.Configuration);
var modules = ModulesExtensions.LoadModules(assemblies);

foreach (var module in modules)
    module.Register(builder.Services, builder.Configuration);

var app = builder.Build();

foreach (var module in modules)
    module.Use(app);

assemblies.Clear();
modules.Clear();

app.Run();