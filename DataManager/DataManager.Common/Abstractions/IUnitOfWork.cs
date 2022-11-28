using DataManager.Common.Abstractions.Repositories;
using DataManager.Common.POCOs;

namespace DataManager.Common.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }

        Task<int> SaveAsync();
    }
}
