namespace TaskManagerAPI.Models.Paging
{
    public class TaskManagerPageResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }
        public int Take { get; set; }

        public int TotalCount { get; set; }
    }
}