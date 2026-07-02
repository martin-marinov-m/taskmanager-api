using TaskManagerAPI.Models.Dtos.TaskItemDtos;
using TaskManagerAPI.Models.Filters;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Models.Paging;

namespace TaskManagerAPI.Services
{
    public interface ITaskItemService
    {
        Task<TaskManagerPageResult<TaskItemDto>> GetAllAsync(TaskItemFilters filters, UserInfoDto userInfo, CancellationToken ct);

        Task<TaskItemDto> GetByIdAsync(int id, UserInfoDto userInfo, CancellationToken ct);

        Task<TaskItemDto> AddAsync(CreateTaskItemDto createTaskDto, UserInfoDto userInfo, CancellationToken ct);

        Task UpdateAsync(int id, UpdateTaskItemDto updateDto, UserInfoDto userInfo, CancellationToken ct);

        Task DeleteAsync(int id, UserInfoDto userInfo, CancellationToken ct);
    }
}