using DataManager.Common.Abstractions.Repositories;
using DataManager.Common.POCOs;

namespace DataManager.Common.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<RefreshToken> RefreshTokenRepository { get; }

        Task<int> SaveAsync();
    }
}
