using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Constants;
using TaskManagerAPI.Models.Dtos.TaskItemDtos;
using TaskManagerAPI.Models.Filters;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;

        public TaskItemsController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTaskItemsAsync([FromQuery] TaskItemFilters filters, CancellationToken ct)
        {
                var userInfoDto = GetUserInfoDto();

                var pageResult = await _taskItemService.GetAllAsync(filters, userInfoDto, ct);

                Response.Headers["X-Paging-TotalCount"] = pageResult.TotalCount.ToString();
                Response.Headers["X-Paging-Page"] = pageResult.Page.ToString();
                Response.Headers["X-Paging-Take"] = pageResult.Take.ToString();

                return Ok(pageResult.Items);
        }

        [HttpGet("{id}", Name = "GetTaskByIdAsync")]
        public async Task<ActionResult<TaskItemDto>> GetTaskItemByIdAsync(int id, CancellationToken ct)
        {
                var userInfoDto = GetUserInfoDto();

                var taskItemDto = await _taskItemService.GetByIdAsync(id, userInfoDto, ct);

                return Ok(taskItemDto);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskItemAsync([FromBody] CreateTaskItemDto createDto, CancellationToken ct)
        {
                var userInfoDto = GetUserInfoDto();

                var result = await _taskItemService.AddAsync(createDto, userInfoDto, ct);
                return CreatedAtRoute("GetTaskByIdAsync", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItemAsync(int id, [FromBody] UpdateTaskItemDto updateDto, CancellationToken ct)
        {
                var userInfoDto = GetUserInfoDto();

                await _taskItemService.UpdateAsync(id, updateDto, userInfoDto, ct);
                return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItemAsync(int id, CancellationToken ct)
        {
                var userInfoDto = GetUserInfoDto();

                await _taskItemService.DeleteAsync(id, userInfoDto, ct);
                return NoContent();
        }
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Unauthorized access");
        }

        private bool IsUserAdmin()
        {
            return User.IsInRole(Roles.Admin);
        }

        private UserInfoDto GetUserInfoDto()
        {
            return new UserInfoDto
            {
                UserId = GetUserId(),
                IsAdmin = IsUserAdmin()

            };
        }

    }
}
