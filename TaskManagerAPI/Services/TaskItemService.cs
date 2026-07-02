using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos.TaskItemDtos;
using TaskManagerAPI.Models.Filters;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Models.Paging;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ITaskItemStatusRepository _taskItemStatusRepository;
        private readonly IMapper _mapper;

        public TaskItemService(ITaskItemRepository taskItemRepository, ITaskItemStatusRepository taskItemStatusRepository, IMapper mapper)
        {
            _taskItemRepository = taskItemRepository;
            _taskItemStatusRepository = taskItemStatusRepository;
            _mapper = mapper;
        }

        public async Task<TaskManagerPageResult<TaskItemDto>> GetAllAsync(TaskItemFilters filters, UserInfoDto userInfo, CancellationToken ct)
        {
            var page = Math.Max(filters.Page, 1);

            var take = Math.Max(filters.Take, 0);
            take = Math.Min(take, 100);

            var skip = (page - 1) * take;

            var taskItemsQuery = _taskItemRepository.GetAllTaskItemsQuery();

            if (!userInfo.IsAdmin)
                taskItemsQuery = taskItemsQuery.Where(ti => ti.UserId == userInfo.UserId);


            if (!string.IsNullOrWhiteSpace(filters.Title))
            {
                var titleFilter = filters.Title?.Trim();
                taskItemsQuery = taskItemsQuery.Where(ti => EF.Functions.Like(ti.Title, $"%{titleFilter}%"));
            }

            var totalCount = await taskItemsQuery.CountAsync(ct);

            var taskItemDtos = await taskItemsQuery.OrderBy(ti => ti.CreatedDate).ThenBy(ti => ti.Id).Skip(skip).Take(take).ProjectTo<TaskItemDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

            var pageResult = new TaskManagerPageResult<TaskItemDto>
            {
                Items = taskItemDtos,
                Page = page,
                Take = take,
                TotalCount = totalCount
            };

            return pageResult;
        }


        public async Task<TaskItemDto> GetByIdAsync(int id, UserInfoDto userInfo, CancellationToken ct)
        {

            var taskItem = await _taskItemRepository.GetByIdAsync(id, ct);

            if (taskItem == null)
                throw new NotFoundException("TaskItem", id.ToString());

            if (!userInfo.IsAdmin && userInfo.UserId != taskItem.UserId)
                throw new ForbiddenOperationException("User can access only his TaskItem");

            var taskItemDto = _mapper.Map<TaskItemDto>(taskItem);

            return taskItemDto;
        }

        public async Task<TaskItemDto> AddAsync(CreateTaskItemDto createDto, UserInfoDto userInfo, CancellationToken ct)
        {
            if (createDto.DueDate.HasValue && createDto.DueDate < DateTime.UtcNow)
                throw new ParameterValidationException("Date cannot be in the past", "DueDate", createDto.DueDate.Value.ToString());

            if (!await _taskItemStatusRepository.ExistsAsync(createDto.StatusId, ct))
                throw new NotFoundException("TaskItemStatus", createDto.StatusId.ToString());

            var taskItem = _mapper.Map<TaskItem>(createDto);

            taskItem.UserId = userInfo.UserId!;

            await _taskItemRepository.AddAsync(taskItem, ct);
            await _taskItemRepository.SaveChangesAsync(ct);

            return await GetByIdAsync(taskItem.Id, userInfo, ct);
        }


        public async Task UpdateAsync(int id, UpdateTaskItemDto updateDto, UserInfoDto userInfo, CancellationToken ct)
        {
            if (id != updateDto.Id)
                throw new ArgumentMismatchException(nameof(id), id.ToString(), nameof(updateDto.Id), updateDto.Id.ToString());

            if (updateDto.DueDate.HasValue && updateDto.DueDate < DateTime.UtcNow)
                throw new ParameterValidationException("Date cannot be in the past", "DueDate", updateDto.DueDate.Value.ToString());

            if (!await _taskItemStatusRepository.ExistsAsync(updateDto.StatusId, ct))
                throw new NotFoundException("TaskItemStatus", updateDto.StatusId.ToString());

            var taskItem = await _taskItemRepository.GetByIdAsync(updateDto.Id, ct);

            if (taskItem == null)
                throw new NotFoundException("TaskItem", updateDto.Id.ToString());

            if (!userInfo.IsAdmin && taskItem.UserId != userInfo.UserId)
                throw new ForbiddenOperationException("User can update only his TaskItem");

            _mapper.Map(updateDto, taskItem);

            try
            {
                await _taskItemRepository.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _taskItemRepository.ExistsAsync(taskItem.Id, ct))
                    throw new NotFoundException("TaskItem", taskItem.Id.ToString());

                else
                    throw;
            }

        }

        public async Task DeleteAsync(int id, UserInfoDto userInfo, CancellationToken ct)
        {
            var taskItem = await _taskItemRepository.GetByIdAsync(id, ct);

            if (taskItem == null)
                throw new NotFoundException("TaskItem", id.ToString());

            if (!userInfo.IsAdmin && userInfo.UserId != taskItem.UserId)
                throw new ForbiddenOperationException("User can delete only his TaskItem");

            _taskItemRepository.DeleteByEntity(taskItem);
            await _taskItemRepository.SaveChangesAsync(ct);
        }
    }
}
