using System.ComponentModel.DataAnnotations;

namespace Common.Infrastructure.Database;

internal sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required] public string ConnectionString { get; init; } = null!;
    public bool EnableDetailedErrors { get; init; }
    public bool EnableSensitiveDataLogging { get; init; }
}
