using DataManager.Common.Abstractions;
using DataManager.Common.Abstractions.Repositories;
using DataManager.Common.DataAccess;
using DataManager.Common.POCOs;

namespace DataManager.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private readonly AppDbContext _context;

        public IRepository<User> UserRepository { get; init; }
        public IRepository<Role> RoleRepository { get; init; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            UserRepository = new Repository<User>(_context);
            RoleRepository = new Repository<Role>(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
