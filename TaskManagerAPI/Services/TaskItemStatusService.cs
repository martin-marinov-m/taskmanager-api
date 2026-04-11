using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.Dtos;
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
                throw new KeyNotFoundException("Status not Found.");

            var taskItemStatusDto = _mapper.Map<TaskItemStatusDto>(taskItemStatus);

            return taskItemStatusDto;
        }

        public async Task<TaskItemStatusDto> CreateAsync(CreateTaskItemStatusDto createStatusDto, CancellationToken ct)
        {
            var taskItemStatus = _mapper.Map<TaskItemStatus>(createStatusDto);

            await _taskItemStatusRepository.AddAsync(taskItemStatus, ct);
            await _taskItemStatusRepository.SaveChangesAsync(ct);

            var taskItemStatusDto = _mapper.Map<TaskItemStatusDto>(taskItemStatus);

            return taskItemStatusDto;
        }

        public async Task UpdateAsync(int id, TaskItemStatusDto statusDto, CancellationToken ct)
        {
            if (id != statusDto.Id)
                throw new ArgumentException("Id from URL and Id from object does not match");

            var taskItemStatus = await _taskItemStatusRepository.GetByIdAsync(id, ct);

            if (taskItemStatus == null)
                throw new KeyNotFoundException("Status not Found");

            _mapper.Map(statusDto, taskItemStatus);

            try
            {
                await _taskItemStatusRepository.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _taskItemStatusRepository.ExistsAsync(statusDto.Id, ct))
                    throw new KeyNotFoundException("Status Not Found");
                else
                    throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var result = await _taskItemStatusRepository.DeleteByIdAsync(id, ct);

            if (!result)
                throw new KeyNotFoundException("Status not Found");

            await _taskItemStatusRepository.SaveChangesAsync(ct);
        }




    }
}
