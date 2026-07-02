namespace TaskManagerAPI.Models
{
    public class TaskItemStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}