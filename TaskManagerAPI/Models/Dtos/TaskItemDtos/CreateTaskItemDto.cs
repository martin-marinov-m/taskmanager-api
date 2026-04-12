using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models.Dtos.TaskItemDtos
{
    public class CreateTaskItemDto
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int StatusId { get; set; }
    }
}
