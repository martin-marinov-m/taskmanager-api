using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;

namespace TaskManagerAPI.Models.Dtos.TaskItemDtos
{
    public class TaskItemDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]

        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public TaskItemStatusDto? Status { get; set; }

    }
}
