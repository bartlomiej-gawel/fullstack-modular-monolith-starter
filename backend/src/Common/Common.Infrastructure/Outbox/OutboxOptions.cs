using System.ComponentModel.DataAnnotations;

namespace Common.Infrastructure.Outbox;

internal sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    [Required] public int IntervalSeconds { get; init; }
    [Required] public int CleanupIntervalSeconds { get; init; }
    [Required] public int BatchSize { get; init; }
    [Required] public int RetentionDays { get; init; }
    [Required] public int MaxRetryCount { get; init; }
}
