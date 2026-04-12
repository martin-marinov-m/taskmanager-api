using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models.Dtos.TaskItemStatusDtos
{
    public class TaskItemStatusDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
