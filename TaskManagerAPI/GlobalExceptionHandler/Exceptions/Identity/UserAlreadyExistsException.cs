namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity
{
    public sealed class UserAlreadyExistsException : TaskManagerException
    {
        public string Email { get; }
        public override string Title => "User already exists";
        public override int StatusCode => StatusCodes.Status409Conflict;

        public UserAlreadyExistsException(string email) : base("User already exists")
        {
            Email = email;
        }
    }
}
