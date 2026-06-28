using System.Linq.Expressions;

namespace Berger.Extensions.Repository;

public interface ISpecification<TEntity> where TEntity : class
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    IReadOnlyList<Expression<Func<TEntity, object>>> Includes { get; }
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; }
    int? Skip { get; }
    int? Take { get; }
    bool AsNoTracking { get; }
    bool IgnoreQueryFilters { get; }
}

public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : class
{
    private readonly List<Expression<Func<TEntity, object>>> _includes = [];

    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => _includes;
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; private set; }
    public int? Skip { get; private set; }
    public int? Take { get; private set; }
    public bool AsNoTracking { get; private set; } = true;
    public bool IgnoreQueryFilters { get; private set; }

    protected void Where(Expression<Func<TEntity, bool>> criteria) => Criteria = criteria;
    protected void Include(Expression<Func<TEntity, object>> include) => _includes.Add(include);
    protected void Order(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy) => OrderBy = orderBy;
    protected void Page(int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        Skip = (page - 1) * pageSize;
        Take = pageSize;
    }
    protected void WithTracking() => AsNoTracking = false;
    protected void WithoutQueryFilters() => IgnoreQueryFilters = true;
}
