using System.Linq.Expressions;

namespace DataManager.Common.Abstractions.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class, IEntity
    {
        TEntity? Get(
            Expression<Func<TEntity, bool>> filter,
            string includeProperties = "");
        Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> filter,
            string includeProperties = "");

        IEnumerable<TEntity> Search(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        Task<IEnumerable<TEntity>> SearchAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        TEntity? GetById(object id);
        Task<TEntity?> GetByIdAsync(object id);

        TEntity? Insert(TEntity entity);
        Task<TEntity?> InsertAsync(TEntity entity);

        void Delete(object id);
        Task DeleteAsync(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);
    }
}
