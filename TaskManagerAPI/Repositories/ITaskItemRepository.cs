using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public interface ITaskItemRepository : IRepository<TaskItem>
    {
        IQueryable<TaskItem> GetAllTaskItemsQuery();

        Task<bool> ExistsAsync(int id, CancellationToken ct);

        void DeleteByEntity(TaskItem entity);

    }
}
