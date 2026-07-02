using Microsoft.AspNetCore.Identity;

namespace TaskManagerAPI.Models.Identity
{
    public class TaskManagerUser : IdentityUser
    {
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}