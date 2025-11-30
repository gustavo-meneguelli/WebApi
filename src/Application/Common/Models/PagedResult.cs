namespace Application.Common.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}