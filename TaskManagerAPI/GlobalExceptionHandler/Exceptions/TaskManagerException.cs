namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions
{
    public abstract class TaskManagerException : Exception
    {
        public virtual string Title => "Internal server error";
        public virtual int StatusCode => StatusCodes.Status500InternalServerError;
        public virtual string Type => $"https://httpstatuses.com/{StatusCode}";

        public TaskManagerException(string message) : base(message)
        {
        }
    }
}
