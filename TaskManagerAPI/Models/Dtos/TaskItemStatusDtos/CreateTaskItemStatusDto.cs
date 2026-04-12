using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models.Dtos
{
    public class CreateTaskItemStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
