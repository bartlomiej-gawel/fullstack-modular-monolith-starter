using System.ComponentModel.DataAnnotations;

namespace Common.Infrastructure.Inbox;

public sealed class InboxOptions
{
    public const string SectionName = "Inbox";

    [Required] public int CleanupIntervalSeconds { get; init; }
    [Required] public int RetentionDays { get; init; }
}
