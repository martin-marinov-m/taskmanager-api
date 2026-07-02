namespace TaskManagerAPI.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);

        Task<T?> GetByIdAsync(int id, CancellationToken ct);

        Task AddAsync(T entity, CancellationToken ct);

        void Update(T entity);

        Task<bool> DeleteByIdAsync(int id, CancellationToken ct);

        Task SaveChangesAsync(CancellationToken ct);
    }
}