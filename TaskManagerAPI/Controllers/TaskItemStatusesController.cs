using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models.Dtos;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemStatusesController : ControllerBase
    {
        private readonly ITaskItemStatusService _taskItemStatusService;

        public TaskItemStatusesController(ITaskItemStatusService taskItemStatusService)
        {
            _taskItemStatusService = taskItemStatusService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemStatusDto>>> GetAllStatusesAsync(CancellationToken ct)
        {
            var taskItemStatusDtos = await _taskItemStatusService.GetAllAsync(ct);

            return Ok(taskItemStatusDtos);
        }

        [HttpGet("{id}", Name = "GetStatusByIdAsync")]
        public async Task<ActionResult<TaskItemStatusDto>> GetStatusByIdAsync(int id, CancellationToken ct)
        {
                var taskItemStatusDto = await _taskItemStatusService.GetByIdAsync(id, ct);

                return Ok(taskItemStatusDto);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemStatusDto>> CreateStatusAsync([FromBody] CreateTaskItemStatusDto createStatusDto, CancellationToken ct)
        {
            var result = await _taskItemStatusService.AddAsync(createStatusDto, ct);

            return CreatedAtRoute("GetStatusByIdAsync", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromBody] TaskItemStatusDto statusDto, CancellationToken ct)
        {
                await _taskItemStatusService.UpdateAsync(id, statusDto, ct);
                return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatusAsync(int id, CancellationToken ct)
        {
                await _taskItemStatusService.DeleteAsync(id, ct);
                return NoContent();
        }

    }
}
