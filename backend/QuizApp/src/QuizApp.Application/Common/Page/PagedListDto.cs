namespace QuizApp.Application.Common.Page;

public class PagedListDto<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Total { get; init; }
    public int Skip { get; init; }
    public int Take { get; init; }

    public PagedListDto(IEnumerable<T> items, int total, int skip, int take)
    {
        Items = items.ToList();
        Total = total;
        Skip = skip;
        Take = take;
    }
}
