using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public interface ITaskItemStatusRepository : IRepository<TaskItemStatus>
    {
        Task<bool> ExistsAsync(int id, CancellationToken ct);

    }
}
