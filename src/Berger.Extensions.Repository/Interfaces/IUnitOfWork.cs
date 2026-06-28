using System.Linq.Expressions;

namespace Berger.Extensions.Repository
{
	public interface IUnitOfWork
	{
		IRepository<TEntity> Repository<TEntity>() where TEntity : class;

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}