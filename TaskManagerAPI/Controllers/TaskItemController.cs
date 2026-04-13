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

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Unauthorize access");
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
