namespace DataManager.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
