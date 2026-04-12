using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly TaskManagerDbContext _dbContext;

        public TaskItemRepository(TaskManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TaskItem entity, CancellationToken ct)
        {
            await _dbContext.TaskItems.AddAsync(entity, ct);
        }

        public void DeleteByEntity(TaskItem entity)
        {
            _dbContext.TaskItems.Remove(entity);
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken ct)
        {
            var taskItem = await _dbContext.TaskItems.FindAsync(id, ct);

            if (taskItem == null)
                return false;

            _dbContext.TaskItems.Remove(taskItem);
            return true;
        }


        public async Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.TaskItems.AsNoTracking().Include(ti => ti.Status).Include(ti => ti.User).ToListAsync(ct);
        }

        public IQueryable<TaskItem> GetAllTaskItemsQuery()
        {
            return _dbContext.TaskItems.AsNoTracking();
        }

        public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _dbContext.TaskItems.Include(ti => ti.Status).Include(ti => ti.User).FirstOrDefaultAsync(ti => ti.Id == id, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct)
        {
            return await _dbContext.TaskItems.AnyAsync(ti => ti.Id == id, ct);
        }

        public void Update(TaskItem entity)
        {
            _dbContext.TaskItems.Update(entity);
        }
    }
}
