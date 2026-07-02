namespace TaskManagerAPI.Models.Dtos.TaskItemDtos
{
    public class UpdateTaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int StatusId { get; set; }
    }
}