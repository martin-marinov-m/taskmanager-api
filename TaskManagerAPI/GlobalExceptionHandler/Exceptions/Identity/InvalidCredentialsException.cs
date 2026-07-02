namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity
{
    public class InvalidCredentialsException : TaskManagerException
    {
        public override string Title => "Authentication failed";
        public override int StatusCode => StatusCodes.Status401Unauthorized;

        public InvalidCredentialsException() : base("Invalid email or password")
        {
        }
    }
}
