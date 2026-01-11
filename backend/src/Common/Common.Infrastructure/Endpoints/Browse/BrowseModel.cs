using Common.Infrastructure.Endpoints.Browse.Base;

namespace Common.Infrastructure.Endpoints.Browse;

public record BrowseModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }
    public string? Search { get; set; }
    public SearchPattern SearchPattern { get; set; } = SearchPattern.Contains;
    public bool SearchCaseSensitive { get; set; }
}
