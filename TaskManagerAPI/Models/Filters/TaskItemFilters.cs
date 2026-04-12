namespace TaskManagerAPI.Models.Filters
{
    public class TaskItemFilters
    {
        //Paging
        public int Page { get; set; } = 1;
        public int Take { get; set; } = 10;

        //Search
        public string? Title { get; set; }

    }
}
