namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Server
{
    public class InvalidConfigurationException : TaskManagerException
    {
        public string SourceName { get; }
        public override string Title => "Invalid or missing configuration";
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public InvalidConfigurationException(string sourceName) : base($"Invalid configuration: {sourceName}")
        {
            SourceName = sourceName;
        }
    }
}