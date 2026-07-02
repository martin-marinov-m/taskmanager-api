namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity
{
    public class RoleAssignmentFailedException : IdentityOperationFailedException
    {
        public string Email { get; }
        public string Role { get; }
        public override string Title => "Role assignment failed";
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public RoleAssignmentFailedException(string email, string role, IEnumerable<string> errors) : base("Role assignment failed", errors)
        {
            Email = email;
            Role = role;
        }
    }
}