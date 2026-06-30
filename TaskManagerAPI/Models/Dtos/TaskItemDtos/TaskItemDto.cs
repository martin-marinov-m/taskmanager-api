using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;

namespace TaskManagerAPI.Models.Dtos.TaskItemDtos
{
    public class TaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskItemStatusDto? Status { get; set; }

    }
}
