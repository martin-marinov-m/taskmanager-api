namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity
{
    public class RoleCreationFailedException : IdentityOperationFailedException
    {
        public string Role { get; }
        public override string Title => "Role creation failed";
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public RoleCreationFailedException(string role, IEnumerable<string> errors) : base("Role creation failed", errors)
        {
            Role = role;
        }
    }
}