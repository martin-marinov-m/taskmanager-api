namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business
{
    public class ForbiddenOperationException : TaskManagerException
    {
        public string? ResourceName { get; }

        public string? ResourceValue { get; }
        public override string Title => "Forbidden operation";
        public override int StatusCode => StatusCodes.Status403Forbidden;

        public ForbiddenOperationException(string message, string? resourceName = null, string? resourceValue = null) : base(message)
        {
            ResourceName = resourceName;
            ResourceValue = resourceValue;
        }
    }
}