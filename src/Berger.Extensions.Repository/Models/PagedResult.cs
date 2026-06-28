namespace Berger.Extensions.Repository;

public sealed record PagedResult<TEntity>
(
    IReadOnlyList<TEntity> Items,
    int Page,
    int PageSize,
    long TotalItems
)
{
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
