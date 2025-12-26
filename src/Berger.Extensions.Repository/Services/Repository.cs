using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Berger.Extensions.Abstractions;

namespace Berger.Extensions.Repository
{
    public abstract class Repository<T> : IRepository<T>, IDisposable where T : class
    {
        #region Properties
        private readonly DbSet<T> _entity;
        private readonly DbContext _context;
        #endregion

        #region Constructors
        protected Repository(DbContext context)
        {
            _entity = context.Set<T>();

            _context = context;
        }
        #endregion

        #region Methods
        public IQueryable<T> Get(bool tracking = false)
        {
            return tracking ? _entity : _entity.AsNoTracking();
        }
        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return Get().Where(expression);
        }
        public IQueryable<T> GetIgnoreFilters()
        {
            return Get().IgnoreQueryFilters();
        }
        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return Get().Where(expression).FirstOrDefault();
        }
        public T GetById(Guid id)
        {
            return _entity.Find(id);
        }
        public T Add(T entity, bool detach = false)
        {
            _entity.Add(entity);

            this.SaveChanges();

            if (detach)
                _context.Detach(entity);

            return entity;
        }
        public void Add(IQueryable<T> entities, bool detach = false)
        {
            foreach (var entity in entities)
                _entity.Add(entity);

            this.SaveChanges();

            if (detach)
                _context.Detach(entities);
        }
        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;

            this.SaveChanges();

            return entity;
        }
        public void Delete(Guid id)
        {
            var entity = GetById(id);

            _context.SoftDelete(entity);

            this.SaveChanges();
        }
        public void Delete(IQueryable<T> entities)
        {
            foreach (var entity in entities)
                _context.SoftDelete<T>(entity);

            this.SaveChanges();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _entity.AddAsync(entity);

            await this.SaveChangesAsync();

            return entity;
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _entity.Update(entity);

            await this.SaveChangesAsync();

            return entity;
        }
        public async Task UpdateAsync(Func<T, string> field, string value)
        {
            throw new NotImplementedException();
            //await _entity.ExecuteUpdateAsync(e => e.SetProperty(field, field + value));
        }
        public async Task DeleteAsync(Guid id)
        {
            var entity = GetById(id);

            _context.SoftDelete(entity);

            await this.SaveChangesAsync();
        }
        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async virtual Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
        #endregion
    }
}