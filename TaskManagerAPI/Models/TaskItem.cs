using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string UserId { get; set; } = string.Empty;
        public TaskManagerUser? User { get; set; }

        public int StatusId { get; set; }
        public TaskItemStatus? Status { get; set; }
    }
}