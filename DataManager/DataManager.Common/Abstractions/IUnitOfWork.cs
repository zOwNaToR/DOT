namespace DataManager.Common.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
