using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models.Dtos;
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
    }
}
