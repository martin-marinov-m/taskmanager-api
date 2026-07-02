namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business
{
    public class NotFoundException : TaskManagerException
    {
        public string ResourceName { get; }
        public string ResourceValue { get; }
        public override string Title => "Resource not found";
        public override int StatusCode => StatusCodes.Status404NotFound;

        public NotFoundException(string resourceName, string resourceValue) : base($"{resourceName} not found")
        {
            ResourceName = resourceName;
            ResourceValue = resourceValue;
        }
    }
}
