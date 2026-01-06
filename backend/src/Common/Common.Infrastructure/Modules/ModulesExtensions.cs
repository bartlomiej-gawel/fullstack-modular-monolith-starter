using System.Reflection;
using Common.Abstractions.Modules;

namespace Common.Infrastructure.Modules;

public static class ModulesExtensions
{
    private const string ModuleAssemblyPrefix = "Modules.";

    public static IList<Assembly> LoadAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .ToList();

        var locations = assemblies
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => assembly.Location)
            .ToList();

        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(file => !locations.Contains(file, StringComparer.InvariantCultureIgnoreCase))
            .Where(file => Path.GetFileName(file).StartsWith(ModuleAssemblyPrefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var file in files)
            assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file)));

        return assemblies;
    }

    public static IList<IModule> LoadModules(IEnumerable<Assembly> assemblies)
    {
        var modules = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type is { IsInterface: false } &&
                type.IsAssignableTo(typeof(IModule)))
            .OrderBy(type => type.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();

        return modules;
    }
}