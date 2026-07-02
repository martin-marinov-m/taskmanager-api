using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public class TaskItemStatusRepository : ITaskItemStatusRepository
    {
        private readonly TaskManagerDbContext _dbContext;

        public TaskItemStatusRepository(TaskManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TaskItemStatus entity, CancellationToken ct)
        {
            await _dbContext.TaskItemStatuses.AddAsync(entity, ct);
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken ct)
        {
            var taskItemStatus = await _dbContext.TaskItemStatuses.FindAsync(id, ct);

            if (taskItemStatus == null)
                return false;

            _dbContext.TaskItemStatuses.Remove(taskItemStatus);
            return true;
        }

        public async Task<IEnumerable<TaskItemStatus>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.TaskItemStatuses.AsNoTracking().ToListAsync(ct);
        }

        public async Task<TaskItemStatus?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _dbContext.TaskItemStatuses.FirstOrDefaultAsync(tis => tis.Id == id, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct)
        {
            return await _dbContext.TaskItemStatuses.AnyAsync(tis => tis.Id == id, ct);
        }

        public void Update(TaskItemStatus entity)
        {
            _dbContext.TaskItemStatuses.Update(entity);
        }
    }
}