namespace TaskManagerAPI.Models.Dtos.TaskItemDtos
{
    public class CreateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int StatusId { get; set; }
    }
}
