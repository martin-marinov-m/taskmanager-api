using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Services
{
    public class TaskItemStatusService : ITaskItemStatusService
    {
        private readonly ITaskItemStatusRepository _taskItemStatusRepository;
        private readonly IMapper _mapper;

        public TaskItemStatusService(ITaskItemStatusRepository taskItemStatusRepository, IMapper mapper)
        {
            _taskItemStatusRepository = taskItemStatusRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskItemStatusDto>> GetAllAsync(CancellationToken ct)
        {
            var taskItemStatuses = await _taskItemStatusRepository.GetAllAsync(ct);

            var taskItemStatusesDto = taskItemStatuses.Select(x => _mapper.Map<TaskItemStatusDto>(x)).ToList();

            return taskItemStatusesDto;
        }

        public async Task<TaskItemStatusDto> GetByIdAsync(int id, CancellationToken ct)
        {
            var taskItemStatus = await _taskItemStatusRepository.GetByIdAsync(id, ct);

            if (taskItemStatus == null)
                throw new NotFoundException("TaskItemStatus", id.ToString());

            var taskItemStatusDto = _mapper.Map<TaskItemStatusDto>(taskItemStatus);

            return taskItemStatusDto;
        }

        public async Task<TaskItemStatusDto> AddAsync(CreateTaskItemStatusDto createStatusDto, CancellationToken ct)
        {
            var taskItemStatus = _mapper.Map<TaskItemStatus>(createStatusDto);

            await _taskItemStatusRepository.AddAsync(taskItemStatus, ct);
            await _taskItemStatusRepository.SaveChangesAsync(ct);

            var taskItemStatusDto = _mapper.Map<TaskItemStatusDto>(taskItemStatus);

            return taskItemStatusDto;
        }

        public async Task UpdateAsync(int id, TaskItemStatusDto updateDto, CancellationToken ct)
        {
            if (id != updateDto.Id)
                throw new ArgumentMismatchException(nameof(id), id.ToString(), nameof(updateDto.Id), updateDto.Id.ToString());

            var taskItemStatus = await _taskItemStatusRepository.GetByIdAsync(updateDto.Id, ct);

            if (taskItemStatus == null)
                throw new NotFoundException("TaskItemStatus", updateDto.Id.ToString());

            _mapper.Map(updateDto, taskItemStatus);

            try
            {
                await _taskItemStatusRepository.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _taskItemStatusRepository.ExistsAsync(taskItemStatus.Id, ct))
                    throw new NotFoundException("TaskItemStatus", taskItemStatus.Id.ToString());
                else
                    throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var result = await _taskItemStatusRepository.DeleteByIdAsync(id, ct);

            if (!result)
                throw new NotFoundException("TaskItemStatus", id.ToString());

            await _taskItemStatusRepository.SaveChangesAsync(ct);
        }
    }
}