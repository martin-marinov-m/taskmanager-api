using TaskManagerAPI.Models.Dtos;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;

namespace TaskManagerAPI.Services
{
    public interface ITaskItemStatusService
    {
        Task<IEnumerable<TaskItemStatusDto>> GetAllAsync(CancellationToken ct);
        Task<TaskItemStatusDto> GetByIdAsync(int id, CancellationToken ct);

        Task<TaskItemStatusDto> CreateAsync(CreateTaskItemStatusDto createStatusDto, CancellationToken ct);

        Task UpdateAsync(int id, TaskItemStatusDto statusDto, CancellationToken ct);

        Task DeleteAsync(int id, CancellationToken ct);
    }
}
