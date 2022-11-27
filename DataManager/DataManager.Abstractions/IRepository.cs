using System.Linq.Expressions;

namespace DataManager.Abstractions
{
    public interface IRepository<TEntity>: IDisposable where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        TEntity? GetById(object id);
        Task<TEntity?> GetByIdAsync(object id);

        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);

        void Delete(object id);
        Task DeleteAsync(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);
    }
}
