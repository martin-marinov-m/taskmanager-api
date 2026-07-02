namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity
{
    public abstract class IdentityOperationFailedException : TaskManagerException
    {
        public IReadOnlyCollection<string> Errors { get; }

        protected IdentityOperationFailedException(string message, IEnumerable<string> errors) : base(message)
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}