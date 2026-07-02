namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity
{
    public class UserCreationFailedException : IdentityOperationFailedException
    {
        public string Email { get; }
        public override string Title => "User creation failed";
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public UserCreationFailedException(string email, IEnumerable<string> errors) : base("User creation failed", errors)
        {
            Email = email;
        }
    }
}
