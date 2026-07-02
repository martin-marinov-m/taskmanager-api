namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business
{
    public class UnauthorizedException : TaskManagerException
    {
        public override string Title => "Unauthorized";
        public override int StatusCode => StatusCodes.Status401Unauthorized;

        public UnauthorizedException(string message = "Unauthorized access") : base(message)
        {
        }
    }
}