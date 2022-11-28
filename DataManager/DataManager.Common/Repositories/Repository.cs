using DataManager.Common.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataManager.Common.DataAccess
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private bool _disposed = false;
        protected readonly AppDbContext context;
        protected readonly DbSet<TEntity> dbSet;

        public Repository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }
        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual TEntity? GetById(object id) => dbSet.Find(id);
        public virtual async Task<TEntity?> GetByIdAsync(object id) => await dbSet.FindAsync(id);

        public virtual void Insert(TEntity entity) => dbSet.Add(entity);
        public virtual async Task InsertAsync(TEntity entity) => await dbSet.AddAsync(entity);

        public virtual void Delete(object id)
        {
            var entityToDelete = GetById(id);
            if (entityToDelete is null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found");
            }

            Delete(entityToDelete);
        }
        public virtual async Task DeleteAsync(object id)
        {
            var entityToDelete = await GetByIdAsync(id);
            if (entityToDelete is null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found");
            }

            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
